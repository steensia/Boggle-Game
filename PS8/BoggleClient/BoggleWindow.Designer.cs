namespace BoggleClient
{
    partial class BoggleWindow
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
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.DomainBox = new System.Windows.Forms.TextBox();
            this.DomainLabel = new System.Windows.Forms.Label();
            this.TimerBox = new System.Windows.Forms.TextBox();
            this.TimerLabel = new System.Windows.Forms.Label();
            this.RequestButton = new System.Windows.Forms.Button();
            this.CancelRequestButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Help = new System.Windows.Forms.ToolStripMenuItem();
            this.ScoreLabel = new System.Windows.Forms.Label();
            this.ScoreBox = new System.Windows.Forms.TextBox();
            this.WordBox = new System.Windows.Forms.TextBox();
            this.WordBoxLabel = new System.Windows.Forms.Label();
            this.WordList = new System.Windows.Forms.RichTextBox();
            this.BoggleBoard = new SSGui.SpreadsheetPanel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsernameBox
            // 
            this.UsernameBox.Location = new System.Drawing.Point(86, 30);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(100, 20);
            this.UsernameBox.TabIndex = 0;
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(12, 33);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(68, 15);
            this.UsernameLabel.TabIndex = 1;
            this.UsernameLabel.Text = "Username:";
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(499, 28);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(418, 28);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(75, 23);
            this.RegisterButton.TabIndex = 3;
            this.RegisterButton.Text = "Register";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // DomainBox
            // 
            this.DomainBox.Location = new System.Drawing.Point(254, 30);
            this.DomainBox.Name = "DomainBox";
            this.DomainBox.Size = new System.Drawing.Size(158, 20);
            this.DomainBox.TabIndex = 4;
            // 
            // DomainLabel
            // 
            this.DomainLabel.AutoSize = true;
            this.DomainLabel.Location = new System.Drawing.Point(194, 33);
            this.DomainLabel.Name = "DomainLabel";
            this.DomainLabel.Size = new System.Drawing.Size(54, 15);
            this.DomainLabel.TabIndex = 5;
            this.DomainLabel.Text = "Domain:";
            // 
            // TimerBox
            // 
            this.TimerBox.Location = new System.Drawing.Point(639, 30);
            this.TimerBox.Name = "TimerBox";
            this.TimerBox.Size = new System.Drawing.Size(100, 20);
            this.TimerBox.TabIndex = 6;
            this.TimerBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StartGame);
            // 
            // TimerLabel
            // 
            this.TimerLabel.AutoSize = true;
            this.TimerLabel.Location = new System.Drawing.Point(595, 33);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(38, 15);
            this.TimerLabel.TabIndex = 7;
            this.TimerLabel.Text = "Time:";
            // 
            // RequestButton
            // 
            this.RequestButton.Location = new System.Drawing.Point(15, 131);
            this.RequestButton.Name = "RequestButton";
            this.RequestButton.Size = new System.Drawing.Size(102, 23);
            this.RequestButton.TabIndex = 8;
            this.RequestButton.Text = "Request User";
            this.RequestButton.UseVisualStyleBackColor = true;
            this.RequestButton.Click += new System.EventHandler(this.RequestButton_Click);
            // 
            // CancelRequestButton
            // 
            this.CancelRequestButton.Location = new System.Drawing.Point(123, 131);
            this.CancelRequestButton.Name = "CancelRequestButton";
            this.CancelRequestButton.Size = new System.Drawing.Size(102, 23);
            this.CancelRequestButton.TabIndex = 9;
            this.CancelRequestButton.Text = "Cancel Request";
            this.CancelRequestButton.UseVisualStyleBackColor = true;
            this.CancelRequestButton.Click += new System.EventHandler(this.CancelRequestButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Help});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(780, 27);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Help
            // 
            this.Help.Name = "Help";
            this.Help.Size = new System.Drawing.Size(49, 23);
            this.Help.Text = "Help";
            this.Help.Click += new System.EventHandler(this.Help_Click);
            // 
            // ScoreLabel
            // 
            this.ScoreLabel.AutoSize = true;
            this.ScoreLabel.Location = new System.Drawing.Point(595, 59);
            this.ScoreLabel.Name = "ScoreLabel";
            this.ScoreLabel.Size = new System.Drawing.Size(42, 15);
            this.ScoreLabel.TabIndex = 13;
            this.ScoreLabel.Text = "Score:";
            // 
            // ScoreBox
            // 
            this.ScoreBox.Enabled = false;
            this.ScoreBox.Location = new System.Drawing.Point(639, 56);
            this.ScoreBox.Name = "ScoreBox";
            this.ScoreBox.ReadOnly = true;
            this.ScoreBox.Size = new System.Drawing.Size(100, 20);
            this.ScoreBox.TabIndex = 14;
            // 
            // WordBox
            // 
            this.WordBox.Location = new System.Drawing.Point(94, 488);
            this.WordBox.Name = "WordBox";
            this.WordBox.Size = new System.Drawing.Size(240, 20);
            this.WordBox.TabIndex = 15;
            this.WordBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WordBox_KeyDown);
            // 
            // WordBoxLabel
            // 
            this.WordBoxLabel.AutoSize = true;
            this.WordBoxLabel.Location = new System.Drawing.Point(9, 491);
            this.WordBoxLabel.Name = "WordBoxLabel";
            this.WordBoxLabel.Size = new System.Drawing.Size(79, 15);
            this.WordBoxLabel.TabIndex = 16;
            this.WordBoxLabel.Text = "Enter a word:";
            // 
            // WordList
            // 
            this.WordList.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.WordList.Location = new System.Drawing.Point(355, 160);
            this.WordList.Name = "WordList";
            this.WordList.ReadOnly = true;
            this.WordList.Size = new System.Drawing.Size(138, 322);
            this.WordList.TabIndex = 17;
            this.WordList.Text = "";
            // 
            // BoggleBoard
            // 
            this.BoggleBoard.Enabled = false;
            this.BoggleBoard.Location = new System.Drawing.Point(15, 160);
            this.BoggleBoard.Name = "BoggleBoard";
            this.BoggleBoard.Size = new System.Drawing.Size(322, 322);
            this.BoggleBoard.TabIndex = 18;
            // 
            // BoggleWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 538);
            this.Controls.Add(this.BoggleBoard);
            this.Controls.Add(this.WordList);
            this.Controls.Add(this.WordBoxLabel);
            this.Controls.Add(this.WordBox);
            this.Controls.Add(this.ScoreBox);
            this.Controls.Add(this.ScoreLabel);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.CancelRequestButton);
            this.Controls.Add(this.RequestButton);
            this.Controls.Add(this.TimerLabel);
            this.Controls.Add(this.TimerBox);
            this.Controls.Add(this.DomainLabel);
            this.Controls.Add(this.DomainBox);
            this.Controls.Add(this.RegisterButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.UsernameBox);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BoggleWindow";
            this.Text = "Boggle";
            this.Load += new System.EventHandler(this.BoggleWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.TextBox DomainBox;
        private System.Windows.Forms.Label DomainLabel;
        private System.Windows.Forms.TextBox TimerBox;
        private System.Windows.Forms.Label TimerLabel;
        private System.Windows.Forms.Button RequestButton;
        private System.Windows.Forms.Button CancelRequestButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Help;
        private System.Windows.Forms.Label ScoreLabel;
        private System.Windows.Forms.TextBox ScoreBox;
        private System.Windows.Forms.TextBox WordBox;
        private System.Windows.Forms.Label WordBoxLabel;
        private System.Windows.Forms.RichTextBox WordList;
        private SSGui.SpreadsheetPanel BoggleBoard;
    }
}

