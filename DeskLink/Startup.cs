using System;
using System.IO;
using DeskLink.Infrastructure.Data;
using DeskLink.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DeskLink
{
	/// <summary>
	/// 간단한 조립/초기화 헬퍼 (DI 없이 최소한의 컨텍스트/리포지토리 구성)
	/// </summary>
	public static class Startup
	{
		// SQLite 파일 경로 계산
		public static string GetDbPath()
		{
			var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DeskLink");
			Directory.CreateDirectory(dir);
			return Path.Combine(dir, "desklink.db");
		}

		// DbContextOptions 생성
		public static DbContextOptions<DeskLinkDbContext> CreateDbOptions()
		{
			var builder = new DbContextOptionsBuilder<DeskLinkDbContext>();
			var connectionString = new SqliteConnectionStringBuilder
			{
				DataSource = GetDbPath(),
				Cache = SqliteCacheMode.Default,
				Mode = SqliteOpenMode.ReadWriteCreate
			}.ToString();
			builder.UseSqlite(connectionString);
			return builder.Options;
		}

		// 리포지토리 생성 팩토리
		public static LinkRepository CreateLinkRepository()
		{
			var db = new DeskLinkDbContext(CreateDbOptions());
			// 마이그레이션 없이 스키마 생성 (초기 개발 편의)
			db.Database.EnsureCreated();
			return new LinkRepository(db);
		}
	}
}
