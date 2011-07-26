using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeTypeDeclarationEx : CodeTypeDeclaration, ICodeObjectEx, IEquatable<CodeTypeDeclarationEx>
    {
        #region Constructors

        public CodeTypeDeclarationEx() : base() { }
        public CodeTypeDeclarationEx(string name) : base(name) { }
        public CodeTypeDeclarationEx(string name, bool buildin) : base(name) { IsBuildInType = buildin; }

        #endregion

        #region Properties

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

        public bool IsBuildInType {
            get;
            set;
        }

        public Projecting.ProjectItemCode CodeDocumentItem {
            get;
            set;
        }

        #endregion


        #region Public Methods

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


        /// <summary>
        /// Checks if the asked typeref is a subclass of the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsSubclassOf(CodeTypeReferenceEx type) {
            var typerefs = this.BaseTypes.Cast<CodeTypeReferenceEx>().ToList();
            if(typerefs.Contains(type)) {
                return true;
            } else {
                foreach(CodeTypeReferenceEx bt in typerefs) {
                    var btdecl = bt.ResolveTypeDeclarationCache();
                    if(btdecl != null && btdecl.IsSubclassOf(type)) {
                        return true;
                    }
                }
            }
            return false;
        }


        public IEnumerable<CodeTypeReferenceEx> GetBaseTypeHirarchy() {
            List<CodeTypeReferenceEx> types = new List<CodeTypeReferenceEx>();
            foreach(CodeTypeReferenceEx bt in this.BaseTypes) {
                types.Add(bt);
                var btdecl = bt.ResolveTypeDeclarationCache();
                if(btdecl != null) {
                    types.AddRange(btdecl.GetBaseTypeHirarchy());
                }
            }
            return types;
        }

        #endregion

        public override string ToString() {
            return string.Format("TypeDeclaration: {0}", this.Name);
        }

        #region IEquatable

        public bool Equals(CodeTypeDeclarationEx other) {
            if(other == null)
                return false;
            return this.Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeTypeDeclarationEx);
        }

        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        #endregion
    }
}
