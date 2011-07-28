using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.CodeLanguages;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeMethodReferenceExpressionEx : CodeMethodReferenceExpression, ICodeContext
    {
        #region Fields

        protected CodeMemberMethodEx _methodref;
        SmartCodeProject _project;
        Projecting.ProjectItemCode _codeDocumentItem;

        #endregion

        #region Constructor

        public CodeMethodReferenceExpressionEx(ProjectItemCode codeDocumentItem, CodeExpression target, string methodName, CodeTypeDeclarationEx enclosingType) 
            : base(target, methodName) {
                _enclosingType = enclosingType;
                _codeDocumentItem = codeDocumentItem;
        }

        #endregion

        #region Properties

        public CodeMemberMethodEx ResolvedMethodMember {
            get { return _methodref; }
            set { _methodref = value; }
        }

        CodeTypeDeclarationEx _enclosingType;
        public CodeTypeDeclarationEx EnclosingType {
            get { return _enclosingType; }
        }



        public ProjectItemCode CodeDocumentItem {
            get { return _codeDocumentItem; }
            set { _codeDocumentItem = value; }
        }

        public SmartCodeProject Project {
            get {
                return (CodeDocumentItem != null) ? CodeDocumentItem.Project : _project;
            }
            set {
                _project = value;
            }
        }

        protected CodeLanguage Language {
            get {
                return Project != null ? Project.Language : null;
            }
        }


        public string CommentInfo {
            get {
                if(ResolvedMethodMember != null) {
                    return GetDocumentCommentString(ResolvedMethodMember.Comments);
                }
                return null;
            }
        }

        #endregion

        #region Static Methods

        public static string GetDocumentCommentString(CodeCommentStatementCollection comments) {
            var info = new StringBuilder();
            foreach(CodeCommentStatement com in comments) {
                if(com.Comment.DocComment)
                    info.AppendLine(com.Comment.Text);
            }
            return info.ToString();
        }

        #endregion

        public virtual CodeMemberMethodEx ResolveMethodDeclarationCache() {
            var lang = Language;


            if(_methodref == null && EnclosingType != null) {
                CodeTypeDeclarationEx typedecl = EnclosingType;

                var members = from m in typedecl.GetInheritedMembers()
                                let memberMethod = m as CodeMemberMethodEx
                                where memberMethod != null && memberMethod.Name.Equals(this.MethodName, lang.NameComparisation)
                                select memberMethod;

                if(members.Any())
                    _methodref = members.First();
            }

            if(_methodref == null) {
                var p = Project;
                if(p != null && p.DOMService.RootType != EnclosingType) {

                    var members = from member in p.DOMService.RootType.Members.Cast<CodeTypeMember>()
                                  let methodMember = member as CodeMemberMethodEx
                                  where methodMember != null && methodMember.Name.Equals(this.MethodName, lang.NameComparisation)
                                  select methodMember;

                    if(members.Any())
                        _methodref = members.First();
                }
            }
            return _methodref;
        }

        public override string ToString() {
            return "CodeMethodReferenceExpression: " + this.MethodName + "\n" + CommentInfo;
        }



    }
}
