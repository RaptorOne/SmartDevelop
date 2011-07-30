using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM.Types;
using SmartDevelop.Model.Projecting;
using System.CodeDom;

namespace SmartDevelop.AHK.AHKv1.DOM.Types
{
    public class CodeMethodReferenceExpressionExAHK : CodeMethodReferenceExpressionEx
    {
        public CodeMethodReferenceExpressionExAHK(ProjectItemCodeDocument codeDocumentItem, System.CodeDom.CodeExpression target, string methodName, CodeTypeDeclarationEx enclosingType)
            : base(codeDocumentItem, target, methodName, enclosingType) {
        }
        public CodeMethodReferenceExpressionExAHK(ProjectItemCodeDocument codeDocumentItem, CodeMemberMethodEx resolvedCommand)
            : base(codeDocumentItem, null, resolvedCommand.Name, null) {
                _methodDeclaration = resolvedCommand;
        }


        public override CodeMemberMethodEx ResolveMethodDeclarationCache() {
            var lang = Language;


            if(_methodDeclaration == null && EnclosingType != null) {
                CodeTypeDeclarationEx typedecl = EnclosingType;

                var members = from m in typedecl.GetInheritedMembers()
                              let memberMethod = m as CodeMemberMethodExAHK
                              where memberMethod != null && memberMethod.IsDefaultMethodInvoke && memberMethod.Name.Equals(this.MethodName, lang.NameComparisation)
                              select memberMethod;

                if(members.Any())
                    _methodDeclaration = members.First();
            }

            if(_methodDeclaration == null) {
                var p = Project;
                if(p != null && p.DOMService.RootTypeUnSave != EnclosingType) {

                    var members = from member in p.DOMService.RootTypeUnSave.Members.Cast<CodeTypeMember>()
                                  let methodMember = member as CodeMemberMethodExAHK
                                  where methodMember != null && methodMember.IsDefaultMethodInvoke && methodMember.Name.Equals(this.MethodName, lang.NameComparisation)
                                  select methodMember;

                    if(members.Any())
                        _methodDeclaration = members.First();
                }
            }
            return _methodDeclaration;
        }
    }
}
