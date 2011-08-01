using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.ViewModel.DocumentFiles;
using Archimedes.Patterns.WPF.ViewModels;
using System.Windows.Input;
using Archimedes.Patterns.WPF.Commands;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class SolutionExplorerVM : WorkspaceViewModel
    {
        List<TreeViewProjectItem> _solutions = new List<TreeViewProjectItem>();
        SmartSolution _smartSolution;
        TreeViewProjectItem _solutionRoot;

        public SolutionExplorerVM(SmartSolution solution){
           
            _smartSolution = solution;
            
            _solutionRoot = new TreeViewProjectItemSolutionFolder(_smartSolution);
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

        void OnProjectAdded(object sender, ProjectEventArgs e) {
            AddProject(e.Project);
        }

        void OnProjectRemoved(object sender, ProjectEventArgs e) {

        }

        void AddProject(SmartCodeProject p) {
            _solutionRoot.Children.Add(LoadProject(p));
        }


        #endregion




        TreeViewProjectItem LoadProject(SmartCodeProject p){
            var projecttree = new TreeViewProjectItemProject(p, _solutionRoot);

            return projecttree;
        }
    }
}
