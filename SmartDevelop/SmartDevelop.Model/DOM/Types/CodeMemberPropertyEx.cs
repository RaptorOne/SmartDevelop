using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns.Utils;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeMemberPropertyEx : CodeMemberProperty, ICodeMemberEx
    {
        SmartCodeProject _project;

        #region Constrcutors

        public CodeMemberPropertyEx(ProjectItemCode codeItem) : base() {
            ThrowUtil.ThrowIfNull(codeItem);
            CodeDocumentItem = codeItem;
        }

        public CodeMemberPropertyEx(bool buildIn) 
            : base() { IsBuildInType = buildIn; }

        #endregion

        #region Properties

        public bool IsHidden {
            get;
            set;
        }

        public bool IsBuildInType {
            get;
            set;
        }

        public Projecting.ProjectItemCode CodeDocumentItem {
            get;
            set;
        }

        
        public Projecting.SmartCodeProject Project {
            get { return (CodeDocumentItem != null) ?CodeDocumentItem.Project : _project ; }
            set { _project = value; }
        }

        #endregion

        public CodeSegment TryFindSegment() {
            CodeSegment s = null;

            var codeDocument = this.CodeDocumentItem;
            var segmentLines = codeDocument.SegmentService.GetCodeSegmentLinesMap();

            if(segmentLines.ContainsKey(this.LinePragma.LineNumber)) {
                var segments = segmentLines[this.LinePragma.LineNumber];
                if(!segments.IsEmpty) {
                    s = segments.CodeSegments.Find(x => this.Equals(x.CodeDOMObject));
                }
            }
            return s;
        }
    }
}
