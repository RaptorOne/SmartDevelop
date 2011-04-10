using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Projecting
{
    public class ProjectManager
    {
        CodeProject _current;
        List<CodeProject> _projects = new List<CodeProject>();

        public event EventHandler CurrentChanged;

        public ProjectManager() { }

        public CodeProject Current {
            get { return _current; }
            set { 
                _current = value;
                if(CurrentChanged != null)
                    CurrentChanged(this, EventArgs.Empty);
            }
        }

        public void Add(CodeProject p) {
            _projects.Add(p);
            if(_current == null)
                Current = p;
        }

        public void Remove(CodeProject p) {
            _projects.Remove(p);
        }

    }
}
