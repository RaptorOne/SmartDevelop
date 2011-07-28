using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Errors;

namespace SmartDevelop.Model.Projecting
{
    public class SmartSolution
    {
        #region Fields

        SmartCodeProject _current;
        List<SmartCodeProject> _projects = new List<SmartCodeProject>();
        string _name = "Default Solution";
        IErrorService _errorService;

        #endregion

        #region Events

        public event EventHandler CurrentChanged;
        public event EventHandler<ProjectEventArgs> ProjectAdded;
        public event EventHandler<ProjectEventArgs> ProjectRemoved;

        #endregion

        public SmartSolution() {
            _errorService = new ErrorService();
        }

        #region Properties

        public IErrorService ErrorService {
            get { return _errorService; }
        }

        /// <summary>
        /// Active Project in this Solution
        /// </summary>
        public SmartCodeProject Current {
            get { return _current; }
            set { 
                _current = value;
                if(CurrentChanged != null)
                    CurrentChanged(this, EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// Name of this Solution
        /// </summary>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region Methods

        public void Add(SmartCodeProject p) {
            _projects.Add(p);
            p.Solution = this;
            if(ProjectAdded != null)
                ProjectAdded(this, new ProjectEventArgs(p));

            if(_current == null)
                Current = p;
        }

        public void Remove(SmartCodeProject p) {
            _projects.Remove(p);
            p.Solution = null;
            if(ProjectAdded != null)
                ProjectRemoved(this, new ProjectEventArgs(p));
        }

        public IEnumerable<SmartCodeProject> GetProjects() {
            return new List<SmartCodeProject>(_projects);
        }

        #endregion
    }


    public class ProjectEventArgs : EventArgs
    {
        readonly SmartCodeProject _p;

        public ProjectEventArgs(SmartCodeProject p){
            _p = p;
        }
        public SmartCodeProject Project{
            get{ return _p; }
        }
    }
}
