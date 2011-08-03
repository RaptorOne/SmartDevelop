using System;
using System.Collections.Generic;
using SmartDevelop.Model.Projecting;
namespace SmartDevelop.Model.CodeLanguages
{
    /// <summary>
    /// Service which manages all avaiable Languages of SmartDevelop
    /// </summary>
    public interface ICodeLanguageService
    {
        /// <summary>
        /// Get the language by its id
        /// </summary>
        /// <param name="languageid"></param>
        /// <returns></returns>
        CodeLanguage GetById(string languageid);

        /// <summary>
        /// Get the language by extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        CodeLanguage GetByExtension(string extension);

        /// <summary>
        /// Returns all registered CodeLanguages
        /// </summary>
        /// <returns></returns>
        IEnumerable<CodeLanguage> GetAllLanguages();


        bool IsProjectFile(string filepath);

        SmartCodeProject LoadProjectFromFile(string filepath);


        /// <summary>
        /// Register a language
        /// </summary>
        /// <param name="language"></param>
        void Register(CodeLanguage language);
    }
}
