using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGame
{

    public abstract class IGame
    {
        
        public abstract void AddVar(string varName, int amt);
        public abstract void Debug(string s);
        public abstract int GetObjectAttr(int id, string name);
        public abstract int GetObjectId(string name);
        public abstract int GetObjectProp(int id, string name);
        public abstract int GetStringId(string s);
        public abstract int GetFailStringId(string s);
        public abstract int GetVarVal(string name);
        public abstract bool IsVariable(string name);
        public abstract bool IsFunction(string name);
        public abstract void Look();
        public abstract void Move();
        public abstract void PrintString(int index);
        public abstract void PrintString(string message);
        public abstract void PrintStringCr(string str);
        public abstract void PrintStringCr(int strId);
        public abstract void PrintCr();
        public abstract void Ask();
        public abstract void PrintObjectName(int id);
        public abstract void SetObjectAttr(int id, string name, int val);
        public abstract void SetVar(string name, int val);
        public abstract void CallFunction(string name);
        public abstract bool PlayerHas(int id);
        public abstract bool ObjectHas(int parent, int child);
        public abstract bool ObjectSees(int parent, int child);
        public abstract bool IsAsking();
    }

    public interface IGameXml
    {
        int GetObjectId(string name);
        int GetStringId(string s);
        int GetFailStringId(string s);
        bool IsVariable(string name);
        string GetVarAddr(string name);
        bool IsFunction(string name);
    }


    /// <summary>
    /// Use to loosely couple the game to an output window
    /// </summary>

    public interface ITextWindow
    {
        string Text { get; set; }
        void ScrollToCaret();
        int TextLength { get; }
        int SelectionStart { get; set; }
        int SelectionLength { get; set; }
        void DeselectAll();
        void Refresh();
    }

}
