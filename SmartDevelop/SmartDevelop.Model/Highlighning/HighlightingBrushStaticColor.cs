using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Media;

namespace SmartDevelop.Model.Highlighning
{
    public class HighlightingBrushStaticColor : HighlightingBrush
    {
        readonly Brush _brush;

        public HighlightingBrushStaticColor(Brush b) {
            _brush = b;
        }

        public override Brush GetBrush(ICSharpCode.AvalonEdit.Rendering.ITextRunConstructionContext context) {
            return _brush;
        }
    }
}
