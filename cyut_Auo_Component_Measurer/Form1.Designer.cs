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
            this.btn_Camera = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.檔案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Load_Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Save_Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.Menu_Load_Old_Image = new System.Windows.Forms.ToolStripMenuItem();
            this.設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dotGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.圖像翻轉ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.image_Rotate_0_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.image_Rotate_90_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.image_Rotate_180_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.image_Rotate_270_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.image_Flip_Horizontal_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.image_Flip_Verticle_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.btn_Batch_Setting = new System.Windows.Forms.Button();
            this.panel_Standard = new System.Windows.Forms.Panel();
            this.list_Of_Camera_Devices = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Threshold_NG)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Load
            // 
            this.btn_Load.Location = new System.Drawing.Point(18, 30);
            this.btn_Load.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.Size = new System.Drawing.Size(100, 40);
            this.btn_Load.TabIndex = 0;
            this.btn_Load.Text = "載入圖片";
            this.btn_Load.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_Load.UseVisualStyleBackColor = true;
            this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
            // 
            // btn_Camera
            // 
            this.btn_Camera.Location = new System.Drawing.Point(139, 30);
            this.btn_Camera.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Camera.Name = "btn_Camera";
            this.btn_Camera.Size = new System.Drawing.Size(159, 40);
            this.btn_Camera.TabIndex = 2;
            this.btn_Camera.Text = "camera / capture";
            this.btn_Camera.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_Camera.UseVisualStyleBackColor = true;
            this.btn_Camera.Click += new System.EventHandler(this.btn_Use_Camera_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.menuStrip1.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.檔案ToolStripMenuItem,
            this.設定ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(10, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(1435, 27);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 檔案ToolStripMenuItem
            // 
            this.檔案ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Load_Setting,
            this.Menu_Save_Setting,
            this.toolStripMenuItem2,
            this.Menu_Load_Old_Image});
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
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(131, 6);
            // 
            // Menu_Load_Old_Image
            // 
            this.Menu_Load_Old_Image.Name = "Menu_Load_Old_Image";
            this.Menu_Load_Old_Image.Size = new System.Drawing.Size(134, 22);
            this.Menu_Load_Old_Image.Text = "讀取舊檔案";
            this.Menu_Load_Old_Image.Click += new System.EventHandler(this.Menu_Load_Old_Image_Click);
            // 
            // 設定ToolStripMenuItem
            // 
            this.設定ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dotGridToolStripMenuItem,
            this.圖像翻轉ToolStripMenuItem});
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
            // 圖像翻轉ToolStripMenuItem
            // 
            this.圖像翻轉ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.image_Rotate_0_toolStripMenuItem,
            this.image_Rotate_90_toolStripMenuItem,
            this.image_Rotate_180_toolStripMenuItem,
            this.image_Rotate_270_toolStripMenuItem,
            this.toolStripMenuItem1,
            this.image_Flip_Horizontal_ToolStripMenuItem,
            this.image_Flip_Verticle_ToolStripMenuItem});
            this.圖像翻轉ToolStripMenuItem.Name = "圖像翻轉ToolStripMenuItem";
            this.圖像翻轉ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.圖像翻轉ToolStripMenuItem.Text = "圖像翻轉";
            // 
            // image_Rotate_0_toolStripMenuItem
            // 
            this.image_Rotate_0_toolStripMenuItem.Checked = true;
            this.image_Rotate_0_toolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.image_Rotate_0_toolStripMenuItem.Name = "image_Rotate_0_toolStripMenuItem";
            this.image_Rotate_0_toolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.image_Rotate_0_toolStripMenuItem.Tag = "0";
            this.image_Rotate_0_toolStripMenuItem.Text = "0";
            this.image_Rotate_0_toolStripMenuItem.Click += new System.EventHandler(this.ImageRotate);
            // 
            // image_Rotate_90_toolStripMenuItem
            // 
            this.image_Rotate_90_toolStripMenuItem.Name = "image_Rotate_90_toolStripMenuItem";
            this.image_Rotate_90_toolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.image_Rotate_90_toolStripMenuItem.Tag = "90";
            this.image_Rotate_90_toolStripMenuItem.Text = "90";
            this.image_Rotate_90_toolStripMenuItem.Click += new System.EventHandler(this.ImageRotate);
            // 
            // image_Rotate_180_toolStripMenuItem
            // 
            this.image_Rotate_180_toolStripMenuItem.Name = "image_Rotate_180_toolStripMenuItem";
            this.image_Rotate_180_toolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.image_Rotate_180_toolStripMenuItem.Tag = "180";
            this.image_Rotate_180_toolStripMenuItem.Text = "180";
            this.image_Rotate_180_toolStripMenuItem.Click += new System.EventHandler(this.ImageRotate);
            // 
            // image_Rotate_270_toolStripMenuItem
            // 
            this.image_Rotate_270_toolStripMenuItem.Name = "image_Rotate_270_toolStripMenuItem";
            this.image_Rotate_270_toolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.image_Rotate_270_toolStripMenuItem.Tag = "270";
            this.image_Rotate_270_toolStripMenuItem.Text = "270";
            this.image_Rotate_270_toolStripMenuItem.Click += new System.EventHandler(this.ImageRotate);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(119, 6);
            // 
            // image_Flip_Horizontal_ToolStripMenuItem
            // 
            this.image_Flip_Horizontal_ToolStripMenuItem.Name = "image_Flip_Horizontal_ToolStripMenuItem";
            this.image_Flip_Horizontal_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.image_Flip_Horizontal_ToolStripMenuItem.Tag = "horizontal";
            this.image_Flip_Horizontal_ToolStripMenuItem.Text = "水平翻轉";
            this.image_Flip_Horizontal_ToolStripMenuItem.Click += new System.EventHandler(this.image_Flip_Horizontal_ToolStripMenuItem_Click);
            // 
            // image_Flip_Verticle_ToolStripMenuItem
            // 
            this.image_Flip_Verticle_ToolStripMenuItem.Name = "image_Flip_Verticle_ToolStripMenuItem";
            this.image_Flip_Verticle_ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.image_Flip_Verticle_ToolStripMenuItem.Tag = "verticle";
            this.image_Flip_Verticle_ToolStripMenuItem.Text = "垂直翻轉";
            this.image_Flip_Verticle_ToolStripMenuItem.Click += new System.EventHandler(this.image_Flip_Verticle_ToolStripMenuItem_Click);
            // 
            // btn_Measure_Standard
            // 
            this.btn_Measure_Standard.Enabled = false;
            this.btn_Measure_Standard.Location = new System.Drawing.Point(18, 30);
            this.btn_Measure_Standard.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Measure_Standard.Name = "btn_Measure_Standard";
            this.btn_Measure_Standard.Size = new System.Drawing.Size(123, 40);
            this.btn_Measure_Standard.TabIndex = 6;
            this.btn_Measure_Standard.Text = "測量標準物";
            this.btn_Measure_Standard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_Measure_Standard.UseVisualStyleBackColor = true;
            this.btn_Measure_Standard.Click += new System.EventHandler(this.btn_Measure_Standard_Click);
            // 
            // btn_Measure_Product
            // 
            this.btn_Measure_Product.Enabled = false;
            this.btn_Measure_Product.Location = new System.Drawing.Point(158, 30);
            this.btn_Measure_Product.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Measure_Product.Name = "btn_Measure_Product";
            this.btn_Measure_Product.Size = new System.Drawing.Size(123, 40);
            this.btn_Measure_Product.TabIndex = 7;
            this.btn_Measure_Product.Text = "測量待測物";
            this.btn_Measure_Product.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_Measure_Product.UseVisualStyleBackColor = true;
            this.btn_Measure_Product.Click += new System.EventHandler(this.btn_Measure_Product_Click);
            // 
            // num_Threshold_NG
            // 
            this.num_Threshold_NG.DecimalPlaces = 2;
            this.num_Threshold_NG.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.num_Threshold_NG.Location = new System.Drawing.Point(171, 118);
            this.num_Threshold_NG.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_Threshold_NG.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_Threshold_NG.Name = "num_Threshold_NG";
            this.num_Threshold_NG.Size = new System.Drawing.Size(85, 29);
            this.num_Threshold_NG.TabIndex = 22;
            this.num_Threshold_NG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num_Threshold_NG.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // label_Threshold_NG
            // 
            this.label_Threshold_NG.AutoSize = true;
            this.label_Threshold_NG.Location = new System.Drawing.Point(160, 90);
            this.label_Threshold_NG.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Threshold_NG.Name = "label_Threshold_NG";
            this.label_Threshold_NG.Size = new System.Drawing.Size(86, 20);
            this.label_Threshold_NG.TabIndex = 21;
            this.label_Threshold_NG.Text = "NG門檻值:";
            // 
            // panel_Measure
            // 
            this.panel_Measure.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.panel_Measure.BackColor = System.Drawing.SystemColors.Control;
            this.panel_Measure.Location = new System.Drawing.Point(899, 248);
            this.panel_Measure.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel_Measure.Name = "panel_Measure";
            this.panel_Measure.Padding = new System.Windows.Forms.Padding(20);
            this.panel_Measure.Size = new System.Drawing.Size(176, 87);
            this.panel_Measure.TabIndex = 23;
            // 
            // listBox_Measure
            // 
            this.listBox_Measure.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.listBox_Measure.FormattingEnabled = true;
            this.listBox_Measure.ItemHeight = 20;
            this.listBox_Measure.Location = new System.Drawing.Point(898, 61);
            this.listBox_Measure.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBox_Measure.Name = "listBox_Measure";
            this.listBox_Measure.Size = new System.Drawing.Size(177, 164);
            this.listBox_Measure.TabIndex = 25;
            this.listBox_Measure.SelectedIndexChanged += new System.EventHandler(this.listBox_Measure_Selected_Changed);
            // 
            // label_Measure
            // 
            this.label_Measure.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label_Measure.AutoSize = true;
            this.label_Measure.Location = new System.Drawing.Point(895, 37);
            this.label_Measure.Name = "label_Measure";
            this.label_Measure.Size = new System.Drawing.Size(89, 20);
            this.label_Measure.TabIndex = 27;
            this.label_Measure.Text = "孔洞清單：";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(896, 354);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 20);
            this.label1.TabIndex = 30;
            this.label1.Text = "NG 孔洞清單：";
            // 
            // listBox_NG
            // 
            this.listBox_NG.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.listBox_NG.FormattingEnabled = true;
            this.listBox_NG.ItemHeight = 20;
            this.listBox_NG.Location = new System.Drawing.Point(899, 377);
            this.listBox_NG.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBox_NG.Name = "listBox_NG";
            this.listBox_NG.Size = new System.Drawing.Size(177, 164);
            this.listBox_NG.TabIndex = 29;
            this.listBox_NG.SelectedIndexChanged += new System.EventHandler(this.listBox_NG_Selected_Changed);
            // 
            // panel_NG
            // 
            this.panel_NG.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.panel_NG.BackColor = System.Drawing.SystemColors.Control;
            this.panel_NG.Location = new System.Drawing.Point(900, 549);
            this.panel_NG.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel_NG.Name = "panel_NG";
            this.panel_NG.Size = new System.Drawing.Size(176, 87);
            this.panel_NG.TabIndex = 28;
            // 
            // btn_Batch_Search
            // 
            this.btn_Batch_Search.Font = new System.Drawing.Font("新細明體", 12F);
            this.btn_Batch_Search.Location = new System.Drawing.Point(69, 537);
            this.btn_Batch_Search.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Batch_Search.Name = "btn_Batch_Search";
            this.btn_Batch_Search.Size = new System.Drawing.Size(189, 44);
            this.btn_Batch_Search.TabIndex = 44;
            this.btn_Batch_Search.Text = "搜尋相同尺寸";
            this.btn_Batch_Search.UseVisualStyleBackColor = true;
            this.btn_Batch_Search.Visible = false;
            this.btn_Batch_Search.Click += new System.EventHandler(this.btn_Batch_Search_Click);
            // 
            // btn_Batch_Setting
            // 
            this.btn_Batch_Setting.Enabled = false;
            this.btn_Batch_Setting.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_Batch_Setting.Location = new System.Drawing.Point(69, 589);
            this.btn_Batch_Setting.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Batch_Setting.Name = "btn_Batch_Setting";
            this.btn_Batch_Setting.Size = new System.Drawing.Size(189, 44);
            this.btn_Batch_Setting.TabIndex = 39;
            this.btn_Batch_Setting.Text = "批次設定尺寸";
            this.btn_Batch_Setting.UseVisualStyleBackColor = true;
            this.btn_Batch_Setting.Visible = false;
            this.btn_Batch_Setting.Click += new System.EventHandler(this.btn_Batch_Setting_Click);
            // 
            // panel_Standard
            // 
            this.panel_Standard.BackColor = System.Drawing.SystemColors.Control;
            this.panel_Standard.Location = new System.Drawing.Point(44, 418);
            this.panel_Standard.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel_Standard.Name = "panel_Standard";
            this.panel_Standard.Size = new System.Drawing.Size(226, 89);
            this.panel_Standard.TabIndex = 23;
            // 
            // list_Of_Camera_Devices
            // 
            this.list_Of_Camera_Devices.FormattingEnabled = true;
            this.list_Of_Camera_Devices.Location = new System.Drawing.Point(614, 4);
            this.list_Of_Camera_Devices.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.list_Of_Camera_Devices.Name = "list_Of_Camera_Devices";
            this.list_Of_Camera_Devices.Size = new System.Drawing.Size(134, 28);
            this.list_Of_Camera_Devices.TabIndex = 46;
            this.list_Of_Camera_Devices.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(264, 127);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "mm";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.listBox_NG);
            this.panel1.Controls.Add(this.panel_Measure);
            this.panel1.Controls.Add(this.listBox_Measure);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label_Measure);
            this.panel1.Controls.Add(this.panel_NG);
            this.panel1.Location = new System.Drawing.Point(338, 40);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1085, 699);
            this.panel1.TabIndex = 23;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(12, 8);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(876, 674);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Double_Click);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Mose_Down);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Move);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Load);
            this.groupBox1.Controls.Add(this.btn_Camera);
            this.groupBox1.Location = new System.Drawing.Point(12, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 89);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "圖片";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_Measure_Standard);
            this.groupBox2.Controls.Add(this.btn_Measure_Product);
            this.groupBox2.Controls.Add(this.num_Threshold_NG);
            this.groupBox2.Controls.Add(this.label_Threshold_NG);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 186);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(309, 161);
            this.groupBox2.TabIndex = 48;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "量測";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 394);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 20);
            this.label3.TabIndex = 21;
            this.label3.Text = "標準值:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1435, 765);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Batch_Search);
            this.Controls.Add(this.list_Of_Camera_Devices);
            this.Controls.Add(this.btn_Batch_Setting);
            this.Controls.Add(this.panel_Standard);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Close);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Threshold_NG)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private System.Windows.Forms.Button btn_Batch_Setting;
        private System.Windows.Forms.Panel panel_Standard;
        private System.Windows.Forms.ComboBox list_Of_Camera_Devices;
        private System.Windows.Forms.ToolStripMenuItem Menu_Load_Old_Image;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 圖像翻轉ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem image_Rotate_0_toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem image_Rotate_90_toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem image_Rotate_180_toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem image_Rotate_270_toolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem image_Flip_Horizontal_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem image_Flip_Verticle_ToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
    }
}

