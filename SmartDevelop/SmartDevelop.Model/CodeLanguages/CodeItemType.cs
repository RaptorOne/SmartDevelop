using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.CodeLanguages
{
    [Obsolete("Hardcoded Languages are obsolete, use CodeLanguage-Ids")]
    public enum CodeItemType
    {
        None = 0,
        IA = 1,
        AHK = 2,
        AHK_L = 4,
        AHK2 = 8
    }
}
