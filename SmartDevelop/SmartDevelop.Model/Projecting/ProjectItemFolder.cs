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
                OnNameChanged();
            }
        }

        public override bool Add(string fileToOpen) {
            string name = Path.GetFileName(fileToOpen);
            string targetLocation = Path.Combine(FilePath, name);

            if(!targetLocation.Equals(fileToOpen, StringComparison.InvariantCultureIgnoreCase)) {
                File.Copy(fileToOpen, targetLocation);
            }

            var file = new ProjectItemCodeDocument(name, this.Project.Language, this);

            if(file != null) {
                this.Add(file);
                file.ShowInWorkSpace();
                return true;
            } else
                return false;
        }

        public override bool CanAdd(string file) {
            return this.Project.Language.Extensions.Contains(Path.GetExtension(file));
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
