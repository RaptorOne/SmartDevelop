using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase.IA;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.TokenizerBase;

namespace SmartDevelop.Tests.TestCases
{
    class TokenizerTest
    {
        public static IEnumerable<CodeSegment> Test(string str) {

            var doc = new TextDocument();
            doc.Text = str;
            var tokenizer = new SimpleTokinizerIA(doc);

            tokenizer.TokenizeSync();
            return tokenizer.GetSegmentsSnapshot();
        }
    }
}
