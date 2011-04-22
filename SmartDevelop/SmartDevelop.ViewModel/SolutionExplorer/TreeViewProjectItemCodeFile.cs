using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using System.Windows.Input;
using SmartDevelop.ViewModel.DocumentFiles;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemCodeFile : TreeViewProjectItem
    {
        readonly ProjectItemCode _codeitem;
        public TreeViewProjectItemCodeFile(ProjectItemCode codeitem, TreeViewProjectItem parent) 
            : base(parent){
                _codeitem = codeitem;
                ImageSource = @"../Images/ironAHK.ico";
        }

        #region Properties

        public override string DisplayName {
            get {
                return _codeitem.Name;
            }
            set {
                _codeitem.Name = value;
            }
        }

        #endregion

        #region Commands


        CodeFileViewModel _codeVM;

        public ICommand ViewCodeCommand {
            get {
                if(_codeVM == null) {
                    _codeVM = CodeFileViewModel.Create(_codeitem);
                }
                return _codeVM.ShowCommand;
            }
        }

        #endregion

    }
}
