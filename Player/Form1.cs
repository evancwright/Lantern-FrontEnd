using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XMLtoAdv;
using System.IO;
using System.Xml.Serialization;
using XTAC;
using PlayerLib;
using IGame;

namespace Player
{
    public partial class Form1 : Form
    {
        Xml xproject;
        Game game;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Select an XML File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Xml.Serialization.XmlSerializer reader = new XmlSerializer(typeof(Xml));

                // Read the XML file.
               // StreamReader file = new StreamReader(openFileDialog1.FileName);
                string fileName = openFileDialog1.FileName;

//                // Deserialize the content of the file into a Book object.
  //              xproject = (Xml)reader.Deserialize(file);
                
  //              file.Close();
                outputWindow.Text = ""; 
                game = Game.GetInstance();
                game.SetOutputWindow(outputWindow);
                game.SetGameData(fileName);
                game.Run();
                outputWindow.SelectionStart = outputWindow.Text.Length;

            }
        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
          
        }

        private void outputWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (outputWindow.Text.Length > 0)
            {
                outputWindow.SelectionStart = outputWindow.Text.Length;// add some logic if length is 0
                outputWindow.SelectionLength = 0;

                if (e.KeyChar == '\b')
                {
                    if (outputWindow.Text[outputWindow.Text.Length - 1] != '>')
                    {
                        outputWindow.Text = outputWindow.Text.Remove(outputWindow.Text.Length - 1);
                    }
                    e.Handled = true;
                }
                else if (e.KeyChar == '\r')
                {
                    outputWindow.ScrollToCaret();
                    int start = outputWindow.Text.LastIndexOf('>');
                    string command = outputWindow.Text.Substring(start + 1);

                    try
                    {
                        game.AcceptCommand(command);
                    }
                    catch (Exception ex)
                    {

                      

                        MessageBox.Show(ex.Message);
                    }
                    e.Handled = true;
                }
                else
                {
                    outputWindow.ScrollToCaret();
                    outputWindow.Text += e.KeyChar;
                    e.Handled = true;
                }
                outputWindow.SelectionStart = outputWindow.Text.Length;
                outputWindow.ScrollToCaret();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("By Evan Wright, 2017");
        }

        private void outputWindow_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }
    }
}
