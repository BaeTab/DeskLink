using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeskLink.Infrastructure.Repositories;
using DeskLink.Core.Models;
using DeskLink.Core.ViewModels;
using DeskLink.Infrastructure.Services;
using DeskLink.Core.Abstractions;
using DeskLink.Views;

namespace DeskLink
{
	public partial class MainWindow : ThemedWindow
	{
		private readonly ILinkRepository _repo;
		private readonly ILinkLauncher _launcher;
		private readonly ISettingsService _settings;
		private readonly IHealthCheckService _health;
		private readonly ILocalStateService _local;
		private readonly IShortcutService _shortcut;
		private readonly MainVm _vm;

		public MainWindow()
		{
			InitializeComponent();
			_repo = Startup.CreateLinkRepository();
			_launcher = new LinkLauncher();
			_settings = new SettingsService();
			_health = new HealthCheckService();
			_local = new LocalStateService();
			_shortcut = new ShortcutService();
			_vm = MainVm.Create(_repo, _launcher, _settings, _health, _local);
			DataContext = _vm;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			_shortcut.QuickOpenRequested += OnQuickOpenRequested;
		}

		private async void OnLoaded(object? sender, RoutedEventArgs e)
		{
			// DB 초기화 - 테스트용: 왼쪽 Shift 누르고 시작하면 DB 리셋
			var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DeskLink", "desklink.db");
			if (File.Exists(dbPath) && Keyboard.IsKeyDown(Key.LeftShift))
			{
				File.Delete(dbPath);
				System.Windows.MessageBox.Show("Database reset! Restarting...", "Reset", MessageBoxButton.OK);
				System.Diagnostics.Process.Start(Environment.ProcessPath!);
				System.Windows.Application.Current.Shutdown();
				return;
			}

			await _vm.InitializeAsync();
			_shortcut.RegisterQuickOpen(this, "Alt+Space");
			Title = $"DeskLink - {_vm.Items.Count} links";
		}

		private void OnUnloaded(object sender, RoutedEventArgs e) => _shortcut.UnregisterQuickOpen(this);

		private async void OnQuickOpenRequested()
		{
			var dlg = new QuickOpenWindow { Owner = this };
			dlg.LoadItems(_vm.Items);
			if (dlg.ShowDialog() == true && dlg.Selected != null)
			{
				_vm.Selected = dlg.Selected;
				await _vm.RunSelectedAsync();
			}
		}

		private async void OnHealthAllClick(object sender, RoutedEventArgs e)
		{
			try { await _vm.HealthCheckAllAsync(); }
			catch (Exception ex) { System.Windows.MessageBox.Show($"Health check failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private async void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e) => await _vm.RunSelectedAsync();

		private async void OnTileClick(object sender, MouseButtonEventArgs e)
		{
			if (sender is FrameworkElement fe && fe.DataContext is LinkItem item) { _vm.Selected = item; await _vm.RunSelectedAsync(); }
		}

		private async void OnTileRunClick(object sender, RoutedEventArgs e)
		{
			if (sender is FrameworkElement fe && fe.DataContext is LinkItem item) { _vm.Selected = item; await _vm.RunSelectedAsync(); }
		}

		private void OnTileEditClick(object sender, RoutedEventArgs e)
		{
			if (sender is FrameworkElement fe && fe.DataContext is LinkItem item) _vm.Selected = item;
		}

		private async void OnTileFavoriteClick(object sender, RoutedEventArgs e)
		{
			if (sender is FrameworkElement fe && fe.DataContext is LinkItem item) { _vm.Selected = item; await _vm.ToggleFavoriteAsync(); }
		}

		private async void OnTileHealthCheckClick(object sender, RoutedEventArgs e)
		{
			if (sender is FrameworkElement fe && fe.DataContext is LinkItem item) { _vm.Selected = item; await _vm.HealthCheckSelectedAsync(); }
		}

		private async void OnTileDeleteClick(object sender, RoutedEventArgs e)
		{
			if (sender is FrameworkElement fe && fe.DataContext is LinkItem item)
			{
				_vm.Selected = item;
				if (System.Windows.MessageBox.Show($"Delete '{item.Name}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					await _vm.DeleteSelectedAsync();
			}
		}

		private async void OnItemsDrop(object sender, System.Windows.DragEventArgs e)
		{
			try
			{
				if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
				{
					var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
					foreach (var f in files)
						await _vm.NewLinkAsync(new LinkItem { Name = Path.GetFileName(f), Target = f, Type = Directory.Exists(f) ? LinkType.Folder : LinkType.File, Category = "파일", ColorHex = "#9C27B0", Health = LinkHealthStatus.Unknown });
				}
				else if (e.Data.GetDataPresent(System.Windows.DataFormats.Text))
				{
					var text = (string)e.Data.GetData(System.Windows.DataFormats.Text);
					if (Uri.TryCreate(text, UriKind.Absolute, out var uri))
						await _vm.NewLinkAsync(new LinkItem { Name = uri.Host, Target = uri.ToString(), Type = LinkType.Url, Category = "웹", ColorHex = "#2196F3", Health = LinkHealthStatus.Unknown });
				}
			}
			catch (Exception ex) { System.Windows.MessageBox.Show($"Drop failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private async void OnImportClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*" };
				if (dlg.ShowDialog() == true) { await _vm.ImportJsonAsync(dlg.FileName); System.Windows.MessageBox.Show("Import completed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information); }
			}
			catch (Exception ex) { System.Windows.MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private async void OnExportClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var dlg = new Microsoft.Win32.SaveFileDialog { Filter = "JSON files (*.json)|*.json", FileName = $"DeskLink_Export_{DateTime.Now:yyyyMMdd}.json" };
				if (dlg.ShowDialog() == true) { await _vm.ExportJsonAsync(dlg.FileName); System.Windows.MessageBox.Show("Export completed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information); }
			}
			catch (Exception ex) { System.Windows.MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private void OnNewLinkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var newLink = new LinkItem
				{
					Id = Guid.Empty,
					Name = "New Link",
					Type = LinkType.Url,
					Target = "https://",
					Category = "기타",
					ColorHex = "#607D8B",
					Tags = "",
					Health = LinkHealthStatus.Unknown,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};

				_vm.Selected = newLink;
				_vm.StatusText = "New link created. Edit and click Save.";
			}
			catch (Exception ex) { System.Windows.MessageBox.Show($"Failed to create new link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private async void OnSaveClick(object sender, RoutedEventArgs e)
		{
			try
			{
				// null 체크
				if (_vm?.Selected == null)
				{
					System.Windows.MessageBox.Show("No item selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Selected 객체를 로컬 변수에 복사 (프록시 객체 문제 방지)
				var selected = _vm.Selected;

				// 필수 필드 검증
				if (string.IsNullOrWhiteSpace(selected.Name))
				{
					System.Windows.MessageBox.Show("Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (string.IsNullOrWhiteSpace(selected.Target))
				{
					System.Windows.MessageBox.Show("Target is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// 저장 작업
				if (selected.Id == Guid.Empty)
				{
					// 새 링크 추가
					// Id가 Empty인 경우 새 Id 생성
					selected.Id = Guid.NewGuid();
					selected.CreatedAt = DateTime.UtcNow;
					selected.UpdatedAt = DateTime.UtcNow;

					await _vm.NewLinkAsync(selected);
					System.Windows.MessageBox.Show($"'{selected.Name}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					// 기존 링크 수정
					selected.UpdatedAt = DateTime.UtcNow;
					await _vm.UpdateLinkAsync(selected);
					System.Windows.MessageBox.Show($"'{selected.Name}' updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (NullReferenceException ex)
			{
				System.Windows.MessageBox.Show($"Null reference error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show($"Save failed: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void OnDeleteClick(object sender, RoutedEventArgs e)
		{
			if (_vm.Selected == null) { System.Windows.MessageBox.Show("No item selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
			try
			{
				if (System.Windows.MessageBox.Show($"Delete '{_vm.Selected.Name}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{ await _vm.DeleteSelectedAsync(); System.Windows.MessageBox.Show("Deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information); }
			}
			catch (Exception ex) { System.Windows.MessageBox.Show($"Delete failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private void OnColorPickClick(object sender, RoutedEventArgs e)
		{
			try
			{
				if (_vm?.Selected == null) return;

				// 현재 색상 파싱
				var currentColor = System.Windows.Media.Colors.Gray;
				if (!string.IsNullOrWhiteSpace(_vm.Selected.ColorHex))
				{
					try
					{
						currentColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(_vm.Selected.ColorHex);
					}
					catch { }
				}

				// Color Picker 다이얼로그 표시
				var colorDialog = new System.Windows.Forms.ColorDialog
				{
					Color = System.Drawing.Color.FromArgb(currentColor.A, currentColor.R, currentColor.G, currentColor.B),
					FullOpen = true,
					AnyColor = true
				};

				if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					var color = colorDialog.Color;
					_vm.Selected.ColorHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
					_vm.StatusText = $"Color changed to {_vm.Selected.ColorHex}";
				}
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show($"Color pick failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
