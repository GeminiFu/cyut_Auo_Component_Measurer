namespace cyut_Auo_Component_Measurer
{
    partial class Form_Dot_Grid
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
            this.btn_Apply = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.label_Y = new System.Windows.Forms.Label();
            this.label_X = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Apply
            // 
            this.btn_Apply.Location = new System.Drawing.Point(288, 159);
            this.btn_Apply.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btn_Apply.Name = "btn_Apply";
            this.btn_Apply.Size = new System.Drawing.Size(168, 41);
            this.btn_Apply.TabIndex = 44;
            this.btn_Apply.Text = "確定";
            this.btn_Apply.UseVisualStyleBackColor = true;
            this.btn_Apply.Click += new System.EventHandler(this.btn_Apply_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(50, 159);
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(168, 41);
            this.btn_Cancel.TabIndex = 43;
            this.btn_Cancel.Text = "取消";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // numY
            // 
            this.numY.Location = new System.Drawing.Point(164, 79);
            this.numY.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.numY.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(165, 27);
            this.numY.TabIndex = 42;
            this.numY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // numX
            // 
            this.numX.Location = new System.Drawing.Point(164, 33);
            this.numX.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.numX.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(165, 27);
            this.numX.TabIndex = 41;
            this.numX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_Y
            // 
            this.label_Y.AutoSize = true;
            this.label_Y.Location = new System.Drawing.Point(112, 81);
            this.label_Y.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label_Y.Name = "label_Y";
            this.label_Y.Size = new System.Drawing.Size(23, 16);
            this.label_Y.TabIndex = 40;
            this.label_Y.Text = "y :";
            // 
            // label_X
            // 
            this.label_X.AutoSize = true;
            this.label_X.Location = new System.Drawing.Point(112, 37);
            this.label_X.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label_X.Name = "label_X";
            this.label_X.Size = new System.Drawing.Size(23, 16);
            this.label_X.TabIndex = 39;
            this.label_X.Text = "x :";
            // 
            // Form_Dot_Grid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 216);
            this.Controls.Add(this.btn_Apply);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.numY);
            this.Controls.Add(this.numX);
            this.Controls.Add(this.label_Y);
            this.Controls.Add(this.label_X);
            this.Font = new System.Drawing.Font("新細明體", 12F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form_Dot_Grid";
            this.Text = "Form_Dot_Grid";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Closed);
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Apply;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.NumericUpDown numY;
        private System.Windows.Forms.NumericUpDown numX;
        private System.Windows.Forms.Label label_Y;
        private System.Windows.Forms.Label label_X;
    }
}