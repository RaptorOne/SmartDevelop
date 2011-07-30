using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.Model.Errors
{
    /// <summary>
    /// Represents a single Error
    /// </summary>
    public class ErrorItem
    {
        readonly CodeSegment _errorSegment;
        readonly ProjectItemCodeDocument _codeProjectItem;

        public ErrorItem(CodeSegment errorSegment, ProjectItemCodeDocument codeItem) {
            _errorSegment = errorSegment;
            _codeProjectItem = codeItem;
        }

        /// <summary>
        /// Gets the CodeSegment which has an Error
        /// </summary>
        public CodeSegment Segment {
            get { return _errorSegment; }
        }

        /// <summary>
        /// Gets the Code Projectitem 
        /// </summary>
        public ProjectItemCodeDocument CodeItem {
            get { return _codeProjectItem; }
        }
    }
}
