using System;
using System.Windows;
using WPFCommon.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using ServicesCommon.WPF.WorkBenchServices.MessageBox;


namespace ServicesCommon.WPF.WorkBenchServices
{
    /// <summary>
    /// Window Managing Service
    /// </summary>
    public interface IWorkBenchService
    {
        void ActivateMainWindow();
        void ShowFloating(WorkspaceViewModel viewModel, SizeToContent sizeToContent = SizeToContent.WidthAndHeight, Size? windowSize = null);
        bool? ShowDialog(WorkspaceViewModel viewModel, SizeToContent sizeToContent = SizeToContent.WidthAndHeight, Size? windowSize = null);
        void ShowDockedContent(WorkspaceViewModel viewModel, string title);
        void ShowDockedDocument(WorkspaceViewModel viewModel, string title);

        void LoaderView();
        void LoaderView(bool display);

        DialogWPFResult MessageBox(string message, string title, MessageBoxType type = MessageBoxType.None, MessageBoxWPFButton button = MessageBoxWPFButton.OK);
        
        /*
        MessageBoxResult ShowMessageBox(
            string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icon);*/


        Dispatcher STADispatcher { get; }
    }
}
