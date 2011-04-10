using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.CodeCompleting;

namespace SmartDevelop.Model.Projecting
{

    public abstract class CodeProject
    {
        List<ProjectItem> _items = new List<ProjectItem>();

        public CodeProject() {
            Name = "New Project";
        }

        public void Add(ProjectItem item) {
            _items.Add(item);
        }

        public void Remove(ProjectItem item) {
            _items.Remove(item);
        }

        string _name="";
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        public abstract ICodeContextService CodeContextService { get; }
    }
}
