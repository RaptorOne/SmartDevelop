using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Archimedes.Patterns.Services;
using Archimedes.Services.WPF.AvalonDockService;
using Archimedes.Services.WPF.WindowViewModelMapping;
using Archimedes.Services.WPF.WorkBenchServices;
using ICSharpCode.AvalonEdit.Highlighting;
using SmartDevelop.Model.Highlighning;
using SmartDevelop.Model.Projecting;
using SmartDevelop.View.DocumentFiles;
using SmartDevelop.ViewModel.DocumentFiles;
using SmartDevelop.ViewModel.Main;
using Archimedes.Patterns.MVMV.ViewModels.PoolCache;
using SmartDevelop.View.Main;
using SmartDevelop.Model.CodeLanguages;
using SmartDevelop.AHK.AHKv1;
using SmartDevelop.Model.StdLib;

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
        SmartSolution _solution;

        #endregion

        #region App Bootstrapper

        void Application_Startup(object sender, StartupEventArgs e) {

            RegisterServices();
            _solution = new SmartSolution();
            _mainVM = new MainViewModel(_solution);

            _mainView = new MainWindow();
            _mainView.DataContext = _mainVM;
            _mainView.WindowState = WindowState.Maximized;
            _mainView.Loaded += OnMainWindowLoaded;
            _mainView.Show();
        }

        void OnMainWindowLoaded(object sender, EventArgs e) {
            _mainVM.SetDockManager(_mainView.DockManger);
            AddDemoProject();
        }

        void AddDemoProject() {
            _solution.Add(StdLibLoader.LoadStLib());
        }


        #endregion

        #region Global Service Setup

        void RegisterServices() {
            _serviceLocator.RegisterSingleton<IWindowViewModelMappings, WindowViewModelMappings>();
            _serviceLocator.RegisterSingleton<IWorkBenchService, AvalonWorkBenchService>();
            _serviceLocator.RegisterSingleton<IAvalonService, AvalonService>();
            _serviceLocator.RegisterSingleton<IViewModelPoolService, ViewModelPoolService>();
            _serviceLocator.RegisterSingleton<ICodeLanguageService, CodeLanguageService>();
            

            SetupViewModelViewMappings();
            LoadLanguages();
        }

        void SetupViewModelViewMappings() {
            var viewmodelMapping = _serviceLocator.Resolve<IWindowViewModelMappings>();

            viewmodelMapping.RegisterMapping(typeof(CodeFileViewModel), typeof(CodeDocumentView));
        }

        #endregion

        void LoadLanguages() {

            var langserv = _serviceLocator.Resolve<ICodeLanguageService>();
            // here we actually load all plugins dynamically
            // and serach the assemblys for classes which implement "CodeLanguage"
            // to register them to the known languages

            // for the time being, we staticaly link to the plugin assembly 
            // and register the languages manually
            langserv.Register(new CodeLanguageAHKv1());

        }

    }
}
