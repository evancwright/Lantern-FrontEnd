using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangDef
{
    public partial class LangDef
    {
        /// <summary>
        /// populates dasm with disassembly
        /// </summary>
        /// <param name="data"></param>
        /// <param name=""></param>
        public static void Disassemble(byte[] data, int index, List<string> lines)
        {
            List<Tuple<string, string>> statement = new List<Tuple<string, string>>();
         
            int len = 0;
            //get the 1st opcode btyte
            
            for (int i=0; i < 30; i++)
            {
                DASMData dasm = new DASMData();
                len = ParseInstruction(data, index, dasm);
                index += len;
                if (len == 0)
                    break;
                lines.Add(dasm.Text);
            }
        }

        /// <summary>
        /// returns the length of the instruction
        /// </summary>
        /// <param name="data">ram</param>
        /// <param name="index">where to disassemble</param>
        /// <param name="dasm">dasm</param>
        /// <returns></returns>
        static int ParseInstruction(byte[] data, int index, DASMData dasm)
        {
            //copy the data into 
            byte[] ir = new byte[5];

            for (int i=0; i < 5; i++)
            {
                if (i + index >= data.Length)
                    break;

                ir[i] = data[i + index];
            }


            Decode(ir, dasm);
            GetOpCodeText(dasm);
            return dasm.TotalBytes;
        }

        static bool IsRNInstruction(ushort opcode)
        {
            if (opcode == 0xc6 ||  //add n
                opcode == 0xe6 || //and n
                opcode == 0xfe || //cp n
                opcode == 0x3e || //ld a
                opcode == 0x06 || //ld b)
                opcode == 0x0e || //ld c)
                opcode == 0x16 || //ld d)
                opcode == 0x1e || //ld e)
                opcode == 0x26 || //ld h)
                opcode == 0xf6 || //or h)
                opcode == 0xd6 || //sub n
                opcode == 0xee )  //xor n)
            {
                 return true;
            }
             return false;
        }

        static bool GetOpCodeText(DASMData data, Dictionary<string, ushort> labels = null)
        {
            // r,r instructions (no immediates)
            if (data.OpCode == 0x00) data.Text = "nop";
            else if (data.OpCode == 0x86) data.Text = "add a,(hl)";
            else if (data.OpCode == 0x87) data.Text = "add a,a";
            else if (data.OpCode == 0x80) data.Text = "add a,b";
            else if (data.OpCode == 0x81) data.Text = "add a,c";
            else if (data.OpCode == 0x82) data.Text = "add a,d";
            else if (data.OpCode == 0x83) data.Text = "add a,e";
            else if (data.OpCode == 0x84) data.Text = "add a,h";
            else if (data.OpCode == 0x85) data.Text = "add a,l";

            else if (data.OpCode == 0x09) data.Text = "add hl,bc";
            else if (data.OpCode == 0x19) data.Text = "add hl,de";
            else if (data.OpCode == 0x29) data.Text = "add hl,hl";
            else if (data.OpCode == 0x39) data.Text = "add hl,sp";

            else if (data.OpCode == 0xA6) data.Text = "and a,(hl)";
            else if (data.OpCode == 0xA7) data.Text = "and a,a";
            else if (data.OpCode == 0xA0) data.Text = "and a,b";
            else if (data.OpCode == 0xA1) data.Text = "and a,c";
            else if (data.OpCode == 0xA2) data.Text = "and a,d";
            else if (data.OpCode == 0xA3) data.Text = "and a,e";
            else if (data.OpCode == 0xA4) data.Text = "and a,h";
            else if (data.OpCode == 0xA5) data.Text = "and a,l";

            else if (data.OpCode == 0xBE) data.Text = "cp (hl)";
            else if (data.OpCode == 0xBF) data.Text = "cp a";
            else if (data.OpCode == 0xB8) data.Text = "cp b";
            else if (data.OpCode == 0xB9) data.Text = "cp c";
            else if (data.OpCode == 0xBA) data.Text = "cp d";
            else if (data.OpCode == 0xBB) data.Text = "cp e";
            else if (data.OpCode == 0xBC) data.Text = "cp h";
            else if (data.OpCode == 0xBD) data.Text = "cp l";

            else if (data.OpCode == 0x2F) data.Text = "cpl";

            else if (data.OpCode == 0x35) data.Text = "dec (hl)";
            else if (data.OpCode == 0x3D) data.Text = "dec a";
            else if (data.OpCode == 0x05) data.Text = "dec b";
            else if (data.OpCode == 0x0B) data.Text = "dec bc";
            else if (data.OpCode == 0x0D) data.Text = "dec c";
            else if (data.OpCode == 0x15) data.Text = "dec d";
            else if (data.OpCode == 0x1B) data.Text = "dec de";
            else if (data.OpCode == 0x1D) data.Text = "dec e";
            else if (data.OpCode == 0x25) data.Text = "dec h";
            else if (data.OpCode == 0x2B) data.Text = "dec hl";
            else if (data.OpCode == 0x2D) data.Text = "dec l";
            else if (data.OpCode == 0x3B) data.Text = "dec sp";

            else if (data.OpCode == 0x34) data.Text = "inc (hl)";
            else if (data.OpCode == 0x3C) data.Text = "inc a";
            else if (data.OpCode == 0x04) data.Text = "inc b";
            else if (data.OpCode == 0x03) data.Text = "inc bc";
            else if (data.OpCode == 0x0C) data.Text = "inc c";
            else if (data.OpCode == 0x14) data.Text = "inc d";
            else if (data.OpCode == 0x13) data.Text = "inc de";
            else if (data.OpCode == 0x1c) data.Text = "inc e";
            else if (data.OpCode == 0x24) data.Text = "inc h";
            else if (data.OpCode == 0x23) data.Text = "inc hl";
            else if (data.OpCode == 0x2c) data.Text = "inc l";
            else if (data.OpCode == 0x33) data.Text = "inc sp";

            else if (data.OpCode == 0xE9) data.Text = "jp (hl)";

            else if (data.OpCode == 0x02) data.Text = "ld (bc),a";
            else if (data.OpCode == 0x12) data.Text = "ld (de),a";
            else if (data.OpCode == 0x77) data.Text = "ld (hl),a";
            else if (data.OpCode == 0x70) data.Text = "ld (hl),b";
            else if (data.OpCode == 0x71) data.Text = "ld (hl),c";
            else if (data.OpCode == 0x72) data.Text = "ld (hl),d";
            else if (data.OpCode == 0x73) data.Text = "ld (hl),e";
            else if (data.OpCode == 0x74) data.Text = "ld (hl),h";
            else if (data.OpCode == 0x75) data.Text = "ld (hl),l";

            else if (data.OpCode == 0x02) data.Text = "ld (bc),a";
            else if (data.OpCode == 0x12) data.Text = "ld (de),a";
            else if (data.OpCode == 0x77) data.Text = "ld (hl),a";
            else if (data.OpCode == 0x70) data.Text = "ld (hl),b";
            else if (data.OpCode == 0x71) data.Text = "ld (hl),c";
            else if (data.OpCode == 0x72) data.Text = "ld (hl),d";
            else if (data.OpCode == 0x73) data.Text = "ld (hl),e";
            else if (data.OpCode == 0x74) data.Text = "ld (hl),h";
            else if (data.OpCode == 0x75) data.Text = "ld (hl),l";

            else if (data.OpCode == 0x0A) data.Text = "ld a,(bc)";
            else if (data.OpCode == 0x1A) data.Text = "ld a,(de)";
            else if (data.OpCode == 0x7E) data.Text = "ld a,(hl)";

            else if (data.OpCode == 0x7F) data.Text = "ld a,a";
            else if (data.OpCode == 0x78) data.Text = "ld a,b";
            else if (data.OpCode == 0x79) data.Text = "ld a,c";
            else if (data.OpCode == 0x7A) data.Text = "ld a,d";
            else if (data.OpCode == 0x7B) data.Text = "ld a,e";
            else if (data.OpCode == 0x7C) data.Text = "ld a,h";
            else if (data.OpCode == 0x7D) data.Text = "ld a,l";
            // ld b,r
            else if (data.OpCode == 0x47) data.Text = "ld b,a";
            else if (data.OpCode == 0x40) data.Text = "ld b,b";
            else if (data.OpCode == 0x41) data.Text = "ld b,c";
            else if (data.OpCode == 0x42) data.Text = "ld b,d";
            else if (data.OpCode == 0x43) data.Text = "ld b,e";
            else if (data.OpCode == 0x44) data.Text = "ld b,h";
            else if (data.OpCode == 0x45) data.Text = "ld b,l";
            else if (data.OpCode == 0x46) data.Text = "ld b,(hl)";
            //ld c,r
            else if (data.OpCode == 0x4f) data.Text = "ld c,a";
            else if (data.OpCode == 0x48) data.Text = "ld c,b";
            else if (data.OpCode == 0x49) data.Text = "ld c,c";
            else if (data.OpCode == 0x4a) data.Text = "ld c,d";
            else if (data.OpCode == 0x4b) data.Text = "ld c,e";
            else if (data.OpCode == 0x4c) data.Text = "ld c,h";
            else if (data.OpCode == 0x4d) data.Text = "ld c,l";
            else if (data.OpCode == 0x4e) data.Text = "ld c,(hl)";
            //ld d,r
            else if (data.OpCode == 0x57) data.Text = "ld d,a";
            else if (data.OpCode == 0x50) data.Text = "ld d,b";
            else if (data.OpCode == 0x51) data.Text = "ld d,c";
            else if (data.OpCode == 0x52) data.Text = "ld d,d";
            else if (data.OpCode == 0x53) data.Text = "ld d,e";
            else if (data.OpCode == 0x54) data.Text = "ld d,h";
            else if (data.OpCode == 0x55) data.Text = "ld d,l";
            else if (data.OpCode == 0x56) data.Text = "ld d,(hl)";
            //ld e,r
            else if (data.OpCode == 0x5F) data.Text = "ld e,a";
            else if (data.OpCode == 0x58) data.Text = "ld e,b";
            else if (data.OpCode == 0x59) data.Text = "ld e,c";
            else if (data.OpCode == 0x5A) data.Text = "ld e,d";
            else if (data.OpCode == 0x5B) data.Text = "ld e,e";
            else if (data.OpCode == 0x5C) data.Text = "ld e,h";
            else if (data.OpCode == 0x5D) data.Text = "ld e,l";
            else if (data.OpCode == 0x5E) data.Text = "ld e,(hl)";
            //ld h,r
            else if (data.OpCode == 0x67) data.Text = "ld h,a";
            else if (data.OpCode == 0x60) data.Text = "ld h,b";
            else if (data.OpCode == 0x61) data.Text = "ld h,c";
            else if (data.OpCode == 0x62) data.Text = "ld h,d";
            else if (data.OpCode == 0x63) data.Text = "ld h,e";
            else if (data.OpCode == 0x64) data.Text = "ld h,h";
            else if (data.OpCode == 0x65) data.Text = "ld h,l";
            else if (data.OpCode == 0x66) data.Text = "ld h,(hl)";
            //ld l,r
            else if (data.OpCode == 0x6F) data.Text = "ld l,a";
            else if (data.OpCode == 0x68) data.Text = "ld l,b";
            else if (data.OpCode == 0x69) data.Text = "ld l,c";
            else if (data.OpCode == 0x6A) data.Text = "ld l,d";
            else if (data.OpCode == 0x6B) data.Text = "ld l,e";
            else if (data.OpCode == 0x6C) data.Text = "ld l,h";
            else if (data.OpCode == 0x6D) data.Text = "ld l,l";
            else if (data.OpCode == 0x6E) data.Text = "ld l,(hl)";

            //or r,r
            else if (data.OpCode == 0xB7) data.Text = "ld l,a";
            else if (data.OpCode == 0xB0) data.Text = "ld l,b";
            else if (data.OpCode == 0xB1) data.Text = "ld l,c";
            else if (data.OpCode == 0xB2) data.Text = "ld l,d";
            else if (data.OpCode == 0xB3) data.Text = "ld l,e";
            else if (data.OpCode == 0xB4) data.Text = "ld l,h";
            else if (data.OpCode == 0xB5) data.Text = "ld l,l";
            else if (data.OpCode == 0xB6) data.Text = "or (hl)";

            else if (data.OpCode == 0xF1) data.Text = "pop af";
            else if (data.OpCode == 0xC1) data.Text = "pop bc";
            else if (data.OpCode == 0xD1) data.Text = "pop de";
            else if (data.OpCode == 0xE1) data.Text = "pop hl";

            else if (data.OpCode == 0xF5) data.Text = "push af";
            else if (data.OpCode == 0xC5) data.Text = "push bc";
            else if (data.OpCode == 0xD5) data.Text = "push de";
            else if (data.OpCode == 0xE5) data.Text = "push hl";


            else if (data.OpCode == 0xc8) data.Text = "retz";
            else if (data.OpCode == 0xc9) data.Text = "ret";

            //2 byte no immediate instructions
            else if (data.OpCode == 0xdd09) data.Text = "add ix,bc";
            else if (data.OpCode == 0xdd19) data.Text = "add ix,de";
            else if (data.OpCode == 0xfd09) data.Text = "add iy,bc";
            else if (data.OpCode == 0xfd19) data.Text = "add iy,de";

            else if (data.OpCode == 0xdd2B) data.Text = "dec ix";
            else if (data.OpCode == 0xfd2B) data.Text = "dec iy";

            else if (data.OpCode == 0xf3) data.Text = "di";
            else if (data.OpCode == 0xfB) data.Text = "ei";

            else if (data.OpCode == 0xdd2B) data.Text = "inc ix";
            else if (data.OpCode == 0xdd2B) data.Text = "inc iy";

            else if (data.OpCode == 0xed80) data.Text = "neg";
            else if (data.OpCode == 0xed80) data.Text = "ldir";

            else if (data.OpCode == 0xdde1) data.Text = "pop ix";
            else if (data.OpCode == 0xfde1) data.Text = "pop iy";

            else if (data.OpCode == 0xdde5) data.Text = "push ix";
            else if (data.OpCode == 0xfde5) data.Text = "push iy";

            else if (data.OpCode == 0xcb21) data.Text = "sla c";
            else if (data.OpCode == 0xcb2f) data.Text = "sra";
            else if (data.OpCode == 0xcb3f) data.Text = "srl";

            else if (data.OpCode == 0xEDB8) data.Text = "cls";
            else if (data.OpCode == 0xed79) data.Text = "outlin";
            else if (data.OpCode == 0xed41) data.Text = "setp";
            else if (data.OpCode == 0xedb3) data.Text = "newln";
            else if (data.OpCode == 0xed59) data.Text = "quit";
            else if (data.OpCode == 0xed61) data.Text = "save";
            else if (data.OpCode == 0xed69) data.Text = "restore";
            else if (data.OpCode == 0xed51) data.Text = "readkb";
            else if (data.OpCode == 0xEDAB) data.Text = "stsln";


            // r,n instructions
            else if (data.OpCode == 0xc6) { data.Text = String.Format("add {0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0xe6) { data.Text = String.Format("and {0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0xfe) { data.Text = String.Format("cp {0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x3e) { data.Text = String.Format("ld {0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x06) { data.Text = String.Format("ld b,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x0e) { data.Text = String.Format("ld c,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x16) { data.Text = String.Format("ld d,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x1e) { data.Text = String.Format("ld e,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x26) { data.Text = String.Format("ld h,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x2E) { data.Text = String.Format("ld l,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0xf6) { data.Text = String.Format("or {0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0xd6) { data.Text = String.Format("sub {0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0xee) { data.Text = String.Format("xor {0:X2}", data.ImmediateByte1); }

            //relative jumps
            else if (data.OpCode == 0x18) { data.Text = String.Format("jr {0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x38) { data.Text = String.Format("jr c,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x30) { data.Text = String.Format("jr nc,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x28) { data.Text = String.Format("jr z,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x20) { data.Text = String.Format("jr nz,{0:X2}", data.ImmediateByte1); }
            else if (data.OpCode == 0x10) { data.Text = String.Format("djnz {0:X2}", data.ImmediateByte1); }


            //call instructions
            else if (data.OpCode == 0xC4) { data.Text = "call nz," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xCC) { data.Text = "call z," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xCD) { data.Text = "call " + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xC3) { data.Text = "jp " + GetAddr(data.ImmediateWord, labels); }
            //dajpc,fajpm,d2jpnc,c2jpnz,f2jpp,cajz
            else if (data.OpCode == 0xDA) { data.Text = "jp c," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xFA) { data.Text = "jp m," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xD2) { data.Text = "jp nc," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xC2) { data.Text = "jp nz," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xF2) { data.Text = "jp p," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xCA) { data.Text = "jp z, " + GetAddr(data.ImmediateWord, labels); }
            //rel jumps
            else if (data.OpCode == 0x38) { data.Text = "jr c, " + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0x30) { data.Text = "jr nc, " + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0x20) { data.Text = "jr nz, " + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0x28) { data.Text = "jr z, " + GetAddr(data.ImmediateWord, labels); }


            //op nn instructions (ld bc,0000h)
            else if (data.OpCode == 0x32) { data.Text = "ld (" + GetAddr(data.ImmediateWord, labels) + "),a"; }
            else if (data.OpCode == 0xed43) { data.Text = "ld (" + GetAddr(data.ImmediateWord, labels) + "),bc"; }
            else if (data.OpCode == 0xed53) { data.Text = "ld (" + GetAddr(data.ImmediateWord, labels) + "),de"; }
            else if (data.OpCode == 0x22) { data.Text = "ld (" + GetAddr(data.ImmediateWord, labels) + "),hl"; }
            else if (data.OpCode == 0xdd22) { data.Text = "ld (" + GetAddr(data.ImmediateWord, labels) + "),ix"; }
            else if (data.OpCode == 0xfd22) { data.Text = "ld (" + GetAddr(data.ImmediateWord, labels) + "),iy"; }
            else if (data.OpCode == 0xed73) { data.Text = "ld (" + GetAddr(data.ImmediateWord, labels) + "),sp"; }
            else if (data.OpCode == 0x3a) { data.Text = "ld a,(" + GetAddr(data.ImmediateWord, labels) + ")"; }
            else if (data.OpCode == 0xed4b) { data.Text = "ld bc,(" + GetAddr(data.ImmediateWord, labels) + ")"; }
            else if (data.OpCode == 0x01) { data.Text = "ld bc," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xed5b) { data.Text = "ld de,(" + GetAddr(data.ImmediateWord, labels) + ")"; }
            else if (data.OpCode == 0x11) { data.Text = "ld de," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0x2a) { data.Text = "ld hl,(" + GetAddr(data.ImmediateWord, labels) + ")"; }
            else if (data.OpCode == 0x21) { data.Text = "ld hl," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xdd2a) { data.Text = "ld ix,(" + GetAddr(data.ImmediateWord, labels) + ")"; }
            else if (data.OpCode == 0xdd21) { data.Text = "ld ix," + GetAddr(data.ImmediateWord, labels); }
            else if (data.OpCode == 0xfd2a) { data.Text = "ld iy,(" + GetAddr(data.ImmediateWord, labels) + ")"; }
            else if (data.OpCode == 0xfd21) { data.Text = "ld iy," + GetAddr(data.ImmediateWord, labels); }

            else if (data.OpCode == 0xed7b) { data.Text = "ld sp,(" + GetAddr(data.ImmediateWord, labels) + ")"; }
            else if (data.OpCode == 0x31) { data.Text = "ld sp," + GetAddr(data.ImmediateWord, labels); }

            //2 byte opcode, 1 byte immediate
            //ld r, (ix+d)
            else if (data.OpCode == 0xDD7E && (data.ImmediateByte1 & 0x46) == 0x46)
            {//ld r, (ix+d)
                string r = GetRegister((byte)(data.OpCode % 256));

                data.Text = string.Format(
                    "ld {0},(ix+{1})",
                    r,
                    data.ImmediateByte1
                    );
            }
            //ld r, (iy+d)
            else if (data.OpCode == 0xFFDE && (data.ImmediateByte1 & 0x46) == 0x46)
            {
                string r = GetRegister((byte)(data.OpCode % 256));

                data.Text = string.Format(
                    "ld {0},(iy+{1})",
                    r,
                    data.ImmediateByte1
                    );
            }
            //two immediate bytes ( res, bit, set, ld (ix+n),n )
            else if (data.OpCode == 0xDDCB) //res 0,(ix+n) or bit 0,(ix+n), or set 0,(ix+n)
            {
                int hi2 = GetHi2(data.ImmediateByte2);
                int bit = GetBit(data.ImmediateByte1);
                string op = "";
                if (hi2 == 1) op = "bit";
                else if (hi2 == 2) op = "res";
                else if (hi2 == 3) op = "set";

                data.Text = String.Format("{0} {1},(ix+{2})",
                    op,
                    bit,
                    data.ImmediateByte1
                    );
            }
            else if (data.OpCode == 0xFDCB)
            {
                int hi2 = GetHi2(data.ImmediateByte2);
                int bit = GetBit(data.ImmediateByte1);
                string op = "";
                if (hi2 == 1) op = "bit";
                else if (hi2 == 2) op = "res";
                else if (hi2 == 3) op = "set";

                data.Text = String.Format("{0} {1},(iy+{2})",
                    op,
                    bit,
                    data.ImmediateByte1
                    );
            }
            else if (data.OpCode == 0xdd36)
            {//ld (ix+d),n
                data.Text = String.Format("ld (ix+{0}),{1}",
                   data.ImmediateByte1,
                   data.ImmediateByte2
                   );
            }
            else if (data.OpCode == 0xdd36)
            {//ld (ix+d),n
                data.Text = String.Format("ld (ix+{0}),{1}",
                   data.ImmediateByte1,
                   data.ImmediateByte2
                   );
            }
            else if (data.OpCode == 0xfd36)
            {//ld (iy+d),n
                data.Text = String.Format("ld (iy+{0}),{1}",
                   data.ImmediateByte1,
                   data.ImmediateByte2
                   );
            }
            else
            {
                data.Text = "0x??";
                return false;
            }

            return true;
            //immediates, not same size ld ?
        }

        /// <summary>
        /// returns the addr or symbol name if found
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        static string GetAddr(ushort addr, Dictionary<string, ushort> labels)
        {
            if (labels == null)
                return string.Format("{0:X4}", addr);

            foreach (var pair in labels)
            {
                if (pair.Value == addr)
                    return pair.Key;
            } 

            return string.Format("{0:X4}", addr);
        }

        static int GetBit(byte b)
        {
            b = (byte)(b & 0x3f);
            b = (byte)(b >> 3);
            return b;
        }

        static int GetHi2(byte b)
        {
            b = (byte)(b & 0xC0);
            b = (byte)(b >> 6);
            return b;
        }

        static string GetRegister(byte b)
        {
            int r = (b & 38) >> 3;

            if (r == 7) return "a";
            else if (r == 0) return "b";
            else if (r == 1) return "c";
            else if (r == 2) return "d";
            else if (r == 3) return "e";
            else if (r == 4) return "h";
            else if (r == 5) return "l";
            return "?";
        }
    }
}
