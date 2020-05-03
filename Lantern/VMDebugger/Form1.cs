using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using VMBase;
using LangDef;
 

namespace VMDebugger
{
    public partial class Form1 : Form
    {
        byte[] data;
        List<string> input = new List<string>();
        int clickCount = 0;
        VMBase.VMBase vm = new VMBase.VMBase();
        
        public Form1()
        {
            InitializeComponent();
            vm.SetTextWriter(CharOut);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                data = File.ReadAllBytes(ofd.FileName);
                vm.SetRam(data);
                button1.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                
                List<string> lines = new List<string>();
                LangDef.LangDef.Disassemble(data, 0, lines);

                IRText.Text = lines[0];

                foreach (string s in lines)
                {
                    textBox1.Text += s +"\r\n";
                }
                
//                LangDef.
            }
        }

        private void loadSymbolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //load the data into ram
            


        }

        private void button1_Click(object sender, EventArgs e)
        {
            label13.Text = (clickCount++).ToString();

            ushort oldPC = vm.PC;
            label12.Text = String.Format("{0:0000X}",vm.PC.ToString());
            vm.Fetch();

            List<string> lines = new List<string>();

            LangDef.LangDef.Disassemble(data, oldPC, lines);
            IRText.Text = lines[0];

            vm.Execute();
            
            ShowCPU();

            if (vm.PC >= data.Length)
            {
                MessageBox.Show("PC is out of range!");
            }
        }

        void CharOut(byte b)
        {
            char ch = (char) b;

            textBox3.Text += ch.ToString();

        }

        void ShowCPU()
        {
            textBox2.Text = String.Format("{0:X4}", vm.PC);
            textBox2.Refresh();
            RegAText.Text = String.Format("{0:X2}", vm.A);
            RegBCText.Text = String.Format("{0:X4}", vm.BC);
            RegDEText.Text = String.Format("{0:X4}", vm.DE);
            RegHLText.Text = String.Format("{0:X4}", vm.HL);
            RegIXText.Text = String.Format("{0:X4}", vm.IX);
            RegIYText.Text = String.Format("{0:X4}", vm.IY);
            SPText.Text = String.Format("{0:X4}", vm.SP);

            ZFText.Text = vm.ZF.ToString();
            MFText.Text = vm.MF.ToString();
            CFText.Text = vm.CF.ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string t = textBox5.Text;
            int i = 0;
            if (t.StartsWith("0x"))
            {
                string h = t.Substring(2);
                i = Convert.ToInt32(h, 16);
            }
            else
            {

            }

            textBox3.Text ="";

            for (int j=i; j < i+40; j++)
            {
                textBox3.Text += string.Format("{0:X2}",data[j]);
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i=0; i < 20; i++)
            //while (true)
            {
                try
                {
                    button1.PerformClick();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception:" + ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string t = textBox6.Text;

            if (t.StartsWith("0x") || t.StartsWith("0X"))
            {
                string h = t.Substring(2);
                int i = Convert.ToInt32(h, 16);

                while (vm.PC != i)
                {
                    try
                    {
                        button1.PerformClick();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception:" + ex.Message);
                        break;
                    }
                }
            }
        }
    }
}
