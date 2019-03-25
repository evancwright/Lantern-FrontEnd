using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PlayerLib
{
    class Variable
    {
        public Variable(string name, int value)
        {
            this.name = name;
            val = value;
        }
        
        public int Value()
        {
            return val;
        }

        public void Add(int amt)
        {
            val += amt;

            if (val > 255)
            {
                val = val % 255;
            }

            while (val < 0)
            {
                val = 255 - Math.Max(Math.Abs(val),255); 
            }
        }

        public string name;
        public int val;
    }

    partial class Game
    {
        void LoadVars(XmlDocument doc)
        {
            this.vars.Clear();
            XmlNodeList vars = doc.SelectNodes("//project/variables/builtin/var");

            foreach (XmlNode n in vars)
            {
                int val = 0;
                try {
                    val = Convert.ToInt32(n.Attributes.GetNamedItem("value").Value);
                } 
                catch (Exception ex)
                {
                    val = 0;
                }

                Variable v = new Variable(
                    n.Attributes.GetNamedItem("name").Value,
                    val
                    );

                this.vars[v.name] = v;
            }

            vars = doc.SelectNodes("//project/variables/user/var");

            foreach (XmlNode n in vars)
            {
                Variable v = new Variable(
                    n.Attributes.GetNamedItem("name").Value,
                    Convert.ToInt32(n.Attributes.GetNamedItem("value").Value)
                    );

                this.vars[v.name] = v;
            }
        }
    }
}
