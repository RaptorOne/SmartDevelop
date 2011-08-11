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
using SmartDevelop.Model.Tokenizing;
using SmartDevelop.Model.DOM;

namespace SmartDevelop.ViewModel.TextTransformators
{
    class ContextHighlightTransformator : DocumentColorizingTransformer
    {
        #region Fields

        readonly ProjectItemCodeDocument _codeProject;
        Brush _classtypeBrush = new SolidColorBrush(Colors.CadetBlue);
        Brush _stringBrush = new SolidColorBrush(Colors.Crimson);
        Brush _errorBrush = new SolidColorBrush(Color.FromArgb(0xAA, 0xFF, 0x00, 0x00));

        #endregion

        public ContextHighlightTransformator(ProjectItemCodeDocument codeProject) {
            _codeProject = codeProject;
        }

        #region Colorizer

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line) {

            if(_codeProject.IsDocumentDirty)
                return;
            
            var codeline = _codeProject.SegmentService.QueryCodeTokenLine(line.LineNumber);

            if(codeline.IsEmpty)
                return;
            int lineStartOffset = line.Offset;
            
            foreach(var segment in codeline.CodeSegments) {


                if(segment.Token == Token.TraditionalString) {

                    if(segment.Range.Offset >= lineStartOffset && segment.Range.EndOffset <= line.EndOffset) {


                        base.ChangeLinePart(
                            segment.Range.Offset, // startOffset
                            segment.Range.EndOffset, // endOffset
                            (VisualLineElement element) => {
                                Typeface tf = element.TextRunProperties.Typeface;
                                element.TextRunProperties.SetForegroundBrush(_stringBrush);
                            });
                    }

                } else if(segment.CodeDOMObject is CodeMethodReferenceExpressionEx) {

                    var methodRef = segment.CodeDOMObject as CodeMethodReferenceExpressionEx;
                    if(methodRef.ResolvedMethodMember != null) {

                        if(segment.Range.Offset >= lineStartOffset && segment.Range.EndOffset <= line.EndOffset) {
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
                        }
                    }

                } else if(segment.CodeDOMObject is CodeTypeReferenceEx) {

                    if(((CodeTypeReferenceEx)segment.CodeDOMObject).ResolvedTypeDeclaration != null) {


                        if(segment.Range.Offset >= lineStartOffset && segment.Range.EndOffset <= line.EndOffset) {
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

                        }

                    }
                }
            }

        }

        #endregion
    }
}
