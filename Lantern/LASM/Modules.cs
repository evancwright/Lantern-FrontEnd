using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASM
{
    partial class Assembly
    {
        public void AssignModuleNumbers()
        {
            int moduleNumber = 1;

            foreach (Statement s in statements)
            {
                if (s is Module)
                {
                    moduleNumber++;
                }
                s.ModuleNumber = moduleNumber;
            }
        }
    }
}
