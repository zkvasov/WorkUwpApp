using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using WorkUwpApp.Models;

namespace WorkUwpApp.Converters
{
    public class SelectedItemsToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = new List<IconImage>();
            foreach (var item in ((parameter as ListViewBase).SelectedItems))
            {
                list.Add(item as IconImage);
            }
            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (parameter as ListViewBase).SelectedItems;
        }
    }
}
