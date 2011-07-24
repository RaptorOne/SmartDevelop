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
    }
}
