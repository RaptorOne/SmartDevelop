using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.CodeDom;
using System.Windows.Input;
using SmartDevelop.Model.DOM.Types;

namespace SmartDevelop.ViewModel.CodeCompleting
{
    public class CompletionItem : ICompletionData
    {
        #region Fields

        string _text;
        string _description;

        #endregion

        public static CompletionItem Build(CodeObject m) {
            if(m is CodeMemberMethodEx) {
                var method = m as CodeMemberMethodEx;
                return new CompletionItemMethod(method.Name, string.Format("Method {0}({1})\n{2}", method.Name, method.GetParamInfo(), GetDocumentCommentString(method.Comments)));
            }else if(m is CodeTypeDeclaration && ((CodeTypeDeclaration)m).IsClass) {
                var classdecl = ((CodeTypeDeclaration)m);
                return new CompletionItemClass(classdecl.Name, string.Format("class {0}\n{1}", classdecl.Name, GetDocumentCommentString(classdecl.Comments)));
            }else if(m is CodeMemberProperty) {
                var prop = ((CodeMemberProperty)m);
                if((prop.Attributes & MemberAttributes.Static) == MemberAttributes.Static)
                    return new CompletionItemPropertyStatic(prop.Name, string.Format("Property {0}\n{1}", prop.Name, GetDocumentCommentString(prop.Comments)));
                else
                    return new CompletionItemProperty(prop.Name, string.Format("Property {0}\n{1}", prop.Name, GetDocumentCommentString(prop.Comments)));
                
            }else if(m is CodeMemberField) {
                var prop = ((CodeMemberField)m);
                return new CompletionItemField(prop.Name, string.Format("Field {0}\n{1}", prop.Name, GetDocumentCommentString(prop.Comments)));
            } else if(m is CodeParameterDeclarationExpression) {
                var argument = m as CodeParameterDeclarationExpression;
                return new CompletionItemField(argument.Name, string.Format("Argument {0}", argument.Name));
            } else
                throw new NotSupportedException("Cant handle obj type: " + m.GetType().ToString());

            //return /* new CompletionItem(m.ToString(), ""); */
        }

        public static string GetDocumentCommentString(CodeCommentStatementCollection comments) {
            var info = new StringBuilder();
            foreach(CodeCommentStatement com in comments) {
                if(com.Comment.DocComment)
                    info.AppendLine(com.Comment.Text);
            }
            return info.ToString();
        }

        //static string GetParamInfo(CodeParameterDeclarationExpressionCollection parsams) {
        //    string str = "";
        //    foreach(CodeParameterDeclarationExpression p in parsams) {
        //        str += p.Name + ", ";
        //    }
        //    return str;
        //}


        public CompletionItem(string text, string description) {
            _text = text;
            _description = description;
        }

        public virtual ImageSource Image {
            get { return null; }
        }

        public virtual string Text {
            get { return _text; }
        }

        public virtual object Content {
            get { return _text; }
        }

        public virtual object Description {
            get { return _description; }
        }

        public virtual double Priority {
            get { return 1; }
        }

        public virtual void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs) {
            var last = textArea.Document.GetCharAt(completionSegment.Offset-1);
            var e = insertionRequestEventArgs as TextCompositionEventArgs;
            if(e != null && e.Text == ".")
                return;

            ISegment replaceSegment = completionSegment;
            if(last != '.' && last != ' ') {
                replaceSegment = SubSegment(completionSegment);
            }
            textArea.Document.Replace(replaceSegment, this.Text);
        }

        ISegment SubSegment(ISegment segment) {
            return new SimpleSegment(segment.Offset-1, segment.Length+1);;
        }

    }
}
