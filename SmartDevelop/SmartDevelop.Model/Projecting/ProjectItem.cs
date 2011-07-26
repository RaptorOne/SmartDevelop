using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns;

namespace SmartDevelop.Model.Projecting
{
    public abstract class ProjectItem
    {
        #region Fields

        ProjectItem _parent;
        List<ProjectItem> _children = new List<ProjectItem>();

        #endregion

        #region Events

        /// <summary>
        /// Raised when the background tokenizer has refreshed the tokens 
        /// This Event blubbles up
        /// </summary>
        public event EventHandler<EventArgs<ProjectItemCode>> TokenizerUpdated;

        public event EventHandler<ProjectItemEventArgs> ItemAdded;

        public event EventHandler<ProjectItemEventArgs> ItemRemoved;

        #endregion

        public ProjectItem(ProjectItem parent) {
            _parent = parent;
        }

        public List<ProjectItem> Children {
            get { return _children; }
        }

        public ProjectItem Parent {
            get { return _parent; }
        }

        public virtual SmartCodeProject Project {
            get {
                if(_parent != null)
                    return _parent.Project;
                else
                    return null;
            }
        }

        public abstract string Name { get; set; }


        #region ProjectItem Access

        public void Add(ProjectItem item) {
            Children.Add(item);
            if(ItemAdded != null)
                ItemAdded(this, new ProjectItemEventArgs(item));
            item.TokenizerUpdated += OnTokenizerUpdated;
        }


        public void Remove(ProjectItem item) {
            item.TokenizerUpdated -= OnTokenizerUpdated;
            Children.Remove(item);
            if(ItemRemoved != null)
                ItemRemoved(this, new ProjectItemEventArgs(item));
        }

        public IEnumerable<ProjectItem> GetAllItems() {
            return new List<ProjectItem>(Children);
        }

        public IEnumerable<T> GetAllItems<T>()
            where T : ProjectItem {
            return from i in Children
                   where i is T
                   select i as T;
        }

        #endregion

        /// <summary>
        /// Occurs when the file Tokenizer has updated a single file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="codeProjectEventArgs"></param>
        protected virtual void OnTokenizerUpdated(object sender, EventArgs<ProjectItemCode> codeProjectEventArgs) {
            if(TokenizerUpdated != null) {
                TokenizerUpdated(this, codeProjectEventArgs);
            }
        }

    }
}
