using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.Model.CodeLanguages
{


    public class IncludeDirective : PreProcessorDirective
    {
        public string ResolvedFilePath { get; set; }

        public ProjectItemCodeDocument ResolvedCodeDocument { get; set; }

        public override string ToString() {
            return ResolvedFilePath ?? Name;
        }
    }

    public class PreProcessorDirective : CodeKeyWord
    {

    }
}
