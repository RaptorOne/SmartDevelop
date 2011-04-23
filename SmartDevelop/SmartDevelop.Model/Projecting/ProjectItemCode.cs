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


namespace SmartDevelop.Model.Projecting
{
    public enum CodeItemType
    {
        None = 0,
        IA = 1,
        AHK = 2,
        AHK_L = 4,
        AHK2 = 8
    }

    /// <summary>
    /// Represents a single Codefile
    /// </summary>
    public class ProjectItemCode : ProjectItem
    {
        #region Fields

        readonly TextDocument _codedocument;
        CodeItemType _type = CodeItemType.None;
        SimpleTokinizerIA _tokenizer;
        CodeTokenService _codeTokenService;
        bool _documentdirty = false;
        bool _isModified = false;

        #endregion

        /// <summary>
        /// Raised when the background tokenizer has refreshed the tokens 
        /// </summary>
        public event EventHandler TokenizerUpdated;

        #region Constructors

        /// <summary>
        /// Attempts to load a code file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ProjectItemCode FromFile(string filepath, ProjectItem parent) {
            if(!File.Exists(filepath))
                return null;

            ProjectItemCode newp = new ProjectItemCode(CodeItemTypeFromExtension(Path.GetExtension(filepath)), parent);
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
            return newp;
        }     

        ProjectItemCode(CodeItemType type, ProjectItem parent) 
            : base(parent) {
            _codedocument = new TextDocument();
            Encoding = Encoding.UTF8;

            _codedocument.Changed += OnCodedocumentChanged;

            _codeTokenService = new CodeTokenService(this);
            _tokenizer = new SimpleTokinizerIA(_codedocument);

            _tokenizer.Finished += (s, e) => {
                _codeTokenService.Reset(_tokenizer.GetSegmentsSnapshot());
                if(TokenizerUpdated != null) {
                    TokenizerUpdated(this, EventArgs.Empty);
                }
            };
            _type = type;

            DispatcherTimer tokenUpdateTimer = new DispatcherTimer();
            tokenUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
            tokenUpdateTimer.Tick += CheckUpdateTokenRepresentation;
            tokenUpdateTimer.Start();
        }

        #endregion

        public string FilePath {
            get;
            set;
        }

        public override string Name {
            get {
                if(string.IsNullOrEmpty(FilePath)) {
                    return "New Item";
                } else {
                    return Path.GetFileName(FilePath);
                }
            }
            set { }
        }

        #region Save the document

        /// <summary>
        /// Saves the text to the stream.
        /// </summary>
        /// <remarks>
        /// This method sets <see cref="IsModified"/> to false.
        /// </remarks>
        public void Save(Stream stream) {
            if(stream == null)
                throw new ArgumentNullException("stream");
            StreamWriter writer = new StreamWriter(stream, Encoding);
            writer.Write(_codedocument.Text);
            writer.Flush();
            // do not close the stream
            this.IsModified = false;
        }


        public Encoding Encoding {
            get;
            set;
        }

        /// <summary>
        /// Saves the text to the file.
        /// </summary>
        public void Save(string fileName) {
            if(fileName == null)
                throw new ArgumentNullException("fileName");
            using(FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                Save(fs);
            }
            IsModified = false;
        }

        #endregion

        public CodeItemType Type {
            get { return _type; }
        }

        public TextDocument Document {
            get { return _codedocument; }
        }

        public CodeTokenService TokenService {
            get { return _codeTokenService; }
        }

        void OnCodedocumentChanged(object sender, EventArgs e){
            _documentdirty = true;
        }

        void CheckUpdateTokenRepresentation(object sender, EventArgs e) {
            if(_documentdirty && !_tokenizer.IsBusy) {
                _documentdirty = false;
                _tokenizer.TokenizeAsync();
            }
        }

        public bool IsModified { 
            get { return _isModified; } 
            private set { 
                _isModified = value; 
            } 
        }

        public override string ToString() {
            return string.Format("{0} ({1})", this.Name, Type);
        }


        public static CodeItemType CodeItemTypeFromExtension(string ext) {
            switch(ext.ToLowerInvariant()) {
                case ".ia":
                    return CodeItemType.IA;
                case ".ahk":
                    return CodeItemType.AHK;
                default:
                    return CodeItemType.None;
            }
        }
    }
}
