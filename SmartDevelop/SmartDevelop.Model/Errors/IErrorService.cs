using System;
namespace SmartDevelop.Model.Errors
{
    public interface IErrorService
    {
        #region

        event EventHandler<Archimedes.Patterns.EventArgs<ErrorItem>> ErrorAdded;

        event EventHandler<Archimedes.Patterns.EventArgs<ErrorItem>> ErrorRemoved;

        #endregion

        /// <summary>
        /// Add an Error
        /// </summary>
        /// <param name="error"></param>
        void Add(ErrorItem error);

        /// <summary>
        /// Remove an Error
        /// </summary>
        /// <param name="error"></param>
        void Remove(ErrorItem error);

        void ClearAllErrorsFrom(SmartDevelop.Model.Projecting.ProjectItemCodeDocument codeDocument);
        void ClearAllErrorsFrom(SmartDevelop.Model.Projecting.ProjectItemCodeDocument codeDocument, ErrorSource source);


        /// <summary>
        /// Clear all Errors which have the given Source
        /// </summary>
        /// <param name="source"></param>
        void ClearAllErrorsFrom(ErrorSource source);

        System.Collections.Generic.IEnumerable<ErrorItem> GetAllErrors();
        System.Collections.Generic.IEnumerable<ErrorItem> GetErrorsFromDocument(SmartDevelop.Model.Projecting.ProjectItemCodeDocument codeDocument);
        System.Collections.Generic.IEnumerable<ErrorItem> GetErrorsFromProject(SmartDevelop.Model.Projecting.SmartCodeProject codeProject);
        
    }
}
