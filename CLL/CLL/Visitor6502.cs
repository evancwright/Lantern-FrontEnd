using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLL
{

    public class Visitor6502 : AsmCommon, IVisitor
    {
        StreamWriter sw;
        protected int _label = 0;
        protected int labelId = 97;
        IGame.IGameXml game;

        public Visitor6502(IGame.IGameXml g)
        {
            game = g;
            //
        }

        public void SetStreamWriter(StreamWriter sw)
        {
            this.sw = sw;
        }

        ~Visitor6502()
        {
            if (sw!= null)
            {
                sw.Close();
            }
        }

        public void Visit(IfStatement ifs)
        {
            //arg has been left on stack
            Console.WriteLine("; if statement");

            sw.WriteLine("\tpla ; pop condition");
            sw.WriteLine("\tcmp #1");
            sw.WriteLine("\tDB $F0,$03 ; beq 3 ; enter if-body");
//            sw.WriteLine("\tjmp " + ifs.Body.endBody.Text);

            //                Console.WriteLine("pop stack into a ");
            //                Console.WriteLine("compare to 0");
            //                Console.WriteLine("jump on zero to label?");
        }

        /*This is the same as an IfStatement
         */
        public void Visit(WhileLoop wh)
        {
            //arg has been left on stack
            sw.WriteLine("\t; while loop jump");

            sw.WriteLine("\tpla ; pop condition");
            sw.WriteLine("\tcmp #1");
            sw.WriteLine("\tDB $F0,$03 ; beq 3 ; enter if-body");
            //            sw.WriteLine("\tjmp " + ifs.Body.endBody.Text);

            //                Console.WriteLine("pop stack into a ");
            //                Console.WriteLine("compare to 0");
            //                Console.WriteLine("jump on zero to label?");
        }

        public void Visit(Print ps)
        {
            try
            {
                //arg has been left on stack
                sw.WriteLine("\t;call print");
                sw.WriteLine("\t;building a print statement");

                sw.WriteLine("\tlda #<string_table");
                sw.WriteLine("\tsta tableAddr");
                sw.WriteLine("\tlda #>string_table");
                sw.WriteLine("\tsta tableAddr+1");
                sw.WriteLine("\tlda #" + game.GetStringId(ps.text) + " ; " + "\"" + ps.text + "\"");
                sw.WriteLine("\tjsr printix");
            }
            catch (Exception ex)
            {
                throw new Exception("Visit(Print(" + ps.text + ")) failed", ex);
            }
        }

        public void Visit(NewLn nl)
        {
            sw.WriteLine("\tjsr printcr ; newline");
        }

        public void Visit(PrintLn ps)
        {
            sw.WriteLine("\t;building a println statement");

            sw.WriteLine("\tlda #<string_table");
            sw.WriteLine("\tsta tableAddr");
            sw.WriteLine("\tlda #>string_table");
            sw.WriteLine("\tsta tableAddr+1");
            sw.WriteLine("\tlda #" + game.GetStringId(ps.text) + " ; " + "\"" + ps.text + "\"");
            sw.WriteLine("\tjsr printix");
            sw.WriteLine("\tjsr printcr ; print newline");
        }

        public void Visit(PrintVar pv)
        {
            sw.WriteLine("\t; printing " + pv.VarName);
            sw.WriteLine("\tlda " + game.GetVarAddr(pv.VarName));
            sw.WriteLine("\tsta divisor");
            sw.WriteLine("\tjsr itoa ; convert and print");
        }


        public void Visit(PrintObjectName ps)
        {
            sw.WriteLine("\tpla");
            sw.WriteLine("\tjsr print_obj_name");
        }

        public void Visit(Rand r)
        {
            //upper val is already on the stack
            Console.WriteLine("call rand()");
            Console.WriteLine("pop upper val off stack");
            sw.WriteLine("\t; building a rand(x) call");
            sw.WriteLine("\tjsr next_rand ;");
            sw.WriteLine("\tpla ; pull # to mod by");
            sw.WriteLine("\ttay ; (divisor)");
            sw.WriteLine("\tlda rval ; thing to divide");
            sw.WriteLine("\tjsr div ; computes the mod");
            sw.WriteLine("\tlda remainder ; (from div)");
            sw.WriteLine("\tpha ; push result of Rand(x)");
        }

        public void Visit(Has ph)
        {
            sw.WriteLine("\tpla ; get child");
            sw.WriteLine("\tsta child");
            sw.WriteLine("\tpla ; get parent");
            sw.WriteLine("\tsta parent");
            sw.WriteLine("\tjsr check_ancestor");
            sw.WriteLine("\tlda ancestorFlag");
            sw.WriteLine("\tpha ; push it onto stack");
        }

        public void Visit(VariableRVal v)
        {
            sw.WriteLine("\tlda " + game.GetVarAddr(v.VarName));
            sw.WriteLine("\tpha");
        }

        public void Visit(Constant Mc)
        {
            Console.WriteLine("Push constant " + Mc.Value + " onto stack");
            sw.WriteLine("\tlda #" + Mc.Value);
            sw.WriteLine("\tpha");
        }

        public void Visit(StringLiteral sl)
        {
            try
            {
                Console.WriteLine("Push id of  " + sl.s + " onto stack");

                //push game.GetStrignId(sl);
                sw.WriteLine("\tlda #" + game.GetStringId(sl.s) + " ; " + sl.s);
                sw.WriteLine("\tpha");
            }
            catch (Exception e)
            {
                throw new Exception("Invalid string literal " + sl, e);
            }
        }

        public void Visit(And c)
        {
            sw.WriteLine("\t; writing && statement");
            Console.WriteLine("writing &&");
            Console.WriteLine("push 1 if zero flag set, else push 0");

            sw.WriteLine("\tpla ; pop a into temp");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpla ; pop rhs");

            sw.WriteLine("\tand temp");
            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tlsr a ; shift z bit in rightmost place");
            sw.WriteLine("\tclc");
            sw.WriteLine("\tadc #1 ; flip it");
            sw.WriteLine("\tand #1 ; mask off 0 bit");
            sw.WriteLine("\tpha ; push result of &&");

            Console.WriteLine("push z flag onto stack");
        }


        public void Visit(Or c)
        {

   //         Console.WriteLine("push 1 if zero flag set, else push 0");
            
  //          Console.WriteLine("Pop stack into a");
            sw.WriteLine("\t; building || statement");

            sw.WriteLine("\tpla");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tora temp ; set z flag");
            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tlsr a ; put z flag in rightmost place");
            sw.WriteLine("\tclc");
            sw.WriteLine("\tadc #1 ; flip it");
            sw.WriteLine("\tand #1 ; mask off z bit");
            sw.WriteLine("\tpha ; push result of ||");

//            Console.WriteLine("push z flag onto stack");

        }
        /*
                public void Visit(Not m)
                {
                    Console.WriteLine("Pop stack into a");
                    Console.WriteLine("call negate a");
                    Console.WriteLine("push inverted val back onto the stack");
                }
        */
        //==
        public void Visit(Compare c)
        {
            sw.WriteLine("\t; building == statement");

     //       Console.WriteLine("Pop stack into a");
     //       Console.WriteLine("Compare a,b");
     //       Console.WriteLine("push z flag onto stack");

            PopAndCompare();
            
            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tlsr a ; put z bit in rightmost place");
            sw.WriteLine("\tand #1  ; mask z bit");
            sw.WriteLine("\tpha ; push result of == ");
        }

        /// <summary>
        /// compare and push inverse of z flag
        /// </summary>
        /// <param name="m"></param>
        public void Visit(NotEquals m)
        {
            sw.WriteLine("\t    ; building != statement");
            /*
            Console.WriteLine("Pop stack into a");
            Console.WriteLine("Pop stack into b");
            Console.WriteLine("compare a,b");
            Console.WriteLine("push ! zero flag onto stack");

            Console.WriteLine("Pop stack into a");
            */
            PopAndCompare();

//            Console.WriteLine("Compare a,b");

            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tlsr a ; put z bit in rightmost place");
            sw.WriteLine("\tand #1 ; mask off 0 bit");
            sw.WriteLine("\tclc");
            sw.WriteLine("\tadc #1 ; flip it");
            sw.WriteLine("\tand #1 ; mask off 0 bit (again)");
            sw.WriteLine("\tpha ; push result of != ");

//            Console.WriteLine("push z flag onto stack");
        }


        public void Visit(Plus p)
        {
            //operand are both on stack
  //          Console.WriteLine("Pop stack into a");
            sw.WriteLine("\t; building a + operation");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tclc");
            sw.WriteLine("\tadc temp");
            sw.WriteLine("\tpha");
        }

        public void Visit(Mult p)
        {
            //operand are both on stack'
            /*
            Console.WriteLine("Pop stack into a");
            Console.WriteLine("Pop stack into b");
            Console.WriteLine("mult a, b");
            Console.WriteLine("push result of a * b onto stack");
            */
            throw new Exception("multiplication is not supported.");
        }


        public void Visit(Minus m)
        {
            //operand are both on stack
            sw.WriteLine("\t;building a minus");
            /*
            Console.WriteLine("Pop stack into a");
            Console.WriteLine("Pop stack into b");
            Console.WriteLine("a sub b");
            Console.WriteLine("push result of a-b onto stack");
            Console.WriteLine("Pop stack into a");
            */
            sw.WriteLine("\tpla ; get lhs");
            sw.WriteLine("\tsta temp ; save it");
            sw.WriteLine("\tpla ; pull rhs");
            sw.WriteLine("\tsec");
            sw.WriteLine("\tsbc temp");
            sw.WriteLine("\tpha ; push result of minus");
        }

        public void Visit(GreaterThan m)
        {/*
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("compare a,b");
            Console.WriteLine("push a 1 if carry flag set");
            */
            PopAndCompare();

            //carry flag is not set or the 
            //zero flag is set
            //zc
            //00 - greater than (yes)
            //01 - less than (no)
            //10 - equal (no)
            //11 - impossible
            //need to determine z + c == 0
            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tand #3 ; isolate z and c for greater than");
            sw.WriteLine("\tcmp #0 ; test zc");
            sw.WriteLine("\tphp ; zc -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tand #2 ; isolate z");
            sw.WriteLine("\tlsr a ; right align z");
            sw.WriteLine("\tpha ; push result of gt");
        }

        public void Visit(LessThan m)
        {

            sw.WriteLine("\t    ; building < statement");
            /*
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("push 1 if carry set");
            */
            //carry set = less than
            PopAndCompare();
            //carry flag is not set or the 
            //zero flag is set
            //zc
            //00 - greater than (no)
            //01 - less than (yes)
            //10 - equal (no)
            //11 - impossible
            //need to determine that carry flag is 1

            sw.WriteLine("\tphp ; flags -> ");
            sw.WriteLine("\tpla");
            sw.WriteLine("\teor #$FF ; isolate carry flag");
            sw.WriteLine("\tand #1 ; isolate carry flag");
            sw.WriteLine("\tpha ; push carry flag");
        }

        public void Visit(GreaterThanEquals m)
        {
/*            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("push a 1 if carry flag or z flag set");
            */
            sw.WriteLine("\t;building  a >= operation");
            PopAndCompare();

            //carry flag is not set or the 
            //zero flag is set
            //zc
            //00 - greater than (yes)
            //01 - less than (no)
            //10 - equal (yes)
            //11 - impossible
            //need to determine that carry flag is 0
            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tand #1 ; isolate c for >=");
            sw.WriteLine("\tcmp #0 ; test");
            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tand #1  ; mask it");
            sw.WriteLine("\tpha ; push result of gte (carry == 0)");
        }

        public void Visit(LessThanEquals m)
        {
            sw.WriteLine("\t; build <= operation");
            /*
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("compare a,b");
            Console.WriteLine("push a 1 if carry  or z");
            */
            PopAndCompare();

            //zero flag is set
            //zc
            //00 - greater than (no)
            //01 - less than (yes)
            //10 - equal (yes)
            //11 - impossible
            //need to determine that z + c != 0 

            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tand #3 ; isolate zc for <=");
    //        sw.WriteLine("\tcmp #0 ; isolate zc for <=");
    //        sw.WriteLine("\tphp ; flags -> a");
    //        sw.WriteLine("\tpla ; ");
            sw.WriteLine("\tcmp #0 ; are z and c zero?");
            sw.WriteLine("\tphp ; flags -> a");
            sw.WriteLine("\tpla ; ");
            sw.WriteLine("\tlsr ; isolate z");
            sw.WriteLine("\tclc");
            sw.WriteLine("\tadc #1 ; flip it");
            sw.WriteLine("\tand #1 ; mask it");
            sw.WriteLine("\tpha ; push result of lte");
        }


        public void Visit(VarAssignment va)
        {
//            Console.WriteLine("pop stack into a ");
  //          Console.WriteLine("sta " + va.VarName + ", a");
            sw.WriteLine("\t;variable assignment");
            sw.WriteLine("\tpla");
            sw.WriteLine("\tsta " + va.VarName);
        }

        /// <summary>
        /// Builds the call to assign and object's attr
        /// </summary>
        /// <param name="va"></param>
        public void Visit(AttrAssignment va)
        {
            sw.WriteLine("\t; building set attr call");

            sw.WriteLine("\tpla ; attr val -> x");
            sw.WriteLine("\ttax");
            try
            {
                sw.WriteLine("\tlda #" + attrIndexes[va.attrName] + "   ; attr # -> y (" + va.attrName + ")");
                sw.WriteLine("\ttay ; ");
            }
            catch
            {
                throw new Exception(va.attrName + " is not a valid attribute");
            }

            sw.WriteLine("\tpla ; pull object # off stack");
           
            sw.WriteLine("\tjsr set_obj_attr");
//            Console.WriteLine("jsr set_obj_attr");
        }

        public void Visit(PropAssignment pa)
        {
            sw.WriteLine("\t; building property assignment call");

            //id,attr,and val are on stack
            
            sw.WriteLine("\tpla ; pull rhs off stack");
            sw.WriteLine("\ttay ; val goes in Y");
            try
            {
                sw.WriteLine("\tldx #" + propIndexes[pa.propName] + " ; " + pa.propName);
            }
            catch
            {
                throw new Exception(pa.propName + " is not a valid property");
            }
            sw.WriteLine("\tpla ; pull object id");
//            sw.WriteLine("\tlda #" + game.GetObjectId(pa.objName) + " ; " + pa.objName);
            sw.WriteLine("\tjsr set_obj_prop");
  //          Console.WriteLine("call set_obj_prop");
            
        }

        public void Visit(PropertyRVal prv)
        {
    //        Console.WriteLine("call get_obj_prop ; returns val in a");
            //Console.WriteLine("push a onto stack");
            sw.WriteLine("\t; getting a property");
            sw.WriteLine("\tpla ; get object id into A");
            //int bit = (int)Math.Log(Convert.ToDouble(propBytes[prv.PropName]), 2) + 1;
            int bit = propIndexes[prv.PropName];
            sw.WriteLine("\tldx #" + bit + " ; " + prv.PropName);
            sw.WriteLine("\tjsr get_obj_prop ; A=1-16 ");
            sw.WriteLine("\tpha");
        }

        public void Visit(AttributeRVal prv)
        {
            sw.WriteLine("\t;getting an attribute");
      //      Console.WriteLine("call get_obj_attr ; returns val in a");
            sw.WriteLine("\tpla ; get object id from stack");
            // sw.WriteLine("\tlda #" + game.GetObjectId(prv.ObjName) + " ; " + prv.ObjName);
            sw.WriteLine("\tldy #" + attrIndexes[prv.AttrName] + " ; " + prv.AttrName);
            sw.WriteLine("\tjsr get_obj_attr");
            sw.WriteLine("\tpha ; put attr on stack");
        }

        public void Visit(Label lbl)
        {
            sw.WriteLine(lbl.Text);
        }

        public void Visit(Look look)
        {
            sw.WriteLine("\tjsr look_sub");
        }

        public void Visit(Ask ask)
        {
            /*read a line of input*/
            sw.WriteLine("\t;writing ask");
            sw.WriteLine("\tjsr ask");
//            throw new NotImplementedException("Visitor6502::Visit(Ask) not implemented.");
}

public void Visit(Move move)
{
    sw.WriteLine("\tjsr move_player");
}

public void Visit(Jump j, string lbl)
{
    sw.WriteLine("\tjmp " + lbl);
}


public void Visit(Call c)
{
    string name = c.name;
    if (name.IndexOf('(') !=-1 )
    {
        name = name.Substring(0, name.IndexOf('(')).Trim();
    }

    sw.WriteLine("\tjsr " + name + "_sub");
}

public void Visit(Function f)
{
    try
    {
        sw.WriteLine(f.name + " ; start subroutine");
        f.body.Accept(this);
        sw.WriteLine("\trts");
    }
    catch (Exception ex)
    {
        throw new Exception("6502Visitor::Visit(Function) failed", ex);
    }
}

public string GetNextLabel()
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

    return "_" + s;
}


void PopAndCompare()
{
    sw.WriteLine("\tpla ; rhs");
    sw.WriteLine("\tsta temp");
    sw.WriteLine("\tpla ; lhs");
    sw.WriteLine("\tcmp temp");
            /*
    sw.WriteLine("\tpla");
    sw.WriteLine("\ttax ; move operand out of the way");
    sw.WriteLine("\tpla");
    sw.WriteLine("\tsta temp");
    sw.WriteLine("\ttxa ; move operand back");
    sw.WriteLine("\tcmp temp");
    */
        }


public void WriteLine(String s)
{
    sw.WriteLine(s);
}


public void WriteSubName(string text)
{
    sw.WriteLine(text + " ; start subroutine");
}

public void WriteReturn()
{
    sw.WriteLine("\trts");
}

public void WriteEventCall(string label)
{
    sw.WriteLine("\tjsr " + label);
}

public static string TruncateName(string name)
{
    if (name.Length < 20)
    {
        return name;
    }
    return name.Substring(0, 20);
}

public void SaveRegs() { }
public void RestoreRegs() { }
}

}
 