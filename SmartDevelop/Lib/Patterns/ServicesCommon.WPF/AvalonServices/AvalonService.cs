using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvalonDock;

namespace ServicesCommon.WPF.AvalonServices
{
    public class AvalonService : IAvalonService
    {
        /// <summary>
        /// Raised when the PrimaryDockingManager changes :)
        /// </summary>
        public event EventHandler PrimaryDockManagerChanged;

        public event EventHandler PrimaryDockManagerLoaded;

        DockingManager _primaryDockManager;

        /// <summary>
        /// Reference to the Root DockManager.
        /// </summary>
        public DockingManager PrimaryDockManager {
            get { return _primaryDockManager; }
            set {
                _primaryDockManager = value;
                if (PrimaryDockManagerChanged != null)
                    PrimaryDockManagerChanged(this, null);
            }
        }


        

        public void OnPrimaryDockManagerLoaded() {
            if (PrimaryDockManagerLoaded != null)
                PrimaryDockManagerLoaded(this, EventArgs.Empty);
        }
    }
}
