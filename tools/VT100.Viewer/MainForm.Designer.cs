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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tbInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.tbRawInput = new System.Windows.Forms.TextBox();
            this.spOutput = new System.Windows.Forms.SplitContainer();
            this.pbBuffer = new System.Windows.Forms.PictureBox();
            this.fctbOutput = new BeaverSoft.Texo.View.Terminal.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.spOutput)).BeginInit();
            this.spOutput.Panel1.SuspendLayout();
            this.spOutput.Panel2.SuspendLayout();
            this.spOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBuffer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fctbOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // tbInput
            // 
            this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbInput.Location = new System.Drawing.Point(12, 12);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(619, 23);
            this.tbInput.TabIndex = 0;
            this.tbInput.TabStop = false;
            this.tbInput.Text = "git status";
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(643, 12);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 1;
            this.btnSend.TabStop = false;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.HandleSendButtonClick);
            // 
            // tbRawInput
            // 
            this.tbRawInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRawInput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRawInput.Location = new System.Drawing.Point(12, 41);
            this.tbRawInput.Name = "tbRawInput";
            this.tbRawInput.ReadOnly = true;
            this.tbRawInput.Size = new System.Drawing.Size(706, 23);
            this.tbRawInput.TabIndex = 2;
            this.tbRawInput.TabStop = false;
            this.tbRawInput.Text = "input";
            this.tbRawInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbRawInput.Enter += new System.EventHandler(this.HandleRawInputTextBoxEnter);
            this.tbRawInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleRawInputTextBoxKeyDown);
            this.tbRawInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleRawInputTextBoxKeyUp);
            this.tbRawInput.Leave += new System.EventHandler(this.HandleRawInputTextBoxLeave);
            // 
            // spOutput
            // 
            this.spOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spOutput.Location = new System.Drawing.Point(12, 70);
            this.spOutput.Name = "spOutput";
            this.spOutput.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spOutput.Panel1
            // 
            this.spOutput.Panel1.Controls.Add(this.pbBuffer);
            // 
            // spOutput.Panel2
            // 
            this.spOutput.Panel2.Controls.Add(this.fctbOutput);
            this.spOutput.Size = new System.Drawing.Size(706, 512);
            this.spOutput.SplitterDistance = 171;
            this.spOutput.TabIndex = 3;
            // 
            // pbBuffer
            // 
            this.pbBuffer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbBuffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbBuffer.Location = new System.Drawing.Point(0, 0);
            this.pbBuffer.Name = "pbBuffer";
            this.pbBuffer.Size = new System.Drawing.Size(706, 171);
            this.pbBuffer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbBuffer.TabIndex = 4;
            this.pbBuffer.TabStop = false;
            // 
            // fctbOutput
            // 
            this.fctbOutput.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fctbOutput.AutoIndent = false;
            this.fctbOutput.AutoIndentChars = false;
            this.fctbOutput.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*" +
    "(?<range>:)\\s*(?<range>[^;]+);";
            this.fctbOutput.AutoIndentExistingLines = false;
            this.fctbOutput.AutoScrollMinSize = new System.Drawing.Size(179, 15);
            this.fctbOutput.BackBrush = null;
            this.fctbOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fctbOutput.CaretColor = System.Drawing.Color.Fuchsia;
            this.fctbOutput.CharHeight = 15;
            this.fctbOutput.CharWidth = 7;
            this.fctbOutput.CurrentLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.fctbOutput.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctbOutput.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctbOutput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fctbOutput.IsReplaceMode = false;
            this.fctbOutput.LineNumberColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.fctbOutput.Location = new System.Drawing.Point(0, 0);
            this.fctbOutput.Name = "fctbOutput";
            this.fctbOutput.Paddings = new System.Windows.Forms.Padding(0);
            this.fctbOutput.ReservedCountOfLineNumberChars = 4;
            this.fctbOutput.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctbOutput.ServiceColors = ((BeaverSoft.Texo.View.Terminal.ServiceColors)(resources.GetObject("fctbOutput.ServiceColors")));
            this.fctbOutput.ShowCaretWhenInactive = true;
            this.fctbOutput.Size = new System.Drawing.Size(706, 337);
            this.fctbOutput.TabIndex = 0;
            this.fctbOutput.Text = "fastColoredTextBox1";
            this.fctbOutput.Zoom = 100;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 594);
            this.Controls.Add(this.spOutput);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbRawInput);
            this.Name = "MainForm";
            this.Text = "VT100 Viewer";
            this.spOutput.Panel1.ResumeLayout(false);
            this.spOutput.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spOutput)).EndInit();
            this.spOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbBuffer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fctbOutput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer spOutput;
        private System.Windows.Forms.PictureBox pbBuffer;
        private BeaverSoft.Texo.View.Terminal.FastColoredTextBox fctbOutput;
    }
}

