using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM.Types;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.AHK.AHKv1.DOM.Types
{
    public class CodeMemberMethodExAHK : CodeMemberMethodEx
    {
        public CodeMemberMethodExAHK(ProjectItemCodeDocument codeDocumentItem)
            : base(codeDocumentItem) {

            IsTraditionalCommand = false;
            IsDefaultMethodInvoke = true;
        }

        public CodeMemberMethodExAHK(bool buildin) 
            : base(buildin) { }

        public bool IsFlowCommand {
            get;
            set;
        }


        public bool IsTraditionalCommand {
            get;
            set;
        }

        public bool IsDefaultMethodInvoke {
            get;
            set;
        }

        public override string ToString() {
            if(IsTraditionalCommand && !IsDefaultMethodInvoke)
                return string.Format("Command {0}\n{1}", this.Name, Helper.GetDocumentCommentString(Comments));
            else
                return string.Format("{0}()\n{1}", this.Name, Helper.GetDocumentCommentString(Comments));
        }

    }
}
