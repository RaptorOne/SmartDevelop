using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.WPF.ViewModels.Trees;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns.Utils;
using System.Windows.Input;
using Archimedes.Patterns.WPF.Commands;
using System.Windows;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItem : TreeViewItemViewModel<TreeViewProjectItem>
    {
        ProjectItem _item;
        string _imageSource;

        #region Static Builder

        public static TreeViewProjectItem Build(ProjectItem item, TreeViewProjectItem parent) {
            TreeViewProjectItem treeitem = null;

            if(item is SmartCodeProject) {
                treeitem = new TreeViewProjectItemProject(item as SmartCodeProject, parent);
            } else if(item is ProjectItemCodeDocument) {
                treeitem = new TreeViewProjectItemCodeFile(item as ProjectItemCodeDocument, parent);
            } else if(item is ProjectItemFolder) {
                treeitem = new TreeViewProjectItemFolder(item as ProjectItemFolder, parent);
            } else
                throw new NotSupportedException();
            return treeitem;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Root Constructor
        /// </summary>
        public TreeViewProjectItem() 
            : base() { }

        public TreeViewProjectItem(ProjectItem item, TreeViewProjectItem parent) 
            : base(parent) {

                ThrowUtil.ThrowIfNull(item);
                _item = item;
                
            // import existing
                foreach(var child in _item.GetAllItems()) {
                    Add(child);
                }

            // listen for new Items
                _item.ItemAdded += (s, e) => {
                        Add(e.Item);
                };

            // listen for remvoe
                _item.ItemRemoved += (s, e) => {
                    Remove(e.Item);
                };
        }

        //public TreeViewProjectItem(TreeViewProjectItem parent, bool lazyload) 
        //    : base(parent, lazyload) {
        //    throw new NotImplementedException("LazyLoading");
        //}

        #endregion

        #region Properties

        public override string DisplayName {
            get {
                return _item.Name;
            }
            set {
                _item.Name = value;
                OnPropertyChanged(() => DisplayName);
                EndRename();
            }
        }

        /// <summary>
        /// Relative URI to Image Resource
        /// </summary>
        public string ImageSource {
            get { return _imageSource; }
            set {
                _imageSource = value;
                OnPropertyChanged(() => ImageSource);
            }
        }

        /// <summary>
        /// Gets the underlying domain object
        /// </summary>
        public ProjectItem Item {
            get { return _item; }
        }

        #region Visibility

        public Visibility NameVisibility {
            get { 
                return (EditableNameVisibility == Visibility.Collapsed || EditableNameVisibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        Visibility _editableNameVisibility = Visibility.Collapsed;
        public Visibility EditableNameVisibility {
            get { return _editableNameVisibility; }
            set { 
                _editableNameVisibility = value;
                OnPropertyChanged(() => EditableNameVisibility);
                OnPropertyChanged(() => NameVisibility);
            }
        }

        #endregion

        #endregion

        #region Helper Methods

        protected void Add(ProjectItem item) {
            this.Children.Add(TreeViewProjectItem.Build(item, this));
        }

        protected void Remove(ProjectItem item) {
            var child = this.Children.ToList().Find(x => x.Item.Equals(item));
            this.Children.Remove(child);
        }

        protected override void LoadChildren() {
            // todo: lazy loading
        }
        
        #endregion

        #region Commands

        #region Remove Command

        ICommand _removeCommand;

        public ICommand RemoveCommand {
            get {
                if(_removeCommand == null) {
                    _removeCommand = new RelayCommand(x => Remove(x), x => CanRemove);
                }
                return _removeCommand;
            }
        }

        protected virtual bool CanRemove {
            get { return false; } 
        }

        protected virtual void Remove(object arg) { }

        #endregion

        #region Delete Command

        ICommand _deleteCommand;

        public ICommand DeleteCommand {
            get {
                if(_deleteCommand == null) {
                    _deleteCommand = new RelayCommand(x => Delete(x), x => CanDelete);
                }
                return _deleteCommand;
            }
        }

        protected virtual bool CanDelete {
            get { return false; }
        }

        protected virtual void Delete(object arg) { }

        #endregion

        #region Rename Command

        ICommand _startRenameCommand;

        public ICommand StartRenameCommand {
            get {
                if(_startRenameCommand == null) {
                    _startRenameCommand = new RelayCommand(x => StartRename(x), x => CanStartRename);
                }
                return _startRenameCommand;
            }
        }

        protected virtual bool CanStartRename {
            get { return true; }
        }

        protected virtual void StartRename(object arg) {
            EditableNameVisibility = Visibility.Visible;
        }

        protected virtual void EndRename() {
            EditableNameVisibility = Visibility.Collapsed;
        }

        #endregion

        #endregion

        public override string ToString() {
            return this.DisplayName + "->> " + this.ImageSource;
        }

    }
}
