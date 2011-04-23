using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.TokenizerBase
{
    public static class TokenHelper
    {
        static Dictionary<Token, Token> openCloseMap = new Dictionary<Token, Token>();

        static TokenHelper() {
            openCloseMap.Add(Token.LiteralBracketOpen, Token.LiteralBracketClosed);
            openCloseMap.Add(Token.IndexerBracketOpen, Token.IndexerBracketClosed);
            openCloseMap.Add(Token.BlockOpen, Token.BlockClosed);
        }

        public static Token GetClosingToken(Token openingToken) {
            return openCloseMap[openingToken];
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

    }
}
