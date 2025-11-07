using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DeskLink.Infrastructure.Services
{
	/// <summary>
	/// 아이콘 서비스 간단 구현 - IconKey 기반 내장 아이콘 반환
	/// </summary>
	public class IconService : Core.Abstractions.IIconService
	{
		public ImageSource? GetIcon(string? key)
		{
			// 간단 예시: key 기반 고정 아이콘 반환 (실제로는 SVG/PNG 리소스 로딩 구현 필요)
			if (string.IsNullOrWhiteSpace(key)) return null;
			// 기본 플레이스홀더 이미지
			return new BitmapImage(new System.Uri("pack://application:,,,/Assets/default_icon.png", System.UriKind.Absolute));
		}
	}
}
