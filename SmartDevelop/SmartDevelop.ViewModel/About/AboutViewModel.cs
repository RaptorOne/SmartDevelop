using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.WPF.ViewModels;

namespace SmartDevelop.ViewModel.About
{
    public class AboutViewModel : WorkspaceViewModel
    {
        public AboutViewModel() {
            DisplayName = "About";
        }
    }
}
