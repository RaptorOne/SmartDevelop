using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns.WPF.ViewModels;
using System.Collections.ObjectModel;
using SmartDevelop.Model;

namespace SmartDevelop.ViewModel.Main
{
    public class StartUpPageViewModel : WorkspaceViewModel
    {
        public StartUpPageViewModel() {
            AllRecentItems = new ObservableCollection<RecentItemViewModel>();
            IDE.Instance.Recent.RecentListChanged += (s, e) => {
                UpdateAllRecents();
            };
            UpdateAllRecents();
        }


        void UpdateAllRecents() {
            AllRecentItems.Clear();
            IDE.Instance.Recent.GetRecents().ToList().ForEach(x => AllRecentItems.Add(new RecentItemViewModel(x)));
        }

        public ObservableCollection<RecentItemViewModel> AllRecentItems {
            get;
            protected set;
        }
    }
}
