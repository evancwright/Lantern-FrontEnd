using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    public partial class VMBase
    {
        public void Execute()
        {
            byte byte1Upper2 = (byte)(ir[0] & 0b1100_0000);
            byte byte1Upper5 = (byte)(ir[0] & 0b1111_1000);
            byte byte1Outer5 = (byte)(ir[0] & 0b1100_0111);
            byte byte1Inner3 = (byte)((ir[0] & 0b0011_1000)>>3);
            byte byte1Lower3 = (byte)((ir[0] & 0b0000_0111));
            byte byte1outer6 = (byte)((ir[0] & 0b1100_1111));
            byte byte1Reg1 = (byte)((ir[0] & 0b0011_1000) >> 3);
            byte byte1Bits45 = (byte)((ir[0] & 0b0011_0000) >> 4);

            byte byte2Lo = (byte)(ir[1] & 0x0F);
            byte byte2Hi = (byte)((ir[1] & 0xF0) >> 4);
            byte byte2Hi2 = (byte)((ir[1] & 0xC0) >> 6);
            byte byte2Lower3 = (byte)((ir[1] & 0x07));
            byte byte2Outer5 = (byte)(ir[1] & 0x46);
            byte byte2Outer6 = (byte)(ir[1] & 0b11001111);
            byte byte2Bits45 = (byte)( (ir[1] & 0b00110000) >> 4);
            byte byte2Bits345 = (byte)((ir[1] & 0b00111000) >> 3);
            byte byte2Upper5 = (byte)((ir[1] & 0b11111000));
            byte byte4Outer = (byte)(ir[3] & 0b1100_0111);
            byte byte4Bits345 = (byte)((ir[3] & 0b0011_1000) >> 3);

            byte bit = (byte)((ir[1] & 0xC0) >> 4);


            if (opcode == 0x00) return;
            else if (opcode == 0xC6) add_immediate();
            else if (byte1Upper5 == 0b10000000) add_r(byte1Lower3);

            else if (opcode == 0x09) add_hl_bc();
            else if (opcode == 0x19) add_hl_de();
            else if (opcode == 0xA0) and_b();
            else if (opcode == 0xE6) add_a_n();

            else if (opcode == 0xdd09) add_ix_bc();
            else if (opcode == 0xdd19) add_ix_de();
            else if (opcode == 0xfd09) add_iy_bc();
            else if (opcode == 0xfd19) add_iy_de();

            else if (ir[0] == 0xCB && byte2Hi2 == 0x01)
            {//bit b,r
                byte reg = byte2Lower3;
                bit_b_r(byte2Bits345, reg);
            }
            else if (ir[0] == 0xcb && byte2Outer5 == 0b0100_0110)
            {
                bit_b_hl(byte2Bits345);
            }
            else if (ir[0] == 0xdd && ir[1] == 0xcb && byte4Outer == 0x46)
            {//bit b,ix+d  bit 0,ix
                bit_b_ix(byte4Bits345, ir[2]);
            }
            else if (ir[0] == 0xfd && ir[1] == 0xcb && byte4Outer == 0x46)
            {//bit b,iy+d
                bit_b_iy(byte4Bits345, ir[2]);
            }
            else if (opcode == 0xCD) call();
            else if (byte1Outer5 == 0b1100_0100) call_cc(byte1Inner3);

            //compare
            else if (ir[0] == 0xbe) cp_hl();
            else if (byte1Upper5 == 0b1011_1000 && byte1Lower3 != 0b00000110) cp_r(byte1Lower3);
            else if (opcode == 0xfe) cp_n();
            else if (opcode == 0xddbe) cp_ix();
            else if (opcode == 0xfdbe) cp_iy();

            else if (opcode == 0x2f) cpl();

            else if (byte1Outer5 == 0b0000_0101) dec_rcode(byte1Inner3);

            else if (opcode == 0x0B) dec_bc();
            else if (opcode == 0x1B) dec_de();
            else if (opcode == 0x2B) dec_hl();
            else if (opcode == 0xDD2B) dec_ix();
            else if (opcode == 0xFD2B) dec_iy();
            else if (opcode == 0x3B) dec_sp();

            else if (opcode == 0xF3) di();
            else if (opcode == 0xFB) ei();

            else if (opcode == 0x10) djnz();

            else if (byte1Outer5 == 0b0000_0100) inc_rcode(byte1Inner3);

            else if (opcode == 0x34) inc_hl_indexed();
            else if (opcode == 0x03) inc_bc();
            else if (opcode == 0x13) inc_de();
            else if (opcode == 0x23) inc_hl();
            else if (opcode == 0x33) inc_sp();
            else if (opcode == 0xDD23) inc_ix();
            else if (opcode == 0xFD23) inc_iy();
            else if (opcode == 0xDD34) inc_ix_plus_d();
            else if (opcode == 0xFD34) inc_iy_plus_d();

            else if (opcode == 0xC3) jmp();
            else if (opcode == 0xDA) jmp_c();
            else if (opcode == 0xFA) jmp_m();
            else if (opcode == 0xF2) jmp_p();
            else if (opcode == 0xD2) jmp_nc();
            else if (opcode == 0xC2) jmp_nz();
            else if (opcode == 0xe9) jmp_hl();
            else if (opcode == 0xCA) jmp_z();

            else if (opcode == 0x18) jr();
            else if (opcode == 0x38) jrc();
            else if (opcode == 0x30) jrnc();
            else if (opcode == 0x28) jrz();
            else if (opcode == 0x20) jrnz();

            else if (opcode == 0x02) ld_bc_a();
            else if (opcode == 0x12) ld_de_a();
            //ld hl,r
            else if (byte1Upper5 == 0b01110000) ld_hl_r(byte1Lower3);

            else if (ir[0] == 0xDD &&
                byte2Upper5 == 0b01110000
                )
            {//ld (ix+n),r or ld (ix+n),0
                int reg = ir[1] & 0x0F;
                ld_ix_r((byte)reg);
            }
            else if (byte1Outer5 == 0b00000110)
            {//ld r,n
                ld_r_n(byte1Inner3);
            }
            else if (byte1Upper2 == 0b01000000 && byte1Lower3 != 0b00000110)
            {
                byte r1 = byte1Reg1;
                byte r2 = byte1Lower3;
                ld_r_r(r1, r2);
            }
            else if (ir[0] == 0xFD &&
                byte2Upper5 == 0b0111_0000
            )
            {//ld (iy+n),r
                int reg = byte2Lower3;
                ld_iy_r((byte)reg);
            }
            else if (opcode == 0xdd36)
            {//ld (ix+d),n
                ld_ix_n();
            }
            else if (opcode == 0xfd36)
            {//ld (iy+d),n
                ld_iy_n();
            }
            else if (opcode == 0x32) ld_mm_a();
            else if (ir[0] == 0xed && byte2Outer6 == 0b0100_0011) ld_mm_rr(byte2Bits45);
            else if (opcode == 0x22) ld_mm_rr(0b00000010);
            else if (opcode == 0xDD22) ld_mm_ix();
            else if (opcode == 0xFD22) ld_mm_iy();
            //  else if (opcode == 0xed73) ld_mm_sp();
            else if (opcode == 0x0a) ld_a_bc();
            else if (opcode == 0x1a) ld_a_de();
            else if (byte1Outer5 == 0b01000110) ld_r_hl(byte1Reg1);
            else if (ir[0] == 0xDD && byte2Outer5 == 0b01000110) ld_r_ix(byte2Bits345);
            else if (ir[0] == 0xFD && byte2Outer5 == 0b01000110) ld_r_iy(byte2Bits345);
            else if (opcode == 0x3A) ld_a_mm();
            else if (ir[0] == 0xED && byte2Outer6 == 0b01001011) ld_rr_mm(byte2Bits45);
            else if (byte1outer6 == 0b00000001) ld_rr_nn(byte1Bits45);
            else if (opcode == 0xdd21) ld_ix_nn();
            else if (opcode == 0xfd21) ld_iy_nn();
            else if (opcode == 0xdd2A) ld_ix_mm();
            else if (opcode == 0xfd2A) ld_iy_mm();
            else if (opcode == 0xEDB0) ldir();
            else if (opcode == 0xED44) neg();
            else if (byte1Upper5 == 0b1011_0000) or_r(byte1Lower3);
            else if (opcode == 0xf6) or_n();
            else if (opcode == 0xb6) or_hl();
            else if (opcode == 0xddb6) or_ix();
            else if (opcode == 0xfdb6) or_iy();
            else if (byte1outer6 == 0b1100_0001) pop_rr(byte1Bits45);
            else if (opcode == 0xdde1) pop_ix();
            else if (opcode == 0xfde1) pop_iy();
            else if (byte1outer6 == 0b1100_0101) push_rr(byte1Bits45);
            else if (opcode == 0xdde5) push_ix();
            else if (opcode == 0xfde5) push_iy();
            else if (ir[0] == 0xCB && byte2Hi2 == 0b00000010) res_b_r(byte2Bits345, byte2Lower3);
            else if (ir[0] == 0xDD && ir[1] == 0xCB && byte4Outer == 0b10000110) res_b_ix(byte4Bits345, ir[2]);
            else if (ir[0] == 0xFD && ir[1] == 0xCB && byte4Outer == 0b10000110) res_b_iy(byte4Bits345, ir[2]);
            //ret
            else if (opcode == 0xC9) ret();
            //set
            else if (ir[0] == 0xCB && byte2Hi2 == 0b00000010) set_b_r(byte2Bits345, byte2Lower3);
            else if (ir[0] == 0xDD && ir[1] == 0xCB && byte4Outer == 0b11000110)
            {
                set_b_ix(byte4Bits345, ir[2]);
            }

            else if (opcode == 0xCB2F) sra_a();
            else if (opcode == 0xCB21) sla_c();
            else if (opcode == 0xCB3F) srl_a();
            //sub
            else if (byte1Upper5 == 0b1001_0000) sub_r(byte1Lower3);
            else if (opcode == 0xD6) sub_n();
            else if (opcode == 0xED79) charout();
            else if (opcode == 0xED51) readkb();
            else if (opcode == 0xED49) anykey();
            else if (opcode == 0xEDB8) cls();
            else if (opcode == 0xEDB3) printcr();
            else if (opcode == 0xEDAB) status_line();
            else if (opcode == 0xED41) setp();
            else if (opcode == 0xED61) save();
            else if (opcode == 0xED69) restore();
            else if (opcode == 0xED59) quit();

            else
                throw new Exception(
                    String.Format("Unhandled op code {0:X4} at {1:X4}", opcode, pc)
                    );

            if (sp < (ushort)(0x44fd + 40))
            {
                throw new Exception("Stack hit input buffer!");
            }
        }

    }
}
