using Archimedes.Patterns.WPF.ViewModels;
using SmartDevelop.Model.Projecting;
using SmartDevelop.ViewModel.DocumentFiles;
using SmartDevelop.ViewModel.SolutionExplorer;
using Archimedes.Patterns.Services;
using AvalonDock;
using Archimedes.Services.WPF.WorkBenchServices;
using Archimedes.Services.WPF.AvalonDockService;

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
            var codeVM = CodeFileViewModel.Create(new ProjectItemCode(CodeItemType.IA));
            codeVM.DisplayName = "Example Content";
            _workbenchService.ShowDockedDocument(codeVM, codeVM.DisplayName);
        }


        
        public SolutionExplorerVM SolutionVM {
            get { return _solutionVM; }
        }
        
    }
}
