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
            try
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

                //pairs.Add("SET 7,", "SET 0FEH,");

                pairs.Add("SET 0,", "SET0 ");
                pairs.Add("SET 1,", "SET1 ");
                pairs.Add("SET 2,", "SET2 ");
                pairs.Add("SET 3,", "SET3 ");
                pairs.Add("SET 4,", "SET4 ");
                pairs.Add("SET 5,", "SET5 ");
                pairs.Add("SET 6,", "SET6 ");
                pairs.Add("SET 7,", "SET7 ");

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

                //make a second pass to replace
                //bit7 (ix+d) -> bit7 bitcode,(ix + d)
                for (int i = 0; i < lines.Count; i++)
                {
                    string s = lines[i];
                    if (Regex.IsMatch(s, @"BIT[0-7][t ]*\([\t ]*IX"))
                    {
                        string ch = s[s.IndexOf("BIT") + 3] + "";
                        uint bit = Convert.ToUInt32(ch) << 3;
                        bit |= 0b01000110;
                        string bitLiteral = String.Format("0{0:X2}H", bit);

                        string[] parts = s.Split('(');
                        s = parts[0] + " " + bitLiteral + ",(" + parts[1];
                        lines[i] = s;
                    }
                    else if (Regex.IsMatch(s, @"RES[0-7][t ]*\([\t ]*IX"))
                    {
                        string ch = s[s.IndexOf("RES") + 3] + "";
                        uint bit = Convert.ToUInt32(ch) << 3;
                        bit |= 0b10000110;
                        string bitLiteral = String.Format("0{0:X2}H", bit);

                        string[] parts = s.Split('(');
                        s = parts[0] + " " + bitLiteral + ",(" + parts[1];
                        lines[i] = s;
                    }
                    else if (Regex.IsMatch(s, @"SET[0-7][t ]*\([\t ]*IX"))
                    {
                        string ch = s[s.IndexOf("SET") + 3] + "";
                        uint bit = Convert.ToUInt32(ch) << 3;
                        bit |= 0b11000110;
                        string bitLiteral = String.Format("0{0:X2}H", bit);

                        string[] parts = s.Split('(');
                        s = parts[0] + " " + bitLiteral + ",(" + parts[1];
                        lines[i] = s;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error rewriting op codes", ex);
            }
        }
    }
}
