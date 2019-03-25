using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLL
{
    partial class CodeParser
    {
        IIntResult ToIIntResult(string operand)
        {
            int val;

            if (operand.ToUpper().Equals("FALSE"))
            {
                return new Constant(0);
            }
            else if (operand.ToUpper().Equals("TRUE"))
            {
                return new Constant(1);
            }
            else if (Int32.TryParse(operand, out val))
            {
                return new Constant(val);
            }
            else if (operand[0] == '\"')
            {
                //unwrap quotes
                string str = operand.Substring(1, operand.Length - 2).Trim();
                return new Constant(game.GetStringId(str));
            }
            else if (game.GetObjectId(operand) != -1)
            {
                return new Constant(game.GetObjectId(operand));
            }
            else if (game.IsVariable(operand))
            {
                return new VariableRVal(operand);
            }
            else if (operand.IndexOf('.') != -1)
            {
                string[] parts = operand.Split('.');

                if (game.GetObjectId(parts[0]) == -1 && !game.IsVariable(parts[0]))
                    throw new Exception(parts[0] + " is not a valid object or variable.");

                if (IsAttr(parts[1]))
                    return new AttributeRVal(parts[0], ToIIntResult(parts[0]), parts[1]);

                if (IsProp(parts[1]))
                    return new PropertyRVal(parts[0], ToIIntResult(parts[0]), parts[1]);

                throw new Exception(parts[1] + " is not a valid attr.");


            }
            else if (operand.ToUpper().StartsWith("RAND"))
            {
                try
                {
                    string inner = operand.Substring(operand.IndexOf('(') + 1);
                    int last = inner.LastIndexOf(')');
                    inner = inner.Substring(0, last).Trim();
                    Rand r = new Rand(BuildExprTree(new StringBuilder(inner)));
                    return r;
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to parse " + operand, e);
                }
            }
            
            throw new Exception("unknown object/variable " + operand );
        } 


       

        
        

        BinaryOperator ToRelationalOperator(string op)
        {
            if (op == "==")
                return new Compare();
            if (op == "!=")
                return new NotEquals();
            if (op == ">=")
                return new GreaterThanEquals();
            if (op == "<=")
                return new LessThanEquals();
            if (op == ">")
                return new GreaterThan();
            if (op == "<")
                return new LessThan();
            if (op == "||")
                return new Or();
            if (op == "&&")
                return new And();
            if (op == "has")
                return new Has();

            throw new Exception("Unable to convert " + op + " to a relational operator");
        }
        

        /// <summary>
        /// Returns the precendence of op
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        int GetPrecedence(string op)
        {
            if (op == "!")
                return BinaryOperator.PRECEDENCE_NOT;
            if (op == "*")
                return BinaryOperator.PRECEDENCE_MULT;
            if (op == "++" || op == "--")
                return BinaryOperator.PRECEDENCE_PREINC;
            if (op == "+" || op == "-")
                return BinaryOperator.PRECEDENCE_PLUS_MINUS;
            if (op == "<=" || op == ">=" || op == "<" || op == ">")
                return BinaryOperator.PRECEDENCE_LT_GT_LTE_GTE;
            if (op == "==" || op == "!=")
                return BinaryOperator.PRECEDENCE_EQ_NE;
            if (op == "&&")
                return BinaryOperator.PRECEDENCE_AND;
            if (op == "||")
                return BinaryOperator.PRECEDENCE_OR;
            if (op == "+=" || op == "-=")
                return BinaryOperator.PRECEDENCE_PLUSEQ_MINUSEQ;
            throw new Exception("unable to get precedence for " + op);
        }

        public static BinaryOperator ToBinaryOp (string nodeText)
        {
            if (nodeText == "==")
                return new Compare();
            if (nodeText == "!=")
                return new NotEquals();
            if (nodeText == ">=")
                return new GreaterThanEquals();
            if (nodeText == "<")
                return new LessThan();
            if (nodeText == ">")
                return new GreaterThan();
            if (nodeText == "<=")
                return new LessThanEquals();
            if (nodeText == "+")
                return new Plus();
            if (nodeText == "-")
                return new Minus();
            if (nodeText == "*")
                return new Mult();
            if (nodeText == "||")
                return new Or();
            if (nodeText == "&&")
                return new And();
            if (nodeText == "has")
                return new Has();
            throw new Exception("Unable to convert " + nodeText + " to a binary operator");


        }

        /// <summary>
        ///Assumes the first matching paren has been popped off
        //the closing ) is not returned 
        /// </summary>
        /// <param name="toks"></param>
        /// <returns></returns>
        List<string> PopToMatchingParen(List<string> toks)
        {
            List<string> expr = new List<string>();
            int count = 1;
            while (toks.Count > 0)
            {
                String s = toks.Pop();

                if (s == "(")
                {
                    count++;
                }
                else if (s == ")")
                {
                    count--;
                    if (count == 0)
                    {
                        return expr;
                    }
                }
                expr.Add(s);
            }
            throw new Exception("missing )");
        }
    }

}
