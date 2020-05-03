using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* inc instruction */


        void inc_rcode(byte code)
        {
            if (code == 7) inc_a();
            if (code == 0) inc_b();
            if (code == 1) inc_c();
            if (code == 2) inc_d();
            if (code == 3) inc_e();
            if (code == 4) inc_h();
            if (code == 5) inc_l();
        }

        void inc_r(ref byte r)
        {
            /*set sign and zero*/
            r++;

            SetZeroAndSign8(r);
        }

        void inc_rr(ref ushort rr)
        {
            rr++;
        }

        void inc_a()
        {
            byte temp = a;
            inc_r(ref temp);
            a = temp;
        }

        void inc_b()
        {
            byte temp = b;
            inc_r(ref temp);
            b = temp;
        }

        void inc_c()
        {
            byte temp = c;
            inc_r(ref temp);
            c = temp;
        }

        void inc_d()
        {
            byte temp = d;
            inc_r(ref temp);
            d = temp;
        }


        void inc_e()
        {
            byte temp = e;
            inc_r(ref temp);
            e = temp;
        }

        void inc_h()
        {
            byte temp = h;
            inc_r(ref temp);
            h = temp;
        }

        void inc_l()
        {
            byte temp = l;
            inc_r(ref temp);
            l = temp;
        }


        void inc_bc()
        {
            ushort temp = bc;
            inc_rr(ref temp);
            bc = temp;
        }

        void inc_de()
        {
            ushort temp = de;
            inc_rr(ref temp);
            de = temp;
        }

        void inc_hl()
        {
            ushort temp = hl;
            inc_rr(ref temp);
            hl = temp;
        }

        void inc_ix()
        {
            ushort temp = ix;
            inc_rr(ref temp);
            ix = temp;
        }

        void inc_ix_plus_d()
        {
            byte d = ir[2];
            byte b = GetByte((ushort)(ix + d));
            b++;
            WriteByte((ushort)(ix + d), b);
            SetZeroAndSign8(b);
        }

        void inc_iy_plus_d()
        {
            byte d = ir[2];
            byte b = GetByte((ushort)(iy + d));
            b++;
            WriteByte((ushort)(iy + d), b);
            SetZeroAndSign8(b);
        }


        void inc_iy()
        {
            ushort temp = iy;
            inc_rr(ref temp);
            iy = temp;
        }

        void inc_sp()
        {
            sp++;
        }

        void inc_hl_indexed()
        {
            ushort addr = hl;
            byte b = GetByte(addr);
            b++;
            SetZeroAndSign8(b);
            WriteByte(addr, b);
        }

    }
}