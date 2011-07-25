using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.DOM.Types
{
    public interface ICodeObjectEx
    {
        bool IsHidden { get; }
        bool IsBuildInType { get; }
    }
}
