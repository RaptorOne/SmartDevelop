using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.Model.DOM.Types
{
    public interface ICodeObjectEx
    {
        bool IsHidden { get; }
        bool IsBuildInType { get; }

        ProjectItemCode CodeDocumentItem { get; }
    }
}
