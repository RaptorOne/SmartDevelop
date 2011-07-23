using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using System.CodeDom;
using SmartDevelop.Model.Tokening;
using SmartDevelop.Model.Projecting;

namespace SmartDevelop.Model.DOM
{

    // ToDO Move this out of here :D

    /// <summary>
    ///  CodeDOM Service Impl. for IA
    /// </summary>
    public class CodeDOMServiceIA : CodeDOMService
    {
        Archimedes.CodeDOM.CodeDOMTraveler _codeDOMTraveler = new Archimedes.CodeDOM.CodeDOMTraveler();


        public CodeDOMServiceIA(SmartCodeProject project)
            : base(project) {
        }

        public override void CompileTokenFile(Projecting.ProjectItemCode codeitem, System.CodeDom.CodeTypeDeclaration initialparent) {
            //remove all old members which are from this code file:
            var oldMembers = (from CodeTypeMember m in RootType.Members
                             where m.LinePragma == null || m.LinePragma.FileName == codeitem.FilePath
                             select m).ToList();
            foreach(var m in oldMembers)
                RootType.Members.Remove(m);


            var codeLineMap = codeitem.SegmentService.GetCodeSegmentLinesMap();
            CodeTypeDeclaration parent = initialparent;
            Stack<CodeSegment> paramstack = new Stack<CodeSegment>();           
            int linecnt = codeitem.Document.LineCount;
            CodeTokenLine line;

            Stack<CodeTypeDeclaration> parentHirarchy = new Stack<CodeTypeDeclaration>();
            int bcc = 0;
            parentHirarchy.Push(initialparent);
            

            for(int i = 0; i < linecnt; i++) {

                if(codeLineMap.ContainsKey(i))
                    line = codeLineMap[i];
                else
                    continue;

                // is class definition?:

                var classkeywordSegment = line.CodeSegments[0].ThisOrNextOmit(whitespacetoken);
                if(classkeywordSegment != null && classkeywordSegment.Type == Token.KeyWord && classkeywordSegment.TokenString.Equals("class", StringComparison.CurrentCultureIgnoreCase)) {
                    var classNameSegment = classkeywordSegment.FindNextOnSameLine(Token.Identifier);
                    if(classNameSegment != null) {

                        var classBodyStart = classNameSegment.NextOmit(whitespacetoken);
                        if(classBodyStart != null) {
                            if(classBodyStart.Type != Token.BlockOpen) {
                                // unexpected token
                                // block open was expected!!
                            } else {
                                //classBodyStart = classBodyStart.Next;
                                //var classBodyEnd = classBodyStart.FindClosingBracked(true);

                                var type = new CodeTypeDeclaration(classNameSegment.TokenString)
                                {
                                    IsClass = true
                                };
                                classNameSegment.CodeDOMObject = type;
                                
                                if(parentHirarchy.Any()) {
                                    parentHirarchy.Peek().Members.Add(type);
                                } else {
                                    RootType.Members.Add(type);
                                }

                                parentHirarchy.Push(type);
                                bcc++;

                                i = classBodyStart.LineNumber; // jumt to:  class Foo { * <---|
                                continue;
                            }
                        }
                    }
                }

                // is method definition?:

                #region Analyze for Method Definition

                var methodSegment = line.CodeSegments[0].ThisOrNextOmit(whitespacetoken);
                if(methodSegment != null && methodSegment.Type == Token.Identifier) {
                    var methodSignatureStart = methodSegment.Next;
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

                                    #region Generate Method Definition DOM

                                    var method = new CodeMemberMethod()
                                    {
                                        Name = methodSegment.TokenString,
                                        LinePragma = new CodeLinePragma(codeitem.FilePath, methodSegment.LineNumber),
                                        ReturnType = new CodeTypeReference(typeof(object))
                                    };
                                    methodSegment.CodeDOMObject = method;


                                    // extract Method Comment
                                    var comment = methodSegment.PreviousOmit(whitespacetoken);
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
                                        if(current.Type == Token.Identifier) {
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

                                    #endregion

                                    // get method statements
                                    method.Statements.AddRange(
                                        CollectAllCodeStatements(codeLineMap, startMethodBody.LineNumber + 1, endMethodBody.LineNumber));

                                    if(parentHirarchy.Any()){
                                        parentHirarchy.Peek().Members.Add(method);
                                    }else{
                                        RootType.Members.Add(method);
                                    }
                                    // move the scanpointer to the method end:
                                    i = endMethodBody.LineNumber;
                                    continue;
                                }
                            }
                        }
                    }
                }

                #endregion

                if(codeLineMap.ContainsKey(i)) {
                    var lineBlock = codeLineMap[i];
                    foreach(var segment in lineBlock.CodeSegments) {
                        if(segment.Type == Token.BlockOpen) {
                            bcc++;
                        } else if(segment.Type == Token.BlockClosed) {
                            bcc--;
                            if(parentHirarchy.Count - 2 == bcc) {
                                parentHirarchy.Pop();
                            }
                        }
                    }
                } else
                    continue;

            }
        }


        /// <summary>
        /// Get all CodeStatements in the given Linerange
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <returns></returns>
        public CodeStatementCollection CollectAllCodeStatements(Dictionary<int, CodeTokenLine> segments, int startLine, int endLine) {
            CodeTokenLine line;
            var codeStatements = new CodeStatementCollection();

            for(int i = startLine; i < endLine; i++) {
                if(segments.ContainsKey(i))
                    line = segments[i];
                else
                    continue;

                var ex = ParseExpression(line.CodeSegments.First());
                codeStatements.Add(ex);
            }
            return codeStatements;
        }

        static readonly List<Token> LocalExpressionEndTokens = new List<Token>() { Token.NewLine, Token.ParameterDelemiter };

        CodeExpression ParseExpression(CodeSegment tokenSegment) {
            //simply parse for Method Invokes
            var nextidentifier = tokenSegment.FindNext(Token.Identifier, LocalExpressionEndTokens);

            if(nextidentifier != null && nextidentifier.Next != null 
                && nextidentifier.Next.Type == Token.LiteralBracketOpen) {
                var invokeExpression = new CodeMethodInvokeExpression();


                // var method = _codeDOMTraveler.FindBestMethod(nextidentifier.TokenString, null, RootType); //<-- todo emit correct type

                var methodRef = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), nextidentifier.TokenString);
                
                invokeExpression.Method = methodRef;
                nextidentifier.CodeDOMObject = methodRef;

                var paramOrClose = nextidentifier.Next.Next;
                if(paramOrClose != null) {
                    if(paramOrClose.Type == Token.LiteralBracketClosed) {
                        // method(void)
                    } else {
                        // method with one or more params
                    }
                } else {
                    // missing closing bracket...
                }

                return invokeExpression;
            }
            return null;
        }


    }
}
