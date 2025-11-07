using System;
using System.Globalization;
using System.Windows.Data;

namespace DeskLink.Core.Converters
{
	public class ColorHexToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var hex = value as string;
			if (string.IsNullOrWhiteSpace(hex)) return System.Windows.Media.Brushes.SlateGray;
			try
			{
				var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
				return new System.Windows.Media.SolidColorBrush(color);
			}
			catch
			{
				return System.Windows.Media.Brushes.SlateGray;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
