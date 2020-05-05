using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using LangDef;

namespace LASM
{

    public interface IWritable
    {
        void WriteBinary(BinaryWriter bw);
    }


    class BlankLine : Statement
    {

    }

    class Label : Statement
    {
        string Name;

        public Label(string name)
        {
            Name = name;
        }

        public bool IsLocal(string l)
        {
            return l.StartsWith("@");
        }
    }


    class Comment : Statement
    {
        public Comment(string s)
        {
            this.Comment = s;
            Text = s;
        }



    }

    class LocalLabel : Label
    {
        public LocalLabel(string name) : base(name)
        {
        }
    }

    class GlobalLabel : Label
    {
        public GlobalLabel(string name) : base(name)
        {
        }
    }

    class Statement
    {
        public int ModuleNumber { get; set; }
        public int LineNumber { get; set; }
        public string Comment { get; set; }
        public string Text { get; set; }
        public string HexText { get; set; } = "";
        public string Label { get; set; } = "";
        public ushort Address { get; set; } //how many bytes into program code

        public virtual ushort GetBinaryLength()
        {
            return 0;
        }

        public virtual string AsHextText()
        {
            return "";
        }

        public void TrimTrailingComment()
        {
            //HACK - this will bomb if two ;; in comment
            /*
            int index = Text.LastIndexOf(';');
            if (index != -1)
            {
                if (index == Text.Length-1)
                    Comment = "";
                else
                    Comment = Text.Substring(index);
                Text = Text.Substring(0, index).Trim();
            }
            */
            bool ignore = false;

            for (int i = 0; i < Text.Length; i++)
            {
                if (Text[i] == '\"')
                {
                    ignore = !ignore;
                }
                else if (Text[i] == ';' && !ignore)
                {
                    Comment = Text.Substring(i).Trim();
                    Text = Text.Substring(0, i).Trim();
                    try
                    {

                    }
                    catch
                    {
                        Comment = "";
                    }
                    break;
                }
            }

        }

        public void WriteBytes(BinaryWriter bw, byte[] data)
        {
            foreach (byte b in data)
            {
                bw.Write(b);
            }
        }


    }

    class Module : Statement
    {
        public Module(String s)
        {
            Text = s;
        }
    }

    /// <summary>
    /// A #define statement
    /// </summary>
    class DefineStatement : Statement
    {
        public DefineStatement(string s)
        {
            Text = s;

            //strip off #define
            string str = s.Substring(7).Trim();

            int ix = str.IndexOf(';');
            if (ix != -1)
            {
                Comment = str.Substring(ix);
                str = str.Substring(0, ix).Trim();
            }

            //split it on the space
            string[] pieces = str.Split(new char[] { ' ', '\t' });

            List<string> list = pieces.ToList();
            list.RemoveAll((str1) => { return str1 == "" || str1 == " " || str1 == "\t"; });
            pieces = list.ToArray();

            if (pieces.Length != 2)
                throw new Exception("Line " + LineNumber + ": Invalid #define");

            Value = pieces[0].Trim();
            Replacement = pieces[1].Trim();
            /*
            if (Replacement.Contains("+") || Replacement.Contains("-"))
            {
                throw new Exception("Error in define: " + str + ". +/- not allowed");
            }*/
        }


        public string Value { get; set; }
        public string Replacement { get; set; }
    }



    abstract class SpaceAllocation : Statement, IWritable
    {
        protected List<string> tokens = new List<string>();

        public abstract void WriteBinary(BinaryWriter bw);

        public void ReplaceLabels(Dictionary<string, ushort> labelOffsets)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (labelOffsets.Keys.Contains(tokens[i]))
                {
                    try
                    {
                        string temp = tokens[i];
                        tokens.RemoveAt(i);
                        tokens.Insert(i, "" + labelOffsets[temp]);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error replacing label with address");
                    }
                }
            }
        }

        protected void ParseValues(string s, Dictionary<string, string> defines)
        {
            StringBuilder b = new StringBuilder(s);

            List<string> pieces = new List<string>();

            while (b.Length != 0)
            {
                pieces.Add(GetNextValue(b));
            }


            if (pieces.Count == 0)
                throw new Exception("Missing value in DB statement");

            foreach (string p in pieces)
            {
                string temp = p;

                if (defines.ContainsKey(temp))
                    temp = defines[p];

                if (temp.IsNumber())
                {
                    tokens.Add(temp);
                }
                else if (temp.StartsWith("\""))
                {//string literal
                    if (!temp.EndsWith("\""))
                    {
                        throw new Exception(temp + " Missing a closing quotation mark");
                    }

                    tokens.Add(temp);

                }
                else
                {//assume its a label
                    if (temp.Contains('+'))
                    {//replace each part
                        int sum = 0;
                        string[] ops = temp.SplitAndTrim('+');

                        for (int i = 0; i < ops.Length; i++)
                        {
                            sum += Convert.ToInt32(defines[ops[i]]);
                        }
                        temp = sum.ToString();
                    }

                    tokens.Add(temp);
                }

            }//end foreach

        }

        /// <summary>
        /// Read until a comma, space, or end of line
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected string GetNextValue(StringBuilder s)
        {
            int i = 0;
            bool completed = false;
            string val = "";

            String temp = s.ToString().Trim();
            s.Clear();
            s.Insert(0, temp);
            if (s.ToString().First() == '\"')
            {

                //read to closing quotation mark
                for (i = 1; i < s.Length; i++)
                {
                    if (s[i] == '\"')
                    {
                        completed = true;
                        val = s.ToString().Substring(0, i + 1);
                        break;
                    }
                }

                if (!completed)
                    throw new Exception("Couldn't find matching quotation mark");

                //skip commas, spaces

                string rest = s.ToString().Substring(i + 1, (s.Length - i) - 1).Trim();

                if (rest == "")
                    s.Clear();
                else if (rest.First() == ',')
                {
                    rest = rest.Substring(1, rest.Length - 1);
                    s.Clear();
                    s.Insert(0, rest);
                }
            }
            else
            {//digit

                for (i = 0; i < s.Length; i++)
                {
                    if (s[i] == ',')
                    {
                        completed = true;
                        val = s.ToString().Substring(0, i).Trim();
                        break;
                    }
                }


                if (completed) //hit a comma 
                {
                    string rest = s.ToString().Substring(i + 1).Trim();
                    s.Clear();
                    s.Insert(0, rest);
                }
                else
                {//hit end
                    val = s.ToString().Trim(); //return whole line
                    s.Clear();
                }

            }

            return val;
        }

    }

    class DBStatement : SpaceAllocation
    {
        /// <summary>
        /// DB can be ffh, 0, "string"
        /// </summary>
        /// <param name="s"></param>
        public DBStatement(string s, Dictionary<string, string> defines)
        {
            //remove "DB"
            Text = s;
            TrimTrailingComment();

            s = Text.Trim().Substring(2).Trim();
            Text = s;
            ParseValues(s, defines);
        }

        /// <summary>
        /// writes the bytes to the file
        /// </summary>
        /// <param name="bw"></param>
        public override void WriteBinary(BinaryWriter bw)
        {
            throw new NotImplementedException("WriteBinary not implemented");
        }

        public override string AsHextText()
        {
            string s = "";



            foreach (string b in tokens)
            {

                if (b.IsHexByteConstant() || b.IsNumber())
                {
                    string temp = b.ToByteHexText();
                    s += temp;
                }
                else if (b.IsStringLiteral())
                {
                    s += b.AsHexText();
                }
                else
                    throw new Exception("DB statement: Unable to handle " + s);

                if (s.Length % 2 != 0)
                {
                    s += "";
                }


            }
            return s;
        }

        public override ushort GetBinaryLength()
        {
            int sum = 0;
            foreach (string s in tokens)
            {
                if (s.StartsWith("\""))
                    sum += s.Length - 2; //don't count "" chars
                else
                    sum++;
            }

            return (ushort)sum;
        }
    }

    class DWStatement : SpaceAllocation
    {

        /// <summary>
        /// Could be more than one constant (comma separated)
        /// </summary>
        /// <param name="s"></param>
        public DWStatement(string s, Dictionary<string, string> defines)
        {
            Text = s;
            TrimTrailingComment();
            s = Text.Replace("DW", " ").Trim();
            ParseValues(s, defines);
        }

        /// <summary>
        /// writes the bytes to the file
        /// </summary>
        /// <param name="bw"></param>
        public override void WriteBinary(BinaryWriter bw)
        {
        }

        public override string AsHextText()
        {
            string s = "";

            foreach (string b in tokens)
            {

                if (b.IsHexWordConstant() || b.IsNumber())
                {
                    //write the bytes flipped
                    string w = b.ToWordHexText();
                    string hi = w.Substring(0, 2);
                    string lo = w.Substring(2);
                    s += lo + hi;
                }
                else
                    throw new Exception("DW statement: Unable to handle " + s);
            }
            return s;
        }

        public override ushort GetBinaryLength()
        {
            return (ushort)(tokens.Count * 2);
        }

    }

    class RESBStatement : SpaceAllocation
    {
        public int HowMany { get; set; }

        public RESBStatement(string s)
        {
            string[] parts = s.Split(' ');
            HowMany = Convert.ToInt32(parts[1]);
        }


        public override void WriteBinary(BinaryWriter bw)
        {
            for (int i = 0; i < HowMany; i++)
            {
                bw.Write((byte)0);
            }
        }

        public override ushort GetBinaryLength()
        {
            return (ushort)HowMany;
        }

        public override string AsHextText()
        {
            string s = "";
            for (int i = 0; i < HowMany; i++)
            {
                s += "00";
            }
            return s;
        }
    }

    class OrgStatement : Statement
    {

        public OrgStatement(string s)
        {
            s = s.Trim();
            //remove any comment
            if (s.Contains(";"))
            {
                s = s.Substring(0, s.IndexOf(";")).Trim();
            }
            s = s.Substring(s.IndexOfAny(new char[] { ' ', '\t' })).Trim();

            if (s.IsHexLiteral())
            {
                Address = s.HexToDecimal();
            }
            else if (s.IsDecimalNumber())
            {
                Address = Convert.ToUInt16(s);
            }
            else throw new Exception("Unable to proccess org address:" + s);

        }
    }
}
