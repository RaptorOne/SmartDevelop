using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokenizing;
using Archimedes.Patterns.Utils;

namespace SmartDevelop.Model.Errors
{
    /// <summary>
    /// Represents a single Error
    /// </summary>
    public class ErrorItem
    {
        readonly CodeSegment _errorSegment;
        readonly ProjectItemCodeDocument _codeProjectItem;
        readonly CodeError _error;

        public ErrorItem(CodeSegment errorSegment, ProjectItemCodeDocument codeItem) {

            ThrowUtil.ThrowIfNull(errorSegment);
            ThrowUtil.ThrowIfNull(codeItem);

            _errorSegment = errorSegment;
            _codeProjectItem = codeItem;
            _error = _errorSegment.ErrorContext;
        }

        public ErrorItem(CodeError err, ProjectItemCodeDocument codeItem) {

            ThrowUtil.ThrowIfNull(err);
            ThrowUtil.ThrowIfNull(codeItem);

            _codeProjectItem = codeItem;
            _error = err;
        }

        /// <summary>
        /// Gets the CodeSegment which has an Error
        /// </summary>
        public CodeSegment Segment {
            get { return _errorSegment; }
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
    }
}
