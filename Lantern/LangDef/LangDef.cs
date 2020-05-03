using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangDef
{ 
    public static partial class LangDef
    {

        static LangDef()
        {
            //LD
            names = new List<String>();
            opcodes = new List<OpCodeDef>();

            AddOpCode("NOP");
            AddMode("NOP", OperandMode.NONE, 0x0);

            AddOpCode("ADD");
            AddMode("ADD", OperandMode.A_IMMEDIATE, 0xC6);
            AddMode("ADD", OperandMode.A_B, 0x80);
            AddMode("ADD", OperandMode.A_C, 0x81);
            AddMode("ADD", OperandMode.A_D, 0x82);
            AddMode("ADD", OperandMode.A_E, 0x83);
            AddMode("ADD", OperandMode.HL_BC, 0x09);
            AddMode("ADD", OperandMode.HL_DE, 0x19);
            AddMode("ADD", OperandMode.IX_BC, 0xDD, 0x09); //2 byhte
            AddMode("ADD", OperandMode.IX_DE, 0xDD, 0x19);
            AddMode("ADD", OperandMode.IY_BC, 0xFD, 0x09);
            AddMode("ADD", OperandMode.IY_DE, 0xFD, 0x19);
             

            AddOpCode("AND");
            AddMode("AND", OperandMode.IMMEDIATE, 0xE6);
            AddMode("AND", OperandMode.B, 0xA0);

            AddOpCode("ANYKEY");
            AddMode("ANYKEY", OperandMode.NONE, 0xed,0x49);


            AddOpCode("BIT");
            AddMode("BIT", OperandMode.IMMEDIATE_IX_INDEXED_PLUS_OFFSET, 0xDD, 0xCB);
            AddMode("BIT", OperandMode.IMMEDIATE_IY_INDEXED_PLUS_OFFSET, 0xFD, 0xCB);

            AddOpCode("CALL");
            AddMode("CALL", OperandMode.IMMEDIATE, 0xCD);

            AddOpCode("CALLZ");
            AddMode("CALLZ", OperandMode.IMMEDIATE, 0xCC);

            AddOpCode("CALLNZ");
            AddMode("CALLNZ", OperandMode.IMMEDIATE, 0xC4);

            AddOpCode("CLS");
            AddMode("CLS", OperandMode.NONE, 0xED, 0xB8);

            //CMP
            AddOpCode("CP");
            AddMode("CP", OperandMode.IMMEDIATE, 0xFE);
            AddMode("CP", OperandMode.B,0xB8);   
            AddMode("CP", OperandMode.C, 0xB9);  
            AddMode("CP", OperandMode.D, 0xBA);
            AddMode("CP", OperandMode.H, 0xBC);
            AddMode("CP", OperandMode.HL_INDEXED, 0xBE);
            AddMode("CP", OperandMode.IX_INDEXED_PLUS_OFFSET, 0xdd, 0xBE);
            AddMode("CP", OperandMode.IY_INDEXED_PLUS_OFFSET, 0xfd, 0xBE);

            AddOpCode("CPL");
            AddMode("CPL", OperandMode.NONE, 0x2f);

            //DEC
            AddOpCode("DEC");
            AddMode("DEC", OperandMode.A,0x3D);
            AddMode("DEC", OperandMode.B, 0x05); //not actually used
            AddMode("DEC", OperandMode.C, 0x0D); //not actually used
            AddMode("DEC", OperandMode.D, 0x15); //not actually used
            AddMode("DEC", OperandMode.IX, 0xDD, 0x2B); //not actually used
            AddMode("DEC", OperandMode.HL, 0x2B);
            AddMode("DEC", OperandMode.IY, 0xFD, 0x2B);

            AddOpCode("DI");
            AddMode("DI", OperandMode.NONE, 0xF3);

            //DJNZ
            AddOpCode("DJNZ");
            AddMode("DJNZ", OperandMode.IMMEDIATE, 0x10);

            AddOpCode("EI");
            AddMode("EI", OperandMode.NONE, 0xFB);

            AddOpCode("GETLINE");
            AddMode("GETLINE", OperandMode.NONE, 0xed, 0x51);

            AddOpCode("READKB");
            AddMode("READKB", OperandMode.NONE, 0xed, 0x51);


            //INC
            AddOpCode("INC");
            AddMode("INC", OperandMode.A, 0x3C);
            AddMode("INC", OperandMode.B, 0x04);
            AddMode("INC", OperandMode.D, 0x14);
            AddMode("INC", OperandMode.BC, 0x03);
            AddMode("INC", OperandMode.DE, 0x13);
            AddMode("INC", OperandMode.HL, 0x23);
            AddMode("INC", OperandMode.SP, 0x33);
            AddMode("INC", OperandMode.HL_INDEXED, 0x34);
            AddMode("INC", OperandMode.IX, 0xDD, 0x23);
            AddMode("INC", OperandMode.IY, 0xFD, 0x23);
             

            AddOpCode("JP");
            AddMode("JP", OperandMode.IMMEDIATE, 0xC3);
            AddMode("JP", OperandMode.HL_INDEXED, 0xE9);

            AddOpCode("JPZ");
            AddMode("JPZ", OperandMode.IMMEDIATE, 0xCA);

            AddOpCode("JPNZ");
            AddMode("JPNZ", OperandMode.IMMEDIATE, 0xC2);

            AddOpCode("JPC");
            AddMode("JPC", OperandMode.IMMEDIATE, 0xDA);

            AddOpCode("JPP");
            AddMode("JPP", OperandMode.IMMEDIATE, 0xf2);

            AddOpCode("JPM");
            AddMode("JPM", OperandMode.IMMEDIATE, 0xfA);


            AddOpCode("JPNC");
            AddMode("JPNC", OperandMode.IMMEDIATE, 0xD2);

            AddOpCode("JR");
            AddMode("JR", OperandMode.IMMEDIATE, 0x18);

            AddOpCode("JRC");
            AddMode("JRC", OperandMode.IMMEDIATE, 0x38);


            AddOpCode("JRNC");
            AddMode("JRNC", OperandMode.IMMEDIATE, 0x30);

            AddOpCode("JRNZ");
            AddMode("JRNZ", OperandMode.IMMEDIATE, 0x20);

            AddOpCode("JRZ");
            AddMode("JRZ", OperandMode.IMMEDIATE, 0x28);


            AddOpCode("LD");

            AddMode("LD", OperandMode.MEMORY_A, 0x32);
            //    AddMode("MEMORY_B", OperandMode.MEMORY_B);

            AddMode("LD", OperandMode.A_B, 0x78);
            AddMode("LD", OperandMode.A_C, 0x79);
            AddMode("LD", OperandMode.A_D, 0x7A);
            AddMode("LD", OperandMode.A_E, 0x7B);
            AddMode("LD", OperandMode.A_H, 0x7C);
            AddMode("LD", OperandMode.A_L, 0x7D);
            AddMode("LD", OperandMode.A_IMMEDIATE, 0x3E);
            AddMode("LD", OperandMode.A_MEMORY, 0x3A);
            AddMode("LD", OperandMode.A_HL_INDEXED, 0x7E);
            AddMode("LD", OperandMode.A_IX_INDEXED_PLUS_OFFSET, 0xDD, 0x7E);
            AddMode("LD", OperandMode.A_IY_INDEXED_PLUS_OFFSET, 0xFD,0x7E);
            

            AddMode("LD", OperandMode.B_A, 0x47);
            AddMode("LD", OperandMode.B_C, 0x41);
            AddMode("LD", OperandMode.B_D, 0x42);
            AddMode("LD", OperandMode.B_E, 0x43);
            //AddMode("LD", OperandMode.B_H);
            //AddMode("LD", OperandMode.B_L);
            AddMode("LD", OperandMode.B_IMMEDIATE, 0x06);
          //  AddMode("LD", OperandMode.B_MEMORY, );
            AddMode("LD", OperandMode.B_HL_INDEXED, 0x46);

            AddMode("LD", OperandMode.B_IX_INDEXED_PLUS_OFFSET, 0xdd, 0x46);
            AddMode("LD", OperandMode.B_IY_INDEXED_PLUS_OFFSET, 0xfd, 0x46);

            AddMode("LD", OperandMode.BC_IMMEDIATE, 0x1);

            AddMode("LD", OperandMode.C_A, 0x4F);
            AddMode("LD", OperandMode.C_B, 0x48);
            AddMode("LD", OperandMode.C_D, 0x4A);
            AddMode("LD", OperandMode.C_IMMEDIATE, 0x0E);

            AddMode("LD", OperandMode.D_A, 0x57);
            AddMode("LD", OperandMode.D_B, 0x50);
            AddMode("LD", OperandMode.D_IMMEDIATE, 0x16);
            AddMode("LD", OperandMode.D_HL_INDEXED, 0x56);

            AddMode("LD", OperandMode.DE_MEMORY, 0xed,0x5b);
            AddMode("LD", OperandMode.MEMORY_DE, 0xED, 0x53);
            AddMode("LD", OperandMode.DE_IMMEDIATE, 0x11);
            AddMode("LD", OperandMode.DE_HL_INDEXED, 0x56);

            AddMode("LD", OperandMode.H_C, 0x61);

            AddMode("LD", OperandMode.E_A, 0x5F);
            AddMode("LD", OperandMode.E_B, 0x58);
            AddMode("LD", OperandMode.E_C, 0x59);
            AddMode("LD", OperandMode.E_H, 0x5C);
            AddMode("LD", OperandMode.E_HL_INDEXED, 0x5E);            
            AddMode("LD", OperandMode.E_IX_INDEXED_PLUS_OFFSET, 0xdd, 0x5e);
            AddMode("LD", OperandMode.E_IY_INDEXED_PLUS_OFFSET, 0xfd, 0x5e);

            AddMode("LD", OperandMode.H_IMMEDIATE, 0x26);
            AddMode("LD", OperandMode.H_IX_INDEXED_PLUS_OFFSET, 0xdd, 0x66);
            AddMode("LD", OperandMode.H_A, 0x67);
            AddMode("LD", OperandMode.H_L, 0x65);
            AddMode("LD", OperandMode.HL_IMMEDIATE, 0x21);
            AddMode("LD", OperandMode.HL_INDEXED_A, 0x77);
            AddMode("LD", OperandMode.HL_INDEXED_IMMEDIATE, 0x36);

            AddMode("LD", OperandMode.L_IX_INDEXED_PLUS_OFFSET, 0xdd, 0x6e);

            AddMode("LD", OperandMode.IX_IMMEDIATE, 0xDD, 0x21);
            AddMode("LD", OperandMode.IY_IMMEDIATE, 0xFD, 0x21);

            AddMode("LD", OperandMode.IX_MEMORY, 0xDD, 0x2A);

            AddMode("LD", OperandMode.L_A, 0x6f);
            AddMode("LD", OperandMode.L_IMMEDIATE, 0x2E);

            AddMode("LD", OperandMode.IX_INDEXED_PLUS_OFFSET_A, 0xDD, 0x77);

            AddMode("LD", OperandMode.IY_INDEXED_PLUS_OFFSET_A, 0xFD, 0x77);
            AddMode("LD", OperandMode.IY_INDEXED_PLUS_OFFSET_IMMEDIATE, 0xFD, 0x36);

            AddMode("LD", OperandMode.IX_INDEXED_PLUS_OFFSET_B, 0xDD, 0x70);
            AddMode("LD", OperandMode.SP_MEMORY, 0xED,0x7B);
            AddMode("LD", OperandMode.SP_IMMEDIATE, 0x31);

            AddOpCode("LDIR");
            AddMode("LDIR", OperandMode.NONE, 0xED, 0xB0);

            AddOpCode("NEG");
            AddMode("NEG", OperandMode.NONE, 0xED, 0x44);

            AddOpCode("OR");
            AddMode("OR", OperandMode.B, 0xB0);

            AddOpCode("POP");
            AddMode("POP", OperandMode.AF, 0xF1);
            AddMode("POP", OperandMode.BC, 0xC1);
            AddMode("POP", OperandMode.DE, 0xD1);
            AddMode("POP", OperandMode.HL, 0xE1);
            AddMode("POP", OperandMode.IX, 0xDD, 0xE1);
            AddMode("POP", OperandMode.IY, 0xFD, 0xE1);

            AddOpCode("CHOUT");
            AddMode("CHOUT", OperandMode.NONE, 0xED, 0x79);

            AddOpCode("SETP");
            AddMode("SETP", OperandMode.NONE, 0xED, 0x41);

            AddOpCode("NEWLN");
            AddMode("NEWLN", OperandMode.NONE, 0xED, 0xB3);

            AddOpCode("PUSH");
            AddMode("PUSH", OperandMode.AF, 0xF5);
            AddMode("PUSH", OperandMode.BC, 0xC5);
            AddMode("PUSH", OperandMode.DE, 0xD5);
            AddMode("PUSH", OperandMode.HL, 0xE5);
            AddMode("PUSH", OperandMode.IX, 0xDD, 0xE5);
            AddMode("PUSH", OperandMode.IY, 0xFD, 0xE5);

            AddOpCode("QUIT");
            AddMode("QUIT", OperandMode.NONE, 0xed, 0x59);

            AddOpCode("RES");
            AddMode("RES", OperandMode.IMMEDIATE_IX_INDEXED_PLUS_OFFSET, 0xDD, 0xCB);

            //TODO add and indexed mode (jsr [vector])
            AddOpCode("RET");
            AddMode("RET", OperandMode.NONE, 0xC9);

            AddOpCode("RESTORE");
            AddMode("RESTORE", OperandMode.NONE, 0xED, 0x69);

            AddOpCode("SAVE");
            AddMode("SAVE", OperandMode.NONE, 0xed, 0x61);
            
            AddOpCode("SET");
            AddMode("SET", OperandMode.IMMEDIATE_IX_INDEXED_PLUS_OFFSET, 0xDD, 0xCB);

            AddOpCode("SLA");
            AddMode("SLA", OperandMode.C, 0xcb, 0x21);

            AddOpCode("SRA");
            AddMode("SRA", OperandMode.A, 0xcb, 0x2f);
            
            AddOpCode("SRL");
            AddMode("SRL", OperandMode.A, 0xcb, 0x3f);

            AddOpCode("SUB");
            AddMode("SUB", OperandMode.IMMEDIATE, 0xd6);
            AddMode("SUB", OperandMode.B, 0x90);
            AddMode("SUB", OperandMode.C, 0x91);

            AddOpCode("SLINE");
            AddMode("SLINE", OperandMode.NONE, 0xed,0xab);

            //  AddOpCode("ASK");
            //  AddMode("ASK", OperandMode.NONE);

            // AddOpCode("PRINT");
            // AddMode("PRINT", OperandMode.A);
            // AddMode("PRINT", OperandMode.B);
            // AddMode("PRINT", OperandMode.C);

            //    SetBinaryValues();

            AddAddressingModes();
            SetImmediateSizes();
            BuildOpCodeSizes();

        }
 
        private static void AddAddressingModes()
        {
            //build the mode strigns to mode enum table
            AddAddressMode("NONE", OperandMode.NONE);
            AddAddressMode("IMMEDIATE", OperandMode.IMMEDIATE);
            AddAddressMode("IMMEDIATE,IX_INDEXED_PLUS_OFFSET", OperandMode.IMMEDIATE_IX_INDEXED_PLUS_OFFSET);
            AddAddressMode("IMMEDIATE,IY_INDEXED_PLUS_OFFSET", OperandMode.IMMEDIATE_IY_INDEXED_PLUS_OFFSET);

            AddAddressMode("A,IX_INDEXED_PLUS_OFFSET", OperandMode.A_IX_INDEXED_PLUS_OFFSET);
            AddAddressMode("A,IY_INDEXED_PLUS_OFFSET", OperandMode.A_IY_INDEXED_PLUS_OFFSET);
            AddAddressMode("A", OperandMode.A);
            AddAddressMode("B", OperandMode.B);
            AddAddressMode("C", OperandMode.C);
            AddAddressMode("D", OperandMode.D);
            AddAddressMode("E", OperandMode.E);
            //AddMode("F", OperandMode.F); //don't need this mode
            AddAddressMode("H", OperandMode.H);
            AddAddressMode("AF", OperandMode.AF);
            AddAddressMode("BC", OperandMode.BC);
            AddAddressMode("DE", OperandMode.DE);
            AddAddressMode("HL", OperandMode.HL);
            AddAddressMode("IX", OperandMode.IX);
            AddAddressMode("IX,BC", OperandMode.IX_BC);
            AddAddressMode("IY", OperandMode.IY);
            AddAddressMode("SP", OperandMode.SP);
            //jp (hl)
            AddAddressMode("HL_INDEXED", OperandMode.HL_INDEXED);

            //memory as op 1
            AddAddressMode("MEMORY,A", OperandMode.MEMORY_A);
            AddAddressMode("MEMORY,DE", OperandMode.MEMORY_DE);


            //a as op1
            AddAddressMode("A,IMMEDIATE", OperandMode.A_IMMEDIATE);
            AddAddressMode("A,MEMORY", OperandMode.A_MEMORY);
            AddAddressMode("A,B", OperandMode.A_B);
            AddAddressMode("A,C", OperandMode.A_C);
            AddAddressMode("A,D", OperandMode.A_D);
            AddAddressMode("A,E", OperandMode.A_E);
            AddAddressMode("A,H", OperandMode.A_H);
            AddAddressMode("A,L", OperandMode.A_L);
            AddAddressMode("A,HL_INDEXED", OperandMode.A_HL_INDEXED);
            AddAddressMode("A,IX_INDEXED_PLUS_OFFSET", OperandMode.A_IX_INDEXED_PLUS_OFFSET);
            AddAddressMode("A,IY_INDEXED_PLUS_OFFSET", OperandMode.A_IY_INDEXED_PLUS_OFFSET);

            //b as op1
            AddAddressMode("B,IMMEDIATE", OperandMode.B_IMMEDIATE);
            AddAddressMode("B,MEMORY", OperandMode.B_MEMORY);
            AddAddressMode("B,A", OperandMode.B_A);
            AddAddressMode("B,C", OperandMode.B_C);
            AddAddressMode("B,D", OperandMode.B_D);
            AddAddressMode("B,E", OperandMode.B_E);
            AddAddressMode("B,HL_INDEXED", OperandMode.B_HL_INDEXED);
            AddAddressMode("B,IX_INDEXED_PLUS_OFFSET", OperandMode.B_IX_INDEXED_PLUS_OFFSET);
            AddAddressMode("B,IY_INDEXED_PLUS_OFFSET", OperandMode.B_IY_INDEXED_PLUS_OFFSET);

            AddAddressMode("BC,IMMEDIATE", OperandMode.BC_IMMEDIATE);

            AddAddressMode("C,A", OperandMode.C_A);
            AddAddressMode("C,B", OperandMode.C_B);
            AddAddressMode("C,D", OperandMode.C_D);
            AddAddressMode("C,IMMEDIATE", OperandMode.C_IMMEDIATE);
            //d as op1

            AddAddressMode("D,IMMEDIATE", OperandMode.D_IMMEDIATE);
            AddAddressMode("D,A", OperandMode.D_A);
            AddAddressMode("D,B", OperandMode.D_B);
            AddAddressMode("D,HL_INDEXED", OperandMode.D_HL_INDEXED);


            AddAddressMode("DE,MEMORY", OperandMode.DE_MEMORY);
            AddAddressMode("DE,IMMEDIATE", OperandMode.DE_IMMEDIATE);

            AddAddressMode("SP,IMMEDIATE", OperandMode.SP_IMMEDIATE);

            AddAddressMode("E,A", OperandMode.E_A);
            AddAddressMode("E,B", OperandMode.E_B);
            AddAddressMode("E,C", OperandMode.E_C);
            AddAddressMode("E,H", OperandMode.E_H);
            AddAddressMode("E,HL_INDEXED", OperandMode.E_HL_INDEXED);
            AddAddressMode("E,IX_INDEXED_PLUS_OFFSET", OperandMode.E_IX_INDEXED_PLUS_OFFSET);
            AddAddressMode("E,IY_INDEXED_PLUS_OFFSET", OperandMode.E_IY_INDEXED_PLUS_OFFSET);

            AddAddressMode("H,A", OperandMode.H_A);
            AddAddressMode("H,C", OperandMode.H_C);
            AddAddressMode("H,L", OperandMode.H_L);
            AddAddressMode("H,IMMEDIATE", OperandMode.H_IMMEDIATE);
            AddAddressMode("H,IX_INDEXED_PLUS_OFFSET", OperandMode.H_IX_INDEXED_PLUS_OFFSET);

            //hl as op 1
            AddAddressMode("HL_INDEXED,A", OperandMode.HL_INDEXED_A);
            AddAddressMode("HL,IMMEDIATE", OperandMode.HL_IMMEDIATE);
            AddAddressMode("HL_INDEXED,IMMEDIATE", OperandMode.HL_INDEXED_IMMEDIATE);
            AddAddressMode("HL,DE", OperandMode.HL_DE);
            AddAddressMode("HL,BC", OperandMode.HL_BC);

            AddAddressMode("L,A", OperandMode.L_A);
            AddAddressMode("L,IMMEDIATE", OperandMode.L_IMMEDIATE);
            AddAddressMode("L,IX_INDEXED_PLUS_OFFSET", OperandMode.L_IX_INDEXED_PLUS_OFFSET);

            //ix as op 1
            AddAddressMode("IX,MEMORY", OperandMode.IX_MEMORY);
            AddAddressMode("IX,IMMEDIATE", OperandMode.IX_IMMEDIATE);
            AddAddressMode("IX,DE", OperandMode.IX_DE);
            AddAddressMode("IX_INDEXED_PLUS_OFFSET", OperandMode.IX_INDEXED_PLUS_OFFSET);
            AddAddressMode("IX_INDEXED_PLUS_OFFSET,A", OperandMode.IX_INDEXED_PLUS_OFFSET_A);
            AddAddressMode("IX_INDEXED_PLUS_OFFSET,B", OperandMode.IX_INDEXED_PLUS_OFFSET_B);
            //iy as op 2
            AddAddressMode("IY,IMMEDIATE", OperandMode.IY_IMMEDIATE);
            AddAddressMode("IY,DE", OperandMode.IY_DE);
            AddAddressMode("IY_INDEXED_PLUS_OFFSET", OperandMode.IY_INDEXED_PLUS_OFFSET);
            AddAddressMode("IY_INDEXED_PLUS_OFFSET,A", OperandMode.IY_INDEXED_PLUS_OFFSET_A);
            AddAddressMode("IY_INDEXED_PLUS_OFFSET,IMMEDIATE", OperandMode.IY_INDEXED_PLUS_OFFSET_IMMEDIATE);

            AddAddressMode("SP,MEMORY", OperandMode.SP_MEMORY);

            //inner function
            void AddAddressMode(string s, OperandMode m, int dataLength = 0)
            {
                if (!modeStringToEnumTable.ContainsKey(s) &&
                    !modeStringToEnumTable.ContainsValue(m))
                {
                    modeStringToEnumTable.Add(s, m);
                }
            }
        }

        private static void AddOpCode(string name)
        {
            names.Add(name);
            opcodes.Add(new OpCodeDef(name.ToUpper()));
        }

        /// <summary>
        /// Adds a mode to the opcode def table
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="mode"></param>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <param name="byte3"></param>
        private static void AddMode(string opcode, OperandMode mode, byte byte1, byte byte2=0, byte byte3 = 0)
        {
            byte[] codes;

            if (byte3 != 0 && byte2 != 0)
            {
                codes = new byte[]
                {
                    byte1,byte2,byte3
                };
            }
            else if (byte3 == 0 && byte2 != 0)
            {
                codes = new byte[]
                {
                    byte1,byte2
                };
            }
            else 
            {
                codes = new byte[]
                {
                    byte1
                };
            }

                

            OpCodeDef op = opcodes.Find(
                    (o) => { return o.Name == opcode; }
                    );

            if (op == null)
                throw new Exception("Opcode " + opcode + " not found while trying to add mode" + mode.ToString());

            op.AddMode(mode, codes);

            
        }

        public static bool HasOpCode(string op)
        {
            return names.Contains(op.ToUpper());
        }


        public static OpCodeDef GetOpCode(string opName)
        {
            OpCodeDef o = opcodes.Find(
               (OpCodeDef op) => { return op.Name == opName; }
               );

            if (o == null)
            {
                throw new Exception(opName + " has no modes!");
            }

            return o;

        }
    

        public static bool SupportsMode(string opCode, string mode)
        {

            OpCodeDef o = opcodes.Find(
                (OpCodeDef op) => { return op.Name == opCode; }
                );

            if (o == null)
            {
                throw new Exception(opCode + " has no modes!");
            }

            return o.SupportsMode(mode);

        }


        public static OperandMode ModeStringToMode(string s)
        {
            if (!modeStringToEnumTable.ContainsKey(s))
                throw new Exception("Mode note found in table! " + s);

            return modeStringToEnumTable[s];
        }

        /// <summary>
        /// Must be 5 chars long and start with $
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        static bool IsAddress(string operand)
        {
            return (operand.Length == 5 && operand.First() == '$');
        }


        static bool IsImmediate(string operand)
        {
            return ((operand.Length == 3 || operand.Length == 5) && operand.First() == '#');
        }

        static bool IsIndexRegister(string operand)
        {
            return operand == "X" || operand == "Y";
        }


        public static int GetImmediateLength(string opcode)
        {
            

            if (ImmediateSizeTable.ContainsKey(opcode))
            {
                return ImmediateSizeTable[opcode];
            }
            throw new Exception("opcode " + opcode + " not found in immediate size table");

        }

        public static bool IsRelativeJump(string opcode)
        {
            return relativeJumps.Contains(opcode);
        }

        static string[] relativeJumps = { "JR", "JRC", "JRNC", "JRZ", "JRNZ", "DJNZ" };

        static string[] registers =
        {
            "AH","AL","BH","BL","CH","CL",
            "A","B","C","X","Y"
        };

        //valid opcode names
        static Dictionary<string, OperandMode> modeStringToEnumTable = new Dictionary<string, OperandMode>();

        public static Dictionary<string, byte> OpCodeTable = new Dictionary<string, byte>();

        //how big the immediates are in an opcode
        static Dictionary<string, int> ImmediateSizeTable = new Dictionary<string, int>();

        static List<string> names;
        static List<OpCodeDef> opcodes;
    }

   


    public enum OperandMode {
        A, B, C, D, E, H, L, IX, IY,  //INC A
        AF, BC, DE, HL,SP ,
        
        IMMEDIATE_IX_INDEXED_PLUS_OFFSET,
        IMMEDIATE_IY_INDEXED_PLUS_OFFSET,

        //a as op 1
        A_IMMEDIATE,
        A_MEMORY,
        A_B, A_C, A_D, A_E, A_H, A_L, //MOV A,B

        A_HL_INDEXED,

        A_IX_INDEXED_PLUS_OFFSET,
        A_IY_INDEXED_PLUS_OFFSET,

        //b as op 1

        B_A,
        B_C,
        B_D,
        B_E,
        B_IMMEDIATE,
        B_MEMORY,
        B_HL_INDEXED,

        B_IX_INDEXED_PLUS_OFFSET,
        B_IY_INDEXED_PLUS_OFFSET,

        //c as op1
        C_IMMEDIATE,
        C_A,
        C_B,
        C_D,
        
        //d as op1
        D_IMMEDIATE,
        D_A,
        D_B,
        D_HL_INDEXED,

        DE_HL_INDEXED,

        //e as op 1
        E_IMMEDIATE,
        E_A,
        E_B,
        E_C,
        E_D,
        E_H,
        E_HL_INDEXED,
        E_IX_INDEXED_PLUS_OFFSET,
        E_IY_INDEXED_PLUS_OFFSET,
    
        //no f modes
        
        //h modes
        H_IMMEDIATE,
        H_A,
        H_C,
        H_L,
        H_IX_INDEXED_PLUS_OFFSET,

        //l modes
        L_IMMEDIATE,
        L_A,
       
        L_IX_INDEXED_PLUS_OFFSET,

        //af
        // no modes

        //bc
        BC_IMMEDIATE,
        
        //de
        DE_IMMEDIATE,
        DE_MEMORY,

        //hl
        HL_INDEXED,
        HL_IMMEDIATE,
        HL_MEMORY,
        HL_INDEXED_A,
        HL_INDEXED_IMMEDIATE,
        HL_BC,
        HL_DE,

        //ix as op1
        IX_IMMEDIATE,
        IX_BC,
        IX_DE,
        //IX_INDEXED,
        IX_MEMORY,
        IX_INDEXED_PLUS_OFFSET,
//        IX_INDEXED_PLUS_OFFSET_IMMEDIATE,
        IX_INDEXED_PLUS_OFFSET_A,
        IX_INDEXED_PLUS_OFFSET_B,

        //iy as op2
        IY_BC,
        IY_DE,
        IY_IMMEDIATE,
        IY_INDEXED_PLUS_OFFSET,
        IY_INDEXED_PLUS_OFFSET_A,
        IY_INDEXED_PLUS_OFFSET_IMMEDIATE,
        //jp (hl)


        //store instructions
        IMMEDIATE_A,
        MEMORY_A,
        MEMORY_DE,  
                
        SP_MEMORY,
        SP_IMMEDIATE,
       // INDEXED_IX_PLUS_CONSTANT,
       // INDEXED_IY_PLUS_CONSTANT,
         
        Invalid,
        NONE,
        IMMEDIATE
    };


}
