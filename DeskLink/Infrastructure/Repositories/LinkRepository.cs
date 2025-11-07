using DeskLink.Core.Abstractions;
using DeskLink.Core.Models;
using DeskLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeskLink.Infrastructure.Repositories
{
	/// <summary>
	/// EF Core 기반 링크 저장소 구현체
	/// </summary>
	public class LinkRepository : ILinkRepository
	{
		private readonly DeskLinkDbContext _db;
		public LinkRepository(DeskLinkDbContext db) { _db = db; }

		public async Task<LinkItem?> GetAsync(Guid id, CancellationToken ct = default)
		=> await _db.Links.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

		public async Task<IReadOnlyList<LinkItem>> SearchAsync(string? query, CancellationToken ct = default)
		{
			// 간단한 Contains 검색 (대소문자 구분 없음)
			var q = _db.Links.AsNoTracking().AsQueryable();
			if (!string.IsNullOrWhiteSpace(query))
			{
				var pattern = query.ToLower();
				q = q.Where(x => x.Name.ToLower().Contains(pattern) || (x.Tags ?? "").ToLower().Contains(pattern) || (x.Category ?? "").ToLower().Contains(pattern));
			}
			return await q.OrderBy(x => x.Name).Take(2000).ToListAsync(ct);
		}

		public async Task AddAsync(LinkItem item, CancellationToken ct = default)
		{
			if (item.Id == Guid.Empty) item.Id = Guid.NewGuid();
			var now = DateTime.UtcNow;
			item.CreatedAt = now;
			item.UpdatedAt = now;
			await _db.Links.AddAsync(item, ct);
			await _db.SaveChangesAsync(ct);
		}

		public async Task UpdateAsync(LinkItem item, CancellationToken ct = default)
		{
			// AsNoTracking으로 조회된 엔티티의 추적 충돌 방지: 기존 추적 중단 후 업데이트
			var tracked = _db.ChangeTracker.Entries<LinkItem>().FirstOrDefault(e => e.Entity.Id == item.Id);
			if (tracked != null)
			{
				_db.Entry(tracked.Entity).State = EntityState.Detached;
			}

			item.UpdatedAt = DateTime.UtcNow;
			_db.Links.Update(item);
			await _db.SaveChangesAsync(ct);
		}

		public async Task DeleteAsync(Guid id, CancellationToken ct = default)
		{
			var entity = await _db.Links.FirstOrDefaultAsync(x => x.Id == id, ct);
			if (entity != null)
			{
				_db.Links.Remove(entity);
				await _db.SaveChangesAsync(ct);
			}
		}
	}
}
