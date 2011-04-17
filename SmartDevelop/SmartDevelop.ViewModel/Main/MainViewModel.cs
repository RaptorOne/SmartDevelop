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
using SmartDevelop.ViewModel.SolutionExplorer;

namespace SmartDevelop.ViewModel.Main
{
    public class MainViewModel : WorkspaceViewModel
    {
        DockingManager _dockManager;
        IWorkBenchService _workbenchService = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        SolutionExplorerVM _solutionVM;

        public MainViewModel(SmartSolution solution) 
        {
            _solutionVM = new SolutionExplorerVM(solution);
        }

        public void SetDockManager(DockingManager dockmanager) {
            _dockManager = dockmanager;
            ServiceLocator.Instance.Resolve<IAvalonService>().PrimaryDockManager = _dockManager;

            // debug only
            var codeVM = new CodeFileViewModel(new ProjectItemCode(CodeItemType.IA));
            codeVM.DisplayName = "Example Content";
            _workbenchService.ShowDockedDocument(codeVM, codeVM.DisplayName);
        }


        
        public SolutionExplorerVM SolutionVM {
            get { return _solutionVM; }
        }
        
    }
}
