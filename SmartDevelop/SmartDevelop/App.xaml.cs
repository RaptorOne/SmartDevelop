using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SmartDevelop.ViewModel.Main;
using ServicesCommon.WPF.WindowViewModelMapping;
using ServicesCommon;
using ServicesCommon.WPF.WorkBenchServices;
using SmartDevelop.ViewModel.DocumentFiles;
using SmartDevelop.View.DocumentFiles;
using ServicesCommon.WPF.AvalonServices;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;
using System.IO;

namespace SmartDevelop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        MainViewModel _mainVM;
        MainWindow _mainView;
        ServiceLocator _serviceLocator = ServiceLocator.Instance;

        #endregion

        #region App Bootstrapper

        void Application_Startup(object sender, StartupEventArgs e) {

            RegisterServices();

            _mainVM = new MainViewModel();

            _mainView = new MainWindow();
            _mainView.DataContext = _mainVM;
            _mainView.WindowState = WindowState.Maximized;
            _mainView.Show();

            _mainView.Loaded += OnMainWindowLoaded;
        }

        void OnMainWindowLoaded(object sender, EventArgs e) {
            _mainVM.SetDockManager(_mainView.DockManger);
        }


        #endregion

        #region Global Service Setup

        void RegisterServices() {
            _serviceLocator.RegisterSingleton<IWindowViewModelMappings, WindowViewModelMappings>();
            _serviceLocator.RegisterSingleton<IWorkBenchService, WorkBenchService>();
            _serviceLocator.RegisterSingleton<IAvalonService, AvalonService>();

            SetupViewModelViewMappings();
            LoadSyntaxHighlighner();
        }

        void SetupViewModelViewMappings() {
            var viewmodelMapping = _serviceLocator.Resolve<IWindowViewModelMappings>();

            viewmodelMapping.RegisterMapping(typeof(CodeFileViewModel), typeof(CodeDocumentView));
        }

        #endregion

        void LoadSyntaxHighlighner() {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;

            StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Syntax\\Autohotkey.xshd"));
            using(XmlReader reader = new XmlTextReader(sr)) {
                customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                    HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("AHK", new string[] { ".ahk" }, customHighlighting);
        }

    }
}
