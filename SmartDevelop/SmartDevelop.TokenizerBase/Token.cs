using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.TokenizerBase
{
    public enum Token : int
    {
        Unknown = 0,
        
        /// <summary>
        /// Identifier which can be a class, field, property, method, local variable, type identifier etc...
        /// The tokenizer doesn't have enought context info to judge about.
        /// </summary>
        Identifier = 1,
        SingleLineComment = 2,
        MultiLineComment = 3,
        LiteralString = 4,
        WhiteSpace = 5,
        ParameterDelemiter = 6,
        NewLine = 7,
        /// <summary>
        /// Member Invoke, mostly a Dot like in:
        /// class.Action()
        /// class.Property
        /// </summary>
        MemberInvoke = 8,
        StringConcat = 9,
        OperatorFlow = 10,
        Number = 11,
        HexNumber = 12,
        KeyWord = 13,

        #region Operators

        /// <summary>
        /// +
        /// </summary>
        Operator_Add = 20,

        /// <summary>
        /// -
        /// </summary>
        Operator_Subtract = 21,

        /// <summary>
        /// &
        /// </summary>
        Operator_BitwiseAnd = 22,
        
        /// <summary>
        /// |
        /// </summary>
        Operator_BitwiseOr = 23,
        
        /// <summary>
        /// &&
        /// </summary>
        Operator_BooleanAnd = 24,
        
        /// <summary>
        /// ||
        /// </summary>
        Operator_BooleanOr = 25,

        /// <summary>
        /// /
        /// </summary>
        Operator_Divide = 26,

        /// <summary>
        /// //
        /// </summary>
        Operator_Modulus = 27,

        /// <summary>
        /// *
        /// </summary>
        Operator_Multiply = 28,

        /// <summary>
        /// x**a 
        /// x ^ (a)
        /// </summary>
        Operator_Power = 29,

        /// <summary>
        /// <<
        /// </summary>
        Operator_ShiftLeft,

        /// <summary>
        /// >>
        /// </summary>
        Operator_ShiftRight,

        Operator_GreaterThan,
        Operator_GreaterThanOrEqual,
        Operator_IdentityEquality,
        Operator_IdentityInequality,
        Operator_LessThan,
        Operator_LessThanOrEqual,


        
        /* Operator_ValueEquality */

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
        LiteralBracketOpen = 50,

        /// <summary>
        /// )
        /// </summary>
        LiteralBracketClosed = 51,

        /// <summary>
        /// [
        /// </summary>
        IndexerBracketOpen = 52,

        /// <summary>
        /// ]
        /// </summary>
        IndexerBracketClosed = 53,

        /// <summary>
        /// {
        /// </summary>
        BlockOpen = 54,

        /// <summary>
        /// }
        /// </summary>
        BlockClosed = 55,


        #endregion

        // Legacy
        TraditionalString,
        TraditionalCommandInvoke,
        TraditionalAssign,
        Deref,
    }

}
