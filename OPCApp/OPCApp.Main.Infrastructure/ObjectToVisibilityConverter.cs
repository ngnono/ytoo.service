//===================================================================================
//
// 
//===================================================================================
// ���ߣ�������
// �������ڣ�2014-2-5
//===================================================================================
// �޸ļ�¼
//
//===================================================================================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OPCApp.Infrastructure
{
    public class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
