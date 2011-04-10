using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using AvalonDock;
using ServicesCommon.WPF.WorkBenchServices.MessageBox;
using WPFCommon.ViewModels;
using ServicesCommon.WPF.WindowViewModelMapping;
using ServicesCommon.WPF.AvalonServices;
using ServicesCommon.WPF.WorkBenchServices.Loader2;
using WPFUserControls.Loaders;

namespace ServicesCommon.WPF.WorkBenchServices
{
    public class WorkBenchService : IWorkBenchService
    {
        #region Fields

        IWindowViewModelMappings _mappingService = ServiceLocator.Instance.Resolve<IWindowViewModelMappings>();
        IAvalonService _avalonService = ServiceLocator.Instance.Resolve<IAvalonService>();
        DockingManager _dockManager = null;

        #endregion

        public WorkBenchService() {
            HiddenContents = new ObservableCollection<DockableContent>();

            _avalonService.PrimaryDockManagerChanged += OnPrimaryDockManagerChanged;

            if (_avalonService.PrimaryDockManager != null)
                DockManager = _avalonService.PrimaryDockManager;
        }

        #region IWorkBenchService


        public Dispatcher STADispatcher {
            get { return DockManager.Dispatcher; }
        }

        public void ActivateMainWindow(){
            var wnd = Window.GetWindow(DockManager);
            if (wnd != null)
                wnd.Activate();
        }


        #region Show VM Content Methods

        public void ShowFloating(WorkspaceViewModel viewModel, SizeToContent sizeToContent = SizeToContent.WidthAndHeight, Size? windowSize = null) {
            if(viewModel == null)
                throw new ArgumentNullException("viewModel");

            var dockableContent = SetUpDockableContent(viewModel, viewModel.DisplayName);
            dockableContent.DockableStyle = DockableStyle.Floating;
            dockableContent.FloatingWindowSizeToContent = sizeToContent;
            dockableContent.HideOnClose = false;
            dockableContent.Closing += (s, e) => viewModel.OnRequestClose();


            if (windowSize.HasValue) {
                dockableContent.FloatingWindowSize = windowSize.Value;
            }
            dockableContent.ShowAsFloatingWindow(DockManager, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="sizeToContent"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public bool? ShowDialog(WorkspaceViewModel viewModel, SizeToContent sizeToContent = SizeToContent.WidthAndHeight, Size? windowSize = null) {
            if(viewModel == null)
                throw new ArgumentNullException("viewModel");

            var dockableContent = SetUpDockableContent(viewModel, viewModel.DisplayName);
            dockableContent.DockableStyle = DockableStyle.Floating;
            dockableContent.FloatingWindowSizeToContent = sizeToContent;
            dockableContent.HideOnClose = false;
            dockableContent.Closing += (s, e) => viewModel.OnRequestClose();
            if (windowSize.HasValue) {
                dockableContent.FloatingWindowSize = windowSize.Value;
            }
            return dockableContent.ShowAsDialoge(DockManager);
        }


        public void ShowDockedContent(WorkspaceViewModel viewModel, string title) {
            if(viewModel == null)
                throw new ArgumentNullException("viewModel");
            var dockableContent = SetUpDockableContent(viewModel, title);
            dockableContent.Show(DockManager);
        }

        public void ShowDockedDocument(WorkspaceViewModel viewModel, string title) {
            if(viewModel == null)
                throw new ArgumentNullException("viewModel");
            var dockableContent = SetUpDockableContent(viewModel, title);
            dockableContent.ShowAsDocument(DockManager);
        }

        #endregion

        #region MessageBox 

        /// <summary>
        /// Displays a simple MessageBox
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        public DialogWPFResult MessageBox(string message, string title, MessageBoxType type = MessageBoxType.None, MessageBoxWPFButton button = MessageBoxWPFButton.OK) {
            var vm = new MessageBoxViewModel(message, button);
            vm.MessageBoxImage = type;
            var view = vm.BuildView();

            var dc = CreateDockableContent(title, view, vm);
            dc.DockableStyle = DockableStyle.Floating;
            dc.FloatingWindowSizeToContent = SizeToContent.WidthAndHeight;
            dc.HideOnClose = false;
            dc.Closing += (s, e) => vm.OnRequestClose();
            dc.ShowAsDialoge(DockManager);

            return vm.DialogeResult;
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="messageBoxText">A string that specifies the text to display.</param>
        /// <param name="caption">A string that specifies the title bar caption to display.</param>
        /// <param name="button">
        /// A MessageBoxButton value that specifies which button or buttons to display.
        /// </param>
        /// <param name="icon">A MessageBoxImage value that specifies the icon to display.</param>
        /// <returns>
        /// A MessageBoxResult value that specifies which message box button is clicked by the user.
        /// </returns>
        [Obsolete("Use MessageBox")]
        public MessageBoxResult ShowMessageBox(
            string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icon) {
            return System.Windows.MessageBox.Show(Window.GetWindow(DockManager), messageBoxText, caption, button, icon);
        }
        #endregion

        #region Loader

        DockableContent _dcLoaderView;

        public void LoaderView(){
            LoaderView(true);
        }

        public void LoaderView(bool display) {

            if (_dcLoaderView == null && display) {
                var loaderView = new LoadingAnimation() { Width = 100, Height = 100 };
                var vm = new LoaderViewModel();
                _dcLoaderView = CreateDockableContent("Bitte Warten", loaderView, vm);
                _dcLoaderView.DockableStyle = DockableStyle.Floating;
                _dcLoaderView.FloatingWindowSizeToContent = SizeToContent.WidthAndHeight;
                //_dcLoaderView.IsCloseable = false;
                _dcLoaderView.HideOnClose = true;
            }

            if (display) {
                _dcLoaderView.ShowAsFloatingWindow(DockManager, false);
            } else {
                if (_dcLoaderView != null)
                    _dcLoaderView.Close();
            }
        }

        #endregion

        DockingManager DockManager {
            get { return _dockManager; }
            set {
                _dockManager = value;
            }
        }

        public ObservableCollection<DockableContent> HiddenContents {
            get;
            private set;
        }
        
        #endregion

        #region Helper Methods

        DockableContent SetUpDockableContent(WorkspaceViewModel viewModel, string title) {

            var dockableContent = FindContentByViewModel(viewModel) as DockableContent;
            if (dockableContent != null)
                return dockableContent;

            // Create New Content
            var viewType = _mappingService.GetViewTypeFromViewModelType(viewModel.GetType());
            var view = (FrameworkElement)Activator.CreateInstance(viewType);

            dockableContent = CreateDockableContent(title, view, viewModel);
            return dockableContent;
        }

        DockableContent CreateDockableContent(string title, FrameworkElement view, WorkspaceViewModel viewModel) {
            var dockableContent = new DockableContent()
            {
                Name = "myNewDialoge",
                Title = title
            };
            dockableContent.Content = view;
            dockableContent.DataContext = viewModel;
            viewModel.RequestClose += (s, e) => CloseParent(view, viewModel);
            return dockableContent;
        }

        ManagedContent FindContentByViewModel(WorkspaceViewModel vm) {
            ManagedContent content = null;
            content = DockManager.DockableContents.ToList().Find(x => ReferenceEquals(x.Content, vm));
            if (content != null)
                return content;

            content = DockManager.Documents.ToList().Find(x => ReferenceEquals(x.Content, vm));
            if (content != null)
                return content;

            foreach (var floatingWnd in DockManager.FloatingWindows) {
                var floatingPane = floatingWnd.Content as FloatingDockablePane;
                foreach (ManagedContent managedContent in floatingPane.Items) {
                    if (ReferenceEquals((managedContent.Content as FrameworkElement).DataContext, vm)) {
                        return managedContent;
                    }
                }
            }

            return content;
        }


        void CloseParent(FrameworkElement element, WorkspaceViewModel vm) {
            var window = Window.GetWindow(element);
            if(window != null){
                window.Closed += (a, b) => CleanUp(element,vm, window);
                try {
                    window.Close();
                } catch {
                    //
                }
            }
            ActivateMainWindow();
        }

        void CleanUp(FrameworkElement element, WorkspaceViewModel vm, Window window) {
            vm.RequestClose -= (s, e) => CloseParent(element, vm);
            window.Closed -= (a, b) => CleanUp(element, vm, window);
        }



        void UpdateHiddenContent(){
            if (DockManager != null) {
                var list = DockManager.DockableContents.Where(dc => dc.State == DockableContentState.Hidden).ToList();
                HiddenContents.Clear();
                foreach (var dc in list)
                    HiddenContents.Add(dc);
            }
        }
        #endregion

        #region Event Handlers

        void OnPrimaryDockManagerChanged(object sender, EventArgs e) {
            DockManager = _avalonService.PrimaryDockManager;
        }

        #endregion
    }
}
