using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlayerLib;
using IGame;

namespace XTAC
{
    public partial class TestClient : Form
    {
        Game game;
        string fileName;
        List<string> commandHist = new List<string>();
        int historyIndex = -1;

        public TestClient()
        {
            InitializeComponent();
        }

        private void TestClient_Load(object sender, EventArgs e)
        {

        }

        public void SetFile(string fileName)
        {
            this.fileName = fileName;
            game = Game.GetInstance();
            game.SetOutputWindow(outputWindow);
            game.SetGameData(fileName);
            game.Run();
            //outputWindow.DeselectAll();
            outputWindow.SelectionStart = outputWindow.Text.Length;
            outputWindow.DeselectAll();
        }

        private void ReloadBtn_Click(object sender, EventArgs e)
        {
            if (fileName != null)
            {
                outputWindow.Text = "";
                game.SetGameData(fileName);
                game.Run();
            }
        }

        private void outputWindow_TextChanged(object sender, EventArgs e)
        {

        }

        private void inputWindow_TextChanged(object sender, EventArgs e)
        {

        }

        private void inputWindow_KeyPress(object sender, KeyPressEventArgs e)
        {/*
            if (game != null)
            {
                if (e.KeyChar == '\r')
                {
                    outputWindow.AppendText(">" + inputWindow.Text.Trim().ToUpper() + "\r\n\r\n");

                    game.AcceptCommand(inputWindow.Text.Trim());


                    inputWindow.Clear();
                    inputWindow.Focus();

                    outputWindow.SelectionStart = outputWindow.TextLength;
                    outputWindow.ScrollToCaret();
                    e.Handled = true;
                }
            }*/
        }

        private void ReplyBtn_Click(object sender, EventArgs e)
        {

        }

        private void outputWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            outputWindow.SelectionStart = outputWindow.Text.Length ;// add some logic if length is 0
            outputWindow.SelectionLength = 0;

            if (e.KeyChar  == '\b' )
            {
                if (outputWindow.Text[outputWindow.Text.Length-1] != '>')
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

                    commandHist.Add(command);
                    historyIndex = commandHist.Count - 1;
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

        private void outputWindow_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 38)
            {//key up
                if (historyIndex > 0)
                {
                    historyIndex -= 1;
                    
                }
            }
             
        }
    }
}
