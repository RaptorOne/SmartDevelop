using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using System.Windows.Input;
using Archimedes.Patterns.WPF.Commands;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemSolutionFolder : TreeViewProjectItem
    {
        SmartSolution _solution;


        public TreeViewProjectItemSolutionFolder(SmartSolution solution)
            : base() {
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

        #region Commands

        ICommand _addNewProjectCommand;

        public ICommand AddNewProjectCommand {
            get {
                if(_addNewProjectCommand == null) {

                    _addNewProjectCommand = new RelayCommand(
                        x => x.Equals(x),
                        x => {
                            return false;
                        });
                }

                return _addNewProjectCommand;
            }
        }

        #endregion

    }
}
