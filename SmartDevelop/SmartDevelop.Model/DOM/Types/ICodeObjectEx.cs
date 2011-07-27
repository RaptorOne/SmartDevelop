using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.Model.DOM.Types
{
    public interface ICodeMemberEx : ICodeContext
    {
        bool IsHidden { get; }
        bool IsBuildInType { get; }
    }

    public interface ICodeContext
    {
        ProjectItemCode CodeDocumentItem { get; set; }
        SmartCodeProject Project { get; set; }
    }
}
