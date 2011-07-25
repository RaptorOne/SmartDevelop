using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemSolutionFolder : TreeViewProjectItem
    {
        SmartSolution _solution;


        public TreeViewProjectItemSolutionFolder(SmartSolution solution, TreeViewProjectItem parent)
            : base(parent) {
                _solution = solution;
                ImageSource = @"../Images/blocks.ico";
        }

        public override string DisplayName {
            get {
                return _solution.Name;
            }
            set {
                _solution.Name = value;
            }
        }

        public override object DomainModel {
            get {
                return _solution;
            }
        }

    }
}
