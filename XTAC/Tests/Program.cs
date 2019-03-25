using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerLib;
using XMLtoAdv;
using System.IO;
//using Emitters;
//using CLikeLang;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            string expr = "if ( player.holder == 5 || (player.holder == 3 && flashlight.lit == 1)) { $health=100; } ";

            using (StreamWriter sw = File.CreateText("Test.asm"))
            {

                Game g = Game.GetInstance();
                

                ProgramData pd = ProgramData.GetInstance();
                pd.SetProgramData(g);

                g.SetGameData("RichardMines5.xml");

                FunctionBuilder fb = new FunctionBuilder();
                Function fn = fb.CreateRoutine("test1", expr );

                Emitter6502 em02 = new Emitter6502(sw);
                fn.AcceptEmitter(em02);

            }
            */
        }
    }
}
