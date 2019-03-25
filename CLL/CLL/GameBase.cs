using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLL
{
    /// <summary>
    /// This interface should be implemented by a game
    /// </summary>
    public abstract class GameBase
    {
        protected static Random r = new Random();

        protected string[] attrs = {
                "id","parent","holder","name","description","mass","n","s","e","w","ne","nw","se","sw","up","down","in","out"
        };

        protected string[] props = {
            "portable",
            "lit",
            "openable",
            "open",
            "lockable",
            "locked"
        };

        protected Dictionary<string, int> vars = new Dictionary<string, int>();
        protected List<string> functionNames = new List<string>();

        public GameBase()
        {

        }

        public int GetVarValue(string name)
        {
            int temp = vars[name];
            return temp;
        }

        public void SetVarValue(string name, int value)
        {
            vars[name] = value;
        }

        public int GetAttrValue(int id, string attr) { return 0; }
        public void SetAttrValue(int id, string attr, int val) { }

        public int GetPropValue(int id, string attr) { return 0; }
        public void SetPropValue(int id, string attr, bool val) { }

        public void Print(string s) { }
        public void PrintLine(string s) { }

        public int Rand(int upper)
        {
            return r.Next(upper);
        }

        public bool IsVar(string s)
        {
            return vars.Keys.Contains(s);
        }

        public bool IsObject(string s)
        {
            return true;
        }
        
        public bool IsProperty(string s)
        {
            return props.ToList().Contains(s);
        }

        public bool IsAttr(string s)
        {
            return attrs.ToList().Contains(s);
        }

        public bool IsFunction(string s)
        {
            return functionNames.Contains(s);
        }

        /*
        public virtual int GetStringId(string s)
        {
            return 5;
        }

        /*
        int GetAttrPosition(string attr)
        {
            return 1;
        }

        int GetPropPosition(string prop)
        {
            return 1;
        }
        /*
        public int GetAttrIndex(string attr)
        {
            return 1;
        }

        public int GetPropIndex(string attr)
        {
            return 1;
        }
        */
        public virtual int GetObjectId(string name)
        {
            return 4;
        }

        public virtual int GetStringId(string s)
        {
            return 12;
        }
    }
}
