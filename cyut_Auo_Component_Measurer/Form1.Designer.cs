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
            this.num_Threshold_NG = new System.Windows.Forms.NumericUpDown();
            this.label_Threshold_NG = new System.Windows.Forms.Label();
            this.panel_Measure = new System.Windows.Forms.Panel();
            this.listBox_Measure = new System.Windows.Forms.ListBox();
            this.label_Measure = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox_NG = new System.Windows.Forms.ListBox();
            this.panel_NG = new System.Windows.Forms.Panel();
            this.btn_Batch_Search = new System.Windows.Forms.Button();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_Batch_Setting = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel_Standard = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Threshold_NG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Load
            // 
            this.btn_Load.Location = new System.Drawing.Point(63, 72);
            this.btn_Load.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.Size = new System.Drawing.Size(112, 31);
            this.btn_Load.TabIndex = 0;
            this.btn_Load.Text = "載入圖片";
            this.btn_Load.UseVisualStyleBackColor = true;
            this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(219, 135);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(648, 414);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Mose_Down);
            // 
            // btn_Camera
            // 
            this.btn_Camera.Location = new System.Drawing.Point(212, 72);
            this.btn_Camera.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Camera.Name = "btn_Camera";
            this.btn_Camera.Size = new System.Drawing.Size(112, 31);
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
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1216, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 檔案ToolStripMenuItem
            // 
            this.檔案ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Load_Setting,
            this.Menu_Save_Setting});
            this.檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            this.檔案ToolStripMenuItem.Size = new System.Drawing.Size(43, 19);
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
            this.設定ToolStripMenuItem.Size = new System.Drawing.Size(43, 19);
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
            this.btn_Measure_Standard.Location = new System.Drawing.Point(30, 361);
            this.btn_Measure_Standard.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Measure_Standard.Name = "btn_Measure_Standard";
            this.btn_Measure_Standard.Size = new System.Drawing.Size(112, 31);
            this.btn_Measure_Standard.TabIndex = 6;
            this.btn_Measure_Standard.Text = "測量標準物";
            this.btn_Measure_Standard.UseVisualStyleBackColor = true;
            this.btn_Measure_Standard.Click += new System.EventHandler(this.btn_Measure_Standard_Click);
            // 
            // btn_Measure_Product
            // 
            this.btn_Measure_Product.Location = new System.Drawing.Point(546, 72);
            this.btn_Measure_Product.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Measure_Product.Name = "btn_Measure_Product";
            this.btn_Measure_Product.Size = new System.Drawing.Size(112, 31);
            this.btn_Measure_Product.TabIndex = 7;
            this.btn_Measure_Product.Text = "測量待測物";
            this.btn_Measure_Product.UseVisualStyleBackColor = true;
            this.btn_Measure_Product.Click += new System.EventHandler(this.btn_Measure_Product_Click);
            // 
            // num_Threshold_NG
            // 
            this.num_Threshold_NG.DecimalPlaces = 1;
            this.num_Threshold_NG.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.num_Threshold_NG.Location = new System.Drawing.Point(1002, 590);
            this.num_Threshold_NG.Margin = new System.Windows.Forms.Padding(4);
            this.num_Threshold_NG.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_Threshold_NG.Name = "num_Threshold_NG";
            this.num_Threshold_NG.Size = new System.Drawing.Size(88, 27);
            this.num_Threshold_NG.TabIndex = 22;
            this.num_Threshold_NG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Threshold_NG.Value = new decimal(new int[] {
            15,
            0,
            0,
            65536});
            // 
            // label_Threshold_NG
            // 
            this.label_Threshold_NG.AutoSize = true;
            this.label_Threshold_NG.Location = new System.Drawing.Point(867, 593);
            this.label_Threshold_NG.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Threshold_NG.Name = "label_Threshold_NG";
            this.label_Threshold_NG.Size = new System.Drawing.Size(113, 16);
            this.label_Threshold_NG.TabIndex = 21;
            this.label_Threshold_NG.Text = "尺寸NG門檻值:";
            // 
            // panel_Measure
            // 
            this.panel_Measure.BackColor = System.Drawing.SystemColors.Control;
            this.panel_Measure.Location = new System.Drawing.Point(870, 226);
            this.panel_Measure.Name = "panel_Measure";
            this.panel_Measure.Size = new System.Drawing.Size(200, 90);
            this.panel_Measure.TabIndex = 23;
            // 
            // listBox_Measure
            // 
            this.listBox_Measure.FormattingEnabled = true;
            this.listBox_Measure.ItemHeight = 16;
            this.listBox_Measure.Location = new System.Drawing.Point(870, 88);
            this.listBox_Measure.Name = "listBox_Measure";
            this.listBox_Measure.Size = new System.Drawing.Size(160, 132);
            this.listBox_Measure.TabIndex = 25;
            this.listBox_Measure.SelectedIndexChanged += new System.EventHandler(this.listBox_Measure_Selected_Changed);
            // 
            // label_Measure
            // 
            this.label_Measure.AutoSize = true;
            this.label_Measure.Location = new System.Drawing.Point(867, 69);
            this.label_Measure.Name = "label_Measure";
            this.label_Measure.Size = new System.Drawing.Size(87, 16);
            this.label_Measure.TabIndex = 27;
            this.label_Measure.Text = "孔洞清單：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(867, 328);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 16);
            this.label1.TabIndex = 30;
            this.label1.Text = "NG 孔洞清單：";
            // 
            // listBox_NG
            // 
            this.listBox_NG.FormattingEnabled = true;
            this.listBox_NG.ItemHeight = 16;
            this.listBox_NG.Location = new System.Drawing.Point(870, 347);
            this.listBox_NG.Name = "listBox_NG";
            this.listBox_NG.Size = new System.Drawing.Size(160, 132);
            this.listBox_NG.TabIndex = 29;
            this.listBox_NG.SelectedIndexChanged += new System.EventHandler(this.listBox_NG_Selected_Changed);
            // 
            // panel_NG
            // 
            this.panel_NG.BackColor = System.Drawing.SystemColors.Control;
            this.panel_NG.Location = new System.Drawing.Point(870, 485);
            this.panel_NG.Name = "panel_NG";
            this.panel_NG.Size = new System.Drawing.Size(200, 90);
            this.panel_NG.TabIndex = 28;
            // 
            // btn_Batch_Search
            // 
            this.btn_Batch_Search.Font = new System.Drawing.Font("新細明體", 12F);
            this.btn_Batch_Search.Location = new System.Drawing.Point(30, 414);
            this.btn_Batch_Search.Name = "btn_Batch_Search";
            this.btn_Batch_Search.Size = new System.Drawing.Size(121, 30);
            this.btn_Batch_Search.TabIndex = 44;
            this.btn_Batch_Search.Text = "搜尋相同尺寸";
            this.btn_Batch_Search.UseVisualStyleBackColor = true;
            this.btn_Batch_Search.Click += new System.EventHandler(this.btn_Batch_Search_Click);
            // 
            // numHeight
            // 
            this.numHeight.DecimalPlaces = 1;
            this.numHeight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numHeight.Location = new System.Drawing.Point(401, 624);
            this.numHeight.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(73, 27);
            this.numHeight.TabIndex = 43;
            this.numHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // numWidth
            // 
            this.numWidth.DecimalPlaces = 1;
            this.numWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numWidth.Location = new System.Drawing.Point(401, 591);
            this.numWidth.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(73, 27);
            this.numWidth.TabIndex = 42;
            this.numWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(336, 624);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 16);
            this.label7.TabIndex = 41;
            this.label7.Text = "標準高:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(336, 593);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 16);
            this.label9.TabIndex = 40;
            this.label9.Text = "標準寬:";
            // 
            // btn_Batch_Setting
            // 
            this.btn_Batch_Setting.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_Batch_Setting.Location = new System.Drawing.Point(30, 462);
            this.btn_Batch_Setting.Name = "btn_Batch_Setting";
            this.btn_Batch_Setting.Size = new System.Drawing.Size(121, 30);
            this.btn_Batch_Setting.TabIndex = 39;
            this.btn_Batch_Setting.Text = "批次設定尺寸";
            this.btn_Batch_Setting.UseVisualStyleBackColor = true;
            this.btn_Batch_Setting.Click += new System.EventHandler(this.btn_Batch_Setting_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(384, 72);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 31);
            this.button1.TabIndex = 45;
            this.button1.Text = "Adjust";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // panel_Standard
            // 
            this.panel_Standard.BackColor = System.Drawing.SystemColors.Control;
            this.panel_Standard.Location = new System.Drawing.Point(12, 498);
            this.panel_Standard.Name = "panel_Standard";
            this.panel_Standard.Size = new System.Drawing.Size(200, 153);
            this.panel_Standard.TabIndex = 23;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 654);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Batch_Search);
            this.Controls.Add(this.numHeight);
            this.Controls.Add(this.numWidth);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btn_Batch_Setting);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox_NG);
            this.Controls.Add(this.panel_NG);
            this.Controls.Add(this.label_Measure);
            this.Controls.Add(this.listBox_Measure);
            this.Controls.Add(this.panel_Standard);
            this.Controls.Add(this.panel_Measure);
            this.Controls.Add(this.num_Threshold_NG);
            this.Controls.Add(this.label_Threshold_NG);
            this.Controls.Add(this.btn_Measure_Product);
            this.Controls.Add(this.btn_Measure_Standard);
            this.Controls.Add(this.btn_Camera);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_Load);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("新細明體", 12F);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Close);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Threshold_NG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
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
        private System.Windows.Forms.NumericUpDown num_Threshold_NG;
        private System.Windows.Forms.Label label_Threshold_NG;
        private System.Windows.Forms.Panel panel_Measure;
        private System.Windows.Forms.ListBox listBox_Measure;
        private System.Windows.Forms.Label label_Measure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox_NG;
        private System.Windows.Forms.Panel panel_NG;
        private System.Windows.Forms.Button btn_Batch_Search;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_Batch_Setting;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel_Standard;
    }
}

