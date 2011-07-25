using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeMethodReferenceExpressionEx : CodeMethodReferenceExpression
    {
        CodeMemberMethodEx _methodref;

        public CodeMethodReferenceExpressionEx(CodeExpression target, string methodName, CodeTypeDeclarationEx enclosingType) 
            : base(target, methodName) {
                _enclosingType = enclosingType;
        }

        public CodeMemberMethodEx ResolvedMethodMember {
            get { return _methodref; }
            set { _methodref = value; }
        }

        CodeTypeDeclarationEx _enclosingType;
        public CodeTypeDeclarationEx EnclosingType {
            get { return _enclosingType; }
        }



        public string CommentInfo {
            get {
                if(ResolvedMethodMember != null) {
                    return GetDocumentCommentString(ResolvedMethodMember.Comments);
                }
                return null;
            }
        }

        public static string GetDocumentCommentString(CodeCommentStatementCollection comments) {
            var info = new StringBuilder();
            foreach(CodeCommentStatement com in comments) {
                if(com.Comment.DocComment)
                    info.AppendLine(com.Comment.Text);
            }
            return info.ToString();
        }


        public CodeMemberMethodEx ResolveMethodDeclarationCache() {

            if(_methodref == null && EnclosingType != null) {
                CodeTypeDeclarationEx typedecl = EnclosingType;

                var members = from m in typedecl.GetInheritedMembers()
                                let memberMethod = m as CodeMemberMethodEx
                                where memberMethod != null && memberMethod.Name.Equals(this.MethodName, StringComparison.CurrentCultureIgnoreCase)
                                select memberMethod;

                if(members.Any())
                    _methodref = members.First();

            }
            return _methodref;
        }

        public override string ToString() {
            return "CodeMethodReferenceExpression: " + this.MethodName + "\n" + CommentInfo;
        }

    }
}
