using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartDevelop.ViewModel.SolutionExplorer;

namespace SmartDevelop.View.Projecting
{
    /// <summary>
    /// Interaction logic for ProjectExplorerTreeView.xaml
    /// </summary>
    public partial class ProjectExplorerTreeView : UserControl
    {
        public ProjectExplorerTreeView() {
            InitializeComponent();
        }

        void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var itemVM = (TreeViewProjectItem)((TreeViewItem)e.Source).DataContext;
            var cc = itemVM.FocusCommand;
        }
    }
}
