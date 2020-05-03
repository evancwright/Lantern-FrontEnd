using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    {
        /* ld instructions */

        void SetReg8(byte rcode, byte data)
        {
            if (rcode == 0) b = data;
            else if (rcode == 1) c = data;
            else if (rcode == 2) d = data;
            else if (rcode == 3) e = data;
            else if (rcode == 4) h = data;
            else if (rcode == 5) l = data;
            else if (rcode == 7) a = data;
        }

        ushort GetReg16(int rrcode)
        {
            if (rrcode == 0) return bc;
            else if (rrcode == 1) return de;
            else if (rrcode == 2) return hl;
            else if (rrcode == 3) return sp;
            else throw new Exception("Invalid rr code in GetReg16");
        }

        void SetReg16(int rrcode, ushort data)
        {
            if (rrcode == 0) bc = data;
            else if (rrcode == 1) de = data;
            else if (rrcode == 2) hl = data;
            else if (rrcode == 3) sp = data;
            else throw new Exception("Invalid rr code in SetReg16");
        }

        void ld_r_r(byte rcode1, byte rcode2)
        {
            byte b = Get8BitReg(rcode2);
            SetReg8(rcode1, b);
        }
         
        void ld16(ref ushort tgt,  ushort src)
        {
            tgt = src;
        }

        void ld_r_n(byte rcode)
        {
            SetReg8(rcode, ir[1]);
        }

        void ld_a_bc()
        {
            byte b = GetByte(bc);
            SetReg8(0x07,b);
        }

        void ld_a_de()
        {
            byte b = GetByte(de);
            SetReg8(0x07, b);
        }

        void ld_a_hl()
        {
            byte b = GetByte(hl);
            SetReg8(0x07, b);
        }

        void ld_a_mm()
        {
            ushort addr = (ushort)(ir[1] + ir[2] * 256);
            byte b = GetByte(addr);
            SetReg8(0x07, b);
        }


        void ld_mm_a()
        {
            ushort addr = (ushort)( ir[1] + ir[2] * 256 );
            WriteByte(addr, a);
        }

        void ld_r_ix(byte rcode)
        {
            ushort addr = ix;
            addr += ir[2];
            byte b = GetByte(addr);
            SetReg8(rcode, b);
        }

        void ld_r_iy(byte rcode)
        {
            ushort addr = iy;
            addr += ir[2];
            byte b = GetByte(addr);
            SetReg8(rcode, b);
        }

        void ld_bc_a()
        {
            WriteByte(bc, a);            //no flags
        }

        void ld_de_a()
        {
            WriteByte(de, a);            //no flags
        }
        
        void ld_hl_r(byte rcode)
        {
            byte b = Get8BitReg(rcode);
            WriteByte(hl, b);            //no flags
        }

        void ld_bc_nn()
        {
            b = ir[1];
            c = ir[2];
        }
        
        void ld_r_hl(byte rcode)
        {
            byte data= GetByte(hl);
            SetReg8(rcode, data);

        }

        /*de instructions*/
        void ld_de_nn()
        {
            d = ir[2];
            e = ir[1];
        }

        void ld_de_mm()
        {
            ushort addr = (ushort)(ir[1] + ir[2] * 256);

            ushort data = GetWord(addr);
            ushort temp = de;
            ld16(ref temp, data);
            de = temp;
        }

        void ld_mm_rr(int rrcode)
        {
            ushort data = GetReg16(rrcode);
            ushort addr = (ushort)(ir[2] + ir[3] * 256);
            WriteWord(addr, data);
        }

        void ld_mm_ix()
        {
            ushort addr = (ushort)(ir[2] + ir[3] * 256);
            WriteWord(addr,ix);
        }

        void ld_mm_iy()
        {
            ushort addr = (ushort)(ir[2] + ir[3] * 256);
            WriteWord(addr,iy);
        }


        /* hl instructions */
        void ld_hl_nn()
        {
            h = ir[2];
            l = ir[1];
        }

        /*(hl),0*/
        void ld_hl_n()
        {
            ushort addr = (ushort)(h + l * 256);
            WriteByte(addr, ir[1]);
        }


        /// <summary>
        /// ld (ix+d),n
        /// </summary>
        void ld_ix_n()
        {
            ushort addr = ix;
            addr += ir[2];
            WriteByte(addr, ir[3]);
        }


        /* ix instructions */
        /// <summary>
        /// ld ix,nn
        /// </summary>
        void ld_ix_nn()
        {
            ushort val = (ushort)(ir[3] * 256 + ir[2]);
            ix = val;
        }

        void ld_ix_mm()
        {
            ushort addr = (ushort)(ir[2]  + ir[3] * 256);
            ushort w = GetWord(addr);
            ix = w;
        }

        void ld_iy_mm()
        {
            ushort addr = (ushort)(ir[2] + ir[3] * 256);
            ushort w = GetWord(addr);
            ix = w;
        }

        /* ld (ix+3),a */
        void ld_ix_r(byte rcode)
        {
            byte r = Get8BitReg(rcode);
            ushort addr = ix;
            addr += ir[2];
            WriteByte(addr, r);
        }

        void ld_iy_r(byte rcode)
        {
            byte r = Get8BitReg(rcode);
            ushort addr = iy;
            addr += ir[2];
            WriteByte(addr, r);
        }


        /* iy instructions */
        void ld_iy_nn()
        {
            ushort val = (ushort)(ir[3] * 256 + ir[2]);
            iy = val;
        }

  
 
        void ld_sp_memory()
        {
            
        }


        void ld_iy_n()
        {
            ushort addr = ix;
            addr += ir[2];
            WriteByte(addr, ir[3]);
        }

        void ld_mm_de()
        {
            ushort addr = (ushort)(ir[1] + ir[2] * 256);
            WriteWord(addr, de);
        }

        void ld_rr_nn(int rrcode)
        {
            ushort data = (ushort)(ir[1] + ir[2] * 256);
            SetReg16(rrcode, data);
        }

        void ld_rr_mm(int rrcode)
        {
            ushort addr = (ushort)(ir[2] + ir[3] * 256);
            ushort data = GetWord(addr);
            SetReg16(rrcode, data);
        }


    }
}
