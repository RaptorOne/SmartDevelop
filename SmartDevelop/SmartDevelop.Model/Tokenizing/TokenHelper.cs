using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Tokenizing
{
    public static class TokenHelper
    {
        public static List<Token> WhiteSpaces = new List<Token> { Token.WhiteSpace };
        public static List<Token> WhiteSpacesNewLine = new List<Token> { Token.WhiteSpace, Token.NewLine };

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
    }
}
