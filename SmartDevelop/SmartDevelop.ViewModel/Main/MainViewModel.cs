using Archimedes.Patterns.WPF.ViewModels;
using SmartDevelop.Model.Projecting;
using SmartDevelop.ViewModel.DocumentFiles;
using SmartDevelop.ViewModel.SolutionExplorer;
using Archimedes.Patterns.Services;
using AvalonDock;
using Archimedes.Services.WPF.WorkBenchServices;
using Archimedes.Services.WPF.AvalonDockService;
using System.Windows.Input;
using Archimedes.Patterns.WPF.Commands;

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
        }

        public SolutionExplorerVM SolutionVM {
            get { return _solutionVM; }
        }


        public ICommand OpenFileCommand {
            get { return null; }
        }


        ICommand _saveCurrentFileCommand;
        public ICommand SaveCurrentFileCommand {
            get {
                if(_saveCurrentFileCommand == null) {
                    _saveCurrentFileCommand = new RelayCommand(x => 
                    {

                    });
                }
                return _saveCurrentFileCommand;
            }
        }

        public ICommand SaveAllCommand {
            get { return null; }
        }

            

    }
}
