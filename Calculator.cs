using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lab01
{
    public static class Calculator
    {
        public static string Evaluate(string expression, Dictionary<string, double> cellValues)
        {
            var lexer = new Lab01Lexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new Lab01Parser(tokens);

            var tree = parser.compileUnit();

            var visitor = new Lab01Visitor(cellValues);

            try { return visitor.Visit(tree).ToString(); }
            catch (System.DivideByZeroException) { return "Помилка: ділення на 0"; }
            catch { return "Помилка обчислення "; }
            }
    }
}
