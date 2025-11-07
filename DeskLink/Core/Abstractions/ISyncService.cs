using System.Threading;
using System.Threading.Tasks;

namespace DeskLink.Core.Abstractions
{
	/// <summary>
	/// 공유 저장소와 동기화 서비스 인터페이스
	/// </summary>
	public interface ISyncService
	{
		Task PullAsync(CancellationToken ct = default);
		Task PushAsync(CancellationToken ct = default);
	}
}
