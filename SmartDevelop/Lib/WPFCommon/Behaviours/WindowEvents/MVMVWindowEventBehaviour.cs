using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPFCommon.Behaviours.WindowEvents
{
    /*
    public class MVMVWindowEventBehaviour
    {
        readonly Window _window;

        public MVMVWindowEventBehaviour(Window window) {
            _window = window;
            _window.DataContextChanged += OnDataContextChanged;
            UpdateEvents();
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            var swmo = e.OldValue as WorkspaceViewModel;
            if (swmo != null)
                swmo.RequestClose -= OnRequestClose; // clean up old events
            UpdateEvents();
        }

        void OnRequestClose(object sender, EventArgs e) {
            _window.Close();
        }

        void UpdateEvents() {
            var wsm = _window.DataContext as WorkspaceViewModel;
            if (wsm != null) {
                wsm.RequestClose += OnRequestClose;
            }
        }

    }
    */
}
