using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DeskLink.Core.Converters
{
    /// <summary>
    /// null 값을 Visibility로 변환: null -> Collapsed, not null -> Visible
    /// DevExpress POCO ViewModel 프록시 객체도 정확하게 처리
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
      // null 체크
            if (value == null)
     return Visibility.Collapsed;

        // DevExpress POCO 프록시 객체 처리
            // 프록시 객체는 null이 아니지만 실제 값이 없을 수 있음
          var type = value.GetType();
            if (type.FullName?.Contains("EntityProxyModule") == true || 
      type.FullName?.Contains("Castle.Proxies") == true)
        {
              // 프록시 객체의 실제 속성 확인
            try
      {
        var props = type.GetProperties();
           if (props.Length == 0)
          return Visibility.Collapsed;
   }
        catch
      {
          return Visibility.Collapsed;
          }
       }

         return Visibility.Visible;
     }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
          throw new NotImplementedException();
    }
    }

    /// <summary>
    /// null 값을 Visibility로 변환 (역전): null -> Visible, not null -> Collapsed
    /// </summary>
    public class InverseNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
            if (value == null)
                return Visibility.Visible;

            var type = value.GetType();
            if (type.FullName?.Contains("EntityProxyModule") == true || 
    type.FullName?.Contains("Castle.Proxies") == true)
        {
    try
          {
     var props = type.GetProperties();
        if (props.Length == 0)
            return Visibility.Visible;
      }
         catch
           {
         return Visibility.Visible;
         }
            }

      return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
throw new NotImplementedException();
        }
    }
}
