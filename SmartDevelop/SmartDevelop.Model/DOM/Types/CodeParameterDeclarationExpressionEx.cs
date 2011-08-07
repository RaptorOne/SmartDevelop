using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeParameterDeclarationExpressionEx : CodeParameterDeclarationExpression, ICloneable
    {

        public CodeParameterDeclarationExpressionEx(string name) 
            : base(typeof(object), name) {
        }

        public CodeParameterDeclarationExpressionEx(CodeTypeReferenceEx typeRef, string name)
            : base(typeRef, name) {
        }

        public CodeParameterDeclarationExpressionEx(Type type, string name)
            : base(type, name) {

        }


        public string ParameterDocumentationComment {
            get;
            set;
        }

        public bool IsOptional {
            get;
            set;
        }

        public object Clone() {
            var clone = new CodeParameterDeclarationExpressionEx(this.Name)
            {
                Direction = this.Direction,
                Type = this.Type
            };

            return clone;
        }
    }
}
