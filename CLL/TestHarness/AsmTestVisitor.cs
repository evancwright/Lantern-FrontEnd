using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLL;
using System.IO;

namespace TestHarness
{
    public class AsmTestVisitor : IVisitor
    {
        int labelNum = 1;
        public AsmTestVisitor(GameBase g)
        {
        }

        public void SetStreamWriter(StreamWriter sw)
        {

        }

        public void Visit(IfStatement ifs) 
        {
            //arg has been left on stack
            Console.WriteLine("; if statement");
            Console.WriteLine("pop stack into a ");
            Console.WriteLine("compare a to 0 (is it false)");
            Console.WriteLine("jump on not equal to " + ifs.exitLabel.Text);
             
        }

        public void Visit(Print ps)
        {
            //arg has been left on stack
            Console.WriteLine("call print");
            Console.WriteLine("pop the stack");
        }

        public void Visit(PrintLn ps)
        {
            //arg has been left on stack
            Console.WriteLine("call print");
            Console.WriteLine("pop the stack");
            Console.WriteLine("call printnewline");
        }

        public void Visit(Rand r)
        {
            //upper val is already on the stack
            Console.WriteLine("call rand()");
            Console.WriteLine("pop upper val off stack");
        }

        public void Visit(Has r)
        {
            //upper val is already on the stack
            Console.WriteLine("pop child Id off stack");
            Console.WriteLine("pop parent Id off stack");
            Console.WriteLine("call player has");
        }
        public void Visit(VariableRVal v)
        {
            Console.WriteLine("ld a, " + v.VarName);
            Console.WriteLine("push a onto stack");
        }

        public void Visit(Constant Mc)
        {
            Console.WriteLine("Push constant " + Mc.Value + " onto stack");
        }

        public void Visit(StringLiteral sl)
        {
            //push game.GetStrignId(sl);
            Console.WriteLine("Push id of  " + sl.s + " onto stack");
        }

        public void Visit(And c)
        {
            
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("and a, b");
            Console.WriteLine("push 1 if zero flag set, else push 0");
        }


        public  void Visit(Or c)
        {
            
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("or b");
            Console.WriteLine("push 1 if zero flag set, else push 0");
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
            Console.WriteLine("Pop stack into a");
            Console.WriteLine("Pop stack into b");
            Console.WriteLine("Compare a,b");
            Console.WriteLine("push z flag onto stack");
        }

        public void Visit(Plus p)
        {
            //operand are both on stack
            Console.WriteLine("Pop stack into a");
            Console.WriteLine("Pop stack into b");
            Console.WriteLine("add a, b");
            Console.WriteLine("push result of a + b onto stack");
        }

        public void Visit(Mult p)
        {
            //operand are both on stack
            Console.WriteLine("Pop stack into a");
            Console.WriteLine("Pop stack into b");
            Console.WriteLine("mult a, b");
            Console.WriteLine("push result of a * b onto stack");
        }
        public void Visit(Minus m)
        {
            //operand are both on stack
            Console.WriteLine("Pop stack into a");
            Console.WriteLine("Pop stack into b");
            Console.WriteLine("a sub b");
            Console.WriteLine("push result of a-b onto stack");
        }

        public void Visit(GreaterThan m)
        {
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("compare a,b");
            Console.WriteLine("push a 1 if carry flag set");
        }

        public void Visit(LessThan m)
        {
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("push a 1 if carry and z clear");
        }

        public void Visit(GreaterThanEquals m)
        {
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("push a 1 if carry flag or z flag set");
        }

        public void Visit(LessThanEquals m)
        {
            Console.WriteLine("pop stack into a");
            Console.WriteLine("pop stack into b");
            Console.WriteLine("compare a,b");
            Console.WriteLine("push a 1 if carry clear");
        }


        public void Visit(NotEquals m)
        {
            Console.WriteLine("Pop stack into a");
            Console.WriteLine("Pop stack into b");
            Console.WriteLine("compare a,b");
            Console.WriteLine("push ! zero flag onto stack");
        }

        public void Visit(VarAssignment va)
        {
            Console.WriteLine("pop stack into a ");
            Console.WriteLine("ld " + va.VarName + ", a");
        }

        public void Visit(AttrAssignment va)
        {
            //id,attr,and val are on stack
            Console.WriteLine("call set_obj_attr");
        }

        public void Visit(PropAssignment va)
        {
            //id,attr,and val are on stack
            Console.WriteLine("call set_obj_prop");
        }

        public void Visit(PropertyRVal prv)
        {
            Console.WriteLine("call get_obj_prop ; returns val in a");
            Console.WriteLine("push a onto stack");
        }

        public void Visit(AttributeRVal prv)
        {
            Console.WriteLine("call get_obj_attr ; returns val in a");
            Console.WriteLine("push a onto stack");
        }

        public void Visit(Jump jmp, string lbl)
        {
            Console.WriteLine("jump " + lbl);
        }

        public void Visit(Label lbl)
        {
            Console.WriteLine("@ " + lbl.Text);
        }

        public void Visit(Move mv)
        {
            Console.WriteLine("call move_sub ");
        }


        public void Visit(Look lbl)
        {
            Console.WriteLine("call look_sub");
        }

        public void Visit(Function f)
        {
            Console.WriteLine("function:" + f.name);
            f.body.Accept(this);
            Console.WriteLine("return from sub");
        }


        public void Visit(Call c)
        {

        }

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }

        public void WriteSubName(string text)
        {
            Console.WriteLine("text subroutine");
        }

        public void WriteReturn()
        {
            Console.WriteLine("return from sub");
        }

        public void WriteEventCall(string evtName)
        {
            Console.WriteLine("jsr " + evtName);
        }

        public string GetNextLabel() { return "@label" + labelNum; }

        public void SaveRegs() { }
        public void RestoreRegs() { }
    }
}
