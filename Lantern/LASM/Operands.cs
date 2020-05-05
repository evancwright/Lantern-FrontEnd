using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LangDef;

namespace LASM
{

    //an operand can be a register,an address, an immediate value, an index register
    class Operand
    {
        public string Value { get; set; }
        public string TextType { get; set; }
        public bool Resolved { get; set; } = false;

        /// <summary>
        /// Example: ld a,b
        /// </summary>
        /// <param name="line"></param>
        public static Operand Parse(string op, List<string> symbols)
        {
            op = op.Trim();


            if (IsRegister(op))
            {
                return new RegisterOperand(op);
            }
            else if (op.IsNumber())
            {
                return new ImmediateOperand(op);
            }
            else if (symbols.Contains(op))
            {
                return new GlobalLabelImmediateOperand(op);
            }
            else if (op.StartsWith("@"))
            {
                return new LocalLabelImmediateOperand(op);
            }
            else if (!op.IsParenExpression() && op.Contains("+"))
            {
                return new ComputedImmediateOperand(op);
            }
            else if (op.IsParenExpression())
            {//paren expression
                string inner = op.GetParenContents();

                if (!inner.Contains('+'))
                {//a memory operand or a register pair

                    if (inner.IsNumber() || symbols.Contains(inner))
                    {
                        return new MemoryOperand(inner);
                    }
                    else if (indexRegisters.Contains(inner))
                    {
                        return new IndexedRegister(inner);
                    }
                    else
                        throw new Exception("Unhandled case" + inner);

                }
                else
                {//register or memory
                    string[] parts = inner.SplitAndTrim('+');

                    if (indexRegisters.Contains(parts[0]))
                    {//IX + constant
                        string reg = parts[0];
                        string part2 = parts[1];

                        if (part2.IsNumber() || symbols.Contains(part2))
                        {
                            return new IndexedRegisterPlusConstant(reg, part2);
                        }
                    }
                    else
                    {//compound operand. Example: (sentence + 1)
                        return new ComputedMemoryOperand(inner);
                    }

                }
            }

            throw new Exception("Couldn't parse operand:" + op);

        }

        public virtual void SetAddresses(Dictionary<string, ushort> labelOffset, string moduleName = "")
        {
        }

        public virtual string GetHexText()
        {
            return "";
        }

        public virtual void WriteBinary(BinaryWriter bw)
        {
        }

        static bool IsRegister(string s)
        {
            return registers.Contains(s);
        }

        static bool IsImmediate(string s)
        {
            return s.StartsWith("#");
        }

        static bool IsAddress(string s)
        {
            return s.StartsWith("$");
        }

        /// <summary>
        /// IX + 4
        /// IX + score
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        bool IsRegisterPlusConstant(string s)
        {

            if (s == "")
                return false;

            if (!s.Contains('+'))
                return false;

            string[] parts = s.SplitAndTrim('+');

            if (parts.Length != 2)
                return false; //missing second operand , or too many

            if (indexRegisters.Contains(parts[0]))
            {//1st part is index register
                if (parts[1].IsNumber())
                    return true;
                else if (Char.IsLetter(parts[1][0]))
                    return true;
            }
            return false;
        }

        
        static string[] registers =
        {
            "A", "B", "C", "D", "E", "F", "H","L", "AF","BC","DE","HL","IX","IY","SP"
        };

        static string[] indexRegisters =
        {
            "DE","HL", "IX", "IY",
        };


        static string[] registerPairs =
        {
            "AF","BC","DE","HL"
        };
    }
     
    class RegisterOperand : Operand
    {
        public string RegisterName { get; set; }

        public RegisterOperand(string reg)
        {
            TextType = reg;
            RegisterName = reg;
        }
    }

    interface ICompoundOperand
    {
    }

    class ComputedMemoryOperand : MemoryOperand, ICompoundOperand
    {
        OperandList operands;

        public ComputedMemoryOperand(string s)  : base(s)
        {
            TextType = "MEMORY";
            operands = new OperandList(s);
        }

        public override void SetAddresses(Dictionary<string, ushort> labelOffsets, string moduleName = "")
        {
            UshortVal = operands.SetAddresses(labelOffsets, moduleName);
        }
    }

    class ComputedImmediateOperand : ImmediateOperand
    {

        OperandList operands;

        public ComputedImmediateOperand(string s) : base(s)
        {
            TextType = "IMMEDIATE";

            operands = new OperandList(s);
        }

        public override void SetAddresses(Dictionary<string, ushort> labelOffsets, string moduleName = "")
        {
            UshortVal = operands.SetAddresses(labelOffsets, moduleName);
        }
    }

    class IndexedRegister : Operand
    {
        public IndexedRegister(string reg)
        {
            TextType = reg.ToUpper() + "_INDEXED";
        }
    }

    class IndexedRegisterPlusConstant  : ImmediateOperand
    {
        public string Constant { get; set; } = "";
        
        public IndexedRegisterPlusConstant(string reg, string constant) : base(constant)
        {
            if (reg == "IX")
                TextType = "IX_INDEXED_PLUS_OFFSET";
            else if (reg == "IY")
                TextType = "IY_INDEXED_PLUS_OFFSET";

            Constant = constant;
        }
    }
}
