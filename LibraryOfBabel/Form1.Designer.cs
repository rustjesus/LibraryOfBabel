namespace LibraryOfBabel
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
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbWall = new System.Windows.Forms.TrackBar();
            this.tbShelf = new System.Windows.Forms.TrackBar();
            this.tbVolume = new System.Windows.Forms.TrackBar();
            this.rtbHex = new System.Windows.Forms.RichTextBox();
            this.findVolButton2 = new System.Windows.Forms.Button();
            this.lblShelf = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblWall = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pageRtb = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbWall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShelf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbOutput
            // 
            this.rtbOutput.Location = new System.Drawing.Point(12, 274);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.Size = new System.Drawing.Size(768, 437);
            this.rtbOutput.TabIndex = 1;
            this.rtbOutput.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(690, 216);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Search Phrase";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(15, 194);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(669, 45);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Input:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 258);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Output:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "v0.0.3";
            // 
            // tbWall
            // 
            this.tbWall.Location = new System.Drawing.Point(46, 102);
            this.tbWall.Maximum = 4;
            this.tbWall.Minimum = 1;
            this.tbWall.Name = "tbWall";
            this.tbWall.Size = new System.Drawing.Size(104, 45);
            this.tbWall.TabIndex = 9;
            this.tbWall.Value = 1;
            this.tbWall.Scroll += new System.EventHandler(this.wall_trackBar1_Scroll);
            // 
            // tbShelf
            // 
            this.tbShelf.Location = new System.Drawing.Point(256, 102);
            this.tbShelf.Maximum = 5;
            this.tbShelf.Minimum = 1;
            this.tbShelf.Name = "tbShelf";
            this.tbShelf.Size = new System.Drawing.Size(104, 45);
            this.tbShelf.TabIndex = 10;
            this.tbShelf.Value = 1;
            this.tbShelf.Scroll += new System.EventHandler(this.shelf_trackBar2_Scroll);
            // 
            // tbVolume
            // 
            this.tbVolume.Location = new System.Drawing.Point(449, 102);
            this.tbVolume.Maximum = 32;
            this.tbVolume.Minimum = 1;
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(104, 45);
            this.tbVolume.TabIndex = 11;
            this.tbVolume.Value = 1;
            this.tbVolume.Scroll += new System.EventHandler(this.vol_trackBar3_Scroll);
            // 
            // rtbHex
            // 
            this.rtbHex.Location = new System.Drawing.Point(46, 31);
            this.rtbHex.Name = "rtbHex";
            this.rtbHex.Size = new System.Drawing.Size(348, 24);
            this.rtbHex.TabIndex = 12;
            this.rtbHex.Text = "";
            this.rtbHex.TextChanged += new System.EventHandler(this.hex_richTextBox1_TextChanged);
            // 
            // findVolButton2
            // 
            this.findVolButton2.Location = new System.Drawing.Point(690, 134);
            this.findVolButton2.Name = "findVolButton2";
            this.findVolButton2.Size = new System.Drawing.Size(75, 23);
            this.findVolButton2.TabIndex = 13;
            this.findVolButton2.Text = "Find";
            this.findVolButton2.UseVisualStyleBackColor = true;
            this.findVolButton2.Click += new System.EventHandler(this.findVolButton2_Click);
            // 
            // lblShelf
            // 
            this.lblShelf.AutoSize = true;
            this.lblShelf.Location = new System.Drawing.Point(201, 102);
            this.lblShelf.Name = "lblShelf";
            this.lblShelf.Size = new System.Drawing.Size(32, 13);
            this.lblShelf.TabIndex = 14;
            this.lblShelf.Text = "shelf:";
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(385, 102);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(44, 13);
            this.lblVolume.TabIndex = 15;
            this.lblVolume.Text = "volume:";
            // 
            // lblWall
            // 
            this.lblWall.AutoSize = true;
            this.lblWall.Location = new System.Drawing.Point(12, 102);
            this.lblWall.Name = "lblWall";
            this.lblWall.Size = new System.Drawing.Size(28, 13);
            this.lblWall.TabIndex = 16;
            this.lblWall.Text = "wall:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Hex:";
            // 
            // pageRtb
            // 
            this.pageRtb.Location = new System.Drawing.Point(680, 99);
            this.pageRtb.Name = "pageRtb";
            this.pageRtb.Size = new System.Drawing.Size(100, 25);
            this.pageRtb.TabIndex = 18;
            this.pageRtb.Text = "";
            this.pageRtb.TextChanged += new System.EventHandler(this.pageRtb_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(639, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Page:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 723);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pageRtb);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblWall);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.lblShelf);
            this.Controls.Add(this.findVolButton2);
            this.Controls.Add(this.rtbHex);
            this.Controls.Add(this.tbVolume);
            this.Controls.Add(this.tbShelf);
            this.Controls.Add(this.tbWall);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rtbOutput);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.tbWall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShelf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar tbWall;
        private System.Windows.Forms.TrackBar tbShelf;
        private System.Windows.Forms.TrackBar tbVolume;
        private System.Windows.Forms.RichTextBox rtbHex;
        private System.Windows.Forms.Button findVolButton2;
        private System.Windows.Forms.Label lblShelf;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.Label lblWall;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox pageRtb;
        private System.Windows.Forms.Label label5;
    }
}

