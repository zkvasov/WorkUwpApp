using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WorkUwpApp.Converters
{
    public class NegativeBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                return Visibility.Visible;
            }
            bool objValue = (bool)value;
            if (objValue)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if ((bool)value)
                {
                    return Visibility.Collapsed;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { }
#pragma warning restore CA1031 // Do not catch general exception types

            return Visibility.Visible;
        }
    }
}
