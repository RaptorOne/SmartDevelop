using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;


namespace WPFCommon.Connectors.Converters.DefaultConverters
{
    public class ConnectionSplineConverter : IMultiValueConverter
    {
        // Methods
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length != 4) {
                return null;
            }
            for (int i = 0; i < 4; i++) {
                if (!(values[i] is double)) {
                    return null;
                }
            }

            double[] offsets = (double[])parameter;

            double x1 = (double)values[0] + offsets[0];
            double y1 = (double)values[1] + offsets[1];
            double x2 = (double)values[2] + offsets[2];
            double y2 = (double)values[3] + offsets[3];

            return GetSpline(x1, y1, x2, y2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            return null;
        }

        internal static Geometry GetSpline(double x1, double y1, double x2, double y2) {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            double num = Math.Max((double)(Math.Abs((double)(x2 - x1)) / 2.0), (double)20.0);
            figure.StartPoint = new Point(x1, y1);
            BezierSegment segment = new BezierSegment
            {
                Point1 = new Point(x1 + num, y1),
                Point2 = new Point(x2 - num, y2),
                Point3 = new Point(x2, y2)
            };
            figure.Segments.Add(segment);
            geometry.Figures.Add(figure);
            return geometry;
        }
    }
}
