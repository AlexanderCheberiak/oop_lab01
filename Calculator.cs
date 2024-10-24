using Antlr4.Runtime;
using Lab01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab01
{
    public static class Calculator
    {
        public static double Evaluate(string expression)
        {
            var lexer = new Lab01Lexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new Lab01Parser(tokens);

            var tree = parser.compileUnit();

            var visitor = new Lab01Visitor();

            return visitor.Visit(tree);
        }
    }
}
