using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFCommon.ViewModels;
using AvalonDock;
using SmartDevelop.ViewModel.DocumentFiles;
using ICSharpCode.AvalonEdit.Document;
using ServicesCommon;
using ServicesCommon.WPF.WorkBenchServices;
using ServicesCommon.WPF.AvalonServices;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.ViewModel.Main
{
    public class MainViewModel : WorkspaceViewModel
    {
        DockingManager _dockManager;
        IWorkBenchService _workbenchService = ServiceLocator.Instance.Resolve<IWorkBenchService>();

        public MainViewModel() { }

        public void SetDockManager(DockingManager dockmanager) {
            _dockManager = dockmanager;
            ServiceLocator.Instance.Resolve<IAvalonService>().PrimaryDockManager = _dockManager;

            // debug only
            var codeVM = new CodeFileViewModel(new ProjectItemCode(CodeItemType.AHK));
            codeVM.DisplayName = "Example Content";
            _workbenchService.ShowDockedDocument(codeVM, codeVM.DisplayName);
        }
        
    }
}
