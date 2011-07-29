using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using SmartDevelop.Model.Projecting;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Archimedes.Patterns.Utils;
using System.Windows.Input;
using SmartDevelop.ViewModel.CodeCompleting;
using SmartDevelop.Model.DOM.Types;
using System.CodeDom;
using SmartDevelop.Model.CodeLanguages.Extensions;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.AHK.AHKv1.CodeCompletion
{
    public class CompletionDataProviderAHK : EditorDocumentExtension
    {
        #region Fields

        TextEditor _texteditor;
        ProjectItemCode _projectitem;
        CompletionWindow _completionWindow;

        #endregion

        #region Static Char Definitions

        static List<char> whitespacesNewLine = new List<char> { ' ', '\t', '\n', '\r' };
        static List<char> whitespaces = new List<char> { ' ', '\t' };
        static List<Token> t_whitespaces = new List<Token> { Token.WhiteSpace };
        static List<char> omitCodeCompletion = new List<char> { '(', ')', '[', ']', '{', '}', ';', ' ', '\t', };
        static List<char> triggerCodeCompletion = new List<char> { '.' };

        #endregion

        public CompletionDataProviderAHK(TextEditor texteditor, ProjectItemCode projectitem) {
            ThrowUtil.ThrowIfNull(texteditor);
            ThrowUtil.ThrowIfNull(projectitem);

            _texteditor = texteditor;
            _projectitem = projectitem;

            _texteditor.TextArea.TextEntered += OnTextEntered;
            _texteditor.TextArea.TextEntering += OnTextEntering;
        }

        CompletionWindow CreateNewCompletionWindow() {

            if(_completionWindow != null)
                _completionWindow.Close();

            _completionWindow = new CompletionWindow(_texteditor.TextArea);
            _completionWindow.Closed += delegate
            {
                _completionWindow = null;
            };
            return _completionWindow;
        }

        #region Event Handlers

        void OnTextEntering(object sender, TextCompositionEventArgs e) {
            if(e.Text.Length > 0 && _completionWindow != null) {
                if(!char.IsLetterOrDigit(e.Text[0])) {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // We still want to insert the character that was typed.
        }

        
        void OnTextEntered(object sender, TextCompositionEventArgs e) {
            char beforeChar;
            char currentChar;
            char carretChar;
            CodeSegment segment;

            try {
                currentChar = e.Text[0];

                if(_texteditor.CaretOffset > 1 && !triggerCodeCompletion.Contains(currentChar)) {
                    beforeChar = _texteditor.Document.GetCharAt(_texteditor.CaretOffset - 2);
                    if(!(beforeChar == ' ' || beforeChar == '\t'))
                        return;
                }

                carretChar = _texteditor.Document.GetCharAt(_texteditor.CaretOffset);

                var tokenLine = _projectitem.SegmentService.QueryCodeTokenLine(_texteditor.TextArea.Caret.Line);
                if(tokenLine.IsEmpty)
                    return;

                segment = _projectitem.SegmentService.QueryCodeSegmentAt(_texteditor.TextArea.Caret.Offset);
                if(segment.Token == Token.TraditionalString || segment.Token == Token.LiteralString)
                    return;

                //var previousUsabeToken = segment.PreviousOmit(t_whitespaces);
                //if(previousUsabeToken.Token == Token.KeyWord)
                //    return;

            } catch {
                return;
            }

            if(e.Text.Length == 1 && !omitCodeCompletion.Contains(currentChar)) {
                // this is just for first debugging purposes
                // as this code belongs to a completion service which handles and caches those completion items

                // Open code completion after the user has pressed dot:
                if(e.Text == ".") {

                    IList<ICompletionData> data = CreateNewCompletionWindow().CompletionList.CompletionData;

                    //ensure we have a updated tokenizer
                    _projectitem.EnsureTokenizerHasWorked();
                    _projectitem.Project.DOMService.EnsureIsUpdated();


                    // do type lookup & list avaiable members
                    var ctx = _projectitem.Project.DOMService.GetCodeContext(_projectitem, _texteditor.CaretOffset - 1, true);
                    if(ctx.Segment.CodeDOMObject is CodeThisReferenceExpression) {
                        foreach(var m in ctx.EnclosingType.GetInheritedMembers()) {
                            data.Add(CompletionItem.Build(m));
                        }
                    } else if(ctx.Segment.CodeDOMObject is CodeBaseReferenceExpression) {
                        foreach(CodeTypeReferenceEx basetype in ctx.EnclosingType.BaseTypes) {

                            var td = basetype.ResolveTypeDeclarationCache();
                            if(td != null) {
                                foreach(var m in td.GetInheritedMembers())
                                    data.Add(CompletionItem.Build(m));
                            }
                        }
                    }
                    _completionWindow.Show();

                } else if(_completionWindow == null && e.Text != "\n" &&
                    (_texteditor.Document.TextLength > _texteditor.CaretOffset) &&
                    (AsciiHelper.IsAsciiLiteralLetter(currentChar) && !AsciiHelper.IsAsciiNum(currentChar))) { // && !whitespaces.Contains(carretChar)
                    // show avaiable global Methods & build in Methods + commands

                    if(segment == null) {
                        return;
                    } else {
                        if(segment.Token == Token.MultiLineComment || segment.Token == Token.SingleLineComment)
                            return;
                    }

                    var ctx = _projectitem.Project.DOMService.GetCodeContext(_projectitem, _texteditor.CaretOffset);
                    IList<ICompletionData> data = CreateNewCompletionWindow().CompletionList.CompletionData;
                    foreach(var item in GetStaticCompletionItems()) {
                        data.Add(item);
                    }

                    bool any = false;
                    foreach(var m in ctx.GetVisibleMembers()) {
                        data.Add(CompletionItem.Build(m));
                        any = true;
                    }
                    if(any) {
                        _completionWindow.Show();
                    }
                }
            } else if(whitespaces.Contains(currentChar)) {

                if(segment != null) {
                    var s = segment.PreviousOmit(TokenHelper.WhiteSpaces);
                    if(s != null && s.Token == Token.KeyWord && (s.TokenString.Equals("new", StringComparison.CurrentCultureIgnoreCase) || s.TokenString.Equals("extends", StringComparison.CurrentCultureIgnoreCase))) {
                        var ctx = _projectitem.Project.DOMService.GetCodeContext(_projectitem, _texteditor.CaretOffset);

                        IList<ICompletionData> data = CreateNewCompletionWindow().CompletionList.CompletionData;
                        bool any = false;
                        foreach(var m in ctx.GetVisibleMembers()) {
                            if(m is CodeTypeDeclaration) {
                                data.Add(CompletionItem.Build(m));
                                any = true;
                            }
                        }
                        if(any) {
                            _completionWindow.Show();
                        }
                    }
                }
            }
        }
#endregion

        #region Helper Methods

        IEnumerable<CompletionItem> GetStaticCompletionItems() {
            var it = CompletionCache.Instance[_projectitem.CodeLanguage];
            if(it == null) {
                it = new CompletionCache.LanguageCompletionCache();
                foreach(var keyword in _projectitem.CodeLanguage.LanguageKeywords) {
                    it.AddStatic(new CompletionItemKeyword(keyword));
                }
                CompletionCache.Instance[_projectitem.CodeLanguage] = it;
            }
            return it.GetAllStaticCompletionItems();
        }

        #endregion


    }
}
