using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangDef
{
    public class OpCodeDef
    {
        public string Name { get; set; } //Pneumonic

        List<OperandMode> modes = new List<OperandMode>();

        public OpCodeDef(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Adds an addressing mode to an opcode
        /// </summary>
        /// <param name="mode"></param>
        public void AddMode(OperandMode mode, byte[] hexVals)
        {
            if (!modes.Contains(mode))
                modes.Add(mode);

            if (!hexValues.ContainsKey(mode))
                hexValues.Add(mode, hexVals);

        }

        /// <summary>
        /// Returns whether or nor an opcode supports an addressing mode
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool SupportsMode(string modeStr)
        {
            try
            {
                OperandMode mode = LangDef.ModeStringToMode(modeStr);

                return modes.Contains(mode);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true, if is in the for $0000 or a label
        /// </summary>
        /// <returns></returns>
        static bool IsAddress(string s)
        {
            try
            {
                if (s.First() == '@') return true;
                //double check is not a register
                if (Char.IsLetter(s.First()))
                {
                    return true;
                }

                if (s.First() == '$' && s.Length == 5)
                {
                    for (int i = 1; i < s.Length; i++)
                    {
                        if (!s[i].IsHexSymbol())
                            return false;
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        static bool IsImmediate(string s)
        {
            try
            {
                if (s[0] == '#')
                {
                    if (s.Length == 3 || s.Length == 5)
                    {
                        for (int i = 1; i < s.Length; i++)
                        {
                            if (!Char.IsDigit(s[i]))
                                return false;
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }

        }

        public string GetHextText(OperandMode mode)
        {
            string s = "";
            byte[] bytes = hexValues[mode];

            for (int i = 0; i < bytes.Length; i++)
            {
                s += String.Format("{0:X2}", bytes[i]);
            }

            return s;
        }

        //returns the bytes for the opcode in the specified mode
        public byte[] GetBinary(OperandMode om)
        {
            if (hexValues.ContainsKey(om))
            {
                return hexValues[om];
            }
            throw new Exception("Unable to get binary for opcode " + Name + " in mode " + om.ToString());
        }

        //supported modes

        //static Dictionary<OperandMode, LasmOpCode> binValues = new Dictionary<OperandMode, LasmOpCode>();
        Dictionary<OperandMode, byte[]> hexValues = new Dictionary<OperandMode, byte[]>();


    }

}
