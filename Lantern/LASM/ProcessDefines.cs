using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LangDef;
namespace LASM
{
    partial class Assembly
    {
        /// <summary>
        /// Puts #defines into a list so they can be replaced
        /// </summary>
        void BuildDefineTable()
        {
            //FIND #defines
            foreach (string line in lines)
            {
                if (line.StartsWith("#DEFINE"))
                {
                    try
                    {
                        DefineStatement ds = new DefineStatement(line);

                        if (defines.ContainsKey(ds.Value))
                        {
                            if (ds.Replacement != defines[ds.Value])
                            {
                                throw new Exception("Redfining define:" + line);
                            }
                           //it's already in the table
                        }
                        else 
                            defines.Add(ds.Value, ds.Replacement);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to build define statement for:" + line, ex);
                    }
                }
            }
        }

        

        void ReplaceDefines()
        {
            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string s = lines[i];

                    if (!s.StartsWith("#DEFINE") &&
                        !Regex.IsMatch(s, @"[ \t]DB[\t ]") &&
                        !Regex.IsMatch(s, @"[ \t]DW[\t ]")
                        )
                    {
                        foreach (string define in defines.Keys)
                        {
                            //split the line on commas, spaces
                            string pattern = @"[ \t,\(+]" + define + @"([ \t,;\)+]|$)";

                            if (Regex.IsMatch(s, pattern))
                            {
                                string repl = defines[define];
                                /*
                                if (repl.IsHexWordConstant())
                                {
                                    ushort flipped = repl.ToUInt16();

                                    ushort lo = (ushort)( flipped % 256 );
                                    ushort hi = (ushort) (flipped / 256 );

                                    repl = string.Format("0{0:X2}{1:X2}H", lo, hi);

                                }
                                else if (repl.IsDecimalWord())
                                {
                                    ushort flipped = repl.ToUInt16();

                                    ushort lo = (ushort)(flipped % 256);
                                    ushort hi = (ushort)(flipped / 256);

                                    repl = string.Format("0{0:X2}{1:X2}H", lo, hi);
                                }*/
                                s = Regex.Replace(s, define, repl);
                            }
                        }
                        lines[i] = s;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error replacing #defines", ex);
            }
        }

    }
}
