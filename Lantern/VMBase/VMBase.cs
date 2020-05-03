using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMBase
{
    public partial class VMBase
    {
        protected byte[] Ram;
        public byte[] InputBuffer = new byte[40];

        public virtual byte GetByte(ushort addr)
        {
            try
            {
                return Ram[addr];
            }
            catch 
            {
                throw new Exception(
                    string.Format("OOB memory access. PC={0:X4}", PC)
                    );
            }
           
        }

        public virtual ushort GetWord(ushort addr)
        {
            byte lo = GetByte( (ushort)(addr) );
            byte hi = GetByte((ushort)(addr+1));
            ushort word = (ushort)( hi * 256 + lo );
            return word;
        }

        public virtual void WriteByte(ushort addr, byte b)
        {
            Ram[addr] = b;
        }

        void WriteWord(ushort addr, ushort w)
        {
            Ram[addr] = (byte)(w % 256); //lo byte 1st
            addr++;
            Ram[addr] = (byte)(w / 256);
        }

        void AdvancePC()
        {
            pc++;
        }

        public void SetRam(byte[] data)
        {
            Ram = data;
        }

        public void Run()
        {
            while (!bQuit)
            {
                Fetch();
                Execute();
            }
        }
    }
}
