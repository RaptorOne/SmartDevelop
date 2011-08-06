using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace SmartDevelop.Model.DOM.Types
{
    public class CloneHelper
    {

        public static CodeLinePragma Clone(CodeLinePragma pragma) {
            return new CodeLinePragma(pragma.FileName, pragma.LineNumber);
        }

        public static CodeCommentStatementCollection Clone(CodeCommentStatementCollection codestatementcol) {
            var col = new CodeCommentStatementCollection();
            foreach(CodeCommentStatement cstat in codestatementcol) {
                col.Add(Clone(cstat));
            }
            return col;
        }

        public static CodeCommentStatement Clone(CodeCommentStatement codestatementcol) {
            return new CodeCommentStatement()
            {
                Comment = Clone(codestatementcol.Comment)
                //EndDirectives = 
               
            };
        }

        public static CodeComment Clone(CodeComment codestatementcol) {
            return new CodeComment()
            {
                DocComment = codestatementcol.DocComment,
                Text = codestatementcol.Text,
            };
        }

        //CodeAttributeDeclarationCollection

        //public static CodeAttributeDeclarationCollection Clone(CodeAttributeDeclarationCollection attributeCol) {

        //    foreach(CodeAttributeDeclaration attr in attributeCol) {



        //    }

        //    //CodeDirectiveCollection clone = new CodeDirectiveCollection(directivecol);
        //    //return clone;
        //}

        public static CodeAttributeDeclaration Clone(CodeAttributeDeclaration attributeDecl) {
            return new CodeAttributeDeclaration(((CodeTypeReferenceEx)attributeDecl.AttributeType).Clone() as CodeTypeReferenceEx)
            {
                Name = attributeDecl.Name,
            };
        }

        public static CodeDirectiveCollection Clone(CodeDirectiveCollection directivecol) {
            CodeDirectiveCollection clone = new CodeDirectiveCollection(directivecol);
            return clone;
        }

        

    }
}
