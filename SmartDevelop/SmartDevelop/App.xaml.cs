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
using SmartDevelop.AHK.AHKv1.Projecting;
using SmartDevelop.ViewModel.Projecting;
using SmartDevelop.View.Projecting;
using SmartDevelop.ViewModel.About;
using SmartDevelop.View.About;
using System.Linq;
using SmartDevelop.Model;

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
        IDE _ide = IDE.Instance;
        #endregion

        #region App Bootstrapper

        void Application_Startup(object sender, StartupEventArgs e) {

            RegisterServices();

            _mainVM = new MainViewModel(IDE.Instance);

            _mainView = new MainWindow();
            _mainView.DataContext = _mainVM;
            _mainView.WindowState = WindowState.Maximized;
            _mainView.Loaded += OnMainWindowLoaded;
            _mainView.Closing += (s, ex) => {
                if(!ex.Cancel && _ide.CurrentSolution != null) {
                    if(!_ide.CurrentSolution.Close()) {
                        ex.Cancel = true;
                    }
                }
                };
            _mainView.Show();
        }

        void OnMainWindowLoaded(object sender, EventArgs e) {
            _mainVM.SetDockManager(_mainView.DockManger);
            //AddDemoSolution();
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

        /// <summary>
        /// Setup the mappings for ViewModel <--> View
        /// </summary>
        void SetupViewModelViewMappings() {
            var viewmodelMapping = _serviceLocator.Resolve<IWindowViewModelMappings>();

            viewmodelMapping.RegisterMapping(typeof(CodeFileViewModel), typeof(CodeDocumentView));
            viewmodelMapping.RegisterMapping(typeof(AddItemViewModel), typeof(AddItemView));
            viewmodelMapping.RegisterMapping(typeof(AboutViewModel), typeof(AboutView));
            viewmodelMapping.RegisterMapping(typeof(CreateNewProjectVM), typeof(CreateNewProjectView));

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

        #region Demo Code


        void AddDemoSolution() {

            var tempProjectPath = Path.Combine(Path.GetTempPath(), "SmartDevelop", "Demo");
            if(!Directory.Exists(tempProjectPath)) {
                Directory.CreateDirectory(tempProjectPath);
            }

            var serviceLang = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
            var language = serviceLang.GetById("ahk-v1.1");

            var solution = new SmartSolution();
            _ide.CurrentSolution = solution;

            var languageTemplate = language.GetProjectTemplates().Last();

            SmartCodeProjectAHK demoProject = languageTemplate.Create("Demo Project", "demoproject",tempProjectPath) as SmartCodeProjectAHK;
            solution.Add(demoProject);
            language.SerializeToFile(demoProject, null);
        }


    }
        #endregion
}
