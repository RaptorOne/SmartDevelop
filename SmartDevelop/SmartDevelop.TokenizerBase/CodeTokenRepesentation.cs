using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;

namespace SmartDevelop.TokenizerBase.IA
{
    /// <summary>
    /// Threadsafe CodeToken Provider
    /// </summary>
    public class CodeTokenRepesentation
    {
        #region Fields

        readonly object _codesegmentsLock = new object();
        readonly List<CodeSegment> _codesegments = new List<CodeSegment>();

        #endregion

        public CodeTokenRepesentation() { }

        #region CodeSegment Access

        public void Clear(){
            lock(_codesegmentsLock) {
                _codesegments.Clear();
            }
        }

        //public void Add(CodeSegment segment) {
        //    lock(_codesegmentsLock) {
        //        _codesegments.Add(segment);
        //    }
        //}

        //public void Remove(CodeSegment segment) {
        //    lock(_codesegmentsLock) {
        //        _codesegments.Remove(segment);
        //    }
        //}

        public void Reset(List<CodeSegment> newtokens) {
            lock(_codesegmentsLock) {
                _codesegments.Clear();
                _codesegments.AddRange(newtokens);
            }
        }

        public int IndexOf(CodeSegment segment) {
            lock(_codesegmentsLock) {
                return _codesegments.IndexOf(segment);
            }
        }

        public int LastIndexOf(CodeSegment segment) {
            lock(_codesegmentsLock) {
                return _codesegments.LastIndexOf(segment);
            }
        }

        public IEnumerable<CodeSegment> GetSegments() {
            return new List<CodeSegment>(_codesegments);
        }

        #endregion

        #region Query Methods

        public CodeSegment QueryCodeSegmentAt(int offset) {
            CodeSegment t = CodeSegment.Empty;
            lock(_codesegmentsLock) {
                foreach(var segment in _codesegments) {
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

        #endregion


    }
}
