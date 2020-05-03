using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LASM
{
    class LocalLabelImmediateOperand : ImmediateOperand
    {
       
        public LocalLabelImmediateOperand(string s) : base(s)
        {
            TextType = "IMMEDIATE";
        }

        public override void SetAddresses(Dictionary<string, ushort> labelOffsets, string moduleName = "")
        {
            string labelName = moduleName + Value;
            try
            {
                UshortVal = labelOffsets[labelName];
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to find local label " + labelName + " table.", ex);
            }
        }
    }

    class GlobalLabelImmediateOperand : ImmediateOperand
    {
        public GlobalLabelImmediateOperand(string s) : base(s)
        {
            TextType = "IMMEDIATE";
        }

        public override void SetAddresses(Dictionary<string, ushort> labelOffsets, string moduleName = "")
        {
            UshortVal = labelOffsets[Value];
        }

    }
}
