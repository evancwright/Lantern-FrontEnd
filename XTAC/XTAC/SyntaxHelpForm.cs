using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XTAC
{
    public partial class SyntaxHelpForm : Form
    {
        public SyntaxHelpForm()
        {
            InitializeComponent();
            AddText();
        }

        void AddText()
        {
            textBox1.Text = "The code editor accept a simple C-like language.  It supports ";
            textBox1.Text += "if/then/else style control flow. All statements (even single statements) ";
            textBox1.Text += "must be enclosed in curly braces.  Statements are ended with the semicolon ; character";
            textBox1.Text += "\n";
            textBox1.Text += "Functions:\n";
            textBox1.Text += "look();    prints the current room description\n";
            textBox1.Text += "move();    executes the move command the user entered.\n"; 
            textBox1.Text += "\n"; 
            textBox1.Text += "Variable Operations:\n";
            textBox1.Text += "set( variable, newvalue);\n";
            textBox1.Text += "add( variable, changeAmount );\n";

        }
    }
}
