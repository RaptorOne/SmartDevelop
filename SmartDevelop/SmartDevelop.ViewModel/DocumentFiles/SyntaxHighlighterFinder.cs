using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using ICSharpCode.AvalonEdit.Highlighting;
using SmartDevelop.Model.CodeLanguages;

namespace SmartDevelop.ViewModel.DocumentFiles
{
    public static class SyntaxHighlighterFinder
    {
        public static IHighlightingDefinition Find(CodeItemType type) {
            switch(type) {

                case CodeItemType.AHK:
                    return HighlightingManager.Instance.GetDefinition("AHK");

                case CodeItemType.IA:
                    return HighlightingManager.Instance.GetDefinition("IA");

                case CodeItemType.AHK2:
                    return HighlightingManager.Instance.GetDefinition("AHK2");

                default:
                    return null;
            }
        }
    }
}
