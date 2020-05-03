using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LangDef;

namespace LASM
{
    class ImmediateOperand : Operand
    {
        public ushort UshortVal { get; set; }
        public int DataLength { get; set; }

        public ImmediateOperand(string op)
        {
            try
            {
                TextType = "IMMEDIATE";
                if (op.IsDecimalNumber())
                    UshortVal = op.ToUInt16();
                if (op.IsHexByteConstant())
                {
                    UshortVal = op.HexToDecimal();
                }
                else if (op.IsHexWordConstant())
                {
                    UshortVal = op.HexToDecimal();
                }
                Value = op;
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't create immedate: " + op, ex);
            }
        }

        public override string GetHexText()
        {
            if (DataLength == 1)
            {
                byte b = (byte)UshortVal;
                return String.Format("{0:X2}", b);
            }
            else if (DataLength == 2)
            {

                byte low = (byte)(UshortVal % 256);
                byte hi = (byte)(UshortVal / 256);
                return string.Format("{0:X2}{1:X2}",
                    (byte)low,
                    (byte)hi
                    );

//                return String.Format("{0:X4}", UshortVal);
            }

            return "";
        }


        public override void WriteBinary(BinaryWriter bw)
        {
            if (DataLength == 1)
            {
                byte b = (byte)UshortVal;
                bw.Write(b);
            }
            else if (DataLength == 2)
            {
                bw.Write(UshortVal);
            }

        }
    }
}
