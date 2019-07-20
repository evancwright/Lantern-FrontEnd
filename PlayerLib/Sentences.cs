using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace PlayerLib
{
    partial class Game
    {

        void BuildSentences(XmlDocument doc)
        {
            //clear lists
            before.Clear();
            instead.Clear();
            after.Clear();

            //build before
            XmlNodeList sentences = doc.SelectNodes("//project/sentences/sentence");
            Game g = Game.GetInstance();

            foreach (XmlNode s in sentences)
            {
                string verb = s.Attributes.GetNamedItem("verb").Value;
                int verbId = verbMap[verb.ToUpper()];
                string prep = s.Attributes.GetNamedItem("prep").Value;
                int prepId = prepTable.IndexOf(prep.ToUpper());


                string doObj = s.Attributes.GetNamedItem("do").Value;
                int doId = GetObjectId(doObj);



                string ioObj = s.Attributes.GetNamedItem("io").Value;
                int ioId = GetObjectId(ioObj);

                string subName = s.Attributes.GetNamedItem("sub").Value;
                // subName += "_sub";


                string stype = s.Attributes.GetNamedItem("type").Value;

                if (stype.Equals("before"))
                    before.Add(new Sentence(verbId, doId, prepId, ioId, subName));
                else if (stype.Equals("instead"))
                    instead.Add(new Sentence(verbId, doId, prepId, ioId, subName));
                else
                    after.Add(new Sentence(verbId, doId, prepId, ioId, subName));

            }
        }

        bool TryRun(List<Sentence> list)
        {
            foreach (Sentence s in list)
            {
                string sv = GetVerbName(s.verb);
                string fn = s.name;

                if (s.verb == verb && s.dobj == dobj && s.prep == prep && s.iobj == iobj)
                {
                    try
                    {

                        if (functions.ContainsKey(s.name))
                        {
                            functions[s.name].Execute(this);
                        }
                        else
                            throw new Exception("Sentence is calling a missing function: " + s.name);

                        return true;
                    }
                    catch (Exception e)
                    {

                        string message = e.Message;
                        Exception ex = e;
                        while (ex.InnerException != null)
                        {
                            message += "\r\n" + ex.Message;
                            ex = ex.InnerException;
                        }

                        throw new Exception("Error in function" + fn + "\r\n" + message);

                    }

                }
            }

            //now try wildcards
            foreach (Sentence s in list)
            {
                int tempDobj = dobj;
                if (s.dobj == 254)
                    tempDobj = 254;

                int tempIobj = iobj;
                if (s.iobj == 254)
                    tempIobj = 254;


                if (s.verb == verb && s.dobj == tempDobj && s.prep == prep && s.iobj == tempIobj)
                {
                    functions[s.name].Execute(this);
                    return true;
                }
            }


            return false;
        }

        string GetVerbName(int id)
        {
            foreach (KeyValuePair<string, int> t in verbMap)
            {
                if (t.Value == id)
                {
                    return t.Key;
                }
            }
            return "NOT FOUND!";
        }
    }
}
