using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model
{
    /// <summary>
    /// Handles last opened projects
    /// </summary>
    public class RecentList
    {
        List<string> _projectPaths = new List<string>();

        public event EventHandler RecentListChanged;

        public RecentList() { }
        public RecentList(IEnumerable<string> initial) { _projectPaths.AddRange(initial); }

        public void AddLatest(string p) {
            _projectPaths.Remove(p);
            _projectPaths.Insert(0, p);
            OnRecentListChanged();
        }

        public List<string> GetRecents() {
            return new List<string>(_projectPaths);
        }

        public void OnRecentListChanged() {
            if(RecentListChanged != null)
                RecentListChanged(this, EventArgs.Empty);
        }

    }
}
