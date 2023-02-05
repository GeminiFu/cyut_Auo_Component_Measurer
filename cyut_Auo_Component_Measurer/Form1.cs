﻿using System;
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
        View c_view;
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

        int pennelIndex = 0;
        int pennelNGIndex = 0;


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

            c_view = new View(ref pictureBox1);
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

        private void DrawEBW8Image()
        {
            pictureBox1.Image = null;
            pictureBox1.Refresh();
            float width = EBW8Image1.Width;
            scalingRatio = CalcRatioWithPictureBox(pictureBox1, EBW8Image1.Width, EBW8Image1.Height);
            EBW8Image1.Draw(graphics, scalingRatio);
        }

        public float CalcRatioWithPictureBox(PictureBox pb, float imageWidth, float imageHeight)
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

                    if(NGIndex == index)
                    {
                        listBox_NG.SelectedIndex = i;
                    }
                }

            }
        }
        public void DrawElement(ref ECodedElement element)
        {
            codedImage1.Draw(graphics, new ERGBColor(0, 0, 255), element, scalingRatio);

            pennelIndex = 0;
            pennelNGIndex = 0;
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

        private void btn_Measure_Standard_Click(object sender, EventArgs e)
        {
            DrawEBW8Image();

            c_measure.Detect(ref EBW8Image1, ref codedImage1, ref codedImage1ObjectSelection);

            c_measure.BuildObjectSet(ref ObjectSetG, ref codedImage1ObjectSelection, c_shape.ShapeDetermine);

            c_measure.SetObjectG(ref ObjectSetG);

            for (int i = 0; i < ObjectSetG.Count; i++)
            {
                ObjectShape shape = (ObjectShape)ObjectSetG[i];

                c_view.ListBoxAddObj(listBox_Measure, shape);
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

                c_view.ListBoxAddObj(listBox_Measure, shape);
            }

            // Inspect
            c_measure.Inspect(ref ObjectSetG, ref ObjectSetU, num_Threshold_NG.Value);

            listBox_NG.Items.Clear();

            foreach (var index in c_measure.GetNGIndex)
            {
                ECodedElement element = codedImage1ObjectSelection.GetElement((uint)index);

                c_view.DrawNGElement(ref codedImage1, ref element);

                element.Dispose();


                ObjectShape shape = (ObjectShape)ObjectSetU[index];

                c_view.ListBoxAddObj(listBox_NG, shape);
            }

            isGolden = false;
        }


        private void listBox_Measure_Selected_Changed(object sender, EventArgs e)
        {
            if(listBox_Measure.SelectedItem == null)
            {
                return;
            }

            int selectedIndex = int.Parse(listBox_Measure.SelectedItem.ToString().Substring(0, 3));

            ECodedElement element = codedImage1ObjectSelection.GetElement((uint)selectedIndex);

            DrawEBW8Image();
            DrawElement(ref element);

            if (isGolden)
            {
                c_view.RenderShapeInfo(panel_Measure_Num, selectedIndex, ObjectSetG);
            }
            else
            {
                c_view.RenderShapeInfo(panel_Measure_Num, selectedIndex, ObjectSetU);
            }

            panel_NG_Num.Controls.Clear();

            element.Dispose();
        }

        private void listBox_NG_Selected_Changed(object sender, EventArgs e)
        {
            if(listBox_NG.SelectedItem == null)
            {
                return;
            }

            int selectedIndex = int.Parse(listBox_NG.SelectedItem.ToString().Substring(0, 3));

            if (selectedIndex < 0)
            {
                return;
            }

            ECodedElement element = codedImage1ObjectSelection.GetElement((uint)selectedIndex);

            DrawEBW8Image();
            c_view.DrawNGElement(ref codedImage1, ref element);

            if (!isGolden)
            {
                c_view.RenderShapeErrorInfo(panel_NG_Num, selectedIndex, ObjectSetU);
            }

            element.Dispose();
        }

        List<int> batchIndexes = new List<int>();

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
                    c_view.DrawElement(ref codedImage1, ref element);

                    element.Dispose();
                }
            }


        }

        private void btn_Batch_Setting_Click(object sender, EventArgs e)
        {

        }
    }
}
