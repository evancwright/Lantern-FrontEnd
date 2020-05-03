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
        /// Assigns labels to addresses
        /// </summary>
        void AssignAddresses()
        {
            ushort addr = 0;

            foreach (Statement st in statements)
            {
                st.Address = addr;

                addr += st.GetBinaryLength();
            }
        }

        /// <summary>
        /// Calculates the address of every global symbol
        /// </summary>
        void GetLabelAddresses()
        {

            //get the global labels
            foreach (Statement st in statements)
            {
                if (st.Label != "")
                {
                    if (globalLabels.Contains(st.Label))
                    {
                        labelOffsets.Add(st.Label, st.Address);
                    }
                    else
                    {
                        /* string labelName = "MOD" + st.ModuleNumber + "_" + st.Label;

                        if (!labelOffsets.ContainsKey(labelName))
                            labelOffsets.Add(labelName, st.Address);
                        else
                            throw new Exception("Line " + st.LineNumber + ":Duplicate local label:" + labelName);
                      */

                        st.Label = "MOD" + st.ModuleNumber + "_" + st.Label;
                        if (!labelOffsets.ContainsKey(st.Label))
                            labelOffsets.Add(st.Label, st.Address);
                        else
                            throw new Exception("Line " + st.LineNumber + ":Duplicate local label:" + st.Label);
                    }
                }
            }
            
        }


        /// <summary>
        /// Assigns actual address to operands in the executable statement
        /// </summary>
        void SetAddresses()
        {
            foreach (Statement st in statements)
            {
                if (st is ExecutableStatement)
                {
                    ExecutableStatement ex = st as ExecutableStatement;

                    try
                    {
                        ex.SetAddresses(labelOffsets);
                    }
                    catch (Exception x)
                    {
                        throw new Exception(
                            string.Format("Line {0}: Unable to find label's address.", st.LineNumber)
                            , x);
                    }

                }
            }
        }


        /// <summary>
        /// If the instruction is a relative jump, the address
        /// is converted into signed byte. 2 is subtracted from
        /// the offset to take into account that PC has already
        /// been incremented when the jump needs to happed
        /// </summary>
        void FixRelativeAddresses()
        {
            foreach (Statement st in statements)
            {
                if (st is ExecutableStatement)
                {
                    ExecutableStatement ex = st as ExecutableStatement;
                    if (LangDef.LangDef.IsRelativeJump(ex.OpCode))
                    {
                        ImmediateOperand op = (ex.LeftOperand as ImmediateOperand);
                        int target = op.UshortVal;
                        int relDist =  (target - st.Address) -2;
                        op.UshortVal = (byte)relDist;
                    }
                }
            }
        }
    }
}
