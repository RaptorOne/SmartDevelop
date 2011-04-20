using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using WPFCommon.ViewModels;
using SmartDevelop.ViewModel.DocumentFiles;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class SolutionExplorerVM : WorkspaceViewModel
    {
        List<TreeViewProjectItem> _solutions = new List<TreeViewProjectItem>();
        SmartSolution _smartSolution;
        TreeViewProjectItem _solutionRoot;

        public SolutionExplorerVM(SmartSolution solution){
           
            _smartSolution = solution;
            
            _solutionRoot = new TreeViewProjectItemSolutionFolder(_smartSolution, null);
            _solutions.Add(_solutionRoot);
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



        public IEnumerable<TreeViewProjectItem> Solutions {
            get { return _solutions; }
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
            var projecttree = new TreeViewProjectItemProject(p, _solutionRoot);
            foreach(var item in p.GetAllItems<ProjectItemCode>()) {
                projecttree.Children.Add(new TreeViewProjectItemCodeFile(item, projecttree));
            }
            return projecttree;
        }
    }
}
