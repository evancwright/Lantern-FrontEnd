using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VMBase
{
    partial class VMBase
    {
        void jmprel()
        {

            byte offset = ir[1];

            if (offset > 127)
            {//negative offset
                offset = (byte)((255 - offset) + 1); /* two's complement */
                pc = (ushort)(pc - offset);
            }
            else
            {
                pc = (ushort)(pc + offset);
            }
        }

        void jmprelcc(bool flag)
        {
            if (flag)
                jmprel();
        }

        void jr()
        {
            jmprel();
        }

        void jrc()
        {
            jmprelcc(carryBit == 1);
        }

        void jrnc()
        {
            jmprelcc(carryBit == 0);
        }

        void jrz()
        {
            jmprelcc(zeroBit == 1);
        }
        void jrnz()
        {
            jmprelcc(zeroBit == 0);
        }
    }
}