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
        ProjectItemCodeDocument _activeDocument;

        List<SmartCodeProject> _projects = new List<SmartCodeProject>();
        string _name = "Default Solution";
        IErrorService _errorService;

        #endregion

        #region Events

        public event EventHandler OutputDataChanged;
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
        string _outputData;
        public string OutputData {
            get { return _outputData; }
        }

        public void ClearOutput(){
            _outputData = "";
            OnOutputDataChanged();
        }

        public void AddNewOutput(string newData) {
            _outputData = newData;
            OnOutputDataChanged();
        }

        public void AddNewOutputLine(string newData) {
            _outputData = newData + "\r\n";
            OnOutputDataChanged();
        }

        protected virtual void OnOutputDataChanged(){
            if(OutputDataChanged != null){
                OutputDataChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the current active Document
        /// </summary>
        public ProjectItemCodeDocument ActiveDocument {
            get {
                return _activeDocument;
            }
            protected set {
                _activeDocument = value;
            }
        }

        #endregion

        #region Methods



        public virtual void DocumentGotFocus(ProjectItemCodeDocument doc) {
            ActiveDocument = doc;
        }
        
        //public virtual void DocumentLostFocus(ProjectItemCodeDocument doc) {

        //}

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

        /// <summary>
        /// Closes this Solution.
        /// Invokes close on all child project of this solution.
        /// </summary>
        /// <returns></returns>
        public virtual bool Close() {
            foreach(var p in _projects) {
                if(!p.Close())
                    return false;
            }
            return true;
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
