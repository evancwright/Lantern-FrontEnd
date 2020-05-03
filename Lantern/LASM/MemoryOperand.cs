using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LangDef;

namespace LASM
{
    class MemoryOperand : Operand
    {
        public string Address { get; set; }
        public ushort UshortVal { get; set; }

        public MemoryOperand(string s)
        {
            TextType = "MEMORY";
            Address = s;
        }

        public override void SetAddresses(Dictionary<string, ushort> labelOffsets, string moduleName = "")
        {
            string labelName = Address;
            if (labelName.IsDecimalNumber())
            {
                UshortVal = Convert.ToUInt16(Address);
            }
            else if (labelName.IsHexLiteral())
            {
                UshortVal = Address.HexToDecimal();
            }
            else
            {
                try
                {
                    UshortVal = labelOffsets[labelName];
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to find memory label " + labelName + " table.", ex);
                }
            }

        }

        public override string GetHexText()
        {
            //flip the bytes
            byte low = (byte)(UshortVal % 256);
            byte hi = (byte)(UshortVal / 256);
            return string.Format("{0:X2}{1:X2}",
                (byte)low,
                (byte)hi
                );
        }


        public override void WriteBinary(BinaryWriter bw)
        {
            //flip the bytes
            byte low = (byte)(UshortVal % 256);
            byte hi = (byte)(UshortVal / 256);
            bw.Write(low);
            bw.Write(hi);

        }
    }
}
