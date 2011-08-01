using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using System.Windows.Input;
using SmartDevelop.ViewModel.Projecting;
using Archimedes.Services.WPF.WorkBenchServices;
using Archimedes.Patterns.Services;
using Archimedes.Patterns.WPF.Commands;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemFolder : TreeViewProjectItem
    {
        IWorkBenchService _workbenchservice = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        ProjectItemFolder _folder;

        public TreeViewProjectItemFolder(ProjectItemFolder folder, TreeViewProjectItem parent)
            : base(folder, parent) {
            _folder = folder;
            ImageSource = @"../Images/folder.ico";
        }

        ICommand _addNewItemCommand;

        public ICommand AddNewItemCommand {
            get{
                if(_addNewItemCommand == null){
                    _addNewItemCommand = new RelayCommand(x => {
                            var vms = from item in _folder.Project.Language.GetAvaiableItemsForNew(_folder)
                                      select new NewItemViewModel(item);

                            var vm = new AddItemViewModel(_folder, vms)
                            {
                                DisplayName = "Add an new Item to this Folder"
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
