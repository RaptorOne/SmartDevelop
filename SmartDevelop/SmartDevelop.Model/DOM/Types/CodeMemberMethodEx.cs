using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeMemberMethodEx : CodeMemberMethod, ICodeObjectEx, IEquatable<CodeMemberMethodEx>
    {
        public CodeMemberMethodEx() : base() { }
        public CodeMemberMethodEx(bool buildIn) : base() { IsBuildInType = buildIn; }

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


        public Projecting.ProjectItemCode CodeDocumentItem {
            get;
            set;
        }

        #region IEquatable

        public bool Equals(CodeMemberMethodEx other) {
            if(other == null)
                return false;
            return other.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeMemberMethodEx);
        }

        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        #endregion


    }
}
