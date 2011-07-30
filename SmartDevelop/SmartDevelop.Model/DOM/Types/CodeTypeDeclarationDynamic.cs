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

        SmartCodeProject _project;
        Projecting.ProjectItemCodeDocument _codeDocumentItem;

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

        CodeLanguage Language {
            get {
                return Project!=null ? Project.Language : null;
            }
        }

        #endregion

        #region Public Methods

        public override IEnumerable<CodeTypeMember> GetInheritedMembers() {
            List<CodeTypeMember> members = new List<CodeTypeMember>();
            return members;
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
            List<CodeTypeReferenceEx> types = new List<CodeTypeReferenceEx>();
            return types;
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
