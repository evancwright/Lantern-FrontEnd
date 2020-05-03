using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LASM;

namespace LASMCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                Assembly lasm = new Assembly(args[0]);
                lasm.Assemble();
            }
            else
            {
                Console.WriteLine("Usage: lasm file");
            }
        }
    }
}
