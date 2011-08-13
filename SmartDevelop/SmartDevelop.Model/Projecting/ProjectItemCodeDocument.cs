using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.TokenizerBase.IA;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using SmartDevelop.Model.Tokening;
using SmartDevelop.Model.CodeLanguages;
using Archimedes.Patterns.Services;
using SmartDevelop.TokenizerBase;
using Archimedes.Patterns;
using System.Windows.Forms;
using Archimedes.Patterns.Utils;
using SmartDevelop.Model.Tokenizing;
using System.ComponentModel;
using SmartDevelop.Model.DOM;


namespace SmartDevelop.Model.Projecting
{


    /// <summary>
    /// Represents a single Codefile
    /// </summary>
    public class ProjectItemCodeDocument : ProjectItem
    {
        #region Fields

        readonly TextDocument _codedocument;

        readonly ICodeLanguageService _languageService = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
        CodeLanguage _language;
        Tokenizer _tokenizer;
        DocumentCodeSegmentService _codeSegmentService;
        CodeDocumentDOMService _ast;

        volatile bool _documentdirty = false;
        volatile bool _documentTokenizerDirty = false;
        volatile bool _documentASTDirty = false;

        bool _hasUnsavedChanges = false;
        string _name;

        #endregion

        #region Events

        /// <summary>
        /// Raised when a text location is requestet
        /// </summary>
        public event EventHandler<EventArgs<int>> RequestTextPosition;

        /// <summary>
        /// Raised when the TextRender has to update its content unexpected.
        /// </summary>
        public event EventHandler RequestTextInvalidation;

        /// <summary>
        /// Raised when this Document wants it self removed from the editor workspace
        /// </summary>
        public event EventHandler RequestClosing;

        /// <summary>
        /// Raised when the HasUnsavedChanges Property has changed
        /// </summary>
        public event EventHandler HasUnsavedChangesChanged;

        /// <summary>
        /// Raised when the IsStartUpDocument Property has changed
        /// </summary>
        public event EventHandler IsStartUpDocumentChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Attempts to load a code file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ProjectItemCodeDocument FromFile(string filepath, ProjectItem parent) {
            if(!File.Exists(filepath))
                return null;

            var language = ServiceLocator.Instance.Resolve<ICodeLanguageService>().GetByExtension(Path.GetExtension(filepath));
            ProjectItemCodeDocument newp = new ProjectItemCodeDocument(Path.GetFileName(filepath), language, parent);
            newp.OverrideFilePath = filepath;
            try
            {
                using(StreamReader sr = new StreamReader(filepath))
                {
                    newp.Document.Text = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                // to do
            }
            newp.HasUnsavedChanges = false;
            newp.Document.UndoStack.ClearAll();
            return newp;
        }

        public ProjectItemCodeDocument(CodeLanguage languageId, ProjectItem parent)
            : this(null, languageId, parent) { }

        public ProjectItemCodeDocument(string name, CodeLanguage languageId, ProjectItem parent) 
            : base(parent) {

            if(languageId == null)
                throw new ArgumentNullException("languageId");

            this.Name = name;

            _codedocument = new TextDocument();
            Encoding = Encoding.UTF8;
            _language = languageId; 
            _codedocument.Changed += OnCodedocumentChanged;

            _codeSegmentService = new DocumentCodeSegmentService(this);
            _tokenizer = _language.CreateTokenizer(this, _codedocument);

            _tokenizer.FinishedSucessfully += (s, e) => {
                _codeSegmentService.Reset(_tokenizer.GetSegmentsSnapshot());

                if(!_documentdirty)
                    _documentTokenizerDirty = false;
                
                OnTokenizerUpdated(this, new EventArgs<ProjectItemCodeDocument>(this));
            };

            _ast = languageId.CreateDOMService(this);

            _ast.Updated += (s, e) => {
                if(!_documentdirty && !_documentTokenizerDirty)
                    _documentASTDirty = false;
            };


            DispatcherTimer tokenUpdateTimer = new DispatcherTimer();
            tokenUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
            tokenUpdateTimer.Tick += CheckUpdateTokenRepresentation;
            tokenUpdateTimer.Start();

            ReloadDocument();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Request that this document is shown in the editor view
        /// </summary>
        public override void ShowInWorkSpace() {
            var p = this.Project;
            
            if(p == null)
                throw new NotSupportedException("Must be atached to a project to use this Method");
            p.ShowDocument(this);
        }

        /// <summary>
        /// Determites if this CodeDocument currently can be shown
        /// </summary>
        public override bool CanShow {
            get { return true; }
        }

        /// <summary>
        /// Set the Document position in the Editor and ensures it is visible
        /// </summary>
        /// <param name="offset">The desired position</param>
        public void SetDocumentPosition(int offset) {
            if(RequestTextPosition != null)
                RequestTextPosition(this, new EventArgs<int>(offset));
        }


        /// <summary>
        /// This Method ensures that the Tokenizer has been updated with the current changes in this document.
        /// This Method will block the calling Thread.
        /// 
        /// Warning: Calling this Method may result in a notable delay.
        /// Warning: Calling this Method on the Standard Thread will likely cause a deadlock, as internaly Std Dispatcher Invokes will occur.
        /// </summary>
        public void EnsureTokenizerHasWorked() {

            _tokenizer.WaitTillCompleted();

            if(_documentdirty && Project.ASTManager.UpdateAtWill && !_tokenizer.IsBusy) {
                _documentdirty = false;
                var tsk = _tokenizer.TokenizeAsync();
                tsk.Wait();
            }
        }

        /// <summary>
        /// This Metthod ensures that the AST is updated. 
        /// Warning: Calling this Method may result in a notable delay
        /// </summary>
        public bool EnsureASTIsUpdated(int maxtimeout) {
            return AST.WaitUntilUpdated(maxtimeout);
        }

        /// <summary>
        /// Reloads the document from the file.
        /// </summary>
        public virtual void ReloadDocument() {
            if(File.Exists(FilePath)) {
                using(StreamReader sr = new StreamReader(FilePath)) {
                    _codedocument.Text = sr.ReadToEnd();
                }
                this.HasUnsavedChanges = false;
                this.Document.UndoStack.ClearAll();
            }
        }

        /// <summary>
        /// Attempts to close this Document
        /// </summary>
        /// <returns></returns>
        public virtual void RequestRemoveFromWorkspace() {
            OnRequestRemoveFromWorkspace();
        }


        #endregion

        #region Save the document


        /// <summary>
        /// Saves the document at the current filepath. If no path is specified, the user will be prompted to select an appriopate path.
        /// </summary>
        public void QuickSave() {
            if(!string.IsNullOrWhiteSpace(this.FilePath))
                this.Save(this.FilePath);
            else {

                var saver = new SaveFileDialog()
                {
                    FileName = this.Name,
                    Filter = "Code Files|*" + (CodeLanguage.Extensions.Any() ? CodeLanguage.Extensions.First() : ""),
                    Title = "Select a Script File",
                    AddExtension = true
                };

                if(saver.ShowDialog() == DialogResult.OK) {
                    this.Save(saver.FileName);
                }
            }
        }


        /// <summary>
        /// Saves the text to the stream.
        /// </summary>
        /// <remarks>
        /// This method sets <see cref="HasUnsavedChanges"/> to false.
        /// </remarks>
        public void Save(Stream stream) {
            if(stream == null)
                throw new ArgumentNullException("stream");
            StreamWriter writer = new StreamWriter(stream, Encoding);
            writer.Write(_codedocument.Text);
            writer.Flush();
            // do not close the stream
            this.HasUnsavedChanges = false;
        }


        public Encoding Encoding {
            get;
            set;
        }

        /// <summary>
        /// Saves the text to the file and updates filepath and HasUnsavedChanges.
        /// </summary>
        public void Save(string fileName = null) {
            ThrowUtil.ThrowIfNull(fileName);
            if(FilePath != fileName) {
                //FilePath = fileName;
            }
            SaveAs(FilePath);
            HasUnsavedChanges = false;
        }


        public void SaveAs(string fileName) {
            ThrowUtil.ThrowIfNull(fileName);
            if(!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            using(FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                Save(fs);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Tokenizer Dirty State. This will be true if the Tokenizer is not updated with the latest changes made to the documents
        /// </summary>
        public bool IsTokenizerDirty {
            get { return _documentTokenizerDirty; }
        }

        /// <summary>
        /// Gets the AST Dirty State. This will be true if the AST is not updated with the latest changes made to the documents
        /// </summary>
        public bool IsASTDirty {
            get { return _documentASTDirty; }
        }

        /// <summary>
        /// Used for non sulution/project files
        /// </summary>
        public string OverrideFilePath { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the Filepath of the CodeDocument
        /// </summary>
        public override string FilePath {
            get {
                if(string.IsNullOrEmpty(OverrideFilePath)) {
                    string directory = "";
                    if(Parent != null) {
                        if(Path.GetExtension(Parent.FilePath) != "") {
                            directory = Path.GetDirectoryName(Parent.FilePath);
                        } else
                            directory = Parent.FilePath;
                    }
                    return Path.Combine(directory, this.Name);
                } else {
                    return OverrideFilePath;
                }
            }
        }

        /// <summary>
        /// Gets/Sets the Name of this CodeDocument
        /// </summary>
        public override string Name {
            get {
                if(string.IsNullOrEmpty(OverrideFilePath)) {
                    if(string.IsNullOrEmpty(_name)) {
                        return "unknown";
                    } else
                        return _name;
                } else
                    return Path.GetFileName(OverrideFilePath);
            }
            set {
                var oldPath = FilePath;
                _name = value;
                //update filesystem
                try {
                    File.Move(oldPath, FilePath);
                } catch(Exception e) {
                    //todo: notify the user about fail
                }
                OnNameChanged();
            }
        }

        /// <summary>
        /// Gets the Save-State of the Document
        /// </summary>
        public bool HasUnsavedChanges {
            get { return _hasUnsavedChanges; }
            protected set {
                if(_hasUnsavedChanges == value)
                    return;
                _hasUnsavedChanges = value;
                if(HasUnsavedChangesChanged != null)
                    HasUnsavedChangesChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the COdeLanguage of this CodeDocument
        /// </summary>
        public CodeLanguage CodeLanguage {
            get { return _language; }
        }

        /// <summary>
        /// Gets the underlying TextDocument which this CodeDocument is representing
        /// </summary>
        public TextDocument Document {
            get { return _codedocument; }
        }

        /// <summary>
        /// Gets the Segment Service
        /// </summary>
        public DocumentCodeSegmentService SegmentService {
            get { return _codeSegmentService; }
        }


        public Tokenizer Tokenizer {
            get { return _tokenizer; }
        }
        
        /// <summary>
        /// Gets the AST
        /// </summary>
        public CodeDocumentDOMService AST {
            get { return _ast; }
        }

        /// <summary>
        /// Gets if this Document is the StartupDocument.
        /// Note: Startupdocuments arn't supported by all CodeLanguages
        /// </summary>
        public bool IsStartUpDocument {
            get {
                return (this.Project != null && this.Project.StartUpCodeDocument == this);
            }
            set {
                if(value && !IsStartUpDocument) {
                    if(this.Project != null) {
                        this.Project.StartUpCodeDocument = this;
                    }
                } else if(!value && this.Equals(this.Project.StartUpCodeDocument)) {
                    this.Project.StartUpCodeDocument = null;
                }
            }
        }

        /// <summary>
        /// Gets/Sets if this document currently is on the editor workspace
        /// </summary>
        public bool IsOnWorkspace { get; set; }


        #endregion

        /// <summary>
        /// Marks the document as dirty sets all appriorate flags
        /// </summary>
        void Dirty() {
            _documentdirty = true;
            _documentTokenizerDirty = true;
            _documentASTDirty = true;
        }

        #region Event Handlers


        protected virtual void OnRequestRemoveFromWorkspace() {
            if(RequestClosing != null)
                RequestClosing(this, EventArgs.Empty);
        }


        /// <summary>
        /// Occurs when the IsStartUpDocument has changed
        /// </summary>
        internal virtual void OnIsStartUpDocumentChanged() {
            if(IsStartUpDocumentChanged != null)
                IsStartUpDocumentChanged(this, EventArgs.Empty);
        }

        void OnCodedocumentChanged(object sender, EventArgs e){
            Dirty();
            HasUnsavedChanges = true;
        }




        void CheckUpdateTokenRepresentation(object sender, EventArgs e) {
            UpdateTokenizer();
        }

        void UpdateTokenizer() {
            if(_documentdirty && Project.ASTManager.UpdateAtWill && !_tokenizer.IsBusy) {
                _documentdirty = false;
                _tokenizer.TokenizeAsync();
            }
        }

        void OnRequestTextInvalidation(){
            if(RequestTextInvalidation != null)
                    RequestTextInvalidation(this, EventArgs.Empty);
        }

        #endregion


        public override string ToString() {
            return string.Format("{0}, IsStartUp: {1}", this.Name, this.IsStartUpDocument);
        }

        #region IEditor

        public string Text {
            get { return _codedocument.Text; }
        }

        public void Select(int start, int length) {
            throw new NotImplementedException();
        }

        public void Replace(int start, int length, string ReplaceWith) {
            _codedocument.Replace(start, length, ReplaceWith);
        }

        public void BeginChange() {
            _codedocument.BeginUpdate();
        }

        public void EndChange() {
            _codedocument.EndUpdate();
        }

        #endregion
    }
}
