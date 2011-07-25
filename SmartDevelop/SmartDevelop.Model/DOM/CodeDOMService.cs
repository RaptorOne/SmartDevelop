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

namespace SmartDevelop.Model.DOM
{
    /// <summary>
    /// Service to compile tokenized RAW Data into CodeDOM Representation and offers several CodeDOM based Querys
    /// </summary>
    public abstract class CodeDOMService
    {
        #region Fields

        SmartCodeProject _project;
        CodeTypeDeclarationEx _scriptRoot;
        protected static List<Token> whitespacetokenNewLines = new List<Token> { Token.WhiteSpace, Token.NewLine };
        protected static List<Token> whitespacetokens = new List<Token> { Token.WhiteSpace };
        
        protected Dictionary<ProjectItemCode, CodeRangeManager> CodeRanges = new Dictionary<ProjectItemCode, CodeRangeManager>();

        #endregion

        #region Constructor

        public CodeDOMService(SmartCodeProject project) {
            _scriptRoot = new CodeTypeDeclarationEx("Global");
            _project = project;
        }

        #endregion

        #region Properties

        public CodeTypeDeclarationEx RootType {
            get { return _scriptRoot; }
        }

        public SmartCodeProject CodeProject {
            get { return _project; }
        }


        #endregion

        #region Public Methods

        IEnumerable<CodeMemberMethod> CollectAllMembersBy(string filepath) {
            List<CodeMemberMethod> methods;

            methods = (from CodeTypeMember m in RootType.Members
                       where m is CodeMemberMethod && m.LinePragma.FileName == filepath
                       select m as CodeMemberMethod).ToList();
            
             // todo look up class methods
            return methods;
        }

        

        public CodeContext GetCodeContext(ProjectItemCode codeitem, TextLocation location, bool includeCurrentSegment = false) {
            return GetCodeContext(codeitem, codeitem.Document.GetOffset(location));
        }

        public virtual CodeContext GetCodeContext(ProjectItemCode codeitem, int offset, bool includeCurrentSegment = false) {
            var context = new CodeContext(this);

            if(CodeRanges.ContainsKey(codeitem)) {
                var ranges = from r in CodeRanges[codeitem].FindEncapsulatingRanges(offset)
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
            }

            if(includeCurrentSegment)
                context.Segment = codeitem.SegmentService.QueryCodeSegmentAt(offset);

            if(context.EnclosingType == null)
                context.EnclosingType = this.RootType;

            return context;
        }

        #endregion

        #region File Compiler

        public abstract void CompileTokenFile(ProjectItemCode codeitem, CodeTypeDeclarationEx initialparent);

        #endregion
    }





        
}
