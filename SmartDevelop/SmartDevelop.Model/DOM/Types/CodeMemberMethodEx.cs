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
    public class CodeMemberMethodEx : CodeMemberMethod, ICodeMemberEx, ICloneable, IEquatable<CodeMemberMethodEx>
    {
        #region Fields

        SmartCodeProject _project;
        Projecting.ProjectItemCodeDocument _codeDocumentItem;

        #endregion

        #region Constructor

        public CodeMemberMethodEx(Projecting.ProjectItemCodeDocument codeDocumentItem) 
            : base() {
                ThrowUtil.ThrowIfNull(codeDocumentItem);
                _codeDocumentItem = codeDocumentItem;
        }

        public CodeMemberMethodEx(bool buildIn) 
            : base() { IsBuildInType = buildIn; }

        #endregion

        #region Methods

        public CodeSegment TryFindSegment() {
            CodeSegment s = null;
            if(CodeDocumentItem != null) {
                var segmentLines = CodeDocumentItem.SegmentService.GetCodeSegmentLinesMap();

                if(segmentLines.ContainsKey(this.LinePragma.LineNumber)) {
                    var segments = segmentLines[this.LinePragma.LineNumber];
                    if(!segments.IsEmpty) {
                        s = segments.CodeSegments.Find(x => this.Equals(x.CodeDOMObject));
                    }
                }
            }
            return s;
        }

        public string GetParamInfo() {
            string str = "";
            foreach(CodeParameterDeclarationExpression p in this.Parameters) {
                str += p.Name + ", ";
            }
            return str.TrimEnd(' ', ',');
        }

        #endregion

        #region Properties

        public CodeTypeDeclarationEx DefiningType {
            get;
            set;
        }

        public bool IsHidden {
            get;
            set;
        }

        public bool IsBuildInType {
            get;
            protected set;
        }


        public ProjectItemCodeDocument CodeDocumentItem {
            get { return _codeDocumentItem ; }
            set { _codeDocumentItem = value; }
        }

        public SmartCodeProject Project {
            get {
                return (CodeDocumentItem != null) ? CodeDocumentItem.Project : _project;
            }
            set {
                _project = value;
            }
        }

        public CodeLanguages.CodeLanguage Language { get { return Project.Language; } }

        #endregion

        #region IEquatable

        public bool Equals(CodeMemberMethodEx other) {
            if(other == null)
                return false;
            return other.Name.Equals(this.Name, Language.NameComparisation);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeMemberMethodEx);
        }

        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        #endregion

        public bool Matches(CodeTypeReferenceEx codeTypeRef) {
            if(codeTypeRef == null) return false;
            return this.Name.Equals(codeTypeRef.TypeName, Language.NameComparisation);
        }

        public override string ToString() {
            return "\nMethod Declaration: " + this.Name + "()";
        }


        public object Clone() {
            var mehtod = new CodeMemberMethodEx(this.IsBuildInType)
            {
                CodeDocumentItem = this.CodeDocumentItem,
                Name = this.Name,
                Project = this.Project,
                IsHidden = this.IsHidden,
                ReturnType = (this.ReturnType as CodeTypeReferenceEx).Clone() as CodeTypeReferenceEx,
                LinePragma = CloneHelper.Clone(this.LinePragma),
            };

            foreach(var p in this.Parameters){
                mehtod.Parameters.Add(((CodeParameterDeclarationExpressionEx)p).Clone() as CodeParameterDeclarationExpressionEx);
            }

            return mehtod;
        }
    }
}
