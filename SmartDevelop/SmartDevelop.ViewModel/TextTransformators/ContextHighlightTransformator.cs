using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;
using System.Windows;
using SmartDevelop.Model.Projecting;
using System.CodeDom;
using SmartDevelop.TokenizerBase;

namespace SmartDevelop.ViewModel.TextTransformators
{
    class ContextHighlightTransformator : DocumentColorizingTransformer
    {
        ProjectItemCode _codeProject;
        Brush _classtypeBrush = new SolidColorBrush(Colors.CadetBlue);


        public ContextHighlightTransformator(ProjectItemCode codeProject) {
            _codeProject = codeProject;
        }


        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line) {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            var codeline = _codeProject.SegmentService.QueryCodeTokenLine(line.LineNumber);

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
                } else if(segment.Token != Token.KeyWord && segment.CodeDOMObject is CodeTypeReference) {

                    var ctx = _codeProject.Project.DOMService.GetCodeContext(_codeProject, segment.Range.Offset);
                    var memb = from m in ctx.GetVisibleMembers()
                               let myclass = m as CodeTypeDeclaration
                               where myclass != null && myclass.Name.Equals(segment.TokenString)
                               select m;

                    if(memb.Any()) {
                        try {
                            base.ChangeLinePart(
                            segment.Range.Offset, // startOffset
                            segment.Range.EndOffset, // endOffset
                            (VisualLineElement element) => {
                                Typeface tf = element.TextRunProperties.Typeface;
                                element.TextRunProperties.SetForegroundBrush(_classtypeBrush);
                                element.TextRunProperties.SetTypeface(new Typeface(
                                    tf.FontFamily,
                                    FontStyles.Normal,
                                    FontWeights.Bold,
                                    tf.Stretch
                                ));
                            });
                        } catch {

                        }
                    }
                }
            }

        }
    }
}
