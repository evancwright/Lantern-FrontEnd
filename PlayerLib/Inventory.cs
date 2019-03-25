using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerLib
{
    partial class Game
    {
     

        public void Inventory()
        {
            if (HasVisibleItems(PLAYER))
            {
                PrintStringCr("You are carrying...");
                ListContents(PLAYER, true);
            }
            else
            {
                PrintStringCr("You are empty handed.");
            }

        }



        void ListContents(int obj, bool printAdjs = false)
        {
            for (int i = 2; i < objTable.GetCount(); i++ )
            {
                if (objTable.GetObjAttr(i, "HOLDER") == obj)
                {
                    if (objTable.GetObjAttr(i, "SCENERY")==0)
                    {
                        PrintString("A ");
                        PrintObjectName(i);
                        if (printAdjs)
                        {
                            if (GetObjectAttr(i,"LIT")==1)
                                PrintString("(providing light)");
                            if (GetObjectAttr(i, "BEINGWORN") == 1)
                                PrintString("(being worn)");

                        }
                        PrintCr();

                        if (ShouldListContents(i))
                        {
                            if (GetObjectAttr(i, "SUPPORTER") == 1)
                            {
                                PrintString("On the ");
                                PrintObjectName(i);
                                PrintStringCr(" is...");
                            }
                            else
                            {
                                PrintString("The ");
                                PrintObjectName(i);
                                PrintStringCr(" contains...");
                            }
                            ListContents(i,printAdjs);
                        }
                    }
                }
            }
        }


        void ListRoomObjects()
        {
            int currentRoom = GetPlayerRoom();

            for (int i = 2; i < objTable.GetCount(); i++)
            {
                if (objTable.GetObjAttr(i, "HOLDER") == currentRoom)
                {
                    if (objTable.GetObjAttr(i, "SCENERY") == 0)
                    {

                        int strId = objTable.GetObjAttr(i, "INITIALDESCRIPTION");
                            

                        if (strId == -1 || strId == 255 || stringTable.GetEntry(strId) == "")
                        {
                            PrintStringCr("There is a " + objTable.objects[i].printedName + " here.");
                        }
                        else
                        {
                            PrintStringCr(objTable.GetObjAttr(i,"INITIALDESCRIPTION"));
                        }

                        if (ShouldListContents(i))
                        {
                            if (GetObjectAttr(i,"SUPPORTER")==1)
                            {
                                PrintString("On the ");
                                PrintObjectName(i);
                                PrintStringCr(" is...");
                            }
                            else
                            { 
                                PrintString("The ");
                                PrintObjectName(i);
                                PrintStringCr(" contains...");
                            }
                            ListContents(i);
                        }
                    }
                }
            }
        
        }

        bool ShouldListContents(int i)
        {
            if (objTable.GetObjAttr(i, "SUPPORTER")==1 ||
                (objTable.GetObjAttr(i, "CONTAINER")==1 && objTable.GetObjAttr(i, "OPEN")==1)
                )
            {
                if (HasVisibleItems(i))
                    return true;
            }
            return false;
        }

        void Indent()
        {
        }

        /// <summary>
        /// Returns true if obj2 is a visible child od obj1
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>        
        bool VisibleAncestor(int obj1, int obj2)
        {
        
            while (true)
            {
                int parent = GetObjectAttr(obj2, "HOLDER");

                if (parent == obj1)
                    return true;

                if (parent == OFFSCREEN)
                   return false;

                if (IsClosedContainer(parent))
                    return false;

                obj2 = parent;
            }

        }

    }
}
