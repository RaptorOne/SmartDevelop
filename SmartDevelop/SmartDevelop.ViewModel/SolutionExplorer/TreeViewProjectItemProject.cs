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


                _project.ItemAdded += (s, e) => {
                        var codeitem = e.Project as ProjectItemCode;
                        if(codeitem != null) {
                            Add(codeitem);
                        }
                    };

                ImportExisting();
        }

        void ImportExisting() {
            foreach(var item in _project.GetAllItems<ProjectItemCode>()) {
                Add(item);
            }
        }

        public override object DomainModel {
            get {
                return _project;
            }
        }


        void Add(ProjectItemCode codeItem) {
            this.Children.Add(new TreeViewProjectItemCodeFile(codeItem, this));
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
