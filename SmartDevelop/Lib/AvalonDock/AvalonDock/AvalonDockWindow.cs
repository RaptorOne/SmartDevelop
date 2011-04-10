﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace AvalonDock
{
    /// <summary>
    /// Base Class for each Avalon floating window
    /// </summary>
    public class AvalonDockWindow : Window
    {
        static AvalonDockWindow()
        {
            ShowInTaskbarProperty.OverrideMetadata(typeof(AvalonDockWindow), new FrameworkPropertyMetadata(false));
        }

        internal AvalonDockWindow()
        {
            if (AvalonGlobals.UseTransparentWindwos) {
                WindowStyle = System.Windows.WindowStyle.None;
                AllowsTransparency = true;
                this.Background = Brushes.Transparent;
            }

            if (AvalonGlobals.UseMinWindowSize) {
                this.MinHeight = 30;
                this.MinWidth = 150;
            }
        }
    }
}
