using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using System.IO;

namespace SmartDevelop.Model.StdLib
{
    public static class StdLibLoader
    {
        // todo put those in config
        const string AHK_STDLIB = @"C:\Program Files\AutoHotkey\Lib";
        const string AHK_STDLIB_x64 = @"C:\Program Files (x86)\AutoHotkey\Lib";

        public static SmartCodeProject LoadStLib() {
            SmartCodeProject stdlib = new SmartCodeProject("Std Lib");

            foreach(var file in Directory.GetFiles(AHK_STDLIB_x64)) {
                var p = ProjectItemCode.FromFile(file, stdlib);
                if(p != null)
                    stdlib.Add(p);
            }
            return stdlib;
        }
    }
}
