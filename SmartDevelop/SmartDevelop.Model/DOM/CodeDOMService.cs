using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Tokening;
using System.CodeDom;

namespace SmartDevelop.Model.DOM
{
    /// <summary>
    /// Service to compile tokenized RAW Data into CodeDOM Representation
    /// </summary>
    public abstract class CodeDOMService
    {
        #region Fields

        SmartCodeProject _project;
        CodeTypeDeclaration _scriptRoot;
        protected static List<Token> whitespacetoken = new List<Token> { Token.WhiteSpace, Token.NewLine };
        
        #endregion

        #region Constructor

        public CodeDOMService(SmartCodeProject project) {
            _scriptRoot = new CodeTypeDeclaration("Global");
            _project = project;
        }

        #endregion

        #region Properties

        public CodeTypeDeclaration RootType {
            get { return _scriptRoot; }
        }

        public SmartCodeProject CodeProject {
            get { return _project; }
        }


        #endregion

        IEnumerable<CodeMemberMethod> CollectAllMembersBy(string filepath) {
            List<CodeMemberMethod> methods;

            methods = (from CodeTypeMember m in RootType.Members
                       where m is CodeMemberMethod && m.LinePragma.FileName == filepath
                       select m as CodeMemberMethod).ToList();
            
             // todo look up class methods
            return methods;
        }

        #region File Compiler

        public abstract void CompileTokenFile(ProjectItemCode codeitem, CodeTypeDeclaration initialparent); 
        

        #endregion


    }

        
}
