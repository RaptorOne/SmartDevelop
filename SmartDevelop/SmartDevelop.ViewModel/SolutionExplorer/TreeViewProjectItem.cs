using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.WPF.ViewModels.Trees;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItem : TreeViewItemViewModel<TreeViewProjectItem>
    {
        ProjectItem _item;
        string _imageSource;

        #region Static Builder

        public static TreeViewProjectItem Build(ProjectItem item, TreeViewProjectItem parent) {
            TreeViewProjectItem treeitem = null;

            if(item is ProjectItemCode) {
                treeitem = new TreeViewProjectItemCodeFile(item as ProjectItemCode, parent);
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

        protected void Add(ProjectItem item) {
            this.Children.Add(TreeViewProjectItem.Build(item, this));
        }

        protected void Remove(ProjectItem item) {
            var child = this.Children.ToList().Find(x => x.Item.Equals(item));
            this.Children.Remove(child);
        }

        public override string DisplayName {
            get {
                return _item.Name;
            }
            set {
                _item.Name = value;
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


        protected override void LoadChildren() {
            // todo: lazy loading
        }

        public override string ToString() {
            return this.DisplayName + "->> " + this.ImageSource;
        }

    }
}
