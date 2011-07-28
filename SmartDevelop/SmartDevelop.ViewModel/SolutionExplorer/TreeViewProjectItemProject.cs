using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.ViewModel.DocumentFiles;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemProject : TreeViewProjectItem
    {
        readonly SmartCodeProject _project;
        public TreeViewProjectItemProject(SmartCodeProject project, TreeViewProjectItem parent)
            : base(project, parent) {
                ImageSource = @"../Images/project-folder.ico";
                _project = project;

                _project.RequestShowDocument += (s, e) => {
                        if(e.Value is ProjectItemCode) {
                            var codeVM = CodeFileViewModel.Create(e.Value as ProjectItemCode);
                            var showcmd = codeVM.ShowCommand;
                            if(showcmd.CanExecute(null))
                                showcmd.Execute(null);
                        }
                    };
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
