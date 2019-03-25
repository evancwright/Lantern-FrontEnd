using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Emitters;
//using XMLtoAdv;
using GameTables;

namespace PlayerLib
{
    class ObjWordTable
    {
        public Dictionary<int, List<int>> entries = new Dictionary<int, List<int>>();

        public ObjWordTable()
        {

        }


        public void AddNewEntry(int id, List<string> toks, Table dictionary)
        {
            List<int> nouns = new List<int>();

            foreach (string s in toks)
            {
                int sid = dictionary.GetEntryId(s);

                if (sid != -1)
                {
                    nouns.Add(sid);
                }
            }

            entries[id] = nouns;
        }

        public void AppendSynonyms(int id, List<string> toks, Table dictionary)
        {
            foreach (string s in toks)
            {
                int sid = dictionary.GetEntryId(s);

                if (sid != -1)
                {
                    entries[id].Add(sid);
                }
            }
            
        }
        public bool Matches(int id, int word)
        {
            List<int> words = entries[id];

            foreach (int w in words)
            {
                if (w == word)
                    return true;
            }
            return false;
        }

        public int GetCount()
        {
            return entries.Count;
        }
    }

     

}
