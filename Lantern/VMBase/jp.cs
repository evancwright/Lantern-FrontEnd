using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* jp instructions */
        /// <summary>
        /// Assumes 2nd byte is the high byte
        /// </summary>
        void jmp()
        {
            pc = (ushort)(ir[1] + ir[2] * 256);
        }

        void jmpcc(bool cc)
        {
            if (cc)
            {
                jmp();
            }
        }
         
        void jmp_hl()
        {
            pc = hl;
        }

        void jmp_z()
        {
            jmpcc(zeroBit == 1);
        }

        void jmp_nz()
        {
            jmpcc(zeroBit == 0);
        }

        void jmp_c()
        {
            jmpcc(carryBit == 1);
        }

        void jmp_nc()
        {
            jmpcc(carryBit == 0);
        }

        void jmp_p()
        {
            jmpcc(signBit == 0);
        }

        void jmp_m()
        {
            jmpcc(signBit == 1);
        }

    }
}
