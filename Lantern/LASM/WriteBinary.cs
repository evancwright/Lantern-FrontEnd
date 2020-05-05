using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LangDef;

namespace LASM
{
    partial class Assembly
    {
        /// <summary>
        /// Writes out the binary file
        /// </summary>
        /// <param name="fileName"></param>
        void WriteBinary(string fileName)
        {
            try
            {
                int byteCounter = 0;
                using (BinaryWriter bw = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate)))
                {
                    foreach (Statement st in statements)
                    {
                        if (st is OrgStatement)
                            byteCounter = (st as OrgStatement).Address;

                        if (st is IWritable)
                        {
                            if (byteCounter != st.Address)
                            {
                                throw new Exception(
                                    String.Format("Address mismatch at line {0}:{1}", st.Address, st.Text)
                                    );
                            }
                            /*
                            if (st is ExecutableStatement)
                            {
                                ExecutableStatement ex = st as ExecutableStatement;
                                byte[] op = LangDef.LangDef.Decode(ex.OpCode.GetBinary());
                                if (ex.GetBinaryLength() != )
                            }
                            */

                            //  IWritable iw = st as IWritable;
                            //  iw.WriteBinary(bw);
                            string s = st.HexText;

                            if (s != "")
                            {
                                string oldS = s;

                                //break string into groups of 2
                                while (s != "")
                                {
                                    try
                                    {
                                        string pair = s.Substring(0, 2);
                                        s = s.Substring(2);
                                        byte b = pair.ToByte();
                                        bw.Write(b);
                                        byteCounter++;
                                    }
                                    catch (Exception x)
                                    {
                                        throw new Exception("Error writing binary for " + st.Text);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error writing binary file.", ex);
            }
        }

    }
}
