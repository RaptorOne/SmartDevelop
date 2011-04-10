using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using WPFCommon.ViewModels;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Controls;

namespace SmartDevelop.ViewModel.DocumentFiles
{
    public class CodeFileViewModel : WorkspaceViewModel
    {
        readonly TextEditor _texteditor = new TextEditor();

        public CodeFileViewModel(TextDocument doc) {
            _texteditor.Document = doc;

            _texteditor.MouseHover += TextEditorMouseHover;
            _texteditor.TextArea.TextEntered += OnTextEntered;
            _texteditor.TextArea.TextEntering += OnTextEntering;
        }

        #region Properties

        public TextEditor Editor {
            get {
                return _texteditor;
            }
        }

        #endregion

        #region Event Handlers

        CompletionWindow completionWindow;

        void OnTextEntered(object sender, TextCompositionEventArgs e) {
            if(e.Text == ".") {
                // Open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(_texteditor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                //data.Add(new MyCompletionData("Item1"));
                //data.Add(new MyCompletionData("Item2"));
                //data.Add(new MyCompletionData("Item3"));
                //completionWindow.Show();

                //completionWindow.Closed += delegate
                //{
                //    completionWindow = null;
                //};
            }
        }

        void OnTextEntering(object sender, TextCompositionEventArgs e) {
            if(e.Text.Length > 0 && completionWindow != null) {
                if(!char.IsLetterOrDigit(e.Text[0])) {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        ToolTip _toolTip = new ToolTip();

        void TextEditorMouseHover(object sender, MouseEventArgs e) {
            var pos = _texteditor.GetPositionFromPoint(e.GetPosition(_texteditor));

            if(pos != null) {
                //_toolTip.PlacementTarget = _texteditor; // required for property inheritance
                _toolTip.Content = pos.ToString();
                _toolTip.IsOpen = true;
                e.Handled = true;
            }
        }

        void TextEditorMouseHoverStopped(object sender, MouseEventArgs e) {
            _toolTip.IsOpen = false;
        }

        #endregion

    }
}
