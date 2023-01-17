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

        // --------------------------Instance--------------------------
        View c_view;
        Control c_control;
        Measure c_measure;
        ShapeManager c_shape;

        // --------------------------Shape--------------------------
        List<ObjectShape> ObjectSetG = new List<ObjectShape>();
        List<ObjectShape> ObjectSetU = new List<ObjectShape>();

        // 要開給 FormDotGrid 讓它傳上父輩
        public int x;
        public int y;

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
            c_shape = new ShapeManager();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string errorMessage;

            //初始化設定
            errorMessage = c_control.InitializeSetting(ref EBW8Image1, ref ObjectSetG, ref EBW8ImageDotGrid, ref x, ref y);

            c_shape.CalibrationX = x;
            c_shape.CalibrationY = y;

            if (errorMessage != c_control.OK)
                MessageBox.Show(errorMessage);
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            //EBW8Image1.Load(Environment.CurrentDirectory + "\\BinImage\\PressItem.png");
            //c_view.DrawEBW8Image(EBW8Image1);
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

            c_view.DrawEBW8Image(EBW8Image1);
        }

        // --------------------------Menu--------------------------
        private void Menu_Save_Setting_Click(object sender, EventArgs e)
        {
            c_control.MenuSaveSetting(ref EBW8Image1, ObjectSetG, ref EBW8ImageDotGrid, c_shape.CalibrationX, c_shape.CalibrationY);
        }

        private void Menu_Load_Setting_Click(object sender, EventArgs e)
        {
            c_control.MenuLoadSetting(ref EBW8Image1, ref ObjectSetG, ref EBW8ImageDotGrid,ref x,ref y);
            c_shape.CalibrationX= x;
            c_shape.CalibrationY= y;
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

                c_view.DrawEBW8Image(EBW8Image1);
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

                    c_view.DrawBitmap(bmp);

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


            if (ObjectSetU.Count != 0)
            {
                index = c_measure.IsClickObject(ref ObjectSetU, e.X / c_view.GetScalingRatio, e.Y / c_view.GetScalingRatio);
            }
            else
            {
                index = c_measure.IsClickObject(ref ObjectSetG, e.X / c_view.GetScalingRatio, e.Y / c_view.GetScalingRatio);
            }

            Console.WriteLine("index is " + index);

            if (index == -1)
            {
                // 沒點到圖型
            }
            else
            {
                ECodedElement element = codedImage1ObjectSelection.GetElement((uint)index);

                c_view.DrawEBW8Image(EBW8Image1);
                c_view.DrawElement(ref codedImage1, ref element);

                element.Dispose();
            }
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
            c_measure.Detect(ref EBW8Image1, ref codedImage1, ref codedImage1ObjectSelection);

            c_measure.BuildObjectSet(ref ObjectSetG, ref codedImage1ObjectSelection, c_shape.ShapeDeterminer);

            c_measure.SetObjectG(ref ObjectSetG);

            Console.WriteLine("Object Set Golden number is: " + ObjectSetG.Count);
            Console.WriteLine(ObjectSetG[0].checkResult);
        }

        private void btn_Measure_Product_Click(object sender, EventArgs e)
        {
            c_measure.Detect(ref EBW8Image1, ref codedImage1, ref codedImage1ObjectSelection);

            c_measure.BuildObjectSet(ref ObjectSetU, ref codedImage1ObjectSelection, c_shape.ShapeDeterminer);

            Console.WriteLine("Object Set Unknow number is: " + ObjectSetU.Count);
            Console.WriteLine(ObjectSetU[0].checkResult);

            // Inspect
        }
    }
}
