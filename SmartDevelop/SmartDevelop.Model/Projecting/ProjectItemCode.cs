using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;

namespace SmartDevelop.Model.Projecting
{



    public class ProjectItemCode : ProjectItem
    {
        readonly TextDocument _codedocument;

        public ProjectItemCode() {
            _codedocument = new TextDocument();
        }

        public TextDocument Document {
            get { return _codedocument; }
        }
    }
}
