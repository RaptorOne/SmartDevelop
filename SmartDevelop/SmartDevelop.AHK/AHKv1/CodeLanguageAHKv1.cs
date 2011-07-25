using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.CodeLanguages;
using ICSharpCode.AvalonEdit.Document;
using SmartDevelop.AHK.AHKv1.Tokenizing;
using SmartDevelop.Model.DOM;
using ICSharpCode.AvalonEdit.Highlighting;
using System.CodeDom;
using SmartDevelop.Model.DOM.Types;
using SmartDevelop.AHK.AHKv1.DOM.Types;

namespace SmartDevelop.AHK.AHKv1
{
    public class CodeLanguageAHKv1 : CodeLanguage
    {
        public CodeLanguageAHKv1() 
            : base("ahk-v1.1")
        {
            // todo
            // those data is actually thougt to be read out of confic files
            //

            this.LanguageKeywords.AddRange(new CodeKeyWord[]
                { 
                    new CodeKeyWord("if"), new CodeKeyWord("else"),
                    new CodeKeyWord("class"), new CodeKeyWord("var"), new CodeKeyWord("new"), new CodeKeyWord("this"),new CodeKeyWord("base"), new CodeKeyWord("extends"),
                    new CodeKeyWord("return"), new CodeKeyWord("break"), new CodeKeyWord("continue"),
                    new CodeKeyWord("global"), new CodeKeyWord("static"), new CodeKeyWord("local"),
                    new CodeKeyWord("loop"), new CodeKeyWord("for"), new CodeKeyWord("while")
                });

            
            BuildInMembers.AddRange(CodeLanguageAHKBuildinMethods.ReadMembers());
            
        }

        static readonly string[] _extensions = { ".ahk" };

        public override string[] Extensions {
            get {
                return _extensions;
            }
        }

        public override TokenizerBase.Tokenizer CreateTokenizer(ITextSource source) {
            return new SimpleTokinizerIA(source, this);
        }

        public override Model.DOM.CodeDOMService CreateDOMService(Model.Projecting.SmartCodeProject codeProject) {
            return new CodeDOMServiceIA(codeProject);
        }

        public override ICSharpCode.AvalonEdit.Folding.AbstractFoldingStrategy CreateFoldingStrategy() {
            throw new NotImplementedException();
        }
    }
}
