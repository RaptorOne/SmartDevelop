using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using SmartDevelop.Model.CodeContexts;
using SmartDevelop.Model.CodeLanguages;
using Archimedes.Patterns.Utils;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeTypeReferenceEx : CodeTypeReference, ICloneable, IEquatable<CodeTypeReferenceEx>
    {
        #region Fields

        string _name;
        Type _type;

        CodeTypeDeclarationEx _typeDeclaration;
        CodeTypeDeclarationEx _enclosingType;

        #endregion

        #region Constructor

        public CodeTypeReferenceEx(ProjectItemCodeDocument item, string name, CodeTypeDeclarationEx enclosingType)
            : base(name) {

                ThrowUtil.ThrowIfNull(item);

                CodeDocumentItem = item;
                _name = name;
                _enclosingType = enclosingType;
        }

        public CodeTypeReferenceEx(ProjectItemCodeDocument item, Type type)
            : base(type) {

            ThrowUtil.ThrowIfNull(item);

            CodeDocumentItem = item;
            _type = type;
            _name = type.Name;
        }

        #endregion

        #region Properties

        public string TypeName {
            get { return _name; }
        }

        public CodeTypeDeclarationEx ResolvedTypeDeclaration {
            get { return _typeDeclaration; }
            set { _typeDeclaration = value; }
        }

        
        public CodeTypeDeclarationEx EnclosingType {
            get { return _enclosingType; }
            //set { _enclosingType = value; }
        }


        SmartCodeProject _project;
        Projecting.ProjectItemCodeDocument _codeDocumentItem;

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

        public string CommentInfo {
            get {
                if(ResolvedTypeDeclaration != null) {
                    return Helper.GetDocumentCommentString(ResolvedTypeDeclaration.Comments);
                }
                return null;
            }
        }

        #endregion

        #region Public Methods

        public CodeTypeDeclarationEx ResolveTypeDeclarationCache() {

            if(_typeDeclaration == null && EnclosingType != null) {

                CodeTypeDeclarationEx typedecl = EnclosingType;
                while(typedecl != null) {
                    var type = typedecl.Members.Cast<CodeTypeMember>().ToList().Find(
                    (x) => {
                        var typedeclaration = x as CodeTypeDeclarationEx;
                        return typedeclaration != null && typedeclaration.Name.Equals(this.TypeName);
                    }) as CodeTypeDeclarationEx;
                    if(type != null) {
                        _typeDeclaration = type;
                        break;
                    } else
                        typedecl = typedecl.Parent;
                }
            }
            return _typeDeclaration;
        }

        #endregion

        #region IEquatable

        public bool Equals(CodeTypeReferenceEx other) {
            if(other == null)
                return false;
            return other.TypeName.Equals(this.TypeName, Project.Language.NameComparisation);
        }

        public override bool Equals(object obj) {
            return Equals(obj as CodeTypeReferenceEx);
        }

        public override int GetHashCode() {
            return this.TypeName.GetHashCode();
        }
        #endregion

        public override string ToString() {
            return string.Format("{0}\n{1}", this.TypeName, this.CommentInfo);
        }

        public object Clone() {
            var clone = new CodeTypeReferenceEx(this.CodeDocumentItem, this.TypeName, this.EnclosingType)
            {
                Project = this.Project,
                BaseType = this.BaseType,
                ResolvedTypeDeclaration = this.ResolvedTypeDeclaration.Clone() as CodeTypeDeclarationEx,
            };
            return clone;
        }
    }
}
