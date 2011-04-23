using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Tokening;
using System.CodeDom;

namespace SmartDevelop.Model.DOM
{
    /// <summary>
    /// Service to compile tokenized RAW Data into CodeDOM Representation
    /// </summary>
    public class CodeDOMService
    {
        #region Fields

        SmartCodeProject _project;
        CodeTypeDeclaration _scriptRoot;
        static List<Token> whitespacetoken = new List<Token> { Token.WhiteSpace, Token.NewLine };

        #endregion

        #region Constructor

        public CodeDOMService(SmartCodeProject project) {
            _scriptRoot = new CodeTypeDeclaration("Global");
            _project = project;
        }

        #endregion

        #region Properties

        public CodeTypeDeclaration RootType {
            get { return _scriptRoot; }
        }

        #endregion

        #region Methods



        #endregion

        #region File Compiler

        public void CompileFile(ProjectItemCode codeitem, CodeTypeDeclaration initialparent) {
            var segments = codeitem.TokenService.GetCodeSegmentLinesMap();
            CodeTypeDeclaration parent = initialparent;
            Stack<CodeSegment> paramstack = new Stack<CodeSegment>();

            List<int> lines = new List<int>(segments.Keys.Count);
            foreach(var k in segments.Keys) {
                lines.Add(k);
            }
            
            int linecnt = codeitem.Document.LineCount;
            CodeTokenLine line;

            for(int i = 0; i < linecnt; i++) {

                if(segments.ContainsKey(i))
                    line = segments[i];
                else
                    continue;

                // is class definition?:


                // is method definition?:
                var methodName = line.CodeSegments[0].ThisOrNextOmit(whitespacetoken);
                if(methodName != null && methodName.Type == Token.Unknown) {
                    var methodSignatureStart = methodName.Next;
                    if(methodSignatureStart != null && methodSignatureStart.Type == Token.LiteralBracketOpen) {
                        var methodSignatureEnd = methodSignatureStart.FindClosingBracked(false);
                        if(methodSignatureEnd != null) {
                            var startMethodBody = methodSignatureEnd.NextOmit(whitespacetoken);
                            if(startMethodBody != null && startMethodBody.Type == Token.BlockOpen) {
                                // jup we have a method definition here.
                                // Method body starts at startMethodBody
                                // Method body ends at
                                var endMethodBody = startMethodBody.FindClosingBracked(true);
                                // move the scanpointer to the method end:
                                if(endMethodBody != null) {

                                    var method = new CodeMemberMethod()
                                    {
                                        Name = methodName.TokenString,
                                        LinePragma = new CodeLinePragma(codeitem.FilePath, methodName.Line),
                                        ReturnType = new CodeTypeReference(typeof(object))
                                    };

                                    paramstack.Clear();
                                    CodeSegment previous = methodSignatureStart;
                                    // get method properties:
                                    while(true){
                                        var current = previous.Next;
                                        if(current.Type == Token.Unknown) {
                                            paramstack.Push(current);
                                        } else if(current.Type == Token.ParameterDelemiter || current.Type == Token.LiteralBracketClosed) {
                                            // end of param reached:
                                            if(paramstack.Count == 1) {
                                                // thread one param as the untyped argument, type of Object
                                                method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), paramstack.Pop().TokenString));
                                            } else if(paramstack.Count > 1) {
                                                // thread two param as the type and argument
                                                method.Parameters.Add(new CodeParameterDeclarationExpression(paramstack.Pop().TokenString, paramstack.Pop().TokenString));
                                            }
                                            if(current.Type == Token.LiteralBracketClosed)
                                                break;
                                        }
                                        previous = current;
                                    }
                                    parent.Members.Add(method);
                                    continue;
                                }

                            }
                        }
                    }
                }
            }
        }
        #endregion
    }

        
}
