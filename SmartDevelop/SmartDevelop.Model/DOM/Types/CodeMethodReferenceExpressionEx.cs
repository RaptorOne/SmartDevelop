using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CodeMethodReferenceExpressionEx : CodeMethodReferenceExpression
    {
        CodeMemberMethodEx _methodref;

        public CodeMethodReferenceExpressionEx(CodeExpression target, string methodName) 
            : base(target, methodName) { 
        }

        public CodeMemberMethodEx FindCached(CodeContexts.CodeContext ctx) {
            if(_methodref == null)
                return Find(ctx);
            return _methodref;
        }

        public CodeMemberMethodEx Find(CodeContexts.CodeContext ctx) {
            var methods = from m in ctx.GetVisibleMembers()
                          let met = m as CodeMemberMethodEx
                          where met != null && met.Name.Equals(this.MethodName, StringComparison.CurrentCultureIgnoreCase)
                          select m as CodeMemberMethodEx;
            if(methods.Any()){
            _methodref = methods.First();
            }
            return _methodref;
        }
    }
}
