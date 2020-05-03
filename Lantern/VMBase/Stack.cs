using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    public partial class VMBase
    {
        void pop_rr(int rrcode)
        {
            sp++;
            byte lo = GetByte(sp);
            sp++;
            byte hi = GetByte(sp);
            
            ushort temp = (ushort)(hi * 256 + lo);

            if (rrcode == 0b00) bc = temp;
            else if (rrcode == 0b01) de = temp;
            else if (rrcode == 0b10) hl = temp;
            else if (rrcode == 0b11) af = temp;
             
        }

        void pop_ix()
        {
            sp++;
            byte lo = GetByte(sp);
            sp++;
            byte hi = GetByte(sp);
            
            ix = (ushort)(hi * 256 + lo);
        }

        void pop_iy()
        {
            sp++;
            byte lo = GetByte(sp);
            sp++;
            byte hi = GetByte(sp);
            
            iy = (ushort)(hi * 256 + lo);
        }

        void push_rr(int rrcode)
        {
            ushort reg = 0;

            if (rrcode == 0b00) reg = bc;
            else if (rrcode == 0b01) reg = de;
            else if (rrcode == 0b10) reg = hl;
            else if (rrcode == 0b11) reg = af;
            
            byte lo = (byte)(reg % 256);
            byte hi = (byte)(reg / 256);

            WriteByte(sp,hi);
            sp--;
            
            WriteByte(sp, lo);
            sp--;

        }

        void push_ix()
        {
            byte lo = (byte)(ix % 256);
            byte hi = (byte)(ix / 256);

            WriteByte(sp, hi);
            sp--;

            WriteByte(sp, lo);
            sp--;            
        }

        void push_iy()
        {
            byte lo = (byte)(iy %256);
            byte hi = (byte)(iy / 256);

            WriteByte(sp, hi);
            sp--;
            
            WriteByte(sp, lo);
            sp--;
        }
    }
}
