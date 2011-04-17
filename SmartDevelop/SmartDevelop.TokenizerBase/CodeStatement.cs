using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit.Document;

namespace SmartDevelop.TokenizerBase
{
    public class CodeSegment
    {
        SimpleSegment _codesegment;
        Token _type;
        string _tokenstring;

        public CodeSegment(Token token, string tokenstr, SimpleSegment anchorsegment) {
            _type = token;
            _tokenstring = tokenstr;
            _codesegment = anchorsegment;
        }

        public Token Type {
            get { return _type; }
        }

        public string TokenString {
            get { return _tokenstring; }
        }

        public SimpleSegment Range {
            get { return _codesegment; }
        }

        public override string ToString() {
            return string.Format("{0}::{1}", _type, _tokenstring);
        }


        public static CodeSegment Empty {
            get {
                return new CodeSegment(Token.Unknown, "", new SimpleSegment());
            }
        }
    }
}
