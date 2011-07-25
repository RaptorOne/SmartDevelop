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
using SmartDevelop.AHK.AHKv1.DOM.Types;

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
        //CodeTypeDeclarationEx _superBase;
        CodeTypeReferenceEx _superBase;

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

            #region Base Object

            var baseobj = new CodeTypeDeclarationEx("Object") { IsClass = true, IsBuildInType = true };
            _superBase = new CodeTypeReferenceEx("Object");
            baseobj.Comments.Add(new CodeCommentStatement("Base Object of all other Custom Objects", true));

            CodeMemberMethodExAHK method;

            #region Object.Insert

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Insert",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "key"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "value"));
            method.Comments.Add(new CodeCommentStatement("Inserts key-value pairs into the object, automatically adjusting existing keys if appropriate.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            baseobj.Members.Add(method);

            #endregion

            #region Object.Remove

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Remove",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "key"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "value"));
            method.Comments.Add(new CodeCommentStatement("Removes key-value pairs from an object.", true));
            method.ReturnType = new CodeTypeReference(typeof(bool));
            baseobj.Members.Add(method);

            #endregion

            #region Object.Clone

            method = new CodeMemberMethodExAHK(true)
            {
                Name = "Clone",
                IsDefaultMethodInvoke = true,
                IsTraditionalCommand = false
            };
            method.Comments.Add(new CodeCommentStatement("Returns a shallow copy of the object.", true));
            method.ReturnType = new CodeTypeReference(typeof(object));
            baseobj.Members.Add(method);

            #endregion

            RootType.Members.Add(baseobj);

            #endregion

            foreach(var m in project.Language.BuildInMembers) {
                RootType.Members.Add(m);
            }
        }

        public override void CompileTokenFile(Projecting.ProjectItemCode codeitem, CodeTypeDeclarationEx initialparent) {

            #region Clean Up

            //remove all old members which are from this code file:

            var oldMembers = (from CodeTypeMember m in RootType.Members
                              where (m.LinePragma == null || m.LinePragma.FileName == codeitem.FilePath)
                              let meth = m as ICodeObjectEx
                              where meth == null || (!meth.IsHidden && !meth.IsBuildInType)
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

                var classkeywordSegment = line.CodeSegments[0].ThisOrNextOmit(whitespacetokenNewLines);
                if(classkeywordSegment != null && classkeywordSegment.Token == Token.KeyWord && classkeywordSegment.TokenString.Equals("class", StringComparison.CurrentCultureIgnoreCase)) {
                    var classNameSegment = classkeywordSegment.FindNextOnSameLine(Token.Identifier);
                    if(classNameSegment != null) {

                        var next = classNameSegment.NextOmit(whitespacetokenNewLines);
                        if(next != null) {

                            CodeTypeReferenceEx basecls = null;
                            if(next.Token == Token.KeyWord && next.TokenString.Equals("extends", StringComparison.InvariantCultureIgnoreCase)) {
                                var refclass = next.NextOmit(whitespacetokenNewLines);
                                refclass.CodeDOMObject = basecls = new CodeTypeReferenceEx(refclass.TokenString);
                                next = refclass.NextOmit(whitespacetokenNewLines);
                            }

                            if(next.Token == Token.BlockOpen) {
                                CodeSegment classBodyStart = next;

                                var type = new CodeTypeDeclarationEx(classNameSegment.TokenString)
                                {
                                    IsClass = true
                                };
                                classNameSegment.CodeDOMObject = type;

                                if(basecls != null)
                                    type.BaseTypes.Add(basecls);
                                else
                                    type.BaseTypes.Add(_superBase);

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


                // is class property / field

                #region Parse Class Properties / Fields

                var decl = line.CodeSegments[0].ThisOrNextOmit(whitespacetokens);
                if(decl != null && decl.Token == Token.KeyWord && decl.TokenString == "var") {
                    var property = decl.NextOmit(whitespacetokens);

                    if(parentHirarchy.Count > 1) {
                        // we must be in a class to have method properties
                        if(property != null && property.Token == Token.Identifier) {
                            // this is a class field declaration

                            var propertyType = new CodeTypeReference(typeof(object));
                            var memberprop = new CodeMemberProperty()
                            {
                                Name = property.TokenString,
                                Attributes = MemberAttributes.Public,
                                Type = propertyType
                            };
                            property.CodeDOMObject = memberprop;
                            decl.CodeDOMObject = propertyType;
                            parentHirarchy.Peek().Members.Add(memberprop);
                        } else {
                            var err = new CodeError() { Message = "unexpected Token -> Expected Identifier!" };
                            property.ErrorContext = err;
                        }
                    } else {
                        var err = new CodeError() { Message = "unexpected class field declaration -> not in class body" };
                        if(property != null)
                            property.ErrorContext = err;
                        decl.ErrorContext = err;
                    }



                }

                #endregion


                // is method definition?:

                #region Analyze for Method Definition

                var methodSegment = line.CodeSegments[0].ThisOrNextOmit(whitespacetokenNewLines);
                if(methodSegment != null && methodSegment.Token == Token.Identifier) {
                    var methodSignatureStart = methodSegment.Next;
                    if(methodSignatureStart != null && methodSignatureStart.Token == Token.LiteralBracketOpen) {
                        var methodSignatureEnd = methodSignatureStart.FindClosingBracked(false);
                        if(methodSignatureEnd != null) {
                            var startMethodBody = methodSignatureEnd.NextOmit(whitespacetokenNewLines);
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
                                        ReturnType = new CodeTypeReferenceEx(typeof(object))
                                    };
                                    methodSegment.CodeDOMObject = method;


                                    // extract Method Comment
                                    var comment = methodSegment.PreviousOmit(whitespacetokenNewLines);
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
                                if(parentHirarchy.Any())
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
            CodeExpression expression = null;
            nextToParse = tokenSegment.Next;

            //simply parse for Method Invokes
            if(tokenSegment.Token == Token.Identifier && tokenSegment.Next != null
                    && tokenSegment.Next.Token == Token.LiteralBracketOpen) {

                var invokeExpression = new CodeMethodInvokeExpression();

                // var method = _codeDOMTraveler.FindBestMethod(nextidentifier.TokenString, null, RootType); //<-- todo emit correct type

                var methodRef = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), tokenSegment.TokenString);

                invokeExpression.Method = methodRef;
                tokenSegment.CodeDOMObject = methodRef;

                nextToParse = tokenSegment.Next.Next;
                expression = invokeExpression;
            } else if(tokenSegment.Token == Token.KeyWord) {
                // parse for new Object Expressions
                if(tokenSegment.TokenString.Equals("new", StringComparison.InvariantCultureIgnoreCase)) {
                    var newObjectInvoke = tokenSegment.NextOmit(whitespacetokenNewLines);
                    if(newObjectInvoke != null && newObjectInvoke.Token == Token.Identifier) {

                        var objectinstangicing = new CodeObjectCreateExpression();
                        objectinstangicing.CreateType = new CodeTypeReferenceEx(newObjectInvoke.TokenString);
                        tokenSegment.CodeDOMObject = objectinstangicing;
                        newObjectInvoke.CodeDOMObject = objectinstangicing.CreateType;

                        expression = objectinstangicing;
                        nextToParse = newObjectInvoke.Next;
                    }
                } else if(tokenSegment.TokenString.Equals("this", StringComparison.InvariantCultureIgnoreCase)) {
                    var thisrefExpression = new CodeThisReferenceExpression();
                    tokenSegment.CodeDOMObject = thisrefExpression;
                    expression = thisrefExpression;
                }else if(tokenSegment.TokenString.Equals("base", StringComparison.InvariantCultureIgnoreCase)) {
                    var baserefExpression = new CodeBaseReferenceExpression();
                    tokenSegment.CodeDOMObject = baserefExpression;
                    expression = baserefExpression;
                }

                
            }

            if(!(nextToParse != null && nextToParse.LineNumber == tokenSegment.LineNumber)) {
                nextToParse = null;
            }
            return expression;
        }



    }
}
