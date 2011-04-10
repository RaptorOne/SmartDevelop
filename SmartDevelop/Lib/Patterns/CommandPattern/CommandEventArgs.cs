using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandPattern
{
    public class CommandEventArgs : EventArgs
    {
        readonly ICommandUndo _command;
        public CommandEventArgs(ICommandUndo command) {
            _command = command;
        }

        public ICommandUndo Command {
            get { return _command; }
        }
    }
}
