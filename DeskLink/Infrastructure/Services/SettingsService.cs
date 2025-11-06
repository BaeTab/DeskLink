using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DeskLink.Core.Abstractions;
using DeskLink.Core.Settings;

namespace DeskLink.Infrastructure.Services
{
 /// <summary>
 /// JSON 파일 기반 설정 관리
 /// </summary>
 public class SettingsService : ISettingsService
 {
 private readonly string _path;
 public AppSettings Current { get; private set; } = new AppSettings();
 public SettingsService()
 {
 var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DeskLink");
 Directory.CreateDirectory(dir);
 _path = Path.Combine(dir, "settings.json");
 }
 public async Task LoadAsync()
 {
 if (File.Exists(_path))
 {
 var json = await File.ReadAllTextAsync(_path);
 var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
 var loaded = JsonSerializer.Deserialize<AppSettings>(json, opt);
 if (loaded != null) Current = loaded;
 }
 }
 public async Task SaveAsync()
 {
 var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
 await File.WriteAllTextAsync(_path, json);
 }
 }
}
