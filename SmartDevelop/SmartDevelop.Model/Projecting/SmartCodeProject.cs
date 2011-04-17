using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.CodeCompleting;

namespace SmartDevelop.Model.Projecting
{

    public class SmartCodeProject
    {
        List<ProjectItem> _items = new List<ProjectItem>();

        public event EventHandler<ProjectItemEventArgs> ItemAdded;
        public event EventHandler<ProjectItemEventArgs> ItemRemoved;


        public SmartCodeProject(string name) {
            Name = name;
        }

        public void Add(ProjectItem item) {
            _items.Add(item);
            if(ItemAdded != null)
                ItemAdded(this, new ProjectItemEventArgs(item));
        }

        public void Remove(ProjectItem item) {
            _items.Remove(item);
            if(ItemRemoved != null)
                ItemRemoved(this, new ProjectItemEventArgs(item));
        }

        public IEnumerable<ProjectItem> GetAllItems() {
            return new List<ProjectItem>(_items);
        }

        public IEnumerable<T> GetAllItems<T>()
            where T : ProjectItem {
            return from i in _items
                   where i is T
                   select i as T;
        }

        string _name="";
        public string Name {
            get { return _name; }
            set { _name = value; }
        }
    }

    public class ProjectItemEventArgs : EventArgs
    {
        readonly ProjectItem _p;

        public ProjectItemEventArgs(ProjectItem p) {
            _p = p;
        }
        public ProjectItem Project {
            get { return _p; }
        }
    }

}
