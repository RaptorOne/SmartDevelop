﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.TokenizerBase.IA;
using SmartDevelop.Model.Tokening;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.AHK.AHKv1.Folding
{
    /// <summary>
    /// Allows producing foldings from a document based on braces.
    /// </summary>
    public class FoldingStrategyAHKv1 : AbstractFoldingStrategy
    {
        DocumentCodeSegmentService _tokenservice;
        readonly Token _openingBrace;
        readonly Token _closingBrace;


        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public FoldingStrategyAHKv1(DocumentCodeSegmentService tokenservice) {
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
            int len = document.TextLength;

            foreach(var segment in _tokenservice.GetSegments()) {
                if(segment.Token == Token.BlockOpen || segment.Token == Token.BlockClosed) {

                    if(segment.Token == _openingBrace) {
                        startOffsets.Push(segment.Range.Offset);
                    } else if(segment.Token == _closingBrace && startOffsets.Count > 0) {
                        int startOffset = startOffsets.Pop();
                        // don't fold if opening and closing brace are on the same line
                        if(startOffset < lastNewLineOffset) {

                            int endoffset = segment.Range.Offset + 1;
                            if(startOffset < len && endoffset < len) {
                                newFoldings.Add(new NewFolding(startOffset, endoffset));
                            }
                        }
                    }
                } else if(segment.Token == Token.NewLine) {
                    lastNewLineOffset = segment.Range.Offset;
                }
            }

            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }

    }
}
