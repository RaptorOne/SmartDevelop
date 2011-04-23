using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit.Document;

namespace SmartDevelop.TokenizerBase
{
    public class CodeSegment
    {
        #region Fields

        SimpleSegment _codesegment;
        Token _type;
        string _tokenstring;
        readonly int _line;
        CodeSegment _next;
        readonly CodeSegment _previous;

        #endregion

        CodeSegment() {
            _codesegment = new SimpleSegment();
            _type = Token.Unknown;
            _line = -1;
            _next = null;
            _previous = null;
        }

        public CodeSegment(Token token, string tokenstr, SimpleSegment anchorsegment, int line, CodeSegment previous) {
            _type = token;
            _tokenstring = tokenstr;
            _codesegment = anchorsegment;
            _line = line;
            _previous = previous;
        }

        #region Access neightbour CodeSegments

        /// <summary>
        /// Find the next CodeSegment starting on this which has the given specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CodeSegment NextOf(Token type) {
            if(this.Next == null)
                return null;
            else if(this.Next.Type == type)
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
            else if(!tokenstoignore.Contains(this.Next.Type))
                return this.Next;
            else
                return this.Next.NextOmit(tokenstoignore);
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
                if(Next.Type == tokeoFind)
                    return Next;
                else
                    return Next.FindNextOnSameLine(tokeoFind);
            } else
                return null;
        }

        /// <summary>
        /// Very handy Method to find the Closing Bracket Codesegment of this Segment
        /// </summary>
        /// <param name="allowNewlinesbetween">Should the search go over newlines</param>
        /// <returns>The cloasing codesegment or NULL if nothing was found</returns>
        public CodeSegment FindClosingBracked(bool allowNewlinesbetween) {
            if(!TokenHelper.IsOpenBracketToken(this.Type))
                throw new NotSupportedException("Must be called on open barcket type");

            int openBracketCounter = 1;
            CodeSegment current;
            CodeSegment previous = this;
            CodeSegment closingSegment = null;

            Token openbracket = this.Type;
            Token closingbracket = TokenHelper.GetClosingToken(this.Type);

            while((current = previous.Next) != null) {
                if(current.Type == Token.NewLine && !allowNewlinesbetween)
                    break;
                else if(current.Type == openbracket)
                    openBracketCounter++;
                else if(current.Type == closingbracket) {
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

        #region Properties

        public Token Type {
            get { return _type; }
        }

        public string TokenString {
            get { return _tokenstring; }
        }

        public int Line {
            get { return _line; }
        }

        public SimpleSegment Range {
            get { return _codesegment; }
        }

        public override string ToString() {
            return string.Format("{0}::{1}", _type, _tokenstring);
        }

        public CodeSegment Next {
            get { return _next; }
            internal set { _next = value; }
        }

        public CodeSegment Previous {
            get { return _previous; }
        }

        #endregion

        public static CodeSegment Empty {
            get {
                return new CodeSegment(Token.Unknown, "", new SimpleSegment(), 0, null);
            }
        }
    }
}
