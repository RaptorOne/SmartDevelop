using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.TokenizerBase.IA;

namespace SmartDevelop.ViewModel.Folding
{
    /// <summary>
    /// Allows producing foldings from a document based on braces.
    /// </summary>
    public class IAFoldingStrategy : AbstractFoldingStrategy
    {
        CodeTokenRepesentation _tokenservice;
        readonly char _openingBrace;
        readonly char _closingBrace;


        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public IAFoldingStrategy(CodeTokenRepesentation tokenservice) {
            _openingBrace = '{';
            _closingBrace = '}';
            _tokenservice = tokenservice;
        }


        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset) {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document) {
            List<NewFolding> newFoldings = new List<NewFolding>();
            Stack<int> startOffsets = new Stack<int>();
            int lastNewLineOffset = 0;

            foreach(var token in _tokenservice.GetSegments()) {
                if(token.Type == TokenizerBase.Token.Bracket) {

                    if(token.TokenString[0] == _openingBrace) {
                        startOffsets.Push(token.Range.Offset);
                    } else if(token.TokenString[0] == _closingBrace && startOffsets.Count > 0) {
                        int startOffset = startOffsets.Pop();
                        // don't fold if opening and closing brace are on the same line
                        if(startOffset < lastNewLineOffset) {
                            newFoldings.Add(new NewFolding(startOffset, token.Range.Offset + 1));
                        }
                    }
                } else if(token.Type == TokenizerBase.Token.NewLine) {
                    lastNewLineOffset = token.Range.Offset;
                }
            }

            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }





    }
}
