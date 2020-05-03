using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangDef
{
    static partial class LangDef
    {
        /// <summary>
        /// Returns the length of the opcode (factoring in the addressing mode)
        /// and any operands
        /// </summary>
        /// <param name="op">"Example: LD A,HL_INDEXED</param>
        /// <returns></returns>
        public static ushort GetInstructionSize(string op)
        {
            if (instructionSizes.ContainsKey(op))
            {
                return (ushort)(instructionSizes[op]);
            }
            else
                throw new Exception("Instruction size table doesn't contain an entry for " + op);
        }

        static void BuildOpCodeSizes()
        {
            instructionSizes["ADD IMMEDIATE"] = 2;
            instructionSizes["ADD A,IMMEDIATE"] = 2;
            instructionSizes["ADD A,B"] = 1;
            instructionSizes["ADD A,C"] = 1;
            instructionSizes["ADD A,D"] = 1;
            instructionSizes["ADD A,E"] = 1;
            instructionSizes["ADD HL,BC"] = 1;
            instructionSizes["ADD HL,DE"] = 1;
           // instructionSizes["ADD IX,BC"] = 2;
            instructionSizes["ADD IX,DE"] = 2;
            instructionSizes["ADD IX,BC"] = 2;
            instructionSizes["ADD IY,BC"] = 2;
            instructionSizes["ADD IY,DE"] = 2;

            instructionSizes["AND B"] = 1;
            instructionSizes["AND IMMEDIATE"] = 2;

            instructionSizes["BIT IMMEDIATE,IX_INDEXED_PLUS_OFFSET"] =  4;
            instructionSizes["BIT IMMEDIATE,IY_INDEXED_PLUS_OFFSET"] = 4;

            instructionSizes["CALL IMMEDIATE"] = 3;
            instructionSizes["CALLZ IMMEDIATE"] = 3;
            instructionSizes["CALLNZ IMMEDIATE"] = 3;
            instructionSizes["CALLP IMMEDIATE"] = 3;
            instructionSizes["CALLM IMMEDIATE"] = 3;
            instructionSizes["CALLC IMMEDIATE"] = 3;
            instructionSizes["CALLNC IMMEDIATE"] = 3;

            instructionSizes["CP IMMEDIATE"] = 2;
            instructionSizes["CP B"] = 1;
            instructionSizes["CP C"] = 1;
            instructionSizes["CP D"] = 1;
            instructionSizes["CP H"] = 1;
            instructionSizes["CP HL_INDEXED"] = 1;
            instructionSizes["CP IX_INDEXED_PLUS_OFFSET"] = 3;
            instructionSizes["CP IY_INDEXED_PLUS_OFFSET"] = 3;

            instructionSizes["CLS NONE"] = 2;
            instructionSizes["CPL NONE"] = 1;

            instructionSizes["DI NONE"] = 1;

            instructionSizes["DEC A"] = 1;
            instructionSizes["DEC B"] = 1;
            instructionSizes["DEC C"] = 1;
            instructionSizes["DEC D"] = 1;
            instructionSizes["DEC IX"] = 2;
            instructionSizes["DEC HL"] = 1;
            instructionSizes["DEC IY"] = 2;

            instructionSizes["DJNZ IMMEDIATE"] = 2;

            instructionSizes["EI NONE"] = 1;

            instructionSizes["INC A"] = 1;
            instructionSizes["INC B"] = 1;
            instructionSizes["INC D"] = 1;
            instructionSizes["INC BC"] = 1;
            instructionSizes["INC DE"] = 1;
            instructionSizes["INC HL"] = 1;
            instructionSizes["INC SP"] = 1;
            instructionSizes["INC HL_INDEXED"] = 1;
            instructionSizes["INC IX"] = 2;
            instructionSizes["INC IY"] = 2;

            instructionSizes["JP IMMEDIATE"] = 3;
            instructionSizes["JP HL_INDEXED"] = 3;

            instructionSizes["JP IMMEDIATE"] =  3;
            instructionSizes["JP HL_INDEXED"] = 1;
            
            instructionSizes["JPZ IMMEDIATE"] = 3;
            instructionSizes["JPNZ IMMEDIATE"] = 3;        
            instructionSizes["JPC IMMEDIATE"] = 3;
            instructionSizes["JPNC IMMEDIATE"] = 3;
            instructionSizes["JPP IMMEDIATE"] = 3;
            instructionSizes["JPM IMMEDIATE"] = 3;

            instructionSizes["JR IMMEDIATE"] = 2;
            instructionSizes["JRC IMMEDIATE"] = 2;
            instructionSizes["JRNC IMMEDIATE"] = 2;
            instructionSizes["JRNZ IMMEDIATE"] = 2;
            instructionSizes["JRZ IMMEDIATE"] = 2;

            instructionSizes["LD A,B"] = 1;
            instructionSizes["LD A,C"] = 1;
            instructionSizes["LD A,D"] = 1;
            instructionSizes["LD A,E"] = 1;
            instructionSizes["LD A,H"] = 1;
            instructionSizes["LD A,L"] = 1;
            instructionSizes["LD A,IMMEDIATE"] = 2;
            instructionSizes["LD A,MEMORY"] = 3;
            instructionSizes["LD A,HL_INDEXED"] = 1;
            instructionSizes["LD A,IX_INDEXED_PLUS_OFFSET"] = 3;
            instructionSizes["LD A,IY_INDEXED_PLUS_OFFSET"] = 3;
            instructionSizes["LD MEMORY,A"] = 3;
            instructionSizes["LD SP,IMMEDIATE"] = 3;

            instructionSizes["LD B,A"] = 1;
            instructionSizes["LD B,C"] = 1;
            instructionSizes["LD B,D"] = 1;
            instructionSizes["LD B,E"] = 1;
            instructionSizes["LD B,IMMEDIATE"] = 2;
            instructionSizes["LD B,HL_INDEXED"] = 1;
            instructionSizes["LD B,IX_INDEXED_PLUS_OFFSET"] = 3;
            instructionSizes["LD B,IY_INDEXED_PLUS_OFFSET"] = 3;
            instructionSizes["LD BC,IMMEDIATE"] = 3;

            instructionSizes["LD C,A"] = 1;
            instructionSizes["LD C,B"] = 1;
            instructionSizes["LD C,D"] = 1;
            instructionSizes["LD C,IMMEDIATE"] = 2;

            instructionSizes["LD D,A"] = 1;
            instructionSizes["LD D,B"] = 1;
            instructionSizes["LD D,IMMEDIATE"] = 2;
            instructionSizes["LD D,HL_INDEXED"] = 1;

            instructionSizes["LD DE,MEMORY"] = 4;
            instructionSizes["LD MEMORY,DE"] = 4;
            instructionSizes["LD DE,IMMEDIATE"] = 3;
        //    instructionSizes["LD DE,HL_INDEXED"] = 1;

           
            instructionSizes["LD E,A"] = 1;
            instructionSizes["LD E,B"] = 1;
            instructionSizes["LD E,C"] = 1;
            instructionSizes["LD E,H"] = 1;
            instructionSizes["LD E,HL_INDEXED"] = 1;
            instructionSizes["LD E,IX_INDEXED_PLUS_OFFSET"] = 3;
            instructionSizes["LD E,IY_INDEXED_PLUS_OFFSET"] = 3;

            instructionSizes["LD H,IMMEDIATE"] = 2;
            instructionSizes["LD H,IX_INDEXED_PLUS_OFFSET"] = 3;
            instructionSizes["LD H,A"] = 1;
            instructionSizes["LD H,C"] = 1;
            instructionSizes["LD H,L"] = 1;
            instructionSizes["LD HL,IMMEDIATE"] = 3;
            instructionSizes["LD HL_INDEXED,A"] = 1;
            instructionSizes["LD HL_INDEXED,IMMEDIATE"] = 2;

            instructionSizes["LD L,IX_INDEXED_PLUS_OFFSET"] = 3;

            instructionSizes["LD IX,IMMEDIATE"] = 4;
            instructionSizes["LD IY,IMMEDIATE"] = 4;

            instructionSizes["LD IX,MEMORY"] = 4;

            instructionSizes["LD L,A"] = 1;
            instructionSizes["LD L,IMMEDIATE"] = 2;

            instructionSizes["LD IX_INDEXED_PLUS_OFFSET,A"] = 3;

            instructionSizes["LD IY_INDEXED_PLUS_OFFSET,A"] = 3;
            instructionSizes["LD IY_INDEXED_PLUS_OFFSET,IMMEDIATE"] = 4;

            instructionSizes["LD IX_INDEXED_PLUS_OFFSET,B"] = 3;
            instructionSizes["LD SP,MEMORY"] = 4;
            instructionSizes["LD SP,IMMEDIATE"] = 3;

            instructionSizes["LDIR NONE"] = 2;


            instructionSizes["NEG NONE"] = 2;

            instructionSizes["NOP NONE"] = 1;

            instructionSizes["QUIT NONE"] = 2;
            instructionSizes["SAVE NONE"] = 2;
            instructionSizes["RESTORE NONE"] = 2;
            instructionSizes["CHOUT NONE"] = 2;
            instructionSizes["NEWLN NONE"] = 2;
            instructionSizes["SETP NONE"] = 2;
            instructionSizes["READKB NONE"] = 2;
            instructionSizes["GETLINE NONE"] = 2;
            instructionSizes["SLINE NONE"] = 2;

            instructionSizes["OR B"] = 1;

            instructionSizes["POP AF"] = 1;
            instructionSizes["POP BC"] = 1;
            instructionSizes["POP DE"] = 1;
            instructionSizes["POP HL"] = 1;
            instructionSizes["POP IX"] = 2;
            instructionSizes["POP IY"] = 2;

            instructionSizes["PUSH AF"] = 1;
            instructionSizes["PUSH BC"] = 1;
            instructionSizes["PUSH DE"] = 1;
            instructionSizes["PUSH HL"] = 1;
            instructionSizes["PUSH IX"] = 2;
            instructionSizes["PUSH IY"] = 2;


            instructionSizes["RES IMMEDIATE,IX_INDEXED_PLUS_OFFSET"] = 4;

            instructionSizes["RET NONE"] = 1;

            instructionSizes["SET IMMEDIATE,IX_INDEXED_PLUS_OFFSET"] = 4;
             
            instructionSizes["SLA C"] = 2;

            instructionSizes["SRA A"] = 2;

            instructionSizes["SRL A"] = 2;

            instructionSizes["SUB IMMEDIATE"] = 2;
            instructionSizes["SUB B"] = 1;
            instructionSizes["SUB C"] = 1;

        }

        static Dictionary<string, int> instructionSizes = new Dictionary<string, int>();

    }
}
