using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System.Windows;
using System.Windows.Media;
using SmartDevelop.Model.Projecting;
using SmartDevelop.TokenizerBase;

namespace SmartDevelop.ViewModel.BackgroundRenderer
{

    public class CurrentLineHighlightRenderer : IBackgroundRenderer
    {
        #region Fields

        TextEditor _editor;
        Pen _borderPen = null; /*new Pen(new SolidColorBrush(Colors.Red), 1);*/
        Brush _lineSelection = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xCC, 0x33));
        Brush _braketMatch = new SolidColorBrush(Colors.Green);
        Brush _braketMatchFail = new SolidColorBrush(Colors.Pink);
        ProjectItemCode _projectitem;
        int lastLine = -1;

        #endregion

        public CurrentLineHighlightRenderer(TextEditor editor, ProjectItemCode projectitem) {
            _editor = editor;
            _projectitem = projectitem;

            _editor.TextArea.Caret.PositionChanged += (s, e) => Invalidate();
        }

        public KnownLayer Layer {
            get { return KnownLayer.Text; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext) {
            textView.EnsureVisualLines();

            #region Highlight Current Line
            
            
            if(lastLine != _editor.TextArea.Caret.Line) {
                var line = _editor.Document.GetLineByOffset(_editor.CaretOffset);
                var segment = new TextSegment { StartOffset = line.Offset, EndOffset = line.EndOffset };
                foreach(Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment)) {
                    drawingContext.DrawRoundedRectangle(_lineSelection, _borderPen, new Rect(r.Location, new Size(textView.ActualWidth, r.Height)), 3, 3);
                }
            }

            #endregion

            #region Highlight Brackets

            if(_editor.TextArea.Caret.Offset > 1 && TokenHelper.BRAKETS.ContainsKey(_editor.TextArea.Document.GetCharAt(_editor.TextArea.Caret.Offset - 1))) {
                var offset = _editor.TextArea.Caret.Offset;
                var seg = _projectitem.SegmentService.QueryCodeSegmentAt(offset);
                if(TokenHelper.BRAKETS.ContainsValue(seg.Token)) {
                    var other = seg.FindOtherBracked(true);

                    var col = other != null ? _braketMatch : _braketMatchFail;
                    var braketsegment = new TextSegment { StartOffset = seg.Range.Offset, EndOffset = seg.Range.EndOffset };
                    foreach(Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, braketsegment)) {
                        drawingContext.DrawRectangle(col, _borderPen, r);
                    }

                    
                    if(other != null) {
                        braketsegment = new TextSegment { StartOffset = other.Range.Offset, EndOffset = other.Range.EndOffset };
                        foreach(Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, braketsegment)) {
                            drawingContext.DrawRectangle(_braketMatch, _borderPen, r);
                        }
                    }
                }
            }

            #endregion

        }

        void Invalidate() {
            _editor.TextArea.TextView.InvalidateLayer(this.Layer);
        }
    }
}
