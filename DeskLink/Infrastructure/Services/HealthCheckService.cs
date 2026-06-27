using System;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DeskLink.Core.Abstractions;
using DeskLink.Core.Models;

namespace DeskLink.Infrastructure.Services
{
	/// <summary>
	/// HTTP/파일/폴더 기본 헬스체크 구현.
	/// URL 은 HEAD 요청을 먼저 시도하고, 서버가 HEAD 를 거부하면(405/501) GET 으로 폴백한다.
	/// </summary>
	public class HealthCheckService : IHealthCheckService
	{
		private static readonly HttpClient Http = CreateClient();

		private static HttpClient CreateClient()
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = true };
			var client = new HttpClient(handler);
			// 일부 서버는 User-Agent 가 없으면 403 을 반환하므로 기본 UA 지정
			client.DefaultRequestHeaders.UserAgent.ParseAdd("DeskLink/1.0 (+health-check)");
			return client;
		}

		public async Task<LinkHealthStatus> CheckAsync(LinkItem item, int timeoutMs = 3000, CancellationToken ct = default)
		{
			try
			{
				switch (item.Type)
				{
					case LinkType.Url:
						return await CheckUrlAsync(item.Target, timeoutMs, ct);
					case LinkType.File:
					case LinkType.Exe:
					case LinkType.Rdp:
						return File.Exists(item.Target) ? LinkHealthStatus.Ok : LinkHealthStatus.Error;
					case LinkType.Folder:
						return Directory.Exists(item.Target) ? LinkHealthStatus.Ok : LinkHealthStatus.Error;
					case LinkType.Ssh:
					case LinkType.Custom:
					default:
						return LinkHealthStatus.Unknown;
				}
			}
			catch (OperationCanceledException) when (ct.IsCancellationRequested)
			{
				throw;
			}
			catch
			{
				return LinkHealthStatus.Error;
			}
		}

		private static async Task<LinkHealthStatus> CheckUrlAsync(string target, int timeoutMs, CancellationToken ct)
		{
			// http/https 가 아니면 도달성 판단 불가
			if (!Uri.TryCreate(target, UriKind.Absolute, out var uri) ||
				(uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
			{
				return LinkHealthStatus.Unknown;
			}

			using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
			cts.CancelAfter(timeoutMs);

			try
			{
				var status = await SendAsync(HttpMethod.Head, uri, cts.Token);
				// HEAD 미지원(405 Method Not Allowed / 501 Not Implemented) 시 GET 으로 재시도
				if (status == (LinkHealthStatus)(-1))
					status = await SendAsync(HttpMethod.Get, uri, cts.Token);
				return status == (LinkHealthStatus)(-1) ? LinkHealthStatus.Warning : status;
			}
			catch (OperationCanceledException) when (ct.IsCancellationRequested)
			{
				throw; // 외부 취소는 호출자에게 전파
			}
			catch (OperationCanceledException)
			{
				return LinkHealthStatus.Warning; // 타임아웃: 도달은 가능하나 느림
			}
		}

		// 결과: Ok/Warning, 혹은 "HEAD 미지원" 신호로 (LinkHealthStatus)(-1) 반환
		private static async Task<LinkHealthStatus> SendAsync(HttpMethod method, Uri uri, CancellationToken token)
		{
			using var req = new HttpRequestMessage(method, uri);
			using var resp = await Http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, token);
			var code = (int)resp.StatusCode;
			if (code == 405 || code == 501) return (LinkHealthStatus)(-1); // HEAD 미지원 신호
			if (code >= 200 && code < 400) return LinkHealthStatus.Ok;
			return LinkHealthStatus.Warning;
		}
	}
}
