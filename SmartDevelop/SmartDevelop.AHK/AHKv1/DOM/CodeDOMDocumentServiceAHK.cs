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
using System.Collections;
using SmartDevelop.Model.Tokenizing;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Windows;
using SmartDevelop.AHK.AHKv1.DOM;


namespace SmartDevelop.Model.DOM
{

    /// <summary>
    ///  CodeDOM Service Impl. for AHK
    /// </summary>
    public class CodeDOMDocumentServiceAHK : CodeDocumentDOMService
    {    
        #region Fields

        Archimedes.CodeDOM.CodeDOMTraveler _codeDOMTraveler = new Archimedes.CodeDOM.CodeDOMTraveler();
        CodeMemberMethodEx _autoexec;
        CodeTypeDeclarationEx _superBase;
        BackgroundWorker _fileCompileWorker;

        object startcompilerLock = new object();
        ProjectItemCodeDocument _currentItem = null;

        #endregion

        #region Constructor

        public CodeDOMDocumentServiceAHK(ProjectItemCodeDocument document)
            : base(document) {

                var _checkQueueTimer = new System.Timers.Timer(); // Timer anlegen
                _checkQueueTimer.Interval = 100; // Intervall festlegen, hier 100 ms
                _checkQueueTimer.Elapsed += (s, e) => CheckQueue();
                _checkQueueTimer.Start(); 


                _fileCompileWorker = new BackgroundWorker();
                _fileCompileWorker.DoWork += CompileTokenFile;
                _fileCompileWorker.RunWorkerCompleted += (s, e) => {
                    OnRunWorkerCompleted();
                };
                _fileCompileWorker.WorkerSupportsCancellation = true;
                _fileCompileWorker.WorkerReportsProgress = false;


            lock(_languageRootLock) {

                #region Create  auto exec method for AHK Scripts

                _languageRoot.Members.Add(_autoexec = new CodeMemberMethodExAHK(true)
                {
                    Name = "AutoExec",
                    Attributes = MemberAttributes.Public | MemberAttributes.Static,
                    DefiningType = _languageRoot,
                    ReturnType = new CodeTypeReference(typeof(void)),
                    LinePragma = new CodeLinePragma("all", 0),
                    IsHidden = true,
                    Project = document.Project
                });

                #endregion

                #region Setup Base Object

                var baseobj = new CodeTypeDeclarationEx(null, "Object")
                {
                    Project = document.Project,
                    IsClass = true,
                    IsBuildInType = true
                };
                /*_superBase = new CodeTypeReferenceEx("Object", );*/
                baseobj.Comments.Add(new CodeCommentStatement("Base Object of all other Custom Objects", true));

                CodeMemberMethodExAHK method;

                #region Object.Insert

                method = new CodeMemberMethodExAHK(true)
                {
                    Name = "Insert",
                    Project = document.Project,
                    IsDefaultMethodInvoke = true,
                    IsTraditionalCommand = false,
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
                    Project = document.Project,
                    IsDefaultMethodInvoke = true,
                    IsTraditionalCommand = false
                };
                method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "key"));
                method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "value"));
                method.Comments.Add(new CodeCommentStatement("Removes key-value pairs from an object.", true));
                method.ReturnType = new CodeTypeReference(typeof(bool));
                baseobj.Members.Add(method);

                #endregion

                //#region Object.Clone

                //method = new CodeMemberMethodExAHK(true)
                //{
                //    Name = "Clone",
                //    Project = project,
                //    IsDefaultMethodInvoke = true,
                //    IsTraditionalCommand = false
                //};
                //method.Comments.Add(new CodeCommentStatement("Returns a shallow copy of the object.", true));
                //method.ReturnType = new CodeTypeReference(typeof(object));
                //baseobj.Members.Add(method);

                //#endregion

                //#region Object.MinIndex

                //method = new CodeMemberMethodExAHK(true)
                //{
                //    Name = "MinIndex",
                //    Project = project,
                //    IsDefaultMethodInvoke = true,
                //    IsTraditionalCommand = false
                //};
                //method.Comments.Add(new CodeCommentStatement("If any integer keys are present, MinIndex returns the lowest and MaxIndex returns the highest. Otherwise an empty string is returned.", true));
                //method.ReturnType = new CodeTypeReference(typeof(int));
                //baseobj.Members.Add(method);

                //#endregion

                //#region Object.MaxIndex

                //method = new CodeMemberMethodExAHK(true)
                //{
                //    Name = "MaxIndex",
                //    Project = project,
                //    IsDefaultMethodInvoke = true,
                //    IsTraditionalCommand = false
                //};
                //method.Comments.Add(new CodeCommentStatement("If any integer keys are present, MinIndex returns the lowest and MaxIndex returns the highest. Otherwise an empty string is returned.", true));
                //method.ReturnType = new CodeTypeReference(typeof(int));
                //baseobj.Members.Add(method);

                //#endregion

                //#region Object.SetCapacity

                //method = new CodeMemberMethodExAHK(true)
                //{
                //    Name = "SetCapacity",
                //    Project = project,
                //    IsDefaultMethodInvoke = true,
                //    IsTraditionalCommand = false
                //};
                //method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "MaxItemsOrKey"));
                //method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "ByteSize"));
                //method.Comments.Add(new CodeCommentStatement("If any integer keys are present, MinIndex returns the lowest and MaxIndex returns the highest. Otherwise an empty string is returned.", true));
                //method.ReturnType = new CodeTypeReference(typeof(int));
                //baseobj.Members.Add(method);

                //#endregion

                //#region Object.GetCapacity

                //method = new CodeMemberMethodExAHK(true)
                //{
                //    Name = "GetCapacity",
                //    Project = project,
                //    IsDefaultMethodInvoke = true,
                //    IsTraditionalCommand = false
                //};
                //method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Key"));
                //method.Comments.Add(new CodeCommentStatement("Returns the current capacity of an object or one of its fields.", true));
                //method.ReturnType = new CodeTypeReference(typeof(int));
                //baseobj.Members.Add(method);

                //#endregion

                //#region Object.HasKey

                //method = new CodeMemberMethodExAHK(true)
                //{
                //    Name = "HasKey",
                //    Project = project,
                //    IsDefaultMethodInvoke = true,
                //    IsTraditionalCommand = false
                //};
                //method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "Key"));
                //method.Comments.Add(new CodeCommentStatement("Returns true if Key is associated with a value (even '') within Object, otherwise false.", true));
                //method.ReturnType = new CodeTypeReference(typeof(bool));
                //baseobj.Members.Add(method);

                //#endregion

                //#region Object._NewEnum

                //method = new CodeMemberMethodExAHK(true)
                //{
                //    Name = "_NewEnum",
                //    Project = project,
                //    IsDefaultMethodInvoke = true,
                //    IsTraditionalCommand = false
                //};

                //method.Comments.Add(new CodeCommentStatement("Returns a new enumerator to enumerate this object's key-value pairs.", true));
                //method.ReturnType = new CodeTypeReference(typeof(IEnumerator));
                //baseobj.Members.Add(method);

                //#endregion


                _languageRoot.Members.Add(baseobj);

                #endregion

                // Import build-in members
                foreach(var m in document.CodeLanguage.BuildInMembers) {
                    var codeobj = m as ICodeMemberEx;
                    if(codeobj != null) {
                        codeobj.Project = document.Project;
                    }
                    _languageRoot.Members.Add(m);
                }
            }
        }



        void CheckQueue()
        {
            ProjectItemCodeDocument compileme = null;
            lock(_itemsToCompileLock) {
                if(_compileQueue.Any()) {
                    compileme = _compileQueue.First();
                }
            }

            if(compileme != null) {
                if(_fileCompileWorker.IsBusy) {
                    if(_currentItem == compileme) { // can we cancel it?
                        if(!_fileCompileWorker.CancellationPending)
                            _fileCompileWorker.CancelAsync();

                        while(true) {
                            if(_fileCompileWorker.IsBusy) {
                                System.Threading.Thread.Sleep(1);
                            } else
                                break;
                        }
                    } else
                        return; // no change.. we have to wait until it has finished.
                }
                // if we are here the compiler is finished. either by completion or by cancelation

                lock(_itemsToCompileLock) {
                        _currentItem = compileme;
                        _compileQueue.Remove(_currentItem);
                        _fileCompileWorker.RunWorkerAsync(new CompilerArgument(compileme, null));
                }
            }

        }


        #endregion

        #region Public Methods


        List<ProjectItemCodeDocument> _compileQueue = new List<ProjectItemCodeDocument>();
        object _itemsToCompileLock = new object();


        /// <summary>
        /// Ensures that there is no item queued and the parser is finished.
        /// </summary>
        public override void EnsureIsUpdated() {
            while(true) {
                lock(_itemsToCompileLock) {
                    if(!_compileQueue.Any()) {
                        if(!IsBusy)
                            break;
                    }  
                }
                System.Threading.Thread.Sleep(1);
            }
        }


        public override void CompileTokenFileAsync(ProjectItemCodeDocument codeitem) {
            lock(startcompilerLock) {
                // enqueue the given codedocument
                // the queue will be checked by a timer
                lock(_itemsToCompileLock) {
                    if(!_compileQueue.Contains(codeitem))
                        _compileQueue.Add(codeitem);
                }
            }
        }


        #endregion

        #region Token Compiler Worker

        protected virtual void CompileTokenFile(object sender, DoWorkEventArgs e) {

            if(e.Cancel)
                return;


            var arg = e.Argument as CompilerArgument;
            ProjectItemCodeDocument codeitem = arg.Codeitem;

            lock(_languageRootLock) {

                CodeTypeDeclarationEx initialparent = _languageRoot; //arg.Initialparent;

                if(e.Cancel)
                    return;

                #region Clean Up

                //remove all old members which are from this code file:

                var oldMembers = (from CodeTypeMember m in _languageRoot.Members
                                  let meth = m as ICodeMemberEx
                                  where (meth == null || (!meth.IsHidden && !meth.IsBuildInType && codeitem.Equals(meth.CodeDocumentItem)))
                                  select m).ToList();
                foreach(var m in oldMembers)
                    _languageRoot.Members.Remove(m);

                if(_codeRanges.ContainsKey(codeitem))
                    _codeRanges[codeitem].Clear();
                else
                    _codeRanges.Add(codeitem, new CodeRangeManager());
                var currentRanges = _codeRanges[codeitem];

                // cleanup errors
                codeitem.Project.Solution.ErrorService.ClearAllErrorsFrom(codeitem);

                #endregion

                var codeLineMap = codeitem.SegmentService.GetCodeSegmentLinesMap();
                CodeTypeDeclaration parent = initialparent;
                Stack<CodeSegment> paramstack = new Stack<CodeSegment>();
                int linecnt = 0;
                if(codeLineMap.Keys.Any())
                    linecnt = codeLineMap.Keys.Max();

                CodeTokenLine line;

                Stack<CodeTypeDeclarationEx> parentHirarchy = new Stack<CodeTypeDeclarationEx>();
                int bcc = 0;
                parentHirarchy.Push(initialparent);

                if(e.Cancel)
                    return;

                #region Parse

                for(int i = 0; i <= linecnt; i++) {

                    if(e.Cancel)
                        return;

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
                                CodeTypeDeclarationEx thisparent = parentHirarchy.Any() ? parentHirarchy.Peek() : _languageRoot;
                                CodeTypeReferenceEx basecls = null;
                                CodeSegment refBaseClass = null;
                                if(next.Token == Token.KeyWord && next.TokenString.Equals("extends", StringComparison.InvariantCultureIgnoreCase)) {
                                    refBaseClass = next.NextOmit(whitespacetokenNewLines);

                                    if(refBaseClass != null) {
                                        if(refBaseClass.Token == Token.Identifier) {
                                            refBaseClass.CodeDOMObject = basecls = new CodeTypeReferenceEx(codeitem, refBaseClass.TokenString, thisparent);
                                            next = refBaseClass.NextOmit(whitespacetokenNewLines);
                                        } else {
                                            RegisterError(codeitem, next.Next, "Expected: Class Name Identifier");
                                            next = next.NextOmit(whitespacetokenNewLines);
                                        }
                                    } else {
                                        if(next.Next != null && next.Next.Token != Token.BlockOpen) {
                                            RegisterError(codeitem, next.Next, "Expected: Class Name Identifier");
                                            next = next.NextOmit(whitespacetokenNewLines);
                                        }
                                    }

                                }

                                if(next.Token == Token.BlockOpen) {

                                    #region Add Class Declaration

                                    CodeSegment classBodyStart = next;

                                    var type = new CodeTypeDeclarationEx(codeitem, classNameSegment.TokenString)
                                    {
                                        IsClass = true,
                                        LinePragma = CreatePragma(classNameSegment, codeitem.FilePath),
                                        CodeDocumentItem = codeitem
                                    };
                                    classNameSegment.CodeDOMObject = type;


                                    // check if this type was alread defined in this scope
                                    if(thisparent.GetInheritedMembers().Contains(type)) {
                                        RegisterError(codeitem, classNameSegment, "oh my dear, this class already exisits in the current scope!");
                                    } else {

                                        #region Check & Resolve Baseclass

                                        if(basecls != null) {
                                            //check if we have a circual interhance tree
                                            var baseclassImpl = basecls.ResolveTypeDeclarationCache();
                                            if(baseclassImpl != null && baseclassImpl.IsSubclassOf(new CodeTypeReferenceEx(codeitem, classNameSegment.TokenString, thisparent))) {
                                                //circular dependency detected!!
                                                RegisterError(codeitem, refBaseClass, "Woops you just produced a circular dependency in your inheritance tree!");
                                            } else {
                                                if(basecls != null)
                                                    type.BaseTypes.Add(basecls);
                                                else
                                                    type.BaseTypes.Add(new CodeTypeReferenceEx(codeitem, "Object", thisparent) { ResolvedTypeDeclaration = _superBase });
                                            }
                                        }

                                        #endregion


                                        // extract class documentation Comment
                                        var comment = ExtractComment(classkeywordSegment);
                                        if(comment != null)
                                            type.Comments.Add(comment);


                                        // Add it to the CodeDOM Tree
                                        thisparent.Members.Add(type);
                                        type.Parent = thisparent;
                                    }

                                    // Create a CodeRange Item
                                    int startOffset = classBodyStart.Range.Offset;
                                    var classBodyEnd = classBodyStart.FindClosingBracked(true);
                                    if(classBodyEnd != null) {
                                        int length = (classBodyEnd.Range.Offset - startOffset);
                                        currentRanges.Add(new CodeRange(new SimpleSegment(startOffset, length), type));
                                    } else {
                                        RegisterError(codeitem, classBodyStart, "Expected: " + Token.BlockClosed);
                                    }

                                    parentHirarchy.Push(type);
                                    bcc++;

                                    i = classBodyStart.LineNumber; // jumt to:  class Foo { * <---|
                                    continue;

                                    #endregion

                                } else {
                                    RegisterError(codeitem, next, "Expected: " + Token.BlockOpen);
                                    i = (next.Next != null) ? next.Next.LineNumber : next.LineNumber;
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
                                var memberprop = new CodeMemberPropertyEx(codeitem)
                                {
                                    Name = property.TokenString,
                                    Attributes = MemberAttributes.Public,
                                    Type = propertyType,
                                    LinePragma = CreatePragma(property, codeitem.FilePath)
                                };
                                property.CodeDOMObject = memberprop;
                                decl.CodeDOMObject = propertyType;
                                parentHirarchy.Peek().Members.Add(memberprop);
                            } else {
                                RegisterError(codeitem, property, "unexpected Token -> Expected Identifier!");
                            }
                        } else {
                            var err = "unexpected class field declaration -> not in class body";
                            if(property != null)
                                RegisterError(codeitem, property, err);
                            RegisterError(codeitem, decl, err);
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
                                var startMethodBody = methodSignatureEnd.NextOmit(whitespacetokenNewLinesComments);
                                if(startMethodBody != null && startMethodBody.Token == Token.BlockOpen) {
                                    // jup we have a method definition here.
                                    // Method body starts at startMethodBody


                                        CodeTypeDeclarationEx thisparent = parentHirarchy.Any() ? parentHirarchy.Peek() : _languageRoot;
                                        bool hasDeclarationError = false;

                                        #region Generate Method Definition DOM

                                        var method = new CodeMemberMethodExAHK(codeitem)
                                        {
                                            Name = methodSegment.TokenString,
                                            LinePragma = CreatePragma(methodSegment, codeitem.FilePath),
                                            CodeDocumentItem = codeitem,
                                            ReturnType = new CodeTypeReferenceEx(codeitem, typeof(object))
                                        };
                                        methodSegment.CodeDOMObject = method;

                                        //check if this method is not already defined elsewere in current scope

                                        var equalmethods = from m in thisparent.Members.Cast<CodeTypeMember>()
                                                           let meth = m as CodeMemberMethodExAHK
                                                           where meth != null && !meth.IsBuildInType && meth.Equals(method)
                                                           select meth;


                                        if(equalmethods.Any()) {
                                            RegisterError(codeitem, methodSegment,
                                                string.Format("The Methodename '{0}' is already used in the current scope!", method.Name));
                                            hasDeclarationError = true;
                                        } else {


                                            // extract Method Comment
                                            var comment = ExtractComment(methodSegment);
                                            if(comment != null)
                                                method.Comments.Add(comment);

                                            // extract method params
                                            paramstack.Clear();
                                            CodeSegment previous = methodSignatureStart;

                                            // get method properties:
                                            while(true) {
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
                                        }
                                        #endregion

                                        // Method body ends at
                                        var endMethodBody = startMethodBody.FindClosingBracked(true);
                                        if(endMethodBody != null) {

                                            // get method statements
                                            method.Statements.AddRange(
                                                CollectAllCodeStatements(e, codeitem, thisparent, codeLineMap, startMethodBody.LineNumber + 1, endMethodBody.LineNumber));


                                            // add it to the code DOM Tree
                                            if(!hasDeclarationError) {
                                                thisparent.Members.Add(method);
                                                method.DefiningType = thisparent;
                                            }


                                            // Create a CodeRange Item
                                            int startOffset = startMethodBody.Range.Offset;
                                            int length = (endMethodBody.Range.Offset - startOffset);
                                            currentRanges.Add(new CodeRange(new SimpleSegment(startOffset, length), method));

                                            // move the scanpointer to the method end:
                                            i = endMethodBody.LineNumber;
                                            continue;
                                        } else {
                                            RegisterError(codeitem, startMethodBody, "Missing: " + Token.BlockClosed);
                                            

                                        }
                                }
                            }
                        }
                    }

                    #endregion

                    #region Parse Remaining Tokens

                    if(codeLineMap.ContainsKey(i)) {
                        var lineBlock = codeLineMap[i];
                        CodeTypeDeclarationEx thisparent = parentHirarchy.Any() ? parentHirarchy.Peek() : _languageRoot;

                        foreach(var segment in lineBlock.CodeSegments) {

                            if(e.Cancel)
                                return;

                            if(segment.Token == Token.BlockOpen) {
                                bcc++;
                            } else if(segment.Token == Token.BlockClosed) {
                                bcc--;
                                if(parentHirarchy.Count - 2 == bcc) {
                                    if(parentHirarchy.Any())
                                        parentHirarchy.Pop();
                                }
                            }
                        }
                        _autoexec.Statements.AddRange(
                                        CollectAllCodeStatements(e, codeitem, thisparent, codeLineMap, i, i));

                    } else
                        continue;

                    #endregion
                }

                #endregion

                AnalyzeAST(codeitem, e);
            }
        }

        CodeCommentStatement ExtractComment(CodeSegment identifier) {
            var comment = identifier.PreviousOmit(whitespacetokenNewLines);
            if(comment != null && comment.Token == Token.MultiLineComment) {
                return new CodeCommentStatement(GetDocumentationComment(comment.TokenString), true);
            } else if(comment != null && comment.Token == Token.SingleLineComment) {

                //todo: collect all above singleline comments
            }
            return null;
        }


        string GetDocumentationComment(string data) {
            StringBuilder sb = new StringBuilder();
            foreach(var line in data.Split('\n')) {
                var cline = line.Trim();
                cline = cline.TrimStart(' ', '\t', ';', '/');
                cline = cline.TrimStart('*');
                cline = cline.TrimEnd(' ', '\t', '/');
                cline = cline.TrimEnd('*');
                cline = cline.Trim();
                if(!string.IsNullOrWhiteSpace(cline))
                    sb.AppendLine(cline);
            }
            return sb.ToString();
        }

        #endregion

        #region Analyze AST


        void AnalyzeAST(Projecting.ProjectItemCodeDocument codeitem, DoWorkEventArgs e) {

            var segmentService = codeitem.SegmentService;
            var segments = segmentService.GetSegments();

            foreach(var segment in segments) {

                if(e.Cancel)
                    return;


                #region Resolve CodeTypeReferencees

                if(segment.CodeDOMObject is CodeTypeReferenceEx) {
                    var codeTypeRef = segment.CodeDOMObject as CodeTypeReferenceEx;

                    if(codeTypeRef.ResolvedTypeDeclaration == null) {
                        var refi = codeTypeRef.ResolveTypeDeclarationCache();
                        if(refi == null) {
                            RegisterError(codeitem, segment,
                                string.Format("Type '{0}' does not exist!", segment.TokenString));
                        }
                    }

                }

                #endregion

                #region Resolve Code Method Invoke Referencees


                if(segment.CodeDOMObject is CodeMethodReferenceExpressionEx) {
                    var methodRef = segment.CodeDOMObject as CodeMethodReferenceExpressionEx;
                    if(methodRef.ResolvedMethodMember == null && !(methodRef.EnclosingType is CodeTypeDeclarationDynamic)) {
                        var refi = methodRef.ResolveMethodDeclarationCache();
                        if(refi == null) {
                            RegisterError(codeitem, segment,
                                string.Format("Method '{0}' does not exist!", segment.TokenString));
                        }
                    }
                }

                #endregion

                #region Resolve Code Property Invoke Referencees

                if(segment.CodeDOMObject is CodePropertyReferenceExpressionEx) {
                    var propRef = segment.CodeDOMObject as CodePropertyReferenceExpressionEx;
                    if(propRef.ResolvedPropertyMember == null && !(propRef.EnclosingType is CodeTypeDeclarationDynamic)) {
                        var refi = propRef.ResolvePropertyDeclarationCache();
                        if(refi == null) {
                            RegisterError(codeitem, segment,
                                string.Format("Property '{0}' does not exist!", segment.TokenString));
                        }
                    }
                }

                #endregion
            }
        }

        #endregion

        #region Helper Methods

        void RegisterError(Projecting.ProjectItemCodeDocument codeitem, CodeSegment segment, string errorDescription) {
            var errorService = codeitem.Project.Solution.ErrorService;
            segment.ErrorContext = new CodeError() { Description = errorDescription };
            errorService.Add(new Errors.ErrorItem(segment, codeitem));
        }

        CodeLinePragma CreatePragma(CodeSegment segment, string filename) {
            return new CodeLinePragma() { LineNumber = segment.LineNumber, FileName = filename };
        }

        #endregion

        #region Parse CodeStatements

        /// <summary>
        /// Get all CodeStatements in the given Linerange
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <returns></returns>
        CodeStatementCollection CollectAllCodeStatements(DoWorkEventArgs e, Projecting.ProjectItemCodeDocument codeitem, CodeTypeDeclarationEx enclosingType, Dictionary<int, CodeTokenLine> segments, int startLine, int endLine) {
            CodeTokenLine line;
            var codeStatements = new CodeStatementCollection();

            for(int i = startLine; i <= endLine; i++) {

                if(e.Cancel)
                    break;

                if(segments.ContainsKey(i))
                    line = segments[i];
                else
                    continue;

                CodeSegment toParse = line.CodeSegments.First();
                CodeSegment next;
                while(toParse != null) {
                    var ex = ParseExpression(codeitem, toParse, out next, enclosingType);
                    codeStatements.Add(ex);
                    toParse = next;
                }
            }
            return codeStatements;
        }

        static readonly List<Token> LocalExpressionEndTokens = new List<Token>() { Token.NewLine, Token.ParameterDelemiter };


        CodeExpression ParseExpression(Projecting.ProjectItemCodeDocument codeitem, CodeSegment tokenSegment, out CodeSegment nextToParse, CodeTypeDeclarationEx enclosingType) {
            CodeExpression expression = null;
            nextToParse = tokenSegment.Next;


            if(tokenSegment.Token == Token.Identifier && tokenSegment.Next != null
                    && tokenSegment.Next.Token == Token.LiteralBracketOpen) {



                        bool ismethodDeclaration = false;
                var closingliteral = tokenSegment.Next.FindClosingBracked(true);
                if(closingliteral != null) {
                    //ensure that it is not a defect method declaration
                    var bra = closingliteral.NextOmit(whitespacetokenNewLines);
                    if(bra != null && bra.Token == Token.BlockOpen) {
                        // it is a method indeed
                        ismethodDeclaration = true;
                    }
                } else {
                    RegisterError(codeitem, tokenSegment.Next, "Missing: )");
                }
                


                #region Parse for Method Invokes

                if(!ismethodDeclaration) {

                    CodeTypeDeclarationEx methodContext = null;

                    if(tokenSegment.Previous != null && tokenSegment.Previous.Previous != null
                        && tokenSegment.Previous.Token == Token.MemberInvoke) {

                        var invoker = tokenSegment.Previous.Previous;

                        #region adjust Method Context

                        if(codeitem.CodeLanguage.SELFREF_CAN_BE_OMITTED)
                            methodContext = enclosingType;

                        if(invoker.CodeDOMObject is CodeBaseReferenceExpression) {
                            foreach(CodeTypeReferenceEx bt in enclosingType.BaseTypes) {
                                var typedeclaration = bt.ResolveTypeDeclarationCache();
                                if(typedeclaration != null && typedeclaration.IsClass) {
                                    methodContext = typedeclaration;
                                    break;
                                }
                            }
                        } else if(invoker.CodeDOMObject is CodeThisReferenceExpression) {
                            methodContext = enclosingType;
                        } else if(invoker.Token == Token.Identifier) {
                            invoker.CodeDOMObject = CodeTypeDeclarationDynamic.Default;
                            methodContext = CodeTypeDeclarationDynamic.Default;
                        }

                        #endregion
                    }

                    var invokeExpression = new CodeMethodInvokeExpression();
                    var methodRef = new CodeMethodReferenceExpressionExAHK(codeitem, null, tokenSegment.TokenString, methodContext);

                    invokeExpression.Method = methodRef;
                    tokenSegment.CodeDOMObject = methodRef;
                    expression = invokeExpression;
                }

                nextToParse = tokenSegment.Next.Next;
                
                #endregion

            } else if(tokenSegment.Token == Token.KeyWord) {

                #region Parse Keywords

                if(tokenSegment.TokenString.Equals("new", codeitem.CodeLanguage.NameComparisation)) {

                    #region NEW parse for new Object Expressions

                    var newObjectInvoke = tokenSegment.NextOmit(whitespacetokenNewLines);
                    if(newObjectInvoke != null && newObjectInvoke.Token == Token.Identifier) {

                        var objectinstangicing = new CodeObjectCreateExpression();
                        objectinstangicing.CreateType = new CodeTypeReferenceEx(codeitem, newObjectInvoke.TokenString, enclosingType);
                        tokenSegment.CodeDOMObject = objectinstangicing;
                        newObjectInvoke.CodeDOMObject = objectinstangicing.CreateType;

                        expression = objectinstangicing;
                        nextToParse = newObjectInvoke.Next;
                    }

                    #endregion

                } else if(tokenSegment.TokenString.Equals("this", codeitem.CodeLanguage.NameComparisation)) {
                    var thisrefExpression = new CodeThisReferenceExpression();
                    tokenSegment.CodeDOMObject = thisrefExpression;
                    expression = thisrefExpression;
                } else if(tokenSegment.TokenString.Equals("base", codeitem.CodeLanguage.NameComparisation)) {
                    var baserefExpression = new CodeBaseReferenceExpression();
                    tokenSegment.CodeDOMObject = baserefExpression;
                    expression = baserefExpression;
                }

                #endregion

            } else if(tokenSegment.Token == Token.Identifier  && tokenSegment.Previous != null
                    && tokenSegment.Previous.Token == Token.MemberInvoke) {

                #region Parse MemberInvoke

                var context = tokenSegment.Previous.Previous;
                if(context == null) {
                    //unexpected!
                    var err = "Unexpected Member Invoke!";
                    RegisterError(codeitem, tokenSegment, err);
                    RegisterError(codeitem, tokenSegment.Previous, err);
                    nextToParse = tokenSegment.Next;
                } else if(context.Token == Token.KeyWord && context.TokenString.Equals("this", codeitem.CodeLanguage.NameComparisation)) {

                    var propRef = new CodePropertyReferenceExpressionEx(codeitem, null, tokenSegment.TokenString, enclosingType);
                    tokenSegment.CodeDOMObject = propRef;

                } else if(context.Token == Token.KeyWord && context.TokenString.Equals("base", codeitem.CodeLanguage.NameComparisation)) {

                    CodeTypeDeclarationEx typedeclaration = null;
                    foreach(CodeTypeReferenceEx bt in enclosingType.BaseTypes) {
                        typedeclaration = bt.ResolveTypeDeclarationCache();
                        if(typedeclaration != null && typedeclaration.IsClass) {
                            break;
                        }
                    }
                    var propRef = new CodePropertyReferenceExpressionEx(codeitem, null, tokenSegment.TokenString, typedeclaration);
                    tokenSegment.CodeDOMObject = propRef;

                } else if(context.Token == Token.Identifier) {
                    // we currently not supprt real expression parsing, so leave here...

                    context.CodeDOMObject = CodeTypeDeclarationDynamic.Default;
                    var propRef = new CodePropertyReferenceExpressionEx(codeitem, null, tokenSegment.TokenString, CodeTypeDeclarationDynamic.Default);
                    tokenSegment.CodeDOMObject = propRef;

                }

                #region Parse for one hirarchy this/base Property/Field Invokes



                #endregion

                #endregion

            } else if(tokenSegment.Token == Token.TraditionalCommandInvoke) {

                #region Parse Traditional Command Invoke

                var members = from m in _languageRoot.Members.Cast<CodeTypeMember>()
                              let methd = m as CodeMemberMethodExAHK
                              where methd != null && methd.IsTraditionalCommand && methd.Name.Equals(tokenSegment.TokenString, StringComparison.InvariantCultureIgnoreCase)
                              select methd;
                if(members.Any()) {

                    var invokeExpression = new CodeMethodInvokeExpression();
                    var methodRef = new CodeMethodReferenceExpressionExAHK(codeitem, members.First());

                    tokenSegment.CodeDOMObject = methodRef;
                    expression = invokeExpression;
                } else {
                    RegisterError(codeitem, tokenSegment, string.Format("Unknown traditional Command '{0}'", tokenSegment.TokenString));
                }

                #endregion
            }

            if(!(nextToParse != null && nextToParse.LineNumber == tokenSegment.LineNumber)) {
                nextToParse = null;
            }
            return expression;
        }

        #endregion

        protected virtual void OnRunWorkerCompleted() {
            _currentItem = null;
            OnASTUpdated();
        }

        public override bool IsBusy {
            get { return _fileCompileWorker.IsBusy; }
        }

        public override CodeTypeDeclarationEx GetRootTypeSnapshot() {
            throw new NotImplementedException();
        }




    }
}
