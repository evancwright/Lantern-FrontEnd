using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PlayerLib
{    

    partial class Game
    {
        delegate bool VerbCheckDlgt();
        const int MaxInvWeight = 10;

        List<Tuple<int,string>> checkList = new List<Tuple<int,string>>();
        Dictionary<string, VerbCheckDlgt> checkTable = new Dictionary<string, VerbCheckDlgt>();

        void BuildCheckTable(XmlDocument doc)
        {
            checkTable.Clear();

            //create jump table
            checkTable.Add("check_move", new VerbCheckDlgt(check_move));
            checkTable.Add("check_dobj_supplied", new VerbCheckDlgt(check_dobj_supplied));
            checkTable.Add("check_iobj_supplied", new VerbCheckDlgt(check_iobj_supplied));
            checkTable.Add("check_prep_supplied", new VerbCheckDlgt(check_prep_supplied));
            checkTable.Add("check_see_dobj", new VerbCheckDlgt(check_see_dobj));
            checkTable.Add("check_see_iobj", new VerbCheckDlgt(check_see_iobj));
            checkTable.Add("check_have_dobj", new VerbCheckDlgt(check_have_dobj));
            checkTable.Add("check_have_iobj", new VerbCheckDlgt(check_have_iobj));
            checkTable.Add("check_dobj_portable", new VerbCheckDlgt(check_dobj_portable));
            checkTable.Add("check_dobj_open", new VerbCheckDlgt(check_dobj_open));
            checkTable.Add("check_dobj_closed", new VerbCheckDlgt(check_dobj_closed));
            checkTable.Add("check_dobj_opnable", new VerbCheckDlgt(check_dobj_opnable));
            checkTable.Add("check_dobj_lockable", new VerbCheckDlgt(check_dobj_lockable));
            checkTable.Add("check_dobj_locked", new VerbCheckDlgt(check_dobj_locked));
            checkTable.Add("check_dobj_unlocked", new VerbCheckDlgt(check_dobj_unlocked));
            checkTable.Add("check_dont_have_dobj", new VerbCheckDlgt(check_dont_have_dobj));
            checkTable.Add("check_dobj_wearable", new VerbCheckDlgt(check_dobj_wearable));
            checkTable.Add("check_light", new VerbCheckDlgt(check_light));
            checkTable.Add("check_weight", new VerbCheckDlgt(check_weight));
            checkTable.Add("check_enter_dobj", new VerbCheckDlgt(check_enterable));
            checkTable.Add("check_not_self_or_child", new VerbCheckDlgt(check_not_self_or_child));
            checkTable.Add("check_put", new VerbCheckDlgt(check_put));

            XmlNodeList checks = doc.SelectNodes("//project/checks/check");

            foreach (XmlNode c in checks)
            {
                string verb = c.Attributes.GetNamedItem("verb").Value;

                if (verb.IndexOf(",") != -1)
                {//if there are synonyms, get 1st
                    verb = verb.Substring(0, verb.IndexOf(","));
                }


                if (!verbMap.ContainsKey(verb.ToUpper()))
                    throw new Exception("A check is referring to verb '" + verb + "', but this verb is not in the verb table");

                int verbId = verbMap[verb.ToUpper()];

                if (verbId != -1)
                {
                    string checkName = c.Attributes.GetNamedItem("check").Value.Trim();
                    checkList.Add(new Tuple<int,string>(verbId, checkName));
                }


            }

          
        }

        bool check_see_dobj()
        {
            //if there were potential matches
            //but none were visible, print the err msg
            if (dobj == -1)
            {
                PrintStringCr("You don't see that.\n");
                return false;
            }

            if (IsVisibleToPlayer(dobj))
                return true;

            PrintStringCr("You don't see that.\n");
            return false;
        }

        bool check_see_iobj()
        {
            //if there were potential matches
            //but none were visible, print the err msg
            if (iobj == -1)
            {
                PrintStringCr("You don't see that.\n");
                return false;
            }

            if (IsVisibleToPlayer(iobj))
                return true;

            PrintStringCr("You don't see that.\n");
            return false;
        }

        bool check_have_dobj()
        {
            if (!VisibleAncestor(PLAYER,dobj))
            {
                PrintStringCr("You don't have that.");
                return false;
            }
            return true;
        }

        bool check_dont_have_dobj()
        {
            if (VisibleAncestor(PLAYER, dobj))
            {
                PrintStringCr("You already have it.");
                return false;
            }
            return true;
        }

        bool check_have_iobj()
        {
            if (!VisibleAncestor(PLAYER, iobj))
            {
                PrintStringCr("You don't have that.");
                return false;
            }
            return true;
        }

        bool check_dobj_portable()
        {
            if (GetObjectAttr(dobj,"PORTABLE") == 0)
            {
                PrintStringCr("You can't take that.");
                return false;
            }
            return true;
        }


        bool check_light()
        {
            return PlayerCanSee();
        }

        bool check_enterable()
        {
            if (GetObjectAttr(dobj, "IN") > 127)
            {
                    PrintStringCr("You can't enter that.");
                    return false;
            }
            return true;
        }

        bool check_weight()
        {
            int sum = 0;
            for (int i = 2; i < objTable.GetCount(); i++)
            {
                if (PlayerHas(i))
                {
                    sum += objTable.GetObjAttr(i, "mass");
                }
            }

            if (sum + objTable.GetObjAttr(dobj, "mass") > MaxInvWeight)
            {
                PrintStringCr("Your load is too heavy.");
                return false;
            }
            return true;
        }

        bool check_dobj_supplied()
        {
            //if (dobj == -1)
            if (!dobjSupplied)
            {
                PrintStringCr("Missing noun.");
                return false;
            }
            return true;
        }

        bool check_iobj_supplied()
        {
            //if (iobj == -1)
            if (!dobjSupplied)
            {
                PrintStringCr("Missing noun.");
                return false;
            }
            return true;
        }


        bool check_prep_supplied()
        {
            if (prep == -1)
            {
                PrintStringCr("Missing preposition?");
                return false;
            }
            return true;
        }

        bool check_dobj_unlocked()
        {
             if (GetObjectAttr(dobj, "LOCKED") == 1)
            {
                
                PrintStringCr("The " + objTable.GetObj(dobj).printedName + " is locked.");
                return false;
            }
            return true;
        }

        bool check_dobj_lockable()
        {
            if (GetObjectAttr(dobj, "LOCKABLE") == 0)
            {
                PrintStringCr("That's not lockable.");
                return false;
            }
            return true;
        }

        bool check_dobj_locked()
        {
            if (GetObjectAttr(dobj, "LOCKED") == 0)
            {
                PrintStringCr("It's already unlocked.");
                return false;
            }
            return true;
        }

        bool check_iobj_container()
        {
            if (GetObjectAttr(dobj, "CONTAINER") == 0)
            {
                PrintStringCr("That's not a container.");
                return false;
            }
            return true;
        }

        bool check_player_has_dobj()
        {
            if (!VisibleAncestor(PLAYER, dobj))
            {
                PrintStringCr("You don't see that.");
                return false;
            }
            return true;
        }

        bool check_dobj_closed()
        {
            if (GetObjectAttr(dobj, "OPEN") == 0)
            {
                return true;
            }
            PrintStringCr("It's already closed.");
            return true;
        }


        bool check_dobj_opnable()
        {
            if (GetObjectAttr(dobj, "OPENABLE") == 0)
            {
                PrintStringCr("That's not openable.");
                return false;
            }
            if (GetObjectAttr(dobj, "LOCKED") == 1)
            {
                PrintStringCr("It' locked.");
                return false;
            }
            return true;
        }

        bool check_dobj_wearable()
        {
            if (GetObjectAttr(dobj, "WEARABLE") == 0)
            {
                PrintStringCr("You can't wear that.");
                return false;
            }
            return true;
        }

        bool check_dobj_open()
        {
            if (GetObjectAttr(dobj, "OPEN") == 1)
            {
                PrintStringCr("It's not open.");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool check_put()
        {
            if (prep == 0)
            {//in
                if (!IsContainer(iobj))
                {
                    PrintStringCr("You can't put things in that.");
                    return false;
                }

                if (GetObjectAttr(iobj,"OPEN")==0)
                {
                    PrintStringCr("It's closed.");
                    return false;
                }
            }
            else if (prep == 6)
            {//on

            }
            return true;
        }

        bool check_not_self_or_child()
        {
            if (dobj == iobj || ObjectHas(dobj, iobj))
            {
                PrintStringCr("That's not physically possible.");
                return false;
            }
            return true;
        }

        bool check_move()
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
                        return false;
                    }
                }

                return true;
            }
            else
            {
                newRoom = 255 - newRoom;
                PrintStringCr(nogoTable.GetEntry(newRoom + 1));
                return false;
            }

        }

        bool RunChecks()
        {
                
                foreach (Tuple <int,string> t in checkList)
                {
                    if (t.Item1 == verb)
                    {
                        try {
                            VerbCheckDlgt chk = checkTable[t.Item2];
                            if (!chk())
                            {
                                return false;
                            }
                        } 
                        catch (Exception e)
                        {
                            throw new Exception("Unable to run check [" + t.Item2 + "] for verb " + t.Item1, e);     
                        }

                    }
                }
            

            return true;
        }

        public override bool IsAsking()
        {
            return asking;
        }
    }
}
