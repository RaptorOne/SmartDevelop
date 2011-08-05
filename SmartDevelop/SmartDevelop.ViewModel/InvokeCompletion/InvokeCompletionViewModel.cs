using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.WPF.ViewModels;
using System.Collections.ObjectModel;

namespace SmartDevelop.ViewModel.InvokeCompletion
{
    public class InvokeCompletionViewModel : WorkspaceViewModel
    {
        #region Fields

        InvokeParameter _activeParameter;
        string _invokeDescription;
        string _prefix;
        string _sufix;

        #endregion


        public InvokeCompletionViewModel() {
            AllParameters = new ObservableCollection<InvokeParameter>();
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

        
        public InvokeParameter ActiveParameter {
            get {
                return _activeParameter;
            }
            set {
                _activeParameter = value;
                OnPropertyChanged(() => ActiveParameter);
            }
        }

    }
}
