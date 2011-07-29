using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM.Types;
using System.CodeDom;
using SmartDevelop.AHK.AHKv1.DOM.Types;
using SmartDevelop.AHK.AHKv1.Tokenizing;
using SmartDevelop.Model.CodeLanguages;

namespace SmartDevelop.AHK.AHKv1
{
    /// <summary>
    /// This is just for debuging Purposes
    /// </summary>
    public static  class CodeLanguageAHKBuildinMethods
    {

        static List<char> SpecailWordCharacters = new List<char> { '_' };


        public static IEnumerable<CodeTypeMember> ReadMembers() {

            var members = new List<CodeTypeMember>();

            #region Create the build in Members Methods

            // for now we add some of the manualy for debug puposes

            #region Command vs Method Example

            CodeMemberMethodEx method;

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Msgbox",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = true
            };
            //, Options, Title, Text, Timeout]
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "Options"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Title"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Text"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Timeout"));
            method.Comments.Add(new CodeCommentStatement("Displays the specified text in a small window containing one or more buttons (such as Yes and No).",true));
            members.Add(method);

            #endregion

            #region Trigo Math Functions

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Sin",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the trigonometric sine of Number. Number must be expressed in radians.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Cos",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the trigonometric cosine of Number. Number must be expressed in radians.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Tan",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the trigonometric tangent of Number. Number must be expressed in radians.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "ASin",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the trigonometric arcsine of Number in radians.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "ACos",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the trigonometric arccosine of Number in radians.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "ATan",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the trigonometric arctangent of Number in radians.", true));
            members.Add(method);
            
            #endregion

            #region General Math Functions

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Abs",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the absolute value of Number. The return value is the same type as Number (integer or floating point).", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Ceil",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns Number rounded up to the nearest integer (without any .00 suffix).", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Exp",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "N"));
            method.Comments.Add(new CodeCommentStatement("Returns e (which is approximately 2.71828182845905) raised to the Nth power.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Floor",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns Number rounded down to the nearest integer (without any .00 suffix).", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Log",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the logarithm (base 10) of Number.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Ln",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the natural logarithm (base e) of Number.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Mod",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Modulo.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Round",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "N"));
            method.Comments.Add(new CodeCommentStatement("Returns the rounded number.", true));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Sqrt",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(decimal)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the square root of Number. The result is formatted as floating point. If Number is negative, the function yields a blank result (empty string).", true));
            members.Add(method);

            #endregion

            #region Low Level Methods


            method = new CodeMemberMethodExAHK(true)
            {
                Name = "NumPut",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Number"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "VarOrAddress"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Offset"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Type"));
            method.Comments.Add(new CodeCommentStatement("Stores Number in binary format at the specified address+offset and returns the address to the right of the item just written.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "NumGet",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "VarOrAddress"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Offset"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Type"));
            method.Comments.Add(new CodeCommentStatement("Returns the binary number stored at the specified address+offset.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "OnMessage",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "MsgNumber"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "FunctionName"));
            method.Comments.Add(new CodeCommentStatement("Monitors a message/event. See OnMessage() for details.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "StrGet",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Address"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Lenght"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Encoding"));
            method.Comments.Add(new CodeCommentStatement("Copies a string from a memory address, optionally converting it between code pages.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "StrPut",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "String"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Address"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Lenght"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Encoding"));
            method.Comments.Add(new CodeCommentStatement("Copies a string to a memory address, optionally converting it between code pages.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "VarSetCapacity",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "UnquotedVarName "));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "RequestedCapacity"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "FillByte"));
            method.Comments.Add(new CodeCommentStatement("Enlarges a variable's holding capacity or frees its memory.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);


            method = new CodeMemberMethodExAHK(true)
            {
                Name = "RegisterCallback",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(typeof(string)), "FunctionName"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(typeof(int)), "Options"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(typeof(int)), "ParamCount"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(typeof(int)), "EventInfo"));
            members.Add(method);

            #endregion

            #region Common

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Asc",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "String"));
            method.Comments.Add(new CodeCommentStatement("Returns the ASCII code (a number between 1 and 255) for the first character in String. If String is empty, 0 is returned.", true));
            method.ReturnType = new CodeTypeReference(typeof(int));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Chr",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Number"));
            method.Comments.Add(new CodeCommentStatement("Returns the single character corresponding to the ASCII code indicated by Number.", true));
            method.ReturnType = new CodeTypeReference(typeof(int));
            members.Add(method);

            #endregion

            #region Keys
 
            method = new CodeMemberMethodExAHK(true)
            {
                Name = "GetKeyState",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "KeyName "));
            method.Comments.Add(new CodeCommentStatement("this function returns true (1) if the key is down and false (0) if it is up. ", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            #endregion

            #region String Methods


            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Trim",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "String "));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "OmitChars"));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "InStr",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Haystack"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Needle"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(bool)), "CaseSensitive"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "StartingPos"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Occurrence"));
            method.Comments.Add(new CodeCommentStatement("Searches a string", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "RegExMatch",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Haystack"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "NeedleRegEx"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(bool)), "UnquotedOutputVar"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "StartingPos"));
            method.Comments.Add(new CodeCommentStatement("Regual Expression Match", true));
            method.ReturnType = new CodeTypeReference(typeof(int));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "RegExReplace",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Haystack"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "NeedleRegEx"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(bool)), "Replacement"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "OutputVarCount"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Limit"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "StartingPos"));
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Comments.Add(new CodeCommentStatement("Regual Expression Match", true));
            members.Add(method);



            method = new CodeMemberMethodExAHK(true)
            {
                Name = "SubStr",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "String"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "StartingPos"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Length"));
            method.Comments.Add(new CodeCommentStatement("Searches a string", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);


            method = new CodeMemberMethodExAHK(true)
            {
                Name = "StrLen",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "String"));
            method.Comments.Add(new CodeCommentStatement("Returns the Lenght of the string", true));
            method.ReturnType = new CodeTypeReference(typeof(int));
            members.Add(method);

            #endregion

            #region Window Methods

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "WinActive",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "WinTitle"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "WinText"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "ExcludeTitle"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "ExcludeText"));
            method.Comments.Add(new CodeCommentStatement("If FunctionName does not exist explicitly in the script (by means such as #Include or a non-dynamic call to a library function), Func() returns 0. Otherwise, it returns a reference to the function.", true));
            method.ReturnType = new CodeTypeReference(typeof(object));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "WinExist",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "WinTitle"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "WinText"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "ExcludeTitle"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "ExcludeText"));
            method.Comments.Add(new CodeCommentStatement("If FunctionName does not exist explicitly in the script (by means such as #Include or a non-dynamic call to a library function), Func() returns 0. Otherwise, it returns a reference to the function.", true));
            method.ReturnType = new CodeTypeReference(typeof(object));
            members.Add(method);


            #endregion

            
            method = new CodeMemberMethodExAHK(true)
            {
                Name = "FileExist",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "FilePath"));
            method.Comments.Add(new CodeCommentStatement("Checks if the given filepath exists", true));
            method.ReturnType = new CodeTypeReference(typeof(string));
            members.Add(method);


            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Func",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "FunctionName"));
            method.Comments.Add(new CodeCommentStatement("If FunctionName does not exist explicitly in the script (by means such as #Include or a non-dynamic call to a library function), Func() returns 0. Otherwise, it returns a reference to the function.", true));
            method.ReturnType = new CodeTypeReference(typeof(object));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "DllCall",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "DllFileAndFunction"));

            for(int i = 1; i < 10; i++) {
                method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "Type" + i));
                method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "Arg" + i));
            }
            method.Comments.Add(new CodeCommentStatement("Calls a function inside a DLL, such as a standard Windows API function.", true));
            method.ReturnType = new CodeTypeReference(typeof(int));
            members.Add(method);

            #region Object Methods

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "IsFunc",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "FunctionName"));
            method.Comments.Add(new CodeCommentStatement("If FunctionName does not exist explicitly in the script (by means such as #Include or a non-dynamic call to a library function), IsFunc() returns 0.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "IsLabel",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "LabelName"));
            method.Comments.Add(
                new CodeCommentStatement("Returns a non-zero number if LabelName exists in the script as a subroutine, hotkey, or hotstring (do not include the trailing colon(s) in LabelName).", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "IsObject",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "obj"));
            method.Comments.Add(new CodeCommentStatement("Determites if the given Value is an Object", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            members.Add(method);

            #endregion

            #endregion

            #region Generate Properties

            foreach(var prop in BuildInProperties()) {
                var propr = new CodeMemberPropertyEx(true)
                {
                    Name = prop.Trim()
                };
                members.Add(propr);
            }

            #endregion

            #region Generate build in Commands


            var commands = BuildInCommands().ToList();


            foreach(var prop in BuildInCommands()) {

                var commandStr = prop.Trim();
                if(string.IsNullOrWhiteSpace(commandStr))
                    continue;

                    

                var commandName = SimpleTokinizerIA.ExtractWord(ref commandStr, (prop[0] == '*') ? 1 : 0, SpecailWordCharacters);

                var propr = new CodeMemberMethodExAHK(true)
                {
                    Name = commandName,
                    IsTraditionalCommand = true,
                    IsDefaultMethodInvoke = false,
                    IsFlowCommand = (prop[0] == '*')
                };
                members.Add(propr);
            }

            #endregion Generate build in Commands

            return members;
        }


        public static IEnumerable<PreProcessorDirective> GetDirectives() {
            #region Generate Directives

            foreach(var d in Directives()){

                var commandStr = d.Trim();
                if(string.IsNullOrWhiteSpace(commandStr))
                    continue;

                var commandName = SimpleTokinizerIA.ExtractWord(ref commandStr, 1, SpecailWordCharacters);

                yield return new PreProcessorDirective()
                {
                    Name = commandName
                };

            }

            #endregion
        }

        static string[] BuildInProperties() {
            #region Rawstring
            string propertyrawString =
@"A_Ahkpath
A_Ahkversion
A_Appdata
A_Appdatacommon
A_Autotrim
A_Batchlines
A_Caretx
A_Carety
A_Computername
A_Controldelay
A_Cursor
A_DD
A_DDD
A_DDDD
A_Defaultmousespeed
A_Desktop
A_Desktopcommon
A_Detecthiddentext
A_Detecthiddenwindows
A_Endchar
A_Eventinfo
A_Exitreason
A_Formatfloat
A_Formatinteger
A_Gui
A_Guievent
A_Guicontrol
A_Guicontrolevent
A_Guiheight
A_Guiwidth
A_Guix
A_Guiy
A_Hour
A_Iconfile
A_Iconhidden
A_Iconnumber
A_Icontip
A_Index
A_Ipaddress1
A_Ipaddress2
A_Ipaddress3
A_Ipaddress4
A_Isadmin
A_Iscompiled
A_Issuspended
A_Keydelay
A_Language
A_Lasterror
A_Linefile
A_Linenumber
A_Loopfield
A_Loopfileattrib
A_Loopfiledir
A_Loopfileext
A_Loopfilefullpath
A_Loopfilelongpath
A_Loopfilename
A_Loopfileshortname
A_Loopfileshortpath
A_Loopfilesize
A_Loopfilesizekb
A_Loopfilesizemb
A_Loopfiletimeaccessed
A_Loopfiletimecreated
A_Loopfiletimemodified
A_Loopreadline
A_Loopregkey
A_Loopregname
A_Loopregsubkey
A_Loopregtimemodified
A_Loopregtype
A_Mday
A_Min
A_Mm
A_Mmm
A_Mmmm
A_Mon
A_Mousedelay
A_Msec
A_Mydocuments
A_Now
A_Nowutc
A_Numbatchlines
A_Ostype
A_Osversion
A_Priorhotkey
A_Programfiles
A_Programs
A_Programscommon
A_Screenheight
A_Screenwidth
A_Scriptdir
A_Scriptfullpath
A_Scriptname
A_Sec
A_Space
A_Startmenu
A_Startmenucommon
A_Startup
A_Startupcommon
A_Stringcasesense
A_Tab
A_Temp
A_Thishotkey
A_Thismenu
A_Thismenuitem
A_Thismenuitempos
A_Tickcount
A_Timeidle
A_Timeidlephysical
A_Timesincepriorhotkey
A_Timesincethishotkey
A_Titlematchmode
A_Titlematchmodespeed
A_Username
A_Wday
A_Windelay
A_Windir
A_Workingdir
A_Yday
A_Year
A_Yweek
A_YYYY
Clipboard
Clipboardall
Comspec
Errorlevel
Programfiles
True
False
A_Thisfunc
A_Thislabel
A_Ispaused
A_Iscritical
A_Isunicode
A_Ptrsize";
#endregion
            return propertyrawString.Split('\n');
        }

        static string[] BuildInCommands() {

            #region commands 
            string str =
@"
ImageSearch , OutputVarX, OutputVarY, X1, Y1, X2, Y2, ImageFile
IniDelete , Filename, Section [, Key]
IniRead , OutputVar, Filename [, Section, Key, Default]\n(The Section and Key parameters are only optional on AutoHotkey_L)
IniWrite , Value, Filename, Section [, Key]\n(The Key parameter is only optional on AutoHotkey_L)
Input [, OutputVar, Options, EndKeys, MatchList]
InputBox , OutputVar [, Title, Prompt, HIDE, Width, Height, X, Y, Font, Timeout, Default]
KeyHistory
KeyWait , KeyName [, Options]
ListHotkeys
ListLines
ListVars
Menu , MenuName, Cmd [, P3, P4, P5]
MouseClick , WhichButton [, X, Y, ClickCount, Speed, D|U, R]
MouseClickDrag , WhichButton, X1, Y1, X2, Y2 [, Speed, R]
MouseGetPos [, OutputVarX, OutputVarY, OutputVarWin, OutputVarControl, 1|2|3]
MouseMove , X, Y [, Speed, R]
MsgBox [, Options, Title, Text, Timeout]\nDisplays the specified text in a small window containing one or more buttons  (such as Yes and No).
OnExit [, Label]
OutputDebug , Text
Pause [, On|Off|Toggle, OperateOnUnderlyingThread?]
PixelGetColor , OutputVar, X, Y [, Alt|Slow|RGB]
PixelSearch , OutputVarX, OutputVarY, X1, Y1, X2, Y2, ColorID [, Variation, Fast|RGB]
PostMessage , Msg [, wParam, lParam, Control, WinTitle, WinText, ExcludeTitle, ExcludeText]
Process , Cmd, PID-or-Name [, Param3]
Progress , Param1 [, SubText, MainText, WinTitle, FontName]
Random , OutputVar [, Min, Max]
RegDelete , HKLM|HKU|HKCU|HKCR|HKCC, SubKey [, ValueName]
RegRead , OutputVar, HKLM|HKU|HKCU|HKCR|HKCC, SubKey [, ValueName]
RegWrite , REG_SZ|REG_EXPAND_SZ|REG_MULTI_SZ|REG_DWORD|REG_BINARY, HKLM|HKU|HKCU|HKCR|HKCC, SubKey [, ValueName, Value]
*Reload
Run , Target [, WorkingDir, Max|Min|Hide|UseErrorLevel, OutputVarPID]
RunAs [, User, Password, Domain] 
RunWait , Target [, WorkingDir, Max|Min|Hide|UseErrorLevel, OutputVarPID]
Send , Keys
SendEvent , Keys
SendInput , Keys
SendMessage , Msg [, wParam, lParam, Control, WinTitle, WinText, ExcludeTitle, ExcludeText]
SendMode , Event|Play|Input|InputThenPlay
SendPlay , Keys
SendRaw , Keys
SetBatchLines , -1 | 20ms | LineCount
SetCapsLockState , On|Off|AlwaysOn|AlwaysOff
SetControlDelay , Delay
SetDefaultMouseSpeed , Speed
SetEnv , Var, Value
SetFormat , float|integer, TotalWidth.DecimalPlaces|hex|d
SetKeyDelay [, Delay, PressDuration]
SetMouseDelay , Delay
SetNumLockState , On|Off|AlwaysOn|AlwaysOff
SetScrollLockState , On|Off|AlwaysOn|AlwaysOff
SetStoreCapslockMode , On|Off
SetTimer , Label [, Period|On|Off]
SetTitleMatchMode , Fast|Slow|RegEx|1|2|3
SetWinDelay , Delay
SetWorkingDir , DirName
Shutdown , Code
Sleep , Delay
Sort , VarName [, Options]
SoundBeep [, Frequency, Duration]
SoundGet , OutputVar [, ComponentType, ControlType, DeviceNumber]
SoundGetWaveVolume , OutputVar [, DeviceNumber]
SoundPlay , Filename [, wait]
SoundSet , NewSetting [, ComponentType, ControlType, DeviceNumber]
SoundSetWaveVolume , Percent [, DeviceNumber]
SplashImage [, ImageFile, Options, SubText, MainText, WinTitle, FontName]
SplashTextOff
SplashTextOn [, Width, Height, Title, Text]
SplitPath , InputVar [, OutFileName, OutDir, OutExtension, OutNameNoExt, OutDrive]
StatusBarGetText , OutputVar [, Part#, WinTitle, WinText, ExcludeTitle, ExcludeText]
StatusBarWait [, BarText, Seconds, Part#, WinTitle, WinText, Interval, ExcludeTitle, ExcludeText]
StringCaseSense , On|Off|Locale
StringGetPos , OutputVar, InputVar, SearchText [, L#|R#, Offset]
StringLeft , OutputVar, InputVar, Count
StringLen , OutputVar, InputVar
StringLower , OutputVar, InputVar [, T]
StringMid , OutputVar, InputVar, StartChar [, Count, L]
StringReplace , OutputVar, InputVar, SearchText [, ReplaceText, All]
StringRight , OutputVar, InputVar, Count
StringSplit , OutputArray, InputVar [, Delimiters, OmitChars]
StringTrimLeft , OutputVar, InputVar, Count
StringTrimRight , OutputVar, InputVar, Count
StringUpper , OutputVar, InputVar [, T]
Suspend [, On|Off|Toggle|Permit]
SysGet , OutputVar, Sub-command [, Param3]
Thread , Setting, P2 [, P3]
ToolTip [, Text, X, Y, WhichToolTip]
Transform , OutputVar, Cmd, Value1 [, Value2]
TrayTip [, Title, Text, Seconds, Options]
URLDownloadToFile , URL, Filename
WinActivate [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinActivateBottom [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinClose [, WinTitle, WinText, SecondsToWait, ExcludeTitle, ExcludeText]
WinGet , OutputVar [, Cmd, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinGetActiveStats , Title, Width, Height, X, Y
WinGetActiveTitle , OutputVar
WinGetClass , OutputVar [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinGetPos [, X, Y, Width, Height, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinGetText , OutputVar [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinGetTitle , OutputVar [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinHide [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinKill [, WinTitle, WinText, SecondsToWait, ExcludeTitle, ExcludeText]
WinMaximize [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinMenuSelectItem , WinTitle, WinText, Menu [, SubMenu1, SubMenu2, SubMenu3, SubMenu4, SubMenu5, SubMenu6, ExcludeTitle, ExcludeText]
WinMinimize [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinMinimizeAll
WinMinimizeAllUndo
WinMove , WinTitle, WinText, X, Y [, Width, Height, ExcludeTitle, ExcludeText]
WinRestore [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinSet , AlwaysOnTop|Trans, On|Off|Toggle|Value(0-255) [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinSetTitle , WinTitle, WinText, NewTitle [, ExcludeTitle, ExcludeText]
WinShow [, WinTitle, WinText, ExcludeTitle, ExcludeText]
WinWait , WinTitle, WinText, Seconds [, ExcludeTitle, ExcludeText]
WinWaitActive [, WinTitle, WinText, Seconds, ExcludeTitle, ExcludeText]
WinWaitClose , WinTitle, WinText, Seconds [, ExcludeTitle, ExcludeText]
WinWaitNotActive [, WinTitle, WinText, Seconds, ExcludeTitle, ExcludeText]
AutoTrim , On|Off
BlockInput , On|Off|Send|Mouse|SendAndMouse|Default|MouseMove|MouseMoveOff
Click
ClipWait [, SecondsToWait, 1]
Control , Cmd [, Value, Control, WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlClick [, Control-or-Pos, WinTitle, WinText, WhichButton, ClickCount, Options, ExcludeTitle, ExcludeText]
ControlFocus [, Control, WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlGet , OutputVar, Cmd [, Value, Control, WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlGetFocus , OutputVar [WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlGetPos [, X, Y, Width, Height, Control, WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlGetText , OutputVar [, Control, WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlMove , Control, X, Y, Width, Height [, WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlSend [, Control, Keys, WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlSendRaw [, Control, Keys, WinTitle, WinText, ExcludeTitle, ExcludeText]
ControlSetText , Control, NewText [, WinTitle, WinText, ExcludeTitle, ExcludeText]
CoordMode , ToolTip|Pixel|Mouse [, Screen|Relative]
Critical [, Off]
DetectHiddenText , On|Off
DetectHiddenWindows , On|Off
Drive , Sub-command [, Drive , Value]
DriveGet , OutputVar, Cmd [, Value]
DriveSpaceFree , OutputVar, C:\\
Edit
EnvAdd , Var, Value [, TimeUnits]
EnvDiv , Var, Value
EnvGet , OutputVar, EnvVarName
EnvMult , Var, Value
EnvSet , EnvVar, Value
EnvSub , Var, Value [, TimeUnits]
EnvUpdate
*Exit [, ExitCode]
*ExitApp [, ExitCode]
FileAppend [, Text, Filename, Encoding] \n(The encoding parameter is AutoHotkeyU-exclusive)
FileCopy , Source, Dest [, Flag (1 = overwrite)]
FileCopyDir , Source, Dest [, Flag]
FileCreateDir , Path
FileCreateShortcut , Target, C:\\My Shortcut.lnk [, WorkingDir, Args, Description, IconFile, ShortcutKey, IconNumber, RunState]
FileDelete , FilePattern
FileGetAttrib , OutputVar(RASHNDOCT) [, Filename]
FileGetShortcut , LinkFile [, OutTarget, OutDir, OutArgs, OutDescription, OutIcon, OutIconNum, OutRunState]
FileGetSize , OutputVar [, Filename, Units]
FileGetTime , OutputVar [, Filename, WhichTime (M, C, or A -- default is M)]
FileGetVersion , OutputVar [, Filename]
FileInstall , Source, Dest [, Flag (1 = overwrite)]
FileMove , Source, Dest [, Flag (1 = overwrite)]
FileMoveDir , Source, Dest [, Flag (2 = overwrite)]
FileRead , OutputVar, Filename
FileReadLine , OutputVar, Filename, LineNum
FileRecycle , FilePattern
FileRecycleEmpty [, C:\\]
FileRemoveDir , Path [, Recurse? (1 = yes)]
FileSelectFile , OutputVar [, Options, RootDir[\\DefaultFilename], Prompt, Filter]
FileSelectFolder , OutputVar [, *StartingFolder, Options, Prompt]
FileSetAttrib , Attributes(+-^RASHNOT) [, FilePattern, OperateOnFolders?, Recurse?]
FileSetTime [, YYYYMMDDHH24MISS, FilePattern, WhichTime (M|C|A), OperateOnFolders?, Recurse?]
FormatTime , OutputVar [, YYYYMMDDHH24MISS, Format]
GetKeyState , OutputVar, WhichKey [, Mode (P|T)]
*gosub , Label\nJumps to the specified label and continues execution until Return is encountered.
*goto , Label\nJumps to the specified label and continues execution.
GroupActivate , GroupName [, R]
GroupAdd , GroupName, WinTitle [, WinText, Label, ExcludeTitle, ExcludeText]
GroupClose , GroupName [, A|R]
GroupDeactivate , GroupName [, R]
Gui , sub-command [, Param2, Param3, Param4]
GuiControl , Sub-command, ControlID [, Param3]
GuiControlGet , OutputVar [, Sub-command, ControlID, Param4]
Hotkey , KeyName [, Label, Options]

IfEqual , var, value
IfExist , File|Dir|Pattern
IfGreater , var, value
IfGreaterOrEqual , var, value
IfInString , Var, SearchString
IfLess , var, value
IfLessOrEqual , var, value
IfMsgBox , Yes|No|OK|Cancel|Abort|Ignore|Retry|Timeout
IfNotEqual , var, value
IfNotExist , File|Dir|Pattern
IfNotInString , Var, SearchString
IfWinActive [, WinTitle, WinText, ExcludeTitle, ExcludeText]
IfWinExist [, WinTitle, WinText, ExcludeTitle, ExcludeText]
IfWinNotActive [, WinTitle, WinText, ExcludeTitle, ExcludeText]
IfWinNotExist [, WinTitle, WinText, ExcludeTitle, ExcludeText]
";
            #endregion
            return str.Split('\n');
        }


        static string[] Directives() {

            #region directives
            string directives =
@"
#Include
#AllowSameLineComments \n(Only for .aut scripts) Allows comments on the same line.
#ClipboardTimeout milliseconds\nChanges how long the script keeps trying to access the clipboard when the first attempt fails.
#CommentFlag NewString
#ErrorStdOut
#EscapeChar NewChar
#HotkeyInterval Value
#HotkeyModifierTimeout milliseconds
#Hotstring NewOptions
#If [expression] \n[AutoHotkey_L] Makes subsequent hotkeys and hotstrings only function when the specified expression is true.
#IfTimeout timeout \n[AutoHotkey_L] Sets the maximum time that may be spent evaluating a single #If expression.
#IfWinActive [, WinTitle, WinText] \nMakes subsequent hotkeys and hotstrings only function when the specified window is active.
#IfWinExist [, WinTitle, WinText] \nMakes subsequent hotkeys and hotstrings only function when the specified window exists.
#IfWinNotActive [, WinTitle, WinText] \nMakes subsequent hotkeys and hotstrings only function when the specified window is not active.
#IfWinNotExist [, WinTitle, WinText] \nMakes subsequent hotkeys and hotstrings only function when the specified window doesn't exist.
#Include FileName \nCauses the script to behave as though the specified file's contents are present at this exact position.
#IncludeAgain FileName \nCauses the script to behave as though the specified file's contents are present at this exact position.
#InstallKeybdHook
#InstallMouseHook
#KeyHistory MaxEvents
#LTrim On|Off
#MaxHotkeysPerInterval Value
#MaxMem ValueInMegabytes
#MaxThreads Value
#MaxThreadsBuffer On|Off
#MaxThreadsPerHotkey Value
#MenuMaskKey keyname \n[AutoHotkey_L] Changes which key is used to mask Win or Alt keyup events.
#NoEnv
#NoTrayIcon
#Persistent
#SingleInstance [force|ignore|off]
#UseHook [On|Off]
#Warn [WarningType, WarningMode] \n[AutoHotkey_L] Enables or disables warnings for selected load-time or run-time conditions that may be indicative of developer errors.
#WinActivateForce
";
            #endregion
            return directives.Split('\n');
        }

    }
}
