using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Aruco;
using Emgu.CV.Flann;
using Emgu.CV.Reg;
using Euresys.Open_eVision_22_08;
using static System.Net.Mime.MediaTypeNames;

namespace cyut_Auo_Component_Measurer
{
    // ObjectSetG: Object Set Golden，標準
    // ObjectSetU: Object Set Unknown，待測物
    public partial class Form1 : Form
    {
        // 說明
        // 管理
        // EImageBW8
        // ECodedImage2
        // EObjectSelection
        // ObjectSet
        // 以及簡單邏輯(error判定, if else)

        EImageBW8 EBW8Image1 = new EImageBW8();

        bool isGolden;

        // --------------------------Instance--------------------------
        Control c_control;
        Measure c_measure;
        ShapeOperator c_shape;

        // --------------------------Shape--------------------------
        ArrayList ObjectSetG = new ArrayList();
        ArrayList ObjectSetU = new ArrayList();
        // 在讀檔時好像只會讀取 ObjectShape 的屬性(好像不叫屬性但我只記得這個)，rectangle, circle 的屬性讀不到
        //List<ObjectShape> ObjectSetG = new List<ObjectShape>();
        //List<ObjectShape> ObjectSetU = new List<ObjectShape>();

        // 要開給 FormDotGrid 讓它傳上父輩
        public int x = 5;
        public int y = 5;

        float scalingRatio;
        Graphics graphics;

        int panelIndex = 0;
        int panelNGIndex = 0;
        int panelStandardIndex = 0;

        List<int> batchIndexes = new List<int>();


        EImageBW8 EBW8ImageDotGrid = new EImageBW8();

        // --------------------------Detect--------------------------
        ECodedImage2 codedImage1 = new ECodedImage2();
        EObjectSelection codedImage1ObjectSelection = new EObjectSelection();


        // --------------------------Camera--------------------------
        bool isStreaming = false;
        Bitmap bmp;
        VideoCapture capture;

        // --------------------------Form--------------------------
        public Form1()
        {
            InitializeComponent();

            c_control = new Control();
            c_measure = new Measure();
            c_shape = new ShapeOperator();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string errorMessage;

            //初始化設定
            errorMessage = c_control.InitializeSetting(ref EBW8Image1, ref ObjectSetG, ref EBW8ImageDotGrid, ref x, ref y);

            c_shape.CalibrationX = x;
            c_shape.CalibrationY = y;

            //for(int i = 0; i < ObjectSetG.Count; i++)
            //{
            //    ObjectShape obj = (ObjectShape)ObjectSetG[i];
            //    switch (obj.shapeName)
            //    {
            //        case "square":
            //            ObjectRectangle square = (ObjectRectangle)ObjectSetG[i];
            //            Console.WriteLine("width is " + square.width);
            //            Console.WriteLine("height is " + square.height);
            //            break;
            //        case "rectangle":
            //            ObjectRectangle rect = (ObjectRectangle)ObjectSetG[i];
            //            Console.WriteLine("width is " + rect.width);
            //            Console.WriteLine("height is " + rect.height);
            //            break;
            //        case "circle":
            //            ObjectCircle circle = (ObjectCircle)ObjectSetG[i];
            //            Console.WriteLine("circle is " + circle.diameter);
            //            break;
            //    }
            //}


            if (errorMessage != c_control.OK)
                MessageBox.Show(errorMessage);


            graphics = pictureBox1.CreateGraphics();

            Learn();
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            //EBW8Image1.Load(Environment.CurrentDirectory + "\\BinImage\\PressItem.png");
            //DrawEBW8Image();
            //btn_Detect_Click(sender, e);
            //btn_Shape_Click(sender, e);
        }

        private void Form_Close(object sender, FormClosedEventArgs e)
        {
            capture?.Stop();
        }



        // --------------------------Button--------------------------

        private void btn_Load_Click(object sender, EventArgs e)
        {
            string errorMessage;

            errorMessage = c_control.LoadEImageBW8(ref EBW8Image1);

            if (errorMessage != c_control.OK)
            {
                MessageBox.Show(errorMessage);
                return;
            }

            DrawEBW8Image();
        }


        // --------------------------Menu--------------------------
        private void Menu_Save_Setting_Click(object sender, EventArgs e)
        {
            string errorMessage;
            errorMessage = c_control.MenuSaveSetting(ref EBW8Image1, ObjectSetG, ref EBW8ImageDotGrid, c_shape.CalibrationX, c_shape.CalibrationY);

            if (errorMessage != c_control.OK)
                MessageBox.Show(errorMessage);
        }

        private void Menu_Load_Setting_Click(object sender, EventArgs e)
        {
            c_control.MenuLoadSetting(ref EBW8Image1, ref ObjectSetG, ref EBW8ImageDotGrid, ref x, ref y);
            c_shape.CalibrationX = x;
            c_shape.CalibrationY = y;
        }

        private void dotGridToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // 開相機
            if (isStreaming == false)
            {
                btn_Camera_Click(sender, e);
            }

            // 設定 x, y
            x = c_shape.CalibrationX;
            y = c_shape.CalibrationY;

            Form_Dot_Grid f2 = new Form_Dot_Grid(x, y);
            f2.ShowDialog(this);

            btn_Camera_Click(sender, e);

            EBW8ImageDotGrid.SetSize(EBW8Image1);
            EasyImage.Copy(EBW8Image1, EBW8ImageDotGrid);

            c_shape.AutoCalibration(ref EBW8ImageDotGrid, x, y);
        }
        // --------------------------Camera--------------------------
        private void btn_Camera_Click(object sender, EventArgs e)
        {
            // 判斷停止還是開始
            if (isStreaming == false)
            {
                if (capture == null)
                {
                    capture = new VideoCapture(0);
                }

                capture.ImageGrabbed += Capture_ImageGrabbed;

                capture.Start(); //開始攝影
            }
            else
            {
                capture.Pause();

                Thread.Sleep(100);

                // bitmap to EImageBW8
                BitmapToEImageBW8(ref bmp, ref EBW8Image1);

                DrawEBW8Image();
            }

            isStreaming = !isStreaming;
        }


        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            Mat m = new Mat();
            try
            {
                if (isStreaming)
                {
                    //取得影像
                    //轉成 Bitmap
                    capture.Retrieve(m);
                    bmp = m.ToBitmap(); //不能使用 new Bitmap(m.Bitmap)

                    pictureBox1.Image = bmp;

                    m.Dispose();

                    if (bmp == null)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }


                //btnDetectObject_Click(sender, e); //跨執行緒作業無效: 存取控制項 'listBox1' 時所使用的執行緒與建立控制項的執行緒不同。
                //btnRecord_Click(sender, e); //跨執行緒作業無效: 存取控制項 'listBox1' 時所使用的執行緒與建立控制項的執行緒不同。
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void BitmapToEImageBW8(ref Bitmap bitmap, ref EImageBW8 image)
        {
            EImageC24 EC24ImageTemp = new EImageC24();
            BitmapData bmpData;

            try
            {
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                bmpData =
                    bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bitmap.PixelFormat); // 如果 bitmap.PixelFormat == Format8bppIndexed，會呈現出三個黑白影像

                EC24ImageTemp.SetImagePtr(bitmap.Width, bitmap.Height, bmpData.Scan0);

                bitmap.UnlockBits(bmpData);

            }
            catch (EException e)//EException為eVision的例外處理
            {
                Console.WriteLine(e.ToString());
            }

            EBW8Image1.SetSize(EC24ImageTemp);
            EasyImage.Convert(EC24ImageTemp, EBW8Image1);

            EC24ImageTemp.Dispose();
        }
        // --------------------------Camera end--------------------------
        private void pictureBox_Mose_Down(object sender, MouseEventArgs e)
        {
            // 點擊判定
            // 畫面有沒有圖案
            // 有沒有 ObjectSet

            int index;

            if (isGolden)
            {
                index = c_measure.IsClickObject(ref ObjectSetG, e.X / scalingRatio, e.Y / scalingRatio);
            }
            else
            {
                index = c_measure.IsClickObject(ref ObjectSetU, e.X / scalingRatio, e.Y / scalingRatio);
            }

            if (index == -1)
            {
                // 沒點到圖型
            }
            else
            {
                // 如果 NG index == index
                // NG selected

                listBox_Measure.SelectedIndex = index;

                listBox_NG.SelectedIndex = -1;

                for (int i = 0; i < listBox_NG.Items.Count; i++)
                {
                    int NGIndex = int.Parse(listBox_NG.Items[i].ToString().Substring(0, 3));

                    if (NGIndex == index)
                    {
                        listBox_NG.SelectedIndex = i;
                    }
                }

            }
        }

        private void btn_Measure_Standard_Click(object sender, EventArgs e)
        {
            DrawEBW8Image();

            c_measure.Detect(ref EBW8Image1, ref codedImage1, ref codedImage1ObjectSelection);

            c_measure.BuildObjectSet(ref ObjectSetG, ref codedImage1ObjectSelection, c_shape.ShapeDetermine);

            c_measure.SetObjectG(ref ObjectSetG);

            for (int i = 0; i < ObjectSetG.Count; i++)
            {
                ObjectShape shape = (ObjectShape)ObjectSetG[i];

                ListBoxAddObj(listBox_Measure, shape);
            }

            isGolden = true;
        }


        private void btn_Measure_Product_Click(object sender, EventArgs e)
        {
            DrawEBW8Image();

            c_measure.Detect(ref EBW8Image1, ref codedImage1, ref codedImage1ObjectSelection);

            c_measure.BuildObjectSet(ref ObjectSetU, ref codedImage1ObjectSelection, c_shape.ShapeDetermine);

            // show object set information
            for (int i = 0; i < ObjectSetU.Count; i++)
            {
                ObjectShape shape = (ObjectShape)ObjectSetU[i];

                ListBoxAddObj(listBox_Measure, shape);
            }

            // Inspect
            c_measure.Inspect(ref ObjectSetG, ref ObjectSetU, num_Threshold_NG.Value);

            listBox_NG.Items.Clear();

            foreach (var index in c_measure.GetNGIndex)
            {
                ECodedElement element = codedImage1ObjectSelection.GetElement((uint)index);

                DrawNGElement(ref element);

                element.Dispose();


                ObjectShape shape = (ObjectShape)ObjectSetU[index];

                ListBoxAddObj(listBox_NG, shape);
            }

            isGolden = false;
        }

        private void listBox_Measure_Selected_Changed(object sender, EventArgs e)
        {
            if (listBox_Measure.SelectedItem == null)
            {
                return;
            }

            int selectedIndex = int.Parse(listBox_Measure.SelectedItem.ToString().Substring(0, 3));

            ECodedElement element = codedImage1ObjectSelection.GetElement((uint)selectedIndex);

            DrawEBW8Image();
            DrawElement(ref element);

            if (isGolden)
            {
                RenderShapeInfo(selectedIndex, ObjectSetG);
                RenderStandard(selectedIndex, ObjectSetG);

            }
            else
            {
                RenderShapeInfo(selectedIndex, ObjectSetU);
                RenderStandard(selectedIndex, ObjectSetU);

            }

            panel_NG.Controls.Clear();

            element.Dispose();
        }

        private void listBox_NG_Selected_Changed(object sender, EventArgs e)
        {
            if (listBox_NG.SelectedItem == null)
            {
                return;
            }

            int selectedIndex = int.Parse(listBox_NG.SelectedItem.ToString().Substring(0, 3));

            if (selectedIndex < 0)
            {
                return;
            }

            listBox_Measure.SelectedIndex = selectedIndex;

            ECodedElement element = codedImage1ObjectSelection.GetElement((uint)selectedIndex);

            DrawEBW8Image();
            DrawNGElement(ref element);

            if (!isGolden)
            {
                RenderShapeErrorInfo(selectedIndex, ObjectSetU);
            }

            element.Dispose();
        }


        private void btn_Batch_Search_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox_Measure.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            ObjectShape shape = (ObjectShape)ObjectSetG[selectedIndex];
            string selectedShape = shape.shapeName;

            ECodedElement element;

            for (int i = 0; i < ObjectSetG.Count; i++)
            {
                shape = (ObjectShape)ObjectSetG[i];

                if (shape.shapeName == selectedShape)
                {
                    batchIndexes.Add(i);
                    element = codedImage1ObjectSelection.GetElement((uint)i);
                    DrawElement(ref element);

                    element.Dispose();
                }
            }


        }

        private void btn_Batch_Setting_Click(object sender, EventArgs e)
        {

        }


        // -------------------------------Method-------------------------------

        internal void ListBoxAddObj(ListBox listBox, ObjectShape obj)
        {
            string objIndex = obj.index.ToString("000");
            string objCenterX = obj.centerX.ToString("#.#");
            string objCenterY = obj.centerY.ToString("#.#");

            listBox.Items.Add(objIndex + "(" + objCenterX + "," + objCenterY + ")");
        }


        // -------------------------------View-------------------------------
        private void DrawEBW8Image()
        {
            pictureBox1.Image = null;
            pictureBox1.Refresh();
            float width = EBW8Image1.Width;
            scalingRatio = CalcRatioWithPictureBox(pictureBox1, EBW8Image1.Width, EBW8Image1.Height);
            EBW8Image1.Draw(graphics, scalingRatio);
        }
        private float CalcRatioWithPictureBox(PictureBox pb, float imageWidth, float imageHeight)
        {
            if (pb == null)
            {
                MessageBox.Show("No pictureBox.");
                return 0;
            }

            if (imageWidth == 0 || imageHeight == 0)
            {
                MessageBox.Show("長、寬不能為 0。");
                return 0;
            }


            float pbWidth = pb.Width;
            float pbHeight = pb.Height;
            float pbRatio = pbWidth / pbHeight;

            float imageRatio = imageWidth / imageHeight;

            if (imageRatio > pbRatio)
                return pbWidth / imageWidth;
            else
                return pbHeight / imageHeight;
        }



        private void DrawElement(ref ECodedElement element)
        {
            codedImage1.Draw(graphics, new ERGBColor(0, 0, 255), element, scalingRatio);

            panelIndex = 0;
            panelNGIndex = 0;
            panelStandardIndex = 0;
        }
        private void DrawNGElement(ref ECodedElement element)
        {
            codedImage1.Draw(graphics, element, scalingRatio);

            panelIndex = 0;
            panelNGIndex = 0;
            panelStandardIndex = 0;
        }



        private void RenderShapeInfo(int index, ArrayList ObjectSet)
        {
            ObjectShape shape = (ObjectShape)ObjectSet[index];

            panel_Measure.Controls.Clear();

            switch (shape.shapeName)
            {
                case "square":
                    ObjectRectangle square = (ObjectRectangle)ObjectSet[index];
                    AddItemInPanelMeasure("寬", square.width);
                    AddItemInPanelMeasure("高", square.height);
                    break;
                case "rectangle":
                    ObjectRectangle rect = (ObjectRectangle)ObjectSet[index];
                    AddItemInPanelMeasure("寬", rect.width);
                    AddItemInPanelMeasure("高", rect.height);
                    break;
                case "circle":
                    ObjectCircle circle = (ObjectCircle)ObjectSet[index];
                    AddItemInPanelMeasure("半徑", circle.diameter);
                    break;
                case "special1":
                    ObjectSpecial1 special1 = (ObjectSpecial1)ObjectSet[index];
                    AddItemInPanelMeasure("寬", special1.width);
                    AddItemInPanelMeasure("高", special1.height);
                    break;
            }

        }
        private void AddItemInPanelMeasure(string labelText, float value)
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(0, panelIndex * 30);
            label_Title.Width = 70;

            Label label_Value = new Label();
            decimal number = decimal.Round((decimal)value, 1);
            label_Value.Text = number.ToString();
            label_Value.Location = new Point(label_Title.Width + 10, panelIndex * 30);
            label_Value.BackColor = Color.FromArgb(255, 224, 192);
            label_Value.Width = 80;

            panel_Measure.Controls.Add(label_Title);
            panel_Measure.Controls.Add(label_Value);
            panelIndex++;
        }

        private void RenderShapeErrorInfo(int index, ArrayList ObjectSet)
        {
            ObjectShape shape = (ObjectShape)ObjectSet[index];

            panel_NG.Controls.Clear();

            switch (shape.shapeName)
            {
                case "square":
                    ObjectRectangle square = (ObjectRectangle)ObjectSet[index];
                    AddItemInPanelNG("寬誤差", square.widthError);
                    AddItemInPanelNG("高誤差", square.heightError);
                    break;
                case "rectangle":
                    ObjectRectangle rect = (ObjectRectangle)ObjectSet[index];
                    AddItemInPanelNG("寬誤差", rect.widthError);
                    AddItemInPanelNG("高誤差", rect.heightError);
                    break;
                case "circle":
                    ObjectCircle circle = (ObjectCircle)ObjectSet[index];
                    AddItemInPanelNG("半徑誤差", circle.diameterError);
                    break;
                case "special1":
                    ObjectSpecial1 special1 = (ObjectSpecial1)ObjectSet[index];
                    AddItemInPanelNG("寬誤差", special1.widthError);
                    AddItemInPanelNG("高誤差", special1.heightError);
                    break;
            }

        }
        private void AddItemInPanelNG(string labelText, float value)
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(0, panelNGIndex * 30);
            label_Title.Width = 70;

            Label label_Value = new Label();
            decimal number = decimal.Round((decimal)value, 1);
            label_Value.Text = number.ToString();
            label_Value.Location = new Point(label_Title.Width + 10, panelNGIndex * 30);
            label_Value.BackColor = Color.FromArgb(255, 224, 192);
            label_Value.Width = 80;

            panel_NG.Controls.Add(label_Title);
            panel_NG.Controls.Add(label_Value);
            panelNGIndex++;
        }

        private void RenderStandard(int index, ArrayList ObjectSet)
        {
            ObjectShape shape = (ObjectShape)ObjectSet[index];

            panel_Standard.Controls.Clear();

            switch (shape.shapeName)
            {
                case "square":
                    ObjectRectangle square = (ObjectRectangle)ObjectSet[index];
                    AddItemInPanelStandard("標準寬", square.widthStd);
                    AddItemInPanelStandard("標準高", square.heightStd);
                    break;
                case "rectangle":
                    ObjectRectangle rect = (ObjectRectangle)ObjectSet[index];
                    AddItemInPanelStandard("標準寬", rect.widthStd);
                    AddItemInPanelStandard("標準高", rect.heightStd);
                    break;
                case "circle":
                    ObjectCircle circle = (ObjectCircle)ObjectSet[index];
                    AddItemInPanelStandard("標準半徑", circle.diameterStd);
                    break;
                case "special1":
                    ObjectSpecial1 special1 = (ObjectSpecial1)ObjectSet[index];
                    AddItemInPanelStandard("標準寬", special1.widthStd);
                    AddItemInPanelStandard("標準高", special1.heightStd);
                    break;
            }


        }
        private void AddItemInPanelStandard(string labelText, float value) 
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(0, panelStandardIndex * 30);
            label_Title.Width = 70;

            NumericUpDown num_Standard = new NumericUpDown();
            decimal number = decimal.Round((decimal)value, 1);
            num_Standard.Increment = 2;
            num_Standard.Maximum = 5000;
            num_Standard.Value = number;

            num_Standard.Location = new Point(label_Title.Width + 10, panelStandardIndex * 30);
            num_Standard.Width = 80;

            panel_Standard.Controls.Add(label_Title);
            panel_Standard.Controls.Add(num_Standard);
            panelStandardIndex++;

        }


        // -------------------------------Adjust-------------------------------
        float adjustRatio;
        Point EBW8Image1Center;
        EImageBW8 EBW8Image2 = new EImageBW8();
        EImageBW8 EBW8ImageStd = new EImageBW8();
        // EFind
        EPatternFinder EPatternFinder1; // EPatternFinder instance
        EFoundPattern[] EPatternFinder1FoundPatterns; // EFoundPattern instances
        EPatternFinder EPatternFinder2; // EPatternFinder instance
        EFoundPattern[] EPatternFinder2FoundPatterns; // EFoundPattern instances
        float finder1CenterX;
        float finder1CenterY;

        float finder2CenterY;

        // ERoi
        EROIBW8 EBW8Roi1 = new EROIBW8(); //EROIBW8 instance
        EROIBW8 EBW8Roi2 = new EROIBW8(); //EROIBW8 instance
        Point ERoi1Center = new Point(383, 679); //手動調整


        private void btn_Adjust_Click(object sender, EventArgs e)
        {
            // 如果沒有 EBWIImage1
            if (EBW8Image1 == null || (EBW8Image1.Width == 0 && EBW8Image1.Height == 0))
            {
                MessageBox.Show("請先載入圖片或相機截圖");
                return;
            }

            Adjust_Horizontal(sender, e); //鏡像

            Adjust_Vertical(sender, e); //垂直翻轉

            Adjust_Fixed(sender, e); //放大 或 縮小會需要多次矯正

            EBW8Image2.CopyTo(EBW8Image1); //讓 EBW8IImage1 為正確的圖像

            // 畫出 EBW8Image1
            scalingRatio = CalcRatioWithPictureBox(pictureBox1, EBW8Image2.Width, EBW8Image2.Height);

            DrawEBW8Image();

            // 畫出 finder
            EPatternFinder1FoundPatterns = EPatternFinder1.Find(EBW8Image1); //找 ERoi1 的位置
            EPatternFinder2FoundPatterns = EPatternFinder2.Find(EBW8Image1); //找 ERoi2 的位置
            EPatternFinder1FoundPatterns[0].Draw(graphics, scalingRatio);
            EPatternFinder2FoundPatterns[0].Draw(graphics, scalingRatio);

        }

        private void Adjust_Fixed(object sender, EventArgs e)
        {
            // 位置校正 & 水平校正
            EPatternFinder1FoundPatterns = EPatternFinder1.Find(EBW8Image1); //找 ERoi1 的位置

            // 如果沒找到
            if (EPatternFinder1FoundPatterns[0].Score < 0.8)
            {
                MessageBox.Show("找不到類似圖形，請確認圖像是否正確。");
                return;
            }

            finder1CenterX = EPatternFinder1FoundPatterns[0].Center.X;
            finder1CenterY = EPatternFinder1FoundPatterns[0].Center.Y;

            EBW8Image2 = new EImageBW8();
            EBW8Image2.SetSize(EBW8ImageStd);

            adjustRatio = 1 / EPatternFinder1FoundPatterns[0].Scale;

            // ERoi1Center 需要手動調整
            EasyImage.ScaleRotate(EBW8Image1, finder1CenterX, finder1CenterY, ERoi1Center.X, ERoi1Center.Y, adjustRatio, adjustRatio, EPatternFinder1FoundPatterns[0].Angle, EBW8Image2, 0);
        }

        private void Adjust_Horizontal(object sender, EventArgs e)
        {
            // 水平校正後
            Adjust_Fixed(sender, e); //水平旋轉 和 大小縮放
            // 在 EBW8Image2 找 EROI2
            EPatternFinder2FoundPatterns = EPatternFinder2.Find(EBW8Image2);

            // 如果 finder2 分數太低
            // 鏡像旋轉
            if (EPatternFinder2FoundPatterns[0].Score < 0.8)
            {
                EBW8Image2 = new EImageBW8();
                EBW8Image2.SetSize(EBW8Image1);

                EBW8Image1Center = new Point(EBW8Image1.Width / 2, EBW8Image1.Height / 2);

                EasyImage.ScaleRotate(EBW8Image1, EBW8Image1Center.X, EBW8Image1Center.Y, EBW8Image1Center.X, EBW8Image1Center.Y, -1.00f, 1.00f, 0, EBW8Image2, 0);

                EBW8Image2.CopyTo(EBW8Image1);
            }
            else
            {
                //Console.WriteLine("Don't need horizontal.");
            }
        }

        private void Adjust_Vertical(object sender, EventArgs e)
        {
            // 水平校正後
            Adjust_Fixed(sender, e); //水平旋轉 和 大小縮放
            // 在 EBW8Image2 找 EROI2
            EPatternFinder1FoundPatterns = EPatternFinder1.Find(EBW8Image2); //找 ERoi1 的位置
            EPatternFinder2FoundPatterns = EPatternFinder2.Find(EBW8Image2); //找 ERoi2 的位置

            // 如果 finder1Y > finder2Y 旋轉
            finder1CenterX = EPatternFinder1FoundPatterns[0].Center.Y;
            finder2CenterY = EPatternFinder2FoundPatterns[0].Center.Y;

            if (finder1CenterX > finder2CenterY)
            {
                EBW8Image2 = new EImageBW8();
                EBW8Image2.SetSize(EBW8Image1);

                EBW8Image1Center = new Point(EBW8Image1.Width / 2, EBW8Image1.Height / 2);

                EasyImage.ScaleRotate(EBW8Image1, EBW8Image1Center.X, EBW8Image1Center.Y, EBW8Image1Center.X, EBW8Image1Center.Y, 1.00f, -1.00f, 0, EBW8Image2, 0);

                EBW8Image2.CopyTo(EBW8Image1);
            }
            else
            {
                //Console.WriteLine("Don't need vertical.");
            }
        }

        private void Learn()
        {
            EBW8ImageStd.Load(Environment.CurrentDirectory + "\\BinImage\\PressItem.png");

            EPatternFinder1 = new EPatternFinder();
            // 先學習不規則圖形
            // 可用於校正水平位置
            // Attach the roi to its parent
            EBW8Roi1.Attach(EBW8ImageStd);
            EBW8Roi1.SetPlacement(67, 558, 632, 242);
            EPatternFinder1.Learn(EBW8Roi1);

            EPatternFinder1.AngleTolerance = 50.00f;
            EPatternFinder1.ScaleTolerance = 0.60f;

            EPatternFinder2 = new EPatternFinder();
            // 第二個標準點
            // 用於 鏡像 和 垂直翻轉
            EBW8Roi2.Attach(EBW8ImageStd);
            EBW8Roi2.SetPlacement(714, 753, 594, 78);
            EPatternFinder2.Learn(EBW8Roi2);

            // 由於前面都會校正一次，所以容忍值不用太高，速度會更快
            EPatternFinder2.AngleTolerance = 10.00f;
            EPatternFinder2.ScaleTolerance = 0.10f;
        }

    }
}
