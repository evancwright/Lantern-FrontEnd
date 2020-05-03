using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangDef;

namespace LASM
{
    /// <summary>
    /// Utility class used by computed operands
    /// </summary>
    class OperandList
    {
        List<Operand> operands = new List<Operand>();

        public OperandList(string op)
        {
            string[] parts = op.SplitAndTrim('+');

            foreach (string s in parts)
            {
                if (s.IsDecimalNumber() || s.IsHexLiteral())
                    operands.Add(new ImmediateOperand(s));
                else if (Char.IsLetter(s[0]))
                    operands.Add(new GlobalLabelImmediateOperand(s));
                else
                    throw new Exception("Compound operand: don't know how to parse:" + s);
            }
        }

        public ushort SetAddresses(Dictionary<string, ushort> labelOffsets, string moduleName = "")
        {
            ushort sum = 0;
            foreach (ImmediateOperand o in operands)
            {
                o.SetAddresses(labelOffsets);
                sum = (ushort)(sum + o.UshortVal);
            }
            return sum;
        }

    }
}
