using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IGame;

namespace XTAC
{
    class TextWindow : TextBox, ITextWindow
    {
        public TextWindow() : base()
        {
            Location = new Point(13, 27);
            Size = new Size(514, 277);
            ScrollBars = ScrollBars.Vertical;
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
    }
}
