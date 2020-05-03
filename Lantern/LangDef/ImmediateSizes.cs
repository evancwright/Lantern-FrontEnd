using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangDef
{
    static partial class LangDef
    {
        static void SetImmediateSizes()
        {
            try
            {
                ImmediateSizeTable.Add("ADD A,IMMEDIATE", 1);

                ImmediateSizeTable.Add("AND IMMEDIATE", 1);

                ImmediateSizeTable.Add("BIT IMMEDIATE,IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("BIT IMMEDIATE,IY_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("CALL IMMEDIATE", 2);
                ImmediateSizeTable.Add("CALLZ IMMEDIATE", 2);
                ImmediateSizeTable.Add("CALLNZ IMMEDIATE", 2);
                ImmediateSizeTable.Add("CP IMMEDIATE", 1);
                ImmediateSizeTable.Add("CP IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("CP IY_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("DJNZ IMMEDIATE", 1);
                ImmediateSizeTable.Add("JP IMMEDIATE", 2);
                ImmediateSizeTable.Add("JPZ IMMEDIATE", 2);
                ImmediateSizeTable.Add("JPNC IMMEDIATE", 2);
                ImmediateSizeTable.Add("JPNZ IMMEDIATE", 2);
                ImmediateSizeTable.Add("JPP IMMEDIATE", 2);
                ImmediateSizeTable.Add("JPM IMMEDIATE", 2);
                ImmediateSizeTable.Add("JPC IMMEDIATE", 2);
                ImmediateSizeTable.Add("JR IMMEDIATE", 1);
                ImmediateSizeTable.Add("JRZ IMMEDIATE", 1);
                ImmediateSizeTable.Add("JRC IMMEDIATE", 1);
                ImmediateSizeTable.Add("JRNC IMMEDIATE", 1);
                ImmediateSizeTable.Add("JRNZ IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD A,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD A,IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("LD A,IY_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("LD B,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD B,IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("LD B,IY_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("LD BC,IMMEDIATE", 2);
                ImmediateSizeTable.Add("LD C,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD D,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD E,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD E,IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("LD E,IY_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("LD H,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD H,IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("LD L,IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("LD L,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD DE,IMMEDIATE", 2);
                ImmediateSizeTable.Add("LD HL,IMMEDIATE", 2);
                ImmediateSizeTable.Add("LD HL_INDEXED,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD IX,IMMEDIATE", 2);
                ImmediateSizeTable.Add("LD IY,IMMEDIATE", 2);
                ImmediateSizeTable.Add("LD IX,MEMORY", 2);
                ImmediateSizeTable.Add("LD IY,MEMORY", 2);
                ImmediateSizeTable.Add("LD IX_INDEXED_PLUS_OFFSET,A", 1);
                ImmediateSizeTable.Add("LD IX_INDEXED_PLUS_OFFSET,B", 1);
                ImmediateSizeTable.Add("LD IX_INDEXED_PLUS_OFFSET,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD IY_INDEXED_PLUS_OFFSET,A", 1);
                ImmediateSizeTable.Add("LD IY_INDEXED_PLUS_OFFSET,IMMEDIATE", 1);
                ImmediateSizeTable.Add("LD DE,MEMORY", 2);
                ImmediateSizeTable.Add("LD SP,IMMEDIATE", 2);
                ImmediateSizeTable.Add("RES IMMEDIATE,IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("SET IMMEDIATE,IX_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("SET IMMEDIATE,IY_INDEXED_PLUS_OFFSET", 1);
                ImmediateSizeTable.Add("SUB IMMEDIATE", 1);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception trying to set immediate lengths.", ex);
            }
        }


    }
}
