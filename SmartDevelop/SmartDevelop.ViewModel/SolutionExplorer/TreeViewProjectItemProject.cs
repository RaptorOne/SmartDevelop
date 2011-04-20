using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemProject : TreeViewProjectItem
    {
        readonly SmartCodeProject _project;
        public TreeViewProjectItemProject(SmartCodeProject project, TreeViewProjectItem parent)
            : base(parent) {
                ImageSource = @"../Images/project-folder.ico";
                _project = project; 
        }

        public override string DisplayName {
            get {
                return _project.Name;
            }
            set {
                _project.Name = value;
            }
        }


    }
}
