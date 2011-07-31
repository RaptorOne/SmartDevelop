using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.DOM.Types;

namespace SmartDevelop.AHK.AHKv1.DOM
{
    public class CompilerArgument
    {
        public CompilerArgument(ProjectItemCodeDocument codeitem, CodeTypeDeclarationEx initialparent) {
            Codeitem = codeitem;
            Initialparent = initialparent;
        }

        public ProjectItemCodeDocument Codeitem { get; protected set; }
        public CodeTypeDeclarationEx Initialparent { get; protected set; }
    }
}
