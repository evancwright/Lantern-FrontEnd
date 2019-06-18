using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XTAC;
using XMLtoAdv;
using GameTables;
using IGame;
using CLL;

//n,n,push tree,n,n,n,e,ne,w,w,get leaves,w,s,s,get gunk,n,pick lock,e,open safe,get coin,open desk,get lighter,w,n,w,w,n,open box,put coin in box,s,e,s,w,get gloves,wear gloves,wear gloves,e,n,n,get wrench,s,w,n,put coin in recess,s,w,nw,drop leaves,burn leaves,se,sw,get crate,ne,nw,u,u,drop crate,d,d,get gold,u,u,u,unbolt cupola,u
namespace PlayerLib
{
    public partial class Game : IGame.IGame, IGameXml    {
        delegate void DfltSentence();
        bool debug = false;
        //Xml gameData;
        ObjTable objTable;
        Table stringTable = new Table();
        Table nogoTable = new Table();
        Table verbTable = new Table(); //raw verbs
        Dictionary<string, int> verbMap = new Dictionary<string, int>();
        Table dictionary = new Table();
        ObjWordTable objWordTable = new ObjWordTable();
        const int OFFSCREEN = 0; 
        const int PLAYER = 1;
        const int INVALID = -1;
        int[] scores;
        List<string> articleTable = new List<String>();
        List<string> prepTable = new List<String>();
        List<Tuple<int, DfltSentence>> defaultSentences = new List<Tuple<int, DfltSentence>>();
        List<Sentence> before = new List<Sentence>();
        List<Sentence> instead = new List<Sentence>();
        List<Sentence> after = new List<Sentence>();
        Dictionary<string, CLL.Function> functions = new Dictionary<string, CLL.Function>();
        List<string> functionNames = new List<string>();
        List<CLL.Function> events = new List<CLL.Function>();
        Dictionary<string, Variable> vars = new Dictionary<string, Variable>();
        TextBox outputWindow;
        NameTable nameTable = new NameTable();
        string welcome;
        string author;
        string version;
        string walkThrough;
        string prevCommand = "LOOK";
        int verb;
        int dobj;
        int prep;
        int iobj;

        int prevVerb;
        int prevDobj;
        int prevPrep;
        int prevIobj;
        
        bool dobjSupplied;
        bool iobjSupplied;
        static Game _game;

        string[] dirs = { "n", "s", "e", "w", "ne", "se", "nw", "sw", "up", "down" };

        private Game() {



            articleTable.Add("A");
            articleTable.Add("AN");
            articleTable.Add("THE");

            /*      "in", "on", "at", "under", "into", "inside", "through", "out", "behind", "off", 
             *      "up", "with", "to", "off", "north", "south", "east", "west", "northeast", "southeast",
             *      "northwest", "southwest", "up", "down", "about", "over", "across" };
             */

            prepTable.Add("IN");
        	prepTable.Add("ON");
	        prepTable.Add("AT");
        	prepTable.Add("TO");
        	prepTable.Add("INSIDE");
        	prepTable.Add("OUT");
        	prepTable.Add("UNDER");
            prepTable.Add("OFF");
            prepTable.Add("UP");
            prepTable.Add("WITH");
            prepTable.Add("BEHIND");
            prepTable.Add("NORTH");
            prepTable.Add("SOUTH");
            prepTable.Add("EAST");
            prepTable.Add("WEST");
            prepTable.Add("NORTHEAST");
            prepTable.Add("SOUTHEAST");
            prepTable.Add("NORTHWEST");
            prepTable.Add("SOUTHWEST");
            prepTable.Add("UP");
            prepTable.Add("DOWN");
            prepTable.Add("ABOUT");
            prepTable.Add("OVER");
            prepTable.Add("ACROSS");
            
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

            for (int i = 0; i < stringTable.GetNumEntries(); i++)
            {
                string s = stringTable.GetEntry(i);
                if (s.Length > 255)
                {
                    MessageBox.Show("Warning: string \"" + s + "\" is greater than 255 characters long.  This will run fine in test player but will cause problems when exporting.");
                }

                if (s.Contains('\"'))
                {
                    MessageBox.Show("Warning: string \"" + s + "\" contains a double quote.  Please change it to a single quote.");
                }
                if (s.Contains('\r') || s.Contains('\n'))
                {
                    MessageBox.Show("Warning: string \"" + s + "\" contains a newline.  Please remove it.");
                }
                

            }


            BuildNogoTable(doc);
            BuildVerbTable(doc);

            PopulateObjectAndDictionary(doc);
            BuildObjWordTable();

            //this.gameData = data;

//            ProgramData progData = ProgramData.GetInstance();
//            progData.SetProgramData(Game.GetInstance());

            BuildDefaultSentenceMap();
            LoadVars(doc);
            BuildSentences(doc);
            GetFunctionNames(doc);
            BuildFunctions(doc);
            BuildCheckTable(doc);
            scores = new int[objTable.GetCount()];
        }

        public void SetOutputWindow(TextBox tb)
        {
            outputWindow = tb;
        }

        public override void PrintCr()
        {
            outputWindow.Text += "\r\n";
        }

        public override void PrintString(int strId)
        {
            string s = stringTable.GetEntry(strId);
            outputWindow.Text += s;
        
        }

        public override void PrintStringCr(int strId)
        {
            string s = stringTable.GetEntry(strId);
            outputWindow.Text += s + "\r\n";
        }


        public override void PrintString(string s)
        {
            outputWindow.Text += s;
        }

        public override void PrintStringCr(string s)
        {
            outputWindow.Text += s + "\r\n";
            outputWindow.SelectionStart = outputWindow.TextLength;
            outputWindow.ScrollToCaret();
        }

        public override int GetObjectId(string name)
        {
            if (name == "")
                return -1;
            if (name == "*")
                return 254;

            int val;
            
            if (Int32.TryParse(name, out val))
            {
                return val;
            }


             
            if (name == "dobj")
                return dobj;

            if (name == "iobj")
                return iobj;
             
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

        public override void SetObjectAttr(int id, string name, int val)
        {
            try
            {
                objTable.SetObjAttr(id, name, val);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to set attr " + name + " on object " + id, e);
            }
        }

  
        public override int GetObjectAttr(int id, string name)
        {
            return objTable.GetObjAttr(id, name);
        }

        public override int GetObjectProp(int id, string name)
        {
            return objTable.GetObjAttr(id,name);
        }

         
        public void Start()
        {

        }


       

        public override void Look()
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
                PrintStringCr("It is pitch dark.");
            }
        }


        public void LookAt()
        {
            PrintStringCr(objTable.GetObjAttr(dobj, "DESCRIPTION"));

            if (IsSupporter(dobj) && HasVisibleItems(dobj))
            {
                PrintString("On the ");
                PrintObjectName(dobj);
                PrintStringCr(" is:");
                ListContents(dobj);
            }

        }


        public void LookIn()
        { 
            if (!IsContainer(dobj))
            {
                PrintString("You can't see inside that.");
                return;
            }
            
            if (IsClosedContainer(dobj))
            {
                PrintString("The ");
                PrintObjectName(dobj);
                PrintStringCr(" is closed.");
                return;
            }

            if (HasVisibleItems(dobj))
            {
                PrintString("The ");
                PrintObjectName(dobj);
                PrintStringCr(" contains:");
                ListContents(dobj);
            }
            else
            {
                PrintString("The ");
                PrintObjectName(dobj);
                PrintStringCr(" is empty.");
            }

        }

        public override void Move()
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
                        PrintStringCr("The " + newr.printedName + " is closed.");
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
            PrintStringCr("Taken.");
        }

        public void Drop()
        {
            SetObjectAttr(dobj, "HOLDER", GetPlayerRoom());
            PrintStringCr("Dropped.");
        }

        public void Open()
        {
            SetObjectAttr(dobj, "OPEN", 1);

            if (GetObjectAttr(dobj, "DOOR")==0)
            {
                if (HasVisibleItems(dobj))
                {
                    PrintString("Opening the  ");
                    PrintObjectName(dobj);
                    PrintStringCr(" reveals:");
                    ListContents(dobj);
                }
                else
                {
                    PrintStringCr("Opened.");
                }
            }
            else
            {
                PrintStringCr("Opened.");
            }
        }

        public void Close()
        {
            SetObjectAttr(dobj, "OPEN", 0);
            PrintStringCr("Closed.");

        }

           
        public void Enter()
        {
            if (GetObjectAttr(dobj, "IN") != 255)
            {
                if (GetObjectAttr(dobj, "OPEN") == 0)
                {
                    PrintStringCr("It's closed.");
                }
                else
                {
                    SetObjectAttr(PLAYER, "HOLDER", GetObjectAttr(dobj, "IN"));
                    Look();
                }
            }
            else
            {
                PrintStringCr("You can't enter that.");
            }
        }

        public void Put()
        {
            SetObjectAttr(dobj, "HOLDER", iobj);
            PrintStringCr("Done.");
        }

        public void Wear()
        {
            if (GetObjectProp(dobj, "WEARABLE") == 1)
            {
                PrintString("You put on the " + objTable.GetObj(dobj).printedName + ".");
                SetObjectAttr(dobj, "BEINGWORN", 1);
            }
            else
            {
                PrintString("You can't wear that.");
            }
        }

        public void Quit()
        {

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
            stringTable.AddEntry("You notice nothing unexpected.");

            XmlToTables x = XmlToTables.GetInstance();
            x.ParseForStrings(doc, stringTable);
        }

        void BuildNogoTable(XmlDocument doc)
        { 
            nogoTable.Clear();
            nogoTable.AddEntry("You notice nothing unexpected.");

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
            stringTable.AddEntry("You notice nothing unexpected.");

            XmlNodeList list = doc.SelectNodes("//project/objects/object");
          
            foreach (XmlNode n in list)
            {
                string name = n.Attributes.GetNamedItem("name").Value;
                string printedName = name;

                try
                {
                    printedName = n.Attributes.GetNamedItem("printedname").Value;
                }
                catch
                {
                    printedName = name.Replace('_', ' ');
                }
                
                try
                {
                    //get the child node with the description
                    XmlNode child = n.ChildNodes[0];
                    string desc = child.InnerText;

                    //don't add blank descriptions
                    if (desc != "")
                    {
                        stringTable.AddEntry(desc);
                    }
                    else
                    {
                        stringTable.AddEntry("You notice nothing unexpected.");
                        n.ChildNodes[0].InnerText = "You notice nothing unexpected.";
                    }

                    //initial desc

                    try
                    {
                        string initialDesc = n.SelectSingleNode("initialdescription").InnerText.Trim();

                        if (initialDesc != "" && initialDesc != null)
                        {
                            stringTable.AddEntry(initialDesc);
                        }
                    }
                    catch
                    {
                    }
                    //break the name into words and put each word in the dictionary
                    char[] delimiterChars = { ' ' };
                    string[] toks = printedName.Split(delimiterChars);

                    foreach (string s in toks)
                    {
                        if (s != null && !s.Equals(""))
                        {
                            dictionary.AddEntry(s);
                        }
                    }

                    //create the object from the data
                    GameObject gobj = null;
                    try
                    {
                        gobj = new GameObject(n);
                        objTable.Add(new ObjTableEntry(gobj, stringTable));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to add object to name", ex);
                    }


                    //put synoyms in table
                    if (gobj.synonyms != null)
                    {
                        foreach (string s in gobj.synonyms)
                        {
                            if (!s.Equals(""))
                                dictionary.AddEntry(s);
                        }
                    }

                    //check for nogo messages
                    //if any movement dir has a corresponding entry in the nogo table
                    //set the atr to 255 - string id
                    foreach (string d in dirs)
                    {
                        if (gobj.HasNogoMsg(d))
                        {
                            string msg = gobj.GetNogoMsg(d);
                            int nogoIndex = nogoTable.GetEntryId(msg);
                            objTable.SetObjAttr(gobj.id, d.ToUpper(), (255 - nogoIndex) + 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding " + name + " to dictionary", ex);
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

            list = doc.SelectNodes("//project/walkthrough");

            try
            {
                walkThrough = list[0].InnerText;
            }
            catch
            {
                walkThrough = "No walkthrough provided - for shame! ";
            }

        }

        public override void PrintObjectName(int o)
        {
            PrintString(objTable.GetObj(o).printedName);
        }
          
        public void Run()
        {
            PrintStringCr(welcome);
            PrintStringCr(version);
            PrintStringCr(author);
            PrintCr();
            Look();
            PrintString(">");
            outputWindow.SelectionLength = 0;
        }

        public void AcceptCommand(string command)
        {
            command = command.Trim();
            string[] commands = command.Split(',');

            PrintCr();

            foreach (string s in commands)
            {
                if (s.ToUpper().Equals("DEBUG"))
                {
                    debug = !debug;

                    if (debug)
                        PrintStringCr("DEBUG ON");
                    else
                        PrintStringCr("DEBUG OFF");
                    PrintString(">");
                    return;
                }

                if (s.ToUpper().Equals("CHEAT"))
                {
                    if (walkThrough == "")
                    {
                        PrintStringCr("No walkthrough provided!");
                    }
                    else
                    {
                        PrintStringCr(walkThrough);
                    }
                    PrintString(">");
                    return;
                }

                if (s.ToUpper().Equals("VARS"))
                {
                    foreach (string v in vars.Keys)
                    {
                        PrintStringCr(v + "=" + vars[v].val);
                    }
                    PrintString(">");
                    return;
                }

                if (s.ToUpper().Equals("DICT"))
                {
                    for (int i = 0; i < dictionary.GetNumEntries(); i++ )
                    {
                        PrintStringCr(dictionary.GetEntry(i) + "=" + i);
                    }
                    return;
                }
                if (s.ToUpper().Equals("OWT"))
                {
                    foreach (int i in objWordTable.entries.Keys)
                    {
                        PrintString(objTable.GetObj(i).name + "=" );
                        
                        List<int> words = objWordTable.entries[i];
                        foreach (int w in words)
                        {
                            PrintString(w + " ");
                        }
                        PrintStringCr("");
                    }
                    return;
                }

                if (s.ToUpper().Equals("SENTENCES"))
                {
                    PrintStringCr("BEFORE SENTENCES");
                    DumpSentences(before);
                    PrintStringCr("INSTEAD SENTENCES");
                    DumpSentences(instead);
                    PrintStringCr("AFTER SENTENCES");
                    DumpSentences(after);
                }


                    if (Parse2(s.Trim()))
                {
//                    if (MapWords())
                    {
                        if (RunChecks())
                            RunCommand();

                        if (!command.ToUpper().StartsWith("AGAIN"))
                        {
                            prevCommand = command.ToUpper();
                        }
                    }
                }
            }
            PrintString(">");
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
                    PrintStringCr("You don't see that.");
                    return false;
                }
                SetVar("dobj", dobj);
            }

            if (iobj != -1)
            {
                iobj = MapObject(iobj);
                if (iobj == -1)
                {
                    PrintStringCr("You don't see that.");
                    return false;
                }
                SetVar("iobj", iobj);
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
            

            bool handled = TryRun(before);

            if (!TryRun(instead))
            {
                if (TryDefaultSentence())
                {
                    handled = true;
                }
            }
            else handled = true;

            if (TryRun(after))
                handled = true;

            if (!handled)
                PrintStringCr("I don't understand.");

            PrintCr();

            DoEvents();
                
            
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
                PrintStringCr("Pardon?");
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

            if (list.Count==0)
            {
                PrintStringCr("Pardon?");
                return false;
            }


            //is verb valid
            if (!verbMap.Keys.Contains(list[0]))
            {
                PrintStringCr("I don't know the verb '" + list[0] + "'");
                return false;
            }

            verb = verbMap[list[0]];

            if (list.Count > 1)
            {//get the dobj
                int dobjId = dictionary.GetEntryId(list[1]) ;
                if (dobjId== -1)
                {
                    PrintStringCr("I don't know the word '" + list[1].ToLower() + "'");
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
                        PrintStringCr("I don't know the word '" + list[prepIndex+1].ToLower() + "'");
                        return false;
                    }
                    iobj = iobjId;
                }
                else
                {
                    PrintStringCr("Missing indirect object.");
                    return false;                 
                }
            }

            return true;
        }

        //is the object a descendant of the player or the player's room.
        bool IsVisibleToPlayer(int objId)
        {
            int playerRoom = GetPlayerRoom();
            /*
            //backdrop
            if (GetObjectAttr(objId, "BACKDROP") == 1)
            {
                if (objTable[objId] )
                {

                }
            }
            */
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

        bool IsContainer(int objId)
        {
            return (GetObjectAttr(objId, "CONTAINER") == 1);
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

        bool IsSupporter(int objId)
        {
            if (GetObjectAttr(objId, "SUPPORTER") == 1)
            {
                return true;
            }
            return false;
        }

        void BuildObjWordTable()
        {

            for (int i = 0; i < objTable.GetCount(); i++ )
            {
                try
                {
                    List<int> l = new List<int>();

                    string n = objTable.objects[i].printedName;
                    string[] words = n.Split(' ');

                    for (int j = 0; j < words.Length; j++)
                    {
                        words[j] = words[j].ToUpper();
                    }

                    objWordTable.AddNewEntry(i, words.ToList<string>(), dictionary);
                }
                catch (Exception e)
                {
                    throw new Exception("Error creating object word table entry for object " + i);
                }
            }

            //add synonyms to objWordTable
            for (int i = 0; i < objTable.GetCount(); i++)
            {
                List<int> l = new List<int>();

                 
                List<string> syns = new List<string>();

                foreach (string s in  objTable.objects[i].synonyms)
                {
                    syns.Add(s.ToUpper().Trim());
                }
                if (syns.Count > 0)
                {
                    objWordTable.AppendSynonyms(i, syns, dictionary);
                }
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
                new Tuple<int, DfltSentence>(verbMap["LOOK IN"], new DfltSentence(LookIn))
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
                if (ote.GetObjAttr("HOLDER") == id)
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
            throw new Exception("Unable to map verb " + verb + " to a direction");
        }

        public override int GetStringId(string s)
        {
            int strId = stringTable.GetEntryId(s);

            if (strId == -1)
            {
                throw new Exception("String \'" + s + "\' not found in table");
            }

            return strId;
        }

        public override void AddVar(string varName, int amt)
        {
            if (varName.Equals("score"))
                varName = "$score";
            vars[varName].Add(amt);  
        }

        public override bool IsVariable(string name)
        {
            return (vars.Keys.Contains(name));
        }

        public override  int GetVarVal(string name)
        {
            if (vars.ContainsKey(name))
            {
                Variable v = vars[name];
                return v.val;
            }
            else
            {
                throw new Exception("Bad variable name (in set): " + name);
            }
        }


        public string GetVarAddr(string name)
        {
            return name; //THIS SHOULD NEVER ACTUALLY GET CALLED
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
            //check if player can see
            try
            {
                if (vars["moves"].val < 255)
                {
                    vars["moves"].val++;
                }

                if (PlayerCanSee())
                {
                    vars["turnsWithoutLight"].val = 0;
                }
                else
                {
                    vars["turnsWithoutLight"].val++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating built-in variables", ex);
            }


            //run the events

            foreach (Function f in events)
            {
                try
                {
                    f.Execute(this);
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

                    MessageBox.Show("Error in event" + f.name + "\r\n" + message);
                }
            }



            Unwear();
        }

        public override void Debug(string s)
        {
            if (debug)
            {
                PrintStringCr(s);
            }
        }

        public override void SetVar(string name, int val)
        {
            if (vars.ContainsKey(name))
            {
                vars[name].val = val;

                if (vars[name].val < 0)
                {
                    vars[name].val = 255 - vars[name].val;
                }
            }
            else
            {
                throw new Exception("Bad variable: " + name);
            }
        }

        public void Unwear()
        {
            foreach (ObjTableEntry ote in objTable.objects.Values)
            {
                if (ote.GetObjAttr("HOLDER") != PLAYER)
                {
                    if (ote.GetObjAttr("BEINGWORN")==1)
                    {
                        ote.SetObjAttr("BEINGWORN", 0);
                        Debug("Unwearing " + ote.name);
                    }
                }
            }
        }

        public override bool PlayerHas(int objId)
        {
            return ObjectHas(PLAYER, objId);
        }

        public override  bool ObjectHas(int parent, int child)
        {
            while (true)
            {
                int tempParent = GetObjectAttr(child, "HOLDER");
                if (tempParent == parent)
                    return true;
                if (tempParent == OFFSCREEN)
                    return false;
                child = tempParent;
            }
        }

        void ClearScores()
        {
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i] = 0;
            } 
        }

        void ScoreObject(int i)
        {
            if (debug)
                PrintStringCr("Scoring object:" + GetObjectName(i));

            scores[i] += 1;

            if (scores[i] != INVALID)
            {

                if (PlayerHas(i))
                {
                    if (debug)
                        PrintStringCr("Player has:" + GetObjectName(i));
                    scores[i] += 2;
                }
                else if (VisibleAncestor(GetPlayerRoom(), i))
                {
                    if (debug)
                        PrintStringCr("Player can see:" + GetObjectName(i));
                    scores[i] += 1;
                }
            }
 //           else 
 //           {
 //               scores[i] = INVALID;
 //           }
            //else if ()
            //    scores[i] += 1;
        }

        int GetMaxScore()
        {
            int max = 0;
            for (int i = 0; i < objTable.GetCount(); i++)
            {
                /*
                if (scores[i] > 0)
                {
                    PrintStringCr(objTable.GetObj(i).name + " matched");
                }
                */
                if (scores[i] > max)
                    max = scores[i];
            }
            if (debug)   
                PrintStringCr("Max score=" + max);
             return max;
        }

        /// <summary>
        /// Returns how many objects match the max score.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        int MaxScoreMatches(int max)
        {
            int count = 0;
            for (int i = 0; i < objTable.GetCount(); i++)
            {
                if (scores[i] == max)
                    count++;
            }
//            PrintStringCr("max score matches=" + count);
            return count;
        }


        int GetMaxScoreObject(int maxScore)
        {
            
            for (int i = 0; i < objTable.GetCount(); i++)
            {
                if (scores[i] == maxScore)
                {
                    if (debug)
                        PrintStringCr("Best match=" + GetObjectName(i));
                    return i;
                }
            }
            return INVALID;
        }
        bool AmbiguousNoun()
        {
            return MaxScoreMatches(GetMaxScore()) > 1;
        }

        void PrintDisambigError()
        {
            PrintStringCr("I don't know which one you mean:");
            int m = GetMaxScore();
            for (int i=0; i < objTable.GetCount(); i++)
            {
                if (scores[i] == m)
                {
                    PrintObjectName(i);
                    PrintCr();
                }
            }
            PrintStringCr("Please be more specific.\n");
        }


        /// <summary>
        /// Loop over all the objects and see if they match the wordId
        /// </summary>
        /// <param name="wordId"></param>
        void ScoreObjects(int wordId)
        {
            int c = objTable.GetCount();
            for (int i = 0; i < c; i++)
            {
                int id = objTable.GetObj(i).GetObjAttr("ID");
                if (WordAppliesToObject(i, wordId))
                {
                    if (debug)
                        PrintStringCr("Finding matches for word " + dictionary.GetEntry(wordId));
                    ScoreObject(id);
                }
                else
                    scores[id] = INVALID;
            }
        }

        bool Parse2(string command)
        {  
            int prepFound = 0;
            verb = -1;
            dobj = -1;
            prep = -1;
            iobj = -1;
            dobjSupplied = false;
            iobjSupplied = false;
            command = command.ToUpper();
            command = command.Trim();
            ClearScores();
            SetVar("dobj", 255);
            SetVar("iobj", 255);
            if (command.Length == 0)
            {
                PrintStringCr("Pardon?");
                return false;
            }

            if (command.StartsWith("AGAIN"))
            {
                command = prevCommand;
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

            words = list.ToArray();
            
            if (list.Count == 0)
            {
                PrintStringCr("I don't understand.");
                return false;
            }

            //consolidate prep
            if (list.Count >= 2)
            {
                if (prepTable.Contains(list[1]))
                {
                    list[0] = list[0] + " " + list[1];
                    list.RemoveAt(1);
                    words = list.ToArray();
                }
            }

            //is verb valid
            if (!verbMap.Keys.Contains(list[0].ToUpper()))
            {
                PrintStringCr("I don't know the verb '" + list[0].ToLower() + "'");
                return false;
            }

            verb = verbMap[list[0]];

            if (list.Count == 1)
            {
                return true;
            }


            for (int i = 1; i < list.Count; i++)
            {
                if (prepTable.Contains(words[i].ToUpper()))
                {
                    prep = GetPrepIndex(words[i]);
                    prepFound = 1;
                    //disambiguate the direct obj
                    if (AmbiguousNoun())
                    {
                        if (!NounsVisible())
                        {
                            PrintStringCr("You don't see that.");
                            return false;
                        }
                        else
                        {
                            PrintDisambigError();
                            return false;
                        }
                    }
                    

                    /*What happes if the prep was the last word?*/
                    /*identify to dobj*/
                    dobj = GetMaxScoreObject(GetMaxScore());
                    SetVar("dobj", dobj);

                    /*map the remaing words to the direct object*/
                    ClearScores();
                    i++;
                    for (; i < words.Length; i++)
                    {
                        int wordId = dictionary.GetEntryId(words[i]);

                        if (wordId == INVALID)
                        {
                            PrintStringCr("I don't know the word '" + words[i].ToLower() + "'.");
                            return false;
                        }
                        /*score indirect objects*/
                        iobjSupplied = true;
                        ScoreObjects(wordId);
                    }

                    if (AmbiguousNoun())
                    {
                        if (!NounsVisible())
                        {
                            PrintStringCr("You don't see that.");
                        }
                        else
                        {
                            PrintDisambigError();
                        }
                        return false;
                    }

                    iobj = GetMaxScoreObject(GetMaxScore());
                    SetVar("iobj", iobj);
                    break;
                }
                else
                {
                    //score direct object word
                    int wordId = dictionary.GetEntryId(words[i]);

                    if (wordId == INVALID)
                    {
                        PrintStringCr("I don't know the word '" + words[i].ToLower()+ "'.");
                        return false;
                    }
                    dobjSupplied = true;
                    ScoreObjects(wordId);
                }
            }

            if (prepFound==0)
            {
                dobj = GetMaxScoreObject(GetMaxScore());
                SetVar("dobj", dobj);

                if (AmbiguousNoun())
                {
                    if (!NounsVisible())
                    {
                        PrintStringCr("You don't see that.");
                    }
                    else
                    {
                        PrintDisambigError();
                    }

                    return false;
                }
            }

            return true;
        }

        bool WordAppliesToObject(int objectId, int wordId)
        {
            return objWordTable.Matches(objectId, wordId);
        }

        int GetPrepIndex(string s)
        {
            for (int i = 0; i < prepTable.Count; i++)
            {
                if (s.ToUpper() == prepTable[i].ToUpper())
                {
                    return i;
                }
            }
            return INVALID;
        }

        bool NounsVisible()
        {
            for (int i = 2; i < objTable.GetCount(); i++)
            {
                if (scores[i] != 0 && scores[i] != INVALID)
                {
                    if (IsVisibleToPlayer(i))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// looks in the xdoc to see if a name is a function
        /// We need to look there because a function could be called
        /// in a function before it is defined.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void GetFunctionNames(XmlDocument doc)
        {
            XmlNodeList evts = doc.SelectNodes("//project/routines/routine");

            foreach (XmlNode n in evts)
            {
                string name = n.Attributes.GetNamedItem("name").Value.Trim();
                functionNames.Add(name + "_sub");
            }
        }

        public override bool IsFunction(string name)
        {
            return functionNames.Contains(name);
        }

        private void DumpSentences(List<Sentence> list)
        {
            foreach (Sentence s in list)
            {
                string sout =  s.verb + " " + s.dobj + " " + s.iobj + "---->" + s.name;
                PrintStringCr(sout);
            }
        }

    }

}
