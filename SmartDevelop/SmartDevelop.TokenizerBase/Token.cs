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
        WhiteSpace,
        ParameterDelemiter,
        NewLine,
        MemberInvoke,
        StringConcat,
        OperatorFlow,
        Number,
        HexNumber,
        KeyWord,

        #region Brackets

        /// <summary>
        /// (
        /// </summary>
        LiteralBracketOpen,
        
        /// <summary>
        /// )
        /// </summary>
        LiteralBracketClosed,

        /// <summary>
        /// [
        /// </summary>
        IndexerBracketOpen,

        /// <summary>
        /// ]
        /// </summary>
        IndexerBracketClosed,

        /// <summary>
        /// {
        /// </summary>
        BlockOpen,

        /// <summary>
        /// }
        /// </summary>
        BlockClosed,


        #endregion

        //Legacy
        TraditionalString,
        TraditionalCommandInvoke,
        TraditionalAssign,
        Deref,
    }
}
