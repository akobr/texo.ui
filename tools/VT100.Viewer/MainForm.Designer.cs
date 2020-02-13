namespace VT100.Viewer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.TextBox tbRawInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.PictureBox pbBuffer;
        //private System.Windows.Forms.RichTextBox tbOutput;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.tbRawInput = new System.Windows.Forms.TextBox();
            this.pbBuffer = new System.Windows.Forms.PictureBox();
            //this.tbOutput = new System.Windows.Forms.RichTextBox();

            //
            // tbInput
            //        
            this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbInput.Location = new System.Drawing.Point(12, 12);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(489, 23);
            this.tbInput.TabIndex = 0;
            this.tbInput.TabStop = false;
            this.tbInput.Text = "git status";
            //
            // btnSend
            //
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(513, 12);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 1;
            this.btnSend.TabStop = false;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += HandleSendButtonClick;
            // 
            // tbRawInput
            //
            this.tbRawInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRawInput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRawInput.Location = new System.Drawing.Point(12, 41);
            this.tbRawInput.Name = "tbRawInput";
            this.tbRawInput.ReadOnly = true;
            this.tbRawInput.Size = new System.Drawing.Size(576, 23);
            this.tbRawInput.TabIndex = 2;
            this.tbRawInput.TabStop = false;
            this.tbRawInput.Text = "input";
            this.tbRawInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbRawInput.Enter += new System.EventHandler(this.HandleRawInputTextBoxEnter);
            this.tbRawInput.Leave += new System.EventHandler(this.HandleRawInputTextBoxLeave);
            this.tbRawInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleRawInputTextBoxKeyDown);
            this.tbRawInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleRawInputTextBoxKeyUp);
            // 
            // pbBuffer
            // 
            this.pbBuffer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbBuffer.Location = new System.Drawing.Point(12, 73);
            this.pbBuffer.Name = "pbBuffer";
            this.pbBuffer.Size = new System.Drawing.Size(576, 359);
            this.pbBuffer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbBuffer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbBuffer.TabIndex = 3;
            this.pbBuffer.TabStop = false;
            //
            // tbOutput
            //            
            //this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //| System.Windows.Forms.AnchorStyles.Left)
            //| System.Windows.Forms.AnchorStyles.Right)));
            //this.tbOutput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.tbOutput.Location = new System.Drawing.Point(12, 41);
            //this.tbOutput.Multiline = true;
            //this.tbOutput.Name = "tbOutput";
            //this.tbOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            //this.tbOutput.Size = new System.Drawing.Size(576, 359);
            //this.tbOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            //this.tbOutput.BackColor = Color.Black;
            //this.tbOutput.ForeColor = Color.FromArgb(203, 204, 205);
            //this.tbOutput.TabIndex = 2;
            //
            // MainForm
            //
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 450);
            this.Text = "VT100 Viewer";
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbRawInput);
            this.Controls.Add(this.pbBuffer);
            //this.Controls.Add(this.tbOutput);
        }

        #endregion
    }
}

