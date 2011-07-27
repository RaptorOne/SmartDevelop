using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns.Utils;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeMemberPropertyEx : CodeMemberProperty, ICodeMemberEx
    {
        public CodeMemberPropertyEx(ProjectItemCode codeItem) : base() {
            ThrowUtil.ThrowIfNull(codeItem);
            CodeDocumentItem = codeItem;
        }

        public CodeMemberPropertyEx(bool buildIn) 
            : base() { IsBuildInType = buildIn; }

        public bool IsHidden {
            get;
            set;
        }

        public bool IsBuildInType {
            get;
            set;
        }

        public Projecting.ProjectItemCode CodeDocumentItem {
            get;
            set;
        }

        SmartCodeProject _project;
        public Projecting.SmartCodeProject Project {
            get { return (CodeDocumentItem != null) ?CodeDocumentItem.Project : _project ; }
            set { _project = value; }
        }
    }
}
