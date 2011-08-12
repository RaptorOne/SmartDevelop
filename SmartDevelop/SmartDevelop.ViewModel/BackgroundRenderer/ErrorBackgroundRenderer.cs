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

    public class ErrorBackgroundRenderer : IBackgroundRenderer
    {
        #region Fields

        readonly SmartSolution _solution;
        readonly ProjectItemCodeDocument _codeitem;
        readonly TextEditor _editor;

        Pen _errorPen = new Pen(new SolidColorBrush(Colors.Red), 1);
        Brush _errorBrush = new SolidColorBrush(Color.FromArgb(0x44, 0xFF, 0x00, 0x00));
        

        #endregion

        public ErrorBackgroundRenderer(TextEditor editor, ProjectItemCodeDocument projectitem) {
            _editor = editor;
            _codeitem = projectitem;
            _solution = projectitem.Project.Solution;

            //_editor.TextArea.Caret.PositionChanged += (s, e) => Invalidate();
        }

        public KnownLayer Layer {
            get { return KnownLayer.Text; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext) {
            textView.EnsureVisualLines();

            #region Highlight Error Tokens

            var errorsToVisualize = _solution.ErrorService.GetErrorsFromDocument(_codeitem);

            foreach(var err in errorsToVisualize) {
                if(err.Range != null) {
                    foreach(Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, err.Range)) {
                        //drawingContext.DrawRectangle(null, _errorPen, r);
                        drawingContext.DrawLine(_errorPen, r.BottomLeft, r.BottomRight);
                    }
                } else {

                    var line = _editor.Document.GetLineByNumber(err.StartLine);
                    if(line != null) {
                        var segment = new TextSegment { StartOffset = line.Offset, EndOffset = line.EndOffset };

                        foreach(Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment)) {
                            drawingContext.DrawRectangle(_errorBrush, _errorPen, r);
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
