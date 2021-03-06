﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.DOM;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokenizing;
using SmartDevelop.AHK.AHKv1.Projecting;
using Archimedes.Patterns.Utils;
using System.IO;
using System.Text.RegularExpressions;
using SmartDevelop.Model.CodeLanguages;
using System.Threading.Tasks;

namespace SmartDevelop.AHK.AHKv1.DOM
{
    public class ASTManagerAHK : ASTManager
    {
        const string INCLUDE_ONCE = "#Include";

        IncludeTreeBuilder _includeTreeBuilder = new IncludeTreeBuilder();

        public ASTManagerAHK(SmartCodeProject project) 
            : base(project) { }


        protected override void OnCodeDocumentTokenizerUpdated(object sender, EventArgs e) {
            if(_project.ASTManager.UpdateAtWill) {
                var document = sender as ProjectItemCodeDocument;
                UpdateDocumentIncludeFlow(document);
                UpdateDocumentOrder();
                document.AST.CompileTokenFileAsync();
            }
        }


        protected override void UpdateDocumentIncludeFlow(ProjectItemCodeDocument document){
            document.Project.Solution.ErrorService.ClearAllErrorsFrom(document);
            var directives = document.SegmentService.GetDirectiveSegments();

            // Update the code documents Include Flow.
            if(_codeDocuments.ContainsKey(document)){
                _codeDocuments[document].Clear();
                foreach(var d in directives) {
                    if(d.TokenString.Equals(INCLUDE_ONCE, StringComparison.InvariantCultureIgnoreCase)) {
                        var includedDoc = ParseIncludeDirective(d, document);
                        if(includedDoc != null) {
                            _codeDocuments[document].Add(includedDoc);
                        } else {

                            if(_project.StartUpCodeDocument == null) {
                                RegisterError(document, d, "This Include can't be analyzed because a start Script file is missing. Please define a Startup-Scriptfile!");
                            } else {
                                RegisterError(document, d, "This Include can't be analyzed because the Target it is not member of the current Project.");
                            }
                        }
                    }
                }
            }           
        }


        protected override void UpdateDocumentOrder() {
            lock(_documentCompileOrderLOCK) {
                _documentCompileOrder.Clear();

                var startUpDoc = _codeDocuments.Keys.ToList().Find(x => x.IsStartUpDocument);
                if(startUpDoc != null) {
                    try {
                        _documentCompileOrder.AddRange(_includeTreeBuilder.BuildHirarchy(new Dictionary<ProjectItemCodeDocument, List<ProjectItemCodeDocument>>(_codeDocuments), startUpDoc));
                    } catch(DependencyTreeException e) {
                        RegisterError(e.Document, null, e.Message);
                    }
                    _documentCompileOrder.Add(startUpDoc); // the last file
                }
            }
            base.UpdateDocumentOrder();
        }


        ProjectItemCodeDocument ParseIncludeDirective(CodeSegment includeDirective, ProjectItemCodeDocument document) {
            ThrowUtil.ThrowIfNull(includeDirective);
            ThrowUtil.ThrowIfNull(document);


            SmartCodeProjectAHK project = document.Project as SmartCodeProjectAHK;
            if(project == null)
                throw new NotSupportedException("Expected an instance of SmartCodeProjectAHK!");

            if(_project.StartUpCodeDocument == null)
                return null;


            var libRegEx = new Regex(@"<(.*?)>");

            // parse include directive
            var next = includeDirective.NextOmit(TokenHelper.WhiteSpaces);

            if(next != null) {

                int len = next.TokenString.Length;

                if(len > 0) {

                    if(libRegEx.IsMatch(next.TokenString)) {

                       string docName = libRegEx.Match(next.TokenString).Groups[1].Value;

                        // seach local library files
                        var doc = project.LocalLib.FindAllItems<ProjectItemCodeDocument>().ToList()
                            .Find(x => Path.GetFileNameWithoutExtension(x.FilePath).Equals(docName, StringComparison.InvariantCultureIgnoreCase));

                        if(doc == null && project.StdLib != null) {
                            // seach in standard library files
                            doc = project.StdLib.FindAllItems<ProjectItemCodeDocument>().ToList()
                                .Find(x => x.FilePath != null && Path.GetFileNameWithoutExtension(x.FilePath).Equals(docName, StringComparison.InvariantCultureIgnoreCase));
                        }
                        if(doc != null) {
                            var directive = new IncludeDirective() { ResolvedFilePath = doc.FilePath, ResolvedCodeDocument = doc };
                            next.CodeDOMObject = directive;
                        } else {
                            next.ErrorContext = new CodeError() { Description = string.Format("File not found!") };
                        }
                        return doc;
                    } else {
                        // parse direct/relative path
                        var directive = ParseIncludePath(document, next);
                        var includeFilePath = directive.ResolvedFilePath;

                         var doc = project.CodeDocuments.ToList()
                            .Find(x => x.FilePath.Equals(includeFilePath, StringComparison.InvariantCultureIgnoreCase));
                         directive.ResolvedCodeDocument = doc;
                         return doc;
                    }
                }
            }
            return null;
        }

        static string WORKINGDIR_VAR = "A_ScriptDir";

        IncludeDirective ParseIncludePath(ProjectItemCodeDocument codeDoc, CodeSegment segment) {

            string workingDir = Path.GetDirectoryName(_project.StartUpCodeDocument.FilePath);
            StringBuilder sb = new StringBuilder();

            CodeSegment next = segment;
            CodeSegment current = null;
            List<CodeSegment> _pathTokens = new List<CodeSegment>();
            while(next != null && next.Token != Token.NewLine) {
                current = next;
                next = next.Next;
                if(current.Token == Token.TraditionalString)
                    sb.Append(current.TokenString);
                else if(current.Token == Token.Deref) {
                    if(current.Next != null && current.Next.Token == Token.Identifier) {
                        if(current.Next.TokenString.Equals(WORKINGDIR_VAR, StringComparison.InvariantCultureIgnoreCase)) {
                            sb.Append(workingDir);
                        } else {
                            RegisterError(codeDoc, current.Next, "Unknown precompiler Variable!");
                        }
                        if(current.Next != null && current.Next.Next != null)
                            next = current.Next.Next.Next;
                    } else if(current.Next != null) {
                        RegisterError(codeDoc, current.Next, "Expected Identifier after Deref!");
                    }
                }
                if(current != null)
                    _pathTokens.Add(current);
            }
            var path = sb.ToString();

            try {
                if(!path.Contains(':')) {
                    path = Path.Combine(workingDir, path);
                }
            } catch {
                path = "";
            }

            var directive = new IncludeDirective() { ResolvedFilePath = path };
            CodeError fail = null;
            if(!File.Exists(path)){
                var err = string.Format("File not found!\n" + path);
                fail = new CodeError() { Description = err };
                if(_pathTokens.Any())
                    RegisterError(codeDoc, _pathTokens.First(), err);
            }
            foreach(var s in _pathTokens) {
                s.CodeDOMObject = directive;
                if(fail != null) {
                    s.ErrorContext = fail;
                }
            }
            return directive;
        }

    }
}
