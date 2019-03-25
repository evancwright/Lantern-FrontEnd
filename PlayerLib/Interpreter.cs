using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
//using Emitters;
using XTAC;

namespace PlayerLib
{
    class Interpreter
    {
        TextBox window;
        XTAC.Xml xproject;
        List<Variable> vars = new List<Variable>();

        public Interpreter(XTAC.Xml xproject, TextBox tb)
        {
            PrintTitle();
            PrintAuthor();
            Printcr();


        }

        //put vars in list
        void InitVars()
        {
            vars.Clear();
   
            foreach (Var v in xproject.Project.Variables.Builtin.Var)
            {
                vars.Add(new Variable(v.Name, Convert.ToInt16(v.Value)));
            }

            foreach (Var v in xproject.Project.Variables.User.Var)
            {
                vars.Add(new Variable(v.Name, Convert.ToInt16(v.Value)));
            }
           
        }

        void SetVar(string name, int value)
        {

        }

        void SubVar(string name, int amt)
        {

        }

        void AddVar(string name, int amt)
        {

        }

        void PrintAuthor()
        {
            window.Text += xproject.Project.Author; 
        }

        void PrintTitle()
        {
            window.Text += xproject.Project.Welcome; 
        }

        void Printcr()
        {
            window.Text += "\r\n"; 
        }        
    }
}
