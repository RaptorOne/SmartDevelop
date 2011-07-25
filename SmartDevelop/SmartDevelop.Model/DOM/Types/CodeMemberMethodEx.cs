using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeMemberMethodEx : CodeMemberMethod, ICodeObjectEx
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
    }
}
