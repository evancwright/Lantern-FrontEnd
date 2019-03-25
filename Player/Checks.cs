using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    
     

    partial class Game
    {
        delegate bool VerbCheckDlgt();

        Dictionary<int,List<string>> checks = new Dictionary<int,List<string>>();
        Dictionary<string, VerbCheckDlgt> checkTable = new Dictionary<string, VerbCheckDlgt>();

        void BuildCheckTable()
        {
            checkTable.Clear();

            //create jump table
            checkTable.Add("check_see_dobj", new VerbCheckDlgt(check_see_dobj));
            checkTable.Add("check_have_dobj", new VerbCheckDlgt(check_have_dobj));
            checkTable.Add("check_have_iobj", new VerbCheckDlgt(check_have_iobj));
            checkTable.Add("check_dobj_portable", new VerbCheckDlgt(check_dobj_portable));
            checkTable.Add("check_dobj_open", new VerbCheckDlgt(check_dobj_open));
            checkTable.Add("check_dobj_opnable", new VerbCheckDlgt(check_dobj_opnable));

             

        }

        bool check_see_dobj()
        {
            return true; //legacy
        }

        bool check_have_dobj()
        {
            if (!VisibleAncestor(PLAYER,dobj))
            {
                PrintStringCr("YOU DON'T SEE THAT.");
            }
            return false;
        }

        bool check_have_iobj()
        {
            if (!VisibleAncestor(PLAYER, iobj))
            {
                PrintStringCr("YOU DON'T SEE THAT.");
            }
            return false;
        }

        bool check_dobj_portable()
        {
            if (GetObjectAttr(dobj,"PORTABLE") == 1)
            {
                PrintStringCr("THAT IS FIXED IN PLACE.");
                return false;
            }
            return true;
        }

        bool check_iobj_container()
        {
            if (GetObjectAttr(dobj, "CONTAINER") == 0)
            {
                PrintStringCr("THAT'S NOT A CONTAINER.");
                return false;
            }
            return true;
        }

        bool check_player_has_dobj()
        {
            return true;
        }

        bool check_dobj_opnable()
        {
            return true;
        }

        bool check_dobj_open()
        {
            return true;
        }

        bool RunChecks()
        {
            try
            {
                List<string> chks = checks[verb];

                foreach (string fn in chks)
                {
                    VerbCheckDlgt chk = checkTable[fn];
                    if (!chk())
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                return true; //verb had no checks
            }
            return true;
        }
    }
}
