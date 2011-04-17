using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using WPFCommon.ViewModels;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class SolutionExplorerVM : WorkspaceViewModel
    {
        SmartSolution _smartSolution;
        TreeViewProjectItem _solutionRoot;
        public SolutionExplorerVM(SmartSolution solution){
           
            _smartSolution = solution;
            _solutionRoot = new TreeViewProjectItem() 
            { 
                DisplayName = "Default SmartSolution",
                ImageSource = @"/Images/blocks.ico"
            };
            _smartSolution.ProjectAdded += OnProjectAdded;
            _smartSolution.ProjectRemoved += OnProjectRemoved;

            Import();
        }
        void Import() {
            foreach(var p in _smartSolution.GetProjects()) {
                AddProject(p);
            }
        }

        public TreeViewProjectItem SolutionRoot {
            get { return _solutionRoot; }
        }


        #region Event Handlers

        void AddProject(SmartCodeProject p) {
            _solutionRoot.Children.Add(LoadProject(p));
        }

        void OnProjectAdded(object sender, ProjectEventArgs e) {
            AddProject(e.Project);
        }

        void OnProjectRemoved(object sender, ProjectEventArgs e) {

        }

        #endregion


        TreeViewProjectItem LoadProject(SmartCodeProject p){
            var tree = new TreeViewProjectItem(_solutionRoot);
            tree.DisplayName = p.Name;
            tree.ImageSource = @"../Images/project-folder.ico";
            foreach(var item in p.GetAllItems<ProjectItemCode>()) {
                tree.Children.Add(new TreeViewProjectItem(tree) { DisplayName = item.Name, ImageSource = @"../Images/ironAHK.ico" });
            }

            return tree;
        }
    }
}
