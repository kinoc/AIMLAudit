namespace VoiceInterface
{
    partial class Form1x
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
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dictationTimer = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.hearText = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.localBot = new System.Windows.Forms.CheckBox();
            this.checkBoxBing = new System.Windows.Forms.CheckBox();
            this.checkBoxGoogle = new System.Windows.Forms.CheckBox();
            this.checkBoxLocal = new System.Windows.Forms.CheckBox();
            this.button5 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "StartDictation";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(15, 127);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(328, 122);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 90);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(320, 20);
            this.textBox1.TabIndex = 2;
            // 
            // dictationTimer
            // 
            this.dictationTimer.Tick += new System.EventHandler(this.dictationTimer_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(118, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 29);
            this.button2.TabIndex = 3;
            this.button2.Text = "Stop Dictation";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(236, 15);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(98, 26);
            this.button3.TabIndex = 4;
            this.button3.Text = "Hear Text";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // hearText
            // 
            this.hearText.Location = new System.Drawing.Point(12, 47);
            this.hearText.Name = "hearText";
            this.hearText.Size = new System.Drawing.Size(320, 20);
            this.hearText.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.localBot);
            this.panel1.Controls.Add(this.checkBoxBing);
            this.panel1.Controls.Add(this.checkBoxGoogle);
            this.panel1.Controls.Add(this.checkBoxLocal);
            this.panel1.Location = new System.Drawing.Point(346, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(98, 230);
            this.panel1.TabIndex = 6;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(11, 152);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(68, 26);
            this.button4.TabIndex = 5;
            this.button4.Text = "Survey";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // localBot
            // 
            this.localBot.AutoSize = true;
            this.localBot.Checked = true;
            this.localBot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.localBot.Location = new System.Drawing.Point(11, 107);
            this.localBot.Name = "localBot";
            this.localBot.Size = new System.Drawing.Size(68, 17);
            this.localBot.TabIndex = 3;
            this.localBot.Text = "LocalBot";
            this.localBot.UseVisualStyleBackColor = true;
            this.localBot.CheckedChanged += new System.EventHandler(this.localBot_CheckedChanged);
            // 
            // checkBoxBing
            // 
            this.checkBoxBing.AutoSize = true;
            this.checkBoxBing.Location = new System.Drawing.Point(11, 62);
            this.checkBoxBing.Name = "checkBoxBing";
            this.checkBoxBing.Size = new System.Drawing.Size(47, 17);
            this.checkBoxBing.TabIndex = 2;
            this.checkBoxBing.Text = "Bing";
            this.checkBoxBing.UseVisualStyleBackColor = true;
            // 
            // checkBoxGoogle
            // 
            this.checkBoxGoogle.AutoSize = true;
            this.checkBoxGoogle.Location = new System.Drawing.Point(11, 39);
            this.checkBoxGoogle.Name = "checkBoxGoogle";
            this.checkBoxGoogle.Size = new System.Drawing.Size(60, 17);
            this.checkBoxGoogle.TabIndex = 1;
            this.checkBoxGoogle.Text = "Google";
            this.checkBoxGoogle.UseVisualStyleBackColor = true;
            // 
            // checkBoxLocal
            // 
            this.checkBoxLocal.AutoSize = true;
            this.checkBoxLocal.Checked = true;
            this.checkBoxLocal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLocal.Location = new System.Drawing.Point(11, 16);
            this.checkBoxLocal.Name = "checkBoxLocal";
            this.checkBoxLocal.Size = new System.Drawing.Size(52, 17);
            this.checkBoxLocal.TabIndex = 0;
            this.checkBoxLocal.Text = "Local";
            this.checkBoxLocal.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(11, 184);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(68, 26);
            this.button5.TabIndex = 6;
            this.button5.Text = "ThrSurvey";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 273);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.hearText);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "UnitySpeechRec";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer dictationTimer;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox hearText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxBing;
        private System.Windows.Forms.CheckBox checkBoxGoogle;
        private System.Windows.Forms.CheckBox checkBoxLocal;
        private System.Windows.Forms.CheckBox localBot;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}

