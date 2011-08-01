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
        public event EventHandler<EventArgs<ProjectItemCodeDocument>> TokenizerUpdated;

        public event EventHandler<ProjectItemEventArgs> ItemAdded;

        public event EventHandler<ProjectItemEventArgs> ItemRemoved;

        /// <summary>
        /// Child bubbling up event
        /// </summary>
        public event EventHandler<ProjectItemEventArgs> ChildItemAdded;

        /// <summary>
        /// Child bubbling up event
        /// </summary>
        public event EventHandler<ProjectItemEventArgs> ChildItemRemoved;

        #endregion

        public ProjectItem(ProjectItem parent) {
            _parent = parent;
        }

        #region Public Properties

        /// <summary>
        /// Gets the Childern of this Item.
        /// DO NOT use this Property to make changes to the childern -> use Add()/Remove() to do so.
        /// This property just for performance reasons to not create swallow copys of the child list
        /// </summary>
        public List<ProjectItem> Children {
            get { return _children; }
        }

        /// <summary>
        /// Gets the Parent of this Item
        /// </summary>
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

        public abstract string FilePath {
            get;
        }

        public abstract string Name { get; set; }



        /// <summary>
        /// Traveles down the tree and finds all matching child items 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> FindAllItems<T>()
            where T : ProjectItem {
                foreach(var c in _children) {
                if(c is T)
                    yield return c as T;
            }
        }


        /// <summary>
        /// Traveles down the tree and finds all items in the childs, recursively
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> FindAllItemsRecursive<T>() 
            where T : ProjectItem {
            List<T> _items = new List<T>();
            foreach(var c in _children) {
                if(c is T)
                    _items.Add(c as T);
                _items.AddRange(c.FindAllItemsRecursive<T>());
            }
            return _items;
        }


        #endregion


        /// <summary>
        /// Request that this item is shown in the workspace view
        /// </summary>
        public virtual void ShowInWorkSpace() { }

        public virtual bool CanShow {
            get { return false; }
        }

        #region ProjectItem Access

        /// <summary>
        /// Add a Item to the childs of this Item
        /// </summary>
        /// <param name="item"></param>
        public void Add(ProjectItem item) {
            _children.Add(item);
            OnItemAdded(item);
        }

        /// <summary>
        /// Remove an Item to the childs of this Item
        /// </summary>
        /// <param name="item"></param>
        public void Remove(ProjectItem item) {
            _children.Remove(item);
            OnItemRemoved(item);
        }

        public IEnumerable<ProjectItem> GetAllItems() {
            return new List<ProjectItem>(_children);
        }

        public IEnumerable<T> GetAllItems<T>()
            where T : ProjectItem {
                return from i in _children
                   where i is T
                   select i as T;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs when the file Tokenizer has updated a single file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="codeProjectEventArgs"></param>
        protected virtual void OnTokenizerUpdated(object sender, EventArgs<ProjectItemCodeDocument> codeProjectEventArgs) {
            if(TokenizerUpdated != null) {
                TokenizerUpdated(this, codeProjectEventArgs);
            }
        }

        /// <summary>
        /// Occurs when an item was added to the childs of this item
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnItemAdded(ProjectItem item) {
            if(ItemAdded != null)
                ItemAdded(this, new ProjectItemEventArgs(item));
            item.TokenizerUpdated += OnTokenizerUpdated;
            item.ItemAdded += OnChildItemAdded;
            OnChildItemAdded(this, new ProjectItemEventArgs(item));
        }

        /// <summary>
        /// Occurs when an item was removed form the childs of this item
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnItemRemoved(ProjectItem item) {
            if(ItemRemoved != null)
                ItemRemoved(this, new ProjectItemEventArgs(item));
            item.TokenizerUpdated -= OnTokenizerUpdated;
            item.ItemAdded -= OnChildItemAdded;
            OnChildItemRemoved(this, new ProjectItemEventArgs(item));
        }

        /// <summary>
        /// Occurs when an item was added to this childs or to a child of child. 
        /// This is a fully Recursively Bubbling Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnChildItemAdded(object sender,ProjectItemEventArgs e) {
            if(ChildItemAdded != null) {
                ChildItemAdded(sender, e);
            }
        }

        /// <summary>
        /// Occurs when an item was removed from this childs or to a child of child. 
        /// This is a fully Recursively Bubbling Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnChildItemRemoved(object sender, ProjectItemEventArgs e) {
            if(ChildItemRemoved != null) {
                ChildItemRemoved(sender, e);
            }
        }


        #endregion

    }
}
