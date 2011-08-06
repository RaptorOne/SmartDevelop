using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.CodeLanguages;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.Model.DOM.Types
{
    /// <summary>
    /// Represents a dynamic Type
    /// </summary>
    public class CodeTypeDeclarationDynamic : CodeTypeDeclarationEx, ICodeMemberEx, IEquatable<CodeTypeDeclarationEx>
    {
        #region Constructors

        public CodeTypeDeclarationDynamic() : base() {
            this.Name = "Dynamic";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Yields an empty list
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<CodeTypeMember> GetInheritedMembers() {
            return new List<CodeTypeMember>();;
        }


        /// <summary>
        /// Checks if the asked typeref is a subclass of the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsSubclassOf(CodeTypeReferenceEx type) {
            return true;
        }


        public IEnumerable<CodeTypeReferenceEx> GetBaseTypeHirarchy() {
            return new List<CodeTypeReferenceEx>();
        }

        #endregion

        public override string ToString() {
            return "Dynamic Variable Declaration";
        }

        #region IEquatable

        public bool Equals(CodeTypeDeclarationDynamic other) {
            if(other == null)
                return false;
            return this.Name.Equals(other.Name, Language.NameComparisation);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeTypeDeclarationDynamic);
        }

        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        #endregion

        public readonly static CodeTypeDeclarationDynamic Default = new CodeTypeDeclarationDynamic();

    }
}
