using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* add.c 
 * Implementations for addition op codes
 */

            /// <summary>
            /// Add value to accumulator
            /// </summary>
            /// <param name="val"></param>
        void add8(byte val)
        {
            ushort temp = (ushort)(a + val);
            SetCarryBit8(temp);
        
            a = (byte)(temp % 256);
            SetFlagsA();
        }

        void add16(ref ushort tgt, ushort src)
        {
            carryBit = 0;
            uint temp = (uint)(tgt + src);
            if (temp > 65535)
            {
                carryBit = 1;
            }

            tgt = (ushort)temp;
        }

        void add_immediate()
        {
            add8(ir[1]);
        }

        void add_r(byte rcode)
        {
            byte b = Get8BitReg(rcode);
            add8(ir[1]);
        }

        void add_a_n()
        {
            add_immediate();
        }

        void add_hl_bc()
        {
            ushort rr = hl;
            add16(ref rr, bc);
            hl = rr;
        }


        void add_hl_de()
        {
            ushort rr = hl;
            add16(ref rr, de);
            hl = rr;
        }

        void add_ix_bc()
        {
            add16(ref ix, bc);
        }

        void add_ix_de()
        {
            add16(ref ix, de);
        }

        void add_iy_bc()
        {
            add16(ref iy, bc);
        }

        void add_iy_de()
        {
            add16(ref iy, de);
        }

    }
}
