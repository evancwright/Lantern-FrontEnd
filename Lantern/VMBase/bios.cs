using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VMBase
{
    partial class VMBase
    {
        bool bQuit = false;
        Action<byte> chout;
        int cmdIndex = 0;
        string[] cmds = {"read note", "i", "n" , "e","s","open locker"};

        public void SetTextWriter(Action<byte> callback)
        {
            chout = callback;
        }

        public virtual void cls()
        {
        }

        /// <summary>
        /// hl contains start of data,
        /// de contains end of data
        /// </summary>
        public virtual void save()
        {
        }

        /// <summary>
        /// hl contains start of data,
        /// de contains end of data
        /// </summary>
        public virtual void restore()
        {
        }

        void quit()
        {
            bQuit = true;
        }

        public virtual void status_line()
        {
        }

        public virtual void anykey()
        {
            Console.ReadKey();
        }

        //hl contains string
        public virtual void charout()
        {
            chout(a);
        }

        public virtual void printcr()
        {
            chout((byte)'\r');
            chout((byte)'\n');
        }

        //reads chars into address stored in HL
        public virtual void readkb()
        {   
           string s = cmds[cmdIndex];
            cmdIndex++;
            if (cmdIndex == cmds.Length) cmdIndex = 0;

            int i = 0;
            for (;  i < Math.Min(s.Length,40); i++)
            {
                WriteByte((ushort)(hl+i), (byte)s[i]);
            }
            WriteByte((ushort)(hl + i) ,0);

        }

        /// <summary>
        /// 
        /// a contains param number
        /// hl contains where to put value
        /// a=0 screen width
        /// a=1 screen height
        /// </summary>
        public virtual void setp()
        {
            if (a == 0)
            {//width
                WriteByte(hl, 80);
            }
            else
            {//height (for scrolling)
                WriteByte(hl, 20);
            }
        }
    }
}
