using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Reg;
using Euresys.Open_eVision_22_08;
using static System.Net.Mime.MediaTypeNames;

namespace cyut_Auo_Component_Measurer
{
    public partial class Form1 : Form
    {
        EImageBW8 EBW8Image1 = new EImageBW8();
        OpenFileDialog openFileDialog1 = new OpenFileDialog();




        View c_view;
        Control c_control;

        public Form1()
        {
            InitializeComponent();

            c_view = new View(ref pictureBox1);
            c_control = new Control();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form_Close(object sender, FormClosedEventArgs e)
        {
            capture?.Stop();
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            string errorMessage;

            errorMessage = c_control.LoadEImageBW8(ref EBW8Image1);

            if(errorMessage != c_control.OK)
            {
                MessageBox.Show(errorMessage);
                return;
            }

            c_view.DrawEBW8Image(EBW8Image1);
        }



        //--------------------------camera--------------------------
        bool isStreaming = false;
        Bitmap bmp;
        VideoCapture capture;

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

                Console.WriteLine("image");

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
    }
}
