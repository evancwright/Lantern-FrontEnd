using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
 

namespace CLL
{

    public class VisitorRPi : AsmCommon, IVisitor
    {
        StreamWriter sw;
        protected int _label = 0;
        protected int labelId = 97;
        IGame.IGameXml game;
        int tabLevel;
        public VisitorRPi(IGame.IGameXml g)
        {
            game = g;
        }

        public void SetStreamWriter(StreamWriter sw)
        {
            this.sw = sw;
        }

        public void Visit(IfStatement ps)
        {
//            Console.WriteLine(Tabs() + "//if statement");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "if (param1 != 0)");
            sw.WriteLine(Tabs() + "{");
            tabLevel++;        

        } 

        /*Same as if statement
         */
        public void Visit(WhileLoop wh)
        {
            //            Console.WriteLine(Tabs() + "//if statement");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "if (param1 != 0)");
            sw.WriteLine(Tabs() + "{");
            tabLevel++;

        }

        public void Visit(Print ps)
        {
            sw.WriteLine(Tabs() + "print_string(" + game.GetStringId(ps.text) + "); /*" + ps.text + "*/");
        
        }

        public void Visit(NewLn ps)
        {
            sw.WriteLine(Tabs() + "print_cr();");
        }

        public void Visit(PrintLn ps)
        {
            sw.WriteLine(Tabs() + "print_string(" + game.GetStringId(ps.text) + "); ");
            sw.WriteLine(Tabs() + "print_cr();");
        }

        public void Visit(PrintObjectName ps)
        {
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "print_obj_name(param1); ");
            sw.WriteLine(Tabs() + "print_cr();");
        }

        public void Visit(PrintVar ps)
        {
            sw.WriteLine(Tabs() + "sprintf(buffer,\"%d\",ps.VarName);");
            sw.WriteLine(Tabs() + "printstr(buffer);");
        }


        public void Visit(Rand r)
        {
            sw.WriteLine(Tabs()+"//rand()");
            sw.WriteLine(Tabs()+" param1 = param_stack_pop();");
            sw.WriteLine(Tabs()+" param1 = rand8(param1)  ; ");
            sw.WriteLine(Tabs()+" param_stack.push(param1) ; ");

        }

        public void Visit(Has ph)
        {
            //pop child
            sw.WriteLine(Tabs()+"; child is on stack");
            sw.WriteLine(Tabs()+"param2 = param_stack_pop()");
            //pop parent
            sw.WriteLine(Tabs()+"; parent is on stack");
            sw.WriteLine(Tabs()+"param1 = param_stack_pop()");
            //call ancestor function
            sw.WriteLine(Tabs()+"param1 = is_child_of(param1,param2);");
            sw.WriteLine(Tabs()+" param_stack_push(param1) ; ");
        }

        public void Visit(VariableRVal v)
        {
            sw.WriteLine(Tabs()+"// variable rval");
            sw.WriteLine(Tabs()+"param1 = " + FixVarName(v.VarName) + ";");
            sw.WriteLine(Tabs()+"param_stack.push(param1);");
        }

        public void Visit(AttributeRVal v)
        {
            //get attr push it on stack
            /*
            ; params are on user stack
            ; top param is obj id
            */

            sw.WriteLine(Tabs()+"// ;getting an attr");

            try
            {
                sw.WriteLine(Tabs()+"param1 = param_stack_pop() ; // get object id");
                sw.WriteLine(Tabs()+"param2 = " + attrIndexes[v.AttrName] + " ; //" + v.AttrName);
                sw.WriteLine(Tabs()+"param1 = get_object_attr(param1, param2) ; ");
                sw.WriteLine(Tabs()+"param_stack.push(param1);");
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
             ; user param_stack. the value will be either 1 or 0
             ; parameters on user stack
             ; 1-object id (top)

             */
            sw.WriteLine(Tabs()+"//getting a property");
            try
            {
                sw.WriteLine(Tabs() + "param1 = param_stack_pop() ; //object id");
                sw.WriteLine(Tabs() + "param2 = " + propIndexes[v.PropName] + " ; //" + v.PropName);
                sw.WriteLine(Tabs() + "param1 =  get_object_prop(param1, param2) ; ");
                sw.WriteLine(Tabs() + "param_stack.push(param1);");
            }
            catch (Exception e)
            {
                throw new Exception(v.PropName + " is not a valid prop for " + v.ObjName, e);
            }
            
         }

        public void Visit(Constant Mc)
        {
            sw.WriteLine(Tabs() + "//constant");
            sw.WriteLine(Tabs() + "param_stack.push(" + Mc.Value + ");");
        }

        public void Visit(StringLiteral sl)
        {
            throw new NotImplementedException();
        }

        public void Visit(And c)
        {
            sw.WriteLine(Tabs() + "//&&");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param2 =  param_stack_pop();");
            sw.WriteLine(Tabs() + "param1 = param1 && param2;");
            sw.WriteLine(Tabs() + "param_stack.push(param1);");
        }
        public void Visit(Or c)
        {
            sw.WriteLine(Tabs() + "//||");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param2 =  param_stack_pop();");
            sw.WriteLine(Tabs() + "param1 = param1 || param2;");
            sw.WriteLine(Tabs() + "param_stack.push(param1);");
        }

        //   void Visit(Not m);
        public void Visit(Compare c)
        {
            sw.WriteLine(Tabs() + "// ==");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param2 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param_stack.push( (short) param1 == param2 );");
        }

        public void Visit(Plus p)
        {
            sw.WriteLine(Tabs() + "//plus");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param2 =  param_stack_pop();");
            sw.WriteLine(Tabs() + "param_stack.push(param1 + param2);");
        }

        public void Visit(Mult p)
        {
            sw.WriteLine(Tabs() + "//*");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param2 =  param_stack_pop();");
            sw.WriteLine(Tabs() + "param_stack.push(param1 * param2);");
        }

        public void Visit(Minus m)
        {
            sw.WriteLine(Tabs() + "//minus");
            sw.WriteLine(Tabs() + "param2 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param1 =  param_stack_pop();");
            sw.WriteLine(Tabs()+"param_stack.push(param1 - param2);");
        }

        public void Visit(GreaterThan m)
        {
            //compare and push 1 if carry and zero are 0
            //zc
            //00 = a > b
            //01 = a < b
            //10 = a == b
            //push a 1 if zc == 0

            sw.WriteLine(Tabs() + "// ;a > b");
            sw.WriteLine(Tabs() + "param2 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param_stack.push((short)(param1 > param2));");
        }

        public void Visit(LessThan m)
        {
            sw.WriteLine(Tabs() + "//a < b");
            sw.WriteLine(Tabs() + "param2 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param_stack.push((short)(param1 < param2));");
        }

        public void Visit(GreaterThanEquals m)
        {
            //push a 1 if c == 0
            sw.WriteLine(Tabs() + "//a >= b");
            sw.WriteLine(Tabs() + "param2 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param_stack.push(param1 >= param2);");
        }

        public void Visit(LessThanEquals m)
        {
            sw.WriteLine(Tabs() + "// <= ");
            sw.WriteLine(Tabs() + "param2 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param_stack.push(param1 <= param2);");
        }

        public void Visit(VarAssignment m)
        {
            sw.WriteLine(Tabs() + "//writing a set var statement");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + m.VarName + " = param1;");
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

            sw.WriteLine(Tabs()+"// ;writing a set attribute statement");

            //            sw.WriteLine(Tabs()+"pop bx ; pull value (rhs)");
            //           sw.WriteLine(Tabs()+"pop ax ; pull obj # (lhs)");
            //            sw.WriteLine(Tabs()+"pshu a ; push obj # param");
            
            try
            {
                //stuff here
                sw.WriteLine(Tabs() + "param3 = param_stack_pop() ; //save value");
                sw.WriteLine(Tabs() + "param1 = param_stack_pop() ; //save obj id");
                sw.WriteLine(Tabs() + "param2 = " + attrIndexes[asn.attrName] + " ; // attr index");
                sw.WriteLine(Tabs() + "set_object_attr(param1,param2,param3);");
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
                sw.WriteLine(Tabs() + "param3 = param_stack_pop() ; //save value");
                sw.WriteLine(Tabs() + "param1 = param_stack_pop() ; //save obj id");
                sw.WriteLine(Tabs() + "param2 = " + propIndexes[m.propName] + " ; //" + m.propName);
                sw.WriteLine(Tabs() + "set_object_prop(param1,param2,param3);");
            }
            catch (Exception e)
            {
                throw new Exception(m.propName + " is not a valid attr for " + m.objName, e);
            }

        }

        public void Visit(NotEquals m)
        {
            sw.WriteLine(Tabs() + "// !=");
            sw.WriteLine(Tabs() + "param1 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param2 = param_stack_pop();");
            sw.WriteLine(Tabs() + "param_stack.push(param1 != param2);");
        }

        public void Visit(Label lbl)
        {
        }

        public void Visit(Jump jmp, string label)
        {
        }

        public void Visit(Move move)
        {
            sw.WriteLine(Tabs() + "//move statement");
            sw.WriteLine(Tabs() + "move_sub();");
        }

        public void Visit(Look look)
        {
            sw.WriteLine(Tabs() + "//look statement");
            sw.WriteLine(Tabs() + "look_sub();");
        }

        public void Visit(Ask ask)
        {
            sw.WriteLine(Tabs() + "ask();");
        }


        public void Visit(Call c)
        {
            string name = c.name;

            if (name.IndexOf('(') != -1)
            {
                name = name.Substring(0, name.IndexOf('(')).Trim();
            }
            sw.WriteLine(Tabs() + name + "_sub();");
        }

        public void Visit(Function f)
        {
            sw.WriteLine("/* machine generated subroutine */");
            sw.WriteLine("");
            sw.WriteLine("void " + f.name + "()");
            sw.WriteLine("{");
            tabLevel = 2;
            f.body.Accept(this);
//            sw.WriteLine(Tabs()+"};");
            sw.WriteLine("}");
        }

        public void WriteLine(string text)
        {
            throw new NotImplementedException();
        }

        public void WriteSubName(string text)
        {
            tabLevel = 1;
            sw.WriteLine("/*" + text + " subroutine  */");
            sw.WriteLine("void " + text + "()");
            sw.WriteLine("{");
        }

        public void WriteReturn()
        {
        //    sw.WriteLine(Tabs()+"};");
            sw.WriteLine("}");
        }

        public void WriteEventCall(string label)
        {
            string name = label;

            if (name.IndexOf('(') != -1)
            {
                name = name.Substring(0, name.IndexOf('(')).Trim();
            }

            sw.WriteLine(Tabs()+"" + label  + "();");
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
        public override void BeginElse()
        {
            sw.WriteLine(Tabs() + "else\r\n" + Tabs() + "{");
            tabLevel++;
        }

        public override void EndBody()
        {
            tabLevel--;
            sw.WriteLine(Tabs() + "} //end body");

        }

        public void WriteElse()
        {
            sw.WriteLine(Tabs() + "else\r\n" + Tabs() + "{ ");
            tabLevel++;
        }

        string Tabs()
        {
            string s = "";
            for (int i = 0; i < tabLevel; i++)
                s += "\t";
            return s;
        }
    }
}
