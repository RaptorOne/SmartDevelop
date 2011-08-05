using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.Projecting
{
    public interface IEditor
    {
        string Text { get; }
        int SelectionStart { get; }
        int SelectionLength { get; }
        /// <summary>
        /// Selects the specified portion of Text and scrolls that part into view.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        void Select(int start, int length);
        void Replace(int start, int length, string ReplaceWith);
        /// <summary>
        /// This method is called before a replace all operation.
        /// </summary>
        void BeginChange();
        /// <summary>
        /// This method is called after a replace all operation.
        /// </summary>
        void EndChange();
    }
}
