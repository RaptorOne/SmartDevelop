using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.TokenizerBase;
using SmartDevelop.Model.Tokenizing;

namespace SmartDevelop.Model.DOM
{
    /// <summary>
    /// The Shunting Yard Algorithm was invented by Edsger Dijkstra around 1960 in connection with one of the first Algol compilers.
    /// http://www.engr.mun.ca/~theo/Misc/exp_parsing.htm
    /// </summary>
    public class ShuntingYardExpressionParser
    {
        CodeSegment _current;
        Stack<CodeSegment> _operands = new Stack<CodeSegment>();
        Stack<CodeSegment> _operators = new Stack<CodeSegment>(); 

        public ShuntingYardExpressionParser() { }


        public void Parse(CodeSegment start){
            // init
            _current = start;
            _operands.Clear();
            _operators.Clear();

            _operators.Push(Sentinel);

            while(true) {
                _operands.Push(_current);
            }
        }

        static CodeSegment Sentinel = new CodeSegment();

        Token Next() {
            return _current.Next.Token;
        }

        void Expect(Token tok) {
            if(Next() == tok)
                Consume();
            else
                Error();
        }

        void Consume() {
            _current = _current.Next;
        }

        void Error() {

        }

    }
}
