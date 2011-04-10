using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFCommon.ViewModels;
using CommandPattern;

namespace WPFCommon.CommandViewModels
{
    public class UnDoCommandViewModel : ViewModelBase
    {
        CommandUndo _command;

        public UnDoCommandViewModel(CommandUndo command) {
            _command = command;
        }

        public string Name {
            get {
                return _command.Name != "" ? _command.Name : _command.GetType().Name;
            }
        }

        public string Description {
            get { return _command.Description; }
        }


    }
}
