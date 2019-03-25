using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//using Emitters;
//using CLikeLang;
using CLL;

namespace PlayerLib
{
    partial class Game
    {

        public override void CallFunction(string name)
        {
            /* can't quit from player*/
            if (name.ToUpper().Equals("QUIT()"))
                return;

            if (!functions.Keys.Contains(name))
                throw new Exception("Trying to execute unkown function " + name);
            try
            {
                functions[name].Execute(this);
            }
            catch (Exception e)
            {
                throw new Exception("Error running function " + name, e);
            }

        }

        void BuildFunctions(XmlDocument doc)
        {
            events.Clear();
            functions.Clear();

            XmlNodeList evts = doc.SelectNodes("//project/events/event");


            foreach (XmlNode n in evts)
            {
                string name = "";
                try
                {
                    name = n.Attributes.GetNamedItem("name").Value;
                    string code = n.InnerText;
                    events.Add(CLL.FunctionBuilder.BuildFunction(this, name + "_sub", code));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in Event or Function " + name, ex);
                }
            }

            evts = doc.SelectNodes("//project/routines/routine");

            foreach (XmlNode n in evts)
            {
                string name = "";
                try
                {
                    CLL.FunctionBuilder fb = new CLL.FunctionBuilder();

                    name = n.Attributes.GetNamedItem("name").Value;
                    string code = n.InnerText;
                    CLL.Function f = CLL.FunctionBuilder.BuildFunction(this, name, code);
                    functions.Add(name, f);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in function or event " + name, ex);
                }
            }
        }

    }
}
