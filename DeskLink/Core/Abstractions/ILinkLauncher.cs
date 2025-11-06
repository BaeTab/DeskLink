using System.Threading;
using System.Threading.Tasks;
using DeskLink.Core.Models;

namespace DeskLink.Core.Abstractions
{
 /// <summary>
 /// 링크 실행 서비스 인터페이스
 /// </summary>
 public interface ILinkLauncher
 {
 Task LaunchAsync(LinkItem item, CancellationToken ct = default);
 }
}
