using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XTAC
{
    public class CodeEditor : TextBox
    {
        
        List<Keys> lastKeys = new List<Keys>();

        public CodeEditor() : base()
        {
            Multiline = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                //   codeTextBox.Paste("   ");

                e.Handled = true;
            }
        }



        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true; //handle tabs ourselves
            }

            lastKeys.Add(e.KeyCode);

            if (lastKeys.Count > 4)
                lastKeys.RemoveAt(0);

        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
            {
                if (SelectionLength == 0)
                {//tabbing a line or inserting a template

                    if (lastKeys.Count == 4)
                    {
                        if (lastKeys[0] == Keys.I &&
                            lastKeys[1] == Keys.F &&
                            lastKeys[2] == Keys.Tab &&
                            lastKeys[3] == Keys.Tab)
                        {//insert if ()  {  } 
                            //delete previous 6 spaces
                            int selStart = SelectionStart;
                            string s = Text.Remove(SelectionStart - 5, 5);
                            Text = s;
                            SelectionStart = selStart - 5;


                            string template = " (  )\r\n";

                            int indents = CodeFormatter.GetIndentSize(s, selStart - 5);
                            string spaces = "";
                            for (int i = 0; i < indents; i++)
                            {
                                spaces += ' ';
                                ;
                            }
                            template += spaces + "{\r\n" + spaces + "}\r\n";


                            s = s.Insert(SelectionStart, template);
                            Text = s;
                            SelectionStart = selStart - 2;
                        }
                        else if (lastKeys[0] == Keys.S &&
                            lastKeys[1] == Keys.E &&
                            lastKeys[2] == Keys.Tab &&
                            lastKeys[3] == Keys.Tab)
                        {
                            int selStart = SelectionStart;
                            string s = Text.Remove(SelectionStart - 5, 5);
                            Text = s;
                            SelectionStart = selStart - 5;

                            string template = " \r\n";

                            int indents = CodeFormatter.GetIndentSize(s, selStart - 5);
                            string spaces = "";
                            string dblspaces = "";
                            int indentLevel = indents / 5;
                            for (int i = 0; i < indents; i++)
                            {
                                spaces += ' ';
                            }
                            for (int i = 0; i < 5 * (indentLevel+1); i++)
                            {
                                dblspaces += ' ';
                            }
                            template += spaces + "{\r\n" + dblspaces + "\r\n" + spaces + "}\r\n";


                            s = s.Insert(SelectionStart, template);
                            Text = s;
                            SelectionStart = selStart + dblspaces.Length + 1;

                        }
                        else
                            Paste("     ");

                    }
                    else
                        Paste("     ");

                    e.Handled = true;
                }
                else
                {//block tabbing
                    string newCode = CodeFormatter.InsertTabs(Text,
                        SelectionStart,
                        SelectionStart + SelectionLength);
                    Text = newCode;
                    e.Handled = true;
                }

            }
        }

        

         
        

    }
}
