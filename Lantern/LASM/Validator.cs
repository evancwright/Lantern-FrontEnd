using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASM
{

    interface IValidator
    {
        bool IsValid(ExecutableStatement ex);
    }

    class Validator : IValidator
    {
         

        public bool IsValid(ExecutableStatement ex)
        {
            if (LangDef.LangDef.HasOpCode(ex.OpCode))
            {
                throw new Exception("Invalid opcode:" + ex.OpCode);
            }

            return true;
        }

    }
}
