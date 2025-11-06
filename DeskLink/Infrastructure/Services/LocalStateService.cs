using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DeskLink.Core.Abstractions;

namespace DeskLink.Infrastructure.Services
{
 /// <summary>
 /// 즐겨찾기/최근/빈도 등 로컬 상태 JSON 파일 관리
 /// </summary>
 public class LocalStateService : ILocalStateService
 {
 private record Model(HashSet<Guid> Favorites, Dictionary<Guid,int> Frequency);
 private Model _state = new(new HashSet<Guid>(), new Dictionary<Guid,int>());
 private readonly string _path;
 public LocalStateService()
 {
 var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DeskLink");
 Directory.CreateDirectory(dir);
 _path = Path.Combine(dir, "state.json");
 }
 public async Task InitializeAsync()
 {
 if (File.Exists(_path))
 {
 var json = await File.ReadAllTextAsync(_path);
 var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
 var tmp = JsonSerializer.Deserialize<Model>(json, opt);
 if (tmp != null) _state = tmp;
 }
 }
 private async Task SaveAsync()
 {
 var json = JsonSerializer.Serialize(_state, new JsonSerializerOptions { WriteIndented = true });
 await File.WriteAllTextAsync(_path, json);
 }
 public bool IsFavorite(Guid id) => _state.Favorites.Contains(id);
 public IReadOnlyCollection<Guid> GetFavorites() => _state.Favorites;
 public async Task ToggleFavoriteAsync(Guid id)
 {
 if (!_state.Favorites.Add(id)) _state.Favorites.Remove(id);
 await SaveAsync();
 }
 public async Task RegisterUseAsync(Guid id)
 {
 if (_state.Frequency.ContainsKey(id)) _state.Frequency[id]++;
 else _state.Frequency[id] =1;
 await SaveAsync();
 }
 public int GetFrequency(Guid id) => _state.Frequency.TryGetValue(id, out var n) ? n :0;
 public IReadOnlyDictionary<Guid,int> GetFrequencyMap() => _state.Frequency;
 }
}
