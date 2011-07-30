using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using System.Windows.Input;
using SmartDevelop.ViewModel.DocumentFiles;
using System.Windows;
using Archimedes.Patterns.WPF.Commands;

namespace SmartDevelop.ViewModel.SolutionExplorer
{
    public class TreeViewProjectItemCodeFile : TreeViewProjectItem
    {
        readonly ProjectItemCodeDocument _codeitem;
        
        public TreeViewProjectItemCodeFile(ProjectItemCodeDocument codeitem, TreeViewProjectItem parent)
            : base(codeitem, parent) {
                _codeitem = codeitem;
                ImageSource = @"../Images/ironAHK.ico";

            _codeitem.IsStartUpDocumentChanged += (s,e) => OnPropertyChanged(() => IsStartUpDocument);
            _codeitem.IsStartUpDocumentChanged += (s,e) => OnPropertyChanged(() => ItemFontWeight);
        }

        #region Properties


        public FontWeight ItemFontWeight {
            get {
                return IsStartUpDocument ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public bool IsStartUpDocument {
            get { 
                return _codeitem.IsStartUpDocument; 
            }
            set {  
                _codeitem.IsStartUpDocument = value;
            }
        }

        #endregion

        #region Commands

        #region View Code Command

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

        #region Set Document as current StartUp Command

        ICommand _setAsStartUpDocumentCommand;

        public ICommand SetAsStartUpDocumentCommand {
            get {

                if(_setAsStartUpDocumentCommand == null) {
                    _setAsStartUpDocumentCommand = new RelayCommand(
                        x => {
                            this.IsStartUpDocument = true;
                        }, x => {
                            return _codeitem.CodeLanguage.SUPPORTS_STARTUP_CODEDOCUMENT && !IsStartUpDocument;
                        });
                }

                return _setAsStartUpDocumentCommand;
            }
        }
        #endregion

        #endregion

    }
}
