using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using System.CodeDom;
using SmartDevelop.Model.Tokening;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.DOM.Types;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.Model.DOM.Ranges;

namespace SmartDevelop.Model.DOM
{

    // ToDO Move this out of here :D

    /// <summary>
    ///  CodeDOM Service Impl. for IA
    /// </summary>
    public class CodeDOMServiceIA : CodeDOMService
    {
        Archimedes.CodeDOM.CodeDOMTraveler _codeDOMTraveler = new Archimedes.CodeDOM.CodeDOMTraveler();

        CodeMemberMethodEx _autoexec;

        public CodeDOMServiceIA(SmartCodeProject project)
            : base(project) {

            RootType.Members.Add(_autoexec = new CodeMemberMethodEx() 
            {
                Name = "AutoExec",
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                DefiningType = RootType,
                ReturnType = new CodeTypeReference(typeof(void)),
                LinePragma = new CodeLinePragma("all", 0),
                IsHidden = true
            });
        }

        public override void CompileTokenFile(Projecting.ProjectItemCode codeitem, CodeTypeDeclarationEx initialparent) {

            #region Clean Up

            //remove all old members which are from this code file:

            var oldMembers = (from CodeTypeMember m in RootType.Members
                              where (m.LinePragma == null || m.LinePragma.FileName == codeitem.FilePath)
                              let meth = m as CodeMemberMethodEx
                              where meth == null || !meth.IsHidden
                              select m).ToList();
            foreach(var m in oldMembers)
                RootType.Members.Remove(m);

            if(CodeRanges.ContainsKey(codeitem))
                CodeRanges[codeitem].Clear();
            else
                CodeRanges.Add(codeitem, new CodeRangeManager());

            var currentRanges = CodeRanges[codeitem];

            #endregion

            var codeLineMap = codeitem.SegmentService.GetCodeSegmentLinesMap();
            CodeTypeDeclaration parent = initialparent;
            Stack<CodeSegment> paramstack = new Stack<CodeSegment>();           
            int linecnt = codeitem.Document.LineCount;
            CodeTokenLine line;

            Stack<CodeTypeDeclarationEx> parentHirarchy = new Stack<CodeTypeDeclarationEx>();
            int bcc = 0;
            parentHirarchy.Push(initialparent);
            

            for(int i = 0; i < linecnt; i++) {

                if(codeLineMap.ContainsKey(i))
                    line = codeLineMap[i];
                else
                    continue;

                // is class definition?:

                #region Parse Class Definition

                var classkeywordSegment = line.CodeSegments[0].ThisOrNextOmit(whitespacetoken);
                if(classkeywordSegment != null && classkeywordSegment.Token == Token.KeyWord && classkeywordSegment.TokenString.Equals("class", StringComparison.CurrentCultureIgnoreCase)) {
                    var classNameSegment = classkeywordSegment.FindNextOnSameLine(Token.Identifier);
                    if(classNameSegment != null) {

                        var classBodyStart = classNameSegment.NextOmit(whitespacetoken);
                        if(classBodyStart != null) {
                            if(classBodyStart.Token != Token.BlockOpen) {
                                // unexpected token
                                // block open was expected!!
                            } else {

                                var type = new CodeTypeDeclarationEx(classNameSegment.TokenString)
                                {
                                    IsClass = true
                                };
                                classNameSegment.CodeDOMObject = type;

                                // Add it to the CodeDOM Tree
                                CodeTypeDeclarationEx thisparent = parentHirarchy.Any() ? parentHirarchy.Peek() : RootType;
                                thisparent.Members.Add(type);
                                type.Parent = thisparent;

                                // Create a CodeRange Item
                                int startOffset = classBodyStart.Range.Offset;
                                var classBodyEnd = classBodyStart.FindClosingBracked(true);
                                if(classBodyEnd != null) {
                                    int length = (classBodyEnd.Range.Offset - startOffset);
                                    currentRanges.Add(new CodeRange(new SimpleSegment(startOffset, length), type));
                                }


                                parentHirarchy.Push(type);
                                bcc++;

                                i = classBodyStart.LineNumber; // jumt to:  class Foo { * <---|
                                continue;
                            }
                        }
                    }
                }
                
                #endregion

                // is method definition?:

                #region Analyze for Method Definition

                var methodSegment = line.CodeSegments[0].ThisOrNextOmit(whitespacetoken);
                if(methodSegment != null && methodSegment.Token == Token.Identifier) {
                    var methodSignatureStart = methodSegment.Next;
                    if(methodSignatureStart != null && methodSignatureStart.Token == Token.LiteralBracketOpen) {
                        var methodSignatureEnd = methodSignatureStart.FindClosingBracked(false);
                        if(methodSignatureEnd != null) {
                            var startMethodBody = methodSignatureEnd.NextOmit(whitespacetoken);
                            if(startMethodBody != null && startMethodBody.Token == Token.BlockOpen) {
                                // jup we have a method definition here.
                                // Method body starts at startMethodBody
                                // Method body ends at
                                var endMethodBody = startMethodBody.FindClosingBracked(true);
                                if(endMethodBody != null) {

                                    #region Generate Method Definition DOM

                                    var method = new CodeMemberMethodEx()
                                    {
                                        Name = methodSegment.TokenString,
                                        LinePragma = new CodeLinePragma(codeitem.FilePath, methodSegment.LineNumber),
                                        ReturnType = new CodeTypeReference(typeof(object))
                                    };
                                    methodSegment.CodeDOMObject = method;


                                    // extract Method Comment
                                    var comment = methodSegment.PreviousOmit(whitespacetoken);
                                    if(comment != null && comment.Token == Token.MultiLineComment) {
                                        method.Comments.Add(new CodeCommentStatement(comment.TokenString, true));
                                    } else if(comment != null && comment.Token == Token.SingleLineComment) {

                                        //todo: collect all above singleline comments
                                    }

                                    // extract method params
                                    paramstack.Clear();
                                    CodeSegment previous = methodSignatureStart;

                                    // get method properties:
                                    while(true){
                                        var current = previous.Next;
                                        if(current.Token == Token.Identifier) {
                                            paramstack.Push(current);
                                        } else if(current.Token == Token.ParameterDelemiter || current.Token == Token.LiteralBracketClosed) {
                                            // end of param reached:
                                            if(paramstack.Count == 1) {
                                                // thread one param as the untyped argument, type of Object
                                                method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), paramstack.Pop().TokenString));
                                            } else if(paramstack.Count > 1) {
                                                // thread two param as the type and argument
                                                method.Parameters.Add(new CodeParameterDeclarationExpression(paramstack.Pop().TokenString, paramstack.Pop().TokenString));
                                            }
                                            if(current.Token == Token.LiteralBracketClosed)
                                                break;
                                        }
                                        previous = current;
                                    }

                                    #endregion

                                    // get method statements
                                    method.Statements.AddRange(
                                        CollectAllCodeStatements(codeLineMap, startMethodBody.LineNumber + 1, endMethodBody.LineNumber));

                                    // add it to the code DOM Tree
                                    CodeTypeDeclarationEx thisparent = parentHirarchy.Any() ? parentHirarchy.Peek() : RootType;
                                    thisparent.Members.Add(method);
                                    method.DefiningType = thisparent;


                                    // Create a CodeRange Item
                                    int startOffset = startMethodBody.Range.Offset;
                                    int length = (endMethodBody.Range.Offset - startOffset);
                                    currentRanges.Add(new CodeRange(new SimpleSegment(startOffset, length), method));

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

                        if(segment.Token == Token.BlockOpen) {
                            bcc++;
                        } else if(segment.Token == Token.BlockClosed) {
                            bcc--;
                            if(parentHirarchy.Count - 2 == bcc) {
                                parentHirarchy.Pop();
                            }
                        }
                        _autoexec.Statements.AddRange(
                                        CollectAllCodeStatements(codeLineMap, i, i));
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

            for(int i = startLine; i <= endLine; i++) {
                if(segments.ContainsKey(i))
                    line = segments[i];
                else
                    continue;

                CodeSegment toParse = line.CodeSegments.First();
                CodeSegment next;
                while(toParse != null) {
                    var ex = ParseExpression(toParse, out next);
                    codeStatements.Add(ex);
                    toParse = next;
                }
               
            }
            return codeStatements;
        }

        static readonly List<Token> LocalExpressionEndTokens = new List<Token>() { Token.NewLine, Token.ParameterDelemiter };


        CodeExpression ParseExpression(CodeSegment tokenSegment, out CodeSegment nextToParse) {

            nextToParse = null;

            //simply parse for Method Invokes
            var nextidentifier = tokenSegment.FindThisOrNext(Token.Identifier, LocalExpressionEndTokens);

            if(nextidentifier != null) {
                if(nextidentifier.Next != null
                    && nextidentifier.Next.Token == Token.LiteralBracketOpen) {
                    var invokeExpression = new CodeMethodInvokeExpression();


                    // var method = _codeDOMTraveler.FindBestMethod(nextidentifier.TokenString, null, RootType); //<-- todo emit correct type

                    var methodRef = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), nextidentifier.TokenString);

                    invokeExpression.Method = methodRef;
                    nextidentifier.CodeDOMObject = methodRef;

                    var paramOrClose = nextidentifier.Next.Next;
                    if(paramOrClose != null) {
                        if(paramOrClose.Token == Token.LiteralBracketClosed) {
                            // method(void)
                        } else {
                            // method with one or more params
                        }
                    } else {
                        // missing closing bracket...
                    }

                    nextToParse = nextidentifier.Next.Next;
                    if(!(nextToParse != null && nextToParse.LineNumber == tokenSegment.LineNumber)) {
                        nextToParse = null;
                    }
                    return invokeExpression;
                } else {
                    if(nextidentifier.Next != null && !LocalExpressionEndTokens.Contains(nextidentifier.Next.Token)) {
                        return ParseExpression(nextidentifier.Next, out nextToParse);
                    }
                }
            }
            return null;
        }


    }
}
