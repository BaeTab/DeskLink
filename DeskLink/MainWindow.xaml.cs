using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System;
using System.Diagnostics;
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
			try
			{
				await _vm.InitializeAsync();
				_shortcut.RegisterQuickOpen(this, "Alt+Space");
				Title = $"DeskLink - {_vm.Items.Count} links";
				SearchBox?.Focus();
			}
			catch (Exception ex)
			{
				ShowError("초기화 실패", ex);
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e) => _shortcut.UnregisterQuickOpen(this);

		// Ctrl+F: 검색창 포커스 / Esc: 검색어 지우기
		private void OnWindowKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				SearchBox?.Focus();
				SearchBox?.SelectAll();
				e.Handled = true;
			}
			else if (e.Key == Key.Escape && SearchBox?.IsKeyboardFocusWithin == true && !string.IsNullOrEmpty(_vm.Query))
			{
				_vm.Query = string.Empty;
				e.Handled = true;
			}
		}

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
			catch (Exception ex) { ShowError("헬스체크 실패", ex); }
		}

		private async void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e) => await _vm.RunSelectedAsync();

		private static LinkItem? ItemOf(object sender) => (sender as FrameworkElement)?.DataContext as LinkItem;

		// 타일: 단일 클릭 선택, 더블 클릭 실행
		private async void OnTileMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (ItemOf(sender) is { } item)
			{
				_vm.Selected = item;
				if (e.ClickCount == 2) await _vm.RunSelectedAsync();
			}
		}

		private async void OnTileRunClick(object sender, RoutedEventArgs e)
		{
			if (ItemOf(sender) is { } item) { _vm.Selected = item; await _vm.RunSelectedAsync(); }
		}

		private void OnTileEditClick(object sender, RoutedEventArgs e)
		{
			if (ItemOf(sender) is { } item) _vm.Selected = item;
		}

		private async void OnTileFavoriteClick(object sender, RoutedEventArgs e)
		{
			if (ItemOf(sender) is { } item) { _vm.Selected = item; await _vm.ToggleFavoriteAsync(); }
		}

		private async void OnTileHealthCheckClick(object sender, RoutedEventArgs e)
		{
			if (ItemOf(sender) is { } item) { _vm.Selected = item; await _vm.HealthCheckSelectedAsync(); }
		}

		private void OnTileCopyTargetClick(object sender, RoutedEventArgs e)
		{
			if (ItemOf(sender) is { } item && !string.IsNullOrEmpty(item.Target))
			{
				try { System.Windows.Clipboard.SetText(item.Target); _vm.StatusText = $"대상 복사됨: {item.Target}"; }
				catch (Exception ex) { ShowError("복사 실패", ex); }
			}
		}

		private void OnTileOpenLocationClick(object sender, RoutedEventArgs e)
		{
			if (ItemOf(sender) is not { } item) return;
			try { OpenLocation(item); }
			catch (Exception ex) { ShowError("위치 열기 실패", ex); }
		}

		private void OpenLocation(LinkItem item)
		{
			switch (item.Type)
			{
				case LinkType.File:
				case LinkType.Exe:
				case LinkType.Rdp:
					if (File.Exists(item.Target))
						Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{item.Target}\"") { UseShellExecute = true });
					else _vm.StatusText = "파일을 찾을 수 없습니다";
					break;
				case LinkType.Folder:
					if (Directory.Exists(item.Target))
						Process.Start(new ProcessStartInfo("explorer.exe", $"\"{item.Target}\"") { UseShellExecute = true });
					else _vm.StatusText = "폴더를 찾을 수 없습니다";
					break;
				default:
					_vm.StatusText = "이 항목은 파일 위치를 열 수 없습니다";
					break;
			}
		}

		private async void OnTileDeleteClick(object sender, RoutedEventArgs e)
		{
			if (ItemOf(sender) is { } item)
			{
				_vm.Selected = item;
				if (Confirm($"'{item.Name}' 항목을 삭제할까요?"))
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
			catch (Exception ex) { ShowError("드롭 실패", ex); }
		}

		private async void OnImportClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*" };
				if (dlg.ShowDialog() == true) await _vm.ImportJsonAsync(dlg.FileName);
			}
			catch (Exception ex) { ShowError("가져오기 실패", ex); }
		}

		private async void OnExportClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var dlg = new Microsoft.Win32.SaveFileDialog { Filter = "JSON files (*.json)|*.json", FileName = $"DeskLink_Export_{DateTime.Now:yyyyMMdd}.json" };
				if (dlg.ShowDialog() == true) await _vm.ExportJsonAsync(dlg.FileName);
			}
			catch (Exception ex) { ShowError("내보내기 실패", ex); }
		}

		private async void OnImportCsvClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*" };
				if (dlg.ShowDialog() == true)
				{
					await new CsvService(_repo).ImportCsvAsync(dlg.FileName);
					await _vm.LoadCategoriesAsync();
					await _vm.RefreshAsync();
					_vm.StatusText = "CSV 가져오기 완료";
				}
			}
			catch (Exception ex) { ShowError("CSV 가져오기 실패", ex); }
		}

		private async void OnExportCsvClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var dlg = new Microsoft.Win32.SaveFileDialog { Filter = "CSV files (*.csv)|*.csv", FileName = $"DeskLink_Export_{DateTime.Now:yyyyMMdd}.csv" };
				if (dlg.ShowDialog() == true)
				{
					await new CsvService(_repo).ExportCsvAsync(dlg.FileName);
					_vm.StatusText = $"CSV 내보내기 완료: {dlg.FileName}";
				}
			}
			catch (Exception ex) { ShowError("CSV 내보내기 실패", ex); }
		}

		private void OnNewLinkClick(object sender, RoutedEventArgs e)
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
			_vm.StatusText = "새 링크를 편집한 뒤 저장을 누르세요.";
		}

		private async void OnSaveClick(object sender, RoutedEventArgs e)
		{
			if (_vm?.Selected == null) { ShowWarn("선택된 항목이 없습니다."); return; }
			var selected = _vm.Selected;
			if (string.IsNullOrWhiteSpace(selected.Name)) { ShowWarn("이름을 입력하세요."); return; }
			if (string.IsNullOrWhiteSpace(selected.Target)) { ShowWarn("대상을 입력하세요."); return; }

			try
			{
				if (selected.Id == Guid.Empty)
				{
					selected.Id = Guid.NewGuid();
					selected.CreatedAt = DateTime.UtcNow;
					selected.UpdatedAt = DateTime.UtcNow;
					await _vm.NewLinkAsync(selected);
				}
				else
				{
					selected.UpdatedAt = DateTime.UtcNow;
					await _vm.UpdateLinkAsync(selected);
				}
			}
			catch (Exception ex) { ShowError("저장 실패", ex); }
		}

		private async void OnDeleteClick(object sender, RoutedEventArgs e)
		{
			if (_vm.Selected == null) { ShowWarn("선택된 항목이 없습니다."); return; }
			if (!Confirm($"'{_vm.Selected.Name}' 항목을 삭제할까요?")) return;
			try { await _vm.DeleteSelectedAsync(); }
			catch (Exception ex) { ShowError("삭제 실패", ex); }
		}

		private void OnColorPickClick(object sender, RoutedEventArgs e)
		{
			if (_vm?.Selected == null) return;
			try
			{
				var currentColor = System.Windows.Media.Colors.Gray;
				if (!string.IsNullOrWhiteSpace(_vm.Selected.ColorHex))
				{
					try { currentColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(_vm.Selected.ColorHex); }
					catch { /* 잘못된 hex 는 기본 회색 유지 */ }
				}

				using var colorDialog = new System.Windows.Forms.ColorDialog
				{
					Color = System.Drawing.Color.FromArgb(currentColor.A, currentColor.R, currentColor.G, currentColor.B),
					FullOpen = true,
					AnyColor = true
				};

				if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					var color = colorDialog.Color;
					_vm.Selected.ColorHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
					_vm.StatusText = $"색상 변경: {_vm.Selected.ColorHex}";
				}
			}
			catch (Exception ex) { ShowError("색상 선택 실패", ex); }
		}

		private static bool Confirm(string msg)
			=> System.Windows.MessageBox.Show(msg, "확인", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

		private static void ShowWarn(string msg)
			=> System.Windows.MessageBox.Show(msg, "알림", MessageBoxButton.OK, MessageBoxImage.Warning);

		private void ShowError(string title, Exception ex)
		{
			if (_vm != null) _vm.StatusText = $"{title}: {ex.Message}";
			System.Windows.MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
