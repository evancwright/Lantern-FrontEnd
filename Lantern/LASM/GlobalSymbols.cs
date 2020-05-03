using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangDef;

namespace LASM
{
    partial class Assembly
    {
        /// <summary>
        /// find lines that start with a letter, but aren't
        /// DB,BW,RESB,STR,STRZ,#define,or #include
        /// </summary>
        void FindGlobalLabels()
        {

            for (int i=0; i  < lines.Count; i++)
            {
                string s = lines[i];
               if (s.StartsWithLabel())
                {
                    string label = s.GetFirstPart();

                    if (globalLabels.Contains(label))
                    {
                        throw new Exception(
                            string.Format(
                                "Line {0}, Duplicate global symbol: {1}",
                                i, s)
                                );
                    }
                    
                    globalLabels.Add(label);
                }

            }

        }


        public List<string> globalLabels = new List<string>();
      //  public Dictionary <string, ushort> globalLabelOffsets = new Dictionary<string,ushort>();
      //  public Dictionary<string, ushort> localLabelOffsets = new Dictionary<string, ushort>();

        public Dictionary<string, ushort> labelOffsets = new Dictionary<string, ushort>();

        List<string> pseudoOps = new List<string>()
        {
            "RESB","DB","DW","#INCLUDE","#DEFINE","@",";"

        };
    }
}
