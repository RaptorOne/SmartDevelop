using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.CodeLanguages;
using System.IO;

namespace SmartDevelop.AHK.AHKv1.Projecting.ProjectTemplates
{
    class ProjectTemplateDemo : ProjectTemplate
    {

        public ProjectTemplateDemo(CodeLanguage lang) 
            : base("Demo AHK Project", lang) { }

        public override SmartCodeProject Create(string displayname, string name, string location) {
            var demoProject = new SmartCodeProjectAHK(name,location, this.Language) { DisplayName = displayname };


            // Create stdlib folder alias
            var stdlibdir = Path.GetDirectoryName(((CodeLanguageAHKv1)_language).Settings.InterpreterPath);
            demoProject.StdLib = new ProjectItemFolderSTdLib("StdLib", stdlibdir, demoProject);
            demoProject.Add(demoProject.StdLib);

            // Create local lib folder alias
            demoProject.Add(new ProjectItemFolder("Lib", demoProject));


            // create a Test Folder and add a demo file

            var dp = new ProjectItemCodeDocument(_language, demoProject) { Name = "DemoFile.ahk" };
            demoProject.Add(dp);
            dp.Document.Text = InitialDemoCode();
            dp.QuickSave();
            dp.IsStartUpDocument = true;
            dp.ShowInWorkSpace(); // present our demo file to the user


            dp = new ProjectItemCodeDocument(_language, demoProject.LocalLib) { Name = "DemoIncludeMe.ahk" };
            demoProject.LocalLib.Add(dp);
            dp.Document.Text = InitialDemoIncludeLibCode();
            dp.QuickSave();


            dp = new ProjectItemCodeDocument(_language, demoProject) { Name = "Car.ahk" };
            demoProject.Add(dp);
            dp.Document.Text = CarFileCode();
            dp.QuickSave();

            dp = new ProjectItemCodeDocument(_language, demoProject) { Name = "AeroPlane.ahk" };
            demoProject.Add(dp);
            dp.Document.Text = AeroPlaneFileCode();
            dp.QuickSave();

            return demoProject;
        }


        #region Static demo codes

        static string AeroPlaneFileCode() {
            return
@"
; Demo Include filepath

/*
    This is an AeroPlane
*/
class AeroPlane
{
    var JetSets ; :-P
    
    Fly(){
        ; run it! :D
    }
}
";
        }



        static string CarFileCode() {
            return
@"
#Include %A_ScriptDir%\AeroPlane.ahk


; Demo Include filepath

/*
    This is a Car
*/
class Car
{
    var Wheels
    
    Run(){
        ; run it! :D
    }
}
";
        }

        static string InitialDemoIncludeLibCode() {
            return
@"

#Include AeroPlane.ahk

; Demo Include

/*
    This is a Method in a include file
*/
IncludeTestMethod(){
    return 0x44
}
";
        }


        static string InitialDemoCode() {
            return @"
    ; Demo Code
    #Include <DemoIncludeMe>
    #Include %A_ScriptDir%\Car.ahk

	; dynamic mini expression evaluator:
	sk += !(a3 == """" ? (sub != """")
		: a3 == ""<="" ? sub <= a5)


    fooinst := new Foo
    str := fooinst.Helper()
    msgbox, 0, I'm a traditional String with a Variable %str%`, and with escape sequecnces `% which is really cool``, % Sin(33) . ""also inline expressions are supported!""
    val = I'm a traditional assignment, for sure!`nThe Result is %str%!
    Run, C:\Folder\%str%
    RunFail

    msgbox =msgbox
    msgbox % msgbox
    msgbox % Add(44, 33)

    plane := new AeroPlane
    mycar := new Car

    ExitApp
    
    ;;;
    ;;; Functions & Classes
    ;;;

    /*
    	Returns the sum of the given numbers
    */
    Add(a,b){
    	return a + b
    }


    /*
        This is a base for all Foos out there
    */
    class Bar
    {
	    var TestProperty
	
	    SimpleMethod(){
		    return ""The property is:"" this.TestProperty
	    }

        /*
            Example Documentation comment
        */
	    Test(num){
            return 0x44 << num
	    }

        MethodWhichQuits(){
            ExitApp
        }
        MethodWhichQuits2(errcode){
            Exit, %errcode%
        }

        __new(){
            this.TestProperty := ""Bar's TestProperty""
        }
    }


    /*
        This is an example sub class
    */
    class Foo extends Bar
    {
        var TestProperty ;property override
        var SubClassProperty := ""fal""

	    Helper(){
		    if(this.SubClassProperty == ""fal""){
                return this.Test(this.TestProperty)
		    }
	    }

        __new(){
            this.TestProperty := ""Foo's TestProperty""
        }

    }";
        }

        #endregion

    }
}
