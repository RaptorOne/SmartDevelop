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

        protected CodeMemberMethodEx _methodDeclaration;
        SmartCodeProject _project;
        Projecting.ProjectItemCodeDocument _codeDocumentItem;

        #endregion

        #region Constructor

        public CodeMethodReferenceExpressionEx(ProjectItemCodeDocument codeDocumentItem, CodeExpression target, string methodName, CodeTypeDeclarationEx enclosingType) 
            : base(target, methodName) {
                _enclosingType = enclosingType;
                _codeDocumentItem = codeDocumentItem;
        }

        #endregion

        #region Properties

        public CodeMemberMethodEx ResolvedMethodMember {
            get { return _methodDeclaration; }
            set { _methodDeclaration = value; }
        }

        CodeTypeDeclarationEx _enclosingType;
        public CodeTypeDeclarationEx EnclosingType {
            get { return _enclosingType; }
        }



        public ProjectItemCodeDocument CodeDocumentItem {
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
                    return Helper.GetDocumentCommentString(ResolvedMethodMember.Comments);
                }
                return null;
            }
        }

        #endregion

        

        public virtual CodeMemberMethodEx ResolveMethodDeclarationCache() {
            var lang = Language;


            if(_methodDeclaration == null && EnclosingType != null) {
                CodeTypeDeclarationEx typedecl = EnclosingType;

                var members = from m in typedecl.GetInheritedMembers()
                                let memberMethod = m as CodeMemberMethodEx
                                where memberMethod != null && memberMethod.Name.Equals(this.MethodName, lang.NameComparisation)
                                select memberMethod;

                if(members.Any())
                    _methodDeclaration = members.First();
            }

            if(_methodDeclaration == null) {
                var p = Project;
                if(p != null && p.DOMService.RootTypeUnSave != EnclosingType) {

                    var members = from member in p.DOMService.RootTypeUnSave.Members.Cast<CodeTypeMember>()
                                  let methodMember = member as CodeMemberMethodEx
                                  where methodMember != null && methodMember.Name.Equals(this.MethodName, lang.NameComparisation)
                                  select methodMember;

                    if(members.Any())
                        _methodDeclaration = members.First();
                }
            }
            return _methodDeclaration;
        }

        public override string ToString() {
            return this.MethodName + "()\n" + CommentInfo;
        }



    }
}
