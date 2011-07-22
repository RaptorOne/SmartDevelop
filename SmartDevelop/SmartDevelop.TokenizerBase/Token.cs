using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.TokenizerBase
{
    public enum Token
    {
        Unknown,
        
        /// <summary>
        /// Identifier which can be a class, field, property, method, local variable, type identifier etc...
        /// The tokenizer doesn't have enought context info to judge about.
        /// </summary>
        Identifier,
        SingleLineComment,
        MultiLineComment,
        LiteralString,
        WhiteSpace,
        ParameterDelemiter,
        NewLine,
        /// <summary>
        /// Member Invoke, mostly a Dot like in:
        /// class.Action()
        /// class.Property
        /// </summary>
        MemberInvoke,
        StringConcat,
        OperatorFlow,
        Number,
        HexNumber,
        KeyWord,

        #region Operators

        Operator_Add,
        Operator_BitwiseAnd,
        Operator_BitwiseOr,
        Operator_BooleanAnd,
        Operator_BooleanOr,
        Operator_Divide,
        Operator_GreaterThan,
        Operator_GreaterThanOrEqual,
        Operator_IdentityEquality,
        Operator_IdentityInequality,
        Operator_LessThan,
        Operator_LessThanOrEqual,
        Operator_Modulus,
        Operator_Multiply,
        Operator_Subtract,
        
        /*Operator_ValueEquality,*/

        Operator_Assign,
        Operator_AssignAdd,
        Operator_AssignSubtract,
        Operator_AssignMultiply,
        Operator_AssignDivide,
        Operator_AssignModulus,
        Operator_AssignAppend,
        Operator_AssignBitOr,
        Operator_AssignBitAnd,
        Operator_AssignBitXOr,
        Operator_AssignShiftRight,
        Operator_AssignShiftLeft,

        Operator_TernaryIf,
        Operator_TernaryElse,

        #endregion


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

        // Legacy
        TraditionalString,
        TraditionalCommandInvoke,
        TraditionalAssign,
        Deref,
    }

}
