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
        /// Assumes code starts with a (
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        StringBuilder ReadToClosingParen(StringBuilder code)
        {
            int count = 1;
            int index = 1;
            string s = code.ToString();
            try
            {
                while (true)
                {
                    if (s[index] == '(')
                        count++;
                    if (s[index] == ')')
                        count--;

                    if (count == 0)
                        break;

                    index++;
                }
            }
            catch
            {
                throw new Exception("Missing ) near " + s);
            }
            //trim the expression out of original buffer
            string remainder = s.Substring(index + 1).Trim();
            code.Clear();
            code.Insert(0, remainder);

            //return the expression
            string inner = s.Substring(1, index - 1).Trim();
            StringBuilder cond = new StringBuilder(inner);
            return cond;

        }

        /// <summary>
        /// Returns up to the closing }
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StringBuilder ReadToMatchingCurly(StringBuilder code)
        {
            int count = 1;
            int index = 1;
            string s = code.ToString();

            while (true)
            {
                try
                {
                    if (s[index] == '{')
                        count++;
                    else if (s[index] == '}')
                        count--;

                    if (count == 0)
                        break;

                    index++;
                }
                catch
                {
                    throw new Exception("Unable to find closing } near " + code);
                }
            }//end while

            string leading = s.Substring(1, index-1);
            string remainder = s.Substring(index + 1).Trim();

            code.Clear();
            code.Insert(0, remainder);

            return new StringBuilder(leading);
        }

        /// <summary>
        /// Returns the leading code enclosed in { } 
        /// and trims the StringBuilder
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StringBuilder ReadToMatchingParen(StringBuilder code)
        {
            int count = 1;
            int index = 1;
            string s = code.ToString();

            while (true)
            {
                try
                {
                    if (s[index] == '(')
                        count++;
                    else if (s[index] == ')')
                        count--;

                    if (count == 0)
                        break;

                    index++;
                }
                catch
                {
                    throw new Exception("Unable to find closing ) near " + code);
                }
            }//end while

            string leading = s.Substring(1, index-1);
            string remainder = s.Substring(index + 1).Trim();

            code.Clear();
            code.Insert(0, remainder);

            return new StringBuilder(leading);
        }
        /// <summary>
        /// Returns the code up to the first ;.
        /// code is truncated.
        /// If the code starts with // the function returns up until the new line
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StringBuilder ReadToSemicolon(StringBuilder code)
        {
            string s = code.ToString();
            if (s.IndexOf("//") != 0)
            {
                int index = s.IndexOf(';');

                if (index == -1)
                    throw new Exception("Expected ; near " + code);

                //remove the leading expr from 'code'
                //TODO FIX ME!
                string remainder = s.Substring(index + 1).Trim();
                code.Clear();
                code.Insert(0, remainder);

                //chop off the leading statement
                s = s.Substring(0, index);
                return new StringBuilder(s);
            }
            else
            {//comment
                int index = s.IndexOf('\n');

                if (index == -1)
                    throw new Exception("Expected ; near " + code);

                //remove the leading comment from 'code'
                string remainder = s.Substring(index + 1).Trim();
                code.Clear();
                code.Insert(0, remainder);

                //chop off the leading comment
                s = s.Substring(0, index);
                return new StringBuilder(s);
            }
        }
        
        /// <summary>
        /// chops off everything up to but not including the (
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        string ReadToParen(StringBuilder s)
        {
            string temp = s.ToString();
            if (temp.ToString().IndexOf("(") == -1)
                throw new Exception("Couldn't find ( in " + temp);

            
            temp = temp.Substring(temp.IndexOf("("));
            return temp;
        }

        /// <summary>
        /// unwraps parens from the statement
        /// puts the inner epxression in a builder which
        /// is returned.  The condition is removed from the
        /// code
        /// </summary>
        /// <param name="code"></param>
        StringBuilder GetCondition(StringBuilder code)
        {
            string s = code.ToString();
            if (s.IndexOf('(') != 0)
                throw new Exception("Expected ( near " + code);

            StringBuilder sb = ReadToClosingParen(code);
            return sb;
        }

        /// <summary>
        /// reads an operator and trims the code accordingly
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetOperator(StringBuilder code)
        {
            string s = code.ToString();

            if (s.IndexOf("==") == 0)
            {
                TrimTwo(code); return "==";
            }
            else if (s.IndexOf("++") == 0)
            {
                TrimTwo(code); return "++";
            }
            else if (s.IndexOf("--") == 0)
            {
                TrimTwo(code); return "--";
            }
            else if (s.IndexOf('=') == 0)
            {
                TrimOne(code); return "=";
            }
            else if (s.IndexOf('+') == 0)
            {
                TrimOne(code); return "+";
            }
            else if (s.IndexOf('-') == 0)
            {
                TrimOne(code); return "-";
            }
            throw new Exception("Unable to find operator in " + code.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetOperand(StringBuilder code)
        {
            char[] toks = { ' ', '=', ';', ')' };
            string s = code.ToString();

            int index = s.IndexOfAny(toks);
            if (index != -1)
            {
                //trim the code buffer
                string remainder = s.Substring(index).Trim();
                code.Clear();
                code.Insert(0, remainder);

                s = s.Substring(0, index).Trim();
               return s;
            }
            else
                throw new Exception("Unable to find token near " + code);
        }

        public string TrimTwo(StringBuilder code)
        {
            string s = code.ToString();
            s = s.Substring(2).ToString().Trim();
            code.Clear();
            s.Insert(0, s);
            return s;
        }

        /// <summary>
        /// Trims 1 char off code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string TrimOne(StringBuilder code)
        {
            string s = code.ToString();
            s = s.Substring(1).ToString().Trim();
            code.Clear();
            code.Insert(0, s);
            return s;
        }

        public bool IsBinaryOperator(String op)
        {
            return (IsLogicalOperator(op) ||
                    IsMathOperator(op) ||
                    IsRelationalOperator(op));
        }

        public bool IsLogicalOperator(string op)
        {
            return (op == "||" || op == "&&");
        }
        

        public bool IsRelationalOperator(string op)
        {
            return (op == "<=" ||
                op == ">=" ||
                op == "<" ||
                op == ">" ||
                op == "!=" ||
                op == "==" ||
                op == "has");
        }
         
        public bool IsMathOperator(string op)
        {
            return (op == "+" || op == "-" || op == "*" );
        }
         

        /// <summary>
        /// Returns the next logical op (&& or ||)
        /// or and empty string ""
        /// </summary>
        /// <param name="toks"></param>
        /// <returns></returns>
        string PeekNextLogicalOp(List<string> toks)
        {
            for (int i = 0; i < toks.Count; i++)
            {
                if (IsLogicalOperator(toks[i]))
                    return  toks[i];
            }
            return "";
        }
    }
}
