using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.AHK.AHKv1.DOM
{
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) 
            : base(message) {

        }
    }
}
