using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lab01
{
    class Lab01Visitor : Lab01BaseVisitor<double>
    {
        // Таблиця значень комірок
        private readonly Dictionary<string, double> tableIdentifier;

        public Lab01Visitor(Dictionary<string, double> tableIdentifier)
        {
            this.tableIdentifier = tableIdentifier;
        }

        public override double VisitCompileUnit(Lab01Parser.CompileUnitContext context)
        {
            try { return Visit(context.expression()); }
            catch (System.DivideByZeroException) { throw new System.DivideByZeroException("Divide by 0"); }
            catch { throw new Exception("Комірка не знайдена"); }
        }

        public override double VisitNumberExpr(Lab01Parser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText());
            Debug.WriteLine(result);

            return result;
        }

        // Обробка виразів з адресами клітин, такими як A1, B2
        public override double VisitCellAddressExpr(Lab01Parser.CellAddressExprContext context)
        {
            var cellAddress = context.GetText(); // Отримуємо адресу комірки як рядок, наприклад «A1»

            if (tableIdentifier.TryGetValue(cellAddress, out double value))
            {
                return value; // Повертаємо значення, якщо адреса існує в словнику
            }
            else
            {
                throw new Exception("Комірка не знайдена");
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

        //Логічне заперечення
        public override double VisitNotExpr(Lab01Parser.NotExprContext context)
        {
            var value = Visit(context.expression());

            return value == 0 ? 1.0 : 0.0;
        }

        //Порівняння <, >, =
        public override double VisitComparisonExpr(Lab01Parser.ComparisonExprContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            switch (context.operatorToken.Type)
            {
                case Lab01Lexer.EQUAL:
                    return left == right ? 1.0 : 0.0;
                case Lab01Lexer.LESS:
                    return left < right ? 1.0 : 0.0;
                case Lab01Lexer.GREATER:
                    return left > right ? 1.0 : 0.0;
                default:
                    throw new InvalidOperationException("Невідомий оператор порівняння.");
            }
        }

        //Степінь
        public override double VisitExponentialExpr(Lab01Parser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} ^ {1}", left, right);
            return Math.Pow(left, right);
        }

        //Додавання
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

        //Множення
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
                    if (right == 0) { throw new DivideByZeroException("Divide by 0"); };
                    return left / right;

                case Lab01Lexer.DIV:
                    Debug.WriteLine("{0} div {1}", left, right);
                    if (right == 0) { throw new DivideByZeroException("Divide by 0"); };
                    return (int)left / (int)right; 

                case Lab01Lexer.MOD:
                    Debug.WriteLine("{0} mod {1}", left, right);
                    if (right == 0) { throw new DivideByZeroException("Divide by 0"); };
                    return left % right; 

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
