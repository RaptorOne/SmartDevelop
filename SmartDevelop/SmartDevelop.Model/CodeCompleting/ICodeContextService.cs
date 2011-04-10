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
        CodeTypeDeclaration GetEnclosingTypeAt(ProjectItemCode projectitem, TextLocation location);

        IEnumerable<CodeTypeMember> GetAvaiableMembersAt(ProjectItemCode projectitem, TextLocation location);

        CodeObject GetCodeObjectAt(ProjectItemCode projectitem, TextLocation location);
    }
}
