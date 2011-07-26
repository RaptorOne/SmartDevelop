using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemFolder : TreeViewProjectItem
    {
        ProjectItemFolder _folder;

        public TreeViewProjectItemFolder(ProjectItemFolder folder, TreeViewProjectItem parent)
            : base(folder, parent) {
            _folder = folder;
            ImageSource = @"../Images/folder.ico";
        }

    }
}
