using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.WPF.ViewModels;
using SmartDevelop.AHK.AHKv1;

namespace SmartDevelop.AHK.ViewModel
{
    public class CodeLanguageSettingsViewModel : WorkspaceViewModel
    {
        AHKSettings _settings;
        public CodeLanguageSettingsViewModel(AHKSettings settings) {
            _settings = settings;
        }

        public string InterpreterPath {
            get { return _settings.InterpreterPath; }
            set { _settings.InterpreterPath = value; }
        }

        public string StdLibName {
            get { return _settings.StdLibName; }
            set { _settings.StdLibName = value; }
        }

        public string LocalLibName {
            get { return _settings.LocalLibName; }
            set { _settings.LocalLibName = value; }
        }

        public string HelpFilePath {
            get { return _settings.HelpFilePath; }
            set { _settings.HelpFilePath = value; }
        }


        //public override void OnClosed() {
        //    _settings.Save();
        //    base.OnClosed();
        //}
        public override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            _settings.Save();
            base.OnClosing(e);
        }
    }
}
