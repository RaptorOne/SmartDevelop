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
using SmartDevelop.Model.CodeContexts;
using System.Windows.Controls;
using SmartDevelop.ViewModel.InvokeCompletion;
using System.Threading.Tasks;
using System.Windows;

namespace SmartDevelop.AHK.AHKv1.CodeCompletion
{
    public class CompletionDataProviderAHK : EditorDocumentExtension
    {
        const int MAX_TIMEOUT = 2000;

        #region Fields

        TextEditor _texteditor;
        ProjectItemCodeDocument _projectitem;
        CompletionWindow _completionWindow;

        #endregion

        #region Static Char Definitions

        static List<char> whitespacesNewLine = new List<char> { ' ', '\t', '\n', '\r' };
        static List<char> whitespaces = new List<char> { ' ', '\t' };
        static List<Token> t_whitespaces = new List<Token> { Token.WhiteSpace };
        static List<char> omitCodeCompletion = new List<char> { '(', ')', '[', ']', '{', '}', ';', ' ', '\t', };
        static List<char> triggerCodeCompletion = new List<char> { '.' };

        static List<char> endCodeCompletion = new List<char> { '(', ')', '[', ']', '{', '}', ';' };

        static List<char> allowedspecailChars = new List<char> { '#', '_' };
        

        #endregion

        public CompletionDataProviderAHK(TextEditor texteditor, ProjectItemCodeDocument projectitem) {
            ThrowUtil.ThrowIfNull(texteditor);
            ThrowUtil.ThrowIfNull(projectitem);

            _texteditor = texteditor;
            _projectitem = projectitem;

            _texteditor.TextArea.TextEntered += OnTextEntered;
        }

        CompletionWindow CreateNewCompletionWindow() {

            if(_completionWindow != null)
                _completionWindow.Close();

            _completionWindow = new CompletionWindow(_texteditor.TextArea);
            //_completionWindow.CompletionList.IsFiltering = false;
            _completionWindow.StartOffset -= 1;

            _completionWindow.Closed += delegate
            {
                _completionWindow = null;
            };
            return _completionWindow;
        }

        #region Event Handlers

        ToolTip _toolTip = new ToolTip();
        
        void OnTextEntered(object sender, TextCompositionEventArgs e) {
            char beforeChar;
            char currentChar;
            //char carretChar;
            CodeSegment segment;

            try {
                currentChar = e.Text[0];

                if(_completionWindow != null && endCodeCompletion.Contains(currentChar))
                    _completionWindow.Close();

                if(_completionWindow != null)
                    return;

                if(_texteditor.CaretOffset > 1 && !triggerCodeCompletion.Contains(currentChar)) {
                    beforeChar = _texteditor.Document.GetCharAt(_texteditor.CaretOffset - 2);
                    if(currentChar != ' ' && !(beforeChar == ' ' || beforeChar == '\t' || beforeChar == '\n'))
                        return;
                }

                //carretChar = _texteditor.Document.GetCharAt(_texteditor.CaretOffset -1);

                //var tokenLine = _projectitem.SegmentService.QueryCodeTokenLine(_texteditor.TextArea.Caret.Line);
                //if(tokenLine.IsEmpty)
                //    return;

                segment = _projectitem.SegmentService.QueryCodeSegmentAt(_texteditor.TextArea.Caret.Offset);
                if(segment.Token == Token.TraditionalString || segment.Token == Token.LiteralString)
                    return;

            } catch {
                return;
            }

            TaskEx.Run(() => HandleCompletionEvent(currentChar, segment, e));
        }


        async void HandleCompletionEvent(char currentChar, CodeSegment segment, TextCompositionEventArgs e) {

            if(e.Text.Length == 1 && !omitCodeCompletion.Contains(currentChar)) {
                // this is just for first debugging purposes
                // as this code belongs to a completion service which handles and caches those completion items

                // Open code completion after the user has pressed dot:
                if(e.Text == ".") {

                    //ensure we have a updated tokenizer
                    _projectitem.EnsureTokenizerHasWorked();
                    await _projectitem.AST.CompileTokenFileAsync();


                    Application.Current.Dispatcher.Invoke(new Action(() => {

                        IList<ICompletionData> data = CreateNewCompletionWindow().CompletionList.CompletionData;
                        // do type lookup & list avaiable members
                        var ctx = _projectitem.AST.GetCodeContext(_texteditor.CaretOffset - 1, true);

                        if(ctx != null && ctx.Segment != null) {
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
                        }
                        if(data.Any()) {
                            _completionWindow.StartOffset++;
                            _completionWindow.Show();
                        }
                    }));
                } else if(_completionWindow == null && e.Text != "\n" &&
                    ((AsciiHelper.IsAsciiLiteralLetter(currentChar) || allowedspecailChars.Contains(currentChar))
                    && !AsciiHelper.IsAsciiNum(currentChar))) { // && !whitespaces.Contains(carretChar)
                    // show avaiable global Methods & build in Methods + commands

                    if(segment == null) {
                        return;
                    } else {
                        if(segment.Token == Token.MultiLineComment || segment.Token == Token.SingleLineComment)
                            return;
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() => {

                        IList<ICompletionData> data = CreateNewCompletionWindow().CompletionList.CompletionData;
                        foreach(var item in GetStaticCompletionItems()) {
                            data.Add(item);
                        }

                        CodeContext ctx;
                        if(_texteditor.Document.TextLength > _texteditor.CaretOffset) {
                            ctx = _projectitem.AST.GetCodeContext(_texteditor.CaretOffset, true);
                        } else {
                            // get root type context
                            ctx = new CodeContext(_projectitem.AST);
                            ctx.EnclosingType = _projectitem.AST.GetRootTypeSnapshot();
                        }

                        foreach(var m in ctx.GetVisibleMembers()) {
                            data.Add(CompletionItem.Build(m));
                        }
                        //_completionWindow.StartOffset++;
                        _completionWindow.Show();

                    }));
                }
            } else if(whitespaces.Contains(currentChar)) {

                if(segment != null) {
                    var s = segment.PreviousOmit(TokenHelper.WhiteSpaces);
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        if(s != null && s.Token == Token.KeyWord && (s.TokenString.Equals("new", StringComparison.CurrentCultureIgnoreCase)
                            || s.TokenString.Equals("extends", StringComparison.CurrentCultureIgnoreCase))) {

                            var ctx = _projectitem.AST.GetCodeContext(_texteditor.CaretOffset);
                            IList<ICompletionData> data = CreateNewCompletionWindow().CompletionList.CompletionData;
                            bool any = false;
                            foreach(var m in ctx.GetVisibleMembers()) {
                                if(m is CodeTypeDeclaration) {
                                    data.Add(CompletionItem.Build(m));
                                    any = true;
                                }
                            }
                            if(any) {
                                _completionWindow.StartOffset++;
                                _completionWindow.Show();
                            }
                        }
                    }));

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
                foreach(var directive in _projectitem.CodeLanguage.LanguageDirectives) {
                    it.AddStatic(new CompletionItemKeyword(directive));
                }
                CompletionCache.Instance[_projectitem.CodeLanguage] = it;
            }
            return it.GetAllStaticCompletionItems();
        }

        #endregion


    }
}
