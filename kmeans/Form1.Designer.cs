namespace generare_nr_aleatoare
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.kmeansBtn = new System.Windows.Forms.Button();
            this.euclidRadio = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.manhattanRadio = new System.Windows.Forms.RadioButton();
            this.cosRadio = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(601, 601);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // kmeansBtn
            // 
            this.kmeansBtn.Location = new System.Drawing.Point(629, 118);
            this.kmeansBtn.Name = "kmeansBtn";
            this.kmeansBtn.Size = new System.Drawing.Size(87, 23);
            this.kmeansBtn.TabIndex = 2;
            this.kmeansBtn.Text = "k-means";
            this.kmeansBtn.UseVisualStyleBackColor = true;
            this.kmeansBtn.Click += new System.EventHandler(this.kmeansBtn_Click);
            // 
            // euclidRadio
            // 
            this.euclidRadio.AutoSize = true;
            this.euclidRadio.Checked = true;
            this.euclidRadio.Location = new System.Drawing.Point(6, 19);
            this.euclidRadio.Name = "euclidRadio";
            this.euclidRadio.Size = new System.Drawing.Size(68, 17);
            this.euclidRadio.TabIndex = 3;
            this.euclidRadio.TabStop = true;
            this.euclidRadio.Text = "Euclidian";
            this.euclidRadio.UseVisualStyleBackColor = true;
            this.euclidRadio.CheckedChanged += new System.EventHandler(this.euclidRadio_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cosRadio);
            this.groupBox1.Controls.Add(this.manhattanRadio);
            this.groupBox1.Controls.Add(this.euclidRadio);
            this.groupBox1.Location = new System.Drawing.Point(629, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(87, 100);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Distance";
            // 
            // manhattanRadio
            // 
            this.manhattanRadio.AutoSize = true;
            this.manhattanRadio.Location = new System.Drawing.Point(6, 42);
            this.manhattanRadio.Name = "manhattanRadio";
            this.manhattanRadio.Size = new System.Drawing.Size(76, 17);
            this.manhattanRadio.TabIndex = 4;
            this.manhattanRadio.TabStop = true;
            this.manhattanRadio.Text = "Manhattan";
            this.manhattanRadio.UseVisualStyleBackColor = true;
            this.manhattanRadio.CheckedChanged += new System.EventHandler(this.manhattanRadio_CheckedChanged);
            // 
            // cosRadio
            // 
            this.cosRadio.AutoSize = true;
            this.cosRadio.Location = new System.Drawing.Point(5, 65);
            this.cosRadio.Name = "cosRadio";
            this.cosRadio.Size = new System.Drawing.Size(43, 17);
            this.cosRadio.TabIndex = 5;
            this.cosRadio.TabStop = true;
            this.cosRadio.Text = "Cos";
            this.cosRadio.UseVisualStyleBackColor = true;
            this.cosRadio.CheckedChanged += new System.EventHandler(this.cosRadio_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 415);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.kmeansBtn);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button kmeansBtn;
        private System.Windows.Forms.RadioButton euclidRadio;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton manhattanRadio;
        private System.Windows.Forms.RadioButton cosRadio;
    }
}

