using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM.Types;
using System.CodeDom;
using SmartDevelop.AHK.AHKv1.DOM.Types;

namespace SmartDevelop.AHK.AHKv1
{
    /// <summary>
    /// This is just for debuging Purposes
    /// </summary>
    public static  class CodeLanguageAHKBuildinMethods
    {
        public static IEnumerable<CodeTypeMember> ReadMembers() {

            var members = new List<CodeTypeMember>();

            #region Read the build in Members

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
                Name = "Trim",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "String "));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "OmitChars"));
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

            return members;
        }
    }
}
