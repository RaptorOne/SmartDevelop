using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.Model.Errors
{
    /// <summary>
    /// Error Service Implementation
    /// </summary>
    public class ErrorService : SmartDevelop.Model.Errors.IErrorService
    {
        List<ErrorItem> _errors = new List<ErrorItem>();
        
        #region Events

        public event EventHandler<EventArgs<ErrorItem>> ErrorAdded;
        public event EventHandler<EventArgs<ErrorItem>> ErrorRemoved;

        #endregion

        #region Public Methods

        public void Add(ErrorItem error) {
            _errors.Add(error);
            OnErrorAdded(error);
        }

        public void Remove(ErrorItem error) {
            _errors.Remove(error);
            OnErrorRemoved(error);
        }

        public void ClearAllErrorsFrom(ProjectItemCodeDocument codeDocument) {
            foreach(var error in GetErrorsFromDocument(codeDocument)) {
                this.Remove(error);
            }
        }

        public void ClearAllErrorsFrom(ProjectItemCodeDocument codeDocument, ErrorSource source) {
            foreach(var error in GetErrorsFromDocument(codeDocument)) {
                if(error.Source == source)
                    this.Remove(error);
            }
        }

        public void ClearAllErrorsFrom(ErrorSource source) {
            foreach(var error in GetAllErrors()) {
                if(error.Source == source)
                    this.Remove(error);
            }
        }

        public IEnumerable<ErrorItem> GetErrorsFromProject(SmartCodeProject codeProject) {
            return _errors.FindAll(x => x.CodeItem.Project.Equals(codeProject));
        }

        public IEnumerable<ErrorItem> GetErrorsFromDocument(ProjectItemCodeDocument codeDocument) {
            return _errors.FindAll(x => x.CodeItem.Equals(codeDocument));
        }

        public IEnumerable<ErrorItem> GetAllErrors() {
            return new List<ErrorItem>(_errors);
        }


        #endregion

        #region Event Handlers

        protected virtual void OnErrorAdded(ErrorItem item){
            if(ErrorAdded != null)
                ErrorAdded(this, new EventArgs<ErrorItem>(item));
        }

        protected virtual void OnErrorRemoved(ErrorItem item) {
            if(ErrorRemoved != null)
                ErrorRemoved(this, new EventArgs<ErrorItem>(item));
        }

        #endregion







    }
}
