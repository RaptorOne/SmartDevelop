using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using System.IO;
using Archimedes.Patterns.Services;
using SmartDevelop.Model.CodeLanguages;

namespace SmartDevelop.Model.StdLib
{
    public static class StdLibLoader
    {
        // todo put those in config
        const string AHK_STDLIB = @"C:\Program Files\AutoHotkey\LibDEBUG";
        const string AHK_STDLIB_x64 = @"C:\Program Files (x86)\AutoHotkey\LibDEBUG";
       
        public static SmartCodeProject LoadStLib() {

            var serviceLang = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
            SmartCodeProject stdlib = new SmartCodeProject("Std Lib", serviceLang.GetById("ahk-v1.1")); // TODO 

            foreach(var file in Directory.GetFiles(Directory.Exists(AHK_STDLIB) ? AHK_STDLIB : AHK_STDLIB_x64)) {
                var p = ProjectItemCode.FromFile(file, stdlib);
                if(p != null)
                    stdlib.Add(p);
            }
            return stdlib;
        }
    }
}
