using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WPFCommon.Input
{
    /// <summary>
    /// Provides binding mouse events support for tools
    /// </summary>
    public interface IMouseListener
    {
        /// <summary>
        /// Handles the mouse-down event.
        /// </summary>
        /// <param name="e">Event data</param>
        void MouseDown(MouseEventArgs e);

        /// <summary>
        /// Handles the mouse-move event.
        /// </summary>
        /// <param name="e">Event data</param>
        void MouseMove(MouseEventArgs e);

        /// <summary>
        /// Handles the mouse-up event.
        /// </summary>
        /// <param name="e"></param>
        void MouseUp(MouseEventArgs e);
    }
}
