using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangDef;

namespace LASM
{
    partial class Assembly 
    {
        /// <summary>
        /// makes sure the operands are supported
        /// </summary>
        void ValidateOperands()
        {
            for (int i=0; i < statements.Count; i++) 
            {
                Statement s = statements[i];
                if (s is ExecutableStatement)
                {
                    ExecutableStatement ex = s as ExecutableStatement;
                    string mode = ex.GetModeText();
                    if (!LangDef.LangDef.SupportsMode(ex.OpCode, mode))
                    {//
                        throw new Exception(
                            String.Format(
                                "Error on line {0}:{3}  {1} does not support mode {2}",
                                i,
                                ex.OpCode,
                                mode,
                                ex.Text)
                                );
                    }
                }
            }

        }

    }
}
