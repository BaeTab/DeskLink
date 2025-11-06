using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DeskLink.Core.Models;
using DeskLink.Core.Abstractions;

namespace DeskLink.Core.ViewModels
{
 [POCOViewModel]
 public class MainVm
 {
 private readonly ILinkRepository _repo;
 private readonly ILinkLauncher _launcher;
 private readonly ISettingsService _settings;
 private readonly IHealthCheckService _health;
 private readonly ILocalStateService _local;

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

 protected MainVm() { }
 
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

 protected void OnQueryChanged()
 {
 // UI 스레드에서 비동기 실행
 Dispatcher.CurrentDispatcher.InvokeAsync(async () => await RefreshAsync());
 }

 protected void OnCategoryFilterChanged()
 {
 // UI 스레드에서 비동기 실행
 Dispatcher.CurrentDispatcher.InvokeAsync(async () => await RefreshAsync());
 }

 protected void OnSortByChanged()
 {
 // UI 스레드에서 비동기 실행
 Dispatcher.CurrentDispatcher.InvokeAsync(async () => await RefreshAsync());
 }

 public async Task InitializeAsync()
 {
 await _settings.LoadAsync();
 await _local.InitializeAsync();
 await LoadCategoriesAsync();
 await RefreshAsync();
 UpdateStatusCount();
 }
 
 private void UpdateStatusCount() => StatusText = $"총 {Items.Count}개 링크";

 public async Task LoadCategoriesAsync()
 {
 var all = await _repo.SearchAsync(null);
 var cats = all.Select(x => x.Category).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
 
 // UI 스레드에서 컬렉션 업데이트
 await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
 {
 Categories.Clear();
 Categories.Add("All");
 foreach (var c in cats) Categories.Add(c!);
 if (string.IsNullOrWhiteSpace(CategoryFilter)) CategoryFilter = "All";
 });
 }

 public async Task RefreshAsync()
 {
 try
 {
 var list = await _repo.SearchAsync(Query);
 
 // 카테고리 필터 적용
 if (!string.IsNullOrWhiteSpace(CategoryFilter) && CategoryFilter != "All")
 {
 list = list.Where(x => x.Category?.Equals(CategoryFilter, StringComparison.OrdinalIgnoreCase) == true).ToList();
 }
 
 // 정렬
 list = SortBy switch
 {
 "Recent" => list.OrderByDescending(x => x.UpdatedAt).ToList(),
 "Frequency" => list.OrderByDescending(x => _local.GetFrequency(x.Id)).ThenBy(x => x.Name).ToList(),
 _ => list.OrderBy(x => x.Name).ToList()
 };
 
 // UI 스레드에서 컬렉션 업데이트
 await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
 {
 Items.Clear();
 foreach (var i in list) Items.Add(i);
 UpdateStatusCount();
 });
 }
 catch (Exception ex)
 {
 StatusText = $"오류: {ex.Message}";
 }
 }

 public async Task RunSelectedAsync()
 {
 if (Selected == null) return;
 try
 {
 await _local.RegisterUseAsync(Selected.Id);
 await _launcher.LaunchAsync(Selected);
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
 await _local.ToggleFavoriteAsync(Selected.Id);
 StatusText = "즐겨찾기 토글 완료";
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
 await RefreshAsync();
 StatusText = $"저장: {item.Name}";
 }
 catch (Exception ex)
 {
 StatusText = $"저장 실패: {ex.Message}";
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
 
 // Settings null 체크
 var timeout = _settings?.Current?.HealthCheck?.TimeoutMs ?? 3000;
 
 var status = await _health.CheckAsync(Selected, timeout);
 Selected.Health = status;
 Selected.LastCheckedAt = DateTime.UtcNow;
 await _repo.UpdateAsync(Selected);
 await RefreshAsync();
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
 StatusText = "헬스체크 수행 중...";
 
 // Settings null 체크
 var timeout = _settings?.Current?.HealthCheck?.TimeoutMs ?? 3000;
 
 var itemsCopy = Items.ToList();
 foreach (var item in itemsCopy)
 {
 var status = await _health.CheckAsync(item, timeout);
 item.Health = status;
 item.LastCheckedAt = DateTime.UtcNow;
 await _repo.UpdateAsync(item);
 
 HealthProgress++;
 StatusText = $"헬스체크 중... ({HealthProgress}/{HealthTotal})";
 }
 
 await RefreshAsync();
 StatusText = $"헬스체크 완료: {HealthTotal}개 항목";
 }
 catch (Exception ex)
 {
 StatusText = $"헬스체크 오류: {ex.Message}";
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
 var name = e.GetProperty("name").GetString() ?? string.Empty;
 var target = e.GetProperty("target").GetString() ?? string.Empty;
 var typeStr = e.GetProperty("type").GetString() ?? "Url";
 Enum.TryParse<LinkType>(typeStr, out var type);
 
 await _repo.AddAsync(new LinkItem
 {
 Name = name,
 Target = target,
 Type = type,
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
 }
}
