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

        static Dictionary<char, Token> BRAKETS = new Dictionary<char, Token>();
        static List<char> ALLOWED_SPECAILCHARS = new List<char> { '_' , '$' };
        static List<char> OPERATORS = new List<char> { '=', '>', '<', '!', '&', '*', '/', ':', '+', '-', '|' , '?' };
        static List<string> KEYWORDS = new List<string> 
            { 
                "if",
                "else",
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

        static TokenMapIA OPERATOR_TOKEN = new TokenMapIA();

        static SimpleTokinizerIA(){
            BRAKETS.Add('(', Token.LiteralBracketOpen);
            BRAKETS.Add(')', Token.LiteralBracketClosed);
            BRAKETS.Add('{', Token.BlockOpen);
            BRAKETS.Add('}', Token.BlockClosed);
            BRAKETS.Add('[', Token.IndexerBracketOpen);
            BRAKETS.Add(']', Token.IndexerBracketClosed);
        }


        #endregion

        public event EventHandler Finished;

        #region Fields

        
        List<CodeSegment> _codesegmentsWorker = new List<CodeSegment>();
        List<CodeSegment> _codesegmentsSave = new List<CodeSegment>();
        object __codesegmentsSaveLock = new object();

        ITextSource _document;
        string _text;
        int _textlen = 0;

        int _currentRangeStart = 0;
        int _currentColStart = 0;

        int _currentLine = 0;
        int _currentColumn = 0;
        Token _activeToken = Token.Unknown;

        BackgroundWorker _tokenizerworker;

        #endregion

        #region Constructor

        public SimpleTokinizerIA(ITextSource document) {
            _document = document;

            _tokenizerworker = new BackgroundWorker();
            _tokenizerworker.DoWork += TokinizeWorker;
            _tokenizerworker.WorkerSupportsCancellation = true;

            _tokenizerworker.RunWorkerCompleted += (s, e) => {
                    if(Finished != null)
                        Finished(this, EventArgs.Empty);
                };
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

        /// <summary>
        /// Get imutalble List of segments
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CodeSegment> GetSegmentsSnapshot() {
            lock(__codesegmentsSaveLock) {
                return new List<CodeSegment>(_codesegmentsSave);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Is tokenizing running now?
        /// </summary>
        public bool IsBusy {
            get { return _tokenizerworker.IsBusy; }
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
            _codesegmentsWorker.Clear();
            _currentRangeStart = 0;
            _currentColStart = 0;
            _currentLine = 0;
            _currentColumn = 0;

            char currentChar;
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
                if((ensureNewToken || BRAKETS.ContainsValue(_activeToken) 
                    || _activeToken == Token.NewLine) || _activeToken == Token.ParameterDelemiter
                    || _activeToken == Token.MemberInvoke
                    || (_activeToken == Token.WhiteSpace && !IsWhiteSpace(i))) 
                {
                    ensureNewToken = false;
                    currentToken = Token.Unknown;
                }

                #endregion

                currentChar = _text[i];

                if(currentChar == '\n') {
                    currentToken = Token.NewLine;
                    _currentLine++;
                    _currentColumn = 0;
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
                        if(_text[i] == '\n')
                            _currentLine++;
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
                        _currentColumn = 0;
                        EndActiveToken(i);
                    }
                } else if(!inliteralString && !IsInAnyComment()) {
                    //expressions
                    if(!traditionalMode && BRAKETS.ContainsKey(currentChar)) {
                        currentToken = BRAKETS[currentChar];
                    } else if(currentChar == SINGLELINE_COMMENT) {
                        currentToken = Token.SingleLineComment;
                    } else if(!traditionalMode && IsWhiteSpace(i)) {
                        currentToken = Token.WhiteSpace;
                    } else if(currentChar == PARAMDELEMITER) {
                        currentToken = Token.ParameterDelemiter;
                    } else if(!traditionalMode && currentChar == MEMBERINVOKE && i > 0 && (!IsWhiteSpace(i - 1) || IsNumber(_text[i - 1]))) {
                        currentToken = Token.MemberInvoke;
                    } else if(!traditionalMode && currentChar == STRINGCONCAT) {
                        currentToken = Token.StringConcat;

                    //}else if(!traditionalMode && IsVariableAsignStart(i)){

                    } else if(!traditionalMode && IsTraditionalCommandBegin(i)) {
                        currentToken = Token.TraditionalCommandInvoke;
                        traditionalMode = true;
                    } else if(!traditionalMode && OPERATORS.Contains(currentChar)) {
                        currentToken = Token.OperatorFlow;
                        // to do: default expressions_activeToken
                    } else if(_activeToken == Token.OperatorFlow && !OPERATORS.Contains(currentChar)) {
                        currentToken = Token.Unknown;
                    }
                }

                if(currentToken != _activeToken || IsSingleCharToken(_activeToken)) {
                    // we have to end the previous token region and set the new one active
                    EndActiveToken(i);
                    _activeToken = currentToken;
                }

                if(currentChar != '\n')
                    _currentColumn++;
            }
            EndActiveToken(_textlen);
            lock(__codesegmentsSaveLock) {
                _codesegmentsSave.Clear();
                _codesegmentsSave.AddRange(_codesegmentsWorker);
            }
        }

        #region Helpermethods

        static char[] trimchars = { ' ', '\t', '\n', '\r' };

        CodeSegment _previous = null;

        void EndActiveToken(int index) {
            int l = index - _currentRangeStart;
            if(l > 0) {
                var str = _text.Substring(_currentRangeStart, l).Trim(trimchars);
                if(!(_activeToken == Token.Unknown && str.Length == 0)){

                    Token? tokenToStore = null;
                    

                    if(_activeToken == Token.Unknown){
                        
                        if(IsNumber(str)) {
                            _activeToken = Token.Number;
                        } else if(IsHexNumber(str)) {
                            _activeToken = Token.HexNumber;
                        } else if(KEYWORDS.Contains(str.ToLowerInvariant())) {
                            _activeToken = Token.KeyWord;
                        } else {
                            tokenToStore = Token.Identifier;
                        }
                    }

                    if(_activeToken == Token.OperatorFlow){
                        // toDo:
                        // handle AHK specail cases like:
                        // := AND = Asign
                        // == AND = Equality
                        // to do so, look back this line for

                        tokenToStore = OPERATOR_TOKEN.FindOperatorToken(str);
                    }


                    var current = new CodeSegment(tokenToStore.HasValue ? tokenToStore.Value : _activeToken, str, new SimpleSegment(_currentRangeStart, l), _currentLine, _currentColStart, _previous);
                    if(_previous != null)
                        _previous.Next = current;
                    _previous = current;
                    _codesegmentsWorker.Add(current);
                }
            }
            _currentRangeStart = index;
            _currentColStart = _currentColumn;
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
            if(str.Length > 2 && str.Substring(0, 2) == "0x") {
                foreach(char c in str.Substring(2)) {
                    if(!AsciiHelper.IsAsciiHexNum(c)) {
                        return false;
                    }
                }
                return true;
            }
            return false;
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

                var command = ExtractWord(index, ALLOWED_SPECAILCHARS);
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
                    if(AsciiHelper.IsAsciiLiteralLetter(c) || ALLOWED_SPECAILCHARS.Contains(c))
                        break;
                }
                return !fail;
            }
            return false;
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
            return token == Token.NewLine || token == Token.ParameterDelemiter || token == Token.MemberInvoke || token == Token.StringConcat || BRAKETS.ContainsValue(token);
        }

        bool IsReservedKeyWordStart(int index) {
            return (KEYWORDS.Contains(ExtractWord(index).ToLowerInvariant()));
        }

        string ExtractWord(int start) {
            return ExtractWord(start, null);
        }
        string ExtractWord(int start, List<char> allowedspecailChars) {
            var sb = new StringBuilder();
            char c;

            for(int sprt = start; sprt < _textlen; sprt++) {
                c = _text[sprt];
                if(!IsWhiteChar(sprt) && (AsciiHelper.IsAsciiLiteralLetter(c) || (allowedspecailChars != null && allowedspecailChars.Contains(c)))) {
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
