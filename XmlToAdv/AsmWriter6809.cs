using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
 
namespace XMLtoAdv
{
    public class AsmWriter6809 : AsmWriter
    {
        
        public override void WriteRoutine(StreamWriter sw, string name, string code)
        {
            labelId = 65;
            sw.WriteLine(""); 
            sw.WriteLine("; machine generate routine from XML file");
            sw.WriteLine(name);
            sw.WriteLine("\tpshs d,x,y");
            code = code.Replace("\r","");
            code = code.Replace("\n", "");
            code = code.Replace("\t", "");
            WriteCode(code, sw);
            sw.WriteLine("\tpuls y,x,d");
            sw.WriteLine("\trts");
            sw.WriteLine("");
        }

        //this function write the code to put a variables
        //attr into register a
        public void WritePutAttrInA(StreamWriter sw, string expr)
        {
            int dot = expr.IndexOf(".");
            string objName = expr.Substring(0, dot);
            string attrName = expr.Substring(dot + 1, expr.Length - (dot + 1));
            int attrNum = attrIndexes[attrName.Trim().ToLower()];
            sw.WriteLine("nop ; setting up rhs attribute");
            sw.WriteLine("\tlda " + ToRegisterLoadOperand(objName) + " ; " + objName);
            sw.WriteLine("\tldb #OBJ_ENTRY_SIZE");
            sw.WriteLine("\tmul");
            sw.WriteLine("\ttfr d,x");
            sw.WriteLine("\tleax obj_table,x");
            sw.WriteLine("\tleax " + attrNum + ",x  ; " + attrName);
            sw.WriteLine("\tlda ,x");
        }

        protected override void WriteJump(StreamWriter sw, string label)
        {
            sw.WriteLine("\tbra " + label + " ; skip else ");
        }

        protected override void WriteAttrTest(StreamWriter sw, string code, string objName, string attrName, int attrNum, string op, string val, string label)
        {
            sw.WriteLine("\tnop ; test (" + code + ")");
            sw.WriteLine("\tlda " + ToRegisterLoadOperand(val) + " ;" + val);
            sw.WriteLine("\tpshs a    ; push right side");

            sw.WriteLine("\tlda " + ToRegisterLoadOperand(objName) + " ; " + objName);
            sw.WriteLine("\tldb #OBJ_ENTRY_SIZE");
            sw.WriteLine("\tmul");
            sw.WriteLine("\ttfr d,x");
            sw.WriteLine("\tleax obj_table,x");
            sw.WriteLine("\tleax " + attrNum + ",x  ; " + attrName);
            sw.WriteLine("\tlda ,x");
            sw.WriteLine("\tcmpa ,s ; compare to right side");
            sw.WriteLine("\tpshu cc ; save flags");
            sw.WriteLine("\tleas 1,s ; pop right side");
            sw.WriteLine("\tpulu cc ; restore flags");
            sw.WriteLine("\t" + GetOpCode(op) + " " + label);

        }


        protected override void WritePropTest(StreamWriter sw, string code, string objName, string propName, string op, int val, string label)
        {

            sw.WriteLine("\tnop ; test (" + code + ")");
            sw.WriteLine("\tlda #" + val);
            sw.WriteLine("\tpshs a    ; push right side");
            sw.WriteLine("\tlda " + ToRegisterLoadOperand(objName));
            sw.WriteLine("\tldb #OBJ_ENTRY_SIZE");
            sw.WriteLine("\tmul");
            sw.WriteLine("\ttfr d,x");
            sw.WriteLine("\tleax obj_table,x");
            string propByte = propBytes[propName];
            sw.WriteLine("\tleax " + propByte + ",x  ; ");
            sw.WriteLine("\tlda ,x  ; get property byte");
            sw.WriteLine("\tanda #" + propBits[propName]  +  " ; isolate " + propName + "  bit ");
            //need property byte
            int bit = Convert.ToInt32(propBits[propName]);
            int pos = (int)Math.Log(bit,2);

            //right justify value so it is a 1 or 0
            for (int i = 0; i < pos; i++ )
            {
//                sw.WriteLine("\tasra ; right justify bit" );
                  sw.WriteLine("\tlsra ; right justify bit");
            }
            sw.WriteLine("\tcmpa ,s ; compare to right side");
            sw.WriteLine("\tpshu cc ; save flags");
            sw.WriteLine("\tleas 1,s ; pop right side");
            sw.WriteLine("\tpulu cc ; restore flags");
            sw.WriteLine("\t" + GetOpCode(op) + " " + label);
        }

        protected override void WriteVarTest(StreamWriter sw, string code, string varName, string op, string val, string label)
        {
            sw.WriteLine("\tnop ; test (" + code + ")");
            sw.WriteLine("\tlda " + ToRegisterLoadOperand(varName) + " ; " + varName);
            sw.WriteLine("\tpshs a    ; push right left");
            sw.WriteLine("\tlda " + ToRegisterLoadOperand(val) + " ; " + val);
             
            sw.WriteLine("\tcmpa ,s ; compare to right side");
            sw.WriteLine("\tpshu cc ; save flags");
            sw.WriteLine("\tleas 1,s ; pop right side");
            sw.WriteLine("\tpulu cc ; restore flags");
            sw.WriteLine("\t" + GetOpCode(op) + " " + label);
        }

        
        //example objProp = player.holder
        //example objProp = $dobj
        protected override void WriteAttrAssignment( StreamWriter sw, string lhs, string rhs)
        {
            //push the value to stack
            PushRHS(rhs, sw);
            
            //...then compute the address to store it in.
            PutLHSAddrInX(lhs, sw);

            //...then store it
            sw.WriteLine("\tpuls a ; restore rhs");
            sw.WriteLine("\tsta ,x");
        }

        //obj must be a var or an attribute, or a litteral
        void PutLHSAddrInX(string obj, StreamWriter sw)
        {
            if (project.IsVar(obj))
            {
                sw.WriteLine("\tldx #" + project.GetVarAddr(obj) + " ; " + obj);
            }
            else if (obj.IndexOf(".") != -1)
            {//a property
                int dot = obj.IndexOf(".");
                string objName = obj.Substring(0, dot);
                string attrName = obj.Substring(dot + 1, obj.Length - (dot + 1));
                int attrNum = attrIndexes[attrName.Trim().ToLower()];
                int objId = -1;
                if (project.IsVar(objName))
                {
                    sw.WriteLine("\tlda " + project.GetVarAddr(objName) + " ; " + obj);
                } else {
                    objId = project.GetObjectId(objName);
                    sw.WriteLine("\tlda #" + objId + " ; " + obj);
                }
                
                sw.WriteLine("\tldb #OBJ_ENTRY_SIZE");
                sw.WriteLine("\tmul");
                sw.WriteLine("\ttfr d,x");
                sw.WriteLine("\tleax obj_table,x");
                sw.WriteLine("\tleax " + attrNum + ",x   ;" + attrName);
            }
            else
            {//an object
                int objId = project.GetObjectId(obj);

                sw.WriteLine("\tlda #" + objId + " ; " + obj);
                sw.WriteLine("\tldb #OBJ_ENTRY_SIZE");
                sw.WriteLine("\tmul");
                sw.WriteLine("\ttfr d,x");
                sw.WriteLine("\tleax obj_table,x");
                 
            }
        }

        void PushRHS(string rhs, StreamWriter sw)
        {
            //top code should put value in a
            int val;
            if (Int32.TryParse(rhs, out val))
            {//int literal
                sw.WriteLine("\tlda #" + val + " ; " + rhs);
            }
            else if (project.IsVar(rhs))
            {
                sw.WriteLine("\tlda " + project.GetVarAddr(rhs) + " ; " + rhs);
            }
            else if (rhs.IndexOf("\"") == 0)
            {//a string
                string rest = rhs.Substring(1);
                string inside = rest.Substring(0, rest.IndexOf("\""));
                int strId = project.GetStringId(inside);
                sw.WriteLine("\tlda #" + strId + " ; " + rhs);
            }
            else if (rhs.IndexOf(".") != -1)
            {//an attribute?
                WritePutAttrInA(sw, rhs);
            }
            else if (rhs[0].Equals("\""))
            {//get string id?
                sw.WriteLine("\tlda " + project.GetVarAddr(rhs) + " ; " + rhs);
            }
            else
            {//an object?
                sw.WriteLine("\tlda #" + project.GetObjectId(rhs) + " ; " + rhs);
            }

            sw.WriteLine("\tpshs a ; save value to put in attr");
        }

        protected override void WritePropAssignment(StreamWriter sw, int objId, string obj, string propName, int val)
        {
            if (!propBytes.ContainsKey(propName))
            {
                throw new Exception("Invalid property:" + propName);
            }
            string propByte = propBytes[propName];
            sw.WriteLine("\tnop ; set " + obj + "." + propName + "=" + val);
            sw.WriteLine("\tlda #" + objId + " ; " + obj);
            sw.WriteLine("\tldb #OBJ_ENTRY_SIZE");
            sw.WriteLine("\tmul");
            sw.WriteLine("\ttfr d,x");
            sw.WriteLine("\tleax obj_table,x");
            sw.WriteLine("\tleax " + propByte + ",x");
            sw.WriteLine("\tlda ,x  ; get property byte");
            // clear the bit
            sw.WriteLine("\tldb #" + propBits[propName] + " ; get the mask for " + propName);
            sw.WriteLine("\tcomb " + propBits[propName] + " ; invert it");
            sw.WriteLine("\tpshs b");
            sw.WriteLine("\tanda ,s   ; clear the bit");
            sw.WriteLine("\tleas 1,s ; pop stack");
            if (val == 1)
            {
                sw.WriteLine("\tora #" + propBits[propName] +  "   ; set the " + propName + " bit");
            }
            sw.WriteLine("\tsta ,x  ; store it");
        }

        private string GetOpCode(string op)
        {
            if (op == "==")
            {
                return "lbne";
            }
            else if (op == "!=")
            {
                return "lbeq";
            }
            else 
            {
                throw new Exception("bad relational operator:" + op);
            }
        }

        protected override void WritePrintStatement(StreamWriter sw, string statement)
        {
            int start = statement.IndexOf("\"");
            string rem = statement.Substring(start + 1);
            int end = rem.IndexOf("\"");
            string text = rem.Substring(0, end);


            sw.WriteLine("\tldx #description_table");
            sw.WriteLine("\tlda #" + project.GetStringId(text) + " ; " + text);
            sw.WriteLine("\tpshu a");
            sw.WriteLine("\tjsr print_table_entry");
        }

        protected override void WritePrintNewline(StreamWriter sw)
        {
            sw.WriteLine("\tjsr PRINTCR");
        }


  
        private int EvalPrint(string expr)
        {
            expr = expr.Trim();

            if (expr[0] == '\"')
            {
                string rest=expr.Substring(1);
                string inside = rest.Substring(0, rest.IndexOf("\""));
                return project.GetStringId(inside);
            }
            else 
            {
                return project.GetObjectId(expr);
            }
        }


        protected override void WriteLookStatement(StreamWriter sw)
        {
            sw.WriteLine("\tjsr look_sub");
        }

        protected override void WriteMoveStatement(StreamWriter sw)
        {
            sw.WriteLine("\tjsr move_player");
        }

        protected override void WriteSubroutineCall(StreamWriter sw, string name)
        {
            sw.WriteLine("\tjsr " + name + "_sub");
        }

        /*could be "SOME STRING"
         * could be object name
         * could be integer constant
         * could be $dobj or $obj
         */
        private string ToRegisterLoadOperand(string tok)
        {
            int result;
            if (Int32.TryParse(tok, out result))
            {
                return "#" + result.ToString();
            }
            else if (project.IsVar(tok) || tok.ToUpper().Equals("DOBJ") || tok.ToUpper().Equals("IOBJ"))
            {
                return project.GetVarAddr(tok);
            }
            else if (tok.IndexOf("\"") == 0)
            {
                string left = tok.Substring(1);
                string str = left.Substring(0, left.IndexOf("\"") - 1);
                return "#" + project.GetStringId(str).ToString();
            }
            else if (project.GetObjectId(tok) != -1)
            {
                return "#"+project.GetObjectId(tok).ToString();
            }
            else throw new Exception("Unable to evaluate: " + tok);

        }

        protected override void WriteSetVar(StreamWriter sw, string code)
        {
            string varName,val;
            GetVarAndVal(code, out varName, out val);
            
            //write the code to set the var
            sw.WriteLine("\tpshs a");
            sw.WriteLine("\tlda #" + val + " ; load new val");
            sw.WriteLine("\tsta " +  varName +  " ; store it back");
            sw.WriteLine("\tpuls a");
        }

        protected override void WriteAddVar(StreamWriter sw,string code)
        {
            string varName, val;
            GetVarAndVal(code, out varName, out val);

            //write the code to set the var
            sw.WriteLine("\tpshs a");
            sw.WriteLine("\tlda " + varName);
            sw.WriteLine("\tpshu a ; push var value");
            sw.WriteLine("\tlda #" + val.Trim() + " ; push val to add");
            sw.WriteLine("\tadda ,u ; add it ");
            sw.WriteLine("\tsta " + varName + " ; store it back");
            sw.WriteLine("\tpulu a ; remove temp");
            sw.WriteLine("\tpuls a");
        }


        public override void WriteCall(StreamWriter sw, string label)
        {
            sw.WriteLine("\tjsr " + label);
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

            return "@" + s.ToString().ToLower();
        }

        public override void WriteRMod(StreamWriter sw, string code)
        {
        }
     }
}
