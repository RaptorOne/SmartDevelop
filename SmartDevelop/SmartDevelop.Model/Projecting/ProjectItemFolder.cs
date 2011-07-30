using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SmartDevelop.Model.Projecting
{
    public class ProjectItemFolder : ProjectItem
    {
        string _name;
        public ProjectItemFolder(string name, ProjectItem parent) 
            : base(parent) {
                _name = name;
        }

        public override string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        public string FolderName {
            get { return _name; }
            set { _name = value; }
        }

        public override string FilePath {
            get {
                string directory = "";
                if(Parent != null) {
                    if(Path.GetExtension(Parent.FilePath) != "") {
                        directory = Path.GetDirectoryName(Parent.FilePath);
                    } else
                        directory = Parent.FilePath;
                }
                return Path.Combine(directory, this.Name);
            }
        }
    }
}
