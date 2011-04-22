using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.WPF.ViewModels.Trees;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItem : TreeViewItemViewModel<TreeViewProjectItem>
    {
        string _imageSource;

        public TreeViewProjectItem() : base() { }
        public TreeViewProjectItem(TreeViewProjectItem parent) : base(parent) { }
        public TreeViewProjectItem(TreeViewProjectItem parent, bool lazyload) : base(parent, lazyload) {
            throw new NotImplementedException("LazyLoading");
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


        protected override void LoadChildren() {
            // todo: lazy loading
        }

        public override string ToString() {
            return this.DisplayName + "->> " + this.ImageSource;
        }

    }
}
