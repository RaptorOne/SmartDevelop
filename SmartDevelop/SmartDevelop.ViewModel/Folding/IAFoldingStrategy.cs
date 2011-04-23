using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.TokenizerBase.IA;
using SmartDevelop.Model.Tokening;
using SmartDevelop.TokenizerBase;

namespace SmartDevelop.ViewModel.Folding
{
    /// <summary>
    /// Allows producing foldings from a document based on braces.
    /// </summary>
    public class IAFoldingStrategy : AbstractFoldingStrategy
    {
        CodeTokenService _tokenservice;
        readonly Token _openingBrace;
        readonly Token _closingBrace;


        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public IAFoldingStrategy(CodeTokenService tokenservice) {
            _openingBrace = Token.BlockOpen;
            _closingBrace = Token.BlockClosed;
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

            foreach(var segment in _tokenservice.GetSegments()) {
                if(segment.Type == TokenizerBase.Token.BlockOpen || segment.Type == TokenizerBase.Token.BlockClosed) {

                    if(segment.Type == _openingBrace) {
                        startOffsets.Push(segment.Range.Offset);
                    } else if(segment.Type == _closingBrace && startOffsets.Count > 0) {
                        int startOffset = startOffsets.Pop();
                        // don't fold if opening and closing brace are on the same line
                        if(startOffset < lastNewLineOffset) {
                            newFoldings.Add(new NewFolding(startOffset, segment.Range.Offset + 1));
                        }
                    }
                } else if(segment.Type == TokenizerBase.Token.NewLine) {
                    lastNewLineOffset = segment.Range.Offset;
                }
            }

            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }





    }
}
