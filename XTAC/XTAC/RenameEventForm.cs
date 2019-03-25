using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XTAC
{
    public partial class RenameEventForm : Form
    {
        public string NewName { get; set; }
        public bool result=false;
        public string OldName
        { 

            set
            {
                oldNameTextBox.Text = value;
            }
        }
        public RenameEventForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            result = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewName = newNameTextBox.Text;
            NewName = NewName.Trim().Replace(' ', '_').Trim();
            
            if (newNameTextBox.Text.Length ==0)
            {
                MessageBox.Show("Name can't be blank.");
                return;
            }

            Close();
            result = true;
        }
    }
}
