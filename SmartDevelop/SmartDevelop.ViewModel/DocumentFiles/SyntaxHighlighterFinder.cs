using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using ICSharpCode.AvalonEdit.Highlighting;

namespace SmartDevelop.ViewModel.DocumentFiles
{
    public static class SyntaxHighlighterFinder
    {
        public static IHighlightingDefinition Find(CodeItemType type) {
            switch(type) {

                case CodeItemType.AHK:
                    return HighlightingManager.Instance.GetDefinition("AHK");



                default:
                    return null;
            }
        }
    }
}
