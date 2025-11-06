using System.Threading;
using System.Threading.Tasks;
using DeskLink.Core.Models;

namespace DeskLink.Core.Abstractions
{
 /// <summary>
 /// 링크 대상 건강상태 점검 서비스 인터페이스
 /// </summary>
 public interface IHealthCheckService
 {
 Task<LinkHealthStatus> CheckAsync(LinkItem item, int timeoutMs =3000, CancellationToken ct = default);
 }
}
