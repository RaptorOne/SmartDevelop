using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.TokenizerBase
{
    public class Class1
    {

        void Test(CodeTypeDeclaration type) {
            CodeTypeMember member = type.Members[0];
            var method = member as CodeMemberMethod;
        }

    }
}
