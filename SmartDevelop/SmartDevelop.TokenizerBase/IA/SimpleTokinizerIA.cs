using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using System.ComponentModel;
using System.Threading;
using System.Text.RegularExpressions;

namespace SmartDevelop.TokenizerBase.IA
{

    static class StringExtensions
    {
        /// <summary>
        /// Get the previous char of the given index (or NULL when index out of bounds)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static char Previous(this string str, int i) {
            if(i != 0)
                return str[i - 1];
            else
                return '\0';
        }

        /// <summary>
        /// Get the next char at the given index of this string (or NULL when index out of bounds)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="i"></param>
        /// <returns></returns>
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
        static List<char> OPERATORS = new List<char> { '=', '>', '<', '!', '&', '*', '/', ':', '+', '-', '|' , '?' };
        static List<string> KEYWORDS = new List<string> 
            { 
                "if",
                "loop",
                "while",
                "return",
                "is",
                "global",
                "static",

                "true",
                "false"
                
                //etc
            };


        #endregion

        #region Fields

        CodeTokenRepesentation _codetokenRep = new CodeTokenRepesentation();
        List<CodeSegment> _codesegmentsTemp = new List<CodeSegment>();

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

        #region Public Methods

        /// <summary>
        /// Starts Tokenizing async
        /// </summary>
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

        /// <summary>
        /// Starts Tokenizing sync
        /// </summary>
        public void TokenizeSync() {
            _text = _document.Text;
            _textlen = _text.Length;
            TokinizeWorker(null, new DoWorkEventArgs(null));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Is tokenizing running now?
        /// </summary>
        public bool IsBusy {
            get { return _tokenizerworker.IsBusy; }
        }

        public CodeTokenRepesentation CodeTokens {
            get { return _codetokenRep; }
        }

        #endregion

        #region Tokenizer

        void TokinizeWorker(object sender, DoWorkEventArgs e) {
            var bgw = sender as BackgroundWorker;
            Token currentToken = Token.Unknown;
            
            bool inliteralString = false;
            bool ensureNewToken = false;
            bool traditionalMode = false;
            int i;
            //clean things up
            _activeToken = Token.Unknown;
            _codesegmentsTemp.Clear();
            _currentRangeStart = 0;

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
                    || _activeToken == Token.MemberInvoke
                    || (_activeToken == Token.WhiteSpace && !IsWhiteSpace(i))) 
                {
                    ensureNewToken = false;
                    currentToken = Token.Unknown;
                }

                #endregion


                if(_text[i] == '\n') {
                    currentToken = Token.NewLine;
                    traditionalMode = false;
                } else if(!traditionalMode && IsLieralStringMarker(i)) {
                    if(_activeToken == Token.LiteralString) {
                        ensureNewToken = true;
                        inliteralString = false;
                    } else {
                        currentToken = Token.LiteralString;
                        inliteralString = true;
                    }
                } else if(!traditionalMode && IsMultiLineCommentStart(i)) {
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
                    char c = _text[i];
                    if(!traditionalMode && BRAKETS.Contains(c)) {
                        currentToken = Token.Bracket;
                    } else if(c == SINGLELINE_COMMENT) {
                        currentToken = Token.SingleLineComment;
                    } else if(!traditionalMode && IsWhiteSpace(i)) {
                        currentToken = Token.WhiteSpace;
                    } else if(c == PARAMDELEMITER){
                        currentToken = Token.ParameterDelemiter;
                    } else if(!traditionalMode && c == MEMBERINVOKE && i > 0 && (!IsWhiteSpace(i - 1) || IsNumber(_text[i - 1]))) {
                        currentToken = Token.MemberInvoke;
                    } else if(!traditionalMode && c == STRINGCONCAT) {
                        currentToken = Token.StringConcat;

                    //}else if(!traditionalMode && IsVariableAsignStart(i)){

                    } else if(!traditionalMode && IsTraditionalCommandBegin(i)) {
                        currentToken = Token.TraditionalCommandInvoke;
                        traditionalMode = true;
                    } else if(!traditionalMode && OPERATORS.Contains(c)) {
                        currentToken = Token.OperatorFlow;
                        // to do: default expressions_activeToken
                    } else if(_activeToken == Token.OperatorFlow && !OPERATORS.Contains(c)) {
                        currentToken = Token.Unknown;
                    }
                }

                if(currentToken != _activeToken || IsSingleCharToken(_activeToken)) {
                    // we have to end the previous token region and set the new one active
                    EndActiveToken(i);
                    _activeToken = currentToken;
                }
            }
            EndActiveToken(_textlen);
            _codetokenRep.Reset(_codesegmentsTemp);
        }

        #region Helpermethods

        static char[] trimchars = { ' ', '\t', '\n', '\r' };

        void EndActiveToken(int index) {
            int l = index - _currentRangeStart;
            if(l > 0) {
                var str = _text.Substring(_currentRangeStart, l).Trim(trimchars);
                if(!(_activeToken == Token.Unknown && str.Length == 0)){

                    if(_activeToken == Token.Unknown){
                        if(IsNumber(str))
                            _activeToken = Token.Number;
                        else if(IsHexNumber(str)) {
                            _activeToken = Token.HexNumber;
                        } else if(KEYWORDS.Contains(str.ToLowerInvariant())) {
                            _activeToken = Token.KeyWord;
                        }
                    }

                    _codesegmentsTemp.Add(new CodeSegment(_activeToken, str, new SimpleSegment(_currentRangeStart, l)));
                }
            }
            _currentRangeStart = index;
        }

        bool IsNumber(string str) {
            bool isNum = true;
                //check for number:
            foreach(char c in str)
                if(!AsciiHelper.IsAsciiNum(c)) {
                    isNum = false;
                    break;
                }
            return isNum;
        }

        bool IsNumber(char c) {
            return AsciiHelper.IsAsciiNum(c);
        }

        bool IsHexNumber(string str) {
            return str.Length > 2 && str.Substring(0, 2) == "0x" && IsNumber(str.Substring(2));
        }

        bool IsWhiteSpace(int index) {
            return (_text[index] == ' ') ||( _text[index] == '\t');
        }

        /// <summary>
        /// Checks if at this index a traditional command starts:
        /// -> Must be sequeled by whitechars
        /// -> Must not be followed by '(' or ')' 
        /// </summary>
        /// <returns></returns>
        bool IsTraditionalCommandBegin(int index) {
            if(IsCleanPrefixSpace(index)) {
                if(IsReservedKeyWordStart(index))
                    return false;

                var command = ExtractWord(index);
                if(command.Length == 0)
                    return false;

                char c;
                bool fail = false;
                for(int scanPtr = index + command.Length; _textlen > scanPtr; scanPtr++) {
                    c = _text[scanPtr];
                    if(c == '(' || OPERATORS.Contains(c)) {
                        fail = true;
                        break;
                    }
                    if(AsciiHelper.IsAsciiLiteralLetter(c))
                        break;
                }
                return !fail;
            }
            return false;
        }

        //bool IsVariableAsignStart(int index) {
        //    if(IsCleanPrefixSpace(index)) {

        //        var word = ExtractWord(index);
        //        if(KEYWORDS.Contains(word))
        //            return false;

        //        //scan for asignment OP:



        //    }
        //}

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

        bool IsReservedKeyWordStart(int index) {
            return (KEYWORDS.Contains(ExtractWord(index).ToLowerInvariant()));
        }

        string ExtractWord(int start) {
            var sb = new StringBuilder();
            char c;

            for(int sprt = start; sprt < _textlen; sprt++) {
                c = _text[sprt];
                if(!IsWhiteChar(sprt) && AsciiHelper.IsAsciiLiteralLetter(c)) {
                    sb.Append(c);
                } else
                    break;
            }
            return sb.ToString();
        }


        bool IsCleanPrefixSpace(int index) {
            bool cleanPrefixSpace = true;
            for(int pPtr = index - 1; pPtr >= 0; pPtr--) {
                if(_text[pPtr] == '\n')
                    break;
                if(!IsWhiteChar(pPtr)) {
                    cleanPrefixSpace = false;
                    break;
                }
            }
            return cleanPrefixSpace;
        }

        bool IsWhiteChar(int index) {
            return _text[index] == ' ' || _text[index] == '\t';
        }

        #endregion

        #endregion
    }
}
