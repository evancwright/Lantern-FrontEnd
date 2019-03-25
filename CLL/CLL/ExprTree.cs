using System;
using System.Collections.Generic;
using System.Linq;
using IGame;
using System.Text;
using System.Threading.Tasks;

namespace CLL
{ 
    public interface IIntResult
    {
        int Eval(IGame.IGame game);
        void Accept(IVisitor v);
    }
     
    public abstract class BinaryOperator : IIntResult
    {

        public const int PRECEDENCE_PLUSEQ_MINUSEQ = 6;
        public const int PRECEDENCE_OR = 7;
        public const int PRECEDENCE_AND = 8;
        public const int PRECEDENCE_EQ_NE = 9;
        public const int PRECEDENCE_LT_GT_LTE_GTE = 10;
        public const int PRECEDENCE_PLUS_MINUS = 11;
        public const int PRECEDENCE_MULT = 12;
        public const int PRECEDENCE_NOT = 20;
        public const int PRECEDENCE_PREINC = 20;
        public IIntResult Left;
        public IIntResult Right;
        public int precedence;
        public virtual int Precedence { get { return precedence; } }
        public abstract int Eval(IGame.IGame game);
        public abstract void Accept(IVisitor v);

        //public BinaryOperator() { precedence = 0; }
       public BinaryOperator(int precedence) { this.precedence = precedence; }
    }

    /// <summary>
    /// base class for assignments and function calls 
    /// </summary>
    public abstract class Statement
    {
        public abstract void Execute(IGame.IGame game);
        public abstract void Accept(IVisitor v);
    }

    public class Body
    {
        public Label endBody = new Label();
        List<Statement> statements = new List<Statement>();

        /// <summary>
        /// Appends a statement the body
        /// </summary>
        /// <param name="n"></param>
        public void Append(Statement n) { statements.Add(n); }


        /// <summary>
        /// Adds an else if onto an if or else if
        /// </summary>
        /// <param name="newIf"></param>
        public void AppendElseIf(IfStatement newIf)
        {
            //recurse to the last node
            Statement s = statements.Last();

            if (!(s is IfStatement))
            {
                throw new Exception("misplaced else.");
            }
            IfStatement ifs = s as IfStatement;

            ifs.elseifs.Add(newIf);

        }

        public void AppendElse(Body elseBody)
        {
            //loop to the last node
            Statement s = statements.Last();

            if (!(s is IfStatement))
            {
                throw new Exception("misplaced else.");
            }
            IfStatement ifs = s as IfStatement;
            if (ifs.Else != null)
            {
                throw new Exception("If already has an else.");
            }
            ifs.Else = elseBody;
        }

        public void Accept(IVisitor v)
        {
            foreach (Statement s in statements)
            {
                s.Accept(v);
            }
        }

        /// <summary>
        /// Returns the last Statemet in the list of statements
        /// This is needed to make sure 'else' nodes are not attached
        /// to statements which aren't ifs or if elses.
        /// </summary>
        /// <returns></returns>
        public Statement GetLastStatement()
        {
            if (statements.Count > 0)
                return statements.Last();
            return null;
        }

        /// <summary>
        /// Runs all the statements in the body
        /// </summary>
        public void Execute(IGame.IGame game)
        {
            foreach (var s in statements)
            {
                s.Execute(game);
            }
        }

    }

    public class And : BinaryOperator
    {
        public And() : base(precedence: PRECEDENCE_AND)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            if (Left.Eval(game) != 0 && Right.Eval(game) != 0)
                return 1;
            return 0;

        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }

    public class Or : BinaryOperator
    {
        public Or() : base(precedence: PRECEDENCE_OR)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            if (Left.Eval(game) != 0 || Right.Eval(game) != 0)
                return 1;
            return 0;
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }
    /*
    public class Not : IIntResult
    {

        public Not() : base(precedence:15)
        {
        }

        public IIntResult Child { get; set; }

        public int Eval() {
            if (Child.Eval() == 0)
                return 1;
            else
                return 0;
        }

        public void Accept(IVisitor v)
        {
            Child.Accept(v);
            v.Visit(this);
        }
    }
    */
    public class LessThan : BinaryOperator
    {
        public LessThan() : base(precedence: PRECEDENCE_LT_GT_LTE_GTE)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            if (Left.Eval(game) < Right.Eval(game))
                return 1;
            return 0;
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }

    public class GreaterThan : BinaryOperator
    {
        public GreaterThan() : base(precedence: PRECEDENCE_LT_GT_LTE_GTE)
        {
        }
        public override int Eval(IGame.IGame game)
        {
            if (Left.Eval(game) > Right.Eval(game))
                return 1;
            return 0;
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }

    public class LessThanEquals : BinaryOperator
    {
        public LessThanEquals() : base(precedence: PRECEDENCE_LT_GT_LTE_GTE)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            if (Left.Eval(game) <= Right.Eval(game))
                return 1;
            return 0;
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }

    public class GreaterThanEquals : BinaryOperator
    {
        public GreaterThanEquals() : base(precedence: PRECEDENCE_LT_GT_LTE_GTE)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            if (Left.Eval(game) >= Right.Eval(game))
                return 1;
            return 0;
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }

    public class Constant : IIntResult
    {
        private int val = 0;
        public int Value { get { return val; } set { val = value; } }

        public Constant(int val) { this.val = val; }
        public int Eval(IGame.IGame game) { return val; }
        public void Accept(IVisitor v) { v.Visit(this); }
    }

    public class StringLiteral : IIntResult
    {
        public string s { get; set; }

        public StringLiteral(string str) { this.s = str; }
        public int Eval(IGame.IGame game)
        {
//            return 0; //look up string in literal table
            return game.GetStringId(s);
        }
        public void Accept(IVisitor v) { v.Visit(this); }
    }

    public class VariableRVal : IIntResult
    {
        public string VarName;

        public VariableRVal(string name)
        {
            VarName = name;
        }

        public int Eval(IGame.IGame game)
        {
            //look up the variable name and return it
            return game.GetVarVal(VarName);
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }

    public class PropertyRVal : IIntResult
    {
        public string ObjName { get; set; }
        public string PropName { get; set; }
        public IIntResult Left { get; set; }

        public PropertyRVal(string name, IIntResult lhs, string property)
        {
            Left = lhs;
            this.ObjName = name;
            this.PropName = property;
        }

        public int Eval(IGame.IGame game)
        {
            //look up the variable name and return it
            
            int objId = Left.Eval(game);
            /*
            if (objId == -1)
                throw new Exception("game doesn't have an object named objName (in GetObjProp)");

            */
            return game.GetObjectProp(objId, PropName);
        }

        public void Accept(IVisitor v)
        {
            Left.Accept(v); //push obj id
            v.Visit(this); //write call
        }
    }

    public class AttributeRVal : IIntResult
    {
        public string ObjName { get; set; }
        public string AttrName { get; set; }
        public IIntResult Left { get; set; }

        public AttributeRVal(string name, IIntResult lhs, string attr)
        {
            this.Left = lhs;
            this.ObjName = name;
            this.AttrName = attr;
        }

        public int Eval(IGame.IGame game)
        {
            //look up the variable name and return it
            //return game.GetProperty(name, prop);
            //look up the variable name and return it

            int objId = Left.Eval(game);

//            int objId = game.GetObjectId(objNum);

  //          if (objId == -1)
  //              throw new Exception("game doesn't have an object named objName (in GetObjAttr)");

            return game.GetObjectAttr(objId, AttrName);

        }

        public void Accept(IVisitor v) {
            Left.Accept(v); //pull obj id
            v.Visit(this); //write call
        }
    }

    public class VarAssignment : Statement
    {
        public string VarName { get; set; }
        public IIntResult Right { get; set; }

        public override void Execute(IGame.IGame game)
        {
            int value = Right.Eval(game);

            //look up the lhs by name and set its value
            game.SetVar(VarName, value);
        }

        public override void Accept(IVisitor v)
        {
            Right.Accept(v); //push value on stack
            v.Visit(this); //assign it

        }
    }

    public class AttrAssignment : Statement
    {
        public string objName { get; set; }
        public string attrName { get; set; }
        public IIntResult Left { get; set; }
        public IIntResult Right { get; set; }

        public AttrAssignment()
        {

        }

        public AttrAssignment(string obj, string attr, IIntResult lhs, IIntResult rhs)
        {
            objName = obj;
            attrName = attr;
            Left = lhs;
            Right = rhs;
        }

        public override void Execute(IGame.IGame game)
        {
            int objId = Left.Eval(game);
            int value = Right.Eval(game);

            //look up the lhs by name and set its value
//            if (objid == -1)
//                throw new Exception("game doesn't have an object named objName (in GetObjAttr)");

            game.SetObjectAttr(objId, attrName, value);

        }

        public override void Accept(IVisitor v)
        {
            Console.WriteLine("pushing rhs for attr assignment");
            Left.Accept(v);
            Right.Accept(v);

            v.Visit(this); //assign it
        }
    }

    public class PropAssignment : Statement
    {
        public string objName { get; set; }
        public string propName { get; set; }
        public IIntResult Left { get; set; }
        public IIntResult Right { get; set; }

        public PropAssignment()
        {

        }
        public PropAssignment(string name, string  attr, IIntResult lhs, IIntResult rhs)
        {
            objName = name;
            propName = attr;
            Left = lhs;
            Right = rhs;
        }

        public override void Execute(IGame.IGame game)
        {
            int objId = Left.Eval(game);
            int value = Right.Eval(game);
            
            //look up the lhs by name and set its value
//            int objId = game.GetObjectId(objName);

  //          if (objId == -1)
  ///              throw new Exception("game doesn't have an object named objName (in PropAssignment)");

            game.SetObjectAttr(objId, propName, value);
        }

        public override void Accept(IVisitor v)
        {
            //push object id
            //push prop id   
            Left.Accept(v);
            Right.Accept(v); //push value on stack
            v.Visit(this); //assign it

        }
    }

    /// <summary>
    /// Represents a print statement
    /// </summary>
    public class Print : Statement
    {
        public string text;

        public Print(string statement)
        {
            //lookup the id number for the text
            try
            {
                string inner = statement;
                int start = inner.ToString().IndexOf("\"");
                string rem = inner.ToString().Substring(start + 1);
                int end = rem.IndexOf("\"");
                text = rem.Substring(0, end);
            }
            catch
            {
                throw new Exception("Unable to parse print statement:" + statement);
            }
        }

        public override void Accept(IVisitor v)
        {
           // text.Accept(v); //push arg onto the stack
            v.Visit(this); //pop and print
        }

        public override void Execute(IGame.IGame game)
        {
            //write text to console
            //int index = text.Eval();
            game.PrintString(game.GetStringId(text));
        }
    }


    /// <summary>
    /// Represents a print statement
    /// </summary>
    public class PrintLn : Statement
    {
        public string text;

        public PrintLn(StringBuilder inner)
        {
            //lookup the id number for the text
            try
            {
                int start = inner.ToString().IndexOf("\"");
                string rem = inner.ToString().Substring(start + 1);
                int end = rem.IndexOf("\"");
                text = rem.Substring(0, end);
            }
            catch
            {
                throw new Exception("Unable to parse println statement:" + inner);
            }
        }

        public override void Accept(IVisitor v)
        {
            v.Visit(this); //pop and print
        }

        public override void Execute(IGame.IGame game)
        {
            //write text to console
            game.PrintStringCr(game.GetStringId(text));
            
        }
    }

    /// <summary>
    /// Represents a print statement
    /// </summary>
    public class PrintObjectName : Statement
    {
        public IIntResult objectId { get; set; }
        
        public PrintObjectName(IIntResult inner)
        {
            objectId = inner;
        }

        public override void Accept(IVisitor v)
        {
            objectId.Accept(v);
            v.Visit(this);
        }

        public override void Execute(IGame.IGame game)
        {
            //write text to console
            int id = objectId.Eval(game);

            game.PrintObjectName(id);

        }
    }

    public class Rand : IIntResult
    {
        IIntResult upper;
        static Random r = new Random();
        public Rand(IIntResult inner)
        {   //get the inner code
            upper = inner;
        }

        public void Accept(IVisitor v)
        {
            upper.Accept(v); //push arg onto the stack
            v.Visit(this); //pop and randmod it
        }

        public int Eval(IGame.IGame game)
        {
            return r.Next(upper.Eval(game));
        }

        public int Execute(IGame.IGame game)
        {
            return r.Next(upper.Eval(game));
        }
    }


    public class Has : BinaryOperator
    {
 
        public Has() : base(PRECEDENCE_AND) { }
         

        public override void Accept(IVisitor v)
        {
            Left.Accept(v); //push arg onto the stack
            Right.Accept(v); //push arg onto the stack
            v.Visit(this); //pop,check if player has it,push result
        }

        public override int Eval(IGame.IGame game)
        {
            if (game.ObjectHas(Left.Eval(game), Right.Eval(game)))
                return 1;
            return 0;
        }
    }

    public class Compare : BinaryOperator
    {
        public Compare() : base(precedence: PRECEDENCE_EQ_NE)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            if (Left.Eval(game) == Right.Eval(game))
                return 1;
            return 0;
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }

    public class NotEquals : BinaryOperator
    {

        public NotEquals() : base(precedence: PRECEDENCE_EQ_NE)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            if (Left.Eval(game) != Right.Eval(game))
                return 1;
            return 0;
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }
    /*
    public class PostDecrement : Node
    {
        public PostDecrement() : base()
        {
            nodeType = "--";
        }
    }

    public class PostIncrement : Node
    {
        public PostIncrement() : base()
        {
            nodeType = "++";
        }
    }
    */


    public class Plus : BinaryOperator
    {
        public Plus() : base(precedence: PRECEDENCE_PLUS_MINUS)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            return Left.Eval(game) + Right.Eval(game);
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }

    public class Mult : BinaryOperator
    {
        public Mult() : base(precedence: PRECEDENCE_MULT)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            return Left.Eval(game) * Right.Eval(game);
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);
            Right.Accept(v);
            v.Visit(this);
        }
    }

    public class Minus : BinaryOperator
    {
        public Minus() : base(precedence: PRECEDENCE_PLUS_MINUS)
        {
        }

        public override int Eval(IGame.IGame game)
        {
            return Left.Eval(game) - Right.Eval(game);
        }

        public override void Accept(IVisitor v)
        {
            Left.Accept(v);  //push left
            Right.Accept(v); //push right
            v.Visit(this); //compare
        }
    }


    public class IfStatement : Statement
    {
        public IIntResult Condition { get; set; }
        public Body Body { get; set; }
        //public IfStatement ElseIf { \get; set; }
        public Body Else { get; set; }
        public Label exitLabel = new Label();
         
        public Jump exitJump = new Jump();
        public Jump skipJump = new Jump();

        public List<IfStatement> elseifs = new List<IfStatement>();

        public IfStatement() : base()
        {
            Body = new Body();
            elseifs = new List<IfStatement>();
        }

        public override void Accept(IVisitor v)
        {
            exitLabel.Text = v.GetNextLabel();
            Body.endBody.Text = v.GetNextLabel();

            Condition.Accept(v); //write code to compute result of expr
            v.Visit(this); //write the test
            v.Visit(skipJump, Body.endBody.Text);
            
            Body.Accept(v);

            //jump out of body
            if (elseifs.Count > 0 || Else != null)
                exitJump.Accept(v, exitLabel.Text);

            v.EndBody();
            Body.endBody.Accept(v);
            //write out all the else-if and supply
            //them with the jump out label
            //foreach (IfStatement i in elseifs)
            for (int i = 0; i < this.elseifs.Count; i++)
            {
                elseifs[i].Body.endBody.Text = v.GetNextLabel();
                
                elseifs[i].Condition.Accept(v); //push condition

                v.Visit(elseifs[i]); //write compare
                 
                v.Visit(skipJump, elseifs[i].Body.endBody.Text);

                elseifs[i].Body.Accept(v);

                //jump out of else-if
                exitJump.Accept(v, exitLabel.Text);

                elseifs[i].Body.endBody.Accept(v);
                v.EndBody();
            }

            if (Else != null)
            {
                v.BeginElse();
                Else.Accept(v);
                v.EndBody();
            }

            //@exit
            exitLabel.Accept(v);
        }

        

        public override void Execute(IGame.IGame game)
        {
            if (Condition.Eval(game) != 0)
            {
                Body.Execute(game);
            }
            else
            {
                //try each else if
                foreach (IfStatement i in elseifs)
                {
                    if (i.Condition.Eval(game) != 0)
                    {
                        i.Body.Execute(game);
                        return;
                    }
                }

                if (Else != null)
                    Else.Execute(game);

            }
        }

    }

    public class Label : Statement
    {
        public string Text = "not set";

        public override void Accept(IVisitor v)
        {
            v.Visit(this);
        }

        public override void Execute(IGame.IGame game)
        {
            //labels don't get executed
        }

    }

    public class Jump : Statement
    {
        public void Accept(IVisitor v, string lbl)
        {
            v.Visit(this, lbl);
        }

        public override void Accept(IVisitor v)
        {
            throw new Exception("Jump::Accept(v) shouldn't be getting called!");
        }

        public override void Execute(IGame.IGame game)
        {
            //jumps don't get executed when interpreting
        }

    }

    public class Move : Statement
    {
        public override void Accept(IVisitor v)
        {
            v.Visit(this);
        }

        public override void Execute(IGame.IGame game)
        {
            game.Move();
        }
    }


    public class Look : Statement
    {
        public override void Accept(IVisitor v)
        {
            v.Visit(this);
        }

        public override void Execute(IGame.IGame game)
        {
            game.Look();
        }
    }

    public class Function
    {
        public string name;
        public Body body;

        public Body get
        {
            get { return body; }
        }

        public Function(string name, Body body)
        {
            this.name = name;
            this.body = body;
        }

        public void Execute(IGame.IGame game)
        {
            body.Execute(game);
        }

        public void Accept(IVisitor v)
        {
            v.WriteSubName(name);
            v.SaveRegs();
            body.Accept(v);
            v.RestoreRegs();
            v.WriteReturn();
        }


        
    }



    public class Call : Statement
    {
        public string name;

        public Call(string name)
        {
            try
            {
                this.name = name.Split(' ')[1].Trim();
            }
            catch (Exception e)
            {
                throw new Exception("Error trying to build a Call statement for " + name);
            }
            
        }

        public override void Execute(IGame.IGame game)
        {
            game.CallFunction(name);
        }

        public override void Accept(IVisitor v)
        {
            v.Visit(this);
         }
    }


    static class StringExt
    {
        static public string ChopFuncName(this string s, char ch)
        {
            int x = s.IndexOf("(");
            string t =  s.Substring(s.IndexOf(')'), s.Length - x);
            return t;
        }

    }

    /// <summary>
    /// public class for creating function for code in XML file
    /// </summary>
    public class FunctionBuilder
    {
        public static Function BuildFunction(IGameXml game, string name, string code)
        {
            CodeParser p = new CodeParser(game);
            Body b = p.ParseFunction(new StringBuilder(code));
            Function f = new Function(name, b);
            return f;
        }
    }



}