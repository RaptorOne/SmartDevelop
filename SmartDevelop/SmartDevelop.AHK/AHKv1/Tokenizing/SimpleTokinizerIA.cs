using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using System.ComponentModel;
using System.Threading;
using System.Text.RegularExpressions;
using Archimedes.Patterns.Utils;
using SmartDevelop.Model.CodeLanguages;
using SmartDevelop.Model.Tokenizing;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.AHK.AHKv1.Tokenizing
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



    public class SimpleTokinizerIA : Tokenizer
    {
        #region Constants

        const char LITERALSTR = '"';
        const char LITERALSTR_ESCAPE = '"';
        const char SINGLELINE_COMMENT = ';';
        const char PARAMDELEMITER = ',';
        const char MEMBERINVOKE = '.';
        const char STRINGCONCAT = '.';
        const char ESCAPECHAR = '`';
        const char VARIABLEDEREF = '%';
        
        static List<char> ALLOWED_SPECAILCHARS = new List<char> { '_' , '$' };
        static List<char> OPERATORS = new List<char> { '=', '>', '<', '!', '&', '*', '/', ':', '+', '^' , '-', '|' , '?' };
        readonly List<string> KEYWORDS;

        static TokenMapIA OPERATOR_TOKEN = new TokenMapIA();

        #endregion

        #region Fields

        
        List<CodeSegment> _codesegmentsWorker = new List<CodeSegment>();
        List<CodeSegment> _codesegmentsSave = new List<CodeSegment>();
        object __codesegmentsSaveLock = new object();

        
        string _text;
        int _textlen = 0;

        int _currentRangeStart;
        int _currentColStart;
        int _currentLine;
        int _currentColumn;
        Token _activeToken = Token.Unknown;
        Token _currentToken = Token.Unknown;
        BackgroundWorker _tokenizerworker;

        readonly ProjectItemCode _codeitem;
        readonly ITextSource _document;

        #endregion

        #region Constructor

        public SimpleTokinizerIA(ProjectItemCode codeitem, ITextSource document)
         {
            _document = document;
            _codeitem = codeitem;


            KEYWORDS = (from w in codeitem.CodeLanguage.LanguageKeywords
                       select w.Name).ToList();


            _tokenizerworker = new BackgroundWorker();
            _tokenizerworker.DoWork += TokinizeWorker;
            _tokenizerworker.WorkerSupportsCancellation = true;

            _tokenizerworker.RunWorkerCompleted += (s, e) => {
                if(!e.Cancelled)
                    OnFinishedSucessfully();
                lock(_tokenizerworkerLock) {
                    _asynctokenizerworkerBusy = false;
                }
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts Tokenizing async
        /// </summary>
        public override void TokenizeAsync() {
            lock(_tokenizerworkerLock) {
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
                _asynctokenizerworkerBusy = true;
                _tokenizerworker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Starts Tokenizing sync
        /// </summary>
        public override void TokenizeSync() {
            lock(_tokenizerworkerLock) {
                if(!_tokenizerworker.IsBusy) {
                    _syncTokenizerBusy = true;

                    _text = _document.Text;
                    _textlen = _text.Length;
                    TokinizeWorker(null, new DoWorkEventArgs(null));

                    _syncTokenizerBusy = false;
                    OnFinishedSucessfully();
                }
            }
        }

        /// <summary>
        /// Get imutalble List of segments
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<CodeSegment> GetSegmentsSnapshot() {
            lock(__codesegmentsSaveLock) {
                return new List<CodeSegment>(_codesegmentsSave);
            }
        }

        #endregion

        #region Properties

        object _tokenizerworkerLock = new object();
        bool _syncTokenizerBusy = false;
        bool _asynctokenizerworkerBusy = false;
        

        /// <summary>
        /// Is tokenizing running now?
        /// </summary>
        public override bool IsBusy {
            get {
                if(_syncTokenizerBusy)
                    return true;

                lock(_tokenizerworkerLock) {
                    return _asynctokenizerworkerBusy;//_tokenizerworker.IsBusy;
                }
            }
        }

        #endregion
        bool _traditionalMode = false;

        #region Tokenizer

        void TokinizeWorker(object sender, DoWorkEventArgs e) {
            var bgw = sender as BackgroundWorker;
            _currentToken = Token.Unknown;
            
            bool ensureNewToken = false;
            
            int i;


            bool inTradEscapeBefore = false;
            bool inderef = false;
            bool inTraditionalCommand = false;
            int openLiteralBracketsInTraditional = 0;

            //clean things up
            _activeToken = Token.Unknown;
            _codesegmentsWorker.Clear();
            _currentRangeStart = 0;
            _currentColStart = 0;
            _currentLine = 1;
            _currentColumn = 0;
            _traditionalMode = false;



            char currentChar;
            for(i = 0; i < _textlen; i++) {

            redo:

                #region Handle Cancel Tokenizer

                if(bgw != null && bgw.CancellationPending) {
                    e.Cancel = true;
                    break;
                }

                #endregion

                #region Force new Tokens if necessary

                // end one sign regions -> braktes/lines
                // ensure that we differ from the token before in those cases
                if((ensureNewToken || TokenHelper.BRAKETS.ContainsValue(_activeToken) 
                    || _activeToken == Token.NewLine) || _activeToken == Token.ParameterDelemiter || _activeToken == Token.Deref
                    || _activeToken == Token.MemberInvoke || _activeToken == Token.StringConcat
                    || (_activeToken == Token.WhiteSpace && !IsWhiteSpace(i))) 
                {
                    ensureNewToken = false;
                    if(!_traditionalMode || inderef)
                        _currentToken = Token.Unknown;
                    else
                        _currentToken = Token.TraditionalString;
                }

                #endregion

                currentChar = _text[i];

                if(currentChar == '\n') {
                    _currentToken = Token.NewLine;
                    //_currentLine++;
                    _currentColumn = 0;
                    _traditionalMode = false;   // we assume that traditional commands not have multilines
                    inTraditionalCommand = false; 
                } else if(!_traditionalMode && IsLieralStringMarkerBegin(i)) {

                    #region Parse literal string

                    EndActiveToken(i);
                    _activeToken = Token.LiteralString;


                    bool previuosWasEscape = true;
                    while(i < _textlen) {
                        if(_text[i] == '\n') {
                            _currentLine++;
                            break;
                        } else if(_text[i] == '"') {
                            if(previuosWasEscape == true)
                                previuosWasEscape = false;
                            else if(_text.Next(i) == '"') {
                                previuosWasEscape = true;
                            } else {
                                break;
                            }
                        } else {
                            previuosWasEscape = false;
                        }
                        i++;
                    }

                    ensureNewToken = true;
                    if((i + 1) < _textlen)
                        i++;
                    goto redo;
                    //EndActiveToken(i);

                    #endregion

                } else if(!_traditionalMode && IsMultiLineCommentStart(i)) {

                    #region Handle Multiline Comment

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

                    #endregion

                } else if(!IsInAnyComment() && !_traditionalMode && IsMultilineTraditionalStringStart(i)){

                    #region Handle Multiline strings
                    // for now extract the wohle thing as one token.

                    EndActiveToken(i);
                    _activeToken = Token.TraditionalString;


                    // lets find the end of multiline string section,
                    // as we dont want to parse the whole string chunk
                    // to speed things up
                    bool endingboundsFound = false;
                    bool matchDirty = true;
                    while(i < _textlen) {
                        if(_text[i] == '\n') {
                            _currentLine++;
                            matchDirty = false;
                        }else if(_text[i] == ')' && !matchDirty) {
                            endingboundsFound = true;
                            break;
                        } else if(!IsWhiteSpace(i)) {
                            matchDirty = true;
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

                    #endregion

                } else if(!IsInAnyComment()) {

                    if(!_traditionalMode && TokenHelper.BRAKETS.ContainsKey(currentChar)) {
                        _currentToken = TokenHelper.BRAKETS[currentChar];
                    } else if(currentChar == SINGLELINE_COMMENT) {
                        _currentToken = Token.SingleLineComment;
                    } else if(!_traditionalMode && IsWhiteSpace(i)) {
                        _currentToken = Token.WhiteSpace;
                    } else if(!_traditionalMode && currentChar == MEMBERINVOKE && i > 0 && (!IsWhiteSpace(i - 1) || IsNumber(_text[i - 1]))) {
                        _currentToken = Token.MemberInvoke;
                    } else if(!_traditionalMode && currentChar == STRINGCONCAT) {
                        _currentToken = Token.StringConcat;
                    //}else if(!traditionalMode && IsVariableAsignStart(i)){

                    } else if(!_traditionalMode && IsTraditionalCommandBegin(i)) {
                        _currentToken = Token.TraditionalCommandInvoke;
                        inTraditionalCommand = true;
                    } else if(!_traditionalMode && OPERATORS.Contains(currentChar)) {
                        _currentToken = Token.OperatorFlow;
                    } else if(!_traditionalMode && _activeToken == Token.OperatorFlow && !OPERATORS.Contains(currentChar)) {
                        _currentToken = Token.Unknown;
                    
                    #region traditional parsing

                    } else if(_text[i] == PARAMDELEMITER && !inTradEscapeBefore) {
                        _currentToken = Token.ParameterDelemiter;
                        if(inTraditionalCommand && openLiteralBracketsInTraditional == 0)
                            _traditionalMode = true;
                    } else if(_text[i] == VARIABLEDEREF && !inTradEscapeBefore && !inderef) {
                        _currentToken = Token.Deref;
                        if(!IsWhiteChar(_text.Next(i))) {
                            inderef = true;
                        } else
                            _traditionalMode = false;
                    } else if(_text[i] == VARIABLEDEREF && inderef) {
                        _currentToken = Token.Deref;
                        inderef = false;
                    }

                    if(_traditionalMode) {
                        if(_text[i] == ESCAPECHAR && !inTradEscapeBefore) {
                            inTradEscapeBefore = true;
                        } else {
                            inTradEscapeBefore = false;
                        }
                    }

                    if(!_traditionalMode && inTraditionalCommand && _activeToken != Token.TraditionalString){
                        if(_text[i] == '('){
                            openLiteralBracketsInTraditional++;
                        }else if(_text[i] == ')'){
                            openLiteralBracketsInTraditional--;
                        }
                    }

                    #endregion

                }

                if(_currentToken != _activeToken || IsSingleCharToken(_activeToken)) {
                    // we have to end the previous token region and set the new one active
                    EndActiveToken(i);
                    _activeToken = _currentToken;
                }

                if(currentChar == '\n')
                    _currentLine++;
                else
                    _currentColumn++;
            }
            EndActiveToken(_textlen);
            lock(__codesegmentsSaveLock) {
                _codesegmentsSave.Clear();
                _codesegmentsSave.AddRange(_codesegmentsWorker);
            }
            lock(_tokenizerworkerLock) {
                _asynctokenizerworkerBusy = false; //_tokenizerworker.IsBusy;
            }
        }

        #endregion

        #region End & Save Tokenflow

        static char[] trimchars = { ' ', '\t', '\n', '\r' };
        CodeSegment _previous = null;

        void EndActiveToken(int index) {
            int l = index - _currentRangeStart;
            if(l > 0) {
                var str = _text.Substring(_currentRangeStart, l).Trim(trimchars);
                if(!(_activeToken == Token.Unknown && str.Length == 0)) {

                    Token? tokenToStore = null;


                    if(_activeToken == Token.Unknown) {

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

                    if(_activeToken == Token.OperatorFlow) {
                        // toDo:
                        // handle AHK specail cases like:
                        // := AND = Asign
                        // == AND = Equality
                        // to do so, look back this line for

                        tokenToStore = OPERATOR_TOKEN.FindOperatorToken(str);
                    }

                    if(l > 1 && _currentToken == Token.NewLine)
                        l--;
                    int linenumber = _currentLine;
                    if(_activeToken == Token.NewLine)
                        --linenumber;


                    if(_activeToken == Token.TraditionalCommandInvoke && _currentToken != Token.NewLine)
                        _traditionalMode = true;

                    var current = new CodeSegment(_codeitem, tokenToStore.HasValue ? tokenToStore.Value : _activeToken,
                        str, new SimpleSegment(_currentRangeStart, l), linenumber, _currentColStart, _previous);
                    if(_previous != null)
                        _previous.Next = current;
                    _previous = current;
                    _codesegmentsWorker.Add(current);
                }
            }
            _currentRangeStart = index;
            _currentColStart = _currentColumn;
        }

        #endregion

        #region Helpermethods

        public static bool IsNumber(string str) {
            bool isNum = true;
            //check for number:
            foreach(char c in str)
                if(!AsciiHelper.IsAsciiNum(c)) {
                    isNum = false;
                    break;
                }
            return isNum;
        }

        public static  bool IsNumber(char c) {
            return AsciiHelper.IsAsciiNum(c);
        }

        public static bool IsHexNumber(string str) {
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
            return (_text[index] == ' ') || (_text[index] == '\t');
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

                int cindex = index + command.Length;
                if(cindex < _text.Length) {
                    char c = _text[cindex];
                    // the next char must be a whitespace or param delemiter
                    if(c == ' ' || c == '\t' || c == '\r' || c == ','){
                        // now, we expect a lot but not-> =, :=, 
                        //get next char which is not an whitespace
                        

                        char nextChar = NextCharOnThisLineOmitWhiteSpace(cindex);
                        if(!(nextChar == '.' || OPERATORS.Contains(nextChar))) {
                            return true;
                        }
                    } else {
                        return false;
                    }
                } else
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Assuming that we are currently NOT in a literal string
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsLieralStringMarkerBegin(int index) {
            return (_text[index] == LITERALSTR && !IsInAnyComment());
        }

        bool IsMultiLineCommentStart(int index) {
            return (_activeToken != Token.LiteralString) && (_text[index] == '/' && _text.Next(index) == '*');
        }

        bool IsMultilineTraditionalStringStart(int index) {
            if((_activeToken != Token.LiteralString) && (_text[index] == '(')) {

                while(true) {
                    if(--index > 0) {
                        if(_text[index] == '\n')
                            return true;
                        else if(!IsWhiteSpace(index)) {
                            break;
                        }
                    } else
                        break;
                }
            }
            return false;
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

        public static bool IsSingleCharToken(Token token) {
            return token == Token.NewLine || token == Token.ParameterDelemiter || token == Token.MemberInvoke || token == Token.StringConcat || token == Token.Deref || TokenHelper.BRAKETS.ContainsValue(token);
        }

        bool IsReservedKeyWordStart(int index) {
            return (KEYWORDS.Contains(ExtractWord(index).ToLowerInvariant()));
        }

        string ExtractWord(int start) {
            return ExtractWord(start, null);
        }
        string ExtractWord(int start, List<char> allowedspecailChars) {
            return ExtractWord(ref _text, start, allowedspecailChars);
        }

        public static string ExtractWord(ref string text, int start, List<char> allowedspecailChars) {
            var sb = new StringBuilder();
            char c;

            for(int sprt = start; sprt < text.Length; sprt++) {
                c = text[sprt];
                if(!IsWhiteChar(c) && (AsciiHelper.IsAsciiLiteralLetter(c) || (allowedspecailChars != null && allowedspecailChars.Contains(c)))) {
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

        char NextCharOnThisLineOmitWhiteSpace(int index) {
            char c;
            while(true) {
                c = _text.Next(index++);
                if(c == '\n' || c == '\0')
                    break;
                else if(c != ' ' && c != '\t')
                    break;
            }
            return c;
        }

        bool IsWhiteChar(int index) {
            return _text[index] == ' ' || _text[index] == '\t';
        }

        public static bool IsWhiteChar(char c) {
            return c == ' ' || c == '\t';
        }

        #endregion
    }
}
