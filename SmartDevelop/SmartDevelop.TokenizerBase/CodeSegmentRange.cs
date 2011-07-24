using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.TokenizerBase
{

    /// <summary>
    /// Defines a range in a CodeSegment-Stream
    /// </summary>
    public class CodeSegmentRange : IEnumerable<CodeSegment>
    {
        IEnumerator<CodeSegment> _enumerator;

        public CodeSegmentRange(CodeSegment start) {
            _enumerator = new CodeSegmentRangeEnumerator(start, null);
        }

        public CodeSegmentRange(CodeSegment start, CodeSegment end) {
            _enumerator = new CodeSegmentRangeEnumerator(start, end);
        }

        public IEnumerator<CodeSegment> GetEnumerator() {
            return _enumerator;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _enumerator;
        }

        class CodeSegmentRangeEnumerator : IEnumerator<CodeSegment>
        {
            readonly CodeSegment _start;
            readonly CodeSegment _end;
            CodeSegment _current;


            public CodeSegmentRangeEnumerator(CodeSegment start, CodeSegment end) {
                _start = start;
                _end = end;
                Reset();
            }

            public CodeSegment Current {
                get { return _current; }
            }

            object System.Collections.IEnumerator.Current {
                get { return Current; }
            }

            public bool MoveNext() {
                var next = _current.Next;
                if(next == null || next == _end)
                    return false;
                else {
                    _current = next;
                    return true;
                }
            }

            public void Reset() {
                _current = _start;
            }

            public void Dispose() {

            }
        }
    }
}
