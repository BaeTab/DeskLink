using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeskLink.Core.Abstractions
{
 /// <summary>
 /// DB 비영속 로컬 상태(즐겨찾기/최근/사용빈도) 저장 서비스
 /// </summary>
 public interface ILocalStateService
 {
 Task InitializeAsync();
 bool IsFavorite(Guid id);
 IReadOnlyCollection<Guid> GetFavorites();
 Task ToggleFavoriteAsync(Guid id);
 Task RegisterUseAsync(Guid id);
 int GetFrequency(Guid id);
 IReadOnlyDictionary<Guid,int> GetFrequencyMap();
 }
}
