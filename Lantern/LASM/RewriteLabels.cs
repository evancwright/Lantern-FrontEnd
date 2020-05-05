using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LASM
{
    partial class Assembly
    {
        /// <summary>
        /// replace $label?  with @label
        /// </summary>
        void RewriteLabels()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string pattern = @"\$[A-Z0-9]*\?";

                if (Regex.IsMatch(line, pattern))
                {
                 
                    string[] parts = Regex.Split(line, pattern);
                    string label = Regex.Match(line, pattern).Value;
                    label = Regex.Replace(label, "\\$", "@");
                    label = Regex.Replace(label, "\\?", String.Empty);
                    
                    try
                    {
                        lines[i] = parts[0] + label + " " + parts[1];
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Line 1: Unable to rewrite label", ex);
                    }
                         
                }
            }
        }

        /// <summary>
        /// Rewrite A equ B as a #define
        /// </summary>
        void RewriteDefines()
        {
            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];

                    if (!line.Trim().StartsWith(";"))
                    {

                        if (Regex.IsMatch(line, "[\t ]EQU[\t ]"))
                        {
                            string[] parts = Regex.Split(line, "[\t ]EQU[\t ]");

                            string inner = "#DEFINE " + parts[0] + " " + parts[1];
                            lines[i] = inner;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error rewrite #defines.", ex);
            }
        }

    }
}
