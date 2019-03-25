using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace XMLtoAdv
{
    public class SyntaxChecker : AsmWriter
    {

        public override void WriteRoutine(StreamWriter sw, string name, string code)
        {
            labelId = 65; //'A' 
 
            code = code.Replace("\r", "");
            code = code.Replace("\n", "");
            code = code.Replace("\t", "");
            WriteCode(code, sw);
        }

        protected override void WriteAttrTest(StreamWriter sw, string code, string objName, string attrName, int attrNum, string op, string val, string label)
        {
             
            GetRHS(sw, val); // puts rhs in a
            WriteObjectAddrToIX(sw, objName, attrName); //preserves af,bc
                 
        }

        //example objProp = player.holder
        //example objProp = $dobj
        protected override void WriteAttrAssignment(StreamWriter sw, string lhs, string rhs)
        {
            //compute rhs and leave it in 'a' 
            GetRHS(sw, rhs);

            //...then compute the address to store it in and put the addr in 'ix'
            PutLHSAddrInIX(sw, lhs);

             
        }

        protected override void WritePrintNewline(StreamWriter sw)
        {
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
            else if (project.IsVariable(lhs))
            {//var?
                GetVarAddr(lhs);
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

            project.GetStringId(text);
            
        }

        protected override void WriteSubroutineCall(StreamWriter sw, string statement)
        {
        }

        protected override void WriteMoveStatement(StreamWriter sw)
        {
        }


        protected override void WritePropAssignment(StreamWriter sw, int objId, string obj, string propName, int val)
        {
          // 

            if (val==0)
            {//clear bit ('or' x with mask)
                WriteObjectAddrToIX(sw, obj, propName);
                GetBitPos(propName);
            }
            else
            {//set bit (or mask with value)
                WriteObjectAddrToIX(sw, obj, propName);
                GetBitPos(propName);
            }

        }


        protected override void WriteVarTest(StreamWriter sw, string code, string varName, string op, string val, string label)
        {
            GetRHS(sw, val);
            GetRHS(sw, varName);
            OperatorToCC(op);

        }

        protected override void WritePropTest(StreamWriter sw, string code, string objName, string propName, string op, int val, string label)
        {

            WriteObjectAddrToIX(sw, objName, propName);
            GetBitPos(propName);

            if (op.Equals("=="))
            {
            }
            else if (op.Equals("!="))
            {
            }
            else throw new Exception("Attributes can only be test with == and !=");

        }

        protected override void WriteLookStatement(StreamWriter sw)
        {
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

            }
            else if (project.IsVariable(rhs) || rhs.ToUpper().Equals("DOBJ") || rhs.ToUpper().Equals("IOBJ"))
            {
                project.GetVarAddr(rhs);
            }
            else if (rhs.IndexOf("\"") == 0)
            {
                string left = rhs.Substring(1);
                string str = left.Substring(0, left.IndexOf("\""));
                project.GetStringId(str);
            }
            else if (rhs.IndexOf(".")!=-1)
            {
                
                string obj, prop;
                SplitOnDot(rhs, out obj, out prop);
                WriteObjectAddrToIX(sw, obj, prop);
                
                if (IsAttribute(prop))
                {
                   
                }
                else
                {
                    
                   WriteFlagsToA(sw);
                    
                  
                }
            }
            else if (project.GetObjectId(rhs) != -1)
            {
                try
                {
                    project.GetObjectId(rhs).ToString();
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
            if (project.IsVariable(obj))
                project.GetVarAddr(obj);
            else
                project.GetObjectId(obj);

            if (attrIndexes.Keys.Contains(attr)) 
            {    
            }
            else if (propBytes.Keys.Contains(attr))
            {
                 
            }
            else
                throw new Exception("Bad object property:" + attr);

        }

        protected override void WriteSetVar(StreamWriter sw, string code)
        {
            string varName, val;
            GetVarAndVal(code, out varName, out val);

        }

        protected override void WriteAddVar(StreamWriter sw, string code)
        {
            string varName, val;
            GetVarAndVal(code, out varName, out val);

        }

        protected override string GetNextLabel()
        {
            char c = Convert.ToChar(labelId++);
            return "$" + c.ToString().ToLower() + "?";
        }

        public override void WriteCall(StreamWriter sw, string label)
        {
        }

        protected override void WriteJump(StreamWriter sw, string label)
        {
        }

        protected void WriteFlagsToA(StreamWriter sw)
        {
        }

        public override void WriteRMod(StreamWriter sw, string code)
        {
        }
    }
}
