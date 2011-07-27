using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns.Utils;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeMemberMethodEx : CodeMemberMethod, ICodeMemberEx, IEquatable<CodeMemberMethodEx>
    {
        

        public CodeMemberMethodEx(Projecting.ProjectItemCode codeDocumentItem) 
            : base() {
                ThrowUtil.ThrowIfNull(codeDocumentItem);
                _codeDocumentItem = codeDocumentItem;
        }

        public CodeMemberMethodEx(bool buildIn) 
            : base() { IsBuildInType = buildIn; }

        public CodeTypeDeclarationEx DefiningType {
            get;
            set;
        }

        public bool IsHidden {
            get;
            set;
        }

        public bool IsBuildInType {
            get;
            protected set;
        }

        SmartCodeProject _project;
        Projecting.ProjectItemCode _codeDocumentItem;

        public ProjectItemCode CodeDocumentItem {
            get { return _codeDocumentItem ; }
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

        public CodeLanguages.CodeLanguage Language { get { return Project.Language; } }

        #region IEquatable

        public bool Equals(CodeMemberMethodEx other) {
            if(other == null)
                return false;
            return other.Name.Equals(this.Name, Language.NameComparisation);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeMemberMethodEx);
        }

        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        #endregion

        public bool Matches(CodeTypeReferenceEx codeTypeRef) {
            if(codeTypeRef == null) return false;
            return this.Name.Equals(codeTypeRef.TypeName, Language.NameComparisation);
        }



    }
}
