using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        void sub8(byte op)
        {
            carryBit = 0;
            if (op > a) carryBit = 1;
            a -= op;
            SetZeroAndSign8(a);
        }

        void sub_n()
        {
            sub8(ir[1]);
        }

        void sub_r(byte rcode)
        {
            byte r = Get8BitReg(rcode);
            ir[1] = r;
            sub_n();            
        }
    }
}
