﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.Model.DOM
{


    
    /// <summary>
    /// This class manages all AST's of a project and ensures that they are connected in the right Order
    /// </summary>
    public abstract class ASTManager
    {
        #region Fields

        protected Dictionary<ProjectItemCodeDocument, List<ProjectItemCodeDocument>> _codeDocuments = new Dictionary<ProjectItemCodeDocument, List<ProjectItemCodeDocument>>();

        protected List<ProjectItemCodeDocument> _documentCompileOrder = new List<ProjectItemCodeDocument>();
        protected object _documentCompileOrderLOCK = new object();

        protected SmartCodeProject _project;

        #endregion

        public ASTManager(SmartCodeProject project) { _project = project; }

        #region Document Handling

        public void Add(ProjectItemCodeDocument document) {
            if(!_codeDocuments.ContainsKey(document)) {
                _codeDocuments.Add(document, new List<ProjectItemCodeDocument>());
                OnCodeDocumentAdded(document);
            } 
        }

        public void Remove(ProjectItemCodeDocument document) {
            _codeDocuments.Remove(document);
            OnCodeDocumentRemoved(document);
        }

        #region Event Handlers

        protected virtual void OnCodeDocumentAdded(ProjectItemCodeDocument document) {
            document.TokenizerUpdated += OnCodeDocumentTokenizerUpdated;
        }

        protected virtual void OnCodeDocumentRemoved(ProjectItemCodeDocument document) {
            document.TokenizerUpdated -= OnCodeDocumentTokenizerUpdated;
        }

        #endregion

        #endregion


        public void UpdateFullAST() {
            lock(_documentCompileOrderLOCK) {
                if(_documentCompileOrder.Any())
                    _documentCompileOrder.First().AST.CompileTokenFileAsync(); // in updating the first item all depending will update itself
            }
        }

        /// <summary>
        /// This Method gets called when anything in _codeDocuments List has changed and will update
        /// the _documentCompileOrder List:
        /// </summary>
        protected virtual void UpdateDocumentOrder() {
            OnDocumentCompileOrderChanged();
        }

        #region Event Handlers

        protected virtual void OnCodeDocumentTokenizerUpdated(object sender, EventArgs e) {
            var doc = sender as ProjectItemCodeDocument;
            doc.Project.Solution.ErrorService.ClearAllErrorsFrom(doc);
        }

        /// <summary>
        /// Occurs when the DocumentCompilerOrder has changed
        /// </summary>
        protected virtual void OnDocumentCompileOrderChanged() {
            // Ensure that the DOM Compilers are conected in the right order
            // Updates all depending-on properties which have changed
            ProjectItemCodeDocument prev = null;
            foreach(var doc in _documentCompileOrder) {
                if(prev == null) {
                    if(doc.AST.DependingOn != null)
                        doc.AST.DependingOn = null;
                } else if(doc.AST.DependingOn != prev.AST) {
                    doc.AST.DependingOn = prev.AST;
                }
                prev = doc;
            }
        }

        #endregion

        protected void RegisterError(Projecting.ProjectItemCodeDocument codeitem, CodeSegment segment, string errorDescription) {
            var errorService = codeitem.Project.Solution.ErrorService;
            var err = new CodeError() { Description = errorDescription };
            if(segment != null) {
                segment.ErrorContext = err;
                errorService.Add(new Errors.ErrorItem(segment, codeitem));
            } else {
                errorService.Add(new Errors.ErrorItem(err, codeitem));
            }
        }

    }
}


        