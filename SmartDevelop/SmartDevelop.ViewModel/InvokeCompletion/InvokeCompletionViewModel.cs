using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.WPF.ViewModels;
using System.Collections.ObjectModel;
using SmartDevelop.Model.Projecting;
using SmartDevelop.ViewModel.DocumentFiles;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartDevelop.ViewModel.InvokeCompletion
{
    public class InvokeCompletionViewModel : WorkspaceViewModel
    {
        #region Fields

        InvokeParameter _activeParameter;
        string _invokeDescription;
        string _prefix;
        string _sufix;


        ProjectItemCodeDocument _document;
        CodeFileViewModel _documentVM;
        ToolTip _toolTip = new ToolTip();

        #endregion

        public InvokeCompletionViewModel(CodeFileViewModel documentVM) {
            _document = documentVM.CodeDocument;
            _documentVM = documentVM;

            _documentVM.Editor.TextArea.TextEntered += OnTextEntered;
            _documentVM.Editor.TextArea.KeyDown += (s, e) => {
                if(e.Key == Key.Escape)
                    this.CloseCommand.Execute(null);
            };

            _toolTip.Content = this;
            AllParameters = new ObservableCollection<InvokeParameter>();
        }


        public void Show() {
            this.IsShown = true;
        }

        #region Properties


        public InvokeParameter ActiveParameter {
            get {
                return _activeParameter;
            }
            set {
                _activeParameter = value;
                OnPropertyChanged(() => ActiveParameter);
            }
        }

        public string Prefix {
            get { return _prefix; }
            set { 
                _prefix = value;
                OnPropertyChanged(() => Prefix);
            }
        }

        public string Sufix {
            get { return _sufix; }
            set { 
                _sufix = value;
                OnPropertyChanged(() => Sufix);
            }
        }

        
        public string InvokeDescription {
            get { return _invokeDescription; }
            set { _invokeDescription = value; }
        }

        public ObservableCollection<InvokeParameter> AllParameters {
            get;
            protected set;
        }

        /// <summary>
        /// Gets/Sets if the current InvokeCompletion VM is shown
        /// </summary>
        public bool IsShown {
            get {
                return _toolTip.IsOpen;
            }
            set {
                _toolTip.IsOpen = value;
                if(!_toolTip.IsOpen)
                OnClosed();
            }
        }

        #endregion

        void OnTextEntered(object sender, TextCompositionEventArgs e) {
            char current = e.Text[0];

            


        }

        public override void OnRequestClose() {
            IsShown = false;
        }

        protected override void OnDispose() {
            _documentVM.Editor.TextArea.TextEntered -= OnTextEntered;
            base.OnDispose();
        }


    }
}
