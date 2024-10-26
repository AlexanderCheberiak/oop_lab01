using Antlr4.Runtime;
using System;
using System.Collections.Generic;

namespace Lab01
{
    public static class Calculator
    {
        public static double Evaluate(string expression, Dictionary<string, double> cellValues)
        {
            var lexer = new Lab01Lexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new Lab01Parser(tokens);

            var tree = parser.compileUnit();

            var visitor = new Lab01Visitor(cellValues);

            return visitor.Visit(tree);
        }
    }
}
