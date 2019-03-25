using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XTAC;
using XMLtoAdv;
//n,n,push tree,n,n,n,e,ne,w,w,get leaves,w,s,s,get gunk,n,pick lock,e,open safe,get coin,open desk,get lighter,w,n,w,w,n,open box,put coin in box,s,e,s,w,get gloves,wear gloves,wear gloves,e,n,n,get wrench,s,w,n,put coin in recess,s,w,nw,drop leaves,burn leaves,se,sw,get crate,ne,nw,u,u,drop crate,d,d,get gold,u,u,u,unbolt cupola,u
namespace PlayerLib
{
    partial class Game
    {
        delegate void DfltSentence();
        bool debug = false;
        Xml gameData;
        ObjTable objTable;
        Table stringTable = new Table();
        Table nogoTable = new Table();
        Table verbTable = new Table(); //raw verbs
        Dictionary<string, int> verbMap = new Dictionary<string, int>();
        Table dictionary = new Table();
        ObjWordTable objWordTable = new ObjWordTable();
        const int OFFSCREEN = 0; 
        const int PLAYER = 1;
        
        List<string> articleTable = new List<String>();
        List<string> prepTable = new List<String>();
        List<Tuple<int, DfltSentence>> defaultSentences = new List<Tuple<int, DfltSentence>>();
        List<Sentence> before = new List<Sentence>();
        List<Sentence> instead = new List<Sentence>();
        List<Sentence> after = new List<Sentence>();
        Dictionary<string, Function> functions = new Dictionary<string, Function>();
        List<Function> events = new List<Function>();
        Dictionary<string, Variable> vars = new Dictionary<string, Variable>();
        TextBox outputWindow;
        NameTable nameTable = new NameTable();
        string welcome;
        string author;
        string version;

        int verb;
        int dobj;
        int prep;
        int iobj;

        static Game _game;

        private Game() {
            articleTable.Add("A");
            articleTable.Add("AN");
            articleTable.Add("THE");

            

            prepTable.Add("IN");
	        prepTable.Add("AT");
        	prepTable.Add("TO");
        	prepTable.Add("INSIDE");
        	prepTable.Add("OUT");
        	prepTable.Add("UNDER");
        	prepTable.Add("ON");
            prepTable.Add("OFF");
            prepTable.Add("INTO");
            prepTable.Add("UP");
            prepTable.Add("WITH");
            
        }


        public static Game GetInstance()
        {
            if (_game == null)
            {
                _game = new Game();
            }
            return _game;
        }

        public void SetGameData(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            GetIntro(doc);

            BuildStringTable(doc);
            

            BuildNogoTable(doc);
            BuildVerbTable(doc);

            PopulateObjectAndDictionary(doc);
            BuildObjWordTable();

            //this.gameData = data;

            BuildDefaultSentenceMap();
            LoadVars(doc);
            BuildSentences(doc);
            BuildFunctions(doc);
            BuildCheckTable();
        }

        public void SetOutputWindow(TextBox tb)
        {
            outputWindow = tb;
        }

        void PrintCr()
        {
            outputWindow.Text += "\r\n";
        }

        public void PrintString(int strId)
        {
            string s = stringTable.GetEntry(strId);
            outputWindow.Text += s;
        
        }

        public void PrintStringCr(int strId)
        {
            string s = stringTable.GetEntry(strId);
            outputWindow.Text += s + "\r\n";
        }


        public void PrintString(string s)
        {
            outputWindow.Text += s;
        }

        public void PrintStringCr(string s)
        {
            outputWindow.Text += s + "\r\n";
            outputWindow.SelectionStart = outputWindow.TextLength;
            outputWindow.ScrollToCaret();
        }

        public int GetObjectId(string name)
        {
            if (name == "")
                return -1;
            if (name == "*")
                return 254;

            int i = 0;
            foreach (ObjTableEntry o in objTable.objects.Values)
            {
                if (o.name.ToUpper().Equals(name.ToUpper()))
                {
                    return i;
                }
                i++;
            }

            return -1;
        }

        public void SetObjectAttr(int id, string name, int val)
        {
            objTable.SetObjAttr(id, name, val);
        }

  
        public int GetObjectAttr(int id, string name)
        {
            return objTable.GetObjAttr(id, name);
        }

        public int GetObjectProp(int id, string name)
        {
            return objTable.GetObjAttr(id,name);
        }

         
        public void Start()
        {

        }


       

        public void Look()
        {

            if (PlayerCanSee())
            {
                int r = GetPlayerRoom();
                PrintObjectName(r);
                PrintCr();
                PrintStringCr(objTable.GetObjAttr(r, "DESCRIPTION"));
                ListRoomObjects();

            }
            else
            {
                PrintStringCr("IT IS PITCH DARK.");
            }
        }


        public void LookAt()
        {
            PrintStringCr(objTable.GetObjAttr(dobj, "DESCRIPTION"));
     

        }

        public void Move()
        {
            string dir = VerbToDir();

            ObjTableEntry curRoom = objTable.GetObj(GetPlayerRoom());
            int newRoom = curRoom.GetObjAttr(dir);
            if (newRoom < 127)
            {
                ObjTableEntry newr = objTable.GetObj(newRoom);
                if (newr.GetObjAttr("DOOR") == 1)
                {
                    if (newr.GetObjAttr("OPEN") == 0)
                    {
                        PrintStringCr("THE " + newr.name + " IS CLOSED.");
                        return;
                    }
                    else
                    {
                        newRoom = GetObjectAttr(newRoom, dir);
                    }
                }

                objTable.SetObjAttr(PLAYER, "HOLDER", newRoom);
                Look();
            }
            else
            {
                newRoom = 255-newRoom;
                PrintStringCr(nogoTable.GetEntry(newRoom+1));
            }
        }

        public void Get()
        {
            SetObjectAttr(dobj, "HOLDER", PLAYER);
            PrintStringCr("TAKEN.");
        }

        public void Drop()
        {
            SetObjectAttr(dobj, "HOLDER", GetPlayerRoom());
            PrintStringCr("DROPPED.");
        }

        public void Open()
        {
            SetObjectAttr(dobj, "OPEN", 1);

            if (GetObjectAttr(dobj, "DOOR")==0)
            {
                if (HasVisibleItems(dobj))
                {
                    PrintString("OPENING THE ");
                    PrintObjectName(dobj);
                    PrintStringCr(" REVEALS:");
                    ListContents(dobj);
                }
                else
                {
                    PrintStringCr("OPENED.");
                }
            }
            else
            {
                PrintStringCr("OPENED.");
            }
        }

        public void Close()
        {
            SetObjectAttr(dobj, "OPEN", 0);
            PrintStringCr("CLOSED.");

        }


        public void Enter()
        {
            if (GetObjectAttr(dobj, "IN") != 255)
            {
                SetObjectAttr(PLAYER, "HOLDER", GetObjectAttr(dobj, "IN"));
                Look();
            }
            else
            {
                PrintStringCr("YOU CAN'T ENTER THAT.");
            }
        }

        public void Put()
        {
            SetObjectAttr(dobj, "HOLDER", iobj);
            PrintStringCr("DONE.");
        }

        public void Wear()
        {
            SetObjectAttr(dobj, "BEINGWORN", 1);
        }

        public bool PlayerCanSee()
        {
            int r = GetPlayerRoom();

            if (GetObjectAttr(r, "LIT") == 1)
                return true;

            for (int i=2; i < objTable.objects.Count; i++)
            {
                if (GetObjectAttr(i,"LIT")==1)
                {
                    if (IsVisibleToPlayer(i))
                        return true;
                }
            }

            return false;
        }

        public int GetPlayerRoom()
        {
            return objTable.GetObjAttr(1, "HOLDER");
        }

        void BuildStringTable(XmlDocument doc)
        {
            stringTable.Clear();
            stringTable.AddEntry("YOU NOTICE NOTHING UNEXPECTED.");

            XmlToTables x = XmlToTables.GetInstance();
            x.ParseForStrings(doc, stringTable);
        }

        void BuildNogoTable(XmlDocument doc)
        { 
            nogoTable.Clear();
            nogoTable.AddEntry("YOU NOTICE NOTHING UNEXPECTED.");

            XmlToTables x = XmlToTables.GetInstance();
            x.PopulateNogoTable(doc, nogoTable);
        }

        void BuildVerbTable(XmlDocument doc)
        {
            verbMap.Clear();
            XmlToTables x = XmlToTables.GetInstance();
            x.PopulateVerbTable(doc, verbTable);

            verbTable.Uppercase();

            for (int i=0; i < verbTable.GetNumEntries(); i++)
            {
                string s = verbTable.GetEntry(i);
                string[] verbs = s.Split(',');
                
                foreach (string v in verbs)
                {
                    verbMap[v] = i;
                }
            }

        }

        private void PopulateObjectAndDictionary(XmlDocument doc)
        {
            objTable = new ObjTable();
            stringTable.AddEntry("YOU NOTICE NOTHING UNEXPECTED.");

            XmlNodeList list = doc.SelectNodes("//project/objects/object");
             
            foreach (XmlNode n in list)
            {
                string name = n.Attributes.GetNamedItem("name").Value;
  
                //get the child node with the description
                XmlNode child = n.ChildNodes[0];
                string desc = child.InnerText;

                //don't add blank descriptions
                if (desc != "")
                {
                    stringTable.AddEntry(desc);
                }

                //initial desc
                string initialDesc = n.SelectSingleNode("initialdescription").InnerText;

                if (initialDesc != "" && initialDesc != null)
                {
                    stringTable.AddEntry(initialDesc);
                }

                //break the name into words and put each word in the dictionary
                char[] delimiterChars = { ' ' };
                string[] toks = name.Split(delimiterChars);

                foreach (string s in toks)
                {
                    if (s != null && !s.Equals(""))
                    {
                        dictionary.AddEntry(s);
                    }
                }

                //create the object from the data
                GameObject gobj = new GameObject(n);
                objTable.Add(new ObjTableEntry(gobj, stringTable));

                 
                //put synoyms in table
                foreach (string s in gobj.synonyms)
                {
                    if (!s.Equals(""))
                        dictionary.AddEntry(s);
                }

            }

        }

        void GetIntro(XmlDocument doc)
        {
            XmlNodeList list = doc.SelectNodes("//project/welcome");
            welcome = list[0].InnerText;

            list = doc.SelectNodes("//project/author");
            author = list[0].InnerText;

            list = doc.SelectNodes("//project/version");
            version = list[0].InnerText;
        }

        void PrintObjectName(int o)
        {
            PrintString(objTable.GetObj(o).GetObjName());
        }

        public void Run()
        {
            PrintStringCr(welcome);
            PrintStringCr(version);
            PrintStringCr(author);
            PrintCr();
            Look();

        }

        public void AcceptCommand(string command)
        {
            string[] commands = command.Split(',');

            foreach (string s in commands)
            {
                if (s.ToUpper().Equals("DEBUG"))
                {
                    debug = !debug;
                    return;
                }

                if (Parse(s.Trim()))
                {
                    if (MapWords())
                    {
                        RunCommand();
                    }
                }
            }
        }

        //make sure the nouns correspond to visible objects
        bool MapWords()
        {
            //dobj
            if (dobj != -1)
            {
                dobj = MapObject(dobj);
                if (dobj == -1)
                {
                    PrintStringCr("YOU DON'T SEE THAT.");
                    return false;
                }
            }

            if (iobj != -1)
            {
                iobj = MapObject(iobj);
                if (iobj == -1)
                {
                    PrintStringCr("YOU DON'T SEE THAT.");
                    return false;
                }
            } 
            return true;
        }

        int MapObject(int word)
        {
            objTable.ClearScores();
            if (word != -1)
            {
                for (int i = 0; i < objWordTable.GetCount(); i++)
                {
                    if (objWordTable.Matches(i, word))
                    {
                        if (IsVisibleToPlayer(i))
                        {
                            objTable.IncScore(i);
                        }
                    }
                } 
            }

            //take the dobj with the highest score
            return objTable.GetHighestScoringObj();

        }

        public void RunCommand()
        {
            if (RunChecks())
            {
                TryRun(before);

                if (!TryRun(instead))
                {
                    TryDefaultSentence();
                }

                TryRun(after);

                PrintCr();

                DoEvents();
            }
        }

        bool Parse(string command)
        {
            int prepIndex = -1;
            verb = -1;
            dobj = -1;
            prep = -1;
            iobj = -1;
            command = command.ToUpper();
            command = command.Trim();

            if (command.Length==0)
            {
                PrintStringCr("PARDON?");
                return false;
            }

            string[] words = command.Split(' ');
            List<string> list = words.ToList<string>();

            //remove articles;
            for (int i = 0; i < list.Count; i++)
            {
                if (articleTable.Contains(list[i]))
                {
                    list.RemoveAt(i);
                    i--;
                    break;
                }
            }

            //consolidate prep
            if (list.Count >= 2)
            {
                if (prepTable.Contains(list[1]))
                {
                    list[0] = list[0] + " " + list[1];
                    list.RemoveAt(1);
                }
            }

            //is verb valid
            if (!verbMap.Keys.Contains(list[0]))
            {
                PrintStringCr("I DON'T KNOW THE VERB '" + list[0] + "'");
                return false;
            }

            verb = verbMap[list[0]];

            if (list.Count > 1)
            {//get the dobj
                int dobjId = dictionary.GetEntryId(list[1]) ;
                if (dobjId== -1)
                {
                    PrintStringCr("I DON'T KNOW THE WORD '" + list[1] + "'");
                    return false;
                }
                dobj = dobjId;
            }

            //find the prep
            if (list.Count > 2)
            {
                for (int i=0; i < list.Count; i++)
                {
                    if (prepTable.Contains(list[i]))
                    {
                        prepIndex = i;
                        prep = prepTable.IndexOf(list[i]);
                        break;
                    }
                }
            }

            if (prepIndex != -1)
            {
                //get word at prep index +1
                if (prepIndex < list.Count)
                {
                    int iobjId = dictionary.GetEntryId(list[prepIndex+1]);
                    if (iobjId == -1)
                    {
                        PrintStringCr("I DON'T KNOW THE WORD '" + list[1] + "'");
                        return false;
                    }
                    iobj = iobjId;
                }
                else
                {
                    PrintStringCr("LOOKS LIKE YOU ARE MISSING AN INDIRECT OBJECT.");
                    return false;                 
                }
            }

            return true;
        }

        //is the object a descendant of the player or the player's room.
        bool IsVisibleToPlayer(int objId)
        {
            int playerRoom = GetPlayerRoom();

            while (true)
            {
                int parent = GetObjectAttr(objId, "HOLDER");
                if (parent == PLAYER || parent == playerRoom)
                    return true;
                if (parent == OFFSCREEN)
                    return false;
                if (IsClosedContainer(parent))
                    return false;

                objId = parent;
            }
        }

        bool IsClosedContainer(int objId)
        {
            if (GetObjectAttr(objId, "CONTAINER")==1)
            {
                if (GetObjectAttr(objId,"OPEN") == 1)
                    return false;
                return true;
            }
            return false;
        }
    
        void BuildObjWordTable()
        {
            for (int i = 0; i < objTable.GetCount(); i++ )
            {
                List<int> l = new List<int>();

                string n = objTable.objects[i].name;
                string[] words = n.Split(' ');

                for (int j = 0; j < words.Length; j++ )
                {
                    words[j] = words[j].ToUpper();
                }

                objWordTable.AddNewEntry(i,words.ToList<string>(), dictionary);
            }
        }

        void BuildDefaultSentenceMap()
        {
            defaultSentences.Add(
                new Tuple<int, DfltSentence> ( verbMap["LOOK"],  new DfltSentence(Look) )
                );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["LOOK AT"], new DfltSentence(LookAt))
                );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["INVENTORY"], new DfltSentence(Inventory))
                );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["GET"], new DfltSentence(Get))
                );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["DROP"], new DfltSentence(Drop))
                );

            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["N"], new DfltSentence(Move))
                );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["S"], new DfltSentence(Move))
                );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["E"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["W"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["NE"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["NW"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["SE"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["SW"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["UP"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["DOWN"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["OUT"], new DfltSentence(Move))
                 );
            defaultSentences.Add(
                 new Tuple<int, DfltSentence>(verbMap["OPEN"], new DfltSentence(Open))
                 );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["CLOSE"], new DfltSentence(Close))
                );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["ENTER"], new DfltSentence(Enter))
                );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["PUT"], new DfltSentence(Put))
                );
            defaultSentences.Add(
                new Tuple<int, DfltSentence>(verbMap["WEAR"], new DfltSentence(Wear))
                );

        }

        bool TryDefaultSentence()
        {
            foreach (Tuple<int, DfltSentence> t in defaultSentences)
            {
                if (t.Item1 == verb)
                {
                    t.Item2();
                    return true;
                }
            }
            return false;
        }

        bool HasVisibleItems(int id)
        {

            foreach (ObjTableEntry ote in objTable.objects.Values)
            {
                if (ote.GetObjAttr("HOLDER") == PLAYER)
                {
                    if (ote.GetObjAttr("SCENERY") == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        string VerbToDir()
        {
            if (verbMap["N"] == verb)
                return "N";
            if (verbMap["S"] == verb)
                return "S";
            if (verbMap["E"] == verb)
                return "E";
            if (verbMap["W"] == verb)
                return "W";
            if (verbMap["NE"] == verb)
                return "NE";
            if (verbMap["NW"] == verb)
                return "NW";
            if (verbMap["SE"] == verb)
                return "SE";
            if (verbMap["SW"] == verb)
                return "SW";
            if (verbMap["UP"] == verb)
                return "UP";
            if (verbMap["DOWN"] == verb)
                return "DOWN";
            if (verbMap["OUT"] == verb)
                return "OUT";
            return "";
        }

        public int GetStringId(string s)
        {
            return stringTable.GetEntryId(s);
        }

        public void AddVar(string varName, int amt)
        {
            if (varName.Equals("score"))
                varName = "$score";
            vars[varName].Add(amt);  
        }

        public bool IsVariable(string name)
        {
            return (vars.Keys.Contains(name));
        }

        public int GetVarVal(string name)
        {
            Variable v = vars[name];
            return v.val;
        }

        public void DumpInstence()
        {
            foreach (Sentence s in instead)
            {
                
            }
        }

        public string GetVerb(int v)
        {
            foreach (KeyValuePair<string,int> kv in verbMap)
            {
                if (kv.Value == v)
                    return kv.Key;
            }
            return "NA";
        }

        public string GetObjectName(int v)
        {
            if (v == -1) return "";

            return objTable.objects[v].name;
        }

        public string GetPrep(int p)
        {
            return prepTable[p];
        }

        void DoEvents()
        {
            foreach (Function f in events)
            {
                f.Execute();
            }
        }

        public void Debug(string s)
        {
            if (debug)
            {
                PrintStringCr(s);
            }
        }
    }

}
