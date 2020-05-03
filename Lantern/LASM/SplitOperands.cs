using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASM
{
    partial class Assembly
    {
        /// <summary>
        /// Breaks up the op code, and operands
        /// </summary>
        void SplitOperands()
        {
            foreach (Statement s in statements)
            {
                if (s is ExecutableStatement)
               {
                    ExecutableStatement ex = s as ExecutableStatement;

                    try
                    {

                        if (s.Text.Contains(";"))
                        {
                            char[] splitChars = { ';' };
                            string[] toks = s.Text.Split(splitChars);
                            ex.TrailingComment = ";" + toks[1];
                            s.Text = toks[0]; //text is now the left side
                        }

                        //anything left is an opcode or an operand
                        StringBuilder sb = new StringBuilder(s.Text);
                        sb.Replace('\t', ' ');

                        //get the first piece of text

                        if (sb.ToString().IndexOf(",") != -1)
                        {//opcode plus two operands
                            char[] splitChars = { ',' };

                            string right = sb.ToString().Substring(s.Text.IndexOf(',') + 1).Trim();
                            string left = sb.ToString().Substring(0, s.Text.IndexOf(',')).Trim();

                            //is the 2nd operand a compound operand [Y] [X]
                            string[] pieces = left.Split(' ');
                            ex.OpCode = pieces[0].Trim();
                            ex.Op1 = pieces[1].Trim();
                            ex.Op2 = right.Trim();

                            //r2 is has a register offset
                            if (ex.Op2.Contains("["))
                            {
                                pieces = ex.Op2.Split('[');
                                left = pieces[0].Trim();
                                right = ex.Op2.Substring(ex.Op2.IndexOf('[') + 1);
                                right = right.Replace(']', ' ');
                                right = right.Trim();
                                ex.Op2 = left;
                                ex.Op3 = right;
                            }

                        }
                        else
                        {//opcoe or opcode + one operand
                            if (s.Text.Contains(" "))
                            {//two operands
                                char[] chars = { ' ' };
                                string[] pieces = s.Text.Split(chars);

                                ex.OpCode = pieces[0].Trim();
                                ex.Op1 = pieces[1].Trim();


                            }
                            else
                            {//one operand
                                ex.OpCode = s.Text;
                            }
                        }
                    }
                    catch (Exception x)
                    {
                        throw new Exception("Line " + s.LineNumber + ":Unable to split statement: " + s.Text, x);
                    }
                }
            }
        }
    }
}
