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
            this.xorBtn = new System.Windows.Forms.Button();
            this.learnPctBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            // xorBtn
            // 
            this.xorBtn.Location = new System.Drawing.Point(620, 13);
            this.xorBtn.Name = "xorBtn";
            this.xorBtn.Size = new System.Drawing.Size(75, 23);
            this.xorBtn.TabIndex = 2;
            this.xorBtn.Text = "XOR";
            this.xorBtn.UseVisualStyleBackColor = true;
            this.xorBtn.Click += new System.EventHandler(this.xorBtn_Click);
            // 
            // learnPctBtn
            // 
            this.learnPctBtn.Location = new System.Drawing.Point(620, 42);
            this.learnPctBtn.Name = "learnPctBtn";
            this.learnPctBtn.Size = new System.Drawing.Size(97, 23);
            this.learnPctBtn.TabIndex = 3;
            this.learnPctBtn.Text = "Learn Puncte";
            this.learnPctBtn.UseVisualStyleBackColor = true;
            this.learnPctBtn.Click += new System.EventHandler(this.learnPctBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 415);
            this.Controls.Add(this.learnPctBtn);
            this.Controls.Add(this.xorBtn);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button xorBtn;
        private System.Windows.Forms.Button learnPctBtn;
    }
}

