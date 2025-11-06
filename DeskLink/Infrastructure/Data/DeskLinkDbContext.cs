using DeskLink.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskLink.Infrastructure.Data
{
 /// <summary>
 /// EF Core DbContext - SQLite 로컬 저장소
 /// </summary>
 public class DeskLinkDbContext : DbContext
 {
 public DeskLinkDbContext(DbContextOptions<DeskLinkDbContext> options) : base(options) { }

 // 링크 테이블
 public DbSet<LinkItem> Links => Set<LinkItem>();

 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
 // Links 테이블 구성
 modelBuilder.Entity<LinkItem>(e =>
 {
 e.HasKey(x => x.Id);
 e.Property(x => x.Name).IsRequired();
 e.Property(x => x.Type).IsRequired();
 e.Property(x => x.Target).IsRequired();
 e.Property(x => x.CreatedAt).IsRequired();
 e.Property(x => x.UpdatedAt).IsRequired();
 e.Property(x => x.Health).HasConversion<int>();
 e.Property(x => x.Type).HasConversion<int>();
 e.HasIndex(x => x.Name);
 e.HasIndex(x => x.Tags);
 e.HasIndex(x => x.Category);
 });
 }
 }
}
