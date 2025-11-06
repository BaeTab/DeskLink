using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DeskLink.Core.Converters
{
 public class InverseBoolConverter : IValueConverter
 {
 public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
 {
 if (value is bool b) return !b;
 return false;
 }

 public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
 {
 if (value is bool b) return !b;
 return false;
 }
 }

 public class InverseBoolToVisConverter : IValueConverter
 {
 public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
 {
 if (value == null) return Visibility.Visible;
 return Visibility.Collapsed;
 }

 public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
 {
 throw new NotSupportedException();
 }
 }
}
