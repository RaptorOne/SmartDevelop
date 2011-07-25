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
using SmartDevelop.Model.DOM.Types;

namespace SmartDevelop.ViewModel.TextTransformators
{
    class ContextHighlightTransformator : DocumentColorizingTransformer
    {
        ProjectItemCode _codeProject;
        Brush _classtypeBrush = new SolidColorBrush(Colors.CadetBlue);
        Brush _errorBrush = new SolidColorBrush(Color.FromArgb(0xAA, 0xFF, 0x00, 0x00));

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


                if(segment.HasError) {
                    HandleSegmentError(segment);
                } else if(segment.CodeDOMObject is CodeMethodReferenceExpressionEx) {
                   

                        var methodRef = segment.CodeDOMObject as CodeMethodReferenceExpressionEx;
                        var ctx = _codeProject.Project.DOMService.GetCodeContext(_codeProject, segment.Range.Offset);
                        var method = methodRef.FindCached(ctx);

                        if(method != null) {
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
                            } catch {

                            }
                        } else {
                            segment.ErrorContext = new CodeError() {  Message="Methodreference not found."};
                            HandleSegmentError(segment);
                        }
                            

                } else if(segment.Token != Token.KeyWord && segment.CodeDOMObject is CodeTypeReferenceEx) {

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

        void HandleSegmentError(CodeSegment segment) {
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
                    element.TextRunProperties.SetBackgroundBrush(_errorBrush);
                });
            } catch {
            }
        }
    }
}
