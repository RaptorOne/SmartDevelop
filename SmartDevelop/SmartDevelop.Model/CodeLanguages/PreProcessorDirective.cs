using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.CodeLanguages
{
    public class PreProcessorDirective : CodeObject
    {
        public string Name { get; set; }


        public string ResolvedFilePath { get; set; }

        public override string ToString() {
            return ResolvedFilePath ?? Name;
        }
    }
}
