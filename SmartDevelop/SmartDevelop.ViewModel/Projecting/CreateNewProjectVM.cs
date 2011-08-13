using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using System.Collections.ObjectModel;
using SmartDevelop.Model.CodeLanguages;
using Archimedes.Patterns.Services;
using Archimedes.Patterns.WPF.ViewModels;
using System.Windows.Input;
using Archimedes.Patterns.WPF.Commands;
using System.IO;
using System.Windows.Forms;
using SmartDevelop.Model;
using Archimedes.Services.WPF.WorkBenchServices;
using Archimedes.Services.WPF.WorkBenchServices.MessageBox;
using SmartDevelop.Model.Resources;

namespace SmartDevelop.ViewModel.Projecting
{
    public class CreateNewProjectVM : WorkspaceViewModel
    {

        #region Fields

        IWorkBenchService _workbenchService = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        ICodeLanguageService _serviceLang = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
        string _name;
        string _location;

        #endregion

        public CreateNewProjectVM() {

            var alltemplates = from lang in _serviceLang.GetAllLanguages()
                               from template in lang.GetProjectTemplates()
                               select new ProjectTemplateVM(template);

            AllTemplates = new ObservableCollection<ProjectTemplateVM>(alltemplates);

            Name = "My Super Project";
            Location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmartDevelop");
        }

        #region Properties

        public ObservableCollection<ProjectTemplateVM> AllTemplates {
            get;
            protected set;
        }

        public ProjectTemplateVM SelectedItem {
            get;
            set;
        }

        /// <summary>
        /// Name of the New Project
        /// </summary>
        public string Name {
            get { return _name; }
            set { 
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }
        
        /// <summary>
        /// Location of the New Project
        /// </summary>
        public string Location {
            get { return _location; }
            set { 
                _location = value;
                OnPropertyChanged(() => Location);
            }
        }


        /// <summary>
        /// Gets the created Project - if any
        /// </summary>
        public SmartCodeProject CreatedProject {
            get;
            protected set;
        }

        #endregion

        #region Commands

        #region Create Selected Project Command

        ICommand _createSelectedProjectCommand;
        public ICommand CreateSelectedProjectCommand {
            get {
                if(_createSelectedProjectCommand == null) {
                    _createSelectedProjectCommand = new RelayCommand(x => {
                        // create the project
                        CreatedProject = SelectedItem.Template.Create(
                            this.Name,
                            EscapeName(this.Name, Path.GetInvalidFileNameChars()),
                            Path.Combine(this.Location, EscapeName(this.Name, Path.GetInvalidPathChars())));
                        CreatedProject.SaveProject();
                        this.CloseCommand.Execute(null);
                    }, x => {
                        return SelectedItem != null;
                    });
                }
                return _createSelectedProjectCommand;
            }
        }

        #endregion

        #region Browse Project Folder Command

        ICommand _browseProjectFolderCommand;
        public ICommand BrowseProjectFolderCommand {
            get {
                if(_browseProjectFolderCommand == null) {
                    _browseProjectFolderCommand = new RelayCommand(x => {

                        using(FolderBrowserDialog dlg = new FolderBrowserDialog()) {
                            dlg.Description = Strings.SelectProjectFolder;
                            if(dlg.ShowDialog() == DialogResult.OK) {
                                this.Location = dlg.SelectedPath;
                            }
                        }

                    });
                }
                return _browseProjectFolderCommand;
            }
        }

        #endregion

        #endregion

        string EscapeName(string name, char[] illegalChars) {
            StringBuilder sb = new StringBuilder();

            var illegal = illegalChars.ToList();
            char cu;
            foreach(char c in name) {
                cu = c;
                if(illegal.Contains(c)) {
                    cu = '_';
                }
                sb.Append(cu);
            }
            return sb.ToString();
        }


    }
}
