using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLtoAdv
{
    public  abstract class AsmWriter
    {

        protected int _label = 0;
        protected int labelId = 97;
        public const int IF_STATEMENT = 0;
        public const int IF_ELSE_STATEMENT = 1;
        public const int ELSE_STATEMENT = 3;
        public const int CODE = 4;

        protected Dictionary<string, Int32> attrIndexes = new Dictionary<string, int>();
        protected Dictionary<string, string> propBytes = new Dictionary<string, string>();  //which byte to look in
        protected Dictionary<string, string> propBits = new Dictionary<string, string>(); //bit masks

        protected List<string> labelStack = new List<string>();

        protected XmlToTables project = null;
        char[] label = new char[5];

        public AsmWriter()
        {
            project = XmlToTables.GetInstance();

            attrIndexes.Add("id", 0);
            attrIndexes.Add("holder", 1);
            attrIndexes.Add("initial_description", 2);
            attrIndexes.Add("initialdescription", 2);
            attrIndexes.Add("description", 3);
            attrIndexes.Add("n", 4);
            attrIndexes.Add("s", 5);
            attrIndexes.Add("e", 6);
            attrIndexes.Add("w", 7);
            attrIndexes.Add("ne", 8);
            attrIndexes.Add("se", 9);
            attrIndexes.Add("sw", 10);
            attrIndexes.Add("nw", 11);
            attrIndexes.Add("u", 12);
            attrIndexes.Add("up", 12);
            attrIndexes.Add("down", 13);
            attrIndexes.Add("d", 13);
            attrIndexes.Add("in", 14);
            attrIndexes.Add("out", 15);
            attrIndexes.Add("mass", 16);

            /* BYTE1
             * SCENERY_MASK equ 1
            SUPPORTER_MASK equ 2
            CONTAINER_MASK equ 4
            TRANSPARENT_MASK equ 8
            OPENABLE_MASK equ 16
            OPEN_MASK equ 32
            LOCKABLE_MASK equ 64
            LOCKED_MASK equ 128
             */

            propBytes.Add("scenery", "PROPERTY_BYTE_1");
            propBytes.Add("supporter", "PROPERTY_BYTE_1");
            propBytes.Add("container", "PROPERTY_BYTE_1");
            propBytes.Add("transparent", "PROPERTY_BYTE_1");
            propBytes.Add("openable", "PROPERTY_BYTE_1");
            propBytes.Add("open", "PROPERTY_BYTE_1");
            propBytes.Add("lockable", "PROPERTY_BYTE_1");
            propBytes.Add("locked", "PROPERTY_BYTE_1");



            /*
            PORTABLE_MASK equ 1
            EDIBLE_MASK equ 2
            DRINKABLE_MASK equ 4
            FLAMMABLE_MASK equ 8
            LIGHTABLE_MASK equ 16
            LIT_MASK equ 32	
            EMITTING_LIGHT_MASK equ 32
            DOOR_MASK equ 64
            UNUSED_MASK equ 128
            */
            propBytes.Add("portable", "PROPERTY_BYTE_2");
            propBytes.Add("backdrop", "PROPERTY_BYTE_2");
            propBytes.Add("wearable", "PROPERTY_BYTE_2");
            propBytes.Add("beingworn", "PROPERTY_BYTE_2");
            propBytes.Add("lightable", "PROPERTY_BYTE_2");
            propBytes.Add("lit", "PROPERTY_BYTE_2");
            propBytes.Add("emittinglight", "PROPERTY_BYTE_2");
            propBytes.Add("door", "PROPERTY_BYTE_2");

            //bit masks

            /*
            SCENERY_MASK equ 1
            SUPPORTER_MASK equ 2
            CONTAINER_MASK equ 4
            TRANSPARENT_MASK equ 8
            OPENABLE_MASK equ 16
            OPEN_MASK equ 32
            LOCKABLE_MASK equ 64
            LOCKED_MASK equ 128
             */

            propBits.Add("scenery", "1");
            propBits.Add("supporter", "2");
            propBits.Add("container", "4");
            propBits.Add("transparent", "8");
            propBits.Add("openable", "16");
            propBits.Add("open", "32");
            propBits.Add("lockable", "64");
            propBits.Add("locked", "128");
            /*
             PORTABLE_MASK equ 1
            EDIBLE_MASK equ 2
            DRINKABLE_MASK equ 4
            FLAMMABLE_MASK equ 8
            LIGHTABLE_MASK equ 16
            LIT_MASK equ 32	
            EMITTING_LIGHT_MASK equ 32
            DOOR_MASK equ 64
            UNUSED_MASK equ 128
             */
            propBits.Add("portable", "1");
            propBits.Add("edible", "2");
            propBits.Add("wearable", "4");
            propBits.Add("beingworn", "8");
            propBits.Add("lightable", "16");
            propBits.Add("lit", "32");
            propBits.Add("emittinglight", "32");
            propBits.Add("door", "64");
            propBits.Add("unused", "128");
        }

        public abstract void WriteRoutine(StreamWriter sw, string name, string code);

        protected bool IsAttribute(string attr)
        {
            return attrIndexes.Keys.Contains(attr);
         }

        protected bool IsProperty(string attr)
        {
            return propBits.Keys.Contains(attr);
        }


        protected abstract void WriteAttrAssignment(StreamWriter sw, string lhs, string rhs);
        protected abstract void WriteSubroutineCall(StreamWriter sw, string args);
        protected abstract void WriteMoveStatement(StreamWriter sw);
        protected abstract void WritePropAssignment(StreamWriter sw, int objId, string obj, string propName, int val);
        protected abstract void WriteLookStatement(StreamWriter sw);
        protected abstract void WritePrintStatement(StreamWriter sw, string text);
        protected abstract void WritePrintNewline(StreamWriter sw);
        protected abstract void WriteAddVar(StreamWriter sw, string code);
        protected abstract void WriteSetVar(StreamWriter sw, string code);
        protected abstract void WriteAttrTest( StreamWriter sw, string code, string objName, string attrName, int attrNum, string op, string val, string label);
        protected abstract void WriteVarTest(StreamWriter sw, string code, string varName, string op, string val, string label);
        protected abstract void WritePropTest(StreamWriter sw, string code, string objName, string propName, string op, int val, string label);
        protected abstract void WriteJump(StreamWriter sw, string label);
        protected abstract string GetNextLabel();
        public abstract void WriteCall(StreamWriter sw, string label);
        public abstract void WriteRMod(StreamWriter sw, string code);

        protected void SplitOnDot(string lhs, out string obj, out string attr)
        {
            int dot = lhs.IndexOf(".");
            obj = lhs.Substring(0, dot);
            attr = lhs.Substring(dot+1);
        }

        public virtual void WriteCode(string code, StreamWriter sw)
        {
            try
            { 

                code = code.Trim();
                if (code.Length > 0 && code != "}")
                {

                    //does it start with an 'if' statement

                    if (code.IndexOf("if") == 0)
                    {
                        string label = GetNextLabel();

                        int start = code.IndexOf("(");
                        int end = code.IndexOf(")");
                        string expr = code.Substring(start, (end - start) + 1);

                        WriteExpr(sw, expr, label);

                        //find the matching }
                        string remainder = code.Substring(code.IndexOf("{"));

                        int closingBrace = findClosingBrace(remainder);
                        string inside = remainder.Substring(1, closingBrace - 1).Trim();
                        string outside = remainder.Substring(closingBrace + 1).Trim();  //the next block or statements

                        WriteCode(inside, sw);

                        //while outside is an else if  or else

                        //will we need a branch
                        if (PeekAheadForElse(outside))
                        {
                            string l = GetNextLabel();
                            labelStack.Add(l);
                            WriteJump(sw, l);

                        }
                        //close bloc
                        if (sw != null)
                            sw.WriteLine(label + "\tnop ; close " + expr);

                        WriteCode(outside, sw);
                    }
                    else if (code.IndexOf("else") == 0)
                    {

                        code = code.Substring(4); //strip off 'else'
                        code = UnWrapCurlyBraces(code);
                        WriteCode(code, sw);

                        string l = labelStack[labelStack.Count - 1];
                        labelStack.RemoveAt(labelStack.Count - 1);
                        
                        if (sw != null)
                            sw.WriteLine(l + "\tnop ; end else");

                    }
                    else
                    {//must just be statements

                        //first part must be a set attr
                        //chop it off, then parse the rest
                        if (code.IndexOf(";") == -1)
                            throw new Exception("Missing ; near " + code);

                        string statement = code.Substring(0, code.IndexOf(";"));

                        if (sw != null)
                            sw.WriteLine("\tnop ; " + statement);

                        //what kind of statement is it?
                        //if there's a += or -=
                        //if there's a '=' and a '.' then it's an object attr or prop assignment
                        //if there's a '=' and no '.' then its a variable assignment
                        //if there's a "print", then it's a print statement

                        if (statement.IndexOf("printl") != -1)
                        {
                            WritePrintStatement(sw, statement);
                            WritePrintNewline(sw);
                        }
                        else if (statement.IndexOf("print") != -1)
                        {
                            WritePrintStatement(sw, statement);
                        }
                        else if (statement.IndexOf("look()") != -1)
                        {
                            WriteLookStatement(sw);
                        }
                        else if (statement.IndexOf("move()") != -1)
                        {
                            WriteMoveStatement(sw);
                        }
                        else if (statement.IndexOf("add(") != -1)
                        {
                            WriteAddVar(sw, statement);
                        }
                        else if (statement.IndexOf("set(") != -1)
                        {
                            WriteSetVar(sw, statement);
                        }
                        else if (statement.IndexOf("call") == 0)
                        {
                            statement = statement.Substring(4).Trim(); //chop off "call"
                            if (XmlToTables.GetInstance().IsSubroutine(statement))
                            {
                                string subName = statement.Substring(0, statement.IndexOf("("));
                                WriteSubroutineCall(sw, subName);
                            }
                            else throw new Exception("Invalid subroutine name near " + statement);
                        }
                        else if (statement.IndexOf(".") != -1)
                        { //attribute or property assignment
                            string right = statement.Substring(statement.IndexOf("=") + 1, statement.Length - statement.IndexOf("=") - 1).Trim();
                            string attr = statement.Substring(statement.IndexOf(".") + 1, statement.IndexOf("=") - statement.IndexOf(".") - 1).Trim();
                            string obj = statement.Substring(0, statement.IndexOf("."));
                            int objId = -1;

                            objId = project.GetObjectId(obj);
                            if (objId == -1 && !IsVar(obj))
                            {
                                throw new Exception("unknown object: " + obj + " near : " + statement);
                            }


                            if (IsAttribute(attr))
                            {//attribute

                                int attrNum = attrIndexes[attr.Trim().ToLower()];

                                //convert right to an id;
                                WriteAttrAssignment(sw, obj + "." + attr, right);
                            }
                            else
                            {//property assignment
                                int bit = ToBit(right);
                                WritePropAssignment(sw, objId, obj, attr, bit);
                            }

                        }//end attr or prop assignment (dot found)
                        else
                        {//var assignment not using set()
                            //                        throw new Exception("Need to Implement var setting");
                            //                      WriteAttrAssignment(statement, obj, right);
                            if (sw != null)
                                sw.WriteLine("\tnop ; this code hasn't been tested.");
                            string right = statement.Substring(statement.IndexOf("=") + 1, statement.Length - statement.IndexOf("=") - 1).Trim();
                            string left = statement.Substring(0, statement.IndexOf("=")).Trim();
                            WriteAttrAssignment(sw, left, right);

                        }

                        string remainder = code.Substring(code.IndexOf(";") + 1).Trim();
                        WriteCode(remainder, sw);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error near code: " + code, ex);
            }
        }

        //the starting { has NOT been chopped off
        protected int findClosingBrace(string code)
        {
            int count = 0;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '{') count++;

                if (code[i] == '}')
                {
                    count--;
                    if (count == 0) { return i; }

                }

            }
            return -1;
        }

        

        protected bool PeekAheadForElse(string code)
        {
            return (code.IndexOf("else") == 0);
        }

        protected string UnWrapCurlyBraces(string code)
        {
            if (code[0] == '{')
            {
                int end = code.LastIndexOf("}");
                int start = code.IndexOf("{");
                string unwrapped = code.Substring(start + 1, end - start - 1);
                return unwrapped;
            }
            else return code;
        }


        protected int GetBlockType(string code)
        {

            if (code.IndexOf("if") == 0)
            {
                //scan ahead for an else

                string right = code.Substring(code.IndexOf("{"));
                int close = findClosingBrace(code);
                string rem = code.Substring(close + 1).Trim();

                if (rem.IndexOf("else") == 0)
                {
                    return IF_ELSE_STATEMENT;
                }
                else
                {
                    return IF_STATEMENT;
                }
            }
            else if (code.IndexOf("else") == 0)
            {
                return ELSE_STATEMENT;
            }
            else
            {
                return CODE;
            }
        }
        protected int ToBit(string val)
        {
            val = val.Trim();
            if (val == "0" || val.ToUpper().Equals("FALSE")) { return 0; }
            return 1;
        }

        protected bool IsVar(string name)
        {
            if (name.Equals("dobj)") || name.Equals("iobj") || name.Equals("prep"))
            {
                return true;
            }
            else
            {
                 return project.IsVariable(name);
            }
        }

        protected string GetVarAddr(string varName)
        {
            return project.GetVarAddr(varName);
        }

        protected void GetVarAndVal(string code, out string varName, out string val)
        {
            string trimmed = code.Substring(code.IndexOf("(")+1);
            trimmed = trimmed.Substring(0, trimmed.Length - 1);
            int commaIx = trimmed.IndexOf(',') ;
            varName = trimmed.Substring(0,commaIx).Trim();
            val = trimmed.Substring(commaIx+1, trimmed.Length - commaIx-1 );
        }


        protected void WriteExpr(StreamWriter sw, string code, string label)
        {
            //extract the ()
            try
            {
                string left, right, op;
                ParseExpr(code, out left, out right, out op);

                //is there a "."  on the left
                if (left.IndexOf(".") != -1)
                {
                    left = code.Substring(1, code.IndexOf(".") - 1);

                    //    string attr = code.Substring(code.IndexOf("."), code.IndexOf(op) - code.IndexOf(op));
                    right = right.Substring(2, right.IndexOf(")") - 2);
                    string val = right;
                    int valId = project.GetObjectId(val);

                    left = left.Trim();

                    string attrName = code.Substring(code.IndexOf(".") + 1, code.IndexOf(op) - code.IndexOf(".") - 1);

                    attrName = attrName.Trim().ToLower();
                    int value;
                    if (attrIndexes.TryGetValue(attrName, out value))
                    {//test attr
                        int attrNum = value;
                        //                    WriteAttrTest(code, objId, left.Trim(), attrName, attrNum, op, valId, label, sw);
                        WriteAttrTest(sw, code, left.Trim(), attrName, attrNum, op, right, label);
                    }
                    else if (IsProperty(attrName))
                    {//test property
                        WritePropTest(sw, code, left.Trim(), attrName, op, valId, label);
                    }
                }
                else if (IsVar(left))
                {//not dot, it's a var test
                    right = right.Substring(op.Length).Trim();
                    right = right.Substring(0, right.Length - 1);
                    WriteVarTest(sw, code, left, op, right, label);
                }
                else
                {
                    throw new Exception("unknown variable: " + left);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\r\nError near: " + code);
            }
        }


        protected void ParseExpr(string code, out string left, out string right, out string op)
        {
            op = GetOperator(code);
            right = code.Substring(code.IndexOf(op)).Trim();
            left = code.Substring(1, code.IndexOf(op)-1).Trim();
        }

        public string GetOperator(string code)
        {
            string[] ops = { "==", "!=", "<", ">" };

            for (int i = 0; i < ops.Length; i++)
            {
                if (code.IndexOf(ops[i])!=-1)
                {
                    return ops[i];
                }
            }

            throw new Exception("Invalid relational operator: " + code);
        }

        protected int GetBitPos(string propName)
        {
            
            double d = Convert.ToDouble(propBits[propName]);
            return (int)Math.Log(d, 2);
        }
    
    }
}
