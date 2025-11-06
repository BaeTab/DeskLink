using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DeskLink.Core.Abstractions;
using DeskLink.Core.Models;
using Serilog;

namespace DeskLink.Infrastructure.Services.Sync
{
 /// <summary>
 /// 공유 JSON 파일 기반 동기화 구현 (Pull/Push)
 /// </summary>
 public class FileSyncService : ISyncService
 {
 private readonly ILinkRepository _repo;
 private readonly string _sharedPath;
 public FileSyncService(ILinkRepository repo, string sharedPath)
 {
 _repo = repo; _sharedPath = sharedPath;
 }

 public async Task PullAsync(CancellationToken ct = default)
 {
 if (!File.Exists(_sharedPath)) { Log.Warning("공유 파일 없음: {Path}", _sharedPath); return; }
 var json = await File.ReadAllTextAsync(_sharedPath, ct);
 using var doc = JsonDocument.Parse(json);
 var links = doc.RootElement.GetProperty("links");
 foreach (var e in links.EnumerateArray())
 {
 var id = e.TryGetProperty("id", out var idEl) ? idEl.GetGuid() : Guid.NewGuid();
 var existing = await _repo.GetAsync(id, ct);
 if (existing != null) continue; // 충돌 방지: 로컬 우선
 var name = e.GetProperty("name").GetString() ?? string.Empty;
 var target = e.GetProperty("target").GetString() ?? string.Empty;
 var typeStr = e.GetProperty("type").GetString() ?? "Url";
 Enum.TryParse<LinkType>(typeStr, out var type);
 await _repo.AddAsync(new LinkItem { Id = id, Name = name, Target = target, Type = type, Health = LinkHealthStatus.Unknown, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, ct);
 }
 Log.Information("Pull 완료: {Path}", _sharedPath);
 }

 public async Task PushAsync(CancellationToken ct = default)
 {
 var list = await _repo.SearchAsync(null, ct);
 var payload = new { version = "1.0", updated = DateTime.UtcNow, links = list.Select(x => new { id = x.Id, name = x.Name, type = x.Type.ToString(), target = x.Target }) };
 var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
 await File.WriteAllTextAsync(_sharedPath, json, ct);
 Log.Information("Push 완료: {Path}", _sharedPath);
 }
 }
}
