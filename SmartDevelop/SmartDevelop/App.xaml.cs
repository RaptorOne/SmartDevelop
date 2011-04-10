using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SmartDevelop.ViewModel.Main;

namespace SmartDevelop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainViewModel _mainVM;

        #region App Bootstrapper

        void Application_Startup(object sender, StartupEventArgs e) {

            _mainVM = new MainViewModel();

            var mainView = new MainWindow();
            mainView.DataContext = _mainVM;
            mainView.WindowState = WindowState.Maximized;
            mainView.Show();
        }

        #endregion
    }
}
