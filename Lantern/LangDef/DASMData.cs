using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangDef
{
    public class DASMData
    {
        public ushort OpCode;
        public byte ImmediateByte1;
        public byte ImmediateByte2;
        public ushort ImmediateWord;
        public int OpCodeLength = 0;
        public int ImmediateSize = 0;
        public int NumOperands = 0;
        public string Text = "";
        public int TotalBytes = 0;
    }
}
