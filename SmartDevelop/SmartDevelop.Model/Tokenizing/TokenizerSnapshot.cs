using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Tokenizing
{
    public class TokenizerSnapshot : IEnumerable<CodeSegment>
    {
        readonly List<CodeSegment> _codesegments;
        readonly List<CodeSegment> _directiveSegments;


        public TokenizerSnapshot(IEnumerable<CodeSegment> codesegments, IEnumerable<CodeSegment> directives) {
            _codesegments = new List<CodeSegment>(codesegments);
            _directiveSegments = new List<CodeSegment>(directives);
        }

        /// <summary>
        /// Returns all CodeSegments which are Directive Tokens
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CodeSegment> GetDirectives() {
            return _directiveSegments;
        }

        public IEnumerator<CodeSegment> GetEnumerator() {
            return _codesegments.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _codesegments.GetEnumerator();
        }
    }
}
