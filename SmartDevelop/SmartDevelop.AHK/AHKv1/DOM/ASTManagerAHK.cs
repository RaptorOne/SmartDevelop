using System;
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

namespace SmartDevelop.AHK.AHKv1.DOM
{
    public class ASTManagerAHK : ASTManager
    {
        const string INCLUDE_ONCE = "#Include";

        public ASTManagerAHK(SmartCodeProject project) 
            : base(project) { }


        protected override void OnCodeDocumentTokenizerUpdated(object sender, EventArgs e) {
            var document = sender as ProjectItemCodeDocument;
            var directives = document.SegmentService.GetDirectiveSegments();

            // Update the code documents Include Flow.
            if(_codeDocuments.ContainsKey(document)){
                _codeDocuments[document].Clear();
                foreach(var d in directives) {
                    if(d.TokenString.Equals(INCLUDE_ONCE, StringComparison.InvariantCultureIgnoreCase)) {
                        var includedDoc = ParseIncludeDirective(d, document);
                        if(includedDoc != null) {
                            _codeDocuments[document].Add(includedDoc);
                        }
                    }
                }
            }
            UpdateDocumentOrder();
        }

        protected override void UpdateDocumentOrder() {
            lock(_documentCompileOrderLOCK) {
                _documentCompileOrder.Clear();

                var startUpDoc = _codeDocuments.Keys.ToList().Find(x => x.IsStartUpDocument);
                if(startUpDoc != null) {
                    _documentCompileOrder.Add(startUpDoc);
                    _documentCompileOrder.AddRange(GenerateDependcyHirarchy(startUpDoc));
                }
            }
            _documentCompileOrder.Reverse(); // we have generated the list Top-Down, reverse it to get the compiler flow
            base.UpdateDocumentOrder();
        }


        List<ProjectItemCodeDocument> GenerateDependcyHirarchy(ProjectItemCodeDocument document) {
            List<ProjectItemCodeDocument> _hirarchy = new List<ProjectItemCodeDocument>();
            foreach(var doc in _codeDocuments[document]) {
                _hirarchy.AddRange(GenerateDependcyHirarchy(doc));
                _hirarchy.Add(doc);
            }
            return _hirarchy;
        }




        ProjectItemCodeDocument ParseIncludeDirective(CodeSegment includeDirective, ProjectItemCodeDocument document) {
            ThrowUtil.ThrowIfNull(includeDirective);
            ThrowUtil.ThrowIfNull(document);


            SmartCodeProjectAHK project = document.Project as SmartCodeProjectAHK;

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
                            .Find(x => Path.GetFileNameWithoutExtension(x.FilePath).Equals(docName));

                        if(doc == null) {
                            // seach in standard library files
                            doc = project.StdLib.FindAllItems<ProjectItemCodeDocument>().ToList()
                                .Find(x => Path.GetFileNameWithoutExtension(x.FilePath).Equals(docName));
                        }
                        if(doc != null) {
                            var directive = new SmartDevelop.Model.CodeLanguages.PreProcessorDirective() { ResolvedFilePath = doc.FilePath };
                            next.CodeDOMObject = directive;
                        } else {
                            next.ErrorContext = new CodeError() { Description = string.Format("File not found!") };
                        }
                        return doc;
                    } else {
                        // parse direct/relative path

                        var includeFilePath = ParseIncludePath(document, next);
                         return project.CodeDocuments.ToList()
                            .Find(x => x.FilePath.Equals(includeFilePath, StringComparison.InvariantCultureIgnoreCase));
                    }
                }
            }
            return null;
        }
        static string WORKINGDIR_VAR = "A_ScriptDir";
        
        string ParseIncludePath(ProjectItemCodeDocument codeDoc, CodeSegment segment) {
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
                        if(current.Next.Equals(WORKINGDIR_VAR)) {
                            sb.Append(workingDir);
                        } else {
                            RegisterError(codeDoc, current.Next, "Unknown precompiler Variable!");
                        }
                    } else if(current.Next != null) {
                        RegisterError(codeDoc, current.Next, "Expected Identifier after Deref!");
                    }
                }
                if(current != null)
                    _pathTokens.Add(current);
            }
            var path = sb.ToString();

            if(!path.Contains(':')) {
                path = Path.Combine(workingDir, path);
            }

            var directive = new PreProcessorDirective() { ResolvedFilePath = path };
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
            return path;
        }

    }
}
