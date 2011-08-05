using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.MVMV;
using System.Windows;

namespace SmartDevelop.ViewModel.InvokeCompletion
{
    public class InvokeParameter : ViewModelBase
    {
        bool _isActiveParameter;
        string _parameterText;
        string _parameterDescripton;


        public InvokeParameter(string parameterText, string parameterDescripton) {
            _parameterText = parameterText;
            _parameterDescripton = parameterDescripton;
        }

        public string ParameterName {
            get { return _parameterText.Trim(' ', ','); }
        }

        public string ParameterText {
            get { return _parameterText; }
            set { 
                _parameterText = value;
                OnPropertyChanged(() => ParameterText);
                OnPropertyChanged(() => ParameterName);
            }
        }

        public FontWeight ParameterFontWeight {
            get {
                return IsActiveParameter ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public string ParameterDescripton {
            get { return _parameterDescripton; }
            set { 
                _parameterDescripton = value;
                OnPropertyChanged(() => ParameterDescripton);
            }
        }

        public bool IsActiveParameter {
            get { return _isActiveParameter; }
            set {
                _isActiveParameter = value;
                OnPropertyChanged(() => IsActiveParameter);
                OnPropertyChanged(() => ParameterFontWeight);
            }
        }

    }
}
