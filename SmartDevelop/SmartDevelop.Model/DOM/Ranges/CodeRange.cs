using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Ranges
{
    public class CodeRange
    {
        public CodeRange(ISegment segment, CodeObject rangedCodeObject) {
            SegmentRange = segment;
            RangedCodeObject = rangedCodeObject;
        }

        public ISegment SegmentRange {
            get;
            protected set;
        }

        public CodeObject RangedCodeObject {
            get;
            protected set;
        }
    }
}
