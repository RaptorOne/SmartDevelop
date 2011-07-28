using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.CodeLanguages;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeTypeDeclarationEx : CodeTypeDeclaration, ICodeMemberEx, IEquatable<CodeTypeDeclarationEx>
    {

        #region Constructors

        public CodeTypeDeclarationEx() : base() { }
        public CodeTypeDeclarationEx(ProjectItemCode codeitem, string name) : base(name) { CodeDocumentItem = codeitem; }
        public CodeTypeDeclarationEx(ProjectItemCode codeitem, string name, bool buildin) : base(name) { IsBuildInType = buildin; CodeDocumentItem = codeitem; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent of this type if it is nested otherwise returns null
        /// </summary>
        public CodeTypeDeclarationEx Parent {
            get;
            set;
        }

        public bool IsHidden {
            get;
            set;
        }

        public bool IsBuildInType {
            get;
            set;
        }

        SmartCodeProject _project;
        Projecting.ProjectItemCode _codeDocumentItem;

        public ProjectItemCode CodeDocumentItem {
            get { return _codeDocumentItem; }
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

        CodeLanguage Language {
            get {
                return Project!=null ? Project.Language : null;
            }
        }

        #endregion

        #region Public Methods

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



        public virtual IEnumerable<CodeTypeMember> GetInheritedMembers() {
            List<CodeTypeMember> members = new List<CodeTypeMember>();
            members.AddRange(this.Members.Cast<CodeTypeMember>());

            foreach(CodeTypeReferenceEx t in BaseTypes) {
                if(t.ResolvedTypeDeclaration == null)
                    t.ResolveTypeDeclarationCache();

                if(t.ResolvedTypeDeclaration != null) {
                    members.AddRange(t.ResolvedTypeDeclaration.GetInheritedMembers());
                }
            }
            return members;
        }


        /// <summary>
        /// Checks if the asked typeref is a subclass of the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsSubclassOf(CodeTypeReferenceEx type) {
            var typerefs = this.BaseTypes.Cast<CodeTypeReferenceEx>().ToList();
            if(typerefs.Contains(type)) {
                return true;
            } else {
                foreach(CodeTypeReferenceEx bt in typerefs) {
                    var btdecl = bt.ResolveTypeDeclarationCache();
                    if(btdecl != null && btdecl.IsSubclassOf(type)) {
                        return true;
                    }
                }
            }
            return false;
        }


        public virtual IEnumerable<CodeTypeReferenceEx> GetBaseTypeHirarchy() {
            List<CodeTypeReferenceEx> types = new List<CodeTypeReferenceEx>();
            foreach(CodeTypeReferenceEx bt in this.BaseTypes) {
                types.Add(bt);
                var btdecl = bt.ResolveTypeDeclarationCache();
                if(btdecl != null) {
                    types.AddRange(btdecl.GetBaseTypeHirarchy());
                }
            }
            return types;
        }

        #endregion

        public override string ToString() {
            return string.Format("TypeDeclaration: {0}", this.Name);
        }

        #region IEquatable

        public virtual bool Equals(CodeTypeDeclarationEx other) {
            if(other == null)
                return false;
            return this.Name.Equals(other.Name, Language.NameComparisation);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeTypeDeclarationEx);
        }

        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        #endregion



    }
}
