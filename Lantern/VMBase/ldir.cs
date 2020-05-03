using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* ldir */
        /* load data increment repeating */

        /*de<-hl until bc==0*/
        void ldir()
        {
            while (bc != 0)
            {
                byte data= GetByte(hl);
                WriteByte(hl, data);
                inc_de();
                inc_hl();
                dec_bc();
            }
        }

        void neg()
        {
            a = (byte)(~a);
        }

        void nop()
        {
        }

        void or_r(byte rcode)
        {
            or_a(rcode);
        }

        void or_n()
        {
            or_a(ir[1]);
        }

        void or_a(byte rcode)
        {
            int r = Get8BitReg(rcode);
            a = (byte)(a | r);
            SetZeroAndSign8(a);
        }

        void or_hl()
        {
            byte b = GetByte(hl);
            or_a(b);
        }

        /// <summary>
        /// or (ix+d)
        /// </summary>
        void or_ix()
        {
            ushort addr = ix;
            addr += ir[2];
            byte b = GetByte(addr);
            or_a(b);
        }

        /// <summary>
        /// or (ix+d)
        /// </summary>
        void or_iy()
        {
            ushort addr = iy;
            addr += ir[2];
            byte b = GetByte(addr);
            or_a(b);
        }

    }
}
