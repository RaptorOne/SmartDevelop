using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using System.CodeDom;

namespace SmartDevelop.TokenizerBase
{
    /// <summary>
    /// Represents a CodeSegment
    /// A CodeSegment is a Code-Token with exact defined document location. It gets automatically extended when the DOM Parser has found its context
    /// </summary>
    public class CodeSegment
    {
        #region Fields

        readonly SimpleSegment _codesegment;
        readonly Token _type;
        readonly string _tokenstring;
        readonly int _line;
        readonly int _column;

        CodeSegment _previous;
        CodeSegment _next;
        CodeObject _codeDOMObject = null;

        #endregion

        #region Constructor

        public CodeSegment() {
            _codesegment = new SimpleSegment();
            _type = Token.Unknown;
            _line = -1;
            _next = null;
            _previous = null;
        }

        public CodeSegment(Token token, string tokenstr, SimpleSegment anchorsegment, int line, int colstart, CodeSegment previous) {
            _type = token;
            _tokenstring = tokenstr;
            _codesegment = anchorsegment;
            _line = line;
            _column = colstart;
            _previous = previous;
        }

        #endregion

        #region Access neightbour CodeSegments

        /// <summary>
        /// Find the next CodeSegment starting on this which has the given specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CodeSegment NextOf(Token type) {
            if(this.Next == null)
                return null;
            else if(this.Next.Token == type)
                return this.Next;
            else
                return this.Next.NextOf(type);
        }

        /// <summary>
        /// Return the Next token by ignoring all given tokens
        /// </summary>
        /// <param name="tokenstoignore">Tokens to ignore</param>
        /// <returns></returns>
        public CodeSegment NextOmit(List<Token> tokenstoignore) {
            if(this.Next == null)
                return null;
            else if(!tokenstoignore.Contains(this.Next.Token))
                return this.Next;
            else
                return this.Next.NextOmit(tokenstoignore);
        }

        /// <summary>
        /// Return the previous codesegment by ignoring all tokens in the ignorelist
        /// </summary>
        /// <param name="tokenstoignore"></param>
        /// <returns></returns>
        public CodeSegment PreviousOmit(List<Token> tokenstoignore) {
            if(this.Previous == null)
                return null;
            else if(!tokenstoignore.Contains(this.Previous.Token))
                return this.Previous;
            else
                return this.Previous.PreviousOmit(tokenstoignore);
        }


        /// <summary>
        /// Return the this or one of the following token depending on the token ignore list
        /// </summary>
        /// <param name="tokenstoignore"></param>
        /// <returns></returns>
        public CodeSegment ThisOrNextOmit(List<Token> tokenstoignore) {
            if(tokenstoignore.Contains(_type)) {
                return NextOmit(tokenstoignore);
            } else
                return this;
        }

        public CodeSegment FindNextOnSameLine(Token tokeoFind) {
            if(this.Next != null) {
                if(Next.Token == tokeoFind)
                    return Next;
                else if(Next.Token == Token.NewLine)
                    return null;
                else
                    return Next.FindNextOnSameLine(tokeoFind);
            } else
                return null;
        }

        public CodeSegment FindNext(Token tokeoFind) {
            return FindNext(tokeoFind, null);
        }

        public CodeSegment FindNext(Token tokeoFind, List<Token> endTokens) {
            if(this.Next != null) {
                if(Next.Token == tokeoFind)
                    return Next;
                else if(endTokens != null && endTokens.Contains(Next.Token))
                    return null;
                else
                    return Next.FindNextOnSameLine(tokeoFind);
            } else
                return null;
        }

        public CodeSegment FindThisOrNext(Token tokeoFind, List<Token> endTokens) {
            if(this.Token == tokeoFind)
                return this;
            else if(endTokens != null && endTokens.Contains(this.Token))
                return null;
            else
                return this.FindNextOnSameLine(tokeoFind);
        }



        #region Braket Finder Helper


        public CodeSegment FindOtherBracked(bool allowNewlinesbetween) {
            if(TokenHelper.IsOpenBracketToken(this.Token)) {
                return FindClosingBracked(allowNewlinesbetween);
            } else if(TokenHelper.IsClosedBracketToken(this.Token)) {
                return FindOpenBracked(allowNewlinesbetween);
            } else
                throw new NotSupportedException("Must be called on a Bracked Token Code Segment!");
        }


        /// <summary>
        /// Very handy Method to find the Closing Bracket Codesegment of this Open Braket Segment
        /// </summary>
        /// <param name="allowNewlinesbetween">Should the search go over newlines</param>
        /// <returns>The cloasing codesegment or NULL if nothing was found</returns>
        public CodeSegment FindClosingBracked(bool allowNewlinesbetween) {
            if(!TokenHelper.IsOpenBracketToken(this.Token))
                throw new NotSupportedException("Must be called on open barcket type");

            int openBracketCounter = 1;
            CodeSegment current;
            CodeSegment previous = this;
            CodeSegment closingSegment = null;

            Token openbracket = this.Token;
            Token closingbracket = TokenHelper.GetClosingToken(this.Token);

            while((current = previous.Next) != null) {
                if(current.Token == Token.NewLine && !allowNewlinesbetween)
                    break;
                else if(current.Token == openbracket)
                    openBracketCounter++;
                else if(current.Token == closingbracket) {
                    openBracketCounter--;
                    if(openBracketCounter == 0) {
                        closingSegment = current;
                        break;
                    }
                }
                previous = current;
            }
            return closingSegment;
        }

        /// <summary>
        /// Very handy Method to find the Open Bracket Codesegment of this Close Braket Segment
        /// </summary>
        /// <param name="allowNewlinesbetween">Should the search go over newlines</param>
        /// <returns>The cloasing codesegment or NULL if nothing was found</returns>
        public CodeSegment FindOpenBracked(bool allowNewlinesbetween) {
            if(!TokenHelper.IsClosedBracketToken(this.Token))
                throw new NotSupportedException("Must be called on closed barcket type");

            int openBracketCounter = 1;
            CodeSegment current;
            CodeSegment previous = this;
            CodeSegment closingSegment = null;

            Token closingbracket = this.Token;
            Token openbracket = TokenHelper.GetOpenToken(this.Token);

            while((current = previous.Previous) != null) {
                if(current.Token == Token.NewLine && !allowNewlinesbetween)
                    break;
                else if(current.Token == closingbracket)
                    openBracketCounter++;
                else if(current.Token == openbracket) {
                    openBracketCounter--;
                    if(openBracketCounter == 0) {
                        closingSegment = current;
                        break;
                    }
                }
                previous = current;
            }
            return closingSegment;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Underlying Token
        /// </summary>
        public Token Token {
            get { return _type; }
        }

        /// <summary>
        /// Source Code String
        /// </summary>
        public string TokenString {
            get { return _tokenstring; }
        }

        /// <summary>
        /// Start Column of this CodeSegment
        /// </summary>
        public int ColumnStart {
            get { return _column; }
        }

        /// <summary>
        /// Line number of this Segment
        /// </summary>
        public int LineNumber {
            get { return _line; }
        }

        /// <summary>
        /// Segment - Location / Range
        /// </summary>
        public SimpleSegment Range {
            get { return _codesegment; }
        }

        /// <summary>
        /// The CodeDOM Member which represents this Token in the AST
        /// </summary>
        public CodeObject CodeDOMObject {
            get { return _codeDOMObject; }
            set { _codeDOMObject = value; }
        }

        CodeError _erorContext = null;

        public CodeError ErrorContext {
            get { return _erorContext; }
            set { _erorContext = value; }
        }

        public bool HasError {
            get { return ErrorContext != null && ErrorContext.HasError; }
        }



        /// <summary>
        /// Get the next CodeSegment -> Look Forward
        /// </summary>
        public CodeSegment Next {
            get { return _next; }
            set { _next = value; }
        }

        /// <summary>
        /// Get the previous CodeSegment -> Look Backwards
        /// </summary>
        public CodeSegment Previous {
            get { return _previous; }
        }

        #endregion

        /// <summary>
        /// Get a empty/undefined CodeSegment
        /// </summary>
        public static CodeSegment Empty {
            get {
                return new CodeSegment(Token.Unknown, "", new SimpleSegment(), 0, 0, null);
            }
        }

        public override string ToString() {
            return string.Format("{0}::{1}", _type, _tokenstring);
        }
    }
}
