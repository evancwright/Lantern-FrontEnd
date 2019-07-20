using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
 

namespace CLL
{


    public class Visitor8086 : AsmCommon, IVisitor
    {
        StreamWriter sw;
        protected int _label = 0;
        protected int labelId = 97;
        IGame.IGameXml game;

        public Visitor8086(IGame.IGameXml g)
        {
            game = g;
        }

        public void SetStreamWriter(StreamWriter sw)
        {
            this.sw = sw;
        }
 
        public void Visit(IfStatement ps)
        {
            Console.WriteLine("\tnop ; if statement");

            sw.WriteLine("\tpop ax; pop condition");
            sw.WriteLine("\tcmp ax,1");
            sw.WriteLine("\tdb 0x74, 0x3 ;jz 3 ; jmp = relative");
        }

        public void Visit(Print ps)
        {
            sw.WriteLine("\tnop ;print statement");
            //sw.WriteLine("\tmov ax,StringTable");
            //sw.WriteLine("\tpush ax");
            sw.WriteLine("\tmov ah,0");
            sw.WriteLine("\tmov al," + game.GetStringId(ps.text) + " ; " + ps.text);
            sw.WriteLine("\tpush ax");
            sw.WriteLine("\tcall print_string");
            sw.WriteLine("\tadd sp,2  ; pop 2 params");
        }

        public void Visit(PrintLn ps)
        {
            sw.WriteLine("\tnop ;print statement");
            //sw.WriteLine("\tmov ax,StringTable");
            //sw.WriteLine("\tpush ax"); 
            sw.WriteLine("\tmov ah,0");
            sw.WriteLine("\tmov al," + game.GetStringId(ps.text) + " ; " + ps.text);
            sw.WriteLine("\tpush ax");
            sw.WriteLine("\tcall print_string");
            sw.WriteLine("\tadd sp,2  ; pop 2 bytes");
            sw.WriteLine("\tcall print_cr");
        }

        public void Visit(PrintObjectName ps)
        {
            sw.WriteLine("\tnop ;print name statement");
            sw.WriteLine("\tcall print_obj_name");
            sw.WriteLine("\tadd sp,2  ; pop params");
        }

        public void Visit(PrintVar pv)
        {
            throw new NotImplementedException("8086 PrintVar not implemented.");
        }

        public void Visit(Rand r)
        {
            sw.WriteLine("\tnop ;rand()");
            sw.WriteLine("\t call rand  ; param already on stack");
            sw.WriteLine("\t add sp,2  ; pop param");
            sw.WriteLine("\t push ax ; push result");
        }

        public void Visit(Has ph)
        {


            //pop child
            sw.WriteLine("\t; child is on stack");
            //pop parent
            sw.WriteLine("\t; parent is on stack");

            //call ancestor function
            sw.WriteLine("\tcall is_child_of");
            sw.WriteLine("\tadd sp,4");
            sw.WriteLine("\tmov ah,0 ; clear high byte");
            sw.WriteLine("\tpush ax");
        }

        public void Visit(VariableRVal v)
        {
            sw.WriteLine("\tnop ; variable rval");
            sw.WriteLine("\tmov ah,0");
            //            sw.WriteLine("\tmov al,[" + game.GetVarAddr(v.VarName) + "]");
            sw.WriteLine("\tmov al,[" + FixVarName(v.VarName) + "]");
            sw.WriteLine("\tpush ax");
        }

        public void Visit(AttributeRVal v)
        {
            //get attr push it on stack
            /*
            ; params are on user stack
            ; top param is obj id
            */

            sw.WriteLine("\tnop ;getting an attr");

            try
            {
                sw.WriteLine("\tpop bx ; get object id");
                sw.WriteLine("\tmov ah,0");
                sw.WriteLine("\tmov al," + attrIndexes[v.AttrName] + " ; " + v.AttrName);
                sw.WriteLine("\tpush ax ; push attr #");
                sw.WriteLine("\tpush bx ; push object id");
                sw.WriteLine("\tcall get_obj_attr ; result->ax");
                sw.WriteLine("\tadd sp,4 ; pop 2 params");
                sw.WriteLine("\tmov ah,0 ; clear high byte");
                sw.WriteLine("\tpush ax");
            }
            catch (Exception e)
            {
                throw new Exception(v.AttrName + " is not a valid attr for " + v.ObjName, e);
            }

        }

        public void Visit(PropertyRVal v)
        {
            /*
             ; returns the property for the object on the 
             ; user stack. the value will be either 1 or 0
             ; parameters on user stack
             ; 1-object id (top)

             */
            sw.WriteLine("\t;getting a property");
            try
            {
                sw.WriteLine("\tpop bx ; pull param so we can reorder stack");
                sw.WriteLine("\tmov ah,0");
                sw.WriteLine("\tmov al," + propIndexes[v.PropName] + " ; " + v.PropName);
                sw.WriteLine("\tpush ax ; property param is on bottom");
                sw.WriteLine("\tpush bx ; push object #");
            }
            catch (Exception e)
            {
                throw new Exception(v.PropName + " is not a valid prop for " + v.ObjName, e);
            }

            sw.WriteLine("\tcall get_obj_prop ; result -> A");
            sw.WriteLine("\tadd sp,4 ; pop 2 params");
            sw.WriteLine("\tmov ah,0 ; clear high byte");
            sw.WriteLine("\tpush ax ; push result");
        }

        public void Visit(Constant Mc)
        {
            sw.WriteLine("\tnop ;constant");
            sw.WriteLine("\tmov ax," + Mc.Value);
            sw.WriteLine("\tpush ax");
        }

        public void Visit(StringLiteral sl)
        {
            throw new NotImplementedException();
        }

        public void Visit(And c)
        {
            sw.WriteLine("\tnop ;&&");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tand ax,bx");
            sw.WriteLine("\tpushf ; flags -> ax");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tnot ax ; 0 == &&");
            sw.WriteLine("\tand ax,64");
            sw.WriteLine("\tmov cx,6");
            sw.WriteLine("\tshr ax,cl ; z-> position 0");
            sw.WriteLine("\tpush ax");
        }
        public void Visit(Or c)
        {
            sw.WriteLine("\tnop ;||");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tor ax,bx");
            sw.WriteLine("\tpushf ; flags -> ax");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tnot ax");
            sw.WriteLine("\tand ax,64");
            sw.WriteLine("\tmov cx,6");
            sw.WriteLine("\tshr ax,cl ; z-> position 0");
            sw.WriteLine("\tpush ax");
        }

        //   void Visit(Not m);
        public void Visit(Compare c)
        {
            sw.WriteLine("\tnop ;==");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tcmp ax,bx");
            sw.WriteLine("\tpushf ; flags -> ax");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tand ax,64 ; isolate z flag");
            sw.WriteLine("\tmov cx,6");
            sw.WriteLine("\tshr ax,cl ; z-> position 0");
            sw.WriteLine("\tpush ax");
        }

        public void Visit(Plus p)
        {
            sw.WriteLine("\tnop ;addition");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tadd ax,bx");
            sw.WriteLine("\tmov ah,0  ; truncate");
            sw.WriteLine("\tpush ax");
        }

        public void Visit(Mult p)
        {
            sw.WriteLine("\tnop ;multiplication (8-bit)");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tmul bx");
            sw.WriteLine("\tpush ax");
        }

        public void Visit(Minus m)
        {
            sw.WriteLine("\tnop ;subtraction");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tsub ax,bx");
            sw.WriteLine("\tpush ax");
        }

        public void Visit(GreaterThan m)
        {
            //compare and push 1 if carry and zero are 0
            //zc
            //00 = a > b
            //01 = a < b
            //10 = a == b
            //push a 1 if zc == 0

            sw.WriteLine("\tnop ;a > b");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tcmp ax,bx");
            string psh = GetNextLabel();
            string zero = GetNextLabel();
            //sw.WriteLine("\tbls 4 ; C or Z is set - load a #0");
            sw.WriteLine("\tjle " + zero);
            sw.WriteLine("\tmov ax,1");
            sw.WriteLine("\tjp " + psh);
            sw.WriteLine(zero);
            sw.WriteLine("\tmox ax,0");
            sw.WriteLine(psh);
            sw.WriteLine("\tpush ax");
        }

        public void Visit(LessThan m)
        {
            //compare and push a 1 if carry set
            sw.WriteLine("\tnop ;a < b");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tcmp ax,bx");
            sw.WriteLine("\tpushf");
            sw.WriteLine("\tpop ax ");
            sw.WriteLine("\tand a,1");
            sw.WriteLine("\tpshs a");
        }

        public void Visit(GreaterThanEquals m)
        {
            //push a 1 if c == 0
            sw.WriteLine("\tnop ;a >= b");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tcmp ax,bx");
            sw.WriteLine("\tcmc ; flip bits");
            sw.WriteLine("\tpushf");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tand ax,1 ; isolate c");
            sw.WriteLine("\tpush ax");
        }

        public void Visit(LessThanEquals m)
        {
            //zc
            //00 = a > b
            //01 = a < b
            //10 = a == b
            //push a 1 if zc != 0

            sw.WriteLine("\tnop ;a >= b");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tcmp ax,bx");
            string one = GetNextLabel();
            string push = GetNextLabel();
            sw.WriteLine("\tjle " + one + " ; C or Z is set - load a #1");
            sw.WriteLine("\tmov ax,0");
            sw.WriteLine("\tjp " + push);
            sw.WriteLine(one);
            sw.WriteLine("\tmov ax,1");
            sw.WriteLine(push);
            sw.WriteLine("\tpush ax");
        }

        public void Visit(VarAssignment m)
        {
            sw.WriteLine("\tnop ;writing a set var statement");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tmov " + m.VarName + ",al");
        }

        public void Visit(AttrAssignment asn)
        {
            /*
            ;set_obj_attribute
            ;params are on user stack
            ;top param is new value
            ;next param is attr to set 
            ;next is object
             */

            sw.WriteLine("\tnop ;writing a set attribute statement");

            //            sw.WriteLine("\tpop bx ; pull value (rhs)");
            //           sw.WriteLine("\tpop ax ; pull obj # (lhs)");
            //            sw.WriteLine("\tpshu a ; push obj # param");
            
            try
            {
                //stuff here
                sw.WriteLine("\tpop cx ; save value");
                sw.WriteLine("\tpop bx ; save obj id");

                sw.WriteLine("\tmov ax," + attrIndexes[asn.attrName] + " ; push attr index");
                sw.WriteLine("\tpush cx ; push value");
                sw.WriteLine("\tpush ax ; push attr # param");
                sw.WriteLine("\tpush bx ; push object id");
                sw.WriteLine("\tcall set_obj_attr");
                sw.WriteLine("\tadd sp,6 ; pop params");
            }
            catch (Exception e)
            {
                throw new Exception(asn.attrName + " is not a valid attr for " + asn.objName, e);
            }

        }

        public void Visit(PropAssignment m)
        {
            /*;set_obj_property
                ; top = value (0 or 1)
                ; under that = object #id
            */

            try
            {
                sw.WriteLine("\tpop cx ; save value");
                sw.WriteLine("\tpop bx ; save obj id");
                sw.WriteLine("\tmov  ax," + propIndexes[m.propName] + " ;" + m.propName);
                sw.WriteLine("\tpush cx ; push value");
                sw.WriteLine("\tpush  ax ; push prop number");
                sw.WriteLine("\tpush bx ; push object id");
                sw.WriteLine("\tcall set_obj_prop");
                sw.WriteLine("\tadd sp,6 ; pop params");
            }
            catch (Exception e)
            {
                throw new Exception(m.propName + " is not a valid attr for " + m.objName, e);
            }

        }

        public void Visit(NotEquals m)
        {
            sw.WriteLine("\tnop ;!=");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tpop bx");
            sw.WriteLine("\tcmp ax,bx");
            sw.WriteLine("\tpushf");
            sw.WriteLine("\tpop ax");
            sw.WriteLine("\tnot ax");
            sw.WriteLine("\tand ax,64; isolate z flag");
            sw.WriteLine("\tmov cx,6");
            sw.WriteLine("\tshr ax,cl; z-> position 0");
            sw.WriteLine("\tpush ax");
        }

        public void Visit(Label lbl)
        {
            sw.WriteLine(lbl.Text + "nop ; ");
        }

        public void Visit(Jump jmp, string label)
        {
            string nocolon = label.Substring(0, label.Length - 1);
            sw.WriteLine("\tjmp " + nocolon);
        }

        public void Visit(Move move)
        {
            sw.WriteLine("\tnop ;move statement");
            sw.WriteLine("\tcall move_sub");
        }

        public void Visit(Look look)
        {
            sw.WriteLine("\tnop ;look statement");
            sw.WriteLine("\tcall look_sub");
        }

        public void Visit(Ask ask)
        {
            sw.WriteLine("\tcall ask");
        }


        public void Visit(Call c)
        {
            string name = c.name;

            if (name.IndexOf('(') != -1)
            {
                name = name.Substring(0, name.IndexOf('(')).Trim();
            }
            sw.WriteLine("\tcall " + name + "_sub");
        }

        public void Visit(Function f)
        {
            sw.WriteLine("/* machine generated subroutine */");
            sw.WriteLine("");
            sw.WriteLine("void " + f.name + "()");
            sw.WriteLine("{");
            sw.WriteLine("\t__asm{");
            //  sw.WriteLine(":" + f.name + " ; start subroutine");
            //  sw.WriteLine("\tpush ebp");
            //  sw.WriteLine("\tmov ebp,esp");
            f.body.Accept(this);
            // sw.WriteLine("\tmov esp,ebp");
            //  sw.WriteLine("\tpop ebp");
            //   sw.WriteLine("\tret");
            sw.WriteLine("\tmov ax,ax\t};");
            sw.WriteLine("}");
        }

        public void WriteLine(string text)
        {
            throw new NotImplementedException();
        }

        public void WriteSubName(string text)
        {
            sw.WriteLine("/*" + text + " subroutine  */");
            sw.WriteLine("void " + text + "()");
            sw.WriteLine("{");
            sw.WriteLine("\t_asm\n\t{");
         }

        public void WriteReturn()
        {
            sw.WriteLine("\tmov ax,0 ; fix Watcom");
            sw.WriteLine("\tmov bx,ax ; fix Watcom");
            sw.WriteLine("\t};");
            sw.WriteLine("}");
        }

        public void WriteEventCall(string label)
        {
            string name = label;

            if (name.IndexOf('(') != -1)
            {
                name = name.Substring(0, name.IndexOf('(')).Trim();
            }

            sw.WriteLine("\t" + label  + "();");
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

            //return "." + s.ToString().ToLower();

            return s.ToString().ToLower() + "1:";
        }

  

        public void SaveRegs() { }
        public void RestoreRegs() { }

    }
}
