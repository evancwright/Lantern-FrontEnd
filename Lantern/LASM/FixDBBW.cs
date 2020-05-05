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
        /// Replaces DB and DW statements that are labels with the addresses
        /// </summary>
        void ReplaceDBWithAddresses()
        {
            try
            {

                foreach (Statement st in statements)
                {
                    if (st is SpaceAllocation)
                    {
                        SpaceAllocation sp = st as SpaceAllocation;

                        sp.ReplaceLabels(labelOffsets);

                    }
                }
            }
            catch   (Exception e)
            {
                throw new Exception("Error replacing labels in DB/BW statements.", e);
            }
        }
    }
}
