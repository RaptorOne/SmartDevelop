using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.DOM.Types
{
    /// <summary>
    /// Represents the root type of an language.
    /// This will equal any instance of CodeTypeDeclarationRoot
    /// </summary>
    public class CodeTypeDeclarationRoot : CodeTypeDeclarationEx
    {
        public CodeTypeDeclarationRoot() 
                : base() { }

        #region IEquatable

        public virtual bool Equals(CodeTypeDeclarationRoot other) {
            if(other == null)
                return false;
            return true;
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeTypeDeclarationRoot);
        }

        static int _hash = typeof(CodeTypeDeclarationRoot).GetHashCode();

        public override int GetHashCode() {
            return _hash;
        }

        #endregion
        
    }
}
