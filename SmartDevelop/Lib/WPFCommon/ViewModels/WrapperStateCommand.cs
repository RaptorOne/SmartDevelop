using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandPattern.States;
using CommandPattern;
using System.Windows;

namespace WPFCommon.ViewModels
{
    public class WrapperStateCommand : WrapperCommand
    {
        public event EventHandler StateChanged;

        public WrapperStateCommand(IStateCommand<bool> command)
            : base(command) {
                command.StateChanged += OnStateChanged;
        }

        public bool State {
            get { return StateCommand.State; }
        }

        public Visibility StateV {
            get { return StateCommand.State ? Visibility.Visible : Visibility.Collapsed; }
        }

        IStateCommand<bool> StateCommand {
            get { return base.Command as IStateCommand<bool>; }
        }

        #region Event Handlers

        void OnStateChanged(object sender, EventArgs e) {
            OnPropertyChanged(
                () => State,
                () => StateV);
            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        #endregion

    }
}
