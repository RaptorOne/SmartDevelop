using System;
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
    public abstract class CodeDOMService
    {
        #region Fields

        readonly SmartCodeProject _project;
        protected readonly CodeTypeDeclarationEx _languageRoot;
        protected object _languageRootLock = new object();

        protected static List<Token> whitespacetokenNewLinesComments = new List<Token> { Token.WhiteSpace, Token.NewLine, Token.MultiLineComment, Token.SingleLineComment };
        protected static List<Token> whitespacetokenNewLines = new List<Token> { Token.WhiteSpace, Token.NewLine };
        protected static List<Token> whitespacetokens = new List<Token> { Token.WhiteSpace };
        
        protected Dictionary<ProjectItemCodeDocument, CodeRangeManager> CodeRanges = new Dictionary<ProjectItemCodeDocument, CodeRangeManager>();

        #endregion

        /// <summary>
        /// Raised when the CodeDOM (AST) has been updated
        /// </summary>
        public event EventHandler ASTUpdated;

        #region Constructor

        public CodeDOMService(SmartCodeProject project) {
            _languageRoot = new CodeTypeDeclarationEx(null, "Global") { Project = project };
            _project = project;
        }

        #endregion

        #region Properties

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

        public CodeContext GetCodeContext(ProjectItemCodeDocument codeitem, TextLocation location, bool includeCurrentSegment = false) {
            return GetCodeContext(codeitem, codeitem.Document.GetOffset(location));
        }

        public virtual CodeContext GetCodeContext(ProjectItemCodeDocument codeitem, int offset, bool includeCurrentSegment = false) {
            //var context = new CodeContext(this);

            //if(CodeRanges.ContainsKey(codeitem)) {
            //    var ranges = from r in CodeRanges[codeitem].FindEncapsulatingRanges(offset)
            //                 where r.RangedCodeObject is CodeTypeDeclarationEx || r.RangedCodeObject is CodeMemberMethodEx
            //                 select r;

            //    if(ranges.Any()) {
            //        var range = ranges.First();
            //        if(range.RangedCodeObject is CodeMemberMethodEx) {
            //            context.EnclosingMethod = range.RangedCodeObject as CodeMemberMethodEx;
            //            context.EnclosingType = context.EnclosingMethod.DefiningType;
            //        }
            //        if(range.RangedCodeObject is CodeTypeDeclarationEx) {
            //            context.EnclosingType = range.RangedCodeObject as CodeTypeDeclarationEx;
            //        }
            //    }
            //}

            //if(includeCurrentSegment)
            //    context.Segment = codeitem.SegmentService.QueryCodeSegmentAt(offset);

            //if(context.EnclosingType == null)
            //    context.EnclosingType = this.RootType;

            //return context;
            return CodeContext.Empty;
        }

        #endregion

        #region File Compiler

        /// <summary>
        /// Compiles the codeitem to parse it async
        /// </summary>
        /// <param name="codeitem"></param>
        /// <param name="initialparent"></param>
        public abstract void CompileTokenFileAsync(ProjectItemCodeDocument codeitem, CodeTypeDeclarationEx initialparent);

        public abstract bool IsBusy { get; }

        #endregion

        #region Event Handlers

        protected virtual void OnASTUpdated(){
            if(ASTUpdated != null)
                ASTUpdated(this, EventArgs.Empty);
        }

        #endregion

        public abstract void EnsureIsUpdated();
    }
 
}
