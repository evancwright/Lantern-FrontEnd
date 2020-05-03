using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASM
{
    partial class Assembly
    {
        /// <summary>
        /// This function replaces Immediate Operands with 
        /// ImmediateByte and ImmediateWord operands based
        /// on what the instructions require
        /// </summary>
        void SetImmediateSizes()
        {
            for (int i = 0; i < statements.Count; i++)
            {
                Statement st = statements[i];

                if (st is ExecutableStatement)
                {
                    try
                    {
                        (st as ExecutableStatement).SetImmediateSizes();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            string.Format("Line {0}:  Unable to set immediate size: {1}",
                            st.LineNumber, st.Text), ex);
                    }
                }
            }
        }
    }
}
