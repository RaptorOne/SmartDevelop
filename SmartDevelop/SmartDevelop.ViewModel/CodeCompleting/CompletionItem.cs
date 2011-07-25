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

namespace SmartDevelop.ViewModel.CodeCompleting
{
    public class CompletionItem : ICompletionData
    {
        #region Fields

        string _text;
        string _description;

        #endregion

        public static CompletionItem Build(object m) {
            if(m is CodeMemberMethod) {
                var method = m as CodeMemberMethod;
                return new CompletionItemMethod(method.Name, string.Format("Method {0}\n{1}", GetDocumentCommentString(method.Comments), GetParamInfo(method.Parameters)));
            }

            if(m is CodeTypeDeclaration && ((CodeTypeDeclaration)m).IsClass) {
                var classdecl = ((CodeTypeDeclaration)m);
                return new CompletionItemClass(classdecl.Name, string.Format("class {0}\n{1}", classdecl.Name, GetDocumentCommentString(classdecl.Comments)));
            }

            if(m is CodeMemberProperty) {
                var prop = ((CodeMemberProperty)m);
                return new CompletionItemProperty(prop.Name, string.Format("class {0}\n{1}", prop.Name, GetDocumentCommentString(prop.Comments)));
            }

            if(m is CodeMemberField) {
                var prop = ((CodeMemberField)m);
                return new CompletionItemField(prop.Name, string.Format("class {0}\n{1}", prop.Name, GetDocumentCommentString(prop.Comments)));
            }


            return new CompletionItem(m.ToString(), "");
        }

        public static string GetDocumentCommentString(CodeCommentStatementCollection comments) {
            var info = new StringBuilder();
            foreach(CodeCommentStatement com in comments) {
                if(com.Comment.DocComment)
                    info.AppendLine(com.Comment.Text);
            }
            return info.ToString();
        }

        static string GetParamInfo(CodeParameterDeclarationExpressionCollection parsams) {
            string str = "";
            foreach(CodeParameterDeclarationExpression p in parsams) {
                str += p.Name + ", ";
            }
            return str;
        }


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
