using System.Net.Sockets;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DeskLink.Core.Abstractions;
using DeskLink.Core.Models;

namespace DeskLink.Infrastructure.Services
{
	/// <summary>
	/// HTTP/파일/폴더/포트 기본 헬스체크 구현
	/// </summary>
	public class HealthCheckService : IHealthCheckService
	{
		private static readonly HttpClient Http = new HttpClient();
		public async Task<LinkHealthStatus> CheckAsync(LinkItem item, int timeoutMs = 3000, CancellationToken ct = default)
		{
			try
			{
				switch (item.Type)
				{
					case LinkType.Url:
						using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct))
						{
							cts.CancelAfter(timeoutMs);
							var resp = await Http.SendAsync(new HttpRequestMessage(HttpMethod.Head, item.Target), cts.Token);
							if ((int)resp.StatusCode >= 200 && (int)resp.StatusCode < 400) return LinkHealthStatus.Ok;
							return LinkHealthStatus.Warning;
						}
					case LinkType.File:
						return File.Exists(item.Target) ? LinkHealthStatus.Ok : LinkHealthStatus.Error;
					case LinkType.Folder:
						return Directory.Exists(item.Target) ? LinkHealthStatus.Ok : LinkHealthStatus.Error;
					case LinkType.Exe:
						return File.Exists(item.Target) ? LinkHealthStatus.Ok : LinkHealthStatus.Error;
					case LinkType.Rdp:
						return File.Exists(item.Target) ? LinkHealthStatus.Ok : LinkHealthStatus.Error;
					case LinkType.Ssh:
					case LinkType.Custom:
						return LinkHealthStatus.Unknown;
					default:
						return LinkHealthStatus.Unknown;
				}
			}
			catch
			{
				return LinkHealthStatus.Error;
			}
		}
	}
}
