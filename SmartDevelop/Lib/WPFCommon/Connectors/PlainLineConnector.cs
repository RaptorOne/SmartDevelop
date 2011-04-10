using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WPFCommon.Connectors
{
    /// <summary>
    /// Very simple implementation of a Connector - a straight line connection.
    /// </summary>
    public class PlainLineConnector : ObjectConnector
    {
        LineGeometry linegeo;

        public PlainLineConnector() {
            linegeo = new LineGeometry();

            this.Stroke = Brushes.Black;
            this.StrokeThickness = 2;
        }

        protected override Geometry DefiningGeometry {
            get {
                linegeo.StartPoint = StartPoint;
                linegeo.EndPoint = EndPoint;
                return linegeo;
            }
        }
    }
}
