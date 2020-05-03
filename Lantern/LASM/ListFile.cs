using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASM
{
    partial class Assembly
    {
        /// <summary>
        /// Sets the hex text of every executable statement
        /// </summary>
        public void SetHexText()
        {
            foreach (Statement st in statements)
            {
                try
                {
                    if (st is IWritable)
                        st.HexText = st.AsHextText();
                }
                catch (Exception e)
                {
                    throw new Exception("Error on line " +  st.LineNumber + " processing:" + st.Text, e);
                }
            }
        }

        public void WriteListFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (Statement st in statements)
                {
                    try
                    {

                        sw.WriteLine("{0}\t{4:X4}\t{1}\t\t\t{2}\t\t\t{3}", st.LineNumber, st.Label, st.AsHextText(), st.Text, st.Address);
                    }
                    catch  (Exception e)
                    {
                        throw new Exception("Unable to write line to list file:" + st.Text, e);
                    }
                }
            }
        }
    }
}
