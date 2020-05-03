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
        /// Rewrites opcode like jp z,label and jpz label
        /// Rewrites (iy) as (iy+0) ...same with ix
        /// </summary>
        void RewriteOpCodes()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("CALL Z,", "CALLZ ");
            pairs.Add("CALL NZ,", "CALLNZ ");
            pairs.Add("JP Z,", "JPZ ");
            pairs.Add("JP NZ,", "JPNZ ");
            pairs.Add("JP NC,", "JPNC ");
            pairs.Add("JP C,", "JPC ");
            pairs.Add("JP P,", "JPP ");
            pairs.Add("JP M,", "JPM ");
            pairs.Add("JR Z,", "JRZ ");
            pairs.Add("JR NZ,", "JRNZ ");
            pairs.Add("JR C,", "JRC ");
            pairs.Add("JR NC,", "JRNC ");
            
            pairs.Add("BIT 0,", "BIT 46H,");
            pairs.Add("BIT 1,", "BIT 4EH,");
            pairs.Add("BIT 2,", "BIT 56H,");
            pairs.Add("BIT 3,", "BIT 5EH,");
            pairs.Add("BIT 4,", "BIT 66H,");
            pairs.Add("BIT 5,", "BIT 6EH,");
            pairs.Add("BIT 6,", "BIT 76H,");
            pairs.Add("BIT 7,", "BIT 7EH,");

            pairs.Add("RES 0,", "RES 086H,");
            pairs.Add("RES 1,", "RES 08EH,");
            pairs.Add("RES 2,", "RES 096H,");
            pairs.Add("RES 3,", "RES 09EH,");
            pairs.Add("RES 4,", "RES 0A6H,");
            pairs.Add("RES 5,", "RES 0AEH,");
            pairs.Add("RES 6,", "RES 0B6H,");
            pairs.Add("RES 7,", "RES 0BEH,");

            pairs.Add("SET 7,", "SET 0FEH,");

            for (int i = 0; i < lines.Count; i++)
            {
                string s = lines[i];

                foreach (string k in pairs.Keys)
                {
                    if (Regex.IsMatch(s, k))
                    {
                        s = Regex.Replace(s, k, pairs[k]);
                    }    

                }

                //now fix IX
                if (Regex.IsMatch(s, @"[(][ \t]*IX[ \t]*[)]"))
                {
                    s = Regex.Replace(s, @"[(][ \t]*IX[ \t]*[)]", "(IX+0)");
                }

                //now fix IY
                //now fix IX
                if (Regex.IsMatch(s, @"[(][ \t]*IY[ \t]*[)]"))
                {
                    s = Regex.Replace(s, @"[(][ \t]*IY[ \t]*[)]", "(IY+0)");
                }

                lines[i] = s;
            }
        }
         
    }

}
