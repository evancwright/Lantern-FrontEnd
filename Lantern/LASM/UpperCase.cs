using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASM
{
    partial class Assembly
    {
        string UpperCase(string line)
        {
            StringBuilder sb = new StringBuilder();
            bool ucase = true;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\"')
                {
                    ucase = !ucase;
                }

                if (ucase)
                {
                    sb.Append(line[i].ToString().ToUpper());
                }
                else
                {
                    sb.Append(line[i].ToString());
                }
                
            }

            return sb.ToString();
            
        }
    }

}
