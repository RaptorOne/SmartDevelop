using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM;
using System.CodeDom;
using SmartDevelop.Model.CodeLanguages;
using Archimedes.Patterns.Services;
using SmartDevelop.Model.CodeContexts;
using Archimedes.Patterns.Utils;
using Archimedes.Patterns;
using System.Threading.Tasks;

namespace SmartDevelop.Model.Projecting
{
    /// <summary>
    /// Represents a smart Project
    /// </summary>
    public class SmartCodeProject : ProjectItem
    {
        #region Fields

        readonly CodeDOMService _domservice;
        readonly CodeLanguage _language;
        readonly ASTManager _ASTManager;

        string _name = "";
        string _projectPath;
        ProjectItemCodeDocument _startUpCodeDocument = null;
        SmartSolution _solution;

        List<ProjectItemCodeDocument> _codeDocuments = new List<ProjectItemCodeDocument>();

        #endregion

        #region Events

        /// <summary>
        /// Raised when a Documents requests to be shown in the editor
        /// </summary>
        public event EventHandler<EventArgs<ProjectItem>> RequestShowDocument;

        /// <summary>
        /// Raised when the StartUpCodeDocument has Changed
        /// </summary>
        public event EventHandler StartUpCodeDocumentChanged;

        /// <summary>
        /// Raised when a CodeDocument was added to this Project
        /// </summary>
        public event EventHandler CodeDocumentAdded;

        /// <summary>
        /// Raised when a CodeDocument was removed from this Project
        /// </summary>
        public event EventHandler CodeDocumentRemoved;

        #endregion

        #region Constructor

        public SmartCodeProject(string name, CodeLanguage language)
            : base(null) {
            ThrowUtil.ThrowIfNull(language);

            Name = name;
            _language = language;
            //_domservice = language.CreateDOMService(this);
            _ASTManager = language.CreateASTManager(this);
        }

        #endregion

        #region Properties

        ///// <summary>
        ///// Gets the DOM Service for this Project
        ///// </summary>
        //public CodeDOMService DOMService {
        //    get { return _domservice; }
        //}

        /// <summary>
        /// Gets the DOM Service for this Project
        /// </summary>
        public ASTManager ASTManager {
            get { return _ASTManager; }
        }

        /// <summary>
        /// Gets the Code-Language of this Project
        /// </summary>
        public CodeLanguage Language {
            get { return _language; }
        }


        /// <summary>
        /// Gets/Sets the Solution to which this Project belongs
        /// </summary>
        public SmartSolution Solution {
            get { return _solution; }
            internal set { _solution = value; }
        }

        /// <summary>
        /// Returns itself ;) 
        /// (children delegate their Project Get up to the parent root)
        /// </summary>
        public override SmartCodeProject Project {
            get {
                return this;
            }
        }

        /// <summary>
        /// Gets/Sets the StartUpCodeDocument of this Project - if any.
        /// </summary>
        public ProjectItemCodeDocument StartUpCodeDocument {
            get {
                return _startUpCodeDocument;
            }
            set {
                var old = _startUpCodeDocument;
                _startUpCodeDocument = value;

                if(old != null)
                    old.OnIsStartUpDocumentChanged();
                if(value != null)
                    value.OnIsStartUpDocumentChanged();

                OnStartUpCodeDocumentChanged();
            }
        }

        
        public IEnumerable<ProjectItemCodeDocument> CodeDocuments {
            get {
                return _codeDocuments;
            }
        }

        
        public override string Name {
            get { return _name; }
            set { _name = value; }
        }

        public void SetProjectFilePath(string newPath) {
            _projectPath = newPath;
        }

        public override string FilePath {
            get { return _projectPath; }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the given document in the editor
        /// </summary>
        /// <param name="documentToShow"></param>
        internal void ShowDocument(ProjectItem documentToShow){
            OnRequestShowDocument(documentToShow);
        }

        /// <summary>
        /// Runs this Project
        /// </summary>
        public virtual void Run() {
            
        }

        public virtual void QuickSaveAll() {
            var codefiles = this.FindAllItemsRecursive<ProjectItemCodeDocument>();
            foreach(var file in codefiles) {
                if(file.HasUnsavedChanges)
                    file.QuickSave();
            }
        }

        public virtual bool CanRun {
            get { return false; }
        }

        public virtual bool Close() {
            var codefiles = this.FindAllItemsRecursive<ProjectItemCodeDocument>();
            foreach(var file in codefiles) {
                file.RequestRemoveFromWorkspace();
                if(file.IsOnWorkspace)
                    return false;
            }
            return true;
        }

        #endregion

        #region Event Handlers

        protected virtual void OnStartUpCodeDocumentChanged() {
            if(StartUpCodeDocumentChanged != null)
                StartUpCodeDocumentChanged(this, EventArgs.Empty);
        }

        protected override void OnTokenizerUpdated(object sender, EventArgs<ProjectItemCodeDocument> codeProjectEventArgs) {
            //Task.Factory.StartNew(() => {
            //        _domservice.CompileTokenFileAsync(codeProjectEventArgs.Value, null);
            //    });
            base.OnTokenizerUpdated(sender, codeProjectEventArgs);
        }

        protected virtual void OnRequestShowDocument(ProjectItem documentToShow) {
            if(RequestShowDocument != null)
                RequestShowDocument(this, new EventArgs<ProjectItem>(documentToShow));
        }

        protected override void OnChildItemAdded(object sender, ProjectItemEventArgs e) {
            var codeDoc = e.Item as ProjectItemCodeDocument;
            if(codeDoc != null) {
                _codeDocuments.Add(codeDoc);
                OnCodeDocumentAdded(codeDoc);
            }
            base.OnChildItemAdded(sender, e);
        }

        protected override void OnChildItemRemoved(object sender, ProjectItemEventArgs e) {
            var codeDoc = e.Item as ProjectItemCodeDocument;
            if(codeDoc != null) {
                _codeDocuments.Remove(codeDoc);
                OnCodeDocumentRemoved(codeDoc);
            }
            base.OnChildItemRemoved(sender, e);
        }

        /// <summary>
        /// Occurs when a CodeDoucument was added to this project
        /// </summary>
        protected virtual void OnCodeDocumentAdded(ProjectItemCodeDocument doc) {
            if(CodeDocumentAdded != null)
                CodeDocumentAdded(this, new EventArgs<ProjectItemCodeDocument>(doc));

            _ASTManager.Add(doc);

        }

        /// <summary>
        /// Occurs when the CodeDoucument Collection has been changed
        /// </summary>
        protected virtual void OnCodeDocumentRemoved(ProjectItemCodeDocument doc) {
            if(CodeDocumentRemoved != null)
                CodeDocumentRemoved(this, new EventArgs<ProjectItemCodeDocument>(doc));

            _ASTManager.Remove(doc);
        }


        #endregion
    }



}
