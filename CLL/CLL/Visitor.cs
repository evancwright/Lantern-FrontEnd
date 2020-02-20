using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CLL
{ 
    public interface IVisitor
    {
        void SetStreamWriter(StreamWriter sw);
        void Visit(IfStatement ps);
        void Visit(WhileLoop ps);
        void Visit(Print ps);
        void Visit(PrintLn ps);
        void Visit(NewLn ps);
        void Visit(PrintObjectName ps);
        void Visit(PrintVar pv);
        void Visit(Rand r);
        void Visit(Has ph);
        void Visit(Sees ph);
        void Visit(VariableRVal v);
        void Visit(AttributeRVal v);
        void Visit(PropertyRVal v);
        void Visit(Constant Mc);
        void Visit(StringLiteral sl);
        void Visit(And c);
        void Visit(Or c);
     //   void Visit(Not m);
        void Visit(Compare c);
        void Visit(Plus p);
        void Visit(Mult p);
        void Visit(Minus m);
        void Visit(GreaterThan m);
        void Visit(LessThan m);
        void Visit(GreaterThanEquals m);
        void Visit(LessThanEquals m);
        //void Visit(Assign m);
        void Visit(VarAssignment m);
        void Visit(AttrAssignment m);
        void Visit(PropAssignment m);
        void Visit(NotEquals m);
        void Visit(Label lbl);
        void Visit(Jump jmp, string label);
        void Visit(Move move);
        void Visit(Look look);
        void Visit(Function f);
        void Visit(Call c);
        void Visit(Ask c);
        void WriteLine(string text);
        void WriteSubName(string text);
        void WriteReturn();
        void WriteEventCall(string label);
        void BeginElse();
        void EndBody();
        string GetNextLabel();
        void SaveRegs();
        void RestoreRegs();
        
    }
}
