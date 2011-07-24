using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM.Types;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.DOM;
using System.CodeDom;

namespace SmartDevelop.Model.CodeContexts
{
    public class CodeContext
    {

        public CodeContext(CodeDOMService domservice) { CodeDOMService = domservice; } 

        public CodeDOMService CodeDOMService { get; protected set; }

        public CodeTypeDeclarationEx EnclosingType { get; set; }

        public CodeMemberMethodEx EnclosingMethod { get; set; }

        public CodeSegment Segment { get; set; }


        public virtual IEnumerable<CodeTypeMember> GetVisibleMembers() {
            var members = new List<CodeTypeMember>();

            members.AddRange(from m in CodeDOMService.RootType.Members.Cast<CodeTypeMember>() 
                             let mimp = m as ICodeObjectEx
                             where mimp == null || !mimp.IsHidden
                             select m);

            if(EnclosingType != CodeDOMService.RootType) {
                members.AddRange(EnclosingType.Members.Cast<CodeTypeMember>());
            }

            return members; 
        }

    }
}
