using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* compare instructions */


        void cp_immediate8(byte val)
        {
            ushort temp = a;

            zeroBit = 0;
            if (val == a)
                zeroBit = 1;

            carryBit = 0;
            if (val > a)
                carryBit = 1;

            //sign bit
            uint tempA = a;
            uint operand = val;
            operand = ~operand;
            operand++;
            uint result = tempA + operand;
            signBit = 0;
            if ( (result & 0x0100) != 0)
    		    signBit = 1;
        }

        void cp_r(byte regcode)
        {
            byte data = Get8BitReg(regcode);
            cp_immediate8(data);
        }

        void dec_sp()
        {
            throw new Exception("dec sp not implemented");
        }

        void cp_n()
        {
            cp_immediate8(ir[1]);
        }

 

        void cp_hl()
        {
            byte b = GetByte(hl);              
            cp_immediate8(b);
        }

        /* compare a to memory address */

       void cp_a_memory(ushort nn)
        {
            ushort addr = nn;

            byte b = GetByte(addr);
            cp_immediate8(b);
        }

        void cp_ix()
        {
            ushort addr = ix;
            addr += ir[2]; //add offfset
            cp_a_memory(addr);
        }

        void cp_iy()
        {
            ushort addr = iy;
            addr += ir[2]; //add offfset
            cp_a_memory(addr);
        }

        void cpl()
        {
            a = (byte)(~a);
        }
    }
}
