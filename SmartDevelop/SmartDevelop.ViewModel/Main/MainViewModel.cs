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
using System.Linq;
using System.Windows.Forms;
using SmartDevelop.ViewModel.Errors;

namespace SmartDevelop.ViewModel.Main
{
    /// <summary>
    /// Main ViewModel
    /// </summary>
    public class MainViewModel : WorkspaceViewModel
    {
        #region Fields

        DockingManager _dockManager;
        IWorkBenchService _workbenchService = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        SolutionExplorerVM _solutionVM;
        SmartSolution _solution;

        #endregion

        public MainViewModel(SmartSolution solution) 
        {
            Globals.MainVM = this;
            _solution = solution;
            _solutionVM = new SolutionExplorerVM(solution);
            _errorListVM = new ErrorListViewModel(solution.ErrorService);
        }

        public void SetDockManager(DockingManager dockmanager) {
            _dockManager = dockmanager;
            ServiceLocator.Instance.Resolve<IAvalonService>().PrimaryDockManager = _dockManager;
        }
        
        #region Child VMs

        public SolutionExplorerVM SolutionVM {
            get { return _solutionVM; }
        }

        ErrorListViewModel _errorListVM;

        public ErrorListViewModel ErrorListVM {
            get { return _errorListVM; }
        }

        #endregion

        #region Commands

        #region Open File Command

        ICommand _openFileCommand;
        public ICommand OpenFileCommand {
            get {
                if(_openFileCommand == null) {
                    _openFileCommand = new RelayCommand(x => {
                        var currentProject = _solution.Current;
                        if(currentProject != null) {


                            // Displays an OpenFileDialog so the user can select a Cursor.
                            var openFileDialog1 = new OpenFileDialog();
                            openFileDialog1.Filter = "Code Files|*" + currentProject.Language.Extensions.First();
                            openFileDialog1.Title = "Select a Script File";

                            if(openFileDialog1.ShowDialog() == DialogResult.OK) {
                                var file = ProjectItemCode.FromFile(openFileDialog1.FileName, currentProject);
                                if(file != null)
                                    currentProject.Add(file);
                            }

                        }
                    });
                }
                return _openFileCommand;
            }
        }

        #endregion

        #region Save Current File Command (ToDo)

        ICommand _saveCurrentFileCommand;
        public ICommand SaveCurrentFileCommand {
            get {
                if(_saveCurrentFileCommand == null) {
                    _saveCurrentFileCommand = new RelayCommand(x => 
                    {
                        //todo
                    });
                }
                return _saveCurrentFileCommand;
            }
        }

        #endregion

        #region Save Current File Command (ToDo)

        public ICommand SaveAllCommand {
            get { return null; }
        }

        #endregion

        #endregion
    }
}
