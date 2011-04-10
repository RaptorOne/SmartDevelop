using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;


namespace WPFCommon.Connectors.Converters.DefaultConverters
{
    public class DefaultAngleCenterConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            Point centerOffset = (Point)parameter;

            Point p1 = new Point(
                System.Convert.ToDouble(values[0]) + centerOffset.X,
                System.Convert.ToDouble(values[1]) + centerOffset.Y);

            Point p2 = new Point(
                System.Convert.ToDouble(values[2]),
                System.Convert.ToDouble(values[3]));


            Vector a = new Vector(0, Math.Abs((p1.Y != p2.Y) ? p1.Y - p2.Y : p2.Y));
            Vector b = new Vector(p2.X - p1.X, p1.Y - p2.Y);
            double angle = Vector.AngleBetween(b, a);

            // TODO: provide correct radius for the shape
            //double radius = centerOffset.X;
            double radius = Math.Max(centerOffset.X, centerOffset.Y);

            double x = radius * Math.Sin(angle * Math.PI / 180);
            double y = radius * Math.Cos(angle * Math.PI / 180);

            Point touchPoint = new Point(p1.X + x, p1.Y - y);

            return touchPoint;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
