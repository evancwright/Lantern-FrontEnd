using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLL;
using IGame;

namespace TestHarness
{
    class TestGame : IGame.IGame, IGame.IGameXml
    {
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


        public TestGame()
        {
            vars.Add("gameover", 0);
            vars.Add("y", 2);
            vars.Add("health", 2);

        }

        public override int GetObjectId(string name)
        {
            if (name == "player")
                return 1;
            return 0;
        }

        public override void AddVar(string varName, int amt) { }
        public override void Debug(string s) { }
        public override int GetObjectAttr(int id, string name)
        {
            return 12;
        }
        //        public abstract int GetObjectId(string name);
        public override int GetObjectProp(int id, string name) { return 0; }
        //        public abstract int GetStringId(string s);
        public override bool IsVariable(string name) { return vars.Keys.Contains(name); }
        public override void Look() { }
        public override void Move() { }
        public override void PrintString(int index) { }
        public override void PrintString(string message) { }
        public override void PrintStringCr(string str) { }
        public override void PrintStringCr(int strId) { }
        public override void SetObjectAttr(int id, string name, int val) { }

        public override int GetVarVal(string name)
        {
            int temp = vars[name];
            return temp;
        }

        public override void SetVar(string name, int value)
        {
            vars[name] = value;
        }

        public override int GetStringId(string s)
        {
            if (s == "hi")
                return 0;
            if (s == "hello")
                return 1;

            return 5;
        }

        public override void PrintCr()
        {
            Console.WriteLine("");
        }

        public string GetVarAddr(string var)
        {
            return var;
        }

        public override void CallFunction(string name)
        {
            Console.WriteLine("call " + name);
        }

        public override bool PlayerHas(int id)
        {
            return false;
        }

        public override bool ObjectHas(int parent, int child)
        {
            return false;
        }

    }
}
