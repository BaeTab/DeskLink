using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskLink.Core.Abstractions;
using DeskLink.Core.Models;

namespace DeskLink.Infrastructure.Services
{
	/// <summary>
	/// CSV Import/Export 서비스 구현
	/// </summary>
	public class CsvService
	{
		private readonly ILinkRepository _repo;
		public CsvService(ILinkRepository repo) { _repo = repo; }

		public async Task ExportCsvAsync(string path)
		{
			var items = await _repo.SearchAsync(null);
			var sb = new StringBuilder();
			sb.AppendLine("Id,Name,Type,Target,Category,Tags,ColorHex,Health");
			foreach (var item in items)
				sb.AppendLine($"{item.Id},{Escape(item.Name)},{item.Type},{Escape(item.Target)},{Escape(item.Category)},{Escape(item.Tags)},{Escape(item.ColorHex)},{item.Health}");
			await File.WriteAllTextAsync(path, sb.ToString());
		}

		public async Task ImportCsvAsync(string path)
		{
			var lines = await File.ReadAllLinesAsync(path);
			foreach (var line in lines.Skip(1))
			{
				var parts = line.Split(',');
				if (parts.Length < 4) continue;
				var item = new LinkItem
				{
					Id = Guid.TryParse(parts[0], out var id) ? id : Guid.NewGuid(),
					Name = parts[1],
					Type = Enum.TryParse<LinkType>(parts[2], out var type) ? type : LinkType.Url,
					Target = parts[3],
					Category = parts.Length > 4 ? parts[4] : null,
					Tags = parts.Length > 5 ? parts[5] : null,
					ColorHex = parts.Length > 6 ? parts[6] : null,
					Health = parts.Length > 7 && Enum.TryParse<LinkHealthStatus>(parts[7], out var h) ? h : LinkHealthStatus.Unknown,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};
				await _repo.AddAsync(item);
			}
		}

		private string Escape(string? s) => s?.Replace(",", ";") ?? string.Empty;
	}
}
