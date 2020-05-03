using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /*shift instructions*/

        /*shift left arithmetic */
        void sla_c()
        {
            int temp = c * 2;

            carryBit = 0;
            if (temp > 255)
                carryBit = 1;

            c = (byte)(temp % 256);
            SetZeroAndSign8(c);
        }

        /*shift right arithmetic */
        void sra_a()
        {
            /*right most bit becomes the carry flag*/

            carryBit = (byte)(a & 0x01); /* isolate lsb */

            a /= 2; /* right shift */

            /*set the msb to be the old sign bit*/
            a = (byte)(a & 0x7F); /* clear sign */

            a = (byte)(a | (signBit * 128)); /* mask on the old sign */

            SetZeroAndSign8(a);
        }

        /*shift right logical */
        void srl_a()
        {
            /*right most bit becomes the carry flag*/
            carryBit = (byte)(a & 0x01); /* isolate lsb */

            a /= 2; /* right shift */

            SetZeroAndSign8(a);
        }
    }
}
