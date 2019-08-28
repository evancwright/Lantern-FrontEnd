using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CLL
{
    public class Visitor6809 : AsmCommon, IVisitor
    {
        StreamWriter sw;
        protected int _label = 0;
        protected int labelId = 97;
        IGame.IGameXml game;


        public Visitor6809(IGame.IGameXml g)
        {
            game = g;
        }

        public void SetStreamWriter(StreamWriter sw)
        {
            this.sw = sw;
        }

        public void Visit(WhileLoop wh)
        {
            throw new Exception("Visit 6809 while loop not implemented.");
        }

        public void Visit(IfStatement ps)
        {
            Console.WriteLine("; if statement");

            sw.WriteLine("\tpuls a ; pop condition");
            sw.WriteLine("\tcmpa #1");
            //sw.WriteLine("\tbeq 3h ; enter if-body");
            sw.WriteLine("\t.db $27,$03 ; beq 3 ; enter if-body");
            //  sw.WriteLine("\t.byte 27h ; enter if-body");
            //  sw.WriteLine("\t.byte 03 ; beq 3");
        }

        public void Visit(Print ps)
        {

            /*calling assembly routine*/

            //cmoc
            sw.WriteLine("\t;print statement");
            sw.WriteLine("\tlda #" + game.GetStringId(ps.text) + " ; /*" + ps.text + "*/");
            sw.WriteLine("\tpshs a");
            sw.WriteLine("\tlda #0");
            sw.WriteLine("\tpshs a");
            sw.WriteLine("\tjsr print_string");
            sw.WriteLine("\tleas 2,s ; pop params");
        }

        public void Visit(NewLn nl)
        {
            sw.WriteLine("\tjsr printcr");
        }

        public void Visit(PrintLn ps)
        {
            sw.WriteLine("\t;print statement");
            sw.WriteLine("\tlda #" + game.GetStringId(ps.text) + " ; /*" + ps.text + "*/");
            sw.WriteLine("\tpshs a");
            sw.WriteLine("\tlda #0");
            sw.WriteLine("\tpshs a");
            sw.WriteLine("\tjsr print_string");
            sw.WriteLine("\tleas 2,s ; pop params");
            sw.WriteLine("\tjsr printcr");
        }

        public void Visit(PrintObjectName ps)
        {
            sw.WriteLine("\t;print name statement (id already on stack)");
            //the object number should already be on the stack
            //sw.WriteLine("\tpuls a ; pop obj name to print");
            sw.WriteLine("\tclra ; pad it with a 0");
            sw.WriteLine("\tpshs a ; to promote it to a short");
            sw.WriteLine("\tjsr print_obj_name");
            sw.WriteLine("\tleas 2,s ; pop param");
        }

        public void Visit(PrintVar pv)
        {
            throw new NotImplementedException("6809 Print var not implemented.");
        }

        public void Visit(Rand r)
        {
            sw.WriteLine("\t;rand()");
            sw.WriteLine("\tpuls a ; get divisor");
            sw.WriteLine("\tclrb ; ");
            sw.WriteLine("\tpshs d");
            sw.WriteLine("\tjsr rand8 ; leaves # in b");
            sw.WriteLine("\tleas 2,s");
            sw.WriteLine("\tpshs b ; put result back on stack");

        }

        public void Visit(Has ph)
        {

            //push return param
            sw.WriteLine("\tpshu a ; space for return val");

            //pop child
            sw.WriteLine("\tpuls a ; push parent");
            sw.WriteLine("\tpshu a");
            //pop parent
            sw.WriteLine("\tpuls a ; push child");
            sw.WriteLine("\tpshu a");

            //call ancestor function
            //; top of stack is player (holder)
            //; under that is object (child)
            //; under that is space for return var
            sw.WriteLine("\tjsr is_child_of");
            sw.WriteLine("\tpulu a ; move result to system stack");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(VariableRVal v)
        {
            string addr;
            //this is done in fix var name()
            if (v.VarName == "dobj")
                addr = "DobjId";
            else if (v.VarName == "iobj")
                addr = "IobjId";
            else
                addr = game.GetVarAddr(v.VarName);

            sw.WriteLine("\tlda " + addr);
            sw.WriteLine("\tpshs a");
        }

        public void Visit(AttributeRVal v)
        {
            //get attr push it on stack
            /*
            ;params are on user stack
            ; top param is attr to get
            ; next param is obj id
            */

            sw.WriteLine("\t;getting an attr");
            sw.WriteLine("\tpuls b ; pull object #");
            sw.WriteLine("\tclra");
            sw.WriteLine("\ttfr d,x");

            try
            {
                sw.WriteLine("\tldb #" + attrIndexes[v.AttrName] + " ; " + v.AttrName);
                sw.WriteLine("\tpshs d");
                sw.WriteLine("\tpshs x");
            }
            catch (Exception e)
            {
                throw new Exception(v.AttrName + " is not a valid attr for " + v.ObjName, e);
            }
            /*
                    puls b ; pull object #
        clra ;
        tfr d,x ;
        ldb #1 ; holder
        pshs d ; push it as a short
        pshs x ; push object # as a short
        jsr get_object_attr
        leas 4,s
        pshs b ; move rslt to sys stack

            */
            sw.WriteLine("\tjsr get_object_attr");
            sw.WriteLine("\tleas 4,s");
            sw.WriteLine("\tpshs b ; move rslt to sys stack");
        }


        public void Visit(PropertyRVal v)
        {
            /*
             ; returns the property for the object on the 
             ; user stack. the value will be either 1 or 0
             ; parameters on sys stack
             ; 1-object id (top)
             ; 2-property number

             */
            sw.WriteLine("\t;getting a property");

            sw.WriteLine("\tpuls b ; pull object #");
            sw.WriteLine("\tclra");
            sw.WriteLine("\ttfr d,x ; save it");


            try
            {
                sw.WriteLine("\tldb #" + propIndexes[v.PropName] + " ; " + v.PropName);
                sw.WriteLine("\tclra");
                sw.WriteLine("\tpshs d");
            }
            catch (Exception e)
            {
                throw new Exception(v.PropName + " is not a valid prop for " + v.ObjName, e);
            }

            sw.WriteLine("\tpshs x ; push object #");

            sw.WriteLine("\tjsr get_object_prop ; result -> B");
            sw.WriteLine("\tleas 4,s");
            sw.WriteLine("\tpshs b ; push result on stack sys stack");
        }

        public void Visit(Constant Mc)
        {
            sw.WriteLine("\t;constant");
            sw.WriteLine("\tlda #" + Mc.Value);
            sw.WriteLine("\tpshs a");
        }

        public void Visit(StringLiteral sl)
        {
            throw new NotImplementedException();
        }

        public void Visit(And c)
        {
            sw.WriteLine("\t;&&");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tanda temp");
            sw.WriteLine("\tcmpa #1");
            sw.WriteLine("\ttfr cc,a");
            sw.WriteLine("\tanda #4 ; isolate z flag");
            sw.WriteLine("\tlsra ; z-> position 0");
            sw.WriteLine("\tlsra");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(Or c)
        {
            sw.WriteLine("\t;||");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tora ,s");
            sw.WriteLine("\tleas 1,s ; pop operand");
            sw.WriteLine("\tpshs a");
        }

        //   void Visit(Not m);
        public void Visit(Compare c)
        {
            sw.WriteLine("\t;==");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tcmpa temp");
            sw.WriteLine("\ttfr cc,a");
            sw.WriteLine("\tanda #4 ; isolate z flag");
            sw.WriteLine("\tlsra ; z-> position 0");
            sw.WriteLine("\tlsra");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(Plus p)
        {
            sw.WriteLine("\t;addition");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tadda temp");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(Mult p)
        {
            sw.WriteLine("\t;multiplication (8-bit)");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tpuls b");
            sw.WriteLine("\tmul");
            sw.WriteLine("\tpshs b");
        }

        public void Visit(Minus m)
        {
            sw.WriteLine("\t;subtraction");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsuba temp");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(GreaterThan m)
        {
            //compare and push 1 if carry and zero are 0
            //zc
            //00 = a > b
            //01 = a < b
            //10 = a == b
            //push a 1 if zc == 0

            sw.WriteLine("\t;a > b");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tcmpa temp");
            string psh = GetNextLabel();
            string zero = GetNextLabel();
            //sw.WriteLine("\tbls #4 ; C or Z is set - load a #0");
            sw.WriteLine("\tbls " + zero);
            sw.WriteLine("\tlda #1");
            sw.WriteLine("\tbra " + psh);
            sw.WriteLine(zero);
            sw.WriteLine("\tlda #0");
            sw.WriteLine(psh);
            sw.WriteLine("\tpshs a");
        }

        public void Visit(LessThan m)
        {
            //compare and push a 1 if carry set
            sw.WriteLine("\t;a < b");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tcmpa temp");
            sw.WriteLine("\ttfr cc,a");
            sw.WriteLine("\tanda #1");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(GreaterThanEquals m)
        {
            //push a 1 if c == 0
            sw.WriteLine("\t;a >= b");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tcmpa temp");
            sw.WriteLine("\ttfr cc,a");
            sw.WriteLine("\tcoma ; flip bits");
            sw.WriteLine("\tanda #1 ; isolate c");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(LessThanEquals m)
        {
            //zc
            //00 = a > b
            //01 = a < b
            //10 = a == b
            //push a 1 if zc != 0

            sw.WriteLine("\t;a >= b");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tcmpa temp");
            string one = GetNextLabel();
            string push = GetNextLabel();
            sw.WriteLine("\tbls " + one + " ; C or Z is set - load a #1");
            sw.WriteLine("\tlda #0");
            sw.WriteLine("\tbra " + push);
            sw.WriteLine(one);
            sw.WriteLine("\tlda #1");
            sw.WriteLine(push);
            sw.WriteLine("\tpshs a");
        }

        //void Visit(Assign m);
        public void Visit(VarAssignment m)
        {
            sw.WriteLine("\t;writing a set var statement");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta " + m.VarName);
        }

        public void Visit(AttrAssignment asn)
        {
            /*
            ;set_object_attribute
            ;params are on system stack
            ;top param is new value
            ;next param is attr to set 
            ;next is object
             */

            sw.WriteLine("\t;writing a set attribute statement");

            sw.WriteLine("\tpuls b ; pull value (rhs)");
            sw.WriteLine("\tclra ; ");
            sw.WriteLine("\ttfr d,x ; ");
            sw.WriteLine("\tpuls b ; pull obj # (lhs)");
            sw.WriteLine("\ttfr d,y ; save obj # param");

            try
            {
                sw.WriteLine("\tldb #" + attrIndexes[asn.attrName] + " ; push attr index");
            }
            catch (Exception e)
            {
                throw new Exception(asn.attrName + " is not a valid attr for " + asn.objName, e);
            }

            sw.WriteLine("\tpshs x ; push value param");
            sw.WriteLine("\tpshs d ; push attr param");
            sw.WriteLine("\tpshs y ; push attr param");
            sw.WriteLine("\tjsr set_object_attr");
            sw.WriteLine("\tleas 6,s");
        }

        public void Visit(PropAssignment m)
        {
            /*;set_object_property
                ; top = object #id
                ; under that = prop# ( 0 - 15)
                ; under that = value (0 or 1)
            */
            sw.WriteLine("\t; writing set obj prop");
            sw.WriteLine("\tpuls b ; pull value (rhs)");
            sw.WriteLine("\tclra ; ");
            sw.WriteLine("\ttfr d,x ; ");
            sw.WriteLine("\tpuls b ; pull obj # (lhs)");
            sw.WriteLine("\ttfr d,y ; save obj # param");
            try
            {
                sw.WriteLine("\tldb #" + propIndexes[m.propName] + " ;" + m.propName);
            }
            catch (Exception e)
            {
                throw new Exception(m.propName + " is not a valid attr for " + m.objName, e);
            }
            sw.WriteLine("\tpshs x ; push value param");
            sw.WriteLine("\tpshs d ; push prop # param");
            sw.WriteLine("\tpshs y ; push obj # param");
            sw.WriteLine("\tjsr set_object_prop");
            sw.WriteLine("\tleas 6,s");
        }

        public void Visit(NotEquals m)
        {
            sw.WriteLine("\t;!=");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tsta temp");
            sw.WriteLine("\tpuls a");
            sw.WriteLine("\tcmpa temp");
            sw.WriteLine("\ttfr cc,a");
            sw.WriteLine("\tcoma ; flip bits");
            sw.WriteLine("\tanda #4 ; isolate z flag");
            sw.WriteLine("\tlsra ; z-> position 0");
            sw.WriteLine("\tlsra");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(Label lbl)
        {
            sw.WriteLine("@" + lbl.Text);
        }

        public void Visit(Jump jmp, string label)
        {
            sw.WriteLine("\tlbra @" + label);
        }

        public void Visit(Move move)
        {
            sw.WriteLine("\t;move statement");
            sw.WriteLine("\tjsr move_sub");
        }

        public void Visit(Look look)
        {
            sw.WriteLine("\t;look statement");
            sw.WriteLine("\tjsr look_sub");
        }

        public void Visit(Ask ask)
        {
            sw.WriteLine("\tjsr ask");
        }


        public void Visit(Call c)
        {
            sw.WriteLine("\tjsr " + c.name + "_sub");
        }

        public void Visit(Function f)
        {
            sw.WriteLine(f.name + " /* start subroutine */ ");
            sw.WriteLine("void " + f.name + "()\n{");
            f.body.Accept(this);
            //sw.WriteLine("\trts"); /* cmoc doesn't need rts */
            sw.WriteLine("}");
        }

        public void WriteLine(string text)
        {
            throw new NotImplementedException();
        }

        public void WriteSubName(string text)
        {
            sw.WriteLine("/*" + text + " subroutine  */");
            sw.WriteLine("void " + text + "() {");
            sw.WriteLine("asm {");
            //sw.WriteLine(text);

        }

        public void WriteReturn()
        {
            //            sw.WriteLine("\trts");
            sw.WriteLine("\t}"); /*close inline asm */
            sw.WriteLine("}"); /*close C function */
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

            return "@" + s.ToString().ToLower();
        }

        public void WriteEventCall(string label)
        {
            //sw.WriteLine("\tjsr " + label);
            sw.WriteLine("\t" + label + "();");
        }


        public void SaveRegs() { }
        public void RestoreRegs() { }
    }
}
