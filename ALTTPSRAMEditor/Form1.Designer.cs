﻿namespace ALTTPSRAMEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opensrmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCTRLSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupFileSelect = new System.Windows.Forms.GroupBox();
            this.buttonErase = new System.Windows.Forms.Button();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.radioFile3 = new System.Windows.Forms.RadioButton();
            this.radioFile2 = new System.Windows.Forms.RadioButton();
            this.radioFile1 = new System.Windows.Forms.RadioButton();
            this.helperText = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.groupFileSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opensrmToolStripMenuItem,
            this.saveCTRLSToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // opensrmToolStripMenuItem
            // 
            this.opensrmToolStripMenuItem.Name = "opensrmToolStripMenuItem";
            this.opensrmToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.opensrmToolStripMenuItem.Text = "Open (Ctrl+O)";
            this.opensrmToolStripMenuItem.Click += new System.EventHandler(this.opensrmToolStripMenuItem_Click);
            // 
            // saveCTRLSToolStripMenuItem
            // 
            this.saveCTRLSToolStripMenuItem.Name = "saveCTRLSToolStripMenuItem";
            this.saveCTRLSToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveCTRLSToolStripMenuItem.Text = "Save (Ctrl+S)";
            this.saveCTRLSToolStripMenuItem.Click += new System.EventHandler(this.saveCTRLSToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click_1);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // groupFileSelect
            // 
            this.groupFileSelect.Controls.Add(this.buttonErase);
            this.groupFileSelect.Controls.Add(this.buttonWrite);
            this.groupFileSelect.Controls.Add(this.buttonCopy);
            this.groupFileSelect.Controls.Add(this.radioFile3);
            this.groupFileSelect.Controls.Add(this.radioFile2);
            this.groupFileSelect.Controls.Add(this.radioFile1);
            this.groupFileSelect.Location = new System.Drawing.Point(12, 27);
            this.groupFileSelect.Name = "groupFileSelect";
            this.groupFileSelect.Size = new System.Drawing.Size(200, 107);
            this.groupFileSelect.TabIndex = 4;
            this.groupFileSelect.TabStop = false;
            this.groupFileSelect.Text = "Current File";
            // 
            // buttonErase
            // 
            this.buttonErase.Enabled = false;
            this.buttonErase.Location = new System.Drawing.Point(95, 65);
            this.buttonErase.Name = "buttonErase";
            this.buttonErase.Size = new System.Drawing.Size(75, 23);
            this.buttonErase.TabIndex = 9;
            this.buttonErase.Text = "Erase";
            this.buttonErase.UseVisualStyleBackColor = true;
            this.buttonErase.Click += new System.EventHandler(this.buttonErase_Click);
            // 
            // buttonWrite
            // 
            this.buttonWrite.Enabled = false;
            this.buttonWrite.Location = new System.Drawing.Point(95, 42);
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.Size = new System.Drawing.Size(75, 23);
            this.buttonWrite.TabIndex = 8;
            this.buttonWrite.Text = "Write";
            this.buttonWrite.UseVisualStyleBackColor = true;
            this.buttonWrite.Click += new System.EventHandler(this.buttonWrite_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Enabled = false;
            this.buttonCopy.Location = new System.Drawing.Point(95, 19);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(75, 23);
            this.buttonCopy.TabIndex = 7;
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // radioFile3
            // 
            this.radioFile3.AutoSize = true;
            this.radioFile3.Enabled = false;
            this.radioFile3.Location = new System.Drawing.Point(6, 65);
            this.radioFile3.Name = "radioFile3";
            this.radioFile3.Size = new System.Drawing.Size(50, 17);
            this.radioFile3.TabIndex = 6;
            this.radioFile3.TabStop = true;
            this.radioFile3.Text = "File 3";
            this.radioFile3.UseVisualStyleBackColor = true;
            // 
            // radioFile2
            // 
            this.radioFile2.AutoSize = true;
            this.radioFile2.Enabled = false;
            this.radioFile2.Location = new System.Drawing.Point(6, 42);
            this.radioFile2.Name = "radioFile2";
            this.radioFile2.Size = new System.Drawing.Size(50, 17);
            this.radioFile2.TabIndex = 5;
            this.radioFile2.TabStop = true;
            this.radioFile2.Text = "File 2";
            this.radioFile2.UseVisualStyleBackColor = true;
            // 
            // radioFile1
            // 
            this.radioFile1.AutoSize = true;
            this.radioFile1.Checked = true;
            this.radioFile1.Enabled = false;
            this.radioFile1.Location = new System.Drawing.Point(6, 19);
            this.radioFile1.Name = "radioFile1";
            this.radioFile1.Size = new System.Drawing.Size(50, 17);
            this.radioFile1.TabIndex = 4;
            this.radioFile1.TabStop = true;
            this.radioFile1.Text = "File 1";
            this.radioFile1.UseVisualStyleBackColor = true;
            this.radioFile1.CheckedChanged += new System.EventHandler(this.radioFile1_CheckedChanged);
            // 
            // helperText
            // 
            this.helperText.AutoSize = true;
            this.helperText.Location = new System.Drawing.Point(9, 428);
            this.helperText.Name = "helperText";
            this.helperText.Size = new System.Drawing.Size(254, 13);
            this.helperText.TabIndex = 5;
            this.helperText.Text = "A Link to the Past SRAM Editor v1.0 by mysterypaint";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.helperText);
            this.Controls.Add(this.groupFileSelect);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "A Link to the Past SRAM Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupFileSelect.ResumeLayout(false);
            this.groupFileSelect.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opensrmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCTRLSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupFileSelect;
        private System.Windows.Forms.RadioButton radioFile3;
        private System.Windows.Forms.RadioButton radioFile2;
        private System.Windows.Forms.RadioButton radioFile1;
        private System.Windows.Forms.Button buttonErase;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Label helperText;
    }
}

