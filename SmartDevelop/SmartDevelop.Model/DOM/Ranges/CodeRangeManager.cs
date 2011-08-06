using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Ranges
{
    /// <summary>
    /// Thread save Range Manager
    /// </summary>
    public class CodeRangeManager
    {
        List<CodeRange> _ranges = new List<CodeRange>();


        /// <summary>
        /// Add a Range to this Manager
        /// </summary>
        /// <param name="range"></param>
        public void Add(CodeRange range) {
            lock(_ranges) {
                _ranges.Add(range);
            }
        }

        /// <summary>
        /// Clears all Ranges
        /// </summary>
        public void Clear() {
            lock(_ranges) {
                _ranges.Clear();
            }
        }

        /// <summary>
        /// Find the innerst Range 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public CodeRange FindInnerstRange(int offset) {
            return FindEncapsulatingRanges(offset).Any() ? FindEncapsulatingRanges(offset).First() : null;
        }

        /// <summary>
        /// Get all CodeRanges which enclose the given offset. The Ranges are ordered from the innerst to the most outer.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public IEnumerable<CodeRange> FindEncapsulatingRanges(int offset) {
            lock(_ranges) {
                return (from r in _ranges
                       where r.SegmentRange.Offset < offset && r.SegmentRange.EndOffset > offset
                       orderby r.SegmentRange.Length
                       select r).ToList();
            }
        }

    }



}
