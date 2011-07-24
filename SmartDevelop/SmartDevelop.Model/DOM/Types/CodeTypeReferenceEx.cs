using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeTypeReferenceEx : CodeTypeReference
    {
        string _name;
        Type _type;

        public CodeTypeReferenceEx(string name)
            : base(name) {
                _name = name;
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

        public CodeTypeDeclarationEx FindTypeDeclaration(CodeTypeDeclarationEx context) {
            if(_typeDeclaration == null) {
                CodeTypeDeclarationEx typedecl = context;
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

    }
}
