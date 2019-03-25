using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GameTables;

namespace XMLtoAdv
{
    public class AsmWriterZ80 : AsmWriter
    {

        public override void WriteRoutine(StreamWriter sw, string name, string code)
        {
            labelId = 65; //'A' 
            sw.WriteLine("");
            sw.WriteLine("; machine generate Z80 routine from XML file");
            sw.WriteLine("*MOD");
            sw.WriteLine(name);
            sw.WriteLine("\tpush af");
            sw.WriteLine("\tpush bc");
            sw.WriteLine("\tpush de");
            sw.WriteLine("\tpush hl");
            sw.WriteLine("\tpush ix");
            sw.WriteLine("\tpush iy");
            code = code.Replace("\r", "");
            code = code.Replace("\n", "");
            code = code.Replace("\t", "");
            WriteCode(code, sw);
            sw.WriteLine("\tpop iy");
            sw.WriteLine("\tpop ix");
            sw.WriteLine("\tpop hl");
            sw.WriteLine("\tpop de");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tret");
            sw.WriteLine("");
        }

        protected override void WriteAttrTest(StreamWriter sw, string code, string objName, string attrName, int attrNum, string op, string val, string label)
        {
            sw.WriteLine("\tnop ; test (" + code + ")");
            
             //   sw.WriteLine("\tlda " + ToRegisterLoadOperand(val) + " ;" + val);
            //    sw.WriteLine("\tpshs a    ; push right side");
            GetRHS(sw, val); // puts rhs in a
            sw.WriteLine("\tld b,a  ; move rhs in b");
            WriteObjectAddrToIX(sw, objName, attrName); //preserves af,bc
            sw.WriteLine("\tld a,(ix)");
            sw.WriteLine("\tcp b ; ==" + val + "?");
            sw.WriteLine("\tjp " + OperatorToCC(op) + "," + label);
                
        }

        //example objProp = player.holder
        //example objProp = $dobj
        protected override void WriteAttrAssignment(StreamWriter sw, string lhs, string rhs)
        {
            //compute rhs and leave it in 'a' 
            GetRHS(sw, rhs);

            //...then compute the address to store it in and put the addr in 'ix'
            PutLHSAddrInIX(sw, lhs);

            //...then store it
            sw.WriteLine("\t ld (ix), a ; store rhs in lhs");
            
        }

        protected override void WritePrintNewline(StreamWriter sw)
        {
            sw.WriteLine("\tcall printcr ; newline");
        }


        /// <summary>
        /// Loads the address of the object+prop+attr into ix
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="lhs"></param>
        protected void PutLHSAddrInIX(StreamWriter sw, string lhs)
        {
            //is lhs a variable, or attr?
            if (lhs.IndexOf(".") != -1)
            {//attr or prop
                string obj; 
                string attr;
                SplitOnDot(lhs, out obj, out attr);
                
                if (project.GetObjectId(obj) == -1 && !project.IsVariable(obj))
                {
                    throw new Exception("Invalid object:" + obj );
                }

                if (IsAttribute(attr))
                {
                    WriteObjectAddrToIX(sw, obj,attr);
                }
                else if (IsProperty(attr))
                {
                    throw new Exception("Setting properties TBD");
                }
                else 
                {
                    throw new Exception("Invalid property" + attr);
                }
            }
            else if (project.IsVar(lhs))
            {//var?
                sw.WriteLine("\tld ix, " + GetVarAddr(lhs) + "; " +  lhs);
            }
            else 
            {
                throw new Exception("invalid lvalue near " + lhs);
            }


        }

        protected override void WritePrintStatement(StreamWriter sw, string statement)
        {
            int start = statement.IndexOf("\"");
            string rem = statement.Substring(start + 1);
            int end = rem.IndexOf("\"");
            string text = rem.Substring(0, end);

            sw.WriteLine("\tpush af");
            sw.WriteLine("\tpush ix");
            sw.WriteLine("\tld ix,string_table");
            sw.WriteLine("\tld b," + project.GetStringId(text) + " ; " + text);
            sw.WriteLine("\tcall print_table_entry");
            sw.WriteLine("\tpop ix");
            sw.WriteLine("\tpop af");

        }

        protected override void WriteSubroutineCall(StreamWriter sw, string statement)
        {
            sw.WriteLine("\tcall " + statement + "_sub");
        }

        protected override void WriteMoveStatement(StreamWriter sw)
        {
            sw.WriteLine("\tcall move_player");
        }


        protected override void WritePropAssignment(StreamWriter sw, int objId, string obj, string propName, int val)
        {
          // 

            if (val==0)
            {//clear bit ('or' x with mask)
                WriteObjectAddrToIX(sw, obj, propName);
                sw.WriteLine("\tres " + GetBitPos(propName) + ",(ix) ; clr " + propName + " bit");
                //sw.WriteLine("\tld b,(ix) ; get property byte");
                //sw.WriteLine("\tld a," +  propBits[propName]+ " ; get " + propName + " bit");
                //sw.WriteLine("\tcpl ; flip bits");
               // sw.WriteLine("\tand b ; clear the bit" );
                //sw.WriteLine("\tld (ix),a ; write bits back ");
            }
            else
            {//set bit (or mask with value)
                WriteObjectAddrToIX(sw, obj, propName);
                sw.WriteLine("\tset " + GetBitPos(propName) + ",(ix) ; set the " + propName + " bit");
            }

        }


        protected override void WriteVarTest(StreamWriter sw, string code, string varName, string op, string val, string label)
        {
          //  throw new Exception("WriteVarTest NotImpl");
            GetRHS(sw, val);
            sw.WriteLine("\tld b,a ; put rhs in b");
            GetRHS(sw, varName);
            sw.WriteLine("\tcp b" + " ; " + op + val + "?");
            sw.WriteLine("\tjp " + OperatorToCC(op) + "," + label);

        }

        protected override void WritePropTest(StreamWriter sw, string code, string objName, string propName, string op, int val, string label)
        {

            WriteObjectAddrToIX(sw, objName, propName);
            sw.WriteLine("\tbit " + GetBitPos(propName) + ",(ix) ; test " + propName + " prop bit");

            if (op.Equals("=="))
            {
                if (val == 0)
                    sw.WriteLine("\tjp nz," + label);
                else
                    sw.WriteLine("\tjp z," + label);
            }
            else if (op.Equals("!="))
            {
                if (val == 0)
                    sw.WriteLine("\tjp z," + label);
                else
                    sw.WriteLine("\tjp nz," + label);
            }
            else throw new Exception("Attributes can only be test with == and !=");

        }

        protected override void WriteLookStatement(StreamWriter sw)
        {
            sw.WriteLine("\tcall look_sub");
        }

        //returns the CC flag for an operator (used to build the jump CC statement)
        public string OperatorToCC(string op)
        {
            if (op.Equals("=="))
                return "nz";
            if (op.Equals("!="))
                return "z";
            else return "OPERATOR " + op + " NOT IMPLEMENTED.";
        }

        //write code to put the rhs in register a
        void GetRHS(StreamWriter sw, string rhs)
        {
            int result;
            if (Int32.TryParse(rhs, out result))
            {
                sw.WriteLine("\tld a," + result.ToString());
            }
            else if (project.IsVar(rhs) || rhs.ToUpper().Equals("DOBJ") || rhs.ToUpper().Equals("IOBJ"))
            {
                sw.WriteLine("\tld a,(" + project.GetVarAddr(rhs) + "); " + rhs);
            }
            else if (rhs.IndexOf("\"") == 0)
            {
                string left = rhs.Substring(1);
                string str = left.Substring(0, left.IndexOf("\""));
                sw.WriteLine("\tld a," + project.GetStringId(str).ToString() + " ;" + rhs);
            }
            else if (rhs.IndexOf(".")!=-1)
            {
                
                string obj, prop;
                SplitOnDot(rhs, out obj, out prop);
                WriteObjectAddrToIX(sw, obj, prop);
                if (IsAttribute(prop))
                {
                    sw.WriteLine("\tld a,(ix) ; get attr byte byte");
                }
                else
                {
                    
                    sw.WriteLine("\tld a,(ix) ; get property byte");
                    sw.WriteLine("\tbit " + propBits[prop] + ",a ; test " + prop + " bit");
                    WriteFlagsToA(sw);
                    
                    sw.WriteLine("\tor 40h ; isolate zero bit");
                }
            }
            else if (project.GetObjectId(rhs) != -1)
            {
                try
                {
                    sw.WriteLine("\t ld a," + project.GetObjectId(rhs).ToString() + " ;" + rhs);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else throw new Exception("Unable to evaluate: " + rhs);

        }


        void WriteObjectAddrToIX(StreamWriter sw, string obj, string attr)
        {
            sw.WriteLine("\tpush af");
            sw.WriteLine("\tpush bc");
            if (project.IsVariable(obj))
                sw.WriteLine("\tld a,(" + project.GetVarAddr(obj) + ")" );
            else
                sw.WriteLine("\tld a," + project.GetObjectId(obj) + "; " + obj);
            sw.WriteLine("\tld b,a");
            sw.WriteLine("\tld c, " + GameObject.SIZE);
            sw.WriteLine("\tcall bmulc");
            sw.WriteLine("\tld ix,obj_table");
            sw.WriteLine("\tadd ix,bc ; jump to object");
            if (attrIndexes.Keys.Contains(attr))
                sw.WriteLine("\tld bc," + attrIndexes[attr] + " ; get '" + attr + "' byte");
            else if (propBytes.Keys.Contains(attr))
                sw.WriteLine("\tld bc," + propBytes[attr] + " ; get prop byte");
            else
                throw new Exception("Bad object property:" + attr);
            sw.WriteLine("\tadd ix,bc ; jump to the object's byte we need");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tpop af");
        }

        protected override void WriteSetVar(StreamWriter sw, string code)
        {
            string varName, val;
            GetVarAndVal(code, out varName, out val);

            //write the code to set the var
            sw.WriteLine("\tpush  af");
            sw.WriteLine("\tld a," + val + " ; load new val");
            sw.WriteLine("\tld (" + varName + "),a  ; store it back");
            sw.WriteLine("\tpop af");
        }

        protected override void WriteAddVar(StreamWriter sw, string code)
        {
            string varName, val;
            GetVarAndVal(code, out varName, out val);

            //write the code to set+ the var
            sw.WriteLine("\tpush af");
            sw.WriteLine("\tld a,(" + varName + ") ; get val of " + varName );
            sw.WriteLine("\tadd a," + val + " ; push val to add");
            sw.WriteLine("\tld (" + varName + "),a ; store it back");
            sw.WriteLine("\tpop af");

        }

        protected override string GetNextLabel()
        {

            string s = "";
            int temp = _label; ;

            char c = Convert.ToChar(temp % 26 + 65);
            s += c;
            temp /= 26;

            while (temp > 0)
            {
                c = Convert.ToChar(temp % 26 + 65);
                s += c;
                temp /= 26;
            }

            _label++;

            return "$" + s.ToLower() + "?";
        }

        public override void WriteCall(StreamWriter sw, string label)
        {
            sw.WriteLine("\tcall " + label);
        }

        protected override void WriteJump(StreamWriter sw, string label)
        {
            sw.WriteLine("\tjp " + label + " ; skip else ");
        }

        protected void WriteFlagsToA(StreamWriter sw)
        {
            sw.WriteLine("\tpush bc ; flags to a");
            sw.WriteLine("\tpush af ; tfr flags to acc");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tld a,c ; get flags in acc ");
            sw.WriteLine("\tpop bc ; end flags to a");
        }

        public override void WriteRMod(StreamWriter sw, string code)
        {
        }

    }
}
