using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Tokenizing
{
    public class CodeError
    {
        public string Description { get; set; }

        public bool HasError { get { return Description != null; } }
    }
}
