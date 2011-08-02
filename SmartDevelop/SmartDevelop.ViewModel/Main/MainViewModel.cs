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
using SmartDevelop.ViewModel.Projecting;
using SmartDevelop.ViewModel.About;

namespace SmartDevelop.ViewModel.Main
{
    /// <summary>
    /// Main ViewModel
    /// This is actually a singleton
    /// </summary>
    public class MainViewModel : WorkspaceViewModel
    {
        #region Fields

        DockingManager _dockManager;
        IWorkBenchService _workbenchService = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        SolutionExplorerVM _solutionVM;
        ErrorListViewModel _errorListVM;
        SmartSolution _solution;

        #endregion

        #region Constrcutor

        public MainViewModel() 
        {
            Globals.MainVM = this;
        }

        #endregion

        #region Public Methods

        public void SetSolution(SmartSolution solution) {
            _solution = solution;

            _solution.OutputDataChanged += (s, e) => {
                    OnPropertyChanged(() => OutputData);
                };

            SolutionVM = new SolutionExplorerVM(_solution);
            ErrorListVM = new ErrorListViewModel(_solution.ErrorService);
        }

        public void SetDockManager(DockingManager dockmanager) {
            _dockManager = dockmanager;
            ServiceLocator.Instance.Resolve<IAvalonService>().PrimaryDockManager = _dockManager;
        }
        
        #endregion

        public string OutputData {
            get {
                if(_solution != null)
                    return _solution.OutputData;
                return "Ready...";
            }
        }

        #region Child VMs

        public SolutionExplorerVM SolutionVM {
            get { return _solutionVM; }
            protected set { 
                _solutionVM = value;
                OnPropertyChanged(() => SolutionVM);
            }
        }

        

        public ErrorListViewModel ErrorListVM {
            get { return _errorListVM; }
            protected set { 
                _errorListVM = value;
                OnPropertyChanged(() => ErrorListVM);
            }
        }

        #endregion

        #region Commands

        #region New File Command
        ICommand _addNewItemCommand;

        public ICommand AddNewItemCommand {
            get {
                if(_addNewItemCommand == null) {

                    _addNewItemCommand = new RelayCommand(x => {
                        var vms = from item in _solution.Current.Language.GetAvaiableItemsForNew(_solution.Current)
                                  select new NewItemViewModel(item);

                        var vm = new AddItemViewModel(_solution.Current, vms)
                        {
                            DisplayName = "Add an new Item to this Project"
                        };

                        _workbenchService.ShowDialog(vm, System.Windows.SizeToContent.WidthAndHeight);
                    }, x => {
                        return _solution != null && _solution.Current != null;
                    });

                }
                return _addNewItemCommand;
            }
        }
        #endregion 

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
                                var file = ProjectItemCodeDocument.FromFile(openFileDialog1.FileName, currentProject);
                                if(file != null)
                                    currentProject.Add(file);
                                file.ShowInWorkSpace();
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

        #region Run Active Projet Command

        ICommand _runActiveProjetCommand;

        public ICommand RunActiveProjetCommand {
            get {
                if(_runActiveProjetCommand == null) {
                    _runActiveProjetCommand = new RelayCommand(x => {
                        _solution.Current.Run();
                        }, 
                        x => {
                            return _solution != null && _solution.Current != null && _solution.Current.CanRun;
                            });
                }
                return _runActiveProjetCommand;
            }
        }

        #endregion

        #region Show Current Language Settings Command

        ICommand _ShowCurrentLanguageSettingsCommand;

        public ICommand ShowCurrentLanguageSettingsCommand {
            get {
                if(_ShowCurrentLanguageSettingsCommand == null) {
                    _ShowCurrentLanguageSettingsCommand = new RelayCommand(x => {
                        _solution.Current.Language.ShowLanguageSettings();
                    },
                        x => {
                            return _solution != null && _solution.Current != null;
                        });
                }
                return _ShowCurrentLanguageSettingsCommand;
            }
        }

        #endregion

        #region Show About Command (todo)

        ICommand _showAboutCommand;

        AboutViewModel _aboutVM;

        public ICommand ShowAboutCommand {
            get {
                if(_showAboutCommand == null) {
                    _showAboutCommand = new RelayCommand(x => {
                        if(_aboutVM == null)
                            _aboutVM = new AboutViewModel();
                        _workbenchService.ShowDialog(_aboutVM, System.Windows.SizeToContent.WidthAndHeight);
                    },
                        x => {
                            return true;
                        });
                }
                return _showAboutCommand;
            }
        }

        #endregion

        #region Show Help Command (todo)

        ICommand _showHelpCommand;

        public ICommand ShowHelpCommand {
            get {
                if(_showHelpCommand == null) {
                    _showHelpCommand = new RelayCommand(x => {
                        //toDo!
                    },
                        x => {
                            return false;
                        });
                }
                return _showHelpCommand;
            }
        }

        #endregion



        #endregion
    }
}
