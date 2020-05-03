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
            Assembly lasm = new Assembly(args[0]);
            lasm.Assemble();
        }
    }
}
