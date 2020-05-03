using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using GameTables;
using CLL;

namespace XMLtoAdv
{
    public partial class XmlToTables : IGame.IGameXml
    {
        protected XmlDocument doc;
        protected Table nogoTable = new Table();
        protected Table descriptionTable = new Table();
        protected Table dict = new Table();
        protected Table verbs = new Table(); //unsplit strings
        protected Table prepTable = new Table();
        protected List<String> routineNames = new List<String>();
        protected Dictionary<string, string> varTable = new Dictionary<string, string>(); //variables
        protected List<UserVar> userVars = new List<UserVar>();
        static protected Dictionary<string, string> skelDirs = new Dictionary<string, string>(); //generic files (i.e. 6502)
        static protected Dictionary<string, string> pltfDirs = new Dictionary<string, string>(); //platform specific files (ie. Apple2)

        public const int TWO_BYTE_POINTERS = 2;
        public const int FOUR_BYTE_POINTERS = 4;
        public const int EIGHT_BYTE_POINTERS = 8;
        public const string NO_EXTERN = "";
        const string Trs80SkelDir = "trs80Skel";
        public string buildDir;
        public string info = "x86 Support";
        //Table strings = new Table(); //for events

        protected List<GameObject> objects = new List<GameObject>();
        protected static string[] noiseWords = { "OF", "THE", "A", "AN" };
        protected static string[] asmFlagNames =   {
                    "SCENERY_MASK", "SUPPORTER_MASK", "CONTAINER_MASK", "TRANSPARENT_MASK",
                    "OPENABLE_MASK","OPEN_MASK", "LOCKABLE_MASK", "LOCKED_MASK", 
                    "PORTABLE_MASK", "USER3_MASK", "WEARABLE_MASK", "BEINGWORN_MASK", 
                    "USER1_MASK", "EMITTING_LIGHT_MASK","DOOR_MASK", "USER2_MASK" 
                           };

         protected string[] preps = new string[] { "IN", "ON", "AT", "UNDER", "INTO", "INSIDE", "THROUGH", "OUT",
         "BEHIND", "OFF", "UP", "WITH", "TO", "OFF", "NORTH", "SOUTH", "EAST", "WEST", "NORTHEAST", "SOUTHEAST",
         "NORTHWEST", "SOUTHWEST", "UP",
         "DOWN", "ABOUT", "OVER", "ACROSS" };
         


        /*
       protected string[] preps = new string[]
       {            	
           "IN",
           "AT",
           "TO",
           "INSIDE",
           "OUT",
           "UNDER",
           "ON",
           "OFF",
           "INTO",
           "UP",
           "WITH",
           "NORTH",
           "SOUTH",
           "EAST",
           "WEST",
           "BEHIND"
       };
       */

        protected struct UserVar
        {
            public string name;
            public string initialVal;

            public UserVar(string name, string value)
            {
                this.name = name;
                this.initialVal = value;
            }

        }

        static XmlToTables()
        {
            skelDirs["_TRS80"] = "z80Skel";
            skelDirs["_CPM"] = "z80Skel";
            skelDirs["_CoCo"] = "CCommon";
            skelDirs["_Apple2"] = "6502Merlin";
            skelDirs["_C64"] = "6502Merlin";
            skelDirs["_CPC464"] = "z80Skel";
            skelDirs["_Spectrum"] = "z80Skel";
            skelDirs["_8086"] = "CCommon";
            skelDirs["_BBCMicro"] = "6502Merlin";
            skelDirs["_RPi"] = "CCommon";
            skelDirs["_x64"] = "CCommon";
            skelDirs["_VM"] = "LASMSkel";

            pltfDirs["_Spectrum"] = "spectrum";
            pltfDirs["_TRS80"] = "trs80";
            pltfDirs["_CPM"] = "cpm";
            pltfDirs["_Apple2"] = "Apple2Merlin";
            pltfDirs["_C64"] = "c64Merlin";
            pltfDirs["_CPC464"] = "cpc464";
            pltfDirs["_BBCMicro"] = "BBCMicro";
            pltfDirs["_RPi"] = "RPiSkel";
            pltfDirs["_8086"] = "8086Skel";
            pltfDirs["_x64"] = "x64Skel";
            pltfDirs["_CoCo"] = "CoCoSkel";
            pltfDirs["_VM"] = "LASMSkel";
        }


        protected void CreateTables(string fileName, string tgtPlatform)
        {
            try
            {
                doc = new XmlDocument();
                doc.Load(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to open XML file.", ex);
            }

            CreateOutputDir(tgtPlatform);


            descriptionTable.Clear();
            descriptionTable.AddEntry("You notice nothing unexpected.");
            PopulateNameTable(doc);
            PopulateNogoTable(doc, nogoTable);
            PopulateVerbTable(doc, verbs);
            PopulateVariableTable(doc);
            PopulateSubroutineNames(doc);
            ParseForStrings(doc, descriptionTable);
            ParseForFailStrings(doc, nogoTable);
            ValidateStringLength();


        }

        private void PopulateNameTable(XmlDocument doc)
        {
            XmlNodeList list = doc.SelectNodes("//project/objects/object");
//            nameTable.Clear();
            objects.Clear();
            descriptionTable.Clear();

            foreach (XmlNode n in list)
            {
                string name = n.Attributes.GetNamedItem("printedname").Value;
  //              nameTable.AddEntry(name);

                //get the child node with the description
                XmlNode child = n.ChildNodes[0];
                string desc = child.InnerText;

                //don't add blank descriptions
                if (desc != "")
                {
                    descriptionTable.AddEntry(desc);
                }


                //initial desc
                string initialDesc = n.SelectSingleNode("initialdescription").InnerText;

                if (initialDesc != "" && initialDesc != null)
                {
                    descriptionTable.AddEntry(initialDesc);
                }

                //break the name into words and put each word in the dictionary
                char[] delimiterChars = { ' ' };
                string[] toks = name.Split(delimiterChars);

                foreach (string s in toks)
                {
                    if (s != null && !s.Equals(""))
                    {
                        dict.AddEntry(s);
                    }
                }

                //create the object from the data
                GameObject gobj = new GameObject(n);
                objects.Add(gobj);

                foreach (string s in gobj.synonyms)
                {
                    if (!s.Equals(""))
                        dict.AddEntry(s);
                }

            }

        }

        public void PopulateNogoTable(XmlDocument doc, Table table)
        {
            XmlNode list = doc.SelectSingleNode("//project/objects");
            table.Clear();
            table.AddEntry("BLANK");
            table.AddEntry("You can't go that way.");

            foreach (XmlNode child in list)
            {
                XmlNode msgs = child.SelectSingleNode("nogo");

                if (msgs != null)
                {
                    foreach (XmlNode n in msgs.ChildNodes)
                    {
                        if (!n.InnerText.Equals(""))
                        {
                            string msg = n.InnerText;
                            table.AddEntry(msg);
                        }
                    }
                }

            }
        }

        void PopulateVariableTable(XmlDocument doc)
        {
            XmlNodeList vars = doc.SelectNodes("//project/variables/builtin/var");
            varTable.Clear();

            foreach (XmlNode n in vars)
            {
                string name = n.Attributes.GetNamedItem("name").Value;
                string addr = n.Attributes.GetNamedItem("addr").Value;
                string val = n.Attributes.GetNamedItem("value").Value;
                varTable[name] = addr;
            }

            userVars.Clear();

            vars = doc.SelectNodes("//project/variables/user/var");

            foreach (XmlNode n in vars)
            {
                string name = n.Attributes.GetNamedItem("name").Value;
                string addr = n.Attributes.GetNamedItem("addr").Value;
                string val = n.Attributes.GetNamedItem("value").Value;
                varTable[name] = name;

                userVars.Add(new UserVar(name, val));
            }
        }

        public void PopulateVerbTable(XmlNode doc, Table table)
        {
            verbs.Clear();

            XmlNode biv = doc.SelectSingleNode("//project/verbs/builtinverbs");

            XmlNodeList v = biv.SelectNodes("verb");

            foreach (XmlNode n in v)
            {
                table.AddEntry(n.InnerText);
            }

            biv = doc.SelectSingleNode("//project/verbs/userverbs");

            v = biv.SelectNodes("verb");

            foreach (XmlNode n in v)
            {
                table.AddEntry(n.InnerText);
            }
        }

        virtual public void Convert() { }

        private void ParseForFailStrings(string code, Table table)
        {
            //fail followed by whitespace 
            //followed by ( 
            //followed by whitespace 
            //followed by "
            //followed by text
            //followed by "
            //followed by )
            string regex = "fail(\\s)*\\((\\s)*\"[^\"]*[\"](\\s)*";

            MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(code, regex);

            foreach (Match m in matches)
            {
                string s = m.Value;
                //trim off the function call
                s = s.Substring(s.IndexOf("\"")+1);
                s = s.Substring(0, s.IndexOf("\""));
                table.AddEntry(s);
            }
        }


        private void ParseForStrings(string code, Table table)
        {
            try
            {
                //remove all commenrs
                string pattern = "//.*\r\n";
                code = Regex.Replace(code, pattern, "\n");

                //remove all the fail strings
                string failPattern = "fail(\\s)*\\((\\s)*\"[^\"]*[\"](\\s)*";
                code = Regex.Replace(code, failPattern, "");

                //remove newlines (these will cause empty strings)
                code = code.Replace("println(\"\")", "");

                int start = code.IndexOf("\"");
                if (start != -1)
                {
                    string rem = code.Substring(start + 1);
                    int end = rem.IndexOf("\"");

                    string substr = rem.Substring(0, end);

                    if (substr.Contains('\\'))
                     {
                        throw new Exception("The string [" + substr + "] contains a ' ' character.  This will create an escape sequence and mess up the string table.  Please remove it.");
                    }

                    if (substr.Length > 253)
                    {
                        throw new Exception("string '" + substr + "' is greater than 253 characters.\r\nShorten it or split it into multiple print statements.");
                    }

                    table.AddEntry(substr);
                    string rest = rem.Substring(end + 1);
                    ParseForStrings(rest, table);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in code near: " + code, ex);
            }
        }

        /*
         * Scans for strings in the events and puts them in the description table.
         */
        
       public void ParseForStrings(XmlDocument doc, Table table)
       {
           XmlNodeList events = doc.SelectNodes("//project/events/event");

           foreach (XmlNode n in events)
           {
               string code = n.InnerText;

               ParseForStrings(code, table);
           }

           events = doc.SelectNodes("//project/routines/routine");

           foreach (XmlNode n in events)
           {
               string code = n.InnerText;
               ParseForStrings(code, table);
           }
       
    }




        public void ParseForFailStrings(XmlDocument doc, Table table)
        {
            XmlNodeList events = doc.SelectNodes("//project/events/event");

            foreach (XmlNode n in events)
            {
                string code = n.InnerText;
                ParseForFailStrings(code, table);
            }

            events = doc.SelectNodes("//project/routines/routine");

            foreach (XmlNode n in events)
            {
                string code = n.InnerText;
                ParseForFailStrings(code, table);
            }
        }


        /*
         * Solves dependency problems
         */
        void PopulateSubroutineNames(XmlDocument doc)
        {
            routineNames.Clear();
            XmlNodeList subs = doc.SelectNodes("//project/routines/routine");

            foreach (XmlNode n in subs)
            {

                string name = n.Attributes.GetNamedItem("name").Value;
                //put name in list
                string fileName = name.Replace(' ', '_');
                routineNames.Add(fileName + "_sub");
            }
        }

        static XmlToTables instance = null;
         
        // string testRoutine = "if (GUARD HUT.holder==5) { print(\"You have the lamp\"); if (WHITE CUBE.description!=1) { print (\"It's open.\"); FLASHLIGHT.holder=offscreen;} }";

        private XmlToTables()
        {
          prepTable.Add(preps);
        }

        public static XmlToTables GetInstance()
        {
            if (instance == null)
            {
                instance = new XmlToTables();
            }

            return instance;
        }
        //ObjectTable objTable = new ObjectTable();

        public void Convert6809(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            //get the file path 
            CreateTables(fileName, "_CoCo");


            WriteWelcomeC();
            WriteStringTableC("StringTable.h", "StringTable", descriptionTable,NO_EXTERN);
            WriteStringTableC("NogoTable.h", "NogoTable", nogoTable, NO_EXTERN);
            WriteStringTableC("PrepTable.h", "PrepTable", prepTable, NO_EXTERN);
            WriteStringTableC("Dictionary.c", "Dictionary", dict, NO_EXTERN);

            WriteObjectTableC(NO_EXTERN);
            WriteObjectWordTableC(NO_EXTERN);
            WriteVerbTableC(TWO_BYTE_POINTERS);
            WriteCheckTableC(TWO_BYTE_POINTERS, "");

            WriteSentenceTableC("BeforeTable.c", "BeforeTable", "before", TWO_BYTE_POINTERS,NO_EXTERN);
            WriteSentenceTableC("AfterTable.c", "AfterTable", "after", TWO_BYTE_POINTERS, NO_EXTERN);
            WriteSentenceTableC("InsteadTable.c", "InsteadTable", "instead", TWO_BYTE_POINTERS, NO_EXTERN);

            /* events are still assembly language */
            WriteEvents(doc, "6809", new CLL.Visitor6809(this));

            WriteUserVarsC();

            Environment.CurrentDirectory = oldDir;
        }


        public void ConvertTRS80(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            //get the file path 
            CreateTables(fileName, "_TRS80");

            WriteZ80Common();

            Environment.CurrentDirectory = oldDir;
        }

        public void ConvertVM(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            //get the file path 
            CreateTables(fileName, "_VM");
            WriteZ80Common();
            Environment.CurrentDirectory = oldDir;
        }


        public void ConvertCPM(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            //get the file path 
            CreateTables(fileName, "_CPM");

            WriteZ80Common();

            //try to build
            FixBuildScript(doc);
            
            Environment.CurrentDirectory = oldDir;
        }

        public void ConvertCPC464(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            //get the file path 
            CreateTables(fileName, "_CPC464");

            WriteZ80Common();

            Environment.CurrentDirectory = oldDir;
        }



        public void ConvertSpectrum(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            //get the file path 
            CreateTables(fileName, "_Spectrum");

            WriteZ80Common();

            Environment.CurrentDirectory = oldDir;
        }
      
        void WriteZ80Common()
        {
            WriteWelcomeMessage("WelcomeZ80.asm", "DB", ",0h");
            WriteStringTableZ80("StringTableZ80.asm", "string_table", descriptionTable);
            WriteStringTableZ80("DictionaryZ80.asm", "dictionary", dict);
            WriteStringTableZ80("NogoTableZ80.asm", "nogo_table", nogoTable);
            WriteStringTableZ80("PrepTableZ80.asm", "prep_table", prepTable);
            WriteObjectTableZ80("ObjectTableZ80.asm");
            WriteObjectWordTable("ObjectWordTableZ80.asm", "DB");
            WriteVerbTableZ80("VerbTableZ80.asm");
            WriteCheckTable("CheckRulesZ80.asm", "DB", "DW");
            WriteSentenceTable("Z80", "before", "DB", "DW");
            WriteSentenceTable("Z80", "instead", "DB", "DW");
            WriteSentenceTable("Z80", "after", "DB", "DW");
            WriteEvents(doc, "Z80", new CLL.VisitorZ80(this));
            WriteUserVarTable(doc, "Z80");
         }


        public void Convert8086(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;
            CreateTables(fileName, "_8086");

            WriteWelcomeC();
            WriteStringTableC("Strings.h", "StringTable" , descriptionTable);
            WriteStringTableC("NogoTable.h", "NogoTable", nogoTable);
            WriteStringTableC("PrepTable.h", "PrepTable", prepTable);
            WriteStringTableC("Dictionary.h", "Dictionary", dict);
            WriteObjectTableC();
            WriteObjectWordTableC();
            WriteVerbTableC();
            WriteCheckTableC();
            WriteUserVarsC();
            WriteSentenceTableC("BeforeTable.c", "BeforeTable", "before");
            WriteSentenceTableC("AfterTable.c","AfterTable","after");
            WriteSentenceTableC("InsteadTable.c","InsteadTable","instead");
            WriteEventsHeader(doc, CDECLYesNo.NO);

            /* 8086 assembly */
            WriteEvents(doc, "8086", new CLL.Visitor8086(this));

            Environment.CurrentDirectory = oldDir;
        }


        public void ConvertRPi(string fileName)
        {
            string oldDir = ".";

            try
            {
                oldDir = Environment.CurrentDirectory;
            } 
            catch (Exception e)
            {
                throw new Exception("Unable to get current directory", e);
            }

            CreateTables(fileName, "_RPi");

            WriteWelcomeC();
            WriteStringTableC("Strings.h", "StringTable", descriptionTable);
            WriteStringTableC("NogoTable.h", "NogoTable", nogoTable);
            WriteStringTableC("PrepTable.h", "PrepTable", prepTable);
            WriteStringTableC("Dictionary.c", "Dictionary", dict);
            WriteObjectTableC();
            WriteObjectWordTableC();
            WriteVerbTableC(FOUR_BYTE_POINTERS);
            WriteCheckTableC(FOUR_BYTE_POINTERS);
            WriteUserVarsC();
            WriteSentenceTableC("BeforeTable.c", "BeforeTable", "before", FOUR_BYTE_POINTERS);
            WriteSentenceTableC("AfterTable.c", "AfterTable", "after", FOUR_BYTE_POINTERS);
            WriteSentenceTableC("InsteadTable.c", "InsteadTable", "instead",FOUR_BYTE_POINTERS);
            WriteEventsHeader(doc, CDECLYesNo.NO);
            WriteEvents(doc, "8086", new CLL.VisitorRPi(this));

            WriteRPiBuildScript();

            Environment.CurrentDirectory = oldDir;
        }
/*
        public void ConvertX64(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;
            CreateTables(fileName, "_x64");

            WriteWelcomeC();
            WriteStringTableC("Strings.h", "StringTable", descriptionTable);
            WriteStringTableC("NogoTable.h", "NogoTable", nogoTable);
            WriteStringTableC("PrepTable.h", "PrepTable", prepTable);
            WriteStringTableC("Dictionary.c", "Dictionary", dict);
            WriteObjectTableC();
            WriteObjectWordTableC();
            //WriteVerbTableC(EIGHT_BYTE_POINTERS);
            //WriteCheckTableC(EIGHT_BYTE_POINTERS);
            WriteVerbTableC(FOUR_BYTE_POINTERS);
            WriteCheckTableC(FOUR_BYTE_POINTERS);
            WriteUserVarsC();
            WriteSentenceTableC("BeforeTable.c", "BeforeTable", "before", FOUR_BYTE_POINTERS);
            WriteSentenceTableC("AfterTable.c", "AfterTable", "after", FOUR_BYTE_POINTERS);
            WriteSentenceTableC("InsteadTable.c", "InsteadTable", "instead", FOUR_BYTE_POINTERS);
            WriteEventsHeader(doc, CDECLYesNo.NO);
            WriteEvents(doc, "8086", new CLL.VisitorRPi(this));
            Environment.CurrentDirectory = oldDir;
        }
*/
        private void WriteStringTable6809(string fileName, string header, Table t)
        {

            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; " + fileName);
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");
                sw.WriteLine(header);
                for (int i = 0; i < t.GetNumEntries(); i++)
                {
                    if (t.GetEntry(i).Length > 0) //safety check
                    {
                        sw.WriteLine("\t.db " + t.GetEntry(i).Length);
                        sw.WriteLine("\t.strz \"" + t.GetEntry(i).ToUpper() + "\" ; " + i);
                    }
                }
                sw.WriteLine("\t.db 0");

            }
             
        }

        private void WriteStringTable6502(string fileName, string header, Table t, bool ucase=true)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; " + fileName);
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");
                sw.WriteLine(header);
                for (int i = 0; i < t.GetNumEntries(); i++)
                {
                    if (t.GetEntry(i).Length > 0) //safety check
                    {
                        sw.WriteLine("\tDB " + t.GetEntry(i).Length);
                        string s = t.GetEntry(i);
                        s = s.Replace("'", "',27,'");  //fix single quotes
                        if (ucase)
                            s = s.ToUpper();
                        
                        sw.WriteLine("\tASC '" + s + "' ; " + i);
                        sw.WriteLine("\tDB 0 ; null terminator");
                    }
                }
                sw.WriteLine("\tDB 0");

            }

        }
        private void WriteStringTableZ80(string fileName, string header, Table t)
        {

            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; " + fileName);
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");
                sw.WriteLine(header);
                for (int i = 0; i < t.GetNumEntries(); i++)
                {
                    if (t.GetEntry(i).Length > 0) //safety check
                    {
                        sw.WriteLine("\tDB " + t.GetEntry(i).Length);
                        sw.WriteLine("\tDB \"" + t.GetEntry(i) + "\" ; " + i);
                        sw.WriteLine("\tDB 0 ; null terminator");
                    }
                }
                sw.WriteLine("\tDB 255 ; end of table");
            }
        }

        private void WriteObjectTableZ80(string fileName)
        {
            WriteObjectTable(fileName, "DB","+");
        }

        private void WriteObjectTable6809(string fileName)
        {
            WriteObjectTable(fileName, ".db", "|");
        }

        private void WriteObjectTable6502(string fileName)
        {
            WriteObjectTable(fileName, "DB", "+");
        }

        //orsym is either | or +
        private void WriteObjectTable(string fileName, string byteSep, string orSym)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; OBJECT_TABLE");
                sw.WriteLine("; FORMAT: ID,HOLDER,INITIAL DESC,DESC,N,S,E,W,NE,SE,SW,NW,UP,DOWN,OUT,MASS");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");

                sw.WriteLine("ObjTblSize " + byteSep + " " + objects.Count);
                sw.WriteLine("obj_table");

                foreach (GameObject o in objects)
                {
                    WriteObjectAttrs(o,sw,byteSep);
                    WriteObjectProps(o, sw, byteSep, orSym);

                }//end each obj
        
                sw.WriteLine("\t" + byteSep + " 255  ; end of array indicator");
             
            }
        }

        
        private void WriteObjectAttrs(GameObject o, StreamWriter sw, String byteSep)
        {
                    //write the data bytes
                    sw.Write("\t" + byteSep + " ");
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
                    sw.Write("   ; " + o.name);

        }


        private void WriteObjectProps(GameObject o, StreamWriter sw, string byteSep, string orSym)
        {
                    sw.WriteLine("");

                    List<string> flags = new List<string>();

                    //first 8 flags
                    for (int i = 0; i < 8; i++)
                    {
                        bool val = o.GetXmlFlag(GameObject.xmlFlagNames[i]);
                        if (val)
                        {
                            flags.Add(asmFlagNames[i]);
                        }
                    }

                    string flagsStr = "";
                    if (flags.Count == 0) { sw.WriteLine("\t" + byteSep + " 0    "); } //;  flags 1 - 8
            else
                    {
                        for (int i = 0; i < flags.Count; i++)
                        {
                            if (i != 0) { flagsStr += orSym; }
                            flagsStr += flags[i];
                        }
                        sw.WriteLine("\t" + byteSep + " "  + flagsStr); // + " ; flags 1-8"
            }


                    //second 8 flags
                    flags.Clear();

                    for (int i = 8; i < GameObject.xmlFlagNames.Length; i++)
                    {
                        bool val = o.GetXmlFlag(GameObject.xmlFlagNames[i]);
                        if (val)
                        {
                            flags.Add(asmFlagNames[i]);
                        }
                    }

                    flagsStr = "";
                    if (flags.Count == 0) { sw.WriteLine("\t" + byteSep + " 0 "  ); } /*  ;  flags 9 - 16" */
                    else
                    {
                        for (int i = 0; i < flags.Count; i++)
                        {
                            if (flagsStr.Length > 0) { flagsStr += orSym; }
                            flagsStr += flags[i];
                        }
                        sw.WriteLine("\t"+ byteSep + " " + flagsStr ); /* + " ; flags 9-16" */
                    }

        }

        private void WriteObjectWordTable(string fileName, string byteDirective=".DB")
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; OBJECT WORD TABLE");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine("NumObjects " + byteDirective + "  " + objects.Count);

                sw.WriteLine("");
                sw.WriteLine("obj_word_table");
                char[] delimiterChars = { ' ' }; 

                foreach (GameObject o in objects)
                {
                    sw.Write("\t" + byteDirective + " " + o.id);

                    string name = o.printedName;

                    string[] toks = name.Split(delimiterChars);
                    List<string> words = new List<string>();
                    foreach (string s in toks)
                    {
                        //don't filter preps or the full name won't print right
                        words.Add(s);
                    }

                    int blanks = 3 - words.Count;

                    for (int i = 0; i < Math.Min(words.Count,3); i++)
                    {
                        //look up the id of each word

                        int wordId = dict.GetEntryId(words[i]);
                        sw.Write("," + wordId);
                    }


                    //now append the blanks
                    for (int i = 0; i < blanks; i++)
                    {
                        sw.Write(",255");
                    }

                    sw.WriteLine("   ;   " + o.name);
                }//end write objs


                //now write any synonyms
                foreach (GameObject o in objects)
                {
                    if (o.synonyms.Count > 0 && o.synonyms[0] != "")
                    {
                        sw.Write("\t" + byteDirective + " " + o.id);
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

                        sw.WriteLine("   ;   synonyms for " + o.name);
                    }//end write synonyms
                }

                sw.WriteLine("\t" + byteDirective + " 255");
//                sw.WriteLine("obj_table_size\t" + byteDirective + " " + objects.Count);
            }
        }

       

        private void WriteVerbTable(string fileName)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; VerbTable6809.asm ");
                sw.WriteLine("; Machine Generated Verb Table");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");

                char[] seps = { ',' };


                for (int i = 0; i < verbs.GetNumEntries(); i++)
                {
                    string verb = verbs.GetEntry(i);
                    string[] toks = verb.Split(seps);
                    string verb_id = toks[0].ToLower().Replace(' ', '_') + "_verb_id";
                    sw.WriteLine(verb_id + " equ " + i);
                }

                sw.WriteLine("");
                sw.WriteLine("");

                sw.WriteLine("verb_table");


                for (int i = 0; i < verbs.GetNumEntries(); i++)
                {
                    string verb = verbs.GetEntry(i);

                    //split it up using commas
                    string[] toks = verb.Split(seps);

                    for (int j = 0; j < toks.Length; j++)
                    {
                        sw.WriteLine("\t.db " + i);
                        sw.WriteLine("\t.db " + toks[j].Length);
                        sw.WriteLine("\t.strz \"" + toks[j].ToUpper() + "\"");
                    }
                }

                sw.WriteLine("\t.db 0,0");
            }
        }


        private void WriteVerbTableZ80(string fileName)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; VerbTableZ80.asm ");
                sw.WriteLine("; Machine Generated Verb Table");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");

                char[] seps = { ',' };


                for (int i = 0; i < verbs.GetNumEntries(); i++)
                {
                    string verb = verbs.GetEntry(i);
                    string[] toks = verb.Split(seps);
                    string verb_id = toks[0].ToLower().Replace(' ', '_') + "_verb_id";
                    sw.WriteLine(verb_id + " equ " + i);
                }

                sw.WriteLine("");
                sw.WriteLine("");

                sw.WriteLine("verb_table");


                for (int i = 0; i < verbs.GetNumEntries(); i++)
                {
                    string verb = verbs.GetEntry(i);

                    //split it up using commas
                    string[] toks = verb.Split(seps);

                    for (int j = 0; j < toks.Length; j++)
                    {
                        sw.WriteLine("\tDB " + i);
                        sw.WriteLine("\tDB " + toks[j].Length);
                        sw.WriteLine("\tDB \"" + toks[j].ToUpper() + "\"");
                        sw.WriteLine("\tDB 0 ; null");
                    }
                }

                sw.WriteLine("\tDB 255");
            }
        }


        private void WriteVerbTable6502(string fileName)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; VerbTable6502.asm ");
                sw.WriteLine("; Machine Generated Verb Table");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");

                char[] seps = { ',' };


                for (int i = 0; i < verbs.GetNumEntries(); i++)
                {
                    string verb = verbs.GetEntry(i);
                    string[] toks = verb.Split(seps);
                    string verb_id = toks[0].ToLower().Replace(' ', '_') + "_verb_id";
                    //       sw.WriteLine("#define " + verb_id + " " + i);
                    sw.WriteLine(verb_id + " EQU " + i);
                }

                sw.WriteLine("");
                sw.WriteLine("");

                sw.WriteLine("verb_table");


                for (int i = 0; i < verbs.GetNumEntries(); i++)
                {
                    string verb = verbs.GetEntry(i);

                    //split it up using commas
                    string[] toks = verb.Split(seps);

                    for (int j = 0; j < toks.Length; j++)
                    {
                        sw.WriteLine("\tDB " + i);
                        sw.WriteLine("\tDB " + toks[j].Length);
                        sw.WriteLine("\tASC '" + toks[j].ToUpper() + "'");
                        sw.WriteLine("\tDB 0 ; null");
                    }
                }

                sw.WriteLine("\tDB 255");
            }
        }
        /*Write each event to a separate file,
         * then writes out one file that includes all of them
         */
        private void WriteEvents(XmlDocument doc, string processor, CLL.IVisitor asm)
        {
            List<String> eventFiles = new List<string>();
            XmlNodeList events = doc.SelectNodes("//project/events/event");
            string extension = ".asm";
            if (processor == "8086" || processor == "6809")
                extension = ".c";
            else if (processor == "6502")
                extension = ".s";

            string sep="_";
            string processr = processor;
            if (extension == ".c")
            {
                processr = "";
                sep = "";
            }

            foreach (XmlNode n in events)
            {
                //
                string name = n.Attributes.GetNamedItem("name").Value;
                //put name in list
                string fileName = name.Replace(' ', '_');

                eventFiles.Add(fileName + "_event");

                // AsmWriter6809 asm = new AsmWriter6809();
                using (StreamWriter sw = File.CreateText(fileName + "_event" + sep + processr + extension))
                {
                    string innerText = n.InnerText;
                    string noComments = Regex.Replace(innerText, "//.*", "");
                    noComments = Regex.Replace(noComments, "\"\"", "\" \"");

                    try
                    {
                        CLL.Function func = CLL.FunctionBuilder.BuildFunction(this, name + "_event", noComments);
                        asm.SetStreamWriter(sw);
                        func.Accept(asm);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error writing event " + name + ": " + e.Message, e);
                    }

                }
            }

            XmlNodeList subs = doc.SelectNodes("//project/routines/routine");

            foreach (XmlNode n in subs)
            {

                string name = n.Attributes.GetNamedItem("name").Value;
                //put name in list
                string fileName = name.Replace(' ', '_');

                eventFiles.Add(fileName + "_sub");
//                AsmWriter6809 asm = new AsmWriter6809();
                using (StreamWriter sw = File.CreateText(fileName + "_sub" + sep + processr + extension))
                {
                    try
                    {
                        string innerText = n.InnerText;
                        string noComments = Regex.Replace(innerText, "//.*", "");


                        noComments = Regex.Replace(noComments, "\"\"", "\" \"");
                  //      asm.WriteRoutine(sw, name + "_sub", noComments);
                        CLL.Function func = CLL.FunctionBuilder.BuildFunction(this, name + "_sub", noComments);
                        asm.SetStreamWriter(sw);
                        func.Accept(asm);

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error Writing routine " + name + ": " + ex.Message , ex);
                    }
                }
            }


            //now write out the main include file
            string incFileName = "events6809.asm";
            if (processor.Equals("Z80"))
                incFileName = "EventsZ80.asm";
            else if (processor.Equals("6502"))
                incFileName = "Events6502.asm";
            else if (processor.Equals("8086"))
                incFileName = "Events.c";
            else if (processor.Equals("6809"))
                incFileName = "EventIncludes.h";

            using (StreamWriter sw = File.CreateText(incFileName))
            {
                if (processor != "8086" && processor != "6809")
                {
                    sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                    sw.WriteLine("; Machine generated include file");
                    sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                }
                else
                {
                    sw.WriteLine("/* Machine generated include file */");
                }
                foreach (string s in eventFiles)
                {
                    if (processor.Equals("6809") || processor.Equals("8086"))
                        sw.WriteLine("\t#include \"" + s + ".c\"");
                    else if (processor.Equals("Z80"))
                        sw.WriteLine("*INCLUDE " + s + "_Z80.asm");
                    else if (processor.Equals("6502"))
                        sw.WriteLine(".include \"" + s + "_6502.asm\""); 
                }
            }


            //now write out the jumps to the event routines

            using (StreamWriter sw = File.CreateText("event_jumps" + sep + processr  + extension))
            {
                if (processor == "8086" || processor == "6809")
                {
                    sw.WriteLine("/* jump to machine generated subroutines */");
                    sw.WriteLine("void run_events()");
                    sw.WriteLine("{");
                }
                else
                {
                    sw.WriteLine("; jump to machine generated subroutines");
                }

                foreach (string s in eventFiles)
                {
                    if (s.IndexOf("event") != -1)
                    {
                        //sw.WriteLine("\tjsr " + s);
                        asm.SetStreamWriter(sw);
                        asm.WriteEventCall(s);
                       // asm.WriteCall(s)
                    }
                }

                if (processor == "8086" || processor == "6809")
                {
                    sw.WriteLine("}");
                }
            }

        }

        /*version 2*/
        
        private void WriteFunctionsAndEvents(XmlDocument doc, string processor, CLL.IVisitor asm)
        {
            List<String> eventFiles = new List<string>();
            XmlNodeList events = doc.SelectNodes("//project/events/event");

            foreach (XmlNode n in events)
            {
                //
                string name = n.Attributes.GetNamedItem("name").Value;
                //put name in list
                string fileName = name.Replace(' ', '_');

                eventFiles.Add(fileName + "_event");

                // AsmWriter6809 asm = new AsmWriter6809();
                using (StreamWriter sw = File.CreateText(fileName + "_event_" + processor + ".asm"))
                {
                    asm.SetStreamWriter(sw);

                    string innerText = n.InnerText;
                    string noComments = Regex.Replace(innerText, "//.*", "");
                    noComments = Regex.Replace(noComments, "\"\"", "\" \"");
                    //asm.WriteRoutine(sw, name + "_event", noComments);

                    //Function f = new Function(noComments);
                    
                    CLL.Function f = CLL.FunctionBuilder.BuildFunction(this, name + "_event", noComments);
                    //f.AcceptEmitter(asm);
                    asm.SetStreamWriter(sw);
                    f.Accept(asm);
                    
                }
            }

            XmlNodeList subs = doc.SelectNodes("//project/routines/routine");

            foreach (XmlNode n in subs)
            {

                string name = n.Attributes.GetNamedItem("name").Value;
                //put name in list
                string fileName = name.Replace(' ', '_');

                eventFiles.Add(fileName + "_sub");
                //                AsmWriter6809 asm = new AsmWriter6809();
                using (StreamWriter sw = File.CreateText(fileName + "_sub_" + processor + ".asm"))
                {
                    try
                    {
                        string innerText = n.InnerText;
                        string noComments = Regex.Replace(innerText, "//.*", "");


                        noComments = Regex.Replace(noComments, "\"\"", "\" \"");
                                                
//                        asm.WriteRoutine(sw, name + "_sub", noComments);
                        CLL.Function f = CLL.FunctionBuilder.BuildFunction(this, fileName, noComments);
                        asm.SetStreamWriter(sw);
                        f.Accept(asm);

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error Writing routine " + name, ex);
                    }
                }
            }


            //now write out the main include file
            string incFileName = "events6809.asm";
            if (processor.Equals("Z80"))
                incFileName = "EventsZ80.asm";
            else if (processor.Equals("6502"))
                incFileName = "Events6502.asm";

            using (StreamWriter sw = File.CreateText(incFileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; Machine generated include file");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");

                foreach (string s in eventFiles)
                {
                    if (processor.Equals("CoCo"))
                        sw.WriteLine("\tinclude " + s + "_CoCo.asm");
                    else if (processor.Equals("Z80"))
                        sw.WriteLine("*INCLUDE " + s + "_Z80.asm");
                    else if (processor.Equals("6502"))
                        sw.WriteLine(".include \"" + s + "_6502.asm\"");

                }
            }


            //now write out the jumps to the event routines
            using (StreamWriter sw = File.CreateText("event_jumps_" + processor + ".asm"))
            {
                sw.WriteLine("; jump to machine generated subroutines");

                foreach (string s in eventFiles)
                {
                    if (s.IndexOf("event") != -1)
                    {
                        //sw.WriteLine("\tjsr " + s);
                        asm.SetStreamWriter(sw);
                        asm.WriteEventCall(s);
                    }
                }


            }

        }


        public int GetStringId(string text)
        {
            return descriptionTable.GetEntryId(text);
        }

        public int GetFailStringId(string s)
        {
            int strId = nogoTable.GetEntryId(s);

            if (strId == -1)
            {
                throw new Exception("String \'" + s + "\' not found in table");
            }

            return 256 - strId;
        }

        public bool IsSubroutine(string name)
        {
            if (name.IndexOf("(") == -1)
            {
                return false;
            }

            string trimmed = name.Substring(0, name.IndexOf("("));
            return routineNames.Contains(trimmed + "_sub");
        }


        public int GetObjectId(string name)
        {
            name = name.Trim().ToUpper();

            //if it's a numeric constant, use that
            int val = -1;
            if (Int32.TryParse(name, out val))
            {
                return val;
            }

            if (name == "") { return 255; }
            if (name == "*") { return 254; }

            foreach (GameObject o in objects)
            {
                if (o.name.ToUpper().Equals(name))
                {
                    return o.id;
                }
            }
            return -1;               
        }


        /*type should be "before" "instead" or "after"
         */
        private void WriteSentenceTable(string processorType, string type, string byteDef, string wordDef)
        {
            XmlNodeList subs = doc.SelectNodes("//project/sentences/sentence");
            string suffix = ".asm";
            if (processorType == "6502")
                suffix = ".s";

            using (StreamWriter sw = File.CreateText(type + "_table_" + processorType + suffix))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; " + type + "_table_" + processorType + ".asm");
                sw.WriteLine("; Machine Generated Sentence Table");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");

                if (type.Equals("before"))
                    sw.WriteLine("preactions_table");
                else if (type.Equals("instead"))
                    sw.WriteLine("actions_table");
                else if (type.Equals("after"))
                    sw.WriteLine("postactions_table");
                else
                    throw new Exception("Invalid sentence type.");

                foreach (XmlNode s in subs)
                {
                    string tp = s.Attributes.GetNamedItem("type").Value;

                    if (tp.Equals(type))
                    {
                        string verb = s.Attributes.GetNamedItem("verb").Value;
                        int verbId = GetVerbId(verb);

                        string prep = s.Attributes.GetNamedItem("prep").Value;
                        int prepId = prepTable.GetEntryId(prep);
                        if (prepId == -1)
                        {
                            prepId = 255;
                            // throw new Exception("Unknown prep \"" + prep + "\" in sentence that starts with: " + verb );
                        }

                        string doObj = s.Attributes.GetNamedItem("do").Value;
                        int doId = GetObjectId(doObj);

                        string ioObj = s.Attributes.GetNamedItem("io").Value;
                        int ioId = GetObjectId(ioObj);

                        sw.WriteLine("\t" + byteDef + " " + verbId + "," + doId + "," + prepId + "," + ioId + "\t;" + verb + " " + doObj + " " + prep + " " + ioObj);

                        string subName = s.Attributes.GetNamedItem("sub").Value;
                        subName += "_sub";

                        if (processorType.Equals("6502"))
                            subName = CLL.Visitor6502.TruncateName(subName);
                        sw.WriteLine("\t" + wordDef + " " + subName);
                    }
                }

                sw.WriteLine("\t" + byteDef + " 255");
                sw.WriteLine("");

            }
        }




        //writes out the check rules
        private void WriteCheckTable(string fileName, string byteSep, string wordSep)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {

                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; check rules table");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");

                XmlNodeList checks = doc.SelectNodes("//project/checks/check");
                sw.WriteLine("");
                sw.WriteLine("check_table");

                foreach (XmlNode c in checks)
                {
                    string verb = c.Attributes.GetNamedItem("verb").Value;
                    

                    if (verb.IndexOf(",")!=-1)
                    {//if there are synonyms, get 1st
                        verb = verb.Substring(0, verb.IndexOf(","));
                    }
                    
                    int verbId = GetVerbId(verb);

                    if (verbId != -1)
                    {
                        string subName = c.Attributes.GetNamedItem("check").Value;
                        sw.WriteLine("\t" + byteSep + " " + verbId + " ; " + verb);
                        sw.WriteLine("\t" + wordSep + " " + subName);
                    }
                    else
                    {
                        throw new Exception("Exception while writing check table. Unknown verb: " + verb);
                    }
                }
                sw.WriteLine("\t" + byteSep + " 255");
            }
        }

        void WriteWelcomeMessage(string fileName, string strDelim, string nullByte = "", bool escapeTicks = false)
        {
            string quot = "\"";
            if (escapeTicks)
                quot = "'";

            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; welcome message include file");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");

                XmlNodeList list = doc.SelectNodes("//project/welcome");
                sw.WriteLine("");
                sw.WriteLine("welcome " + strDelim + "  " + quot + EscapeTicks(list[0].InnerText, escapeTicks) + quot + nullByte);
                list = doc.SelectNodes("//project/author");
                sw.WriteLine("author " + strDelim + " " + quot + EscapeTicks(list[0].InnerText, escapeTicks) + quot + nullByte);
                list = doc.SelectNodes("//project/version");
                sw.WriteLine("version " + strDelim + " " + quot + EscapeTicks(list[0].InnerText, escapeTicks) + quot + nullByte);
            }
        }

        string EscapeTicks(string s, bool yesNo)
        {
            if (yesNo)
                return s.Replace("'", "',27,'");
            else
                return s;
        }

        int GetVerbId(string v)
        {
            char[] seps = { ',' };

            for (int i = 0; i < verbs.GetNumEntries(); i++)
            {
                string s = verbs.GetEntry(i);
                string[] toks = s.Split(seps);

                foreach (string t in toks)
                {
                    if (t.ToUpper().Equals(v.ToUpper()))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public  bool IsVariable(string s)
        {
            return varTable.Keys.Contains(s);
        }

        public bool IsFunction(string s)
        {
            return routineNames.Contains(s);
        }

        int GetObjId(string oname)
        {

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].name.ToUpper().Equals(oname.ToUpper()))
                {
                    return i;
                }
            }

            return -1;
        }


       

        void WriteUserVarTable(XmlDocument doc, string processor)
        {
            string suffix = ".asm";
            if (processor == "6502")
                suffix = ".s";

            using (StreamWriter sw = File.CreateText("UserVars" + processor+ suffix))
            {

                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("; User variables");
                sw.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                sw.WriteLine("");
                string delim = "DB";

                if (processor.Equals("6809"))
                    delim = ".db";
                else if (processor.Equals("6502"))
                    delim = "DB";


                sw.WriteLine("NumUserVars\t" + delim + " " + userVars.Count);
                sw.WriteLine("UserVars");
                foreach (UserVar v in userVars)
                {
                   sw.WriteLine(v.name + "\t" + delim +  " " + v.initialVal);
                }

                sw.WriteLine("NumRecs\t" + delim + " " + (((objects.Count * 19 + varTable.Count + userVars.Count) / 128) + 1) + " ; for CP/M save");

            }
             
        }
 
  
        public string GetVarAddr(string varName)
        {
            if (varTable.Keys.Contains(varName))
            {
                return varTable[varName];
            }
            else
            {
                throw new Exception("Unknown varibale: " + varName);
            }
        }

        /*Creates the directory (if needed) and copies 
         * code in from the appropriate common folder
         */
        void CreateOutputDir(string tgtPlatform)
        {
            XmlNodeList list = doc.SelectNodes("//project/projname");            
            
            if (list.Count == 0)
            {
                throw new Exception("Project XML does not have a name element!");
            }

            string workingDirectory = list[0].InnerText + tgtPlatform;
            

            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }

            Environment.CurrentDirectory = workingDirectory;
            buildDir = Environment.CurrentDirectory;
            //copy skeleton into working directory

            string skelDir = skelDirs[tgtPlatform];
            CopyFiles(".."  + Path.AltDirectorySeparatorChar + skelDir,".");

            string pltfDir = "";
            //copy platform specific files into working dir
            try
            {
                pltfDir = pltfDirs[tgtPlatform];
            }
            catch
            {
                throw new Exception("Unable to find common folder for " + tgtPlatform);
            }

            CopyFiles(".." + Path.AltDirectorySeparatorChar + pltfDir, ".");

            
        }


        public void ConvertApple2(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            try
            {

                //get the file path 
                CreateTables(fileName, "_Apple2");
                Common6502Export();
                FixBuildScript(doc);
            }
            finally
            {
                Environment.CurrentDirectory = oldDir;
            }

        }

        public void ConvertBBCMicro(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            try
            {

                //get the file path 
                CreateTables(fileName, "_BBCMicro");
                Common6502Export();
                WriteEvents(doc, "6502", new CLL.Visitor6502(this));
                WriteBBCLoader();
            }
            finally
            {
                Environment.CurrentDirectory = oldDir;
            }

        }


        public void ConvertC64(string fileName)
        {
            string oldDir = Environment.CurrentDirectory;

            try
            {

                //get the file path 
                CreateTables(fileName, "_C64");
                Common6502Export();
                //try to build
                FixBuildScript(doc);

            }
            finally
            {
                Environment.CurrentDirectory = oldDir;
            }

        }

        void ValidateStringLength()
        {
            for (int i = 0; i < descriptionTable.GetNumEntries(); i++ )
            {
                string s = descriptionTable.GetEntry(i);
                if (s.Length > 255)
                {
                    throw new Exception("The following string exceeds 255 characters: \r\n\"" + s + "\"");
                }
            }
        }

        bool IsPrepOrArticle(string s)
        {
            if (noiseWords.Contains(s.ToUpper()))
            {
                return true;
            }
            return false;
        }

        void WriteBBCLoader()
        {
            Process p = Process.Start("build.bat");
            p.WaitForExit();

            FileInfo fi = new FileInfo("advdata");
            int len = (int)fi.Length;
            int start = 8192;
            int end = len + 6400; //6400 = 1900h (start addr)

            using (StreamWriter sw = File.CreateText("load.txt"))
            {
                sw.WriteLine("5 PRINT \"LOADING GAME FILE\"");
                sw.WriteLine("10 *LOAD ADVDATA 2000");
                sw.WriteLine("15 PRINT \"SAVING GAME FILE AS ADVGAME\"");
                sw.WriteLine("20 *SAVE ADVGAME {0:X} {1:X} 1900 1900",start,end);
                sw.WriteLine("25 PRINT \"D.N Access Load Execution Length Start\"");
                sw.WriteLine("30 *INFO ADVGAME");
                sw.WriteLine("35 PRINT \"DONE\"");
            }

            
            string endaddr = "" + end % 256 + "," + end / 256;
            string cmd = "sed s'/ENDADDR/" + endaddr + "/g savedat.asm > save.asm";
            File.WriteAllText("loadr.bat",cmd);

        }

        void WriteRPiBuildScript()
        {
            using (StreamWriter sw = File.CreateText("build.sh"))
            {
                sw.WriteLine("#!/bin/bash");
                sw.WriteLine("g++ main.c");
                XmlNodeList list = doc.SelectNodes("//project/projname");
                string name = list[0].InnerText;
                sw.WriteLine("mv ./a.out ./" + name);
                sw.WriteLine("echo \"" + name + " compiled\"");
            }
        }
         
        void CopyFiles(string fromDir, string toDir )
        {
            string[] files = Directory.GetFiles(fromDir);

            foreach (string f in files)
            {
                //                string noPath = f.Substring(f.LastIndexOf(Path.AltDirectorySeparatorChar) +1); 
                string noPath =  Path.GetFileName(f);
                File.Copy(fromDir + Path.AltDirectorySeparatorChar + f, toDir + Path.AltDirectorySeparatorChar + noPath, true); 
            }
            
        }

        /// <summary>
        /// replaces the text __EVENTS__ with a list of the files to include
        /// This needs to be done because Merlin does not like multiple levels
        /// of include files
        /// </summary>
        void InsertEvents6502(XmlDocument xmlDoc)
        {
            string includedFiles = "";

            XmlNodeList events = doc.SelectNodes("//project/events/event");
             
            foreach (XmlNode n  in events)
            {
                string s = n.Attributes.GetNamedItem("name").Value;
                includedFiles += "\tput " + s + "_event_6502\r\n";
            }

            events = doc.SelectNodes("//project/routines/routine");

            foreach (XmlNode n in events)
            {
                string s = n.Attributes.GetNamedItem("name").Value;
                includedFiles += "\tput " + s + "_sub_6502\r\n";
            }

            includedFiles += "\r\n";


            //now open main, read it into a buffer, then replace ___INCLUDES__ with the put statements

            string main = File.ReadAllText("main.asm");

            StringBuilder sb = new StringBuilder(main);
            sb.Replace("__INCLUDES__", includedFiles);
            File.WriteAllText("main.s", sb.ToString());
        }

        /*
         * Inserts jsr statements in the do_events
         * This is because Merlin doesn't handle
         */

        void InsertEventJumps6502(XmlDocument xmlDoc)
        {
            string jumps = "";

            XmlNodeList events = doc.SelectNodes("//project/events/event");

            foreach (XmlNode n in events)
            {
                string s = n.Attributes.GetNamedItem("name").Value;
                jumps += "\tjsr " + s + "_event\r\n";
            }

            events = doc.SelectNodes("//project/routines/routine");
            
            //now insert the jump statements
            string main = File.ReadAllText("doevents6502.asm");
            main = main.Replace("__EVENT_JUMPS__", jumps);
            File.WriteAllText("doevents6502.s", main);
        }


        /// <summary>
        /// Writes out tables for a 6502 based computers
        /// </summary>
        void Common6502Export()
        {
            WriteWelcomeMessage("Welcome6502.s", "\tASC", "\n\tDB 0\n", true);
            WriteStringTable6502("StringTable6502.s", "string_table", descriptionTable, false);
            WriteStringTable6502("Dictionary6502.s", "dictionary", dict, false);
            WriteStringTable6502("NogoTable6502.s", "nogo_table", nogoTable, false);
            WriteStringTable6502("PrepTable6502.s", "prep_table", prepTable);
            WriteObjectTable6502("ObjectTable6502.s");
            WriteObjectWordTable("ObjectWordTable6502.s", "\tDB");
            WriteVerbTable6502("VerbTable6502.s");
            WriteCheckTable("CheckRules6502.s", "\tDB", "\tDW");
            WriteSentenceTable("6502", "before", "DB", "DW");
            WriteSentenceTable("6502", "instead", "DB", "DW");
            WriteSentenceTable("6502", "after", "DB", "DW");
            WriteUserVarTable(doc, "6502");           // WriteEvents(doc, "6502", new AsmWriter6809());

            WriteEvents(doc, "6502", new CLL.Visitor6502(this));
            InsertEvents6502(doc);
            InsertEventJumps6502(doc);
        }

        void FixBuildScript(XmlDocument doc)
        {
            string dskName = doc.SelectNodes("//project/output")[0].InnerText;
            if (dskName == "")
                dskName = "adventure";

            //fix Linux
            string s = File.ReadAllText("build.sh");
            s = s.Replace("__DISK_NAME__", dskName);
            File.WriteAllText("build.sh", s);

            //fix DOS
            s = File.ReadAllText("build.bat");
            s = s.Replace("__DISK_NAME__", dskName);
            File.WriteAllText("build.bat", s);
        }

    }//end class
}
