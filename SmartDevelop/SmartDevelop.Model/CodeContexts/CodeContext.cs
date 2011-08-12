using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM.Types;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.DOM;
using System.CodeDom;
using SmartDevelop.Model.Tokenizing;
using Archimedes.Patterns.Utils;

namespace SmartDevelop.Model.CodeContexts
{
    /// <summary>
    /// Represents a Code Context
    /// </summary>
    public class CodeContext
    {

        public CodeContext(CodeDocumentDOMService domservice) {
            ThrowUtil.ThrowIfNull(domservice);
            CodeDOMService = domservice; 
        }
        CodeContext() { }

        public CodeDocumentDOMService CodeDOMService { get; protected set; }

        public CodeTypeDeclarationEx EnclosingType { get; set; }

        public CodeMemberMethodEx EnclosingMethod { get; set; }

        public CodeSegment Segment { get; set; }


        public virtual IEnumerable<CodeObject> GetVisibleMembers() {
            var members = new List<CodeObject>();

            var rootSnapshot = CodeDOMService.GetRootTypeSnapshot();

            if(EnclosingMethod != null) {
                // add all local Method variables here
                foreach(CodeParameterDeclarationExpression param in EnclosingMethod.Parameters) {
                    members.Add(param);
                }
            }

            members.AddRange(from m in rootSnapshot.Members.Cast<CodeTypeMember>()
                                let mimp = m as ICodeMemberEx
                                where mimp == null || !mimp.IsHidden
                                select m);

            if(EnclosingType != null && !(EnclosingType is CodeTypeDeclarationRoot)) {
                members.AddRange(EnclosingType.GetInheritedMembers());
            }

            return members; 
        }

        static CodeContext _empty = new CodeContext();
        public static CodeContext Empty { get { return _empty; } }
    }
}
