using System;
namespace SmartDevelop.Model.Errors
{
    public interface IErrorService
    {
        event EventHandler<Archimedes.Patterns.EventArgs<ErrorItem>> ErrorAdded;
        event EventHandler<Archimedes.Patterns.EventArgs<ErrorItem>> ErrorRemoved;

        void Add(ErrorItem error);
        void Remove(ErrorItem error);

        void ClearAllErrorsFrom(SmartDevelop.Model.Projecting.ProjectItemCode codeDocument);

        System.Collections.Generic.IEnumerable<ErrorItem> GetAllErrors();
        System.Collections.Generic.IEnumerable<ErrorItem> GetErrorsFromDocument(SmartDevelop.Model.Projecting.ProjectItemCode codeDocument);
        System.Collections.Generic.IEnumerable<ErrorItem> GetErrorsFromProject(SmartDevelop.Model.Projecting.SmartCodeProject codeProject);
        
    }
}
