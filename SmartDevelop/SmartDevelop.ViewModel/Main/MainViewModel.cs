﻿using Archimedes.Patterns.WPF.ViewModels;
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
using SmartDevelop.Model;
using Archimedes.Services.WPF.WorkBenchServices.MessageBox;
using System.IO;
using SmartDevelop.Model.CodeLanguages;
using ICSharpCode.AvalonEdit;
using System.Collections.ObjectModel;
using SmartDevelop.ViewModel.FindAndReplace;
using SmartDevelop.Model.Resources;
using Archimedes.Services.WPF.FrameWorkDialogs;

namespace SmartDevelop.ViewModel.Main
{
    /// <summary>
    /// Main ViewModel
    /// This is actually a singleton
    /// </summary>
    public class MainViewModel : WorkspaceViewModel
    {
        #region Fields
        readonly IDE _ide;

        DockingManager _dockManager;
        IWorkBenchService _workbenchService = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        ICodeLanguageService _languageService = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
        SolutionExplorerVM _solutionVM;
        ErrorListViewModel _errorListVM;
        SmartSolution _solution;

        #endregion

        #region Constrcutor

        public MainViewModel(IDE ide) 
        {
            _ide = ide;

            if(_ide.CurrentSolution != null)
                this.SetSolution(_ide.CurrentSolution);

            _ide.CurrentSolutionChanged += (s, e) => {
                this.SetSolution(e.Value);
                };

            _ide.RequestHandleFileOpen += (s, e) => {
                OpenFile(e.Value);
            };

            Globals.MainVM = this;
        }

        #endregion

        #region Public Methods

        protected void SetSolution(SmartSolution solution) {
            _solution = solution;

            if(solution != null) {
                _solution.OutputDataChanged += (s, e) => {
                    OnPropertyChanged(() => OutputData);
                };

                SolutionVM = new SolutionExplorerVM(_solution);
                ErrorListVM = new ErrorListViewModel(_solution.ErrorService);
            } else {
                SolutionVM = null;
                ErrorListVM = null;
            }
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
                return Strings.Ready;
            }
        }

        public TextEditor ActiveEditor {
            get {
                if(this._solution != null && this._solution.ActiveDocument != null) {
                    return CodeFileViewModel.Create(_solution.ActiveDocument).Editor;
                } else
                    return null;
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
                        var vms = from item in _solution.ActiveProject.Language.GetAvaiableItemsForNew(_solution.ActiveProject)
                                  select new NewItemViewModel(item);

                        var vm = new AddItemViewModel(_solution.ActiveProject, vms)
                        {
                            DisplayName = Strings.AddNewItemToProject
                        };

                        _workbenchService.ShowDialog(vm, System.Windows.SizeToContent.WidthAndHeight);
                    }, x => {
                        return _solution != null && _solution.ActiveProject != null;
                    });

                }
                return _addNewItemCommand;
            }
        }
        #endregion 

        #region Create New Project Command

        ICommand _addNewProjectCommand;

        public ICommand AddNewProjectCommand {
            get {
                if(_addNewProjectCommand == null) {

                    _addNewProjectCommand = new RelayCommand(x => {

                            if(IDE.Instance.CurrentSolution != null) {

                                if(_workbenchService.MessageBox(Strings.DLG_OpenProjectMustBeClosedQuestion
                                    , Strings.CloseOpenProject, MessageBoxType.Question, MessageBoxWPFButton.YesNo) == IDDialogResult.No) {
                                    return;
                                }

                                if(!IDE.Instance.CurrentSolution.Close()) {
                                    return;
                                }
                                //IDE.Instance.CurrentSolution.Dispose();
                                IDE.Instance.CurrentSolution = null;
                            }

                            var vm = new CreateNewProjectVM()
                            {
                                DisplayName = Strings.CreateAProject
                            };
                            _workbenchService.ShowDialog(vm, System.Windows.SizeToContent.WidthAndHeight);

                            if(vm.CreatedProject != null) {
                                IDE.Instance.CurrentSolution = new SmartSolution() { Name = "Solution" };
                                IDE.Instance.CurrentSolution.Add(vm.CreatedProject);
                            }

                        }, x => {
                            return true;
                            });
                        

                }
                return _addNewProjectCommand;
            }
        }

        #endregion

        #region Open File Command

        ICommand _openFileCommand;
        public ICommand OpenFileCommand {
            get {
                if(_openFileCommand == null) {
                    _openFileCommand = new RelayCommand(x => {

                        string fileToOpen = null;

                        var openDlg = new OpenFileDialogViewModel();
                        openDlg.Filter = "Code Files|*";
                        openDlg.Title = Strings.SelectAFile;

                        if(_workbenchService.ShowDialog(openDlg, this) == IDDialogResult.OK) {
                            fileToOpen = openDlg.FileName;
                        } else {
                            return;
                        }

                        OpenFile(fileToOpen);

                    });
                }
                return _openFileCommand;
            }
        }

        /// <summary>
        /// Handles any given File Path
        /// </summary>
        /// <param name="fileToOpen"></param>
        protected void OpenFile(string fileToOpen) {

            if(_languageService.IsProjectFile(fileToOpen)) {

                OpenProject(fileToOpen);

            } else {
                if(_ide.CurrentSolution != null) {

                    var currentProject = _ide.CurrentSolution.ActiveProject;
                    if(currentProject.CanAdd(fileToOpen)) {
                        currentProject.Add(fileToOpen);
                    } else {
                        _workbenchService.MessageBox(
                            string.Format(Strings.NoPluginCanHandleExtension, Path.GetExtension(fileToOpen)),
                            Strings.FileOpenError, MessageBoxType.Error, MessageBoxWPFButton.OK);
                    }

                } else {

                    var lang = _languageService.GetByExtension(Path.GetExtension(fileToOpen));

                    if(lang == null) {
                        _workbenchService.MessageBox(Strings.NoPluginCanHandleSelectedFile, Strings.UnknownFile, MessageBoxType.Error);
                        return;
                    }

                    if(_workbenchService.MessageBox(
                        string.Format(Strings.NoActiveProjectCanHandleCreateANew, lang.Name), Strings.OpenAItem
                        , MessageBoxType.Question, MessageBoxWPFButton.YesNo) == IDDialogResult.Yes) {

                        var name = Path.GetFileNameWithoutExtension(fileToOpen);
                        var project = lang.Create(name, name, Path.GetDirectoryName(fileToOpen));

                        var doc = new ProjectItemCodeDocument(Path.GetFileName(fileToOpen), lang, project);
                        project.Add(doc);
                        doc.IsStartUpDocument = true;
                        doc.ReloadDocument();

                        _ide.CurrentSolution = new SmartSolution() { Name = "Solution" };
                        _ide.CurrentSolution.Add(project);

                        doc.ShowInWorkSpace();
                        project.SaveProject();
                    }
                }
            }
        }

        void OpenProject(string fileToOpen) {
            if(_ide.CurrentSolution != null) {

                if(_workbenchService.MessageBox(Strings.DLG_OpenProjectMustBeClosedQuestion
                    , Strings.CloseOpenProject, MessageBoxType.Question, MessageBoxWPFButton.YesNo) == IDDialogResult.No) {
                    return;
                }

                if(!_ide.CurrentSolution.Close()) {
                    return;
                }
                _ide.CurrentSolution = null;
            }

            var loadedProject = _languageService.LoadProjectFromFile(fileToOpen);

            _ide.CurrentSolution = new SmartSolution() { Name = "Solution" };
            _ide.CurrentSolution.Add(loadedProject);
        }


        #endregion

        #region Save Current File Command

        ICommand _saveCurrentFileCommand;

        public ICommand SaveCurrentFileCommand {
            get {
                if(_saveCurrentFileCommand == null) {
                    _saveCurrentFileCommand = new RelayCommand(x => {
                        _solution.ActiveDocument.QuickSave();
                    }, x => {
                        return _solution != null && _solution.ActiveDocument != null && _solution.ActiveDocument.HasUnsavedChanges;
                    });
                }
                return _saveCurrentFileCommand;
            }
        }

        #endregion

        #region Save All Files Command

        ICommand _saveAllCommand;

        public ICommand SaveAllCommand {
            get {
                if(_saveAllCommand == null) {
                    _saveAllCommand = new RelayCommand(x => {
                        _solution.ActiveProject.QuickSaveAll();
                    }, x => {
                        return _solution != null && _solution.ActiveProject != null && _solution.ActiveProject.CanQuickSaveAll;
                    });
                }
                return _saveAllCommand;
            }
        }

        #endregion

        #region Run Active Projet Command

        ICommand _runActiveProjetCommand;

        public ICommand RunActiveProjetCommand {
            get {
                if(_runActiveProjetCommand == null) {
                    _runActiveProjetCommand = new RelayCommand(x => {
                        _solution.ActiveProject.Run();
                        }, 
                        x => {
                            return _solution != null && _solution.ActiveProject != null && _solution.ActiveProject.CanRun;
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
                        _solution.ActiveProject.Language.ShowLanguageSettings();
                    },
                        x => {
                            return _solution != null && _solution.ActiveProject != null;
                        });
                }
                return _ShowCurrentLanguageSettingsCommand;
            }
        }

        #endregion

        #region Show About Command

        ICommand _showAboutCommand;

        AboutViewModel _aboutVM;

        public ICommand ShowAboutCommand {
            get {
                if(_showAboutCommand == null) {
                    _showAboutCommand = new RelayCommand(x => {
                        if(_aboutVM == null)
                            _aboutVM = new AboutViewModel()
                            {
                                DisplayName = "About"
                            };
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

        #region Find And Replace Command

        ICommand _findAndReplaceCommand;

        public ICommand FindAndReplaceCommand {
            get {
                if(_findAndReplaceCommand == null){
                    _findAndReplaceCommand = new RelayCommand(x => {
                        var vm = new FindReplaceViewModel()
                        {
                            DisplayName = Strings.FindAndReplace,
                        };
                        vm.CurrentDocument = _solution.ActiveDocument;
                        _workbenchService.ShowFloating(vm, System.Windows.SizeToContent.WidthAndHeight);
                    }, x => {
                        return _solution != null;
                    });
                }
                return _findAndReplaceCommand;
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
