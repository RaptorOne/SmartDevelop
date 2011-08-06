using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeParameterDeclarationExpressionEx : CodeParameterDeclarationExpression, ICloneable
    {

        public object Clone() {
            var clone = new CodeParameterDeclarationExpressionEx()
            {
                Name = this.Name,
                Direction = this.Direction,
                Type = this.Type
            };

            return clone;
        }
    }
}
