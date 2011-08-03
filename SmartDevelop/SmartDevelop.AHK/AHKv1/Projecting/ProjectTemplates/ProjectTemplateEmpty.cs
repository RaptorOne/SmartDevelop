using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.CodeLanguages;
using System.IO;

namespace SmartDevelop.AHK.AHKv1.Projecting.ProjectTemplates
{
    class ProjectTemplateEmpty : ProjectTemplate
    {

        public ProjectTemplateEmpty(CodeLanguage lang) 
            : base("Empty AHK Project", lang) { }


        public override SmartCodeProject Create(string displayname, string name, string location) {

            var p = new SmartCodeProjectAHK(name, location, this.Language) { DisplayName = displayname };
            p.BeginProjectUpdate();
            {
                // Create stdlib folder alias
                var stdlibdir = Path.GetDirectoryName(((CodeLanguageAHKv1)_language).Settings.InterpreterPath);
                p.StdLib = new ProjectItemFolderSTdLib("StdLib", stdlibdir, p);
                p.Add(p.StdLib);

                // Create local lib folder alias
                p.Add(new ProjectItemFolder("Lib", p));

                var dp = new ProjectItemCodeDocument(_language, p) { Name = "New.ahk" };
                p.Add(dp);
                dp.Document.Text = InitialEmptyFile();
                dp.QuickSave();
                dp.IsStartUpDocument = true;
                //dp.ShowInWorkSpace(); // present our demo file to the user
            } p.EndProjectUpdate();

            return p;
        }


        string InitialEmptyFile() {
            return 
@"
#NoEnv

";
        }
    }
}
