using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeskLink.Core.Models;

namespace DeskLink.Core.Abstractions
{
 /// <summary>
 /// 링크 저장소 인터페이스 - CRUD 및 검색
 /// </summary>
 public interface ILinkRepository
 {
 Task<LinkItem?> GetAsync(Guid id, CancellationToken ct = default);
 Task<IReadOnlyList<LinkItem>> SearchAsync(string? query, CancellationToken ct = default);
 Task AddAsync(LinkItem item, CancellationToken ct = default);
 Task UpdateAsync(LinkItem item, CancellationToken ct = default);
 Task DeleteAsync(Guid id, CancellationToken ct = default);
 }
}
