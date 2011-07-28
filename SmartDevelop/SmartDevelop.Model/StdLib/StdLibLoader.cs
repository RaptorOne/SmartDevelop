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
        const string AHK_STDLIB = @"C:\Program Files\AutoHotkey\Lib";
        const string AHK_STDLIB_x64 = @"C:\Program Files (x86)\AutoHotkey\Lib";
       
        public static SmartCodeProject LoadStLib() {

            var serviceLang = ServiceLocator.Instance.Resolve<ICodeLanguageService>();
            var language = serviceLang.GetById("ahk-v1.1");
            SmartCodeProject demoProject = new SmartCodeProject("Demo Project", language); // TODO 

            var stdlibFolder = new ProjectItemFolder("StdLib", demoProject);
            var dir = (Directory.Exists(AHK_STDLIB) ? AHK_STDLIB : AHK_STDLIB_x64);
            //if(Directory.Exists(dir)) {
            //    foreach(var file in Directory.GetFiles(dir)) {
            //        if(demoProject.Language.Extensions.Contains(Path.GetExtension(file))) {
            //            var codeItem = ProjectItemCode.FromFile(file, demoProject);
            //            if(codeItem != null)
            //                stdlibFolder.Add(codeItem);
            //        }
            //    }
            //}
            demoProject.Add(stdlibFolder);

            var testFolder = new ProjectItemFolder("Test", demoProject);
            demoProject.Add(testFolder);
            var dp = new ProjectItemCode(language, testFolder) { Name = "DemoFile.ahk" };
            testFolder.Add(dp);
            dp.Document.Text = InitialDemoCode();


            dp.OnRequestShowDocument(); 
            // this wont work yet as there is now VM which is listening to this event now.
            // we may have to build some kind of document manager.
            // this may also resolve the odd event blubbling of the tokenizer-updated event 


            return demoProject;
        }


        static string InitialDemoCode() {
    return @"
    foo := {}
    if(world == ""bad""){
	    foo := new Foo()	
    }

    class Foo extends Bar
    {
	    Helper(){
		    if(seg == fal){
			    Test()	
		    }
	    }
	
	    Test(){
	    }
    }


    class Bar
    {
	    var Barbare
	
	    SimpleMethod(){
		    this.Barbare
	    }
    }";
        }
    }
}
