using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTAC;
using System.Collections;
//using Emitters;
using GameTables;

namespace PlayerLib
{

    class ObjTable
    {
        public Dictionary<int, ObjTableEntry> objects = new Dictionary<int, ObjTableEntry>();

        public ObjTable()
        {

        }

        public ObjTableEntry GetObj(int index)
        {
            return objects[index];
        }

        public void Add(ObjTableEntry ote)
        {
            objects[ote.GetObjAttr("id")] = ote;
        }

        public void SetObjAttr(int id, string name, int val)
        {
            if (id == -1)
                throw new Exception("Error: trying to set " + name + " attr on object -1");
            
                objects[id].SetObjAttr(name, val);
        }


        public int GetObjAttr(int id, string name)
        {
            if (id == -1)
                throw new Exception("Error: trying to get " + name + " attr on object -1");
 
            return objects[id].GetObjAttr(name);
        }

        public int GetCount()
        {
            return objects.Count;
        }

        public void ClearScores()
        {
            foreach (ObjTableEntry ote in objects.Values)
            {
                ote.score = 0;
            }
        }

        public void IncScore(int id)
        {
            objects[id].score++;
        }


        //return -1 if not found
        public int GetHighestScoringObj()
        {
            int highest = 0;
            int id = -1;

            for (int i = 0; i < objects.Count; i++ )
            {
                if (objects[i].score > highest)
                {
                    id = i;
                    highest = objects[i].score;
                }
            }

            return id;
        }
    }
     
    class ObjTableEntry
    {
        public int score;
        public string name;
        public string printedName;
        public List<string> synonyms;
        Dictionary<string, int> attrs = new Dictionary<string, int>(); //attrs and props

        public ObjTableEntry(GameObject gobj, Table stringTable)
        {
            score = 0;
            name = gobj.name;
            printedName = gobj.printedName;

            synonyms = gobj.synonyms;
            attrs["ID"] = gobj.id;
            //attrs["NAME"] = stringTable.GetEntryId(gobj.name);
            int strId = stringTable.GetEntryId(gobj.description);
            attrs["DESCRIPTION"] = strId;
            attrs["HOLDER"] = gobj.holder;
            attrs["INITIAL_DESCRIPTION"] = stringTable.GetEntryId(gobj.initialdescription);
            
            for (int i=0; i < GameObject.attribNames.Length; i++)
            {
                attrs[GameObject.attribNames[i].ToUpper()] = gobj.attribs[i] ;
            }

            for (int i = 0; i < GameObject.xmlFlagNames.Length; i++)
            {
                if (GameObject.xmlFlagNames[i].ToUpper()  == "EMITTINGLIGHT")
                    attrs["LIT"] = gobj.properties[i];                
                else
                    attrs[GameObject.xmlFlagNames[i].ToUpper()] = gobj.properties[i];
            }
        }

        public string GetObjName()
        {
            return name;
        }

        public int GetObjAttr(string name)
        {
            if (name.Equals("INITIALDESCRIPTION"))
                name = "INITIAL_DESCRIPTION";

            try
            {
                return attrs[name.ToUpper()];
            }
            catch (Exception ex)
            {
                throw new Exception("Bad attribute: " + name, ex);
            }
        }

        public void SetObjAttr(string name, int val)
        {
            if (name.ToUpper() == "INITIALDESCRIPTION")
                name = "INITIAL_DESCRIPTION";

            if (attrs.ContainsKey(name.ToUpper()))
            {
                attrs[name.ToUpper()] = val;
            }
            else
            {
                throw new Exception("Trying to set invalid attribute: " + name + " on object " + attrs["ID"]);
            }
        }


    }


    /*
    static class Extensions
    {
        public static ObjTableEntry ToTableEntry(this XTAC.Object obj)
        {
            ObjTableEntry o = new ObjTableEntry();

            


            return o;
        }
    }
    */
}
