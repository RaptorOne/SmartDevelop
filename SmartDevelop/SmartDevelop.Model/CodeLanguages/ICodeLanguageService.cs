using System;
namespace SmartDevelop.Model.CodeLanguages
{
    /// <summary>
    /// Service which manages all avaiable Languages of SmartDevelop
    /// </summary>
    public interface ICodeLanguageService
    {
        CodeLanguage GetById(string languageid);
        void Register(CodeLanguage language);
    }
}
