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
using Archimedes.Services.WPF.FrameWorkDialogs;
using SmartDevelop.Model.Resources;
using System.IO;
using Archimedes.Services.WPF.WorkBenchServices.MessageBox;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemFolder : TreeViewProjectItem
    {
        IWorkBenchService _workbenchService = ServiceLocator.Instance.Resolve<IWorkBenchService>();
        ProjectItemFolder _folder;


        protected TreeViewProjectItemFolder(ProjectItem item, TreeViewProjectItem parent) 
            : base(item, parent) { }


        public TreeViewProjectItemFolder(ProjectItemFolder folder, TreeViewProjectItem parent)
            : base(folder, parent) {
            _folder = folder;
            ImageSource = @"../Images/folder.ico";
        }

        #region Commands

        ICommand _addNewItemCommand;
        ICommand _addExistingItemCommand;

        public virtual ICommand AddNewItemCommand {
            get{
                if(_addNewItemCommand == null){
                    _addNewItemCommand = new RelayCommand(x => {
                            var vms = from item in Item.Project.Language.GetAvaiableItemsForNew(Item)
                                      select new NewItemViewModel(item);

                            var vm = new AddItemViewModel(Item, vms)
                            {
                                DisplayName = string.Format("Add an new Item to {0}", this.DisplayName)
                            };

                            _workbenchService.ShowDialog(vm, System.Windows.SizeToContent.WidthAndHeight);
                        }, x => {
                            return true;
                            });
                }
                return _addNewItemCommand; 
            }
        }

       
        public virtual ICommand AddExistingItemCommand {
            get {
                if(_addExistingItemCommand == null) {
                    _addExistingItemCommand = new RelayCommand(x => {
                        // todo existing item
                        var openDlg = new OpenFileDialogViewModel();
                        if(_workbenchService.ShowDialog(openDlg, this) == IDDialogResult.OK) {

                            if(Item.CanAdd(openDlg.FileName)) {
                                Item.Add(openDlg.FileName);
                            } else {
                                _workbenchService.MessageBox(
                                    string.Format(Strings.NoPluginCanHandleExtension, Path.GetExtension(openDlg.FileName)),
                                    Strings.FileOpenError, MessageBoxType.Error, MessageBoxWPFButton.OK);
                            }

                        }

                    });
                }
                return _addExistingItemCommand;
            }
        }

        ICommand _addNewFolderCommand;

        public virtual ICommand AddNewFolderCommand {
            get {

                if(_addNewFolderCommand == null) {
                    _addNewFolderCommand = new RelayCommand(x => {
                        Item.Add(new ProjectItemFolder("New Folder", Item));
                    });
                }
                return _addNewFolderCommand;
            }
        }


        #endregion

    }
}
