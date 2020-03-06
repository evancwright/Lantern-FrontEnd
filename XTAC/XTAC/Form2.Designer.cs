namespace XTAC
{
    partial class TestClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
//            this.outputWindow = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // outputWindow
            // 
            this.outputWindow.Location = new System.Drawing.Point(1, 1);
            this.outputWindow.Multiline = true;
            this.outputWindow.Name = "outputWindow";
            this.outputWindow.Size = new System.Drawing.Size(434, 260);
            this.outputWindow.TabIndex = 0;
            this.outputWindow.TextChanged += new System.EventHandler(this.outputWindow_TextChanged);
            this.outputWindow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.outputWindow_KeyPress);
            this.outputWindow.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.outputWindow_PreviewKeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 268);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(414, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hint:  To speed up testing, you can enter a series of commands separated by comma" +
    "s.";
            // 
            // TestClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 290);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputWindow);
            this.Name = "TestClient";
            this.Text = "Test Client";
            this.Load += new System.EventHandler(this.TestClient_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private System.Windows.Forms.TextBox outputWindow;
        private System.Windows.Forms.Label label1;
    }
}