using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.CodeContexts;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeTypeReferenceEx : CodeTypeReference, IEquatable<CodeTypeReferenceEx>
    {
        #region

        string _name;
        Type _type;

        #endregion

        public CodeTypeReferenceEx(string name, CodeTypeDeclarationEx enclosingType)
            : base(name) {
                _name = name;
                _enclosingType = enclosingType;
        }

        public CodeTypeReferenceEx(Type type)
            : base(type) { 
            _type = type;
            _name = type.Name;
        }

        public string TypeName {
            get { return _name; }
        }

        CodeTypeDeclarationEx _typeDeclaration;

        public CodeTypeDeclarationEx ResolvedTypeDeclaration {
            get { return _typeDeclaration; }
            set { _typeDeclaration = value; }
        }

        CodeTypeDeclarationEx _enclosingType;
        public CodeTypeDeclarationEx EnclosingType {
            get { return _enclosingType; }
            //set { _enclosingType = value; }
        }

        public CodeTypeDeclarationEx ResolveTypeDeclarationCache() {

            if(_typeDeclaration == null && EnclosingType != null) {
                CodeTypeDeclarationEx typedecl = EnclosingType;
                while(typedecl != null) {
                    var type = typedecl.Members.Cast<CodeTypeMember>().ToList().Find(
                    (x) => {
                        var typedeclaration = x as CodeTypeDeclarationEx;
                        return typedeclaration != null && typedeclaration.Name.Equals(this.TypeName);
                    }) as CodeTypeDeclarationEx;
                    if(type != null) {
                        _typeDeclaration = type;
                        break;
                    } else
                        typedecl = typedecl.Parent;
                }
            }
            return _typeDeclaration;
        }

        public override string ToString() {
            return string.Format("TypeReference: {0} --> {1}", this.TypeName, this.ResolvedTypeDeclaration);
        }


        public bool Equals(CodeTypeReferenceEx other) {
            if(other == null)
                return false;
            return other.TypeName.Equals(this.TypeName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeTypeReferenceEx);
        }

        public override int GetHashCode() {
            return this.TypeName.GetHashCode();
        }
    }
}
