using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.AHK.AHKv1.Projecting
{
    public class ProjectItemFolderSTdLib : ProjectItemFolder
    {
        string _folderPath;

        public ProjectItemFolderSTdLib(string displayName, string folderPath, ProjectItem parent) 
            : base(displayName, parent) {
                _folderPath = folderPath;
        }


        public override string FilePath {
            get {
                return _folderPath;
            }
        }

    }
}
