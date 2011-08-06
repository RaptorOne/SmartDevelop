using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Patterns;
using SmartDevelop.Model.CodeLanguages;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeTypeDeclarationEx : CodeTypeDeclaration, ICodeMemberEx, ICloneable, IEquatable<CodeTypeDeclarationEx>
    {
        SmartCodeProject _project;
        Projecting.ProjectItemCodeDocument _codeDocumentItem;


        #region Constructors

        public CodeTypeDeclarationEx() 
            : base() { }

        public CodeTypeDeclarationEx(ProjectItemCodeDocument codeitem, string name)
            : base(name) { CodeDocumentItem = codeitem; }

        public CodeTypeDeclarationEx(ProjectItemCodeDocument codeitem, string name, bool buildin) 
            : base(name) { IsBuildInType = buildin; CodeDocumentItem = codeitem; }

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



        public ProjectItemCodeDocument CodeDocumentItem {
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

        protected CodeLanguage Language {
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
            return string.Format("\nTypeDeclaration: {0}", this.Name);
        }

        #region ICloneable

        /// <summary>
        /// Clones this TypeDeclaration. Members won't be cloned
        /// </summary>
        /// <returns></returns>
        public virtual object Clone() {

            var clone = new CodeTypeDeclarationEx(this.CodeDocumentItem, this.Name, this.IsBuildInType)
            {
                Project = this.Project,
                IsClass = this.IsClass,
                IsEnum = this.IsEnum,
                IsHidden = this.IsHidden,
                IsInterface = this.IsInterface,
                IsStruct = this.IsStruct,
                IsPartial = this.IsPartial,
                Parent = (this.Parent != null) ? this.Parent.Clone() as CodeTypeDeclarationEx : null,
                LinePragma = CloneHelper.Clone(this.LinePragma)
            };

            foreach(CodeCommentStatement comment in this.Comments) {
                clone.Comments.Add(CloneHelper.Clone(comment));
            }

            foreach(CodeTypeReferenceEx typeRef in this.BaseTypes) {
                clone.BaseTypes.Add(typeRef.Clone() as CodeTypeReferenceEx);
            }
            
            foreach(CodeTypeMember member in this.Members){
                //if(member is ICloneable)
                //    clone.Members.Add(((ICloneable)member).Clone() as CodeTypeMember);
                //else
                //    throw new NotSupportedException("type doesn't implement ICloneable!");

                clone.Members.Add(member);
            }

            return clone;
        }

        #endregion

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
