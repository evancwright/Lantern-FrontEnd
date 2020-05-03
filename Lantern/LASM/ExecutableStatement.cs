using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LangDef;

namespace LASM
{
    class ExecutableStatement : Statement, IWritable
    {
       
        public string OpCode { get; set; } = "";
        public string Op1 { get; set; } = "";
        public string Op2 { get; set; } = "";
        public string Op3 { get; set; } = "";
        public string TrailingComment { get; set; } = "";
        bool writeFlipped = false; 
        
        public Operand LeftOperand;
        public Operand RightOperand;
        
        /// <summary>
        /// line is the entire line from the input file
        /// </summary>
        /// <param name="line"></param>
        public ExecutableStatement(string line, List<string> symbols)
        {
            Text = line.Trim();
            
            TrimTrailingComment();
            Text = Text.Replace('\t', ' ');
            OpCode = Text.GetFirstPart();
            line = Text.GetRest();
            //create the operands
            if (line.Trim() != "")
            {
                string first = Text.GetFirstOperand();

                LeftOperand = Operand.Parse(first, symbols);

                if (line.Contains(','))
                {//two operands...process the second
                    string second = Text.GetSecondOperand();
                    RightOperand = Operand.Parse(second, symbols);

                    //if there are two operands, they may need to 
                    //be flipped when written
                    if (LeftOperand is ImmediateOperand &&
                        RightOperand is IndexedRegisterPlusConstant)
                    {
                        writeFlipped = true;
                    }

                }
            }

        }

        /// <summary>
        /// Returns the concatenated mode strings of the operands.
        /// Example IMMEDIATE,A
        /// </summary>
        /// <returns></returns>
        public string GetModeText()
        {
            if (LeftOperand != null && RightOperand == null) return LeftOperand.TextType;
            if (LeftOperand != null && RightOperand != null) return LeftOperand.TextType + "," + RightOperand.TextType;
            return "NONE";
        }

        /// <summary>
        /// returns OP code + mode
        /// </summary>
        /// <returns></returns>
        public string GetFullModeText()
        {
            return OpCode + " " + GetModeText();
        }

        /// <summary>
        /// returns length of opcode plus operand
        /// </summary>
        /// <returns></returns>
        public override ushort GetBinaryLength()
        {
            string modeStr = GetFullModeText();
            
            return LangDef.LangDef.GetInstructionSize(modeStr);
        }

        public void SetAddresses(Dictionary<string,ushort> labelOffsets)
        {
            if (LeftOperand != null)
                LeftOperand.SetAddresses(labelOffsets, "MOD"+this.ModuleNumber+"_");

            if (RightOperand != null)
                RightOperand.SetAddresses(labelOffsets, "MOD" + this.ModuleNumber + "_");
        }


        /// <summary>
        /// Writes out the opcode as hex text along with their op codes
        /// </summary>
        /// <returns></returns>
        public override string AsHextText()
        { 
            //write the opcode bytes
            string s = "";
            OpCodeDef ocd = LangDef.LangDef.GetOpCode(OpCode);
            OperandMode mode = LangDef.LangDef.ModeStringToMode(GetModeText());

            s += ocd.GetHextText(mode);

            if (!writeFlipped)
            {
                if (LeftOperand != null)
                {
                    s += LeftOperand.GetHexText();

                    if (RightOperand != null)
                    {
                        s += RightOperand.GetHexText();
                    }
                }
            }
            else
            { //flipped bytes implies two operands
                s += RightOperand.GetHexText();
                s += LeftOperand.GetHexText();
            }
            return s;
        }

        
        public void WriteBinary(BinaryWriter bw)
        {
            OpCodeDef ocd = LangDef.LangDef.GetOpCode(OpCode);
            OperandMode mode = LangDef.LangDef.ModeStringToMode(GetModeText());
            byte[] bytes = ocd.GetBinary(mode);

            WriteBytes(bw,bytes);

            if (!writeFlipped)
            {
                if (LeftOperand != null)
                {

                    LeftOperand.WriteBinary(bw);

                    if (RightOperand != null)
                    {
                        RightOperand.WriteBinary(bw);
                    }
                }
            }
            else
            { //flipped bytes implies two operands
                RightOperand.WriteBinary(bw);
                LeftOperand.WriteBinary(bw);
            }

        }

        /// <summary>
        /// Figures out whether the immediates are bytes or words
        /// for writing
        /// </summary>
        public void SetImmediateSizes()
        {
            string s = GetFullModeText();

            try
            {
                if (LeftOperand is ImmediateOperand)
                {//only 1 operand  ( cp 1  or call nn )
                    int len = LangDef.LangDef.GetImmediateLength(s);
                    (LeftOperand as ImmediateOperand).DataLength = len;
                }

                if (RightOperand != null && RightOperand  is ImmediateOperand)
                {
                    int len = LangDef.LangDef.GetImmediateLength(s);
                    (RightOperand as ImmediateOperand).DataLength = len;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable able to set immediate size!", e);
            }
        }

         
    }
}
