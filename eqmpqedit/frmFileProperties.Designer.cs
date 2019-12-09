namespace eqmpqedit
{
    partial class frmFileProperties
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.fileFlagsLabel = new System.Windows.Forms.Label();
            this.sizeCompressed = new System.Windows.Forms.Label();
            this.sizeUncompressedLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.hashIndexLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "< error >";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.hashIndexLabel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.sizeUncompressedLabel);
            this.groupBox1.Controls.Add(this.sizeCompressed);
            this.groupBox1.Controls.Add(this.fileFlagsLabel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(15, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 134);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Information";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "File flags:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Size:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Size (uncompressed):";
            // 
            // fileFlagsLabel
            // 
            this.fileFlagsLabel.AutoSize = true;
            this.fileFlagsLabel.Location = new System.Drawing.Point(130, 26);
            this.fileFlagsLabel.Name = "fileFlagsLabel";
            this.fileFlagsLabel.Size = new System.Drawing.Size(46, 13);
            this.fileFlagsLabel.TabIndex = 4;
            this.fileFlagsLabel.Text = "< error >";
            // 
            // sizeCompressed
            // 
            this.sizeCompressed.AutoSize = true;
            this.sizeCompressed.Location = new System.Drawing.Point(130, 56);
            this.sizeCompressed.Name = "sizeCompressed";
            this.sizeCompressed.Size = new System.Drawing.Size(46, 13);
            this.sizeCompressed.TabIndex = 5;
            this.sizeCompressed.Text = "< error >";
            // 
            // sizeUncompressedLabel
            // 
            this.sizeUncompressedLabel.AutoSize = true;
            this.sizeUncompressedLabel.Location = new System.Drawing.Point(129, 84);
            this.sizeUncompressedLabel.Name = "sizeUncompressedLabel";
            this.sizeUncompressedLabel.Size = new System.Drawing.Size(46, 13);
            this.sizeUncompressedLabel.TabIndex = 6;
            this.sizeUncompressedLabel.Text = "< error >";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Hash index:";
            // 
            // hashIndexLabel
            // 
            this.hashIndexLabel.AutoSize = true;
            this.hashIndexLabel.Location = new System.Drawing.Point(129, 113);
            this.hashIndexLabel.Name = "hashIndexLabel";
            this.hashIndexLabel.Size = new System.Drawing.Size(46, 13);
            this.hashIndexLabel.TabIndex = 8;
            this.hashIndexLabel.Text = "< error >";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(340, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "File type:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(67, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "< error >";
            // 
            // frmFileProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 252);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFileProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Properties";
            this.Load += new System.EventHandler(this.frmFileProperties_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label hashIndexLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label sizeUncompressedLabel;
        private System.Windows.Forms.Label sizeCompressed;
        private System.Windows.Forms.Label fileFlagsLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}