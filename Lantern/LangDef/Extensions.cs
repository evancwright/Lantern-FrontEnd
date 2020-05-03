using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LangDef
{
    public static class Extensions
    {
        /// <summary>
        /// Returns true if all chars are 0-F
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsHexSymbol(this char ch)
        {
            return ((ch >= '0' && ch <= '9') ||
                    ch >= 'a' || ch <= 'f' ||
                    ch >= 'A' || ch <= 'F');

        }


        public static string UnwrapParens(this string s)
        {
            int index1 = s.IndexOf('(');
            int index2 = s.LastIndexOf(')');

            if (index2 == -1)
                throw new Exception("Couldn't unwrap parens from " + s);

            string inner = s.Substring(index1, index2 - index1);
            return inner;
        }

        public static string UnwrapQuotationMarks(this string s)
        {
            int index1 = s.IndexOf('"');
            int index2 = s.LastIndexOf('"');

            if (index2 == -1)
                throw new Exception("Couldn't unwrap quotation marks from " + s);

            string inner = s.Substring(index1+1, (index2 - index1)-1);
            return inner;
        }

        public static bool IsStringLiteral(this string s)
        {
            return s.First() == '\"' && s.Last() == '\"';
        }

        /// <summary>
        /// returns a string of ASCII codes
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string AsHexText(this string s)
        {
            string temp = "";
            s = s.UnwrapQuotationMarks();

            foreach (char ch in s)
            {
                byte b = (byte)ch;
                temp += string.Format("{0:X2}", b);
            }
            return temp;
        }

        /// <summary>
        /// Returns true if the string is in the form 0H or 00H
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsHexByteConstant(this string s)
        {
            s = s.Trim();
            Match m = Regex.Match(s, "[0-9a-fA-F]{1,2}H");
            if (m.Success &&  m.Length == s.Length)
                return true;
            return false;
        }

        public static bool IsHexLiteral(this string s)
        {   
            if (s.Last() != 'H')
                return false;

            if (s.Length == 1)
                return false; //it's register h

            if (Char.IsLetter(s.First()))
                return false;

            for (int i=0; i < s.Length-1;i++)
            {
                if (!s[i].IsHexSymbol())
                    return false;
            }
            return true;
        }

        public static bool IsHexWordConstant(this string s)
        {
            s = s.Trim();
            Match m = Regex.Match(s, "0[0-9a-fA-F]{1,4}H");
            if (m.Success && m.Length == s.Length)
                return true;
            return false;
        }

        public static ushort HexToDecimal(this string s)
        {
            if (s.Last() == 'H')
                s = s.Substring(0, s.Length-1);

            string t = new string(s.ToCharArray().Reverse().ToArray());

             
            ushort sum = 0;
            for (int i = 0; i < t.Length; i++)
            {
                int d = t[i].ToDecimalDigit();

                sum += (ushort)(Math.Pow(16, i) * d );
            }

            return sum;
        }

        /// <summary>
        /// Converts a hex symbol to a 0-15 value
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public  static byte ToDecimalDigit(this char ch)
        {
            if (ch >= '0' && ch <= '9')
                return (byte)(ch - '0');
            if (ch >= 'a' && ch <= 'f')
                return (byte)(10 + ch - 'a');
            if (ch >= 'A' && ch <= 'F')
                return (byte)(10 + ch - 'A');

            throw new Exception("Couldn't convert " + ch + " to a digit!");
        }

        /// <summary>
        /// Returns true is s is a decimal number
        /// or literal the form 12345D
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDecimalNumber(this string s)
        {
            if (s.EndsWith("D"))
            {
                if (s.Length > 1)
                    s = s.Substring(0, s.Length - 1);
                else
                    return false;  //it's register d
            }

            int val;
            return int.TryParse(s, out val);
        }

        public static bool IsNumber(this string s)
        {
            return s.IsHexLiteral() || s.IsDecimalNumber();
        }

        /// <summary>
        /// return true if the operand is in the form
        /// nn (0-9,0-9)
        /// </summary>
        /// assumes s is a number
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsByte(this string s)
        {
            //ffh (hex byte)
            if (s.Length <= 2 || (s.Length == 3 && s.Last() == 'H'))
            {
                return true;
            }
            //digit
            else if (s.Length == 2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the hex representation of s.
        /// s is assumed to be a hex or decimal literal
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToByteHexText(this string s)
        {
            if (s == "0" || s == "0H")
                return "00"; //hack! string.Format doesn't format zeros propertly

            if (s.EndsWith("H"))
            {//just chop off the h
                string temp = s.Substring(0, s.Length - 1);

                if (temp.Length == 3 && temp[0] == '0') //0FFH
                {
                    temp = temp.Substring(1);
                }
                return string.Format("{0:X2}", temp);
            }
            else
            {//convert the decimal to it's hex representation
                int x = Convert.ToInt32(s);

                if (x > 255)
                    throw new Exception(s + " is too big to fit in a byte");

                return string.Format("{0:X2}",x);
            }
        }

        /// <summary>
        /// Assumes: 1234 or 1234D
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ushort ToUInt16(this string s)
        {
            if (s.Last()=='D')
            {
                s = s.Substring(0, s.Length - 1);
            }

            return Convert.ToUInt16(s);
        }

        /// <summary>
        /// Returns the hex representation of s.
        /// s is assumed to be a hex or decimal literal
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToWordHexText(this string s)
        {
            if (s.EndsWith("H"))
            {//just chop off the h
                string temp = s.Substring(0, s.Length - 1);
                return temp;
            }
            else
            {//convert the decimal to it's hex representation
                int x = Convert.ToInt32(s);

                if (x > 65535)
                    throw new Exception(s + " is too big to fit in a word");

                return string.Format("{0:X4}", x);
            }
        }


        public static bool StartsWithLabel(this string s)
        {

            List<string> pseudoOps = new List<string>()
            {
                "*MOD", "DB","DW","*INCLUDE","#DEFINE","@",";"
            };

            if (s == "")
                return false;

            foreach (string op in pseudoOps)
            {
                if (s.StartsWith(op))
                    return false;
            }

            if (s.Trim().StartsWith(";"))
            {
                return false;
            }

            if (Char.IsLetter(s[0]))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns everything left of the 1st white space
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFirstPart(this string s)
        {
            int index = s.IndexOfAny(new char[] { ' ', '\t' });

            if (index != -1)
            {
                string temp = s.Substring(0, index);
                return temp;
            }
            else
            {
                return s;
            }
        }


        /// <summary>
        /// Returns everything after the first white space
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetRest(this string s)
        {
            int index = s.Trim().IndexOfAny(new char[] { ' ', '\t' });

            if (index != -1)
            {
                string temp = s.Substring(index).Trim();
                return temp;
            }
            else
            {
                return "";
            }
        }

        public static string GetParenContents(this string s)
        {
            int index1 = s.IndexOf('(');
            int index2 = s.IndexOf(')');
            string inner = s.Substring(index1 + 1, index2 - (index1 + 1)).Trim();
            return inner;
        }

        /// <summary>
        /// assumes opcode is still attached
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFirstOperand(this string s)
        {
            string rest = s.GetRest();

            //does is contain a comma?
            int index = rest.IndexOf(',');
            if (index != -1)
            {// ld a,b
                rest = rest.Trim();
                string left = rest.SplitAndTrim(',')[0];
                return left;
            }
            else
            {//push af
                return rest.Trim();
            }

        }

        /// <summary>
        /// assumes opcode is still attached
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetSecondOperand(this string s)
        {
            string rest = s.GetRest();

            //does is contain a comma?
            int index = rest.IndexOf(',');
            if (index != -1)
            {// ld a,b
                rest = rest.Trim();
                string right = rest.SplitAndTrim(',')[1];
                return right;
            }
            else
            {//push af
                return rest.Trim();
            }

        }


        public static string[] SplitAndTrim(this string s, char ch)
        {
            string[] parts = s.Split(ch);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }
            return parts;
        }

        public static bool IsParenExpression(this string s)
        {
            return (s.First() == '('  && s.Last() == ')');
        }

        public static string AsHextText(this string s)
        {
            string text = "";
            foreach (char ch in s)
            {
                byte b = (byte)ch;
                text += string.Format("{0:2X)", b);
            }
            return text;
        }

        public static byte ToByte(this string s)
        {
            if (s.Length !=2) throw new Exception("Cannot convert " + s + " to byte");

            return (byte) (s[1].ToDecimalDigit() + s[0].ToDecimalDigit() * 16);
        }

    }//end class
}
