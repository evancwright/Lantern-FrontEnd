using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLL;
using System.IO;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {     
                TestGame t = new TestGame();
 //               CodeParser p = new CodeParser(t);

                //string expr="if (gameover == 1) { gameover = 1 * 3 + 4 * 7; player.parent = 5;} ";
                 string expr = "if (health == 0) { if (gameover == 0) {health=55;} player.holder = 1;} else if (health == 1) { player.holder = 2; } else { println(\"hi\"); }";
                Console.WriteLine(expr + " should equal 31");

//                Body body = p.ParseFunction(new StringBuilder(expr));

                Function f = FunctionBuilder.BuildFunction(t, "test_func", expr);

                using (StreamWriter sw = File.CreateText("6502test.asm"))
                {
                    IVisitor v = new Visitor6502(t);
                    v.SetStreamWriter(sw);
                    f.Accept(v);
                }

                Console.ReadKey(); 
            }
            catch (Exception e)
            {
           
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }

}
