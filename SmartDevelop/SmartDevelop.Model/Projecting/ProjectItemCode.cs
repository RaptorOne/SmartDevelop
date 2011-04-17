using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.TokenizerBase.IA;
using System.Threading;
using System.Windows.Threading;


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
        readonly TextDocument _codedocument;
        CodeItemType _type = CodeItemType.None;
        SimpleTokinizerIA _tokenizer;
        bool _documentdirty = false;

        public ProjectItemCode(CodeItemType type) {
            _codedocument = new TextDocument();

            _codedocument.Changed += OnCodedocumentChanged;

            _tokenizer = new SimpleTokinizerIA(_codedocument);
            _type = type;


            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
            foldingUpdateTimer.Tick += CheckUpdateTokenRepresentation;
            foldingUpdateTimer.Start();

        }

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
    }
}
