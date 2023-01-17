namespace cyut_Auo_Component_Measurer
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Load = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_Camera = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.檔案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Load_Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Save_Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dotGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Measure_Standard = new System.Windows.Forms.Button();
            this.btn_Measure_Product = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Load
            // 
            this.btn_Load.Location = new System.Drawing.Point(42, 54);
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.Size = new System.Drawing.Size(75, 23);
            this.btn_Load.TabIndex = 0;
            this.btn_Load.Text = "載入圖片";
            this.btn_Load.UseVisualStyleBackColor = true;
            this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(72, 135);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(463, 247);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Mose_Down);
            // 
            // btn_Camera
            // 
            this.btn_Camera.Location = new System.Drawing.Point(141, 54);
            this.btn_Camera.Name = "btn_Camera";
            this.btn_Camera.Size = new System.Drawing.Size(75, 23);
            this.btn_Camera.TabIndex = 2;
            this.btn_Camera.Text = "使用相機";
            this.btn_Camera.UseVisualStyleBackColor = true;
            this.btn_Camera.Click += new System.EventHandler(this.btn_Camera_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.檔案ToolStripMenuItem,
            this.設定ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 檔案ToolStripMenuItem
            // 
            this.檔案ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Load_Setting,
            this.Menu_Save_Setting});
            this.檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            this.檔案ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.檔案ToolStripMenuItem.Text = "檔案";
            // 
            // Menu_Load_Setting
            // 
            this.Menu_Load_Setting.Name = "Menu_Load_Setting";
            this.Menu_Load_Setting.Size = new System.Drawing.Size(134, 22);
            this.Menu_Load_Setting.Text = "讀取設定檔";
            this.Menu_Load_Setting.Click += new System.EventHandler(this.Menu_Load_Setting_Click);
            // 
            // Menu_Save_Setting
            // 
            this.Menu_Save_Setting.Name = "Menu_Save_Setting";
            this.Menu_Save_Setting.Size = new System.Drawing.Size(134, 22);
            this.Menu_Save_Setting.Text = "儲存設定檔";
            this.Menu_Save_Setting.Click += new System.EventHandler(this.Menu_Save_Setting_Click);
            // 
            // 設定ToolStripMenuItem
            // 
            this.設定ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dotGridToolStripMenuItem});
            this.設定ToolStripMenuItem.Name = "設定ToolStripMenuItem";
            this.設定ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.設定ToolStripMenuItem.Text = "設定";
            // 
            // dotGridToolStripMenuItem
            // 
            this.dotGridToolStripMenuItem.Name = "dotGridToolStripMenuItem";
            this.dotGridToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.dotGridToolStripMenuItem.Text = "Dot Grid";
            this.dotGridToolStripMenuItem.Click += new System.EventHandler(this.dotGridToolStripMenuItem_Click);
            // 
            // btn_Measure_Standard
            // 
            this.btn_Measure_Standard.Location = new System.Drawing.Point(256, 54);
            this.btn_Measure_Standard.Name = "btn_Measure_Standard";
            this.btn_Measure_Standard.Size = new System.Drawing.Size(75, 23);
            this.btn_Measure_Standard.TabIndex = 6;
            this.btn_Measure_Standard.Text = "測量標準物";
            this.btn_Measure_Standard.UseVisualStyleBackColor = true;
            this.btn_Measure_Standard.Click += new System.EventHandler(this.btn_Measure_Standard_Click);
            // 
            // btn_Measure_Product
            // 
            this.btn_Measure_Product.Location = new System.Drawing.Point(369, 67);
            this.btn_Measure_Product.Name = "btn_Measure_Product";
            this.btn_Measure_Product.Size = new System.Drawing.Size(75, 23);
            this.btn_Measure_Product.TabIndex = 7;
            this.btn_Measure_Product.Text = "測量待測物";
            this.btn_Measure_Product.UseVisualStyleBackColor = true;
            this.btn_Measure_Product.Click += new System.EventHandler(this.btn_Measure_Product_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_Measure_Product);
            this.Controls.Add(this.btn_Measure_Standard);
            this.Controls.Add(this.btn_Camera);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_Load);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Close);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Load;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_Camera;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 檔案ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dotGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Menu_Load_Setting;
        private System.Windows.Forms.ToolStripMenuItem Menu_Save_Setting;
        private System.Windows.Forms.Button btn_Measure_Standard;
        private System.Windows.Forms.Button btn_Measure_Product;
    }
}

