using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Archimedes.Patterns.WPF.Commands;
using Archimedes.Patterns.WPF.ViewModels;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.ViewModel.Projecting
{
    public class AddItemViewModel : WorkspaceViewModel
    {
        readonly ProjectItem _parent;
        string _fileName;
        NewItemViewModel _selectedItem;


        public AddItemViewModel(ProjectItem parent, IEnumerable<NewItemViewModel> items) {
            _parent = parent;
            AllAvaiableItems = new ObservableCollection<NewItemViewModel>(items);
        }

        public ObservableCollection<NewItemViewModel> AllAvaiableItems {
            get;
            protected set;
        }

        
        public string FileName {
            get { return _fileName; }
            set { 
                _fileName = value;
                OnPropertyChanged(() => FileName);
            }
        }

        
        public NewItemViewModel SelectedItem {
            get { return _selectedItem; }
            set { 
                _selectedItem = value;
                OnSelectedItemChanged();
                
            }
        }

        #region Commands

        #region Add Item Command

        ICommand _addItemCommand;
        public ICommand AddItemCommand {
            get {
                if(_addItemCommand == null) {
                    _addItemCommand = new RelayCommand(x => {

                        SelectedItem.FileName = this.FileName;

                        var p = SelectedItem.Create(_parent);
                        if(p.CanShow)
                            p.ShowInWorkSpace();
                        this.CloseCommand.Execute(null);

                        }, x => {
                            return SelectedItem != null && 
                                null == _parent.Children.Find(ex => ex.Name.Equals(FileName, StringComparison.InvariantCultureIgnoreCase));
                            });

                }
                return _addItemCommand;
            }
        }
        #endregion

        #endregion

        void OnSelectedItemChanged(){
            if(SelectedItem != null)
                this.FileName = SelectedItem.FileName;

            OnPropertyChanged(() => SelectedItem); 
        }

    }




}
