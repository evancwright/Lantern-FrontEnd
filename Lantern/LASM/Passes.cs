using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LASM
{
    /// <summary>
    /// 
    /// </summary>
    partial class Assembly
    {
        /// <summary>
        /// Break files up modules
        /// </summary>
        /// <returns></returns>
        public void MakeModules(string fileName)
        { 
            //Modules are delineated by a blank line
            //comment lines don't count as blank

            //remove any leading blank lines
            
            /*
            while (lines.Count > 0)
            {
                RemoveLeadingBlankLines(lines);
                ParseModule(lines);
            }
             */
        }

        /// <summary>
        /// Removes leading empty lines
        /// </summary>
        /// <param name="lines"></param>
        void RemoveLeadingBlankLines()
        { 
            while (lines[0].Trim() == "" || lines[0].Trim().StartsWith(";"))
            {
                lines.RemoveAt(0);
            }
        }

        /// <summary>
        /// Moves any labels onto their own lines
        /// </summary>
        /// <param name="lines"></param>
        void IsolateLabels()
        {
            char[] seps = { '\t', ' ' };

            int c = lines.Count;
            for (int i=0; i < c; i++)
            {
                if (lines[i].Trim() == "")
                    continue;

                if (lines[i].Trim().ToUpper().StartsWith("DB") ||
                    lines[i].Trim().ToUpper().StartsWith("DW") ||
                    lines[i].Trim().ToUpper().StartsWith("STR") ||
                    lines[i].Trim().ToUpper().StartsWith("STRZ") ||
                     lines[i].Trim().ToUpper().StartsWith("#DEFINE"))
                    continue;

                char ch = lines[i].First();
                if (!Char.IsWhiteSpace(ch) && ch != ';' ) 
                {
                    //start with a label
                    string line = lines[i];

                    string[] parts = line.Split(seps);
                    string label = parts[0];
                    
                    if (!label.StartsWith("@"))
                    {
                         
                        if (globalLabels.Contains(label))
                        {
                            throw new Exception("Line " + i + ": Duplicate global label" + label);
                        }

                        globalLabels.Add(label);
                    }
                    //remove the existing line
                    lines.RemoveAt(i);

                    //add the label as its own line
                    lines.Insert(i, label);

                    //add the remainder on the line after it
                    string rest = line.Substring(label.Length).Trim();

                    if (rest.Length > 0)
                    {
                        lines.Insert(i + 1, rest);
                        i++;
                        c++;
                    }
                    
                }
            }
        }

        /// <summary>
        /// Removes any commented lines from the source
        /// </summary>
        void RemoveCommentLines()
        {
            int c = lines.Count;
            for (int i = 0; i < c; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith(";"))
                {
                    lines.RemoveAt(i);
                    c--;
                    i--;
                }
            }
        }


          

        void WriteParsedOperands(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (Statement st in statements)
                {
                    if (st is ExecutableStatement)
                    {
                        ExecutableStatement es = st as ExecutableStatement;
                        sw.Write(es.OpCode);
                        if (es.Op1 != "")
                            sw.Write(" | " + es.Op1);
                        if (es.Op2 != "")
                            sw.Write(" | " + es.Op2);
                        if (es.Op3 != "")
                            sw.Write(" | " + es.Op3);

                        if (es.TrailingComment != "" && es.TrailingComment != null)
                            sw.Write(" | " + es.TrailingComment);
                        sw.WriteLine();
                    }
                }

            }
        }


    }//end class
}
