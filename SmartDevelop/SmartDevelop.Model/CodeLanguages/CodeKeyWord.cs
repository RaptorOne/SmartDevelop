using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.CodeLanguages
{
    [Serializable]
    public class CodeKeyWord : CodeObject
    {
        public CodeKeyWord() { }
        public CodeKeyWord(string name) { this.Name = name; }
        public CodeKeyWord(string name, string description) 
            : this(name) { this.Description = description; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
