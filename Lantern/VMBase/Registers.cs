using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    partial class VMBase
    { 
        //registers

        byte a, f;
        byte b, c;
        byte d, e;
        byte h, l;

        ushort ix = 0;
        ushort iy = 0;
        ushort sp = 0;

        byte[] ir = new byte[4]; //instruction register
        ushort pc = 0;
        public ushort PC { get { return pc; } }
        ushort opcode=0;
        //flags
        byte zeroBit = 0;
        byte signBit = 0;
        byte carryBit = 0;

        public byte ZF  { get { return zeroBit; } }
        public byte MF { get { return signBit; } }
        public byte CF { get { return carryBit; } }

        public byte A
        {
            get { return a; }
        }
        

        public ushort af
        {
            get
            {
                return (ushort)((a * 256) + f);
            }
            set
            {
                a = (byte)(value / 256);
                f = (byte)(value % 256);
            }
        }

        public ushort BC { get { return bc; } }

        public ushort bc
        {
            get
            {
                return (ushort)((b * 256) + c);
            }
            set
            {
                b = (byte)(value / 256);
                c = (byte)(value % 256);
            }
        }

        public ushort DE { get { return de; } }

        public ushort de
        {
            get
            {
                return (ushort)((d * 256) + e);
            }
            set
            {
                d = (byte)(value / 256);
                e = (byte)(value % 256);
            }
        }

        public ushort HL { get { return hl; } }

        public ushort hl
        {
            get
            {
                return (ushort)((h * 256) + l);
            }
            set
            {
                h = (byte)(value / 256);
                l = (byte)(value % 256);
            }
        }

        public ushort SP { get { return sp; } }

        public ushort IX { get { return ix; } }
        public ushort IY { get { return iy; } }

        void SetCarryBit8(int sum)
        {
            carryBit = 0;
            if (sum > 255)
                carryBit = 1;
        }

        void SetZeroAndSign8(byte c)
        {
            zeroBit = 0;
            if (c == 0)
                zeroBit = 1;

            signBit = 0;
            if ((c & 127) == 128)
                signBit = 1;
        }

        void SetZeroAndSign16(ushort s)
        {
            /*set sign and zero*/
            zeroBit = 0;
            if (s == 0)
                zeroBit = 1;

            signBit = 0;
            if ((s & 0x7FFF) != 0)
                signBit = 1;
        }

        //sets zero and sign flags
        void SetFlagsA()
        {
            if (a == 0)
                zeroBit = 1;
            else
                zeroBit = 0;

            if ((a & 128) == 1)
                signBit = 1;
            else
                signBit = 0;
        }


    }


}
