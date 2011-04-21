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

            _mainVM = new MainViewModel(new SmartSolution());

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
            _serviceLocator.RegisterSingleton<IWorkBenchService, AvalonWorkBenchService>();
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
            IHighlightingDefinition customHighlighting;

            // ToDo: Load syntaxfiles dynamically....

            #region AHK

            var sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Syntax\\Autohotkey.xshd"));
            using(var reader = new XmlTextReader(sr)) {
                customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                    HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
            HighlightingManager.Instance.RegisterHighlighting("AHK", new string[] { ".ahk" }, customHighlighting);

            #endregion

            #region IA

            Brush b = new SolidColorBrush(Colors.GreenYellow);
            var brushchen = new HighlightingBrushStaticColor(b);

            sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Syntax\\IronAHK.xshd"));
            using(var reader = new XmlTextReader(sr)) {
                customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                    HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            //Add custom but static highligning rules
            // test example -->
            customHighlighting.MainRuleSet.Rules.Add(new HighlightingRule()
            {
                Color = new HighlightingColor()
                {
                    Foreground = brushchen
                },
                Regex = new System.Text.RegularExpressions.Regex("SubStr")
            });
            //<----
            HighlightingManager.Instance.RegisterHighlighting("IA", new string[] { ".ia" }, customHighlighting);

            #endregion
        }

    }
}
