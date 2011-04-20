using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using System.ComponentModel;
using System.Threading;

namespace SmartDevelop.TokenizerBase.IA
{

    static class StringExtensions
    {
        public static char Previous(this string str, int i) {
            if(i != 0)
                return str[i - 1];
            else
                return '\0';
        }

        public static char Next(this string str, int i) {
            if(i != str.Length - 1) {
                return str[i + 1]; 
            }else
                return '\0';
        }
    }

    public class SimpleTokinizerIA
    {
        #region Constants

        const char LITERALSTR = '"';
        const char LITERALSTR_ESCAPE = '"';
        const char SINGLELINE_COMMENT = ';';
        const char PARAMDELEMITER = ',';
        const char MEMBERINVOKE = '.';
        const char STRINGCONCAT = '.';
        static List<char> BRAKETS = new List<char> { '(', ')', '{', '}', '[', ']' };

        #endregion

        #region Fields

        CodeTokenRepesentation _codetokenRep = new CodeTokenRepesentation();
        ITextSource _document;
        string _text;
        int _textlen = 0;
        int _currentRangeStart = 0;
        Token _activeToken = Token.Unknown;

        BackgroundWorker _tokenizerworker;

        #endregion

        #region Constructor

        public SimpleTokinizerIA(ITextSource document) {
            _document = document;

            _tokenizerworker = new BackgroundWorker();
            _tokenizerworker.DoWork += TokinizeWorker;
            _tokenizerworker.WorkerSupportsCancellation = true;
        }

        #endregion


        public void TokenizeAsync() {
            if(_tokenizerworker.IsBusy) {
                _tokenizerworker.CancelAsync();
                while(true) {
                    if(_tokenizerworker.IsBusy) {
                        Thread.Sleep(1);
                    } else
                        break;
                }
            }
            _text = _document.Text;
            _textlen = _text.Length;
            _tokenizerworker.RunWorkerAsync();
        }

        public void TokenizeSync() {
            _text = _document.Text;
            _textlen = _text.Length;
            TokinizeWorker(null, new DoWorkEventArgs(null));
        }

        public bool IsBusy {
            get { return _tokenizerworker.IsBusy; }
        }


        void TokinizeWorker(object sender, DoWorkEventArgs e) {
            var bgw = sender as BackgroundWorker;
            Token currentToken = Token.Unknown;
            bool inliteralString = false;
            bool ensureNewToken = false;
            int i;
            //clean things up
            _activeToken = Token.Unknown;
            _codetokenRep.Clear();

            for(i = 0; i < _textlen; i++) {

                #region Handle Cancel Tokenizer

                if(bgw != null && bgw.CancellationPending) {
                    e.Cancel = true;
                    break;
                }

                #endregion

                #region Force new Tokens if necessary

                // end one sign regions -> braktes/lines
                // ensure that we differ from the token before in those cases
                if( (ensureNewToken || _activeToken == Token.Bracket 
                    || _activeToken == Token.NewLine) || _activeToken == Token.ParameterDelemiter
                    || (_activeToken == Token.WhiteSpace && !IsWhiteSpace(i))) 
                {
                    ensureNewToken = false;
                    currentToken = Token.Unknown;
                }

                #endregion

                if(IsLieralStringMarker(i)) {
                    if(_activeToken == Token.LiteralString) {
                        ensureNewToken = true;
                        inliteralString = false;
                    } else {
                        currentToken = Token.LiteralString;
                        inliteralString = true;
                    }
                } else if(IsMultiLineCommentStart(i)) {
                    EndActiveToken(i);
                    _activeToken = Token.MultiLineComment;

                    // lets find the end of comment section,
                    // as we dont want to parse the whole comment chunk
                    // to speed things up
                    bool endingboundsFound = false;
                    while(i < _textlen) {
                        if(IsMultiLineCommentEnd(i)) {
                            endingboundsFound = true;
                            break;
                        }
                        i++;
                    }
                    
                    if(!endingboundsFound) {
                        EndActiveToken(i);
                        return; // we are done ;)
                    } else {
                        i += 2;
                        EndActiveToken(i);
                    }

                } else if(!inliteralString && !IsInAnyComment()) {
                    //expressions
                    if(BRAKETS.Contains(_text[i])) {
                        currentToken = Token.Bracket;
                    } else if(_text[i] == SINGLELINE_COMMENT) {
                        currentToken = Token.SingleLineComment;
                    } else if(IsWhiteSpace(i)) {
                        currentToken = Token.WhiteSpace;
                    } else if(_text[i] == PARAMDELEMITER){
                        currentToken = Token.ParameterDelemiter;
                    } else if(_text[i] == MEMBERINVOKE && i > 0 && !IsWhiteSpace(i-1)) {
                        currentToken = Token.MemberInvoke;
                    } else if(_text[i] == STRINGCONCAT) {
                        currentToken = Token.StringConcat;
                    } else {
                        // to do: default expressions
                    }
                }

                if(_text[i] == '\n') {
                    currentToken = Token.NewLine;
                }

                if(currentToken != _activeToken || IsSingleCharToken(_activeToken)) {
                    // we have to end the previous token region and set the new one active
                    EndActiveToken(i);
                    _activeToken = currentToken;
                }
            }
            EndActiveToken(_textlen);
        }



        public CodeTokenRepesentation CodeTokens {
            get { return _codetokenRep; }
        }

        #region Helpermethods

        static char[] trimchars = { ' ', '\t', '\n', '\r' };

        void EndActiveToken(int index) {
            int l = index - _currentRangeStart;
            if(l > 0) {
                var str = _text.Substring(_currentRangeStart, l).Trim(trimchars);
                if(!(_activeToken == Token.Unknown && str.Length == 0)){
                    _codetokenRep.Add(new CodeSegment(_activeToken, str, new SimpleSegment(_currentRangeStart, l)));
                }
            }
            _currentRangeStart = index;
        }

        bool IsWhiteSpace(int index) {
            return (_text[index] == ' ') ||( _text[index] == '\t');
        }

        bool IsLieralStringMarker(int index) {
            return (_text[index] == LITERALSTR && !IsInAnyComment());
        }

        bool IsMultiLineCommentStart(int index) {
            return (_activeToken != Token.LiteralString) && (_text[index] == '/' && _text.Next(index) == '*');
        }

        bool IsMultiLineCommentEnd(int index) {
            return (_activeToken != Token.LiteralString) && (_text[index] == '*' && _text.Next(index) == '/');
        }

        bool IsSingleLineCommentStart(int index) {
            return (_activeToken != Token.LiteralString) && !IsInAnyComment() && _text[index] == SINGLELINE_COMMENT;
        }

        bool IsInAnyComment() {
            return _activeToken == Token.SingleLineComment || _activeToken == Token.MultiLineComment;
        }

        bool IsSingleCharToken(Token token) {
            return token == Token.Bracket || token == Token.NewLine || token == Token.ParameterDelemiter || token == Token.MemberInvoke || token == Token.StringConcat;
        }

        #endregion

    }
}
