using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.CodeLanguages
{
    public class CodeLanguageService : ICodeLanguageService
    {
        List<CodeLanguage> _languages = new List<CodeLanguage>();

        public CodeLanguageService() { LoadLanguages(); }

        protected virtual void LoadLanguages() {
            // here we can parse for language definition files
            // and load them. Plugins can additionaly use the Register Method to inject more languages here

            // for debuging purposes, we define here inline some AHK CodeLanguage:
            var lang = new CodeLanguage("ahk-dialect");
            lang.LanguageKeywords.AddRange(new CodeKeyWord[]
                { 
                    new CodeKeyWord("if"), new CodeKeyWord("else"),
                    new CodeKeyWord("class"), new CodeKeyWord("var"), new CodeKeyWord("new"), new CodeKeyWord("this"), new CodeKeyWord("extends"),
                    new CodeKeyWord("return"), new CodeKeyWord("break"), new CodeKeyWord("continue"),
                    new CodeKeyWord("global"), new CodeKeyWord("static"), new CodeKeyWord("local")
                });
            this.Register(lang);
        }

        /// <summary>
        /// Register a Language
        /// </summary>
        /// <param name="language"></param>
        public void Register(CodeLanguage language) {
            _languages.Add(language);
        }

        public CodeLanguage GetById(string languageid) {
            var languages = from l in _languages
                            where l.LanguageID == languageid
                            select l;
            return languages.Any() ? languages.First() : null;
        }
    }
}
