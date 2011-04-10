using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;

namespace SmartDevelop.Model.Projecting
{
    public enum CodeItemType
    {
        None = 0,
        IA = 1,
        AHK = 2,
        AHK_L = 4,
        AHK2 = 8
    }

    public class ProjectItemCode : ProjectItem
    {
        readonly TextDocument _codedocument;
        CodeItemType _type = CodeItemType.None;

        public ProjectItemCode(CodeItemType type) {
            _codedocument = new TextDocument();
            _type = type;
        }

        public CodeItemType Type {
            get { return _type; }
        }

        public TextDocument Document {
            get { return _codedocument; }
        }
    }
}
