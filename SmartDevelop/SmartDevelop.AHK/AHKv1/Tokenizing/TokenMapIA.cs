using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;

namespace SmartDevelop.AHK.AHKv1.Tokenizing
{
    public class TokenMapIA
    {
        Dictionary<string, Token> _tokenMap = new Dictionary<string, Token>();

        public TokenMapIA() {
            FillIATokens();
        }

        void FillIATokens() {
            _tokenMap = new Dictionary<string, Token>() {

                { "+", Token.Operator_Add },
                { "-", Token.Operator_Subtract },
                { "*", Token.Operator_Multiply },
                { "/", Token.Operator_Divide },
                { "//", Token.Operator_AssignModulus },
                { "|", Token.Operator_BitwiseOr },
                { "^", Token.Operator_BitwiseXOr },
                { "&", Token.Operator_BitwiseAnd },
                { "**", Token.Operator_Power },

                { "<<", Token.Operator_ShiftLeft },
                { ">>", Token.Operator_ShiftRight },


                { "&&", Token.Operator_BooleanAnd},
                { "||", Token.Operator_BooleanOr},

                { ">", Token.Operator_GreaterThan },
                { ">=", Token.Operator_GreaterThanOrEqual },

                { "<", Token.Operator_LessThan },
                { "<=", Token.Operator_LessThanOrEqual },

                /*{ "=", Token.Operator_ValueEquality },*/
                { "==", Token.Operator_IdentityEquality },
                { "!=", Token.Operator_IdentityInequality },


                { "?", Token.Operator_TernaryIf },
                { ":", Token.Operator_TernaryElse },

                { ":=", Token.Operator_Assign },
                { "+=", Token.Operator_AssignAdd },
                { "-=", Token.Operator_AssignSubtract },
                { "*=", Token.Operator_AssignMultiply },
                { "/=", Token.Operator_AssignDivide },
                { "//=", Token.Operator_AssignModulus },
                { ".=", Token.Operator_AssignAppend },
                { "|=", Token.Operator_AssignBitOr },
                { "&=", Token.Operator_AssignBitAnd },
                { "^=", Token.Operator_AssignBitXOr },
                { ">>=", Token.Operator_AssignShiftRight },
                { "<<=", Token.Operator_AssignShiftLeft },

            };
        }

        public Token FindOperatorToken(string ex) {
            if(_tokenMap.ContainsKey(ex))
                return _tokenMap[ex];
            else
                return Token.Unknown;
        }
    }
}
