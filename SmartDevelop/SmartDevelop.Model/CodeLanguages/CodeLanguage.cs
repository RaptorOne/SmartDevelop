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
using SmartDevelop.Model.CodeLanguages.Extensions;
using ICSharpCode.AvalonEdit;

namespace SmartDevelop.Model.CodeLanguages
{
    /// <summary>
    /// Abstract baseclase for a code language definition
    /// </summary>
    public abstract class CodeLanguage : IEquatable<CodeLanguage>
    {
        #region Fields

        List<CodeKeyWord> _languageKeywords = new List<CodeKeyWord>();
        List<CodeTypeMember> _buildInMembers = new List<CodeTypeMember>();
        List<EditorDocumentExtension> _documentExtensions = new List<EditorDocumentExtension>();

        #endregion

        #region Constructor

        public CodeLanguage(string id) {
            LanguageID = id;
        }
        
        #endregion

        #region Properties

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

        public virtual List<EditorDocumentExtension> CodeDocumentExtensions {
            get { return _documentExtensions; }
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

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new Tokenizer for this Language
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public abstract Tokenizer CreateTokenizer(ITextSource source);

        /// <summary>
        /// Creates a new DomService for this Language
        /// </summary>
        /// <param name="codeProject"></param>
        /// <returns></returns>
        public abstract CodeDOMService CreateDOMService(SmartCodeProject codeProject);

        /// <summary>
        /// Creates a new Syntax Highlighter for this Language
        /// </summary>
        /// <returns></returns>
        public virtual ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition GetHighlighter() {
            return HighlightingManager.Instance.GetDefinition(this.LanguageID);
        }

        /// <summary>
        /// Creates a new Foldingstrategy for this language
        /// </summary>
        /// <param name="segmentService"></param>
        /// <returns></returns>
        public abstract AbstractFoldingStrategy CreateFoldingStrategy(SmartDevelop.Model.Tokening.DocumentCodeSegmentService segmentService);


        public virtual IEnumerable<EditorDocumentExtension> CreateExtensionsForCodeDocument(TextEditor texteditor, ProjectItemCode projectitem) {
            return new List<EditorDocumentExtension>();
        }

        #endregion

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
