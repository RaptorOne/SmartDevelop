using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.CodeLanguages
{
    public class CodeLanguage : IEquatable<CodeLanguage>
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


        #region IEquatable

        public bool Equals(CodeLanguage other) {
            if(other == null)
                return false;
            return other.LanguageID.Equals(this.LanguageID);
        }
        public override bool Equals(object obj) {
            return Equals(obj as CodeLanguage);
        }
        public override int GetHashCode() {
            return this.LanguageID.GetHashCode();
        }

        #endregion
    }
}
