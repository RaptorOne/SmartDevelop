﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace SmartDevelop.CodeDOM.Tests
{
    class Program
    {
        static void Main(string[] args) {

            var hellowroldUnit = BuildHelloWorldGraph();


            CodeDomProvider provider = new CSharpCodeProvider();

            var options = new CodeGeneratorOptions()
            {
                BlankLinesBetweenMembers = false,
            };

            provider.GenerateCodeFromCompileUnit(hellowroldUnit, Console.Out, options);

            Console.WriteLine(); Console.WriteLine();

            Console.WriteLine("Press any Key to Quit");
            Console.ReadKey();
        }


        // Build a Hello World program graph using 
        // System.CodeDom types.
        public static CodeCompileUnit BuildHelloWorldGraph() {


            #region Something

            // Create a new CodeCompileUnit to contain 
            // the program graph.
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // Declare a new namespace called Samples.
            CodeNamespace samples = new CodeNamespace("Samples");
            // Add the new namespace to the compile unit.
            compileUnit.Namespaces.Add(samples);

            // Add the new namespace import for the System namespace.
            samples.Imports.Add(new CodeNamespaceImport("System"));

            // Declare a new type called Class1.
            CodeTypeDeclaration class1 = new CodeTypeDeclaration("Class1");
            // Add the new type to the namespace type collection.
            samples.Types.Add(class1);

            // Declare a new code entry point method.
            CodeEntryPointMethod start = new CodeEntryPointMethod();

            // Create a type reference for the System.Console class.
            CodeTypeReferenceExpression csSystemConsoleType = new CodeTypeReferenceExpression("System.Console");

            // Build a Console.WriteLine statement.
            CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
                csSystemConsoleType, "WriteLine",
                new CodePrimitiveExpression("Hello World!"));

            // Add the WriteLine call to the statement collection.
            start.Statements.Add(cs1);

            // Build another Console.WriteLine statement.
            CodeMethodInvokeExpression cs2 = new CodeMethodInvokeExpression(
                csSystemConsoleType, "WriteLine",
                new CodePrimitiveExpression("Press the Enter key to continue."));

            // Add the WriteLine call to the statement collection.
            start.Statements.Add(cs2);

            #endregion

            CodeExpression mathExpression = new CodeBinaryOperatorExpression(new CodePrimitiveExpression(1),
                CodeBinaryOperatorType.Add,
                new CodeBinaryOperatorExpression(new CodePrimitiveExpression(6), CodeBinaryOperatorType.BooleanAnd, new CodePrimitiveExpression(5))
                );


            CodeExpression mathAsignExpression = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("test"), CodeBinaryOperatorType.Assign
                , mathExpression);


            start.Statements.Add(mathAsignExpression);





            // Build a call to System.Console.ReadLine.
            CodeMethodInvokeExpression csReadLine = new CodeMethodInvokeExpression(
                csSystemConsoleType, "ReadLine");

            // Add the ReadLine statement.
            start.Statements.Add(csReadLine);

            // Add the code entry point method to
            // the Members collection of the type.
            class1.Members.Add(start);

            return compileUnit;
        }




    }
}
