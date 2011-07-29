using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns.Services;
using System.IO;
using SmartDevelop.Model.CodeLanguages;

namespace SmartDevelop
{
    class DemoProjectLoader
    {
        // todo put those in config
        const string AHK_STDLIB = @"C:\Program Files\AutoHotkey\Lib";
        const string AHK_STDLIB_x64 = @"C:\Program Files (x86)\AutoHotkey\Lib";

        public static void AddStdLibTo(SmartCodeProject project) {

            var stdlibFolder = new ProjectItemFolder("StdLib", project);
            var dir = (Directory.Exists(AHK_STDLIB) ? AHK_STDLIB : AHK_STDLIB_x64);
            if(Directory.Exists(dir)) {
                foreach(var file in Directory.GetFiles(dir)) {
                    if(project.Language.Extensions.Contains(Path.GetExtension(file))) {
                        var codeItem = ProjectItemCode.FromFile(file, project);
                        if(codeItem != null)
                            stdlibFolder.Add(codeItem);
                    
                    }
                }
            }
            project.Add(stdlibFolder);
        }
    }
}
