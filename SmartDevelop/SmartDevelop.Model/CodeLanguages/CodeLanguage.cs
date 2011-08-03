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
using SmartDevelop.Model.Tokenizing;
using System.IO;
using Archimedes.Patterns.Serializing;
using SmartDevelop.Model.Projecting.Serializer;

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
        List<PreProcessorDirective> _languageDirectives = new List<PreProcessorDirective>();

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

        /// <summary>
        /// Human readable name of this language
        /// </summary>
        public string Name {
            get;
            set;
        }

        static string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartDevelop");

        public static string AppSettingsFolder {
            get {
                return appData;
            }
        }

        public virtual List<CodeKeyWord> LanguageKeywords {
            get { return _languageKeywords; }
        }
        

        public virtual List<PreProcessorDirective> LanguageDirectives {
            get { return _languageDirectives; }
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

        public abstract string ProjectExtension {
            get;
        }

        /// <summary>
        /// Specifies that a self reference can be omitted
        /// eg:
        /// this.Call()
        /// Call()
        /// </summary>
        public bool SELFREF_CAN_BE_OMITTED = true;

        /// <summary>
        /// Specifies that in a Project a Code Document can be specified as startup file
        /// </summary>
        public bool SUPPORTS_STARTUP_CODEDOCUMENT = false;

        public abstract StringComparison NameComparisation { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new Tokenizer for this Language
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public abstract Tokenizer CreateTokenizer(ProjectItemCodeDocument codeitem, ITextSource source);

        /// <summary>
        /// Creates a new DomService for this Language
        /// </summary>
        /// <param name="codeProject"></param>
        /// <returns></returns>
        public abstract CodeDOMService CreateDOMService(SmartCodeProject codeProject);

        /// <summary>
        /// Creates a new DOMService for the given Document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public abstract CodeDocumentDOMService CreateDOMService(ProjectItemCodeDocument document);


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


        public virtual IEnumerable<EditorDocumentExtension> CreateExtensionsForCodeDocument(TextEditor texteditor, ProjectItemCodeDocument projectitem) {
            return new List<EditorDocumentExtension>();
        }

        public abstract void ShowLanguageSettings();

        /// <summary>
        ///Gets the project Templates for this Language
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract IEnumerable<ProjectTemplate> GetProjectTemplates();

        /// <summary>
        /// Creates a new SmartCodeProject for this Language
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract SmartCodeProject Create(string displayname, string name, string location);

        #endregion

        #region Serializer

        /// <summary>
        /// Serializes the project to a file
        /// </summary>
        /// <param name="project"></param>
        /// <param name="fileName"></param>
        public virtual void SerializeToFile(SmartCodeProject project, string fileName) {

            if(string.IsNullOrEmpty(fileName)) {
                fileName = Path.Combine(project.FilePath);
            }

            var serializableProjectTree = SProjectItem.Build(project);
            SerializerHelper.SerializeObjectToFile(serializableProjectTree, fileName);
        }

        /// <summary>
        /// Loads a project from a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual SmartCodeProject DeserializeFromFile(string fileName) {
            var p = SerializerHelper.DeserializeObjectFromFile<SSmartCodeProject>(fileName);
            return p.CreateProject(Path.GetDirectoryName(fileName)) as SmartCodeProject;
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

        public abstract ASTManager CreateASTManager(SmartCodeProject project);

        public abstract IEnumerable<NewProjectItem> GetAvaiableItemsForNew(ProjectItem _folder);
    }
}
