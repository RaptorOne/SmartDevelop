using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.Model.Tokening
{
    /// <summary>
    /// Threadsafe CodeSegment Provider
    /// </summary>
    public class DocumentCodeSegmentService
    {
        #region Fields

        readonly object _codesegmentsLock = new object();
        readonly Dictionary<int, CodeTokenLine> _codeLineSegments = new Dictionary<int, CodeTokenLine>();
        IEnumerable<CodeSegment> _segments;
        readonly ProjectItemCode _codeitem;
        int _maxLine = 0;

        #endregion

        public DocumentCodeSegmentService(ProjectItemCode codeitem) {
            _codeitem = codeitem; 
        }

        public CodeTokenLine GetNextLine(int currentLine) {
            CodeTokenLine nextLine = new CodeTokenLine();
            for(int i = currentLine + 1; i <= _maxLine; i++) {
                if(_codeLineSegments.ContainsKey(i)) {
                    nextLine = _codeLineSegments[i];
                    break;
                }
            }
            return nextLine;
        }

        #region CodeSegment Access

        public void Reset(IEnumerable<CodeSegment> newtokens) {
            lock(_codesegmentsLock) {
                _segments = new List<CodeSegment>(newtokens);
                _codeLineSegments.Clear();
                foreach(var t in newtokens) {
                    if(_codeLineSegments.ContainsKey(t.LineNumber))
                        _codeLineSegments[t.LineNumber].Add(t);
                    else {
                        _codeLineSegments.Add(t.LineNumber, new CodeTokenLine(t));
                        _maxLine = t.LineNumber;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a swallow copy of the codeline map
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, CodeTokenLine> GetCodeSegmentLinesMap() {
            lock(_codesegmentsLock) {
                return new Dictionary<int, CodeTokenLine>(_codeLineSegments);
            }
        }

        /// <summary>
        /// returns a swallow copy of all segemnts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CodeSegment> GetSegments() {
            lock(_codesegmentsLock) {
                return new List<CodeSegment>(_segments);
            }
        }

        #endregion

        #region Query Methods

        //[Obsolete("Use line/column for much more performance")]
        public CodeSegment QueryCodeSegmentAt(int offset) {
            CodeSegment t = CodeSegment.Empty;
            lock(_codesegmentsLock) {

                if(_segments == null) return null;

                foreach(var segment in _segments) {
                    if(segment.Range.Offset <= offset && segment.Range.EndOffset >= offset) {
                        t = segment;
                        break;
                    } else if(segment.Range.Offset > offset) {
                        break;
                    }
                }
            }
            return t;
        }

        public CodeSegment QueryCodeSegmentAt(int line, int col) {
            var tokenline = QueryCodeTokenLine(line);
            return tokenline.GetSegmentAt(col);
        }

        /// <summary>
        /// Get the Line with Line-Number x
        /// </summary>
        /// <param name="line">Linenumber</param>
        /// <returns></returns>
        public CodeTokenLine QueryCodeTokenLine(int line) {
            CodeTokenLine tokline = new CodeTokenLine();
            if(_codeLineSegments.ContainsKey(line)) {
                tokline = _codeLineSegments[line];
            }
            return tokline;
        }

        #endregion
    }

    public struct CodeTokenLine
    {
        public CodeTokenLine(CodeSegment initialToken) 
            : this() {
            Line = initialToken.LineNumber;
            CodeSegments = new List<CodeSegment>() { initialToken };
        }

        public bool IsEmpty {
            get {
                return CodeSegments == null;
            }
        }

        public List<CodeSegment> CodeSegments { get; set; }
        public int Line { get; set;}

        public void Add(CodeSegment s) {
            CodeSegments.Add(s);
        }

        public override string ToString() {
            return string.Format("L{0}, [{1}] {{2}}", Line, CodeSegments[0], CodeSegments.Count);
        }

        public CodeSegment GetSegmentAt(int col) {
            if(CodeSegments != null) {
                foreach(var s in CodeSegments)
                    if(s.ColumnStart == col || (s.ColumnStart < col && (s.Next == null || s.Next.ColumnStart > col))) {
                        return s;
                    }
            }
            return null;
        }
    }
}
