using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.CodeLanguages;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodePropertyReferenceExpressionEx : CodePropertyReferenceExpression, ICodeContext
    {
        #region Fields

        CodeMemberPropertyEx _propertyDecl;
        SmartCodeProject _project;
        Projecting.ProjectItemCode _codeDocumentItem;

        #endregion

        #region Constructor

        public CodePropertyReferenceExpressionEx(ProjectItemCode codeDocumentItem, CodeExpression target, string propertyName, CodeTypeDeclarationEx enclosingType) 
            : base(target, propertyName) {
                _enclosingType = enclosingType;
                _codeDocumentItem = codeDocumentItem;
        }

        #endregion

        #region Properties

        public CodeMemberPropertyEx ResolvedPropertyMember {
            get { return _propertyDecl; }
            set { _propertyDecl = value; }
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

        CodeLanguage Language {
            get {
                return Project != null ? Project.Language : null;
            }
        }


        public string CommentInfo {
            get {
                if(ResolvedPropertyMember != null) {
                    return GetDocumentCommentString(ResolvedPropertyMember.Comments);
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

        public CodeMemberPropertyEx ResolvePropertyDeclarationCache() {
            var lang = Language;


            if(_propertyDecl == null && EnclosingType != null) {
                CodeTypeDeclarationEx typedecl = EnclosingType;

                var members = from m in typedecl.GetInheritedMembers()
                              let memberMethod = m as CodeMemberPropertyEx
                                where memberMethod != null && memberMethod.Name.Equals(this.PropertyName, lang.NameComparisation)
                                select memberMethod;

                if(members.Any())
                    _propertyDecl = members.First();
            }

            if(_propertyDecl == null) {
                var p = Project;
                if(p != null && p.DOMService.RootTypeUnSave != EnclosingType) {

                    var members = from member in p.DOMService.RootTypeUnSave.Members.Cast<CodeTypeMember>()
                                  let methodMember = member as CodeMemberPropertyEx
                                  where methodMember != null && methodMember.Name.Equals(this.PropertyName, lang.NameComparisation)
                                  select methodMember;

                    if(members.Any())
                        _propertyDecl = members.First();
                }
            }
            return _propertyDecl;
        }

        public override string ToString() {
            return "CodePropertyReferenceExpression: " + this.PropertyName + "\n" + CommentInfo;
        }



    }
}
