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
        public CodeTypeDeclarationEx(string name, bool buildin) : base(name) { IsBuildInType = buildin; }

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

                if(t.ResolvedTypeDeclaration == null)
                    t.ResolveTypeDeclarationCache();

                if(t.ResolvedTypeDeclaration != null) {
                    members.AddRange(t.ResolvedTypeDeclaration.GetInheritedMembers());
                }
            }

            return members;

        }


        public bool IsBuildInType {
            get;
            set;
        }

        public override string ToString() {
            return string.Format("TypeDeclaration: {0}", this.Name);
        }
    }
}
