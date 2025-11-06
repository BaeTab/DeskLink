using System.Threading.Tasks;
using DeskLink.Core.Settings;

namespace DeskLink.Core.Abstractions
{
 /// <summary>
 /// 앱 설정 로드/저장 서비스 인터페이스
 /// </summary>
 public interface ISettingsService
 {
 AppSettings Current { get; }
 Task LoadAsync();
 Task SaveAsync();
 }
}
