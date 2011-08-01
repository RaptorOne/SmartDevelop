using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.ViewModel.DocumentFiles;
using System.Windows.Input;
using SmartDevelop.ViewModel.Projecting;
using Archimedes.Services.WPF.WorkBenchServices;
using Archimedes.Patterns.Services;
using Archimedes.Patterns.WPF.Commands;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemProject : TreeViewProjectItem
    {
        readonly SmartCodeProject _project;
        IWorkBenchService _workbenchservice = ServiceLocator.Instance.Resolve<IWorkBenchService>();

        public TreeViewProjectItemProject(SmartCodeProject project, TreeViewProjectItem parent)
            : base(project, parent) {
                ImageSource = @"../Images/project-folder.ico";
                _project = project;

                _project.RequestShowDocument += (s, e) => {
                        if(e.Value is ProjectItemCodeDocument) {
                            var codeVM = CodeFileViewModel.Create(e.Value as ProjectItemCodeDocument);
                            var showcmd = codeVM.ShowCommand;
                            if(showcmd.CanExecute(null))
                                showcmd.Execute(null);
                        }
                    };
        }

        public override string DisplayName {
            get {
                return _project.Name;
            }
            set {
                _project.Name = value;
            }
        }


        ICommand _addNewItemCommand;

        public ICommand AddNewItemCommand {
            get {
                if(_addNewItemCommand == null) {

                    _addNewItemCommand = new RelayCommand(x => {
                        var vms = from item in _project.Project.Language.GetAvaiableItemsForNew(_project)
                                  select new NewItemViewModel(item);

                        var vm = new AddItemViewModel(_project, vms)
                            {
                                DisplayName = "Add an new Item to this Project"
                            };

                        _workbenchservice.ShowDialog(vm, System.Windows.SizeToContent.WidthAndHeight);
                    }, x => {
                        return true;
                    });

                }
                return _addNewItemCommand;
            }
        }


    }
}
