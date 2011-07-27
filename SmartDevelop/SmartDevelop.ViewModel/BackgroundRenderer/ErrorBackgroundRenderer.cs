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
        readonly ProjectItemCode _codeitem;
        readonly TextEditor _editor;

        Pen _errorPen = new Pen(new SolidColorBrush(Colors.Red), 1);
        Brush _errorBrush = new SolidColorBrush(Colors.Red);
        

        #endregion

        public ErrorBackgroundRenderer(TextEditor editor, ProjectItemCode projectitem) {
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
                foreach(Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, err.Segment.Range)) {
                    //drawingContext.DrawRectangle(null, _errorPen, r);
                    drawingContext.DrawLine(_errorPen, r.BottomLeft, r.BottomRight);
                }
            }

            #endregion
        }

        void Invalidate() {
            _editor.TextArea.TextView.InvalidateLayer(this.Layer);
        }
    }
}
