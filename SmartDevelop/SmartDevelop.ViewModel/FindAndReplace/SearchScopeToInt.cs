using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace SmartDevelop.ViewModel.FindAndReplace
{

    public class SearchScopeToInt : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            SearchScope val = (SearchScope)value;
            return val;
        }
    }
}
