using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IGame;
using System.Text.RegularExpressions;

namespace CLL
{
    partial class CodeParser
    {
        IGame.IGameXml game;
        List<string> attributes = new List<string>();
        List<string> properties = new List<string>();

        public CodeParser()
        {
            SetupTables();
        }

        public CodeParser(IGame.IGameXml game)
        {
            this.game = game;
            SetupTables();
        }


        void SetupTables()
        {
            attributes.Add("id");
            attributes.Add("holder");
            attributes.Add("initial_description");
            attributes.Add("initialdescription");
            attributes.Add("description");
            attributes.Add("n");
            attributes.Add("s");
            attributes.Add("e");
            attributes.Add("w");
            attributes.Add("ne");
            attributes.Add("se");
            attributes.Add("sw");
            attributes.Add("nw");
            attributes.Add("u");
            attributes.Add("up");
            attributes.Add("down");
            attributes.Add("d");
            attributes.Add("in");
            attributes.Add("out");
            attributes.Add("mass");

            properties.Add("scenery");
            properties.Add("supporter");
            properties.Add("container");
            properties.Add("transparent");
            properties.Add("openable");
            properties.Add("open");
            properties.Add("lockable");
            properties.Add("locked");
            properties.Add("portable");
            properties.Add("backdrop");
            properties.Add("wearable");
            properties.Add("beingworn");
            properties.Add("user1");
            properties.Add("lit");
            properties.Add("emittinglight");
            properties.Add("door");
            properties.Add("user2");
            properties.Add("user3");
        }

        /// <summary>
        /// Public function for creating a function
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Body ParseFunction(StringBuilder code)
        {
            Body b = new Body();

            //remove comments
            string noComments = Preprocess(code.ToString());
          //  string noComments = Regex.Replace(code.ToString(), "//.*\r\n", "");

            noComments = PreprocessFailStrings(noComments);

            code.Clear();
            code.Append(noComments);

            //check if string is empty
            if (code.ToString().Trim().Length > 0)
            {
                ParseCode(b, code);
            }

            return b;
        }

        protected void ParseCode(Body parent, StringBuilder code)
        {
            try
            {
                code = new StringBuilder(code.ToString().Trim());

                UnwrapCurlys(code);

                while (code.Length > 0)
                {
                    if (StartsWithIf(code))
                    {
                        StringBuilder condition = GetCondition(code);
                        StringBuilder body = GetBody(code);
                        IfStatement ifs = new IfStatement();
                        ifs.Condition = BuildExprTree(condition);
                        ParseCode(ifs.Body, body);
                        parent.Append(ifs);
                    }
                    else if (StartsWithElseIf(code))
                    {
                        IfStatement ifs = new IfStatement();
                        StringBuilder condition = GetCondition(code);
                        StringBuilder body = GetBody(code);
                        ifs.Condition = BuildExprTree(condition);
                        ParseCode(ifs.Body, body);
                        parent.AppendElseIf(ifs);
                    }
                    else if (StartsWithElse(code))
                    {
                        Body elseBody = new Body();
                        StringBuilder body = GetBody(code);
                        ParseCode(elseBody, body);
                        parent.AppendElse(elseBody);
                    }
                    else if (StartsWithWhile(code))
                    {
                        StringBuilder condition = GetCondition(code);
                        StringBuilder body = GetBody(code);
                        WhileLoop lp = new WhileLoop();
                        lp.Condition = BuildExprTree(condition);
                        ParseCode(lp.Body, body);
                        parent.Append(lp);
                    }
                    else
                    { //must be a statement 

                        StringBuilder statement = ReadToSemicolon(code);

                        //preprocess statements like += 
                        int operIndex = 0;
                        if (IsMathAssignment(statement.ToString(), ref operIndex))
                        {
                            string rewrite = ReWriteMathAssignment(statement.ToString(), operIndex);
                            statement.Clear();
                            statement.Insert(0, rewrite);
                        }
                        else if (IsIncrementOrDecrement(statement.ToString(), ref operIndex))
                        {
                            string rewrite = ReWriteIncrementDecrement(statement.ToString(), operIndex);
                            statement.Clear();
                            statement.Insert(0, rewrite);
                        } 

                        if (IsPropOrAttrAssignment(statement))
                        {
                            string attrName = ExtractAttrName(statement);
                            if (IsAttr(attrName))
                            {
                                parent.Append(CreateAttrAssignment(statement));
                            }
                            else if (IsProp(attrName))
                            {
                                parent.Append(CreatePropAssignment(statement));
                            }
                            else
                                throw new Exception(attrName + " is not a property or attribute (parsing: " + statement + ")");
                        }
                        else if (statement.ToString().StartsWith("println"))
                        {
                            parent.Append(new PrintLn(statement));
                        }
                        else if(statement.ToString().StartsWith("newline"))
                        {
                            parent.Append(new NewLn());
                        }
                        else if (statement.ToString().StartsWith("printvar"))
                        {
                            string inner = ReadToParen(statement);
                            StringBuilder sb = GetCondition(new StringBuilder(inner));
                            parent.Append(new PrintVar(sb));
                        }
                        else if (statement.ToString().StartsWith("print_name") ||
                            statement.ToString().StartsWith("printname") ||
                            statement.ToString().StartsWith("printobjname") 
                            )
                        {
                            string inner = ReadToParen(statement);
                            StringBuilder sb = GetCondition(new StringBuilder(inner));
                            parent.Append(new PrintObjectName(ToIIntResult(sb.ToString())));
                        }
                        else if (IsVarAssignment(statement))
                        {
                            parent.Append(CreateVarAssignment(statement));
                        }
                        else if (statement.ToString().StartsWith("look"))
                        {
                            parent.Append(new Look());
                        }
                        else if (statement.ToString().StartsWith("move"))
                        {
                            parent.Append(new Move());
                        }
                        else if (statement.ToString().StartsWith("ask"))
                        {
                            parent.Append(new Ask());
                         }
                        else if (statement.ToString().StartsWith("print"))
                        { //do after all other prints
                            parent.Append(new Print(statement.ToString()));
                        }
                        else if (statement.ToString().StartsWith("call "))
                        {
                            throw new Exception("Don't use 'call'.  Use C calling syntax instead.");
                        }
                        else if (StartsWithFunctionName(statement))
                        {
                            Call c = new Call("call " + GetFunctionName(statement));

                            parent.Append(c);
                        }
                        else if (statement.ToString().StartsWith("//"))
                        {
                            //do nothing, just throw the comment away
                        }
                        else
                        {
                            throw new Exception("Couldn't figure out what to do with: " + statement);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing code near " + code.ToString(), ex);
            }
        }


        /// <summary>
        /// Reads until a ; or space - + < >
        /// The leading operand is returned, and code in truncated 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        string ReadOperand(StringBuilder code)
        {
            char[] toks = { ' ', ';', '+', '-', '<', '>', '|', '&', '=', '!', '{', '}', '(', ')' };

            string s = code.ToString().Trim();
            int index = s.IndexOfAny(toks);
            if (index == -1)
            {//whole thing is a token
                code.Clear();
                return s;
            }
            else
            {
                string operand = s.Substring(0, index).Trim();
                string remainder = s.Substring(index + 1).Trim();
                code.Clear();
                code.Insert(0, remainder);
                return operand;
            }

        }

        string ReadLogicalOperator(StringBuilder expr)
        {
            throw new Exception("Not implemented");
        }

        bool IsAttr(string s)
        {
            return attributes.Contains(s);
        }

        bool IsProp(string s)
        {
            return properties.Contains(s);
        }

        /// <summary>
        /// Returns true if the code starts with "if ("
        /// if it does, the if statement is chopped off
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool StartsWithIf(StringBuilder code)
        {
            string s = code.ToString().Trim();
            s = s.Replace("" + Convert.ToChar(8301), "");
            int index = s.IndexOf("if");
            char ch = s[0];
            if (s.IndexOf("if") == 0)
            {
                s = s.Substring(2).Trim();
                if (s.IndexOf('(') == 0)
                {
                    code.Clear();
                    code.Insert(0, s);
                    return true;
                }
            }
            return false;
        }

        bool StartsWithWhile(StringBuilder code)
        {
            string s = code.ToString().Trim();
            s = s.Replace("" + Convert.ToChar(8301), "");
            int index = s.IndexOf("while");
            char ch = s[0];
            if (s.IndexOf("while") == 0)
            {
                s = s.Substring(5).Trim(); //5 = len(while)
                if (s.IndexOf('(') == 0)
                {
                    code.Clear();
                    code.Insert(0, s);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the body enclosed in { }
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StringBuilder GetBody(StringBuilder code)
        {
            string s = code.ToString().Trim();
            if (s[0] == '{')
            {
                StringBuilder body = ReadToMatchingCurly(code);
                return body;
            }
            else
            {
                //read to ; 
                code.Clear();
                code.Insert(0, s);
                StringBuilder body = ReadToSemicolon(code);
                body.Append(";");
                return body;
            }
        }

        public VarAssignment CreateVarAssignment(StringBuilder code)
        {
            string s = code.ToString().Trim();
            string name = GetOperand(code);

            if (!game.IsVariable(name))
            {
                throw new Exception(s + " is not a valid variable");
            }

            s = code.ToString().Trim();
            if (s[0] != '=')
                throw new Exception("expected = near " + s);

            s = s.Substring(1).Trim();

            // if (!Char.IsLetter(s[0]))
            //     throw new Exception("unexpected symbol near " + s);

            IIntResult rhs = BuildExprTree(new StringBuilder(s));

            VarAssignment v = new VarAssignment()
            {
                VarName = name,
                Right = rhs
            };

            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        AttrAssignment CreateAttrAssignment(StringBuilder code)
        {
            int ix = code.ToString().IndexOf("=");

            if (ix == -1)
                throw new Exception("Invalid attr assignment:" + code);

            string left = code.ToString().Substring(0, ix).Trim();
            string right = code.ToString().Substring(ix + 1).Trim();

            string[] toks = left.Split('.');
            string obj = toks[0].Trim();
            string attr = toks[1].Trim();
            IIntResult lhs = ToIIntResult(obj);
            IIntResult rhs = BuildExprTree(new StringBuilder(right));


            //look up the lhs by name and set its value
            //            int objid = game.GetObjectId(obj);

            //          if (objid == -1)
            //              throw new Exception("game doesn't have an object named " + obj + " (in GetObjAttr)");

            return new AttrAssignment()
            {
                objName = obj,
                attrName = attr,
                Left = lhs,
                Right = rhs
            };
        }

        PropAssignment CreatePropAssignment(StringBuilder code)
        {
            int ix = code.ToString().IndexOf("=");

            if (ix == -1)
                throw new Exception("Invalid prop assignment:" + code);

            string left = code.ToString().Substring(0, ix).Trim();
            string right = code.ToString().Substring(ix + 1).Trim();

            string[] toks = left.Split('.');
            string obj = toks[0].Trim();
            string attr = toks[1].Trim();

            if (game.GetObjectId(obj) == -1 && !game.IsVariable(obj))
            {
                throw new Exception("game has no object or variable named " + obj);
            }
            IIntResult lhs = ToIIntResult(obj);
            IIntResult rhs = BuildExprTree(new StringBuilder(right));

            return new PropAssignment()
            {
                objName = obj,
                propName = attr,
                Left = lhs,
                Right = rhs
            };
        }


        /*
        /// <summary>
        /// Returns a statement preincrementing the variable in code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Statement CreatePreincrement(StringBuilder code)
        {
            string s = code.ToString().Substring(2).Trim();

            if (s.IndexOf('.') != -1)
                throw new Exception("++/-- can only be applied to variables");

            if (!game.IsVar(s))
            {
                throw new Exception(s + " is not a valid variable");
            }

            VariableRVal rv = new VariableRVal(s);
            Constant one = new Constant(1);
            Plus pl = new Plus();
            pl.Left = rv;
            pl.Right = one;

            VarAssignment v = new VarAssignment()
            {
                VarName = s,
                Right = one
            };
            return v;
        }

        /// <summary>
        /// Returns a statement predecrementing the variable in code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>

        Statement CreatePredecrement(StringBuilder code)
        {
            string s = code.ToString().Substring(2).Trim();

            if (s.IndexOf('.') != -1)
                throw new Exception("++/-- can only be applied to variables");

            if (!game.IsVar(s))
            {
                throw new Exception(s + " is not a valid variable");
            }

            VariableRVal rv = new VariableRVal(s);
            Constant one = new Constant(1);
            Minus pl = new Minus();
            pl.Left = rv;
            pl.Right = one;

            VarAssignment v = new VarAssignment() {
                VarName = s,
                Right = one
            };
            return v;
        }
        */
        /// <summary>
        /// Returns true if the code starts with else/if
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool StartsWithElseIf(StringBuilder code)
        {

            if (code.ToString().IndexOf("else") == 0)
            {
                //trim off else
                string temp = code.ToString().Substring(4).Trim();


                //is there an else
                if (temp.IndexOf("if") == 0)
                {
                    temp = temp.Substring(2).Trim();
                    code.Clear();
                    code.Insert(0, temp);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns true if code starts with 'else'
        /// If true, 'else' is chopped off
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool StartsWithElse(StringBuilder code)
        {
            if (code.ToString().IndexOf("else") == 0)
            {
                //trim off else
                string temp = code.ToString().Substring(4).Trim();
                code.Clear();
                code.Insert(0, temp);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsVarAssignment(StringBuilder code)
        {
            string s = code.ToString().Trim();
            int index = s.IndexOf('=');
            if (index == -1) return false;
            string name = s.Substring(0, index);
            return true;
        }

        bool IsPropOrAttrAssignment(StringBuilder code)
        {
            string s = code.ToString();
            int ei = s.IndexOf('='); //eq index
            int di = s.IndexOf('.'); //doe index

            if (di == -1)
                return false;

            if (ei == -1) return false;
            return (di < ei); //. found before =
        }

        /// <summary>
        /// buffer not changed
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        string ExtractAttrName(StringBuilder code)
        {
            string s = code.ToString();
            string[] toks = s.Split('.');
            string[] toks2 = toks[1].Split('=');
            string attrName = toks2[0].Trim();
            return attrName;
        }

        /// <summary>
        /// Unwraps one or more layers of curly braces
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        StringBuilder UnwrapCurlys(StringBuilder code)
        {

            if (code.Length == 0)
            {
                throw new Exception("Empty function body.  This usually happens when you have a semicolon right after an if-expression");
            }

            while (code[0] == '{')
            {
                code = ReadToMatchingCurly(code);
            }

            return code;
        }


        bool StartsWithFunctionName(StringBuilder s)
        {
            int p = s.ToString().IndexOf("(");
            if (p == -1)
                return false;

            string name = s.ToString().Substring(0, p).Trim();

            bool yn = game.IsFunction(name + "_sub");
            return yn;
        }

        /// <summary>
        /// Trims off ()
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        string GetFunctionName(StringBuilder s)
        {
            int p = s.ToString().IndexOf("(");
            if (p == -1)
                return s.ToString();

            string name = s.ToString().Substring(0, p).Trim();
            return name;
        }


        /// <summary>
        /// Returns true if the statement contains a
        /// math assignment operator such as += or -= 
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        bool IsMathAssignment(string statement, ref int operIndex)
        {
            operIndex = statement.IndexOf("+=");
            if (operIndex != -1)
                return true;

            operIndex = statement.IndexOf("-=");
            if (operIndex != -1)
                return true;

            operIndex = statement.IndexOf("*=");
            if (operIndex != -1)
                return true;

            operIndex = statement.IndexOf("/=");
            if (operIndex != -1)
                return true;

            return false;
        }

        /// <summary>
        /// Returns true if the statement is in the form {lval}++ or {lval}--
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="operIndex"></param>
        /// <returns></returns>
        bool IsIncrementOrDecrement(string statement, ref int operIndex)
        {

            string[] fs = { "print","look","move","newline" };
            foreach (var s in fs)
            {
                if (statement.StartsWith(s))
                    return false;
            }

            char[] ops = { '=', '+', '-', '*', '/', '%' };

            int index = statement.IndexOfAny(ops);

            if (index == -1)//can't be ++ or --;
                return false;

            operIndex = index;

            if (statement.IndexOf("++") == index)
            {
                return true;
            }
            else if (statement.IndexOf("--") == index)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts Math + Assignment into the long form
        /// Converts x+=5 ; into x = x + 5;
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        string ReWriteMathAssignment(string statement, int operIndex)
        {
            string lhs = statement.Substring(0, operIndex);
            string rhs = statement.Substring(operIndex + 2);
            return lhs + "=" + lhs + statement[operIndex] + rhs;
        }

        string ReWriteIncrementDecrement(string statement, int operIndex)
        {
            string lhs = statement.Substring(0, operIndex);

            string op = statement[operIndex].ToString();

            string result = lhs + "=" + lhs + op + "1;";

            return result;
        }

        string PreprocessFailStrings(string code)
        {
            string regex = "fail(\\s)*\\((\\s)*\"[^\"]*[\"](\\s)*\\)";

            MatchCollection matches = Regex.Matches(code, regex);

            foreach (Match m in matches)
            {
                string s = m.Value;
                //trim off the function call
                s = s.Substring(s.IndexOf("\"")+1);
                s = s.Substring(0, s.IndexOf("\""));
                int id = game.GetFailStringId(s);

                code = code.Replace(m.Value, id.ToString());

            }
            return code;
        }

        /// <summary>
        //Removes comments and replaces fail string with the string ids
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string Preprocess(string code)
        {
            string noComments = Regex.Replace(code.ToString(), "//.*\r\n", "");

            noComments = PreprocessFailStrings(noComments);

            noComments = Regex.Replace(noComments, "println(\\s)*\\(\"(\\s)*\"\\)", "newline()");

            return noComments;
        }

    }
}
