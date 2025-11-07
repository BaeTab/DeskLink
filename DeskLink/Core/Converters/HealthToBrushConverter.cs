using System;
using System.Globalization;
using System.Windows.Data;

namespace DeskLink.Core.Converters
{
	/// <summary>
	/// 링크 건강 상태를 색상 브러시로 변환하는 컨버터
	/// Unknown=Gray, Ok=Green, Warning=Orange, Error=Red
	/// </summary>
	public class HealthToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// 값이 열거형이 아닌 경우 Unknown 처리
			var status = value is DeskLink.Core.Models.LinkHealthStatus s ? s : DeskLink.Core.Models.LinkHealthStatus.Unknown;
			return status switch
			{
				DeskLink.Core.Models.LinkHealthStatus.Ok => System.Windows.Media.Brushes.LimeGreen,
				DeskLink.Core.Models.LinkHealthStatus.Warning => System.Windows.Media.Brushes.Orange,
				DeskLink.Core.Models.LinkHealthStatus.Error => System.Windows.Media.Brushes.IndianRed,
				_ => System.Windows.Media.Brushes.DimGray
			};
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
