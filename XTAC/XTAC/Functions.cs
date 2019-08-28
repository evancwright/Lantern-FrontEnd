using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XTAC
{
    public partial class Lantern : Form
    {

        void AddDefaultFunctions()
        {

            Routine r = new Routine();

            r.Name = "not_possible";
            r.Text = "if (dobj == player) { println(\"Not physically possible.\");  } ";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "get_portable";
            r.Text = "if (dobj.portable == 1) { if (dobj.holder != player) { println(\"(Taken)\"); dobj.holder = player;}  } ";
            xproject.Project.Routines.Routine.Add(r);

            //kill self routine
            r = new Routine();
            r.Name = "kill_self";
            r.Text = "println(\"If you are experiencing suicidal thoughts you should seek psychiatric help.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "default_kill";
            r.Text = "println(\"Perhaps you should count to 3 and calm down.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "kill_player";
            r.Text = "println(\"***YOU HAVE DIED***.\");player.holder=2;\n";
            xproject.Project.Routines.Routine.Add(r);

            //kill self routine
            r = new Routine();
            r.Name = "talk_to_self";
            r.Text = "println(\"Talking to yourself is a sign of impending mental collapse.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "default_talk";
            r.Text = "println(\"That fails to produce an exciting conversation.\");";
            xproject.Project.Routines.Routine.Add(r);

            //listen
            r = new Routine();
            r.Name = "listen";
            r.Text = "println(\"You hear nothing unexpected.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "smell";
            r.Text = "println(\"You smell nothing unexpected.\");";
            xproject.Project.Routines.Routine.Add(r);


            r = new Routine();
            r.Name = "wait";
            r.Text = "println(\"Time passes...\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "yell";
            r.Text = "println(\"AAAAAAAAAAAAARRRRGGGGGG!\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "jump";
            r.Text = "println(\"WHEEEEEE!\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "default_eat";
            r.Text = "println(\"That's not part of a healthy diet.\");";
            xproject.Project.Routines.Routine.Add(r);

            r = new Routine();
            r.Name = "default_drink";
            r.Text = "println(\"You can't drink that.\");";
            xproject.Project.Routines.Routine.Add(r);
        }

        void AddDefaultVars()
        {
            //add variables to a newly created project
            builtinVarsListBox.Items.Clear();
            CreateDefaultVar("dobj", "sentence+1", "0");
            CreateDefaultVar("iobj", "sentence+3", "0");
            CreateDefaultVar("score", "score", "0");
            CreateDefaultVar("moves", "moves", "0");
            CreateDefaultVar("health", "health", "100");
            CreateDefaultVar("turnsWithoutLight", "turnsWithoutLight", "0");
            CreateDefaultVar("gameOver", "gameOver", "0");
            CreateDefaultVar("answer", "answer", "0");

        }


        void AddDefaultSentences()
        {
            Sentence s = new Sentence();
            s = new Sentence();
            s.Verb = "examine";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "get_portable";
            s.Type = "before";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "take";
            s.Do = "PLAYER";
            s.Io = "";
            s.Prep = "";
            s.Sub = "not_possible";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "kill";
            s.Do = "PLAYER";
            s.Io = "";
            s.Prep = "";
            s.Sub = "kill_self";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "kill";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "default_kill";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "kill";
            s.Do = "*";
            s.Io = "*";
            s.Prep = "with";
            s.Sub = "default_kill";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "talk to";
            s.Do = "PLAYER";
            s.Io = "";
            s.Prep = "";
            s.Sub = "talk_to_self";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "talk to";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "default_talk";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "listen";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "listen";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "smell";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "smell";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);


            s = new Sentence();
            s.Verb = "wait";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "wait";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);


            s = new Sentence();
            s.Verb = "yell";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "yell";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "jump";
            s.Do = "";
            s.Io = "";
            s.Prep = "";
            s.Sub = "jump";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "eat";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "default_eat";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);


            s = new Sentence();
            s.Verb = "drink";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "default_drink";
            s.Type = "instead";
            xproject.Project.Sentences.Sentence.Add(s);

            s = new Sentence();
            s.Verb = "wear";
            s.Do = "*";
            s.Io = "";
            s.Prep = "";
            s.Sub = "get_portable";
            s.Type = "before";
            xproject.Project.Sentences.Sentence.Add(s);

        }


        void AddDefaultVerbChecks()
        {
            AddCheck("n", "check_move");
            AddCheck("s", "check_move");
            AddCheck("e", "check_move");
            AddCheck("w", "check_move");
            AddCheck("u", "check_move");
            AddCheck("d", "check_move");
            AddCheck("ne", "check_move");
            AddCheck("nw", "check_move");
            AddCheck("se", "check_move");
            AddCheck("sw", "check_move");
            AddCheck("close", "check_dobj_supplied");
            AddCheck("drink", "check_dobj_supplied");
            AddCheck("drink", "check_have_dobj");
            AddCheck("drop", "check_dobj_supplied");
            AddCheck("drop", "check_have_dobj");
            AddCheck("eat", "check_dobj_supplied");
            AddCheck("eat", "check_have_dobj");
            AddCheck("enter", "check_dobj_supplied");
            AddCheck("enter", "check_see_dobj");
            AddCheck("examine", "check_dobj_supplied");
            AddCheck("examine", "check_see_dobj");
            AddCheck("get", "check_dobj_supplied");
            AddCheck("get", "check_see_dobj");
            AddCheck("get", "check_dont_have_dobj");
            AddCheck("get", "check_dobj_portable");
            AddCheck("kill", "check_dobj_supplied");
            AddCheck("kill", "check_see_dobj");
            AddCheck("light", "check_dobj_supplied");
            AddCheck("light", "check_have_dobj");
            AddCheck("light", "check_see_dobj");
            AddCheck("open", "check_dobj_supplied");
            AddCheck("open", "check_see_dobj");
            AddCheck("open", "check_dobj_opnable");
            AddCheck("open", "check_dobj_unlocked");
            AddCheck("put", "check_dobj_supplied");
            AddCheck("put", "check_prep_supplied");
            AddCheck("put", "check_iobj_supplied");
            AddCheck("put", "check_not_self_or_child");
            AddCheck("put", "check_have_dobj");
            AddCheck("put", "check_see_iobj");
            AddCheck("put", "check_put");
            AddCheck("talk to", "check_dobj_supplied");
            AddCheck("talk to", "check_see_dobj");
            AddCheck("turn on", "check_dobj_supplied");
            AddCheck("turn on", "check_see_dobj");
            //AddCheck("turn on", "check_have_dobj");
            AddCheck("lock", "check_dobj_supplied");
            AddCheck("lock", "check_see_dobj");
            AddCheck("lock", "check_dobj_lockable");
            AddCheck("unlock", "check_dobj_supplied");
            AddCheck("unlock", "check_see_dobj");
            AddCheck("unlock", "check_dobj_lockable");
            AddCheck("look in", "check_dobj_supplied");
            AddCheck("look in", "check_see_dobj");
            AddCheck("wear", "check_see_dobj");
            AddCheck("use", "check_have_dobj");
            AddCheck("wear", "check_have_dobj");
            AddCheck("wear", "check_dobj_wearable");
            AddCheck("give", "check_dobj_supplied");
            AddCheck("give", "check_have_dobj");
            AddCheck("give", "check_iobj_supplied");
            AddCheck("give", "check_see_iobj");
            AddCheck("wear", "check_dobj_wearable");
        }



        void AddCheck(string verb, string check)
        {
            Check c = new Check();
            c.Verb = verb;
            c._check = check;
            xproject.Project.Checks.Check.Add(c);
        }

        void AddPrepositions()
        {
            string[] defaultPreps = new string[] { "in", "on", "at", "under", "into", "inside", "through", "out", "behind", "off", "up", "with", "to", "off", "north", "south", "east", "west", "northeast", "southeast", "northwest", "southwest", "up", "down", "about", "over", "across" };

            xproject.Project.Preps.Prep = new List<string>();
            foreach (string s in defaultPreps) { xproject.Project.Preps.Prep.Add(s); }
        }

        void AddDefaultVerbs()
        {

            string[] defaultVerbs = new string[] {
                "n,go north,north","s,go south,south","e,go east,east","w,go west,west","ne,go northeast,northeast","se,go southeast,southeast","sw,go southwest,southwest","nw,go northwest,northwest",
"up,go up,u","down,go down,d","enter,go in,go inside,get in","out","go","get,take,grab,pick up","give","inventory,i","kill,attack","drop","light","look,l","examine,x,look at","look in","search","open","lock","unlock","close,shut","eat","drink","put,place","quit","smell,sniff","listen","wait","climb",
"yell,scream,shout", "jump", "talk to", "turn on","turn off", "wear", "save", "restore", "push,press","read","use", "again"
            };

            foreach (string s in defaultVerbs) { xproject.Project.Verbs.Builtinverbs.Verb.Add(s); }
        }


        /// <summary>
        /// Removes the leading $ from legacy files
        /// </summary>
        void FixVariableNames()
        {
            foreach (Var v in xproject.Project.Variables.User.Var)
            {
                if (v.Name[0] == '$')
                {
                    v.Name = v.Name.Substring(1); //chop off $
                }
            }

            foreach (Var v in xproject.Project.Variables.Builtin.Var)
            {
                if (v.Name[0] == '$')
                {
                    v.Name = v.Name.Substring(1); //chop off $
                }
            }
        }


        void FixPrintedNames()
        {
            foreach (Object o in xproject.Project.Objects.Object)
            {
                if (o.PrintedName == null || o.PrintedName == "")
                {
                    //if the printed name is null, switch the name and the printed name
                    o.PrintedName = o.Name.Trim();
                    o.Name = o.Name.Replace(' ', '_');
                }

                if (o.PrintedName.Contains('_'))
                {
                    o.PrintedName = o.PrintedName.Replace('_', ' ');
                }
            }

        }

        void FixBlankDescriptions()
        {
            foreach (Object o in xproject.Project.Objects.Object)
            {
                if (o.Description == null || o.Description == "")
                {
                    //if the printed name is null, switch the name and the printed name
                    o.Description = "You notice nothing unexpected.";
                }
            }
        }

        void FixDescriptionsCase(Object o)
        {
            //if the printed name is null, switch the name and the printed name
            o.Description = FixSentenceCase(o.Description);
            o.Initialdescription = FixSentenceCase(o.Initialdescription);
        }

        void FixVerbs()
        {
            if (xproject.Project.Verbs.Builtinverbs.Verb[0] == "go")
            {
                xproject.Project.Verbs.Builtinverbs.Verb.RemoveAt(0);
                xproject.Project.Verbs.Builtinverbs.Verb.Add("go");
            }
             

            if (!xproject.Project.Verbs.Builtinverbs.Verb.Contains("save"))
            {
                xproject.Project.Verbs.Builtinverbs.Verb.Add("save");
            }

            if (!xproject.Project.Verbs.Builtinverbs.Verb.Contains("restore"))
            {
                xproject.Project.Verbs.Builtinverbs.Verb.Add("restore");
            }

        }

        /// <summary>
        /// Attempts to fix missing object names and ids
        /// </summary>
        void FixEmptyObjects()
        {
            int i = 0;
            foreach (Object o in xproject.Project.Objects.Object)
            {
                if (o.PrintedName.Trim() == "")
                {
                    //if the printed name is null, switch the name and the printed name
                    o.PrintedName = "object" + i;
                }

                if (o.Name.Trim() == "")
                {
                    //if the printed name is null, switch the name and the printed name
                    o.Name = "object" + i;
                }
                i++;
            }
        }

        void FixChecks()
        {
            AddMissingCheck("lock", "check_dobj_lockable");
            AddMissingCheck("unlock", "check_dobj_lockable");
            AddMissingCheck("unlock", "check_dobj_lockable");
            AddMissingCheck("unlock", "check_dobj_lockable");
        }

        /// <summary>
        /// Adds a check if not already assigned
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="check"></param>
        void AddMissingCheck(string verb, string check)
        {
            bool found=false;
            foreach (Check c in xproject.Project.Checks.Check)
            {
                if (c.Verb == verb && c._check == check)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                AddCheck(verb, check);
            }
        }

        void FixFunctions()
        {
            //XML serialization turns \r\n into \n
            //replace them back
            foreach (Routine r in xproject.Project.Routines.Routine)
            {
                if (r.Text != null)
                    r.Text = r.Text.Replace("\n", "\r\n");
            }

            //replace any spaces in names
            foreach (Routine r in xproject.Project.Routines.Routine )
            {
                r.Name = r.Name.Trim().Replace(' ', '_');
            }

            foreach (Event e in xproject.Project.Events.Event)
            {
                e.Name = e.Name.Trim().Replace(' ', '_');
            }

            //XML serialization turns \r\n into \n
            //replace them back
            foreach (Event r in xproject.Project.Events.Event)
            {
                if (r.Text != null)
                r.Text = r.Text.Replace("\n", "\r\n");
            }

            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                s.Sub = s.Sub.Trim().Replace(' ', '_');
            }

            

        }

        string FixSentenceCase(string s)
        {
            //return s;
            
            bool ucase = true;
            StringBuilder sb = new StringBuilder(s.ToLower());
            for (int i=0; i < s.Length; i++)
            {
                if (sb[i] != ' ' && ucase)
                {
                    sb[i] = sb[i].ToString().ToUpper()[0];
                    ucase = false;
                }
                else if (sb[i] == '.')
                {
                    ucase = true;
                }
            }

            return sb.ToString();
            
        }

        bool  IsObject(Object o)
        {
            return (o.Directions.N == "255" &&
                    o.Directions.S == "255" &&
                    o.Directions.E == "255" &&
                    o.Directions.W == "255" &&
                    o.Directions.Ne == "255" &&
                    o.Directions.Se == "255" &&
                    o.Directions.Nw == "255" &&
                    o.Directions.Sw == "255" &&
                    o.Directions.Up == "255" &&
                    o.Directions.Down == "255" &&
                    o.Directions.In == "255" &&
                    o.Directions.Out == "255");
        }

        void FixCase()
        {
            foreach (Object o in xproject.Project.Objects.Object)
            {
                FixDescriptionsCase(o);

                if (IsObject(o))
                {
                    o.PrintedName = o.PrintedName.ToLower();
                }
                else
                {
                    string[] names = o.PrintedName.ToLower().Split(' ');
                    string newName = "";
                    bool first = true;
                    foreach (string s in names)
                    {
                        if (!first)
                            newName += " ";

                        first = false;
                        string firstChar = ("" + s[0]).ToUpper();
                        string rem = s.Substring(1);

                        newName += firstChar + rem; 
                        
                    }
                    o.PrintedName = newName;
                }
            }
        }
    }
}
