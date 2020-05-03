using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* reset bit */
        void res_b_r(int bit, int rcode)
        {
            byte mask = (byte)(Math.Pow(2, bit));
            mask = (byte)(~mask);
            byte data = Get8BitReg((byte)rcode);
            data = (byte)(data & mask);
            SetReg8((byte)rcode, data);
        }

        /* reset bit */
        void set_b_r(int bit, int rcode)
        {
            byte mask = (byte)(Math.Pow(2, bit));
            mask = (byte)(~mask);
            byte data = Get8BitReg((byte)rcode);
            data = (byte)(data | mask);
            SetReg8((byte)rcode, data);
        }
        
        void res_b_ix(byte bit, byte disp)
        {
            byte data= 0;
            byte offset = ir[2];
            ushort addr = (ushort)(ix + disp);

            addr += offset;
            data = GetByte(addr);

            byte mask = (byte)(Math.Pow(2, bit));
            mask = (byte)(~mask);
            data = (byte)(data & mask); 
            WriteByte(addr, data);
        }

        void set_b_ix(byte bit, byte disp)
        {
            byte data = 0;
            byte offset = ir[2];
            ushort addr = (ushort)(ix+disp);

            addr += offset;
            data = GetByte(addr);

            byte mask = (byte)(Math.Pow(2, bit));
            data = (byte)(data | mask);
            WriteByte(addr, data);
        }


        void res_b_iy(byte bit, byte disp)
        {
            byte data = 0;
            byte offset = ir[2];
            ushort addr = (ushort)(iy + disp);

            addr += offset;
            data = GetByte(addr);

            byte mask = (byte)(Math.Pow(2, bit));
            mask = (byte)(~mask);
            data = (byte)(data & mask);
            WriteByte(addr, data);
        }


    }
}
