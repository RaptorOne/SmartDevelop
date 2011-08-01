﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Tokening;
using System.CodeDom;
using SmartDevelop.Model.DOM.Types;
using SmartDevelop.Model.CodeContexts;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.Model.DOM.Ranges;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.Model.DOM
{
    /// <summary>
    /// Service to compile tokenized RAW Data into CodeDOM Representation and offers several CodeDOM based Querys
    /// </summary>
    public abstract class CodeDocumentDOMService
    {
        #region Fields

        protected readonly ProjectItemCodeDocument _document;
        protected readonly SmartCodeProject _project;

        protected readonly CodeTypeDeclarationEx _languageRoot;
        protected object _languageRootLock = new object();

        protected static List<Token> whitespacetokenNewLinesComments = new List<Token> { Token.WhiteSpace, Token.NewLine, Token.MultiLineComment, Token.SingleLineComment };
        protected static List<Token> whitespacetokenNewLines = new List<Token> { Token.WhiteSpace, Token.NewLine };
        protected static List<Token> whitespacetokens = new List<Token> { Token.WhiteSpace };
        
        //protected Dictionary<ProjectItemCodeDocument, CodeRangeManager> _codeRanges = new Dictionary<ProjectItemCodeDocument, CodeRangeManager>();
        protected CodeRangeManager _codeRangeManager = new CodeRangeManager();

        #endregion

        #region Events

        /// <summary>
        /// Raised when the CodeDOM (AST) has been updated
        /// </summary>
        public event EventHandler Updated;

        //public event EventHandler 

        #endregion

        #region Constructor

        public CodeDocumentDOMService(ProjectItemCodeDocument document) {
            _languageRoot = new CodeTypeDeclarationEx(null, "Global") { Project = document.Project };
            _document = document;
            _project = document.Project;
        }

        #endregion

        #region Properties


        protected CodeDocumentDOMService _depedingOn;
        protected object _depedingOnLock = new object();



        /// <summary>
        /// Gets Sets the Service on which one this one depends.
        /// </summary>
        public CodeDocumentDOMService DependingOn {
            get { lock(_depedingOnLock) { return _depedingOn; } }
            set {
                lock(_depedingOnLock) {
                    var old = _depedingOn;
                    _depedingOn = value;
                    OnDependingOnChanged(old, value);
                }
            }
        }

        /// <summary>
        /// Occurs when the DependingOn Property has changed
        /// Subclasses can overwrite this Method to add behaviour 
        /// </summary>
        /// <param name="newPostDOMService"></param>
        protected virtual void OnDependingOnChanged(CodeDocumentDOMService old, CodeDocumentDOMService newPostDOMService) {
            if(old != null)
                old.Updated -= OnDependingOnASTUpdated;
            if(newPostDOMService != null)
                newPostDOMService.Updated += OnDependingOnASTUpdated;
        }

        /// <summary>
        /// Occurs when the DependingOn-CodeDocumentDOMService has Updated its AST
        /// Subclasses can overwrite this Method to add behaviour 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnDependingOnASTUpdated(object sender, EventArgs e) {

        }



        /// <summary>
        /// Accessing the RootType will lock it for all other threads
        /// </summary>
        public CodeTypeDeclarationEx RootType {
            get { lock(_languageRootLock) { return _languageRoot; } }
        }

        /// <summary>
        /// Unsave access the RootType
        /// </summary>
        public CodeTypeDeclarationEx RootTypeUnSave {
            get {  return _languageRoot; }
        }

        /// <summary>
        /// Gets a immutable snapshot of the current roottype
        /// </summary>
        /// <returns></returns>
        public abstract CodeTypeDeclarationEx GetRootTypeSnapshot();

        public SmartCodeProject CodeProject {
            get { return _project; }
        }


        #endregion

        #region Public Methods

        public CodeContext GetCodeContext(TextLocation location, bool includeCurrentSegment = false) {
            return GetCodeContext(_document.Document.GetOffset(location));
        }

        public virtual CodeContext GetCodeContext(int offset, bool includeCurrentSegment = false) {
            var context = new CodeContext(this);

            var ranges = from r in _codeRangeManager.FindEncapsulatingRanges(offset)
                            where r.RangedCodeObject is CodeTypeDeclarationEx || r.RangedCodeObject is CodeMemberMethodEx
                            select r;

            if(ranges.Any()) {
                var range = ranges.First();
                if(range.RangedCodeObject is CodeMemberMethodEx) {
                    context.EnclosingMethod = range.RangedCodeObject as CodeMemberMethodEx;
                    context.EnclosingType = context.EnclosingMethod.DefiningType;
                }
                if(range.RangedCodeObject is CodeTypeDeclarationEx) {
                    context.EnclosingType = range.RangedCodeObject as CodeTypeDeclarationEx;
                }
            }

            if(includeCurrentSegment)
                context.Segment = _document.SegmentService.QueryCodeSegmentAt(offset);

            if(context.EnclosingType == null)
                context.EnclosingType = this.RootType;

            return context;
        }

        #endregion

        #region File Compiler

        /// <summary>
        /// Compiles the codeitem to parse it async
        /// </summary>
        /// <param name="codeitem"></param>
        /// <param name="initialparent"></param>
        public abstract void CompileTokenFileAsync();

        public abstract bool IsBusy { get; protected set; }

        #endregion

        #region Event Handlers

        protected virtual void OnASTUpdated(){
            if(Updated != null)
                Updated(this, EventArgs.Empty);
        }

        #endregion

        public abstract bool WaitUntilUpdated(int timeout);
    }
 
}
