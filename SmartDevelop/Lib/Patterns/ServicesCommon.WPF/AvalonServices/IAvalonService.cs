using System;
namespace ServicesCommon.WPF.AvalonServices
{
    public interface IAvalonService
    {
        AvalonDock.DockingManager PrimaryDockManager { get; set; }
        event EventHandler PrimaryDockManagerChanged;
        event EventHandler PrimaryDockManagerLoaded;
        void OnPrimaryDockManagerLoaded();
    }
}
