using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {

        void bit_b_ix(byte b, byte d)
        {
            zeroBit = 0;
            byte mask = (byte)Math.Pow(2, b);
            byte mem = GetByte( (ushort)(ix+d));
            mem = (byte)(mem & mask);

            if (mem == 0)
            {
                zeroBit = 1;
            }
        }

        void bit_b_iy(byte b, byte d)
        {
            zeroBit = 0;
            byte mask = (byte)Math.Pow(2, b);
            byte mem = GetByte((ushort)(iy+d));
            mem = (byte)(mem & mask);

            if (mem == 0)
            {
                zeroBit = 1;
            }
        }


        void bit_b_hl(byte b)
        {
            byte mask = (byte)Math.Pow(2, b);
            byte mem = GetByte(hl);
            mem = (byte)(mem & mask);
            if (mem == 0) zeroBit = 1;
        }

        void bit_b_r(byte b, byte r)
        {
            byte mem = 0;

            mem = Get8BitReg(r);
 
            zeroBit = 0;
            byte mask = (byte)Math.Pow(2, b);
            
            mem = (byte)(mem & mask);

            if (mem == 0)
            {
                zeroBit = 1;
            }

        }

        /* test bit 1,(ix+5) */

        void bit_ix_iy(ushort rr)
        {
            byte offset = ir[2];
            ushort addr = rr;
            byte bit = ir[3];
            addr += offset;

            /* turn the op code into the bit to test */
            bit = (byte)(bit & 32); /* chop off highest 2 bits */
            bit /= 8; /* right shift it two place */

            byte b = GetByte(addr);
            ir[1] = b;
            and_immediate();
        }

        void bit_n_ix()
        {
            bit_ix_iy(ix);
        }

        void bit_n_iy()
        {
            bit_ix_iy(iy);
        }

        byte Get8BitReg(byte r)
        {
            byte mem = 0;
            if (r == 0) mem = b;
            if (r == 1) mem = c;
            if (r == 2) mem = d;
            if (r == 3) mem = e;
            if (r == 4) mem = h;
            if (r == 5) mem = l;
            if (r == 7) mem = a;
            return mem;
        }
    }
}
