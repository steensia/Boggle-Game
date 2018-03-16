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
            this.WordsListLable = new System.Windows.Forms.Label();
            this.Player2Label = new System.Windows.Forms.Label();
            this.Player2UsernameBox = new System.Windows.Forms.TextBox();
            this.Player2ScoreLable = new System.Windows.Forms.Label();
            this.Player2ScoreBox = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsernameBox
            // 
            this.UsernameBox.Location = new System.Drawing.Point(86, 57);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(179, 20);
            this.UsernameBox.TabIndex = 0;
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(12, 60);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(68, 15);
            this.UsernameLabel.TabIndex = 1;
            this.UsernameLabel.Text = "Username:";
            // 
            // CancelButton
            // 
            this.CancelButton.Enabled = false;
            this.CancelButton.Location = new System.Drawing.Point(379, 57);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(102, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(271, 57);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(102, 23);
            this.RegisterButton.TabIndex = 3;
            this.RegisterButton.Text = "Register";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // DomainBox
            // 
            this.DomainBox.Location = new System.Drawing.Point(72, 31);
            this.DomainBox.Name = "DomainBox";
            this.DomainBox.Size = new System.Drawing.Size(623, 20);
            this.DomainBox.TabIndex = 4;
            this.DomainBox.Text = "http://ice.eng.utah.edu/BoggleService.svc/";
            // 
            // DomainLabel
            // 
            this.DomainLabel.AutoSize = true;
            this.DomainLabel.Location = new System.Drawing.Point(12, 33);
            this.DomainLabel.Name = "DomainLabel";
            this.DomainLabel.Size = new System.Drawing.Size(54, 15);
            this.DomainLabel.TabIndex = 5;
            this.DomainLabel.Text = "Domain:";
            // 
            // TimerBox
            // 
            this.TimerBox.Enabled = false;
            this.TimerBox.Location = new System.Drawing.Point(271, 86);
            this.TimerBox.Name = "TimerBox";
            this.TimerBox.Size = new System.Drawing.Size(63, 20);
            this.TimerBox.TabIndex = 6;
            // 
            // TimerLabel
            // 
            this.TimerLabel.AutoSize = true;
            this.TimerLabel.Location = new System.Drawing.Point(227, 89);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(38, 15);
            this.TimerLabel.TabIndex = 7;
            this.TimerLabel.Text = "Time:";
            // 
            // RequestButton
            // 
            this.RequestButton.Enabled = false;
            this.RequestButton.Location = new System.Drawing.Point(487, 112);
            this.RequestButton.Name = "RequestButton";
            this.RequestButton.Size = new System.Drawing.Size(102, 23);
            this.RequestButton.TabIndex = 8;
            this.RequestButton.Text = "Request User";
            this.RequestButton.UseVisualStyleBackColor = true;
            this.RequestButton.Click += new System.EventHandler(this.RequestButton_Click);
            // 
            // CancelRequestButton
            // 
            this.CancelRequestButton.Enabled = false;
            this.CancelRequestButton.Location = new System.Drawing.Point(595, 112);
            this.CancelRequestButton.Name = "CancelRequestButton";
            this.CancelRequestButton.Size = new System.Drawing.Size(102, 23);
            this.CancelRequestButton.TabIndex = 9;
            this.CancelRequestButton.Text = "Cancel Request";
            this.CancelRequestButton.UseVisualStyleBackColor = true;
            this.CancelRequestButton.Click += new System.EventHandler(this.CancelRequestButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Help});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(707, 27);
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
            this.ScoreLabel.Location = new System.Drawing.Point(12, 86);
            this.ScoreLabel.Name = "ScoreLabel";
            this.ScoreLabel.Size = new System.Drawing.Size(42, 15);
            this.ScoreLabel.TabIndex = 13;
            this.ScoreLabel.Text = "Score:";
            // 
            // ScoreBox
            // 
            this.ScoreBox.Enabled = false;
            this.ScoreBox.Location = new System.Drawing.Point(60, 83);
            this.ScoreBox.Name = "ScoreBox";
            this.ScoreBox.ReadOnly = true;
            this.ScoreBox.Size = new System.Drawing.Size(161, 20);
            this.ScoreBox.TabIndex = 14;
            // 
            // WordBox
            // 
            this.WordBox.Enabled = false;
            this.WordBox.Font = new System.Drawing.Font("Comic Sans MS", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WordBox.Location = new System.Drawing.Point(121, 439);
            this.WordBox.Name = "WordBox";
            this.WordBox.Size = new System.Drawing.Size(213, 28);
            this.WordBox.TabIndex = 15;
            this.WordBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WordBox_KeyDown);
            // 
            // WordBoxLabel
            // 
            this.WordBoxLabel.AutoSize = true;
            this.WordBoxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WordBoxLabel.Location = new System.Drawing.Point(12, 443);
            this.WordBoxLabel.Name = "WordBoxLabel";
            this.WordBoxLabel.Size = new System.Drawing.Size(103, 20);
            this.WordBoxLabel.TabIndex = 16;
            this.WordBoxLabel.Text = "Enter a word:";
            // 
            // WordList
            // 
            this.WordList.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.WordList.Font = new System.Drawing.Font("dragon_alphabet", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WordList.Location = new System.Drawing.Point(343, 111);
            this.WordList.Name = "WordList";
            this.WordList.ReadOnly = true;
            this.WordList.Size = new System.Drawing.Size(138, 356);
            this.WordList.TabIndex = 17;
            this.WordList.Text = "";
            // 
            // BoggleBoard
            // 
            this.BoggleBoard.Enabled = false;
            this.BoggleBoard.Font = new System.Drawing.Font("Comic Sans MS", 19.69811F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BoggleBoard.Location = new System.Drawing.Point(12, 111);
            this.BoggleBoard.Name = "BoggleBoard";
            this.BoggleBoard.Size = new System.Drawing.Size(322, 322);
            this.BoggleBoard.TabIndex = 18;
            // 
            // WordsListLable
            // 
            this.WordsListLable.AutoSize = true;
            this.WordsListLable.Location = new System.Drawing.Point(387, 93);
            this.WordsListLable.Name = "WordsListLable";
            this.WordsListLable.Size = new System.Drawing.Size(42, 15);
            this.WordsListLable.TabIndex = 22;
            this.WordsListLable.Text = "Words";
            // 
            // Player2Label
            // 
            this.Player2Label.AutoSize = true;
            this.Player2Label.Location = new System.Drawing.Point(487, 60);
            this.Player2Label.Name = "Player2Label";
            this.Player2Label.Size = new System.Drawing.Size(51, 15);
            this.Player2Label.TabIndex = 23;
            this.Player2Label.Text = "Player2:";
            // 
            // Player2UsernameBox
            // 
            this.Player2UsernameBox.Enabled = false;
            this.Player2UsernameBox.Location = new System.Drawing.Point(542, 60);
            this.Player2UsernameBox.Name = "Player2UsernameBox";
            this.Player2UsernameBox.Size = new System.Drawing.Size(153, 20);
            this.Player2UsernameBox.TabIndex = 24;
            // 
            // Player2ScoreLable
            // 
            this.Player2ScoreLable.AutoSize = true;
            this.Player2ScoreLable.Location = new System.Drawing.Point(487, 86);
            this.Player2ScoreLable.Name = "Player2ScoreLable";
            this.Player2ScoreLable.Size = new System.Drawing.Size(42, 15);
            this.Player2ScoreLable.TabIndex = 25;
            this.Player2ScoreLable.Text = "Score:";
            // 
            // Player2ScoreBox
            // 
            this.Player2ScoreBox.Enabled = false;
            this.Player2ScoreBox.Location = new System.Drawing.Point(535, 86);
            this.Player2ScoreBox.Name = "Player2ScoreBox";
            this.Player2ScoreBox.Size = new System.Drawing.Size(162, 20);
            this.Player2ScoreBox.TabIndex = 26;
            // 
            // BoggleWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 477);
            this.Controls.Add(this.Player2ScoreBox);
            this.Controls.Add(this.Player2ScoreLable);
            this.Controls.Add(this.Player2UsernameBox);
            this.Controls.Add(this.Player2Label);
            this.Controls.Add(this.WordsListLable);
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
        private System.Windows.Forms.Label WordsListLable;
        private System.Windows.Forms.Label Player2Label;
        private System.Windows.Forms.TextBox Player2UsernameBox;
        private System.Windows.Forms.Label Player2ScoreLable;
        private System.Windows.Forms.TextBox Player2ScoreBox;
    }
}

