using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    public partial class VMBase
    {
        void and_b()
        {
            a = (byte)(a & b);
            SetFlagsA();
        }

        void and_immediate()
        {
            a = (byte)(a & ir[1]);
            SetFlagsA();
        }
    }
}
