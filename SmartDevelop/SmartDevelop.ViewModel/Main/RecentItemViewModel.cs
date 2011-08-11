using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.MVMV;
using System.Windows.Input;
using Archimedes.Patterns.WPF.Commands;
using System.IO;
using SmartDevelop.Model;

namespace SmartDevelop.ViewModel.Main
{

    public class RecentItemViewModel : ViewModelBase
    {
        readonly string _recentItemPath;

        public RecentItemViewModel(string recentItemPath) {
            _recentItemPath = recentItemPath;
        }

        public string Name {
            get {
                return Path.GetFileName(_recentItemPath);
            }
        }

        ICommand _openCommand;
        public ICommand OpenCommand {
            get {
                if(_openCommand == null) {
                    _openCommand = new RelayCommand(x => {
                        IDE.Instance.OpenFile(_recentItemPath);
                    });
                }
                return _openCommand;
            }
        }

    }
}
