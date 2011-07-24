using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeTypeDeclarationEx : CodeTypeDeclaration, ICodeObjectEx
    {
        public CodeTypeDeclarationEx() : base() { }
        public CodeTypeDeclarationEx(string name) : base(name) { }

        /// <summary>
        /// Gets the parent of this type if it is nested otherwise returns null
        /// </summary>
        public CodeTypeDeclarationEx Parent {
            get;
            set;
        }

        public bool IsHidden {
            get;
            set;
        }

        public IEnumerable<CodeTypeMember> GetInheritedMembers() {
            List<CodeTypeMember> members = new List<CodeTypeMember>();
            members.AddRange(this.Members.Cast<CodeTypeMember>());
            foreach(CodeTypeReferenceEx t in BaseTypes) {
                var type = t.FindTypeDeclaration(this.Parent);
                members.AddRange(type.GetInheritedMembers());
            }
            return members;
        }
    }
}
