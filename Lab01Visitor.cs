using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lab01
{
    class Lab01Visitor : Lab01BaseVisitor<double>
    {
        // Таблица значений ячеек
        private readonly Dictionary<string, double> tableIdentifier;

        public Lab01Visitor(Dictionary<string, double> tableIdentifier)
        {
            this.tableIdentifier = tableIdentifier;
        }

        public override double VisitCompileUnit(Lab01Parser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(Lab01Parser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText());
            Debug.WriteLine(result);

            return result;
        }

        // Обработка выражений с адресами клеток, такими как A1, B2
        public override double VisitCellAddressExpr(Lab01Parser.CellAddressExprContext context)
        {
            var cellAddress = context.GetText(); // Получаем адрес ячейки как строку, например "A1"

            if (tableIdentifier.TryGetValue(cellAddress, out double value))
            {
                return value; // Возвращаем значение, если адрес существует в словаре
            }
            else
            {
                throw new Exception($"Адрес ячейки '{cellAddress}' не найден");
            }
        }



        // IdentifierExpr
        public override double VisitIdentifierExpr(Lab01Parser.IdentifierExprContext context)
        {
            var identifier = context.GetText();
            return tableIdentifier.TryGetValue(identifier, out double value) ? value : 0.0;
        }

        public override double VisitParenthesizedExpr(Lab01Parser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitExponentialExpr(Lab01Parser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} ^ {1}", left, right);
            return Math.Pow(left, right);
        }

        public override double VisitAdditiveExpr(Lab01Parser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == Lab01Lexer.ADD)
            {
                Debug.WriteLine("{0} + {1}", left, right);
                return left + right;
            }
            else
            {
                Debug.WriteLine("{0} - {1}", left, right);
                return left - right;
            }
        }

        public override double VisitMultiplicativeExpr(Lab01Parser.MultiplicativeExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            switch (context.operatorToken.Type)
            {
                case Lab01Lexer.MULTIPLY:
                    Debug.WriteLine("{0} * {1}", left, right);
                    return left * right;

                case Lab01Lexer.DIVIDE:
                    Debug.WriteLine("{0} / {1}", left, right);
                    return left / right;

                case Lab01Lexer.DIV:
                    Debug.WriteLine("{0} div {1}", left, right);
                    return (int)left / (int)right; // Целочисленное деление

                case Lab01Lexer.MOD:
                    Debug.WriteLine("{0} mod {1}", left, right);
                    return left % right; // Остаток от деления

                default:
                    throw new Exception("Невідомий оператор у виразі.");
            }
        }


        private double WalkLeft(Lab01Parser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<Lab01Parser.ExpressionContext>(0));
        }

        private double WalkRight(Lab01Parser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<Lab01Parser.ExpressionContext>(1));
        }
    }
}
