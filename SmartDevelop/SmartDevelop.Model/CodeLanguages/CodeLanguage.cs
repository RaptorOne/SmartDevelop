using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.Model.DOM;
using SmartDevelop.Model.Projecting;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Folding;
using System.CodeDom;
using SmartDevelop.Model.DOM.Types;

namespace SmartDevelop.Model.CodeLanguages
{
    public abstract class CodeLanguage : IEquatable<CodeLanguage>
    {
        List<CodeKeyWord> _languageKeywords = new List<CodeKeyWord>();
        List<CodeTypeMember> _buildInMembers = new List<CodeTypeMember>();

        #region Constructor

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

        public virtual List<CodeTypeMember> BuildInMembers {
            get { return _buildInMembers; }
        }

        public abstract string[] Extensions {
                get;
        }

        /// <summary>
        /// Specifies that a self reference can be omitted
        /// eg:
        /// this.Call()
        /// Call()
        /// </summary>
        public bool SELFREF_CAN_BE_OMITTED = true;


        public abstract StringComparison NameComparisation { get; }

        public abstract Tokenizer CreateTokenizer(ITextSource source);

        public abstract CodeDOMService CreateDOMService(SmartCodeProject codeProject);

        public virtual ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition GetHighlighter() {
            return HighlightingManager.Instance.GetDefinition(this.LanguageID);
        }

        public abstract AbstractFoldingStrategy CreateFoldingStrategy(SmartDevelop.Model.Tokening.DocumentCodeSegmentService segmentService);

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
