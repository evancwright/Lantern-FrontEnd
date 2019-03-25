using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class Body : Statement
    {
        public Statement parent;
        List<Statement> statements = new List<Statement>();

        public Body()
        {
        }

        public Body(string code)
        {
            FunctionBuilder fb = new FunctionBuilder();
            fb.BuildTree(code, this);
        }

        public IfStatement GetLastIf()
        {
            return (IfStatement)statements.Last();
        }

        public override void Execute()
        {
            foreach (Statement n in statements)
            {
                n.Execute();
            }
        }

        public void Add(Statement s)
        {
            statements.Add(s);
        }

        public IfStatement Last()
        {
            return (IfStatement)statements.Last<Statement>();
        }
    }

    class Node
    {
        string text;
        public Statement parent;
        public virtual int Eval() { return 0; }

        public static int GetLhsObj(string lhs)
        {
            int result;
            if (Int32.TryParse(lhs, out result))
            {
                return result;
            }
            else if (lhs.IndexOf(".") != -1)
            {
                Game g = Game.GetInstance();
                string left = lhs.Substring(0, lhs.IndexOf("."));

                if (g.IsVariable(left))
                    return g.GetVarVal(left);
                else
                    return g.GetObjectId(left);
            }
            else
            {
                Game g = Game.GetInstance();
                return g.GetObjectId(lhs);
            }
        }

        public static Node ToNode(string rhs)
        {
            Game g = Game.GetInstance();
            rhs = rhs.Trim();
            int result;
            if (Int32.TryParse(rhs, out result))
            {
                return new Constant(result);
            }
            else if (rhs.ToUpper() == "TRUE")
            {
                return new Constant(1);
            }
            else if (rhs.ToUpper() == "FALSE")
            {
                return new Constant(0);
            }
            else if (rhs.IndexOf('"') == 0)
            {
                
                rhs = rhs.Replace("\"", string.Empty);
                return new Constant(g.GetStringId(rhs));
            }
            else if (rhs.IndexOf('.') != -1)
            {
                string left = rhs.Substring(0, rhs.IndexOf('.'));
                string right = rhs.Substring(rhs.IndexOf('.')+1, rhs.Length - rhs.IndexOf('.')-1);
                return new Attr(left, right);
            }
            else if (g.GetObjectId(rhs) != -1)
            {
                return new Constant(g.GetObjectId(rhs));
            }
            else if (g.IsVariable(rhs))
            {
                return new GameVar(rhs);
            }
            else
                throw new Exception("Don't know what to do with " + rhs);
        }
    }

    class Statement
    {
        public virtual void Execute() { }



    }

    class SetVariable  : Statement
    {
        int val;
        string varName;

        public SetVariable(string var, string val)
        {
            this.varName = var;
            this.val = Convert.ToInt16(val);
        }

        public override void Execute()
        {

        }
    }


    class AddToVariable : Statement
    {
        Node val;
        string varName;

        public AddToVariable(string var, string val)
        {
            this.varName = var;
            this.val = Node.ToNode(val);
            
        }

        public override void Execute()
        {
            Game g = Game.GetInstance();
            int v = val.Eval();
            g.AddVar(varName, v);
            
        }
    }
    class IfStatement : Statement
    {
        public IfStatement(string express, string code)
        {
            expr = ExpressionBuilder.CreateExpr(express);
            body = new Body(code);
            body.parent = this;
        }

        public override void Execute()
        {
            Game g = Game.GetInstance();
            if (expr.Eval())
            {
                g.Debug("condition is true. executing body");
                body.Execute();
            }
            else if (elseNode != null)
            {
                 g.Debug("condition is false. executing else");

                elseNode.Execute();
            }
        }

        public void AddElse()
        {
            elseNode = new Body();
        }

        public Expression expr;
        public Body body;
        public Body elseNode;  //else or else if

    }

    class Expression 
    {
        public Node lhs;
        public Node rhs;
        public Opr op;
        public string text;
    
        public bool Eval()
        {
            Game g = Game.GetInstance();
            g.Debug("Evaluating:" + text);

            if (op.opType == Opr.EQ)
                return (lhs.Eval() == rhs.Eval());
            if (op.opType == Opr.NEQ)
                return (lhs.Eval() != rhs.Eval());
            if (op.opType == Opr.GT)
                return (lhs.Eval() > rhs.Eval());
            if (op.opType == Opr.LT)
                return (lhs.Eval() < rhs.Eval());
            return false;
        }
    }

    class Opr
    {
        public const int EQ=0;
        public const int NEQ = 1;
        public const int GT = 2;
        public const int LT = 3;
        public  int opType;
        public string txt;

        public Opr(int t)
        {
            opType = t;

            if (opType == EQ)
                txt = "==";
            if (opType == NEQ)
                txt = "==";
            if (opType == GT)
                txt = ">";
            if (opType == LT)
                txt = "<";
        }
        
        public Opr(string t)
        {
            if (t == "==")
                opType = EQ;
            if (t == "!=")
                opType = NEQ;

            if (t == ">")
                opType = GT;

            if (t == "<")
                opType = LT;
            txt = t;
        }
    }

    class Constant : Node
    {
        int val;
        public Constant(int v) { val = v; }

        public override  int Eval() 
        {
            Game g = Game.GetInstance();
            g.Debug("Returning constant:" + val);
            return val; 
        }
    }

    class GameVar : Node
    {
        string name;
        public GameVar(string name) { this.name = name; }

        public override int Eval() 
        {
            Game g = Game.GetInstance();
            return g.GetVarVal(name);
        }
    }
   

   
    //works for properties, too
    class SetAttr : Statement
    {
        string obj;
        string attr;
        Node rhs;

        public SetAttr(string obj, string attr, string rhs)
        {
            this.obj = obj;
            this.attr = attr.ToUpper();

            this.rhs = Node.ToNode(rhs);


        }

        public override void Execute()
        {
            Game g = Game.GetInstance();
            int val = rhs.Eval();
            g.Debug("Setting " + obj + "." + attr + " to " + val );
            
            g.SetObjectAttr(Node.GetLhsObj(obj), attr, val);

        }

       

    }


    class Attr : Node
    {
        string lhs;
        string attr;
        int id;


        public Attr(string obj, string attr)
        {
            this.lhs = obj;
            this.attr = attr;
        }

        public override int Eval() { 
            Game g = Game.GetInstance();
            return g.GetObjectAttr(GetLhsObj(lhs),attr.ToUpper());
        }
    }

    class Assignment : Statement
    {

        public Assignment(string lh, string rhs)
        {

        }

        public override void Execute()
        {

        }
    }

    class PrintStatement : Statement
    {
        string msg;

        public PrintStatement(string msg)
        {
            this.msg = msg;
        }

        public override void Execute()
        {
            Game g = Game.GetInstance();
            g.PrintString(msg);
        }

    }

    class PrintlnStatement : Statement
    {

        string msg;

        public PrintlnStatement(string msg)
        {
            this.msg = msg;
        }

        public override void Execute()
        {
            Game g = Game.GetInstance();
            g.PrintStringCr(msg);
        }

    }


    class LookStatement : Statement
    {
        public override void Execute()
        {
            Game g = Game.GetInstance();
            g.Look();
        }
    }

    class MoveStatement : Statement
    {
        public override void Execute()
        {
            Game g = Game.GetInstance();
            g.Move();
        }
    }

}
