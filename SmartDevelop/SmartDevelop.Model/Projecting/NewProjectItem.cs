using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Projecting
{
    public abstract class NewProjectItem
    {

        public string Name {
            get;
            set;
        }

        public string FileName {
            get;
            set;
        }

        public string Image {
            get;
            set;
        }

        public abstract ProjectItem CreateNewItem(ProjectItem parent);
    }
}
