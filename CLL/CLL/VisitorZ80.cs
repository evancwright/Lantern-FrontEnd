using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CLL
{
    public class VisitorZ80 : AsmCommon, IVisitor
    {

        StreamWriter sw;
        protected int _label = 0;
        protected int labelId = 97;
        IGame.IGameXml game;

        public VisitorZ80(IGame.IGameXml g)
        {
            game = g;
        }


        public void SetStreamWriter(StreamWriter sw)
        {
            this.sw = sw;
        }

        public void Visit(IfStatement ps)
        {
            sw.WriteLine("\tpop af ; pop condition");
            sw.WriteLine("\tcp 0");
            sw.WriteLine("\t;jr nz,3 ; enter if-body");
            sw.WriteLine("\tdb 20h; jrnz");
            sw.WriteLine("\tdb 3; 2");
        }

        public void Visit(Print ps)
        {
            sw.WriteLine("\tld b,"+ game.GetStringId(ps.text)+" ; sub wants arg in b");
            sw.WriteLine("\tld ix,string_table");          
            sw.WriteLine("\tcall print_table_entry");
        }

        public void Visit(PrintLn ps)
        {
            sw.WriteLine("\tld b," + game.GetStringId(ps.text) + " ; sub wants arg in b");
            sw.WriteLine("\tld ix,string_table");
            sw.WriteLine("\tcall print_table_entry");
            sw.WriteLine("\tcall printcr");
        }

        public void Visit(PrintObjectName ps)
        {
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tcall print_obj_name");
        }

        public void Visit(PrintVar ps)
        {
            throw new NotImplementedException("Z80 Print var not implemented.");
        }

        public void Visit(Rand r)
        {
            sw.WriteLine("\t;rand(x)");
            sw.WriteLine("\tpop bc ; modulus -> b");
            sw.WriteLine("\tcall rmod");
            sw.WriteLine("\tpush af");
        }


        public void Visit(Has ph)
        {
            //child is on top of stack
            sw.WriteLine("\tpop af ; pop child");
            //parent is on bottom of stack
            sw.WriteLine("\tpop bc ; pop parent");
            sw.WriteLine("\tld c,a ; child->c");
            sw.WriteLine("\tcall b_ancestor_of_c");
            sw.WriteLine("\tpush af ; result to stack");
        }

        public void Visit(VariableRVal v)
        {
            sw.WriteLine("\t;load var");
            sw.WriteLine("\tld a,(" + game.GetVarAddr(v.VarName) + ")");
            sw.WriteLine("\tpush af");
        }

        public void Visit(AttributeRVal v)
        {
            // returns property c of object b in register a

            sw.WriteLine("\t;attr rval");
            //sw.WriteLine("\tld b," + game.GetObjectId(v.ObjName));
            sw.WriteLine("\tpop bc ; pop obj id"); 
            sw.WriteLine("\tld c," + attrIndexes[v.AttrName] + " ; " + v.AttrName);
            sw.WriteLine("\tcall get_obj_attr ");
            sw.WriteLine("\tpush af");
        }

        public void Visit(PropertyRVal v)
        {
            // returns property c of object b in register a
            // the property should be 0 - 15 inclusive
            sw.WriteLine("\t;get object property");
            sw.WriteLine("\tpop bc ; get obj id");
            // sw.WriteLine("\tld c,b ; sub wants obj in c");
            sw.WriteLine("\tld c," + propIndexes[v.PropName] + " ; " + v.PropName);
            sw.WriteLine("\tcall get_obj_prop");
            sw.WriteLine("\tpush af");
        }

        public void Visit(Constant Mc)
        {
            sw.WriteLine("\tld a," + Mc.Value + " ; constant ");
            sw.WriteLine("\tpush af");
        }

        public void Visit(StringLiteral sl)
        {
            sw.WriteLine("\tld a," + game.GetObjectId(sl.s) + " ; " + sl.s);
            sw.WriteLine("\tpush af");
        }

        public void Visit(And c)
        {
            sw.WriteLine("\t; && statement");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tand a,b");
            sw.WriteLine("\t;jr z,4");
            sw.WriteLine("\tdb 28h ; jr z");
            sw.WriteLine("\tdb 4");
            sw.WriteLine("\tld a,1");
            sw.WriteLine("\t;jr 2");
            sw.WriteLine("\tdb 18h ; jr");
            sw.WriteLine("\tdb 2");
            sw.WriteLine("\tld a,0");
            sw.WriteLine("\tpush af ");
        }


        public void Visit(Or c)
        {
            sw.WriteLine("\t; || statement");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tor a,b");
            sw.WriteLine("\t;jr z,4");
            sw.WriteLine("\tdb 28h ; jr z,");
            sw.WriteLine("\tdb 4");
            sw.WriteLine("\tld a,1");
            sw.WriteLine("\t;jr 2");
            sw.WriteLine("\tdb 18h ; jr");
            sw.WriteLine("\tdb 2");
            sw.WriteLine("\tld a,0");
            sw.WriteLine("\tpush af ");
        }
        //   void Visit(Not m);
        public void Visit(Compare c)
        {
            sw.WriteLine("\t; == statement");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tcp b");
            sw.WriteLine("\t;jr z,4");
            sw.WriteLine("\tdb 28h ; jrz");
            sw.WriteLine("\tdb 4 ; ");
            sw.WriteLine("\tld a,0");
            sw.WriteLine("\t;jr 2");
            sw.WriteLine("\tdb 18h ; jr");
            sw.WriteLine("\tdb 2");
            sw.WriteLine("\tld a,1");
            sw.WriteLine("\tpush af ");
        }
        public void Visit(Plus p)
        {
            sw.WriteLine("\t; plus statement");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tadd a,b");
            sw.WriteLine("\tpush af ");
        }

        public void Visit(Mult p)
        {
            sw.WriteLine("\t; multiply statement");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tld  c,a");
            sw.WriteLine("\tcall bmulc");
            sw.WriteLine("\tld a,c");
            sw.WriteLine("\tpush af");
        }

        public void Visit(Minus m)
        {
            sw.WriteLine("\t; minus statement");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tsub b");
            sw.WriteLine("\tpush af ");
        }

        public void Visit(GreaterThan m)
        {
            //push a 1 if carry = 0 and z == 0 
            sw.WriteLine("\t; a > b");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tcp b");
            sw.WriteLine("\tjr c,6 ; skip to push 0");
            sw.WriteLine("\tdb 38h; jrc");
            sw.WriteLine("\tdb 6;");
            sw.WriteLine("\tld a,1");
            sw.WriteLine("\tjr z,4 ; skip to push 0");
            sw.WriteLine("\tdb 28h; jrz");
            sw.WriteLine("\tdb 4;");
            sw.WriteLine("\tld a,1");
            sw.WriteLine("\t;jr 2");
            sw.WriteLine("\tdb 18h ; jr");
            sw.WriteLine("\tdb  2");
            sw.WriteLine("\tld  a,0");
            sw.WriteLine("\tpush af");
        }

        public void Visit(LessThan m)
        {
            //push a 1 if carry = 1
            sw.WriteLine("\t; a < b");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tcp b");
            sw.WriteLine("\tjr c,4 ; skip to push 0");
            sw.WriteLine("\tld a,0");
            sw.WriteLine("\t;jr 2 ; skip to push af");
            sw.WriteLine("\tdb 18h ; jr");
            sw.WriteLine("\tdb  2");
            sw.WriteLine("\tld  a,1");
            sw.WriteLine("\tpush af");
        }

        public void Visit(GreaterThanEquals m)
        {
            //push a 0 if carry = 1 
            sw.WriteLine("\t; a < b");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tcp b");
            sw.WriteLine("\tjr c,4 ; skip to push 0");
            sw.WriteLine("\tld a,1");
            sw.WriteLine("\t;jr 2 ; skip to push af");
            sw.WriteLine("\tdb 18h ; jr");
            sw.WriteLine("\tdb  2");
            sw.WriteLine("\tld  a,0");
            sw.WriteLine("\tpush af");
        }


        public void Visit(LessThanEquals m)
        {
            //push a 1 if carry = 1 or zero = 1
            sw.WriteLine("\t; a < b (push 1 of c or z set)");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tcp b");
            sw.WriteLine("\tjr z,6 ; skip to push 1");
            sw.WriteLine("\tjr c,4 ; skip to push 1");
            sw.WriteLine("\tld a,0");
            sw.WriteLine("\t;jr 2 ; skip to push af");
            sw.WriteLine("\tdb 18h; jr");
            sw.WriteLine("\tdb 2");
            sw.WriteLine("\tld  a,1");
            sw.WriteLine("\tpush af");
        }
        //void Visit(Assign m);

        public void Visit(VarAssignment m)
        {
            sw.WriteLine("\t; var assignment");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tld (" + m.VarName + "),a");
        }

        public void Visit(AttrAssignment m)
        {
            // set property c of object b to register a
            sw.WriteLine("\tpop af  ; pop value");
            //sw.WriteLine("\tld b," + game.GetObjectId(m.objName));
            sw.WriteLine("\tpop bc ; pop obj id");
            sw.WriteLine("\tld c," + attrIndexes[m.attrName] + " ; " + m.attrName);
            sw.WriteLine("\tcall set_obj_attr");
        }

        public void Visit(PropAssignment m)
        {
            sw.WriteLine("\t;set property");
            /*
            sets property c of object b to val in register 'a'
            the property should be 0 - 15 inclusive
            */
            sw.WriteLine("\tpop af  ; pop value");
            //sw.WriteLine("\tld b," + game.GetObjectId(m.objName));
            sw.WriteLine("\tpop bc  ; object id");
            sw.WriteLine("\tld c," + propIndexes[m.propName] + " ; " + m.propName);
            sw.WriteLine("\tcall set_obj_prop");
        }

        public void Visit(NotEquals m)
        {
            sw.WriteLine("\t; != statement");
            sw.WriteLine("\tpop af");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tcp b");
            sw.WriteLine("\t;jr nz,4");
            sw.WriteLine("\tdb 20h ; jrnz");
            sw.WriteLine("\tdb 4");
            sw.WriteLine("\tld a,0");
            sw.WriteLine("\t;jr 2");
            sw.WriteLine("\tdb 18h; jr");
            sw.WriteLine("\tdb 2");

            sw.WriteLine("\tld a,1");
            sw.WriteLine("\tpush af ");
        }

        public void Visit(Label lbl)
        {
            sw.WriteLine("" + lbl.Text);
        }

        public void Visit(Jump jmp, string label)
        {
            sw.WriteLine("\tjp " + label);
        }

        public void Visit(Move move)
        {
            sw.WriteLine("\tcall move_player ; move command");
        }

        public void Visit(Look look)
        {
            sw.WriteLine("\tcall look_sub ; look command");
        }

        public void Visit(Ask ask)
        {
            sw.WriteLine("\t; ask command");
            sw.WriteLine("\tcall getlin");
            sw.WriteLine("\tld ix,INBUF");
            sw.WriteLine("\tld iy,string_table");
            sw.WriteLine("\tcall get_table_index ; result in b");
            sw.WriteLine("\tld a,b");
            sw.WriteLine("\tld (answer),a");
        }


        public void Visit(Function f)
        {
            sw.WriteLine(f.name + " ; start subroutine");
            sw.WriteLine("*MOD");

            f.body.Accept(this);

            sw.WriteLine("pop iy");
            sw.WriteLine("pop ix");
            sw.WriteLine("pop hl");
            sw.WriteLine("pop de");
            sw.WriteLine("pop bc");
            sw.WriteLine("pop af");

            sw.WriteLine("\tret");
        }

        public void WriteLine(string text)
        {
            throw new NotImplementedException();
        }


        public void WriteSubName(string text)
        {
            sw.WriteLine(text + " ; " + text + " subroutine");
            sw.WriteLine("*MOD");
        }

        public void WriteReturn()
        {
            sw.WriteLine("\tret");
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

            return "$" + s.ToLower() + "?";
        }

        public void Visit(Call c)
        {
            sw.WriteLine("\tcall " + c.name + "_sub");
        }

        public void WriteEventCall(string label)
        {
            sw.WriteLine("\tcall " + label);
        }

        public void SaveRegs()
        {
            sw.WriteLine("\tpush af");
            sw.WriteLine("\tpush bc");
            sw.WriteLine("\tpush de");
            sw.WriteLine("\tpush hl");
            sw.WriteLine("\tpush ix");
            sw.WriteLine("\tpush iy");
        }

        public void RestoreRegs()
        {
            sw.WriteLine("\tpop iy");
            sw.WriteLine("\tpop ix");
            sw.WriteLine("\tpop hl");
            sw.WriteLine("\tpop de");
            sw.WriteLine("\tpop bc");
            sw.WriteLine("\tpop af");
        }

        
    }
}
