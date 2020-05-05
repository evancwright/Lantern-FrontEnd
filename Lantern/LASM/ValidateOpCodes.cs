using System;
using System.Collections.Generic;

namespace LASM
{ 
    partial class Assembly
    {
      
        void ValidateOpCodes()
        {
            try
            {
                foreach (Statement s in statements)
                {
                    if (s is ExecutableStatement)
                    {
                        ExecutableStatement ex = s as ExecutableStatement;
                        if (!LangDef.LangDef.HasOpCode(ex.OpCode))
                        {
                            throw new Exception("Error on line: " + ex.LineNumber + ". Invalid opcode " + ex.OpCode);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating op codes.", ex);
            }
        }


        /// <summary>
        /// Returns everything past the op code and up to a ;
        /// Assumes this line is not a label or empty or comment
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        string GetOperands(string statement)
        {
            statement = statement.Trim();

            //remove any comments
            string comment;
            statement = RemoveComments(statement,out comment);

            int ix = statement.IndexOf(" ");
            if (ix == -1)
                return ""; 
            else
            {
                return statement.Substring(0, ix);    
            }
        }

        /// <summary>
        /// Removes any comment trailing the statement
        /// (if needed)
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        string RemoveComments(string statement, out string comment)
        {
            int semicolonIx = statement.IndexOf(";");
            if (semicolonIx != -1)
            {
                comment = statement.Substring(semicolonIx);
                return statement.Substring(0, semicolonIx);
            }
            else
            {
                comment = "";
                return statement;
            }
        }
    }
}
