using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.TokenizerBase
{
    public enum Token
    {
        Unknown,
        SingleLineComment,
        MultiLineComment,
        LiteralString,
        Bracket,
        WhiteSpace,
        ParameterDelemiter,
        NewLine,
        MemberInvoke,
        StringConcat
    }
}
