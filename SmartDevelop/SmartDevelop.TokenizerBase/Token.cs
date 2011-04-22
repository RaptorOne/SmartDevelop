using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.TokenizerBase
{
    public enum Token
    {
        Unknown,
        Assign,
        SingleLineComment,
        MultiLineComment,
        LiteralString,
        Bracket,
        WhiteSpace,
        ParameterDelemiter,
        NewLine,
        MemberInvoke,
        StringConcat,
        OperatorFlow,
        Number,
        HexNumber,
        KeyWord,

        //Legacy
        TraditionalString,
        TraditionalCommandInvoke,
        TraditionalAssign,
        Deref,
    }
}
