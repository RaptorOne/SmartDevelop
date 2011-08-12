using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokenizing;
using Archimedes.Patterns.Utils;
using ICSharpCode.AvalonEdit.Document;

namespace SmartDevelop.Model.Errors
{

    public enum ErrorSource
    {
        ASTParser = 0,
        External = 1
    }


    /// <summary>
    /// Represents a single Error
    /// </summary>
    public class ErrorItem
    {
        ErrorSource _errorSource;
        readonly CodeSegment _errorSegment;
        readonly ProjectItemCodeDocument _codeProjectItem;
        readonly CodeError _error;

        #region Constructor

        /// <summary>
        /// Creates an ErrorContext for a single Codesegment
        /// </summary>
        /// <param name="errorSegment"></param>
        /// <param name="codeItem"></param>
        public ErrorItem(CodeSegment errorSegment, ProjectItemCodeDocument codeItem) {

            ThrowUtil.ThrowIfNull(errorSegment);
            ThrowUtil.ThrowIfNull(codeItem);

            _errorSegment = errorSegment;
            _codeProjectItem = codeItem;
            _error = _errorSegment.ErrorContext;
            _range = errorSegment.Range;
            ColumnStart = errorSegment.ColumnStart;
            StartLine = errorSegment.LineNumber;
        }

        /// <summary>
        /// An error in a file whit undefined location
        /// </summary>
        /// <param name="err"></param>
        /// <param name="codeItem"></param>
        public ErrorItem(CodeError err, ProjectItemCodeDocument codeItem) {

            ThrowUtil.ThrowIfNull(err);
            ThrowUtil.ThrowIfNull(codeItem);

            _codeProjectItem = codeItem;
            _error = err;
        }


        /// <summary>
        /// An error in a file whit undefined location
        /// </summary>
        /// <param name="err"></param>
        /// <param name="codeItem"></param>
        public ErrorItem(int line, ProjectItemCodeDocument codeItem, string errorDescription) {
            ThrowUtil.ThrowIfNull(codeItem);

            _codeProjectItem = codeItem;
            StartLine = line;
            _error = new CodeError() { Description = errorDescription };
        }


        #endregion

        /// <summary>
        /// Brings that Error in the editor into viewscope
        /// </summary>
        public void BringIntoView() {
            _codeProjectItem.ShowInWorkSpace();
            if(Range != null)
                _codeProjectItem.SetDocumentPosition(Range.Offset);
            else {
                   var offset = _codeProjectItem.Document.GetOffset(StartLine, 0);
                   _codeProjectItem.SetDocumentPosition(offset);
                }
        }

        #region Properties

        ISegment _range;
        public ISegment Range {
            get { return _range; }
        }

        public int ColumnStart {
            get;
            set;
        }

        public int StartLine {
            get;
            set;
        }

        public ErrorSource ErrorSource {
            get { return _errorSource; }
            set { _errorSource = value; }
        }

        public CodeError Error {
            get { return _error; }
        }


        /// <summary>
        /// Gets the Code Projectitem 
        /// </summary>
        public ProjectItemCodeDocument CodeItem {
            get { return _codeProjectItem; }
        }

        #endregion


    }
}
