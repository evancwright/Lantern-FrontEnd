using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLL
{
    public static class TokenizerClass
    {
        /// <summary>
        /// Chops a string of code into a list of tokens so it
        /// can be converted into an expression tree
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<string> Tokenize(StringBuilder code)
        {
            List<string> toks = new List<string>();
             
            string tmp = "";
            while (code.Length != 0)
            {
                code = new StringBuilder(code.ToString().Trim());

                string s = code.ToString();

                //does it start with a letter, if so
                //it is a variable of attr/prop assignment
                char ch = s[0];

                if (ch == ';')
                    break;


                if (s.StartsWith("rand"))
                {
                    s = s.Substring(0, s.IndexOf(')')+1);
                    code.Remove(0, s.Length);
                    toks.Add(s);
                    continue;
                }

                if (Char.IsLetter(ch) || ch=='$')
                {
                    //read til next token
                    string num = TrimVarOrAttr(code);
                    toks.Add(num);
                    continue;
                }

                if (ch == '(' || ch == ')')
                {
                    code = code.Remove(0, 1);
                    toks.Add(ch.ToString());
                    continue;
                }

                //does it start with a number, if so
                //it is a constant
                
                if (Char.IsDigit(ch))
                {
                    //read til next token
                    string num = TrimNumber(code);
                    toks.Add(num);
                    continue;
                }

                //does it start with a ", if so it is a 
                //string constant
                if (ch == '"')
                {
                    string str = TrimString(code);
                    toks.Add(str);
                    continue;
                }

                if (s.Length >= 2)
                {
                    tmp = s.Substring(0, 2);
                    if (tmp.IndexOf("==") == 0 ||
                        tmp.IndexOf("!=") == 0 ||
                        tmp.IndexOf("++") == 0 ||
                        tmp.IndexOf("--") == 0 ||
                        tmp.IndexOf("&&") == 0 ||
                        tmp.IndexOf("||") == 0 ||
                        tmp.IndexOf("<=") == 0 ||
                        tmp.IndexOf(">=") == 0)
                    {
                        Trim(code, 2);
                        toks.Add(tmp);
                        continue;
                    }
                    else if (tmp.IndexOf("<") == 0 ||
                        tmp.IndexOf(">") == 0 ||
                        tmp.IndexOf("+") == 0 ||
                        tmp.IndexOf("-") == 0 ||
                        tmp.IndexOf("*") == 0)
                    {
                        tmp = s.Substring(0, 1);
                        Trim(code, 1);
                        toks.Add(tmp);
                        continue;
                    }
                    else
                        throw new Exception("unrecognized operator: " + tmp);
                }

                //check for single char symbols
                ch = s[0];
                if (ch == '=' ||
                    ch == '<' ||
                    ch == '>' ||
                    ch == '!' ||
                    ch == '+' ||
                    ch == '-' ||
                    ch == '(' ||
                    ch == ')')
                {
                    Trim(code, 1);
                    toks .Add(ch.ToString());
                    continue;
                }    
            }

            return toks;
        }

        /// <summary>
        /// chops two leading chars off the buffer
        /// </summary>
        /// <param name="code"></param>
        static void Trim(StringBuilder code, int howMany)
        {
            string s = code.ToString();
            s = s.Substring(howMany).ToString().Trim();
            code.Clear();
            code.Insert(0, s);
        }

        /// <summary>
        /// Returns up until the 1st space or operator is hit
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        static string TrimVarOrAttr(StringBuilder code)
        {
                int i = 0;

                while (i < code.Length && (IsValidVarChar(code[i])))
                {
                    i++;
                }

                string s = code.ToString().Substring(0, i);
                string temp = code.ToString().Substring(i).Trim();
                code.Clear();
                code.Insert(0, temp);
                return s;

        }

        /// <summary>
        /// Reads, trims, and returns the leading number 
        /// of the supplied buffer
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        static string TrimNumber(StringBuilder code)
        {
            int i = 0;
            string num = "";
            while (i < code.Length && Char.IsDigit(code[i]))
            {
                num += code[i];
                i++;
            }

            string s = code.ToString().Substring(num.Length).Trim();
            code.Clear();
            code.Insert(0, s);
            return num;
        }
        /// <summary>
        /// Trims as string literal 
        /// the quotes are returned.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        static public string TrimString(StringBuilder code)
        {
            string s = code.ToString();

            string temp = s.Substring(1);

            int index = temp.IndexOf('"');
            if (index ==-1)
                throw new Exception("Unclosed string literal near " + code);

            string txt  = s.Substring(0, index + 2);

            temp = s.Substring(index + 2).Trim();
            code.Clear();
            code.Insert(0, temp);
            return txt;
        }

        static bool IsValidVarChar(char ch)
        {
            return (Char.IsLetterOrDigit(ch) || ch == '_' || ch == '.' || ch == '$');
        }

        
    }
}
