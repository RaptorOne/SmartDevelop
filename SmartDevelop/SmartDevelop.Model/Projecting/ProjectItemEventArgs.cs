using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Projecting
{
    public class ProjectItemEventArgs : EventArgs
    {
        readonly ProjectItem _p;

        public ProjectItemEventArgs(ProjectItem p) {
            _p = p;
        }
        public ProjectItem Item {
            get { return _p; }
        }
    }
}
