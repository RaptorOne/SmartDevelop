using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Projecting
{
    public abstract class ProjectItem
    {
        ProjectItem _parent;
        List<ProjectItem> _children = new List<ProjectItem>();

        public ProjectItem() { }

        public ProjectItem(ProjectItem parent) {
            _parent = parent;
        }

        public List<ProjectItem> Children {
            get { return _children; }
        }

        public ProjectItem Parent {
            get { return _parent; }
        }


        public abstract string Name { get; set; }
    }
}
