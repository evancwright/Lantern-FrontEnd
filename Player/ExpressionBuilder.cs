using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class ExpressionBuilder
    {
        public static Expression CreateExpr(string code)
        {
            
            code = Unwrap(code);

            Expression ex = new Expression();
            ex.text = code;

            ex.op = new Opr(GetOperator(code));

            string lhs = code.Substring(0, code.IndexOf(ex.op.txt));
            int rhsStart = code.IndexOf(ex.op.txt)+ex.op.txt.Length;
            string rhs = code.Substring(rhsStart, code.Length - rhsStart);

            ex.lhs = Node.ToNode(lhs);
            ex.rhs = Node.ToNode(rhs);
            return ex;
        }

        public static string GetOperator(string code)
        {
            string[] ops = { "==", "!=", "<", ">" };

            for (int i = 0; i < ops.Length; i++)
            {
                if (code.IndexOf(ops[i]) != -1)
                {
                    return ops[i];
                }
            }

            throw new Exception("Invalid relational operator: " + code);
        }

        public static string Unwrap(string code)
        {
            code = code.Trim();
            if (code.Length > 0)
            {
                if (code[0] == '(')
                {
                    if (code[code.Length - 1] != ')')
                        throw new Exception("Missuing ) near " + code);

                    code = code.Substring(1, code.Length - 2).Trim();

                }
            }

            return code;
        }

    }
}
