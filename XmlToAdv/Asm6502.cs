using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLtoAdv
{
    public class Asm6502 : AsmWriter
    {

        public override void WriteRoutine(StreamWriter sw, string name, string code)
        {

            name = TruncateName(name);
            labelId = 65;
            sw.WriteLine("");
            sw.WriteLine("; machine generate routine from XML file");
            sw.WriteLine("\t.module " +name);
            sw.WriteLine(name);
            sw.WriteLine("\tpha");
            sw.WriteLine("\ttxa");
            sw.WriteLine("\tpha");
            sw.WriteLine("\ttya");
            sw.WriteLine("\tpha");
            code = code.Replace("\r", "");
            code = code.Replace("\n", "");
            code = code.Replace("\t", "");
           
            try
            {
                WriteCode(code, sw);
            }
            catch (Exception ex)
            {
                throw new Exception("Error writing function : " + name, ex);
            }

            sw.WriteLine("\tpla");
            sw.WriteLine("\ttax");
            sw.WriteLine("\tpla");
            sw.WriteLine("\ttya");
            sw.WriteLine("\tpla");
            sw.WriteLine("\trts");
            sw.WriteLine("");
        }

        protected override void WritePrintStatement(StreamWriter sw, string statement)
        {
            int start = statement.IndexOf("\"");
            string rem = statement.Substring(start + 1);
            int end = rem.IndexOf("\"");
            string text = rem.Substring(0, end);

            sw.WriteLine("\tpha ; print " + text);
            sw.WriteLine("\tlda #$string_table%256");
            sw.WriteLine("\tsta $tableAddr");
            sw.WriteLine("\tlda #$string_table/256");
            sw.WriteLine("\tsta $tableAddr+1");
            sw.WriteLine("\tlda #" + project.GetStringId(text) + " ; " + text);
            sw.WriteLine("\tjsr printix");
            sw.WriteLine("\tpla ; end print");
        }

        protected override string GetNextLabel()
        {
            char c = Convert.ToChar(labelId++);
            return "_" + c.ToString().ToLower();
        }

        protected override void WriteJump(StreamWriter sw, string label)
        {
            sw.WriteLine("\tjmp " + label);
        }

        public override void WriteCall(StreamWriter sw, string label)
        {
            sw.WriteLine("\tjsr " + label);
        }

        protected override void WriteSubroutineCall(StreamWriter sw, string label)
        {
            string sub = TruncateName(label + "_sub");
            sw.WriteLine("\tjsr " + sub);
        }

        protected override void WriteLookStatement(StreamWriter sw)
        {
            sw.WriteLine("\tjsr look_sub");
        }

        protected override void WritePrintNewline(StreamWriter sw)
        {
            sw.WriteLine("\tjsr printcr");
        }

        protected override void WritePropTest(StreamWriter sw, string code, string objName, string propName, string op, int val, string label)
        {
            GetRHS(sw, objName + "." + propName);
            sw.WriteLine("\tnop ; test (" + code + ")");
            sw.WriteLine("\tcmp #" + val);
         //   GetRHS(sw, val); // puts rhs in a
            Write6502Jump(sw, op, label);            
        }

        protected override void WriteVarTest(StreamWriter sw, string code, string varName, string op, string val, string label)
        {
            sw.WriteLine("\tnop ; test (" + code + ")");
            GetRHS(sw, val); // puts rhs in a
            sw.WriteLine("\tcmp " + project.GetVarAddr(varName));
            Write6502Jump(sw, op, label);            
        }

        //val is the rhs
        protected override void WriteAttrTest(StreamWriter sw, string code, string objName, string attrName, int attrNum, string op, string val, string label)
        {
            sw.WriteLine("\tnop ; test (" + code + ")");
 
            GetRHS(sw, val); // puts rhs in a
            sw.WriteLine("\tsta temp ; save it");

 //           sw.WriteLine("\tlda #" + project.GetObjectId(objName));
 //          sw.WriteLine("\tldx #" + attrIndexes[attrName] + " ; " + attrName);
 //           sw.WriteLine("\tjsr get_obj_attr ; result in 'a'");
            GetRHS(sw, objName + "." + attrName); // puts rhs in a
            
            sw.WriteLine("\tcmp temp");
       
            Write6502Jump(sw, op, label);
        }

        protected override void WriteAttrAssignment(StreamWriter sw, string lhs, string rhs)
        {
            GetRHS(sw, rhs); //get value and leave on stack

            if (lhs.IndexOf(".") != -1)
            {//attr or prop
                string obj;
                string attr;
                SplitOnDot(lhs, out obj, out attr);

                if (project.GetObjectId(obj) == -1 && !project.IsVariable(obj))
                {
                    throw new Exception("Invalid object:" + obj);
                }

               // sw.WriteLine("\tpla ; get rhs");
               

                if (IsAttribute(attr))
                {
                    sw.WriteLine("\ttax ; move previous result to x");
                    sw.WriteLine("\tlda #" + project.GetObjectId(obj) + " ; " + obj);
                    sw.WriteLine("\tldy #" + attrIndexes[attr] + " ; " + attr);
                    sw.WriteLine("\tjsr set_obj_attr");
                }
                else if (IsProperty(attr))
                {
                    sw.WriteLine("\ttay ; move previous result to y");
                    sw.WriteLine("\tlda #" + project.GetObjectId(obj) + " ; " + obj);
                    int bit = (int)Math.Log(Convert.ToDouble(propBytes[attr]), 2) + 1;
                    sw.WriteLine("\tldx #" + bit + " ; " + attr);

                   // sw.WriteLine("ldx #" + propBytes[attr] + " ; " + attr);
                    sw.WriteLine("jsr set_obj_prop");                     
                }
                else
                {
                    throw new Exception("Invalid property" + attr);
                }
            }
            else if (project.IsVariable(lhs))
            {
                GetRHS(sw, rhs);
                //sw.WriteLine("\tpla");
                sw.WriteLine("\tsta " + project.GetVarAddr(lhs));
            }
            else
            {
                throw new Exception("lhs is not an object or variable: " + lhs);
            }

        }

        protected override void WritePropAssignment(StreamWriter sw, int objId, string obj, string propName, int val)
        {
            if (!propBytes.ContainsKey(propName))
            {
                throw new Exception("Invalid property:" + propName);
            }

            sw.WriteLine("\tlda #" + objId + " ; " + obj);

            int bit =  (int)Math.Log(Convert.ToDouble(propBits[propName]), 2) + 1;
            if (propBytes[propName].Equals("PROPERTY_BYTE_2"))
            {
                bit += 8;
            }

            sw.WriteLine("\tldx #" + bit + " ; " + propName + " bit");
            //sw.WriteLine("\tldx #" + propBits[propName] + " ; " + propName);

            sw.WriteLine("\tldy #" + val + " ; new value");

            sw.WriteLine("\tjsr set_obj_prop");
            
        }

        protected override void WriteSetVar(StreamWriter sw, string code)
        {
            string varName, val;
            GetVarAndVal(code, out varName, out val);

            //write the code to set the var
            sw.WriteLine("\tpha");
            sw.WriteLine("\tlda #" + val + " ; load new val");
            sw.WriteLine("\tsta " + varName + " ; store it back");
            sw.WriteLine("\tpla");
        }

        protected override void WriteAddVar(StreamWriter sw, string code)
        {
            string varName, val;
            GetVarAndVal(code, out varName, out val);

            //write the code to set the var
            sw.WriteLine("\tpha");
            sw.WriteLine("\tlda " + varName);
            sw.WriteLine("\tclc");
            sw.WriteLine("\tadc #" + val.Trim());
            sw.WriteLine("\tsta " + varName + " ; store it back");
            sw.WriteLine("\tpla");

        }

        protected override void WriteMoveStatement(StreamWriter sw)
        {
            sw.WriteLine("\tjsr move_player");
        }

        public static string TruncateName(string name)
        {
           if (name.Length < 20)
           {
               return name;
           }
            return name.Substring(0,20);
        }

        private void WritePushRegs()
        {
        }

        private void WritePullRegs()
        {
        }

        //figures out what the rhs is an pushes it onto stack
        void PushValue(StreamWriter sw, string code)
        {
            GetRHS(sw, code);
        }

        //figures out what the rhs is an pushes it onto stack
        void GetRHS(StreamWriter sw, string rhs)
        {
            int result;
            if (Int32.TryParse(rhs, out result))
            {
                sw.WriteLine("\tlda #" + result.ToString());
            }
            else if (project.IsVariable(rhs) || rhs.ToUpper().Equals("DOBJ") || rhs.ToUpper().Equals("IOBJ"))
            {
                sw.WriteLine("\tlda $" + project.GetVarAddr(rhs) + "; " + rhs);

            }
            else if (rhs.IndexOf("\"") == 0)
            {
                string left = rhs.Substring(1);
                string str = left.Substring(0, left.IndexOf("\""));
                int objId = project.GetStringId(str);
                if (objId == -1)
                {
                    throw new Exception("Unknown object : " + str + ".  Check spelling an be sure to use the object's full name.");
                }

                sw.WriteLine("\tlda #" + objId + " ;" + rhs);
            }
            else if (rhs.IndexOf(".")!=-1)
            {
                string obj, prop;
                SplitOnDot(rhs, out obj, out prop);

                sw.WriteLine("\tlda #" + project.GetObjectId(obj) + " ; " + obj);

                if (IsAttribute(prop))
                {
                    //write push attr

                    sw.WriteLine("\tldy #" + attrIndexes[prop] + " ; " + prop);
                    sw.WriteLine("\tjsr get_obj_attr");                  
                } 
                else if (IsProperty(prop))
                {
                    int bit = (int)Math.Log(Convert.ToDouble(propBits[prop]), 2)+1;
                    if (propBytes[prop].Equals("PROPERTY_BYTE_2"))
                    {
                        bit += 8;
                    }
                    sw.WriteLine("\tldx #" + bit + " ; " + prop);
                    sw.WriteLine("\tjsr get_obj_prop ; " + obj);
                }
                else
                {
                    throw new Exception("invalid property or attribute: " + prop);
                }

            }
            else if (project.GetObjectId(rhs) != -1)
            {
                try
                {
                    sw.WriteLine("\t lda #" + project.GetObjectId(rhs).ToString() + " ;" + rhs);
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else throw new Exception("Unable to evaluate: " + rhs);

            //sw.WriteLine("\tpha ");  //push the result
          
        }

        string OperatorToCC(string op)
        {
            if (op.Equals("=="))
                return "beq";
            if (op.Equals("!="))
                return "bne";
            else throw new Exception ("Unsupported operator :  " + op);
            
        }

        void Write6502Jump(StreamWriter sw, string op, string label)
        {
            string lbl = GetNextLabel();
            sw.WriteLine("\t" + OperatorToCC(op) + " " + lbl + " ; skip over jump");  //THIS DOESN't WORK
            sw.WriteLine("\tjmp " + label + " ; finally do the actual jump");
            sw.WriteLine(lbl + " \tnop ; stupid thing because 6502 has no lbeq instruction");
        }
        public override void WriteRMod(StreamWriter sw, string code)
        {
        }
    }
}
