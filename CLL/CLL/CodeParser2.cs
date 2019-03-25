using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLL
{
    partial class CodeParser
    {  
        /// <summary>
        /// Returns true if the binary operator
        /// </summary>
        /// <param name="op"></param>
        /// <param name="minP"></param>
        /// <returns></returns>
        bool IsBinaryOpWithPrecendence(string op, int minP)
        {
            return (IsBinaryOperator(op) && GetPrecedence(op) > minP);

        }

        /// <summary>
        /// Public function for turning code into a tree
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IIntResult BuildExprTree(StringBuilder code)
        {
            string s = code.ToString();
            try
            {
                List<string> toks = TokenizerClass.Tokenize(code);
                return PrecedenceClimb(toks, 0);
            }
            catch (Exception e)
            {
                throw new Exception("Expression tree builder unable to parse code " + s, e);
            }
            
        }

        /// <summary>
        /// Private method for building an integer 
        /// expression tree
        /// </summary>
        /// <param name="toks"></param>
        /// <param name="minPrec"></param>
        /// <returns></returns>
        IIntResult PrecedenceClimb(List<string> toks, int minPrec)
        {
            IIntResult atom_lhs = FindAtom(toks);

            BinaryOperator lhs = null;
            
            while (toks.Count > 0 &&
                    IsBinaryOperator(toks.First()) &&
                   (ToBinaryOp(toks.First()).Precedence >= minPrec)) 
            {
                string optxt = toks.First();
                toks.Pop(); //remove operator

                BinaryOperator op = ToBinaryOp(optxt);
                int prec = op.Precedence;
                int next_min_prec = prec + 1;

                Console.WriteLine("operator=" + optxt);

                IIntResult rhs = PrecedenceClimb(toks, next_min_prec);
                op.Right = rhs;
                if (lhs == null)
                    op.Left = atom_lhs;
                else
                    op.Left = lhs;
                lhs = op;
            }

            if (lhs == null)
               return atom_lhs;
            return lhs;       
        }

        /// <summary>
        /// Atoms are constants, vars, and rand 
        /// parenthesized expression are sub parsed
        /// </summary>
        /// <param name="toks"></param>
        /// <returns></returns>
        IIntResult FindAtom(List<string> toks)
        {
            string s = toks.Pop();
            if (s[0] == '(')
            {
                List<string> expr = PopToMatchingParen(toks);
                return PrecedenceClimb(expr, 0);
            }
            else
            {
                return ToIIntResult(s);
            }
        }

    }

    static class ListExtensions
    {
        /// <summary>
        /// Returns the first item and removes it from the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Pop(this List<string> list)
        {
            string s = list.First();
            list.RemoveAt(0);
            return s;
        }
        
    }
     
}
