namespace Snipit
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainButton = new System.Windows.Forms.Button();
            mainLabel = new System.Windows.Forms.Label();
            textExtraction = new System.Windows.Forms.CheckBox();
            chatGptExtraction = new System.Windows.Forms.CheckBox();
            snipitScreenShot = new System.Windows.Forms.PictureBox();
            chatGptQuestion = new System.Windows.Forms.RichTextBox();
            chatGptSubmit = new System.Windows.Forms.Button();
            responseLabel = new System.Windows.Forms.Label();
            questionPanel = new System.Windows.Forms.Panel();
            enterQuestionLabel = new System.Windows.Forms.Label();
            snipitButton = new System.Windows.Forms.Button();
            liveExtraction = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)snipitScreenShot).BeginInit();
            questionPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainButton
            // 
            mainButton.Location = new System.Drawing.Point(169, 55);
            mainButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            mainButton.Name = "mainButton";
            mainButton.Size = new System.Drawing.Size(85, 30);
            mainButton.TabIndex = 0;
            mainButton.Text = "Start";
            mainButton.UseVisualStyleBackColor = true;
            mainButton.Click += MainButton_Click;
            // 
            // mainLabel
            // 
            mainLabel.AutoSize = true;
            mainLabel.Location = new System.Drawing.Point(161, 105);
            mainLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            mainLabel.Name = "mainLabel";
            mainLabel.Size = new System.Drawing.Size(104, 15);
            mainLabel.TabIndex = 1;
            mainLabel.Text = "Process Running...";
            // 
            // textExtraction
            // 
            textExtraction.AutoSize = true;
            textExtraction.Location = new System.Drawing.Point(21, 18);
            textExtraction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textExtraction.Name = "textExtraction";
            textExtraction.Size = new System.Drawing.Size(103, 19);
            textExtraction.TabIndex = 2;
            textExtraction.Text = "Text Extraction";
            textExtraction.UseMnemonic = false;
            textExtraction.UseVisualStyleBackColor = true;
            textExtraction.CheckedChanged += useTextExtractionChanged;
            // 
            // chatGptExtraction
            // 
            chatGptExtraction.AutoSize = true;
            chatGptExtraction.Location = new System.Drawing.Point(21, 45);
            chatGptExtraction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chatGptExtraction.Name = "chatGptExtraction";
            chatGptExtraction.Size = new System.Drawing.Size(126, 19);
            chatGptExtraction.TabIndex = 3;
            chatGptExtraction.Text = "ChatGpt Extraction";
            chatGptExtraction.UseVisualStyleBackColor = true;
            chatGptExtraction.CheckedChanged += useChatGptExtractionChanged;
            // 
            // snipitScreenShot
            // 
            snipitScreenShot.BackColor = System.Drawing.SystemColors.AppWorkspace;
            snipitScreenShot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            snipitScreenShot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            snipitScreenShot.Location = new System.Drawing.Point(52, 123);
            snipitScreenShot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            snipitScreenShot.Name = "snipitScreenShot";
            snipitScreenShot.Padding = new System.Windows.Forms.Padding(6);
            snipitScreenShot.Size = new System.Drawing.Size(340, 340);
            snipitScreenShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            snipitScreenShot.TabIndex = 4;
            snipitScreenShot.TabStop = false;
            snipitScreenShot.BackgroundImageChanged += snipitScreenShotBackgroundChanged;
            // 
            // chatGptQuestion
            // 
            chatGptQuestion.Location = new System.Drawing.Point(13, 33);
            chatGptQuestion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chatGptQuestion.Name = "chatGptQuestion";
            chatGptQuestion.Size = new System.Drawing.Size(270, 61);
            chatGptQuestion.TabIndex = 5;
            chatGptQuestion.Text = "";
            // 
            // chatGptSubmit
            // 
            chatGptSubmit.Location = new System.Drawing.Point(96, 100);
            chatGptSubmit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chatGptSubmit.Name = "chatGptSubmit";
            chatGptSubmit.Size = new System.Drawing.Size(85, 30);
            chatGptSubmit.TabIndex = 6;
            chatGptSubmit.Text = "Submit";
            chatGptSubmit.UseVisualStyleBackColor = true;
            chatGptSubmit.Click += ChatGptSubmit_Click;
            // 
            // responseLabel
            // 
            responseLabel.AutoEllipsis = true;
            responseLabel.Location = new System.Drawing.Point(52, 495);
            responseLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            responseLabel.MaximumSize = new System.Drawing.Size(340, 0);
            responseLabel.Name = "responseLabel";
            responseLabel.Padding = new System.Windows.Forms.Padding(2);
            responseLabel.Size = new System.Drawing.Size(340, 100);
            responseLabel.TabIndex = 7;
            responseLabel.Text = "Loading response...";
            // 
            // questionPanel
            // 
            questionPanel.Controls.Add(enterQuestionLabel);
            questionPanel.Controls.Add(chatGptSubmit);
            questionPanel.Controls.Add(chatGptQuestion);
            questionPanel.Location = new System.Drawing.Point(73, 304);
            questionPanel.Name = "questionPanel";
            questionPanel.Size = new System.Drawing.Size(298, 142);
            questionPanel.TabIndex = 8;
            // 
            // enterQuestionLabel
            // 
            enterQuestionLabel.AutoSize = true;
            enterQuestionLabel.Location = new System.Drawing.Point(87, 10);
            enterQuestionLabel.Name = "enterQuestionLabel";
            enterQuestionLabel.Size = new System.Drawing.Size(118, 15);
            enterQuestionLabel.TabIndex = 7;
            enterQuestionLabel.Text = "Enter question below";
            // 
            // snipitButton
            // 
            snipitButton.Location = new System.Drawing.Point(334, 11);
            snipitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            snipitButton.Name = "snipitButton";
            snipitButton.Size = new System.Drawing.Size(85, 30);
            snipitButton.TabIndex = 9;
            snipitButton.Text = "Snip (ctrl+/)";
            snipitButton.UseVisualStyleBackColor = true;
            snipitButton.Click += Snipit_Click;
            // 
            // liveExtraction
            // 
            liveExtraction.AutoSize = true;
            liveExtraction.Location = new System.Drawing.Point(21, 70);
            liveExtraction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            liveExtraction.Name = "liveExtraction";
            liveExtraction.Size = new System.Drawing.Size(47, 19);
            liveExtraction.TabIndex = 10;
            liveExtraction.Text = "Live";
            liveExtraction.UseMnemonic = false;
            liveExtraction.UseVisualStyleBackColor = true;
            liveExtraction.CheckedChanged += useLiveExtractionChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            ClientSize = new System.Drawing.Size(440, 640);
            Controls.Add(liveExtraction);
            Controls.Add(snipitButton);
            Controls.Add(questionPanel);
            Controls.Add(responseLabel);
            Controls.Add(snipitScreenShot);
            Controls.Add(chatGptExtraction);
            Controls.Add(textExtraction);
            Controls.Add(mainLabel);
            Controls.Add(mainButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "MainForm";
            Padding = new System.Windows.Forms.Padding(5);
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "Snipit";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)snipitScreenShot).EndInit();
            questionPanel.ResumeLayout(false);
            questionPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button mainButton;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.CheckBox chatGptExtraction;
        private System.Windows.Forms.CheckBox textExtraction;
        private System.Windows.Forms.PictureBox snipitScreenShot;
        private System.Windows.Forms.RichTextBox chatGptQuestion;
        private System.Windows.Forms.Button chatGptSubmit;
        private System.Windows.Forms.Label responseLabel;
        private System.Windows.Forms.Panel questionPanel;
        private System.Windows.Forms.Label enterQuestionLabel;
        private System.Windows.Forms.Button snipitButton;
        private System.Windows.Forms.CheckBox liveExtraction;
    }
}

