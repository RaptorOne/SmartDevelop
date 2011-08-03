using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SmartDevelop.Model.CodeLanguages
{
    public class CodeLanguageService : ICodeLanguageService
    {
        List<CodeLanguage> _languages = new List<CodeLanguage>();

        public CodeLanguageService() { LoadLanguages(); }

        protected virtual void LoadLanguages() {
            // here we can parse for language definition files
            // and load them. Plugins can additionaly use the Register Method to inject more languages here
        }

        /// <summary>
        /// Register a Language
        /// </summary>
        /// <param name="language"></param>
        public void Register(CodeLanguage language) {
            _languages.Add(language);
        }


        public CodeLanguage GetByExtension(string extension) {
            var languages = from l in _languages
                            where l.Extensions.Contains(extension)
                            select l;
            return languages.Any() ? languages.First() : null;
        }

        public CodeLanguage GetById(string languageid) {
            var languages = from l in _languages
                            where l.LanguageID == languageid
                            select l;
            return languages.Any() ? languages.First() : null;
        }

        public IEnumerable<CodeLanguage> GetAllLanguages() {
            return new List<CodeLanguage>(_languages);
        }


        public bool IsProjectFile(string filepath) {
           var ext = Path.GetExtension(filepath);
           if(ext == null)
               return false;
           var lang = from l in _languages
                      where ext.Equals(l.ProjectExtension, StringComparison.InvariantCultureIgnoreCase)
                      select l;

           return lang.Any();
        }

        public Projecting.SmartCodeProject LoadProjectFromFile(string filepath) {

            var ext = Path.GetExtension(filepath);

            var langs = from l in _languages
                       where ext.Equals(l.ProjectExtension, StringComparison.InvariantCultureIgnoreCase)
                       select l;

            if(langs.Any()) {
                var lang = langs.First();
                return lang.DeserializeFromFile(filepath);
            } else
                return null;
        }
    }
}
