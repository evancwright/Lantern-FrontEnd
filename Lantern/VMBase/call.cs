using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* call instructions */

        void call()
        {
            //push the program counter
            byte hi = (byte)(pc/256);
            byte lo = (byte)(pc%256);

            WriteByte(sp, hi);
            sp--;

            WriteByte(sp, lo);
            sp--;

            ushort newAddr = (ushort) (ir[2] * 256 + ir[1]);
            pc = newAddr;
        }

        void call_zero()
        {
            if (zeroBit==1)
                call();
        }

        void call_not_zero()
        {
            if (zeroBit ==0)
                call();
        }


        void call_carry()
        {
            if (carryBit == 1)
                call();
        }

        void call_no_carry()
        {
            if (carryBit == 0)
                call();
        }


        void call_plus()
        {
            if (signBit != 0)
                call();
        }

        void call_minus()
        {
            if (signBit == 1)
                call();
        }

        void call_cc(byte cc)
        {
            if (cc == 0) call_not_zero();
            else if (cc == 1) call_zero();
            else if (cc == 2) call_no_carry();
            else if (cc == 3) call_carry();
            else if (cc == 6) call_plus();
            else if (cc == 7) call_minus();
        }

        void ret()
        {
            sp++;
            byte lo = GetByte(sp);
            sp++;
            byte hi = GetByte(sp);
        
            ushort newAddr = (ushort)(hi * 256 + lo);
            pc = newAddr;
        }

    }
}
