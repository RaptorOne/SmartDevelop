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


namespace SmartDevelop.Model.Projecting
{


    /// <summary>
    /// Represents a single Codefile
    /// </summary>
    public class ProjectItemCode : ProjectItem
    {
        #region Fields

        readonly TextDocument _codedocument;

        readonly ICodeLanguageService _languageService = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
        CodeLanguage _language;
        Tokenizer _tokenizer;
        DocumentCodeSegmentService _codeSegmentService;
        bool _documentdirty = false;
        bool _isModified = false;

        string _name;

        #endregion

        #region Events

        public event EventHandler<EventArgs<int>> RequestTextPosition;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler RequestTextInvalidation;

        public event EventHandler HasUnsavedChangesChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Attempts to load a code file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ProjectItemCode FromFile(string filepath, ProjectItem parent) {
            if(!File.Exists(filepath))
                return null;

            var language = ServiceLocator.Instance.Resolve<ICodeLanguageService>().GetByExtension(Path.GetExtension(filepath));
            ProjectItemCode newp = new ProjectItemCode(language, parent);
            newp.FilePath = filepath;
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

        public ProjectItemCode(CodeLanguage languageId, ProjectItem parent) 
            : base(parent) {

            if(languageId == null)
                throw new ArgumentNullException("languageId");

            _codedocument = new TextDocument();
            Encoding = Encoding.UTF8;
            _language = languageId; 
            _codedocument.Changed += OnCodedocumentChanged;

            _codeSegmentService = new DocumentCodeSegmentService(this);
            _tokenizer = _language.CreateTokenizer(this, _codedocument);

            _tokenizer.FinishedSucessfully += (s, e) => {
                _codeSegmentService.Reset(_tokenizer.GetSegmentsSnapshot());
                //// notify that we have a new token base to parse
                OnTokenizerUpdated(this, new EventArgs<ProjectItemCode>(this));
                //OnRequestTextInvalidation();
            };

            DispatcherTimer tokenUpdateTimer = new DispatcherTimer();
            tokenUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
            tokenUpdateTimer.Tick += CheckUpdateTokenRepresentation;
            tokenUpdateTimer.Start();
        }

        #endregion

        /// <summary>
        /// Request that this document is shown in the editor view
        /// </summary>
        public void ShowDocument() {
            var p = this.Project;
            if(p == null)
                throw new NotSupportedException("Must be atached to a project to use this Method");
            p.ShowDocument(this);
        }


        public void SetDocumentPosition(int offset) {
            if(RequestTextPosition != null)
                RequestTextPosition(this, new EventArgs<int>(offset));
        }

        /// <summary>
        /// This Method ensures that the Tokenizer has been updated with the current changes in this document.
        /// </summary>
        public void EnsureTokenizerHasWorked() {
            while(_tokenizer.IsBusy) {
                Thread.Sleep(1);
            }
            UpdateTokenizer();
            while(true) {
                if(!_tokenizer.IsBusy)
                    break;
                Thread.Sleep(10);
            }
        }

        public void EnsureASTIsUpdated() {
            Project.DOMService.EnsureIsUpdated();
        }



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
                    Filter = "Code Files|*" + CodeLanguage.Extensions.First(),
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
        public void Save(string fileName) {
            ThrowUtil.ThrowIfNull(fileName);
            if(FilePath != fileName) {
                FilePath = fileName;
            }
            SaveAs(fileName);

            HasUnsavedChanges = false;
        }


        public void SaveAs(string fileName) {
            ThrowUtil.ThrowIfNull(fileName);

            using(FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                Save(fs);
            }
        }

        #endregion

        #region Event Handlers

        void OnCodedocumentChanged(object sender, EventArgs e){
            _documentdirty = true;
            HasUnsavedChanges = true;
        }

        void CheckUpdateTokenRepresentation(object sender, EventArgs e) {
            UpdateTokenizer();
        }

        void UpdateTokenizer() {
            if(_documentdirty && !_tokenizer.IsBusy) {
                _documentdirty = false;
                _tokenizer.TokenizeAsync();
            }
        }

        void OnRequestTextInvalidation(){
            if(RequestTextInvalidation != null)
                    RequestTextInvalidation(this, EventArgs.Empty);
        }

        #endregion

        #region Public Properties

        public string FilePath {
            get;
            set;
        }


        public override string Name {
            get {
                if(string.IsNullOrEmpty(FilePath)) {
                    return _name ?? "unknown";
                } else {
                    return Path.GetFileName(FilePath);
                }
            }
            set { _name = value; }
        }

        public bool HasUnsavedChanges { 
            get { return _isModified; } 
            protected set {
                if(_isModified == value)
                    return;
                _isModified = value;
                if(HasUnsavedChangesChanged != null)
                    HasUnsavedChangesChanged(this, EventArgs.Empty);
            } 
        }

        public CodeLanguage CodeLanguage {
            get { return _language; }
        }

        public TextDocument Document {
            get { return _codedocument; }
        }

        public DocumentCodeSegmentService SegmentService {
            get { return _codeSegmentService; }
        }

        #endregion

        public override string ToString() {
            return string.Format("{0} ({1})", this.Name);
        }

    }
}
