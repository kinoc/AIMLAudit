namespace AIMLAudit
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dictationTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.threadedSurvey = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.localBot = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.userLogFilePath = new System.Windows.Forms.TextBox();
            this.OpenUserLog = new System.Windows.Forms.Button();
            this.OpenOutputDir = new System.Windows.Forms.Button();
            this.outputDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.threadedSurvey);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.localBot);
            this.panel1.Location = new System.Drawing.Point(12, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(481, 33);
            this.panel1.TabIndex = 6;
            // 
            // threadedSurvey
            // 
            this.threadedSurvey.Location = new System.Drawing.Point(8, 3);
            this.threadedSurvey.Name = "threadedSurvey";
            this.threadedSurvey.Size = new System.Drawing.Size(104, 26);
            this.threadedSurvey.TabIndex = 6;
            this.threadedSurvey.Text = "ThreadedSurvey";
            this.threadedSurvey.UseVisualStyleBackColor = true;
            this.threadedSurvey.Click += new System.EventHandler(this.threadedSurvey_Click);
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(118, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(68, 26);
            this.button4.TabIndex = 5;
            this.button4.Text = "Survey";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // localBot
            // 
            this.localBot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localBot.AutoSize = true;
            this.localBot.Checked = true;
            this.localBot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.localBot.Location = new System.Drawing.Point(410, 9);
            this.localBot.Name = "localBot";
            this.localBot.Size = new System.Drawing.Size(68, 17);
            this.localBot.TabIndex = 3;
            this.localBot.Text = "LocalBot";
            this.localBot.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "UserLog:";
            // 
            // userLogFilePath
            // 
            this.userLogFilePath.Location = new System.Drawing.Point(85, 51);
            this.userLogFilePath.Name = "userLogFilePath";
            this.userLogFilePath.Size = new System.Drawing.Size(274, 20);
            this.userLogFilePath.TabIndex = 8;
            // 
            // OpenUserLog
            // 
            this.OpenUserLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenUserLog.Location = new System.Drawing.Point(382, 47);
            this.OpenUserLog.Name = "OpenUserLog";
            this.OpenUserLog.Size = new System.Drawing.Size(117, 23);
            this.OpenUserLog.TabIndex = 9;
            this.OpenUserLog.Text = "Browse";
            this.OpenUserLog.UseVisualStyleBackColor = true;
            this.OpenUserLog.Click += new System.EventHandler(this.OpenUserLog_Click);
            // 
            // OpenOutputDir
            // 
            this.OpenOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenOutputDir.Location = new System.Drawing.Point(382, 87);
            this.OpenOutputDir.Name = "OpenOutputDir";
            this.OpenOutputDir.Size = new System.Drawing.Size(117, 23);
            this.OpenOutputDir.TabIndex = 12;
            this.OpenOutputDir.Text = "Browse";
            this.OpenOutputDir.UseVisualStyleBackColor = true;
            this.OpenOutputDir.Click += new System.EventHandler(this.OpenOutputDir_Click);
            // 
            // outputDir
            // 
            this.outputDir.Location = new System.Drawing.Point(85, 91);
            this.outputDir.Name = "outputDir";
            this.outputDir.Size = new System.Drawing.Size(274, 20);
            this.outputDir.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "OutputDir:";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(20, 126);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(478, 108);
            this.listBox1.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 257);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.OpenOutputDir);
            this.Controls.Add(this.outputDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OpenUserLog);
            this.Controls.Add(this.userLogFilePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "AIMLAudit";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer dictationTimer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox localBot;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button threadedSurvey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox userLogFilePath;
        private System.Windows.Forms.Button OpenUserLog;
        private System.Windows.Forms.Button OpenOutputDir;
        private System.Windows.Forms.TextBox outputDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBox1;
    }
}

