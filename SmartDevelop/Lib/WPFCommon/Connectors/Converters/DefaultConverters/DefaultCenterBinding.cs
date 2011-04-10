using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace WPFCommon.Connectors.Converters.DefaultConverters
{
    public class DefaultCenterBinding : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            double left = 0;
            double top = 0;

            left = System.Convert.ToDouble(values[0]);
            top = System.Convert.ToDouble(values[1]);

            Point centerOffset = new Point();
            if (parameter != null && parameter is Point) {
                centerOffset = (Point)parameter;
            }
            return new Point((left + centerOffset.X), (top + centerOffset.Y));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
