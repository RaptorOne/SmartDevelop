using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Media;

namespace SmartDevelop.ViewModel.CodeCompleting
{
    public class CompletionItem : ICompletionData
    {
        string _text;
        string _description;


        public CompletionItem(string text, string description) {
            _text = text;
            _description = description;
        }

        public virtual ImageSource Image {
            get { return null; }
        }

        public virtual string Text {
            get { return _text; }
        }

        public virtual object Content {
            get { return _text; }
        }

        public virtual object Description {
            get { return _description; }
        }

        public virtual double Priority {
            get { return 1; }
        }

        public virtual void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, EventArgs insertionRequestEventArgs) {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
