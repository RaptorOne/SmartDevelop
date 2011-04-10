using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace WPFCommon.Behaviours.DragMoving
{
    /// <summary>
    /// DragMoveable Behaviour for Controls hosted in a Canvas
    /// </summary>
    public class DragMoveableCanvasBehaviour : DragMoveableBehaviour
    {
        public DragMoveableCanvasBehaviour(FrameworkElement e)
            : base(e) { }

        protected override void SetPosition(double x, double y) {
            Canvas.SetLeft(AttachedElement, x);
            Canvas.SetTop(AttachedElement, y);
        }
    }
}
