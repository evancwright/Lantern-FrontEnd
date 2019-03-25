using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GameTables
{
    public class Table
    {
        List<String> entries = new List<String>();
        
        public Table()
        {
        }

        public void Clear()
        {
            entries.Clear();
        }

        public void Add(string[] vals)
        {
            foreach (string s in vals)
            {
                AddEntry(s);
            }
        }

        public int  AddEntry(string name)
        {
            if (name == "")
            {
                throw new Exception("Attempt to add empty string to table");
            }

            name = name.Replace("\n","");
            name = name.Replace("\r", "");

            if (name.Length > 255)
            {
                name = name.Substring(0, 255);
            }

            if (GetEntryId(name) != -1)
            {//check for dupes
                return GetEntryId(name);
            }
            else
            {
                entries.Add(name);
                return entries.Count; //last index
            }
        }

       
        public int GetEntryId(string name)
        {
            if (name.Length > 255)
            {
                name = name.Substring(0, 255);
            }

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].ToUpper().Equals(name.ToUpper()))
                {
                    return i; //basic indexes start at 1
                }
            }

            return -1;
        }

        public string GetEntry(int id)
        {
            return entries[id];
        }

         
        public int GetNumEntries()
        {
            return entries.Count; //don't count dummy entry
        }
       
        public void Uppercase()
        {
            for (int i=0; i < entries.Count; i++)
            {
                entries[i] = entries[i].ToUpper();
            }
        }
    }
}
