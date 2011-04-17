using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.TokenizerBase.IA;
using System.Threading;
using System.Windows.Threading;
using System.IO;


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

    public class ProjectItemCode : ProjectItem
    {
        #region Fields

        readonly TextDocument _codedocument;
        CodeItemType _type = CodeItemType.None;
        SimpleTokinizerIA _tokenizer;
        bool _documentdirty = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Attempts to load a code file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ProjectItemCode FromFile(string filepath){
            if(!File.Exists(filepath))
                return null;

            ProjectItemCode newp = new ProjectItemCode(CodeItemTypeFromExtension(Path.GetExtension(filepath)));
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

        public static CodeItemType CodeItemTypeFromExtension(string ext) {
            switch(ext.ToLowerInvariant()){
                case ".ia":
                    return CodeItemType.IA;
                case ".ahk":
                    return CodeItemType.AHK;
                default:
                    return CodeItemType.None;
            }
        }

        public ProjectItemCode(CodeItemType type) {
            _codedocument = new TextDocument();
            Encoding = Encoding.UTF8;

            _codedocument.Changed += OnCodedocumentChanged;

            _tokenizer = new SimpleTokinizerIA(_codedocument);
            _type = type;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
            foldingUpdateTimer.Tick += CheckUpdateTokenRepresentation;
            foldingUpdateTimer.Start();
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
        }

        #endregion

        public CodeItemType Type {
            get { return _type; }
        }

        public TextDocument Document {
            get { return _codedocument; }
        }

        public CodeTokenRepesentation TokenService {
            get { return _tokenizer.CodeTokens; }
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

        bool _isModified;
        public bool IsModified { get { return _isModified; } set { _isModified = value; } }


        public override string ToString() {
            return string.Format("{0} ({1})", this.Name, Type);
        }
    }
}
