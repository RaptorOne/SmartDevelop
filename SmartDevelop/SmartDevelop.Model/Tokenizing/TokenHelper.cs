using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Tokenizing
{
    public static class TokenHelper
    {
        public static readonly List<Token> WhiteSpaces = new List<Token> { Token.WhiteSpace };
        public static readonly List<Token> WhiteSpacesNewLine = new List<Token> { Token.WhiteSpace, Token.NewLine };


        #region Bracket Helper Methods

        static Dictionary<Token, Token> openCloseMap = new Dictionary<Token, Token>();

        static TokenHelper() {

            BRAKETS.Add('(', Token.LiteralBracketOpen);
            BRAKETS.Add(')', Token.LiteralBracketClosed);
            BRAKETS.Add('{', Token.BlockOpen);
            BRAKETS.Add('}', Token.BlockClosed);
            BRAKETS.Add('[', Token.IndexerBracketOpen);
            BRAKETS.Add(']', Token.IndexerBracketClosed);

            openCloseMap.Add(Token.LiteralBracketOpen, Token.LiteralBracketClosed);
            openCloseMap.Add(Token.IndexerBracketOpen, Token.IndexerBracketClosed);
            openCloseMap.Add(Token.BlockOpen, Token.BlockClosed);
        }

        public static readonly Dictionary<char, Token> BRAKETS = new Dictionary<char, Token>();

        public static Token GetClosingToken(Token openingToken) {
            return openCloseMap[openingToken];
        }
        public static Token GetOpenToken(Token closingToken) {
            foreach(var t in openCloseMap){
                if(t.Value == closingToken) {
                    return t.Key;
                }
            }
            return Token.Unknown;
        }


        public static bool IsOpenBracketToken(Token token) {
            return (openCloseMap.ContainsKey(token));
        }
        public static bool IsClosedBracketToken(Token token) {
            return (openCloseMap.ContainsValue(token));
        }
        public static bool IsBracketToken(Token token) {
            return (openCloseMap.ContainsKey(token) || openCloseMap.ContainsValue(token));
        }
        #endregion

        #region Operators

        public static readonly List<Token> OPERATORS = new List<Token>() {
            Token.Operator_Add,
            Token.Operator_Subtract,
            Token.Operator_Multiply,
            Token.Operator_Divide,
            Token.Operator_AssignModulus, 
            Token.Operator_BitwiseOr, 
            Token.Operator_BitwiseXOr, 
            Token.Operator_BitwiseAnd,
            Token.Operator_Power,

            Token.Operator_ShiftLeft,
            Token.Operator_ShiftRight,


            Token.Operator_BooleanAnd,
            Token.Operator_BooleanOr,
            Token.Operator_BooleanNot,

            Token.Operator_GreaterThan,
            Token.Operator_GreaterThanOrEqual,

            Token.Operator_LessThan,
            Token.Operator_LessThanOrEqual,

                
            Token.Operator_IdentityEquality,
            Token.Operator_IdentityInequality,


            Token.Operator_TernaryIf,
            Token.Operator_TernaryElse,

            Token.Operator_Assign,
            Token.Operator_AssignAdd,
            Token.Operator_AssignSubtract,
            Token.Operator_AssignMultiply,
            Token.Operator_AssignDivide,
            Token.Operator_AssignModulus,
            Token.Operator_AssignAppend,
            Token.Operator_AssignBitOr,
            Token.Operator_AssignBitAnd,
            Token.Operator_AssignBitXOr,
            Token.Operator_AssignShiftRight,
            Token.Operator_AssignShiftLeft,
        };

        #endregion
    }
}
