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
        void FixBitInstructions()
        {
            foreach (Statement st in statements)
            {
                if (st is ExecutableStatement)
                {
                    ExecutableStatement ex = st as ExecutableStatement;

                    if (ex.OpCode == "BIT" || ex.OpCode == "RES" || ex.OpCode == "SET")
                    {
                        
                        byte b = ex.LeftOperand.GetHexText().ToByte();
                        /*
                        b = (byte)(b << 3);

                        if (ex.OpCode == "BIT")
                        {//and | 01000110
                            b = (byte)(b | 01000110);
                        }
                        else if (ex.OpCode == "RES")
                        {//left shit b 3 and | 10000110
                            b = (byte)(b | 10000110);
                        }
                        else if (ex.OpCode == "SET")
                        {//left shit b 3 and | 11000110
                            b = (byte)(b | 11000110);
                        }
                        */
                        
                        if (ex.GetModeText() == "IMMEDIATE,IX_INDEXED_PLUS_OFFSET" ||
                            ex.GetModeText() == "IMMEDIATE,IY_INDEXED_PLUS_OFFSET")
                        {//left shit b 3 or byte 

                            //now flip the operands because they
                            //get written out in reverse order
                            ex.LeftOperand.Value = ex.RightOperand.Value;
                            ex.RightOperand.Value = string.Format("{0:2X}",b);

                        }
                    }
                }
            }
        }
    }
}
