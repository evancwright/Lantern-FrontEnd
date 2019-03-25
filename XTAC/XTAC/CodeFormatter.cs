using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTAC
{
    class CodeFormatter
    {

        const int numSpaces = 5;
         

        //formats the code in string 's'
        public static string Format(string s)
        {

            int indentLevel = 0;
            int index = 0;
            int charIx = 0;
            char lastChar = ' ';

            StringBuilder code = new StringBuilder(s);
            while (index != code.Length)
            {
                
                if (code[index] == '{')
                {
                    //need to delete white space behind {
                    DeleteTrailing(code, ref index);
                    code.Insert(index, '\n');
                    code.Insert(index, '\r');
                    index += 2; 
                    InsertWhiteSpace(code, ref index, indentLevel);
                    indentLevel++;
                    code.Insert(index + 1, '\n');
                    code.Insert(index+1, '\r');
                    index += 3;
                    InsertWhiteSpace(code, ref index, indentLevel);
                }
                else if (code[index] == '}')
                {  
                    //insert a cr before the }
                    DeleteTrailing(code, ref index);                   
                    code.Insert(index, '\n');
                    code.Insert(index, '\r');
                    index += 2; //skip cr  
                    indentLevel--;
                    InsertWhiteSpace(code, ref index, indentLevel);
                    index++;

                    //is the next thing an else?
                    string temp = code.ToString().Substring(index).Trim();
                    if (temp.StartsWith("else"))
                    {
                        code.Insert(index, "\r\n");
                        index++;
                        index++;
                        InsertWhiteSpace(code, ref index, indentLevel);
                    }
                }
                else if (code[index] == ';')
                {
                    index++; //skip ;
                    code.Insert(index, '\n');
                    code.Insert(index, '\r');
                    index += 2;
                    InsertWhiteSpace(code, ref index, indentLevel); //insert padding for next line
                }
                else
                {
                    index++;
                }
            }
            return code.ToString();
        }
         

        /*
         *Trims off leading white space
         *Adds the appropriate amount back on
         *starting at index
         */
        public static void InsertWhiteSpace(StringBuilder codeStr, ref int index, int indentLevel)
        {

            if (index == codeStr.Length) { return; }

            //strip existing space off
            while (IsWhiteSpace(codeStr[index])) 
            {
                codeStr.Remove(index,1);
                if (index == codeStr.Length) { return; }
            }

            //add the correct amount back
            if (indentLevel > 0)
            {
                for (int i = 0; i < indentLevel * numSpaces; i++) { codeStr.Insert(index, ' '); index++; }
            }
        }

        //assumes index is pointing to a '{'
        //at the end, the index should still point to a '{'
        private static void DeleteTrailing(StringBuilder code, ref int index)
        {
            while (IsWhiteSpace(code[index - 1]))
            {
                code.Remove(index - 1, 1);
                index--;
            }
            
        }

        static bool IsWhiteSpace(char ch)
        {
            if (ch == ' ') return true;
            if (ch == '\r') return true;
            if (ch == '\n') return true;
            if (ch == '\t') return true;
//            if (ch == 'X') return true;

            return false;
        }

        public static int GetIndentSize(string code, int index)
        {
            int indent = 0;
            for (int i=0; i < index; i++)
            {
                if (code[i] == '{')
                    indent += numSpaces;
                if (code[i] == '}')
                    indent -= numSpaces;
            }
            return indent;
        }

        /// <summary>
        /// tabs over the selected code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="selStart"></param>
        /// <param name="selEnd"></param>
        /// <returns></returns>
        public static string InsertTabs(string code, int selStart, int selEnd)
        {
            //back up to previous newline or start

            //insert a tab
            StringBuilder sb = new StringBuilder(code);

            //move past next \r\n
            int start = CodeFormatter.GetLineStartIndex(code, selStart);
            string chunk = code.Substring(start, selEnd - start);
            string tabbed = chunk.Replace("\r\n", "\r\n      ");

            //now extract the selected code and replace it with the tabbed code
            string left = code.Substring(0, start);
            string right = code.Substring(selEnd);

            string newCode = left + tabbed + right;
            return newCode;
        }

        /// <summary>
        /// returns 0 or the index just ahead of the previous crlf
        /// </summary>
        /// <param name="code"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        static int GetLineStartIndex(string code, int selStart)
        {
            string left = code.Substring(0,selStart);
            int index = left.LastIndexOf("\r\n");

            if (index != -1)
            {
                return index;
            }
            return 0;
        }
    }
}
