using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* decrement instructions */
        void dec_rcode(byte rcode)
        {
            if (rcode == 7) dec_a();
            else if (rcode == 0) dec_b();
            else if (rcode == 1) dec_c();
            else if (rcode == 2) dec_d();
            else if (rcode == 3) dec_e();
            else if (rcode == 4) dec_h();
            else if (rcode == 5) dec_l();


        }

        /*sets z and s flags */
        void dec_r(ref byte r)
        {
            r--;
            SetZeroAndSign8(r);
        }

        void dec_rr(ref ushort rr)
        {
            rr--; /* flags not affected*/
        }

        void dec_a()
        {
            byte temp = a;
            dec_r(ref temp);
            a = temp;
        }

        void dec_b()
        {
            byte temp = b;
            dec_r(ref temp);
            b = temp;
        }

        void dec_c()
        {
            byte temp = c;
            dec_r(ref temp);
            c = temp;
        }

        void dec_d()
        {
            byte temp = d;
            dec_r(ref temp);
            d = temp;
        }

        void dec_e()
        {
            byte temp = e;
            dec_r(ref temp);
            e = temp;
        }

        void dec_h()
        {
            byte temp = h;
            dec_r(ref temp);
            h = temp;
        }

        void dec_l()
        {
            byte temp = l;
            dec_r(ref temp);
            l = temp;
        }

        void dec_bc()
        {
            ushort temp = bc;
            dec_rr(ref temp);
            bc = temp;
        }

        void dec_de()
        {
            ushort temp = de;
            dec_rr(ref temp);
            de = temp;
        }


        void dec_hl()
        {
            ushort temp = hl;
            dec_rr(ref temp);
            hl = temp;
        }

        void dec_ix()
        {
            ushort temp = ix;
            dec_rr(ref temp);
            ix = temp;
        }

        void dec_iy()
        {
            ushort temp = iy;
            dec_rr(ref temp);
            iy = temp;
        }

        void djnz()
        {
            dec_b();

            if (zeroBit == 0)
                jmprel();
        }


        //disable interrupts
        void di()
        {
        }

        //enable interrupts
        void ei()
        {
        }
    }
}
