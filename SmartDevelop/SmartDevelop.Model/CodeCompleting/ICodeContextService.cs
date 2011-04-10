using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.Projecting;
using ICSharpCode.AvalonEdit.Document;

namespace SmartDevelop.Model.CodeCompleting
{
    public interface ICodeContextService
    {
        CodeTypeDeclaration GetEnclosingType(ProjectItemCode projectitem, TextLocation location);

        IEnumerable<CodeTypeMember> GetAvaiableMembers(ProjectItemCode projectitem, TextLocation location);
    }
}
