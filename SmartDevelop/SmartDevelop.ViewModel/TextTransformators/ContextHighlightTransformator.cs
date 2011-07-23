using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;
using System.Windows;
using SmartDevelop.Model.Projecting;
using System.CodeDom;

namespace SmartDevelop.ViewModel.TextTransformators
{
    class ContextHighlightTransformator : DocumentColorizingTransformer
    {
        ProjectItemCode _codeProject;
        public ContextHighlightTransformator(ProjectItemCode codeProject) {
            _codeProject = codeProject;
        }


        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line) {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            var codeline = _codeProject.SegmentService.QueryCodeTokenLine(line.LineNumber-1);

            if(codeline.IsEmpty)
                return;

            foreach(var segment in codeline.CodeSegments) {

                if(segment.CodeDOMObject is CodeMethodReferenceExpression) {

                    try {
                        base.ChangeLinePart(
                        segment.Range.Offset, // startOffset
                        segment.Range.EndOffset, // endOffset
                        (VisualLineElement element) => {
                            // This lambda gets called once for every VisualLineElement
                            // between the specified offsets.
                            Typeface tf = element.TextRunProperties.Typeface;
                            // Replace the typeface with a modified version of
                            // the same typeface
                            element.TextRunProperties.SetTypeface(new Typeface(
                                tf.FontFamily,
                                FontStyles.Italic,
                                FontWeights.Bold,
                                tf.Stretch
                            ));
                        });
                    }catch{

                    }

                }
      
            }

        }
    }
}
