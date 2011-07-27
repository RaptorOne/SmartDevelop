using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.TokenizerBase
{
    public class CodeError
    {
        public string Description { get; set; }

        public bool HasError { get { return Description != null; } }
    }
}
