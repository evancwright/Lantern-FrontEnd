using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTAC
{
    class CmdHist
    {
        private CmdHist instance;

        public CmdHist GetInstance()
        {
            if (instance == null)
            {
                instance = new CmdHist();
            }
            return instance;
        }

        public void AddCommand(string input)
        {

        }
    }
}
