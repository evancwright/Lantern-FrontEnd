using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGame;
using CLL;
using System.IO;
using System.Xml;
using GameTables;

namespace XMLtoAdv
{
    public partial class XmlToTables
    {
        public void WriteWelcomeC()
        {
            using (StreamWriter sw = File.CreateText("Welcome.c"))
            {
                sw.WriteLine("/* Machine generated include */");

                XmlNodeList list = doc.SelectNodes("//project/welcome");
                sw.WriteLine("");
                sw.WriteLine("const char *WelcomeStr=  \"" + list[0].InnerText + "\";");
                list = doc.SelectNodes("//project/author");
                sw.WriteLine("const char *AuthorStr=\"" + list[0].InnerText + "\";");
                list = doc.SelectNodes("//project/version");
                sw.WriteLine("const char *VersionStr=\"" + list[0].InnerText + "\";");
            }
        }

        void WriteStringTableC(string fileName, string tableName, Table table, string externFlag="extern ")
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine("/**********************************************");
                sw.WriteLine("Machine generated string table ");
                sw.WriteLine("***********************************************/");
                sw.WriteLine("");
                sw.WriteLine(externFlag + "const int " + tableName + "Size=" + table.GetNumEntries() + ";\n");
                sw.WriteLine("const char *" + tableName + "[] = { ");

                for (int i = 0; i < table.GetNumEntries(); i++)
                {
                    if (table.GetEntry(i).Length > 0) //safety check
                    {
                        string delim = "";
                        if (i < table.GetNumEntries() - 1)
                            delim += ",";
                        sw.WriteLine("\t\"" + table.GetEntry(i) + "\"" + delim);
                    }
                }
                sw.WriteLine("};");
                sw.WriteLine("");
            }
        }

        void WriteObjectTableC(string externFlag="extern ")
        {
            using (StreamWriter sw = File.CreateText("ObjectTable.c"))
            {
                sw.WriteLine("/**********************************************");
                sw.WriteLine("Machine generated ojbect table ");
                sw.WriteLine("***********************************************/");
                int count = objects.Count();
                sw.WriteLine(externFlag + "const int NumObjects=" + count + ";");
                sw.WriteLine("unsigned char ObjectData[] = {");

                for (int i = 0; i < count; i++)
                {
                    GameObject obj = objects[i];
                    //sw.Write("\t{");
                    sw.Write("\t/*" + obj.name + "*/\n");
                    WriteObjectAttrsC(obj, sw);
                    WriteObjectPropsC(obj, sw, ",", "|");
                    /*
                    if (i < count - 1)
                        sw.Write("\t},\n");
                    else
                        sw.Write("\t}\n");  */
                    if (i < count - 1)
                        sw.Write("\t,\n");
                    else
                        sw.Write("\t\n");
                }
                sw.WriteLine("};");
            }
        }

        void WriteObjectAttrsC(GameObject o, StreamWriter sw)
        {
            //write the data bytes
            sw.Write(o.id.ToString() + ",");
            sw.Write(o.holder.ToString() + ",");

            int initialDescVal = 255;
            string initialDesc = o.initialdescription;
            if (initialDesc != "")
            {
                initialDescVal = descriptionTable.GetEntryId(o.initialdescription);
            }
            sw.Write(initialDescVal + ",");

            if (o.description != "")
            {
                sw.Write(descriptionTable.GetEntryId(o.description));
            }
            else
            {
                sw.Write("0"); //no description
            }

            for (int i = 0; i < GameObject.NUM_ATTRIBS; i++)
            {
                //if attrib is a direction and there is a nogo message, get the id
                if (o.HasNogoMsg(GameObject.attribNames[i])) // directions
                {
                    int msgId = nogoTable.GetEntryId(o.GetNogoMsg(GameObject.attribNames[i]));
                    int uId = 256 - msgId;
                    sw.Write("," + uId.ToString());
                }
                else
                {
                    sw.Write("," + o.attribs[i].ToString());
                }
            }
        }

        void WriteObjectWordTableC(string externFlag="extern ")
        {
            using (StreamWriter sw = File.CreateText("ObjectWordTable.c"))
            {
                sw.WriteLine("/**********************************************");
                sw.WriteLine("Machine generated ojbect word table ");
                sw.WriteLine("***********************************************/");
                sw.WriteLine("");

                int total = objects.Count() + CountSynonyms();
                int c = 0;
                sw.WriteLine(externFlag + "const int ObjectWordTableSize=" + total + ";");
                //                sw.WriteLine("unsigned char ObjectWordTableData[" + total + "][4] = {");
                sw.WriteLine("unsigned char ObjectWordTableData[] = {");
                foreach (GameObject o in objects)
                {
                    //    sw.Write("\t{" + o.id);
                    sw.Write("\t" + o.id);
                    string name = o.printedName;

                    string[] toks = name.Split(' ');
                    int count = toks.Length;

                    List<string> words = new List<string>();
                    foreach (string s in toks)
                    {
                         words.Add(s);
                    }


                    //filter words
                    for (int i = 0; i < Math.Min(words.Count, 3); i++)
                    {
                        //look up the id of each word

                        int wordId = dict.GetEntryId(words[i]);

                        /*make sure -1 is converted to 255*/
                        if (wordId == -1)
                            wordId = 255;

                        sw.Write("," + wordId);
                    }
                    int blanks = 3 - count;

                    //now append the blanks
                    for (int i = 0; i < blanks; i++)
                    {
                        sw.Write(",255");
                    }

                    //sw.Write("}");

                    if (c < total - 1)
                        sw.Write(", /*   " + o.name + "*/\n");
                    else
                        sw.Write(" /*   " + o.name + "*/\n");
                    c++;
                }//end write objs


                //now write any synonyms
                foreach (GameObject o in objects)
                {
                    if (o.synonyms.Count > 0 && o.synonyms[0] != "")
                    {
                        //        sw.Write("\t{" + o.id);
                        sw.Write("\t"+o.id);
                        int blanks = 3 - o.synonyms.Count;

                        for (int i = 0; i < o.synonyms.Count; i++)
                        {
                            //look up the id of each word
                            int wordId = dict.GetEntryId(o.synonyms[i]);
                            sw.Write("," + wordId);
                        }

                        //now append the blanks
                        for (int i = 0; i < blanks; i++)
                        {
                            sw.Write(",255");
                        }

                        //sw.Write("}");

                        if (c < total - 1)
                            sw.Write(", /*  synonym for " + o.name + "*/\n");
                        else
                            sw.Write(" /*   synonym for " + o.name + "*/\n");
                        c++;
                    }

                }//end write synonyms

                sw.WriteLine("\t};");

                //sw.WriteLine("\t255,255,255,255");
                //                sw.WriteLine("obj_table_size\t" + byteDirective + " " + objects.Count);
            }


        }

        int CountSynonyms()
        {
            int count = 0;
            //now write any synonyms
            foreach (GameObject o in objects)
            {
                if (o.synonyms.Count > 0 && o.synonyms[0] != "")
                {
                    count++;
                }//end write synonyms
            }
            return count;
        }

        void WriteSentenceTableC(string fileName, string tableName, string type, int ptrSize = 2, string externFlag="extern ")
        {
            XmlNodeList subs = doc.SelectNodes("//project/sentences/sentence");
            List<string> subNames = new List<string>();
             
            using (StreamWriter sw = File.CreateText(fileName))
            {
                int c = CountSentences(type);

                sw.WriteLine("/**********************************************");
                sw.WriteLine("Machine generated sentence table");
                sw.WriteLine("***********************************************/");
                sw.WriteLine("");
                sw.WriteLine("");

                if (type == "before")
                    sw.WriteLine(externFlag + "const int BeforeTableSize=" + c + ";");
                else if (type == "instead")
                    sw.WriteLine(externFlag + "const int InsteadTableSize=" + c + ";");
                else
                    sw.WriteLine(externFlag + " const int AfterTableSize=" + c + ";");
                sw.WriteLine("unsigned char " + tableName + "Data[] = {");
                int count = 0;
                
                foreach (XmlNode s in subs)
                {
                    string tp = s.Attributes.GetNamedItem("type").Value;

                    if (tp.Equals(type))
                    {
                        count++;
                        string verb = s.Attributes.GetNamedItem("verb").Value;
                        int verbId = GetVerbId(verb);
                         
                        string prep = s.Attributes.GetNamedItem("prep").Value;
                        int prepId = prepTable.GetEntryId(prep);

                        if (prepId == -1)
                        {
                            prepId = 255;
                        }

                        string doObj = s.Attributes.GetNamedItem("do").Value;
                        int doId = GetObjectId(doObj);

                        if (doId == -1)
                        {
                            doId = 255;
                        }

                        string ioObj = s.Attributes.GetNamedItem("io").Value;
                        int ioId = GetObjectId(ioObj);

                        if (ioId == -1)
                        {
                            ioId = 255;
                        }

                        string subName = s.Attributes.GetNamedItem("sub").Value;

                        string comma = ",";
                        if (count == c)
                            comma = " ";
                        string line = "\t" + verbId + "," + doId + "," + prepId + "," + ioId;

                        for (int i = 0; i < ptrSize; i++)
                        {
                            line += ",0";
                        }

                        line +=  comma + "\t /*" + verb + " " + doObj + " " + prep + " " + ioObj + "->" + subName + "*/";
                        sw.WriteLine(line);
                        subNames.Add(subName);

                    }
                }

                if (count == 0)
                {
                    sw.WriteLine("\t0 /* can't have empty array */");
                }

                sw.WriteLine("\t};");
                sw.WriteLine("");

                sw.WriteLine("");
                sw.WriteLine("void init_" + type + "_functions()");
                sw.WriteLine("{");
                sw.WriteLine("\t" + tableName + " = (Sentence*)" + tableName + "Data;");
                for (int i = 0; i < subNames.Count(); i++)
                {
                    sw.WriteLine("\t" + tableName + "[" + i + "].handler = " + subNames[i] + "_sub;");
                }

                sw.WriteLine("}");

            }//end using


        }

        int CountSentences(string type)
        {
            int count = 0;
            XmlNodeList subs = doc.SelectNodes("//project/sentences/sentence");
            foreach (XmlNode s in subs)
            {
                string tp = s.Attributes.GetNamedItem("type").Value;

                if (tp.Equals(type))
                {
                    count++;
                }
            }
            return count;
        }

        void WriteVerbTableC(int ptrSize=2)
        {
            int count = 0;
            char[] seps = { ',' };
            List<string> verbAtoms = new List<string>();

            using (StreamWriter sw = File.CreateText("VerbTable.c"))
            {
                
                sw.WriteLine("char VerbTableData[]={");

                //write the function to initialize it
                for (int i = 0; i < verbs.GetNumEntries(); i++)
                {
                    string verb = verbs.GetEntry(i);

                    //split it up using commas
                    string[] toks = verb.Split(seps);
                    count += toks.Length;
                    for (int j = 0; j < toks.Length; j++)
                    {
                        string comma = ",";
                        if (i == verbs.GetNumEntries() - 1)
                            comma = " ";

                        string line = "\t" + i;
                        for (int k = 0; k < ptrSize; k++)
                            line += ",0";

                        line +=  comma + "/*" + toks[j].ToUpper() + "*/";
                        sw.WriteLine(line);
                        verbAtoms.Add(toks[j].ToUpper());
                    }

                }
                sw.WriteLine("\t};");
                sw.WriteLine("");
                sw.WriteLine("void init_verb_table()");
                sw.WriteLine("{");
                sw.WriteLine("VerbTable = (WordEntry*)VerbTableData;");
                for (int i = 0; i < verbAtoms.Count; i++)
                {
                    sw.WriteLine("\tVerbTable[" + i + "].wrd = (char*)\"" + verbAtoms[i] + "\";");
                }
                sw.WriteLine("}");
            }//end using

            //write header
            using (StreamWriter sw = File.CreateText("VerbDefs.h"))
            {
                for (int i = 0; i < verbs.GetNumEntries(); i++)
                {
                    string verb = verbs.GetEntry(i);
                    string[] toks = verb.Split(seps);
                    string verb_id = toks[0].ToLower().Replace(' ', '_') + "_verb_id";
                    sw.WriteLine("#define " + verb_id.ToUpper() + " " + i);
                }

                sw.WriteLine("const int NumVerbs=" + count + ";\n");

            }
        }


        void WriteCheckTableC(int ptrSize = 2, string externFlag = "extern ")
        {
            using (StreamWriter sw = File.CreateText("CheckTable.h"))
            {
                XmlNodeList checks = doc.SelectNodes("//project/checks/check");
                sw.WriteLine("");
                sw.WriteLine("unsigned char VerbCheckTableData[] = {");
                int count = checks.Count;
                int k = 0;
                foreach (XmlNode c in checks)
                {
                    string verb = c.Attributes.GetNamedItem("verb").Value;

                    if (verb.IndexOf(",") != -1)
                    {//if there are synonyms, get 1st
                        verb = verb.Substring(0, verb.IndexOf(","));
                    }

                    int verbId = GetVerbId(verb);

                    if (verbId != -1)
                    {
                        string subName = c.Attributes.GetNamedItem("check").Value;
                        string comma = ",";

                        if (k == count - 1)
                            comma = "";

                        string line = "\t" + verbId;

                        for (int x =0; x < ptrSize; x++)
                            line += ",0";

                        line += comma + " /*" + verb + "->" + subName + "*/";

                        sw.WriteLine(line);
                    }
                    else
                    {
                        throw new Exception("Exception while writing check table. Unknown verb: " + verb);
                    }
                    k++;
                }//end foreach
                sw.WriteLine("};");

                //write the initialization function
                sw.WriteLine("void init_verb_checks()");
                sw.WriteLine("{");
                sw.WriteLine("\tVerbCheckTable = (VerbCheck*)VerbCheckTableData;");
                int i = 0;
                foreach (XmlNode c in checks)
                {
                    sw.WriteLine("\tVerbCheckTable[" + i + "].check = " + c.Attributes.GetNamedItem("check").Value + ";");
                    i++;
                }

                sw.WriteLine("}");

                sw.WriteLine(externFlag + "const int NumVerbChecks=" + i + ";");
            }//end using
        }

        void WriteUserVarsC()
        {
            using (StreamWriter sw = File.CreateText("UserVars.c"))
            {
                sw.WriteLine("/*********************************");
                sw.WriteLine(" User variables");
                sw.WriteLine("**********************************/");
                sw.WriteLine("");

                foreach (UserVar v in userVars)
                {
                    sw.WriteLine("unsigned char " + v.name + "=" + v.initialVal + ";");
                }
                sw.WriteLine("const int NumUserVars=" + userVars.Count + ";\n");
            }
        }

        private void WriteObjectPropsC(GameObject o, StreamWriter sw, string byteSep, string orSym)
        {
            sw.WriteLine("");

            List<string> flags = new List<string>();

            int sum = 0;
            for (int i = 0; i < 16; i++)
            {
                bool val = o.GetXmlFlag(GameObject.xmlFlagNames[i]);
                if (val)
                {
                    flags.Add(asmFlagNames[i]);
                    sum += (int)Math.Pow(2.0, (double)i);
                }
            }

            string flagsStr = "";
            if (flags.Count == 0) { sw.WriteLine("\t" + byteSep + " 0,0  /*flags*/  "); }
            else
            {
                for (int i = 0; i < flags.Count; i++)
                {
                    if (i != 0) { flagsStr += orSym; }
                    flagsStr += flags[i];
                }
                sw.WriteLine("\t," + sum%256+"," +sum/256 + " /* hi,lo" + flagsStr +"*/");

            }
        }

        void WriteEventsHeader(XmlDocument doc, CDECLYesNo cdelcYN)
        {


            using (StreamWriter sw = File.CreateText("Events.h"))
            {
                sw.WriteLine("/*User defined routines*/");
                XmlNodeList routines = doc.SelectNodes("//project/routines/routine");

                string callStyle = "";
                if (cdelcYN == CDECLYesNo.YES)
                    callStyle = "__cdecl ";

                foreach (XmlNode n in routines)
                {
                    sw.WriteLine("void " + callStyle + n.Attributes.GetNamedItem("name").Value + "_sub();");
                }

                routines = doc.SelectNodes("//project/events/event");

                foreach (XmlNode n in routines)
                {
                    sw.WriteLine("void " + callStyle + n.Attributes.GetNamedItem("name").Value + "_event();");
                }
            }
        }
    }

    enum CDECLYesNo { NO, YES };

}
