using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.CodeLanguage
{
    public class CodeLanguage
    {
        List<CodeKeyWord> _languageKeywords = new List<CodeKeyWord>();

        #region Constructor

        public CodeLanguage() {

        }

        public CodeLanguage(string id) {
            LanguageID = id;
        }
        
        #endregion

        /// <summary>
        /// Identifier for this language
        /// </summary>
        public string LanguageID {
            get;
            set;
        }

        public virtual List<CodeKeyWord> LanguageKeywords {
            get { return _languageKeywords; }
        }
    }
}
