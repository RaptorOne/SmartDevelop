using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM.Types;

namespace SmartDevelop.AHK.AHKv1.DOM.Types
{
    public class CodeMemberMethodExAHK : CodeMemberMethodEx
    {
        public CodeMemberMethodExAHK() : base() {
            IsTraditionalCommand = false;
            IsDefaultMethodInvoke = true;
        }
        public CodeMemberMethodExAHK(bool buildin) : base(buildin) { }


        public bool IsTraditionalCommand {
            get;
            set;
        }

        public bool IsDefaultMethodInvoke {
            get;
            set;
        }

    }
}
