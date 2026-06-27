using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DeskLink.Core.Models;
using DeskLink.Core.Abstractions;
using Serilog;

namespace DeskLink.Core.ViewModels
{
	[POCOViewModel]
	public class MainVm
	{
		/// <summary>즐겨찾기 가상 카테고리 표시 이름.</summary>
		public const string FavoritesFilter = "★ 즐겨찾기";
		public const string AllFilter = "All";

		private readonly ILinkRepository _repo;
		private readonly ILinkLauncher _launcher;
		private readonly ISettingsService _settings;
		private readonly IHealthCheckService _health;
		private readonly ILocalStateService _local;

		private CancellationTokenSource? _searchCts;

		public virtual string? Query { get; set; }
		public virtual string? CategoryFilter { get; set; }
		public virtual string SortBy { get; set; } = "Name";
		public virtual ObservableCollection<LinkItem> Items { get; set; } = new();
		public virtual ObservableCollection<string> Categories { get; set; } = new();
		public virtual LinkItem? Selected { get; set; }
		public virtual bool IsTileView { get; set; } = true;

		public virtual string StatusText { get; set; } = "준비";
		public virtual bool IsHealthChecking { get; set; }
		public virtual int HealthProgress { get; set; }
		public virtual int HealthTotal { get; set; }
		public virtual string SyncState { get; set; } = string.Empty;
		/// <summary>표시할 항목이 없을 때 안내 표시 여부.</summary>
		public virtual bool IsEmpty { get; set; }

		protected MainVm()
		{
			_repo = null!;
			_launcher = null!;
			_settings = null!;
			_health = null!;
			_local = null!;
		}

		protected MainVm(ILinkRepository repo, ILinkLauncher launcher, ISettingsService settings, IHealthCheckService health, ILocalStateService local)
		{
			_repo = repo;
			_launcher = launcher;
			_settings = settings;
			_health = health;
			_local = local;
		}

		public static MainVm Create(ILinkRepository repo, ILinkLauncher launcher, ISettingsService settings, IHealthCheckService health, ILocalStateService local)
		=> ViewModelSource.Create(() => new MainVm(repo, launcher, settings, health, local));

		// 항상 UI 스레드 디스패처를 사용 (백그라운드에서 호출돼도 안전)
		private static Dispatcher Ui =>
			System.Windows.Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

		// 검색어 변경 시 250ms 디바운스 후 한 번만 조회
		protected async void OnQueryChanged()
		{
			var previous = _searchCts;
			previous?.Cancel();
			previous?.Dispose();
			var cts = _searchCts = new CancellationTokenSource();
			try { await Task.Delay(250, cts.Token); }
			catch (TaskCanceledException) { return; }
			if (!cts.IsCancellationRequested) await RefreshAsync();
		}

		protected async void OnCategoryFilterChanged() => await RefreshAsync();

		protected async void OnSortByChanged() => await RefreshAsync();

		public async Task InitializeAsync()
		{
			await _settings.LoadAsync();
			await _local.InitializeAsync();
			await LoadCategoriesAsync();
			await RefreshAsync();
		}

		private void UpdateStatusCount() => StatusText = $"총 {Items.Count}개 링크";

		public async Task LoadCategoriesAsync()
		{
			var all = await _repo.SearchAsync(null);
			var cats = all.Select(x => x.Category).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();

			await Ui.InvokeAsync(() =>
			{
				var previous = CategoryFilter;
				Categories.Clear();
				Categories.Add(AllFilter);
				Categories.Add(FavoritesFilter);
				foreach (var c in cats) Categories.Add(c!);
				// 이전 선택 유지, 없으면 All
				if (string.IsNullOrWhiteSpace(previous) || !Categories.Contains(previous))
					CategoryFilter = AllFilter;
			});
		}

		public async Task RefreshAsync()
		{
			try
			{
				var list = await _repo.SearchAsync(Query);

				// 카테고리/즐겨찾기 필터 적용
				if (CategoryFilter == FavoritesFilter)
				{
					list = list.Where(x => _local.IsFavorite(x.Id)).ToList();
				}
				else if (!string.IsNullOrWhiteSpace(CategoryFilter) && CategoryFilter != AllFilter)
				{
					list = list.Where(x => x.Category?.Equals(CategoryFilter, StringComparison.OrdinalIgnoreCase) == true).ToList();
				}

				// 정렬 후 즐겨찾기 우선
				IEnumerable<LinkItem> sorted = SortBy switch
				{
					"Recent" => list.OrderByDescending(x => x.UpdatedAt),
					"Frequency" => list.OrderByDescending(x => _local.GetFrequency(x.Id)).ThenBy(x => x.Name),
					_ => list.OrderBy(x => x.Name)
				};
				var final = sorted
					.OrderByDescending(x => _local.IsFavorite(x.Id))
					.ToList();

				// 런타임 표시 플래그(즐겨찾기/사용빈도) 주입
				foreach (var i in final)
				{
					i.IsFavorite = _local.IsFavorite(i.Id);
					i.UseCount = _local.GetFrequency(i.Id);
				}

				await Ui.InvokeAsync(() =>
				{
					Items.Clear();
					foreach (var i in final) Items.Add(i);
					IsEmpty = Items.Count == 0;
					UpdateStatusCount();
				});
			}
			catch (Exception ex)
			{
				Log.Error(ex, "목록 갱신 실패");
				StatusText = $"오류: {ex.Message}";
			}
		}

		public async Task RunSelectedAsync()
		{
			if (Selected == null) return;
			try
			{
				await _launcher.LaunchAsync(Selected);
				await _local.RegisterUseAsync(Selected.Id);
				Selected.UseCount = _local.GetFrequency(Selected.Id);
				StatusText = $"실행: {Selected.Name}";
			}
			catch (Exception ex)
			{
				StatusText = $"실행 실패: {ex.Message}";
			}
		}

		public async Task ToggleFavoriteAsync()
		{
			if (Selected == null) return;
			try
			{
				await _local.ToggleFavoriteAsync(Selected.Id);
				Selected.IsFavorite = _local.IsFavorite(Selected.Id);
				StatusText = Selected.IsFavorite ? $"즐겨찾기 추가: {Selected.Name}" : $"즐겨찾기 해제: {Selected.Name}";
				// 즐겨찾기 필터를 보고 있으면 목록 갱신
				if (CategoryFilter == FavoritesFilter) await RefreshAsync();
			}
			catch (Exception ex)
			{
				StatusText = $"즐겨찾기 실패: {ex.Message}";
			}
		}

		public async Task NewLinkAsync(LinkItem item)
		{
			try
			{
				await _repo.AddAsync(item);
				await LoadCategoriesAsync();
				await RefreshAsync();
				StatusText = $"추가: {item.Name}";
			}
			catch (Exception ex)
			{
				StatusText = $"추가 실패: {ex.Message}";
			}
		}

		public async Task UpdateLinkAsync(LinkItem item)
		{
			try
			{
				item.UpdatedAt = DateTime.UtcNow;
				await _repo.UpdateAsync(item);
				await LoadCategoriesAsync();
				await RefreshAsync();
				StatusText = $"수정: {item.Name}";
			}
			catch (Exception ex)
			{
				StatusText = $"수정 실패: {ex.Message}";
			}
		}

		public async Task DeleteSelectedAsync()
		{
			if (Selected == null) return;
			try
			{
				var name = Selected.Name;
				await _repo.DeleteAsync(Selected.Id);
				Selected = null;
				await LoadCategoriesAsync();
				await RefreshAsync();
				StatusText = $"삭제: {name}";
			}
			catch (Exception ex)
			{
				StatusText = $"삭제 실패: {ex.Message}";
			}
		}

		public async Task HealthCheckSelectedAsync()
		{
			if (Selected == null) return;
			try
			{
				StatusText = $"헬스체크: {Selected.Name}";
				var timeout = _settings?.Current?.HealthCheck?.TimeoutMs ?? 3000;
				var status = await _health.CheckAsync(Selected, timeout);
				Selected.Health = status;
				Selected.LastCheckedAt = DateTime.UtcNow;
				await _repo.UpdateAsync(Selected);
				StatusText = $"헬스체크 완료: {Selected.Name} - {status}";
			}
			catch (Exception ex)
			{
				StatusText = $"헬스체크 실패: {ex.Message}";
			}
		}

		public async Task HealthCheckAllAsync()
		{
			if (Items.Count == 0)
			{
				StatusText = "헬스체크할 항목이 없습니다";
				return;
			}

			try
			{
				IsHealthChecking = true;
				HealthTotal = Items.Count;
				HealthProgress = 0;
				StatusText = "헬스체크 시작...";

				var timeout = _settings?.Current?.HealthCheck?.TimeoutMs ?? 3000;

				var itemsCopy = Items.ToList();
				foreach (var item in itemsCopy)
				{
					var status = await _health.CheckAsync(item, timeout);
					item.Health = status; // INPC 로 점 색이 즉시 갱신
					item.LastCheckedAt = DateTime.UtcNow;
					await _repo.UpdateAsync(item);

					HealthProgress++;
					StatusText = $"헬스체크 중... ({HealthProgress}/{HealthTotal})";
				}

				StatusText = $"헬스체크 완료: {HealthTotal}개 항목";
			}
			catch (Exception ex)
			{
				StatusText = $"헬스체크 실패: {ex.Message}";
			}
			finally
			{
				IsHealthChecking = false;
			}
		}

		public async Task ExportJsonAsync(string path)
		{
			try
			{
				var payload = new { version = "1.0", updated = DateTime.UtcNow, links = Items };
				var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
				await File.WriteAllTextAsync(path, json);
				StatusText = $"내보내기 완료: {path}";
			}
			catch (Exception ex)
			{
				StatusText = $"내보내기 실패: {ex.Message}";
			}
		}

		public async Task ImportJsonAsync(string path)
		{
			try
			{
				var json = await File.ReadAllTextAsync(path);
				using var doc = JsonDocument.Parse(json);
				var links = doc.RootElement.GetProperty("links");

				int count = 0;
				foreach (var e in links.EnumerateArray())
				{
					var name = GetStringOrDefault(e, "name");
					var target = GetStringOrDefault(e, "target");
					if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(target)) continue;
					var typeStr = GetStringOrDefault(e, "type") ?? "Url";
					Enum.TryParse<LinkType>(typeStr, out var type);

					await _repo.AddAsync(new LinkItem
					{
						Name = name!,
						Target = target!,
						Type = type,
						Category = GetStringOrDefault(e, "category"),
						ColorHex = GetStringOrDefault(e, "colorHex"),
						Tags = GetStringOrDefault(e, "tags"),
						Health = LinkHealthStatus.Unknown,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					});
					count++;
				}

				await LoadCategoriesAsync();
				await RefreshAsync();
				StatusText = $"가져오기 완료: {count}개 항목";
			}
			catch (Exception ex)
			{
				StatusText = $"가져오기 실패: {ex.Message}";
			}
		}

		private static string? GetStringOrDefault(JsonElement e, string prop)
			=> e.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
	}
}
