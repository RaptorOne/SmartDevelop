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

        // public IEnumerable<CodeMemberMethod> FindAllMethodsFrom()


        //public CodeObject QueryCodeObjectAt(string filepath, int line, int col) {



        //}

        #endregion


        //IEnumerable<CodeMemberMethod> CollectAllMembersBy(string filepath) {

        //    //List<CodeMemberMethod>

        //    //var oldMembers = (from CodeTypeMember m in _scriptRoot.Members
        //    //                  where m.LinePragma.FileName == filepath
        //    //                  select m).ToList();



        //    // todo look up class methods


        //}

        #region File Compiler

        public void CompileTokenFile(ProjectItemCode codeitem, CodeTypeDeclaration initialparent) {

            //remove all old members which are from this code file:
            var oldMembers = (from CodeTypeMember m in _scriptRoot.Members
                             where m.LinePragma.FileName == codeitem.FilePath
                             select m).ToList();
            foreach(var m in oldMembers)
                _scriptRoot.Members.Remove(m);


            var segments = codeitem.TokenService.GetCodeSegmentLinesMap();
            CodeTypeDeclaration parent = initialparent;
            Stack<CodeSegment> paramstack = new Stack<CodeSegment>();           
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
                                if(endMethodBody != null) {

                                    var method = new CodeMemberMethod()
                                    {
                                        Name = methodName.TokenString,
                                        LinePragma = new CodeLinePragma(codeitem.FilePath, methodName.Line),
                                        ReturnType = new CodeTypeReference(typeof(object))
                                    };


                                    // extract Method Comment
                                    var comment = methodName.PreviousOmit(whitespacetoken);
                                    if(comment != null && comment.Type == Token.MultiLineComment) {
                                        method.Comments.Add(new CodeCommentStatement(comment.TokenString, true));
                                    } else if(comment != null && comment.Type == Token.SingleLineComment) {

                                        //todo: collect all above singleline comments
                                    }

                                    // extract method params
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
                                    // move the scanpointer to the method end:
                                    i = endMethodBody.Line;
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
