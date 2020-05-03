using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangDef
{
    static partial class LangDef
    {
        /// <summary>
        /// returns the length
        /// </summary>
        /// <param name="ir"></param>
        /// <returns></returns>
        public static void Decode(byte[] ir, DASMData data)
        {
            byte x, y, z, p, q;
            //x is 2 highest bits
            x = (byte)(ir[0] & (byte)0xC0);

            x = (byte)(x >> 6);

            //y = bits 5,4,3
            y = (byte)(ir[0] & 0x38); /* 00111000 */
            y = (byte)(y >> 3);

            //z = lowest three bits
            z = (byte)(ir[0] & 0x07);

            /*p is bits 4,5*/
            p = (byte)(ir[0] & 0x30);
            p = (byte)(p >> 4);

            /*q = bit 3*/
            q = (byte)(ir[0] & 0x08);
            q = (byte)(q >> 3);

            if (IsPrefix(ir) == false)
            {//1 byte op code

                data.OpCode = ir[0];
                data.OpCodeLength = 1;
                data.TotalBytes = 1;

                if (HasImmediateByte(x, y, z))
                {
                    data.ImmediateByte1 = ir[1];
                    data.ImmediateSize = 1;
                    data.TotalBytes = 2;
                }
                else if (HasImmediateWord(p, q, x, y, z) == true)
                {
                    data.ImmediateWord = (ushort)(ir[1] * 256 + ir[2]);
                    data.OpCodeLength = 1;
                    data.TotalBytes = 3;
                }
                else
                {
                    data.ImmediateSize = 0;
                }
            }
            else
            {//is a prefix (2 byte opcode)
                data.OpCodeLength = 2;
                data.OpCode = (ushort)(ir[0] * 256 + ir[1]);
                
                //how do we figure out if there's 0,1,or 2 immediates?
                if (PrefixedWithImmediateByte(ir))
                {//1 byte immediate
                    data.ImmediateByte1 = ir[2];
                    data.NumOperands = 1;
                    data.ImmediateSize = 1;
                    data.TotalBytes = 3;
                }
                else if (PrefixedWith2ImmediateBytes(ir))
                {//2 immediates  (bit (ix+d),n)
                    data.ImmediateByte1 = ir[2];
                    data.ImmediateByte2 = ir[3];
                    data.NumOperands = 2;
                    data.ImmediateSize = 1;
                    data.TotalBytes = 4;
                }
                else if (PrefixedWithImmediateWord(ir))
                {//2 immediates (ld ix,nn)
                    data.ImmediateByte1 = ir[2];
                    data.ImmediateByte2 = ir[3];
                    data.NumOperands = 2;
                    data.ImmediateSize = 1;
                    data.TotalBytes = 4;
                }
                else
                {
                    data.TotalBytes = 2;
                    data.NumOperands = 0;
                    data.ImmediateSize = 0;
                }
            }
        }

        //for 1 byte op codes
        static bool HasImmediateByte(byte x, byte y, byte z)
        {
            if (x == 0)
            {
                if (z == 0 && y >= 2 && y <= 7)
                    return true; //rel jump
                if (z == 6) //8 bit ld immmediate
                    return true;
            }
            else if (x == 3 && z == 6)
                return true; //add 6

            return false;
        }

        //this is for 1 byte opcodes
        static bool HasImmediateWord(byte p, byte q, byte x, byte y, byte z)
        {
            if (x == 0)
            {
                if (z == 1 && q == 0)
                {
                    return true; // ld rr,nn
                }
                if (z == 2)
                {/* ld rr,nn or (nn)*/
                    if (p == 2 || p == 3)
                        return true; // ld rr,nn
                }
            }
            else if (x == 3)
            {
                if (z == 2)
                    return true; //jp cc,nn

                if (z == 3 && y == 0)
                    return true; //jp nn
                if (z == 4)
                    return true; //call cc,nn
                if (z == 5 && q == 1 && p == 00)
                    return true;
            }
            return false;
        }

        /* return 1 if ir[0] is a prefix for a 2 byte op code */
        static bool IsPrefix(byte[] ir)
        {
            byte b = ir[0];
            if (b == 0xCB ||
                b == 0xDD ||
                b == 0xED ||
                b == 0xFD)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Is this a 2 byte instruction with a 1 byte operand
        /// </summary>
        /// <param name="ir"></param>
        /// <returns></returns>
        static bool PrefixedWithImmediateByte(byte[] ir)
        {
            if (ir[0] == 0xDD || ir[0] == 0xfd)
            {
                if (ir[1] == 0xBE ||
                ir[1] == 0x7E ||
                ir[1] == 0x46 ||
                ir[1] == 0x53 ||
                ir[1] == 0x5e ||
                ir[1] == 0x66 ||
                ir[1] == 0x6e ||
                ir[1] == 0x77 ||
                ir[1] == 0x70)
                    return true;
            }
            return false;
        }


        static bool PrefixedWith2ImmediateBytes(byte[] ir)
        {
            //	DDCB006E  03676         bit OPEN_BIT,(ix+d) ; test open prop bit
            // DDCB00FE  03726                 set LOCKED_BIT,(ix+d)
            //  DDCB00AE  03678         res OPEN_BIT,(ix)
            //  FD360300  01467         ld (iy+3),0  ; null terminate buffer
            //  FD21BC48  01462                 ld iy,itoabuffer
            if (ir[0] == 0xFD || ir[0] == 0xDD)
            {
                if (ir[1] == 0xCB || ir[1] == 0x36)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Assume the prefix has already been tested
        /// </summary>
        /// <param name="ir"></param>
        /// <returns></returns>
        static bool PrefixedWithImmediateWord(byte[] ir)
        {
            if (ir[0] == 0xFD || ir[0] == 0xDD)
            {
                if (ir[1] == 0x43 ||
                    ir[1] == 0x53 ||
                    ir[1] == 0x73 ||
                    ir[1] == 0x4b ||
                    ir[1] == 0x5b ||
                    ir[1] == 0x2a ||
                    ir[1] == 0x21 ||
                    ir[1] == 0x22)
                {
                    return true;
                }

            }
            return false;
        }
    }
        
}

