using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WPFCommon.Connectors
{
    public class ArrowLineConnector : ArrowLineBaseConnector
    {
        #region Constructor

        public ArrowLineConnector()
            : base()
        {
            this.Stroke = Brushes.Black;
            this.StrokeThickness = 2;
        } 

        #endregion


        /// <summary>
        ///     Gets a value that represents the Geometry of the ArrowLine.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Clear out the PathGeometry.
                pathgeo.Figures.Clear();
                // Define a single PathFigure with the points.
                pathfigLine.StartPoint = StartPoint;
                polysegLine.Points.Clear();
                polysegLine.Points.Add(EndPoint);
                pathgeo.Figures.Add(pathfigLine);

                // Call the base property to add arrows on the ends.
                return base.DefiningGeometry;
            }
        }
    }
}
