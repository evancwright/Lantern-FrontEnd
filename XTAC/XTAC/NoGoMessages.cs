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
    public partial class NoGoMessages : Form
    {
        public string RoomName { get; set; }
        Object obj=null;

        public NoGoMessages(int roomNum) : base()
        {
            InitializeComponent();

            obj = Lantern.xproject.Project.Objects.Object[roomNum];
            
            label10.Text = "Blocked movement message for room: " + obj.Name;
            if (obj.Nogo == null)
                obj.Nogo = new Nogo();
            textBox1.Text = obj.Nogo.N;
            textBox2.Text = obj.Nogo.S;
            textBox3.Text = obj.Nogo.E; 
            textBox4.Text = obj.Nogo.W;
            textBox5.Text = obj.Nogo.Ne;
            textBox6.Text = obj.Nogo.Se;
            textBox7.Text = obj.Nogo.Sw;
            textBox8.Text = obj.Nogo.Nw;
            textBox9.Text = obj.Nogo.Up;
            textBox10.Text = obj.Nogo.Down;
            textBox11.Text = obj.Nogo.In;
            textBox12.Text = obj.Nogo.Out;

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            obj.Nogo.N = textBox1.Text;
            obj.Nogo.S = textBox2.Text;
            obj.Nogo.E = textBox3.Text;
            obj.Nogo.W = textBox4.Text;
            obj.Nogo.Ne = textBox5.Text;
            obj.Nogo.Se = textBox6.Text;
            obj.Nogo.Sw = textBox7.Text;
            obj.Nogo.Nw = textBox8.Text;
            obj.Nogo.Up = textBox9.Text;
            obj.Nogo.Down = textBox10.Text;
            obj.Nogo.In = textBox11.Text;
            obj.Nogo.Out = textBox12.Text;

            Close();
        }
    }
}
