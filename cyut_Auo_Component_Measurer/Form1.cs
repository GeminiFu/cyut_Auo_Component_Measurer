using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Flann;
using Emgu.Util;
using Euresys.Open_eVision_22_08;
using MvCamCtrl.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
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

        EImageC24 EC24Image1 = new EImageC24();
        EImageBW8 EBW8Image1 = new EImageBW8();// pictureBox
        EImageBW8 EBW8ImageTemp = new EImageBW8();
        EImageBW8 EBW8ImageStd = new EImageBW8();// Standard

        internal ECodedImage2 codedImageStd = new ECodedImage2();
        internal EObjectSelection codedStdSelection = new EObjectSelection();

        bool isGolden;
        // --------------------------Instance--------------------------
        Control c_control = new Control();

        // --------------------------View--------------------------
        EImageBW8 EBW8ImageView = new EImageBW8();// View
        float viewRatio;
        Graphics graphics;
        ToolStripMenuItem imageTranseformMenuItem;
        int rotateDegree = 0;

        int panelIndex = 0;
        int panelNGIndex = 0;
        int panelStandardIndex = 0;

        // --------------------------Batch--------------------------
        List<int> batchIndexes = new List<int>();


        // --------------------------Camera--------------------------
        bool isStreaming = false;
        Bitmap bmp;
        VideoCapture capture;


        // MVS Camera
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        private MyCamera m_pMyCamera;

        // ch:用于从驱动获取图像的缓存 | en:Buffer for getting image from driver
        UInt32 m_nBufSizeForDriver = 3072 * 2048 * 3;
        byte[] m_pBufForDriver = new byte[3072 * 2048 * 3];

        // ch:用于保存图像的缓存 | en:Buffer for saving image
        UInt32 m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
        byte[] m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];


        // -------------------------------Adjust-------------------------------
        EImageBW8 EBW8ImageAdjust = new EImageBW8();
        EImageBW8 EBW8ImageLearn = new EImageBW8();
        float adjustRatio;
        Point EBW8Image1Center;

        // EFind
        EPatternFinder EPatternFinder1 = new EPatternFinder(); // EPatternFinder instance
        EFoundPattern[] EPatternFinder1FoundPatterns; // EFoundPattern instances
        float finder1CenterX;
        float finder1CenterY;

        // ERoi
        EROIBW8 EBW8Roi1 = new EROIBW8(); //EROIBW8 instance
        Point ERoi1Center = new Point(383, 679); //手動調整

        // -------------------------------Measure-------------------------------
        EImageEncoder codedImage1Encoder = new EImageEncoder();
        internal ECodedImage2 codedImage1 = new ECodedImage2();
        internal ECodedImage2 codedImageView = new ECodedImage2();
        internal EObjectSelection coded1Selection = new EObjectSelection();
        internal EObjectSelection codedViewSelection = new EObjectSelection();

        internal ArrayList ObjectSetG = new ArrayList();
        internal ArrayList ObjectSetU = new ArrayList();

        List<int> NGIndex = new List<int>();

        ObjectInfo[] ObjectSetInspect;
        bool[] isFindTheSameList;
        List<uint> missingList = new List<uint>();
        List<uint> noStdNGList = new List<uint>();
        List<uint> areaErrorList = new List<uint>();
        List<uint> sizeErrorList = new List<uint>();

        // --------------------------Shape--------------------------
        EWorldShape EWorldShape1 = new EWorldShape();
        internal EImageBW8 EBW8ImageDotGrid = new EImageBW8();
        public int calibrationX = 5;
        public int calibrationY = 5;
        public bool isDoCalibration = false;

        // --------------------------Picturebox--------------------------
        int mutiple = 1;
        bool isEnlarge = false;
        float moveXRatio;
        float moveYRatio;
        int currentMouseX;
        int currentMouseY;


        string OK = "Message OK.";

        // --------------------------Form--------------------------
        public Form1()
        {
            InitializeComponent();

            string errorMessage;

            // Learn
            Learn();// 為了 EasyFind 學習

            FormInitialize();

            // 初始化設定
            errorMessage = c_control.InitializeSetting(ref EBW8ImageStd, ref ObjectSetG, ref EBW8ImageDotGrid, ref calibrationX, ref calibrationY);

            if (errorMessage != c_control.OK)
            {
                MessageBox.Show("請設定 點圖校正 和 標準數據 並存檔。");
                return;
            }

            // Calibration
            Calibration(ref EBW8ImageDotGrid);
        }

        private void Form_Close(object sender, FormClosedEventArgs e)
        {
            // 關閉攝影機
            capture?.Stop();

            if (m_pMyCamera != null)
            {
                if (m_pMyCamera.MV_CC_IsDeviceConnected_NET())
                {
                    m_pMyCamera.MV_CC_CloseDevice_NET();
                    m_pMyCamera.MV_CC_DestroyDevice_NET();
                }
            }
        }

        // --------------------------Menu--------------------------
        private void Menu_Save_Setting_Click(object sender, EventArgs e)
        {
            string errorMessage;
            if (EWorldShape1.CalibrationSucceeded() == false)
            {
                MessageBox.Show("校正失敗，請重新校正。");
                return;
            }
            else
            {
                errorMessage = c_control.MenuSaveSetting(ref EBW8Image1, ObjectSetG, ref EBW8ImageDotGrid, calibrationX, calibrationY);

                if (errorMessage != c_control.OK)
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                MessageBox.Show("儲存成功。");
            }
        }

        private void Menu_Load_Setting_Click(object sender, EventArgs e)
        {
            string errorMessage;

            errorMessage = c_control.MenuLoadSetting(ref EBW8Image1, ObjectSetG, ref EBW8ImageDotGrid, calibrationX, calibrationY);

            if (errorMessage != c_control.OK)
            {
                MessageBox.Show(errorMessage);
                return;
            }
        }

        private void dotGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message;

            // 開相機
            if (isStreaming == false)
            {
                message = EmguCV_Camera();

                if (message != OK)
                {
                    MessageBox.Show(message);
                    return;
                }
            }

            if (MessageBox.Show("焦距設定好了？", "問題", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            Form_Dot_Grid f2 = new Form_Dot_Grid(calibrationX, calibrationY);
            f2.ShowDialog(this);

            if (isDoCalibration == false)
            {
                EmguCV_Camera();
                return;
            }

            message = EmguCV_Camera();

            if (message != OK)
            {
                MessageBox.Show(message);
                return;
            }

            BitmapToEImageBW8(bmp, ref EBW8ImageDotGrid);

            // Calibration
            if (EBW8ImageDotGrid.IsVoid == false)
            {
                message = AutoCalibration(ref EBW8ImageDotGrid);
                MessageBox.Show(message);

                if (message == "校正失敗")
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("校正用的點圖為空。");
                return;
            }

            EasyImage.Copy(EBW8ImageDotGrid, EBW8Image1);

            UnwarpEBW8Image1();

            ImageRotate(imageTranseformMenuItem, e);

            DrawEBW8Image(ref EBW8Image1);

            isDoCalibration = false;
        }

        private void Menu_Load_Old_Image_Click(object sender, EventArgs e)
        {
            string errorMessage = "";

            int calibrationX = 0;
            int calibrationY = 0;

            ProductDataReset();

            errorMessage = c_control.LoadOldImage(ref EBW8ImageStd, ref EBW8Image1, ref ObjectSetG, ref EBW8ImageDotGrid, ref calibrationX, ref calibrationY);

            if (errorMessage != c_control.OK)
            {
                //MessageBox.Show(errorMessage);
                MessageBox.Show("沒有找到設定檔。");
                return;
            }

            errorMessage = Detect(ref codedImage1, ref coded1Selection, ref EBW8Image1);

            if (errorMessage != OK)
            {
                MessageBox.Show(errorMessage);
                return;
            }

            // Build ObjectSet
            ObjectSetU.Clear();
            listBox_Measure.Items.Clear();

            for (uint i = 0; i < coded1Selection.ElementCount; i++)
            {
                ECodedElement element = coded1Selection.GetElement(i); //get element

                ObjectInfo objectInfo = ElementToObjectInfo(ref EBW8Image1, ref element, i); //element to shape

                ObjectSetU.Add(objectInfo); //Add into ObjectSet


                ListBoxAddObj(listBox_Measure, objectInfo); //Add in ListBox

                element.Dispose();
            }

            // Inspect
            Inspect((float)num_Threshold_NG.Value);

            // View
            listBox_NG.Items.Clear();

            DrawEBW8Image(ref EBW8Image1);

            foreach (var index in NGIndex)
            {
                ECodedElement element = coded1Selection.GetElement((uint)index);

                DrawNGElement(ref element, new ERGBColor(255, 0, 0));

                element.Dispose();

                ListBoxAddObj(listBox_NG, (ObjectInfo)ObjectSetU[index]);
            }
        }




        // --------------------------Button--------------------------
        private void btn_Load_Click(object sender, EventArgs e)
        {
            // 讀取檔案
            // 圖片旋轉
            // 畫出圖片
            // 重設產品資料
            // 介面呈現變動
            string message;

            message = c_control.LoadEImageBW8(ref EBW8Image1);

            if (message != c_control.OK)
            {
                MessageBox.Show(message);
                return;
            }

            ImageRotate(imageTranseformMenuItem, e);

            DrawEBW8Image(ref EBW8Image1);

            ProductDataReset();
            pictureClickView();
        }

        private void btn_Use_Camera_Click(object sender, EventArgs e)
        {
            // 確認已校正
            // 截圖
            // 調整圖片位置
            // 主動測量產品
            string message;

            if (EWorldShape1.CalibrationSucceeded() == false)
            {
                MessageBox.Show("請先設定點圖校正。");

                dotGridToolStripMenuItem_Click(sender, e);
                return;
            }
            else
            {
                //EmguCV_Camera();
                message = GetCapture();

                if (message != OK)
                {
                    MessageBox.Show(message);
                    return;
                }

                message = btn_Adjust_Click(sender, e);

                if (message != OK)
                {
                    MessageBox.Show(message);
                    return;
                }


                if (checkBox_Direct_Measure.Checked)
                {
                    btn_Measure_Product_Click(sender, e);
                }

            }

        }

        // 校正圖像
        private string btn_Adjust_Click(object sender, EventArgs e)
        {
            // 位置校正
            // 畫出 EBW8Image1
            // 畫出 finder
            string message;

            message = Adjust_Fixed();//放大 或 縮小會需要多次矯正，比較準

            if (message != OK)
            {
                return message;
            }

            DrawEBW8Image(ref EBW8Image1);

            EPatternFinder1FoundPatterns = EPatternFinder1.Find(EBW8Image1); //找 ERoi1 的位置
            EPatternFinder1FoundPatterns[0].Draw(graphics, viewRatio);

            return OK;
        }

        // -------------------------------Measure-------------------------------

        // 偵測物件
        // 建立 ObjectSetG
        // 放進 listBox_Measure
        // 設置 ObjectSetG
        // 存檔
        private void btn_Measure_Standard_Click(object sender, EventArgs e)
        {
            // 偵測產品物件
            // 建立 ObjectSetG
            // 設定標準資料
            // 畫出圖片
            // 存檔
            // 介面變動
            Detect(ref codedImage1, ref coded1Selection, ref EBW8Image1);

            BuildObjectSet(ObjectSetG, listBox_Measure);

            SetObjectG();
            EBW8ImageStd.SetSize(EBW8Image1);
            EasyImage.Copy(EBW8Image1, EBW8ImageStd);

            DrawEBW8Image(ref EBW8Image1);

            c_control.SaveHistorySetting(ref EBW8Image1, ObjectSetG, ref EBW8ImageDotGrid, calibrationX, calibrationY);


            isGolden = true;

            MeasureStdClickView();
        }

        // 偵測物件
        private void btn_Measure_Product_Click(object sender, EventArgs e)
        {
            // 偵測產品物件
            // 建立 ObjectSetG
            // 設定標準資料
            // 畫出圖片
            // 存檔
            // 介面變動

            missingList.Clear();
            sizeErrorList.Clear();
            areaErrorList.Clear();

            if (isStreaming == true)
            {
                btn_Use_Camera_Click(new object(), new EventArgs());
            }
            Detect(ref codedImage1, ref coded1Selection, ref EBW8Image1);

            BuildObjectSet(ObjectSetU, listBox_Measure);



            Inspect((float)num_Threshold_NG.Value);


            Detect(ref codedImageStd, ref codedStdSelection, ref EBW8ImageStd);

            DrawNG();

            // Save File
            if (NGIndex.Count == 0 && missingList.Count == 0)
            {
                c_control.SaveInspectResult(ref EBW8Image1, true);
                OKView();
            }
            else
            {
                c_control.SaveInspectResult(ref EBW8Image1, false);
            }

            isGolden = false;
            MeasureProductClickView();
        }

        // -------------------------------Batch-------------------------------

        private void btn_Batch_Search_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox_Measure.SelectedIndex;

            if (selectedIndex < 0)
            {
                MessageBox.Show("請先選取物件。");
                return;
            }

            ArrayList TempObjectSet;
            ObjectInfo selectedObjectInfo;
            ObjectInfo objectInfo;
            float threshold = 1;

            if (isGolden)
            {
                TempObjectSet = ObjectSetG;
            }
            else
            {
                TempObjectSet = ObjectSetU;
            }

            selectedObjectInfo = (ObjectInfo)TempObjectSet[selectedIndex];

            batchIndexes.Clear();

            for (int i = 0; i < TempObjectSet.Count; i++)
            {
                objectInfo = (ObjectInfo)TempObjectSet[i];
                if (
                     objectInfo.ShapeName == selectedObjectInfo.ShapeName &&
                     Math.Abs(selectedObjectInfo.width - objectInfo.width) < threshold &&
                     Math.Abs(selectedObjectInfo.height - objectInfo.height) < threshold
                   )
                {
                    batchIndexes.Add(i);

                    // View
                    ECodedElement element;
                    element = coded1Selection.GetElement((uint)i);
                    DrawElement(ref element);
                    element.Dispose();
                }
            }

            btn_Batch_Setting.Enabled = true;
        }

        private void btn_Batch_Setting_Click(object sender, EventArgs e)
        {
            float widthStd;
            float heightStd;

            // 取得設定值
            if (panel_Standard.Controls.Count == 3)
            {
                NumericUpDown controlDiameter = (NumericUpDown)panel_Standard.Controls[1];

                widthStd = (float)controlDiameter.Value;
                heightStd = (float)controlDiameter.Value;
            }
            else
            {
                NumericUpDown controlWidth = (NumericUpDown)panel_Standard.Controls[1];
                NumericUpDown controlHeight = (NumericUpDown)panel_Standard.Controls[4];

                widthStd = (float)controlWidth.Value;
                heightStd = (float)controlHeight.Value;
            }

            // 批次設定
            float threashold = 0.5f;

            foreach (var index in batchIndexes)
            {
                ObjectInfo objectInfo = (ObjectInfo)ObjectSetG[index];

                if (Math.Abs(objectInfo.widthStd - widthStd) < threashold)
                {
                    objectInfo.widthStd = widthStd;
                }
                else
                {
                    MessageBox.Show("寬的長度與設定長度差距過大。");
                }
                if (Math.Abs(objectInfo.heightStd - heightStd) < threashold)
                {
                    objectInfo.heightStd = heightStd;
                }
                else
                {
                    MessageBox.Show("高的長度與設定長度差距過大。");
                }
            }

            batchIndexes.Clear();
        }

        // --------------------------Picturebox--------------------------
        private void pictureBox_Mose_Down(object sender, MouseEventArgs e)
        {
            // 點擊判定
            // 畫面有沒有圖案
            // 有沒有 ObjectSet

            int elementIndex;
            ArrayList TempObjectSet;

            if (isGolden)
            {
                TempObjectSet = ObjectSetG;
            }
            else
            {
                TempObjectSet = ObjectSetU;
            }

            elementIndex = IsClickObject(ref TempObjectSet, e.X / viewRatio, e.Y / viewRatio);

            if (elementIndex == -1)
            {
                // 沒點到圖型
            }
            else
            {
                ObjectInfo selectedObject = (ObjectInfo)TempObjectSet[elementIndex];

                SearchIndexInListBox(listBox_Measure, elementIndex);

                // listBox_NG
                listBox_NG.SelectedIndex = -1;

                //for (int i = 0; i < NGIndex.Count; i++)
                //{
                //    if (NGIndex[i] == elementIndex)
                //    {
                //        listBox_NG.SelectedIndex = i;
                //    }
                //}
                SearchIndexInListBox(listBox_NG, elementIndex);

            }
        }

        private void SearchIndexInListBox(ListBox listBox, int Index)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if (int.Parse(listBox.Items[i].ToString().Substring(0, 3)) == Index)
                {
                    listBox.SelectedIndex = i;
                }
            }
        }


        private void pictureBox_Mouse_Wheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                mutiple *= 2;
                isEnlarge = true;
            }
            else
            {
                mutiple /= 2;
            }

            if (mutiple <= 1)
            {
                mutiple = 1;
                isEnlarge = false;
                DrawEBW8Image(ref EBW8Image1);
            }

            if (mutiple >= 1 / viewRatio)
            {
                mutiple = (int)Math.Round((double)1 / (double)viewRatio);
            }
        }

        private void pictureBox_Move(object sender, MouseEventArgs e)
        {
            if (isEnlarge)
            {
                currentMouseX = e.X;
                currentMouseY = e.Y;
                moveXRatio = (viewRatio * mutiple * EBW8Image1.Width - pictureBox1.Width) / pictureBox1.Width;
                moveYRatio = (viewRatio * mutiple * EBW8Image1.Height - pictureBox1.Height) / pictureBox1.Height;
                EBW8Image1.Draw(graphics, viewRatio * mutiple, viewRatio * mutiple, -1f * currentMouseX * moveXRatio / (viewRatio * mutiple), -1f * currentMouseY * moveYRatio / (viewRatio * mutiple));
            }
        }

        private void pictureBox_Double_Click(object sender, MouseEventArgs e)
        {
            currentMouseX = e.X;
            currentMouseY = e.Y;

            if (mutiple == 1)
            {
                mutiple = 2;
                isEnlarge = true;
            }
            else
            {
                mutiple = 1;
                isEnlarge = false;
                DrawEBW8Image(ref EBW8Image1);
            }
        }

        // -------------------------------Listbox-------------------------------
        private void listBox_Measure_Selected_Changed(object sender, EventArgs e)
        {
            if (listBox_Measure.SelectedItem == null)
            {
                return;
            }


            int elementIndex = int.Parse(listBox_Measure.SelectedItem.ToString().Substring(0, 3));

            ECodedElement element = coded1Selection.GetElement((uint)elementIndex);

            DrawEBW8Image(ref EBW8Image1);
            DrawElement(ref element);
            listBox_NG.SelectedIndex = -1;

            for (int i = 0; i < NGIndex.Count; i++)
            {
                if (NGIndex[i] == elementIndex)
                {
                    listBox_NG.SelectedIndex = i;
                }
            }

            if (isGolden)
            {
                RenderShapeInfo(listBox_Measure.SelectedIndex, ObjectSetG);
                RenderStandard(listBox_Measure.SelectedIndex, ObjectSetG);
            }
            else
            {
                RenderShapeInfo(listBox_Measure.SelectedIndex, ObjectSetU);
                RenderStandard(listBox_Measure.SelectedIndex, ObjectSetU);
            }

            panelIndex = 0;
            panelStandardIndex = 0;
            panel_NG.Controls.Clear();

            element.Dispose();
        }

        private void listBox_NG_Selected_Changed(object sender, EventArgs e)
        {
            if (listBox_NG.SelectedItem == null)
            {
                return;
            }

            panelNGIndex = 0;

            int elementIndex = int.Parse(listBox_NG.SelectedItem.ToString().Substring(0, 3));

            if (elementIndex < 0)
            {
                return;
            }

            for (int i = 0; i < listBox_Measure.Items.Count; i++)
            {
                if (int.Parse(listBox_Measure.Items[i].ToString().Substring(0, 3)) == elementIndex)
                {
                    listBox_Measure.SelectedIndex = i;
                }
            }


            ECodedElement element = coded1Selection.GetElement((uint)elementIndex);

            DrawEBW8Image(ref EBW8Image1);
            DrawNGElement(ref element, new ERGBColor(255, 0, 0));

            if (!isGolden)
            {
                RenderShapeErrorInfo(listBox_Measure.SelectedIndex, ObjectSetU);
            }

            element.Dispose();
        }

        // --------------------------Camera--------------------------
        private string EmguCV_Camera()
        {
            // 判斷停止還是開始
            if (isStreaming == false)
            {
                if (capture == null)
                {
                    capture = new VideoCapture(0, VideoCapture.API.DShow);
                    capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 4000);
                    capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 3000);
                }

                if (capture.IsOpened)
                {
                    capture.ImageGrabbed += Capture_ImageGrabbed;

                    capture.Start(); //開始攝影
                }
                else
                {
                    capture = null;
                    return "無法連接相機。";
                }
            }
            else
            {
                if (capture.Grab() == false)
                {
                    capture = null;
                    isStreaming = false;
                    return "相機失去連接。";
                }

                capture.Pause();

                Thread.Sleep(500);

                if (bmp.Width * bmp.Height < 1000000)
                {
                    return "畫素少於一百萬。";
                }

                // bitmap to EImageBW8
                BitmapToEImageBW8(bmp, ref EBW8Image1);

                // Unwarp
                UnwarpEBW8Image1();

                // Init
                ProductDataReset();

                ImageRotate(imageTranseformMenuItem, new EventArgs());

                DrawEBW8Image(ref EBW8Image1);

                btn_Measure_Standard.Enabled = true;
                if (ObjectSetG == null)
                {
                    btn_Measure_Product.Enabled = false;
                }
                else
                {
                    btn_Measure_Product.Enabled = true;
                }

                btn_Batch_Search.Visible = false;
                btn_Batch_Setting.Visible = false;
            }

            isStreaming = !isStreaming;

            return OK;
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



                    BitmapToEImageBW8(bmp, ref EBW8Image1);

                    if (mutiple == 1)
                    {
                        pictureBox1.Image = bmp;
                    }
                    else
                    {
                        isEnlarge = false;

                        moveXRatio = (viewRatio * mutiple * EBW8Image1.Width - pictureBox1.Width) / pictureBox1.Width;
                        moveYRatio = (viewRatio * mutiple * EBW8Image1.Height - pictureBox1.Height) / pictureBox1.Height;
                        EBW8Image1.Draw(graphics, viewRatio * mutiple, viewRatio * mutiple, -1f * currentMouseX * moveXRatio / (viewRatio * mutiple), -1f * currentMouseY * moveYRatio / (viewRatio * mutiple));
                    }


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

        private void BitmapToEImageBW8(Bitmap bitmap, ref EImageBW8 image)
        {
            EImageC24 EC24ImageTemp = new EImageC24();
            BitmapData bmpData;
            if (bitmap == null)
            {
                MessageBox.Show("相機錯誤。");
                return;
            }
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

            image.SetSize(EC24ImageTemp);
            EasyImage.Convert(EC24ImageTemp, image);

            EC24ImageTemp.Dispose();
        }

        private EImageC24 BitmapToEImageC24(ref Bitmap bitmap)
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

            //EBW8Image1.SetSize(EC24ImageTemp);
            //EasyImage.Convert(EC24ImageTemp, EBW8Image1);\

            return EC24ImageTemp;
        }

        private string UnwarpEBW8Image1()
        {
            EImageBW8 EBW8ImageUnwarp = new EImageBW8();
            EImageBW8 EBW8ImageTemp = new EImageBW8();


            if (EWorldShape1.CalibrationSucceeded() == false)
            {
                return "請先校正點圖。";
            }

            EBW8ImageUnwarp.SetSize(EBW8Image1);
            EBW8ImageTemp.SetSize(EBW8Image1);

            // 先 Invert，在 Unwarp，在 Invert 回來，這樣可以消除黑底。
            EasyImage.Oper(EArithmeticLogicOperation.Invert, EBW8Image1, EBW8ImageTemp);
            EWorldShape1.Unwarp(EBW8ImageTemp, EBW8ImageUnwarp);
            EasyImage.Oper(EArithmeticLogicOperation.Invert, EBW8ImageUnwarp, EBW8ImageTemp);

            EBW8ImageTemp.CopyTo(EBW8Image1);

            EBW8ImageUnwarp.Dispose();
            EBW8ImageTemp.Dispose();

            return OK;
        }


        // -----------------------MVSCamera-----------------------
        private void btn_MVSCamera_Click(object sender, EventArgs e)
        {
            // 如果設備還沒載入
            if (m_pMyCamera == null || !m_pMyCamera.MV_CC_IsDeviceConnected_NET())
            {
                try
                {

                    Search_Device(sender, e);
                    Open_Camera(sender, e);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            // 如果沒有開始攝影
            if (isStreaming == false)
            {
                int nRet;

                // ch:开始采集 | en:Start Grabbing
                nRet = m_pMyCamera.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    ShowErrorMsg("Trigger Fail!", nRet);
                    return;
                }

                btn_Camera.Text = "停止攝影";


                // ch:标志位置位true | en:Set position bit true
                isStreaming = true;


                // ch:显示 | en:Display
                nRet = m_pMyCamera.MV_CC_Display_NET(pictureBox1.Handle);

                // 把 m_pMyCamera 轉成 bitmap
                // 再把 bitmpa 轉成 EC24Image 再轉成 EBW8Image

                if (MyCamera.MV_OK != nRet)
                {
                    ShowErrorMsg("Display Fail！", nRet);
                }
            }
            else
            {
                MVS_Image_To_Bmp(sender, e);
                Stop_Streaming(sender, e);

                m_pMyCamera.MV_CC_CloseDevice_NET();
                m_pMyCamera.MV_CC_DestroyDevice_NET();

                viewRatio = CalcRatioWithPictureBox(pictureBox1, EBW8Image1.Width, EBW8Image1.Height);

                pictureBox1.Image = null;
                pictureBox1.Refresh();
                EBW8Image1.Draw(graphics, viewRatio);

                ProductDataReset();
            }

        }

        private void Search_Device(object sender, EventArgs e)
        {
            DeviceListAcq();
        }

        private void DeviceListAcq()
        {
            int nRet;
            // ch:创建设备列表 en:Create Device List
            System.GC.Collect();
            list_Of_Camera_Devices.Items.Clear();
            nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            if (0 != nRet)
            {
                ShowErrorMsg("Enumerate devices fail!", 0);
                return;
            }

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo.chUserDefinedName != "")
                    {
                        list_Of_Camera_Devices.Items.Add("GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        list_Of_Camera_Devices.Items.Add("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        list_Of_Camera_Devices.Items.Add("USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        list_Of_Camera_Devices.Items.Add("USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
            }

            // ch:选择第一项 | en:Select the first item
            if (m_pDeviceList.nDeviceNum != 0)
            {
                list_Of_Camera_Devices.SelectedIndex = 0;
            }
        }

        private void Open_Camera(object sender, EventArgs e)
        {
            if (m_pDeviceList.nDeviceNum == 0 || list_Of_Camera_Devices.SelectedIndex == -1)
            {
                ShowErrorMsg("No device, please select", 0);
                return;
            }
            int nRet = -1;

            // ch:获取选择的设备信息 | en:Get selected device information
            MyCamera.MV_CC_DEVICE_INFO device =
                (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[list_Of_Camera_Devices.SelectedIndex],
                                                              typeof(MyCamera.MV_CC_DEVICE_INFO));

            // ch:打开设备 | en:Open device
            if (null == m_pMyCamera)
            {
                m_pMyCamera = new MyCamera();
                if (null == m_pMyCamera)
                {
                    return;
                }
            }

            nRet = m_pMyCamera.MV_CC_CreateDevice_NET(ref device);
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }

            nRet = m_pMyCamera.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_pMyCamera.MV_CC_DestroyDevice_NET();
                ShowErrorMsg("Device open fail!", nRet);
                return;
            }

            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
            if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                int nPacketSize = m_pMyCamera.MV_CC_GetOptimalPacketSize_NET();
                if (nPacketSize > 0)
                {
                    nRet = m_pMyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                    if (nRet != MyCamera.MV_OK)
                    {
                        Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                }
            }

            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
            m_pMyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", 2);// ch:工作在连续模式 | en:Acquisition On Continuous Mode
            m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", 0);    // ch:连续模式 | en:Continuous

            // ch:控件操作 | en:Control operation

        }
        private void Stop_Streaming(object sender, EventArgs e)
        {
            int nRet = -1;
            // ch:停止采集 | en:Stop Grabbing
            nRet = m_pMyCamera.MV_CC_StopGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                ShowErrorMsg("Stop Grabbing Fail!", nRet);
            }

            // ch:标志位设为false | en:Set flag bit false
            isStreaming = false;

            btn_Camera.Text = "開始攝影";
        }

        private void MVS_Image_To_Bmp(object sender, EventArgs e)
        {
            int nRet;
            UInt32 nPayloadSize = 0;
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            nRet = m_pMyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("Get PayloadSize failed", nRet);
                return;
            }
            nPayloadSize = stParam.nCurValue;
            if (nPayloadSize > m_nBufSizeForDriver)
            {
                m_nBufSizeForDriver = nPayloadSize;
                m_pBufForDriver = new byte[m_nBufSizeForDriver];

                // ch:同时对保存图像的缓存做大小判断处理 | en:Determine the buffer size to save image
                // ch:BMP图片大小：width * height * 3 + 2048(预留BMP头大小) | en:BMP image size: width * height * 3 + 2048 (Reserved for BMP header)
                m_nBufSizeForSaveImage = m_nBufSizeForDriver * 3 + 2048;
                m_pBufForSaveImage = new byte[m_nBufSizeForSaveImage];
            }

            IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForDriver, 0);
            MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
            // ch:超时获取一帧，超时时间为1秒 | en:Get one frame timeout, timeout is 1 sec
            nRet = m_pMyCamera.MV_CC_GetOneFrameTimeout_NET(pData, m_nBufSizeForDriver, ref stFrameInfo, 1000);
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("No Data!", nRet);
                return;
            }

            MyCamera.MvGvspPixelType enDstPixelType;
            if (IsMonoData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
            }
            else if (IsColorData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
            }
            else
            {
                ShowErrorMsg("No such pixel type!", 0);
                return;
            }

            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
            MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
            stConverPixelParam.nWidth = stFrameInfo.nWidth;
            stConverPixelParam.nHeight = stFrameInfo.nHeight;
            stConverPixelParam.pSrcData = pData;
            stConverPixelParam.nSrcDataLen = stFrameInfo.nFrameLen;
            stConverPixelParam.enSrcPixelType = stFrameInfo.enPixelType;
            stConverPixelParam.enDstPixelType = enDstPixelType;
            stConverPixelParam.pDstBuffer = pImage;
            stConverPixelParam.nDstBufferSize = m_nBufSizeForSaveImage;
            nRet = m_pMyCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }

            if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
            {
                //************************Mono8 转 Bitmap*******************************
                Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 1, PixelFormat.Format8bppIndexed, pImage);

                ColorPalette cp = bmp.Palette;
                // init palette
                for (int i = 0; i < 256; i++)
                {
                    cp.Entries[i] = Color.FromArgb(i, i, i);
                }
                // set palette back
                bmp.Palette = cp;

                EC24Image1 = BitmapToEImageC24(ref bmp);
                EBW8Image1.SetSize(EC24Image1);
                EasyImage.Convert(EC24Image1, EBW8Image1);
            }
            else
            {
                //*********************RGB8 转 Bitmap**************************
                for (int i = 0; i < stFrameInfo.nHeight; i++)
                {
                    for (int j = 0; j < stFrameInfo.nWidth; j++)
                    {
                        byte chRed = m_pBufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3];
                        m_pBufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3] = m_pBufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2];
                        m_pBufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2] = chRed;
                    }
                }
                try
                {
                    Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pImage);

                    EC24Image1 = BitmapToEImageC24(ref bmp);
                    EBW8Image1.SetSize(EC24Image1);
                    EasyImage.Convert(EC24Image1, EBW8Image1);
                }
                catch
                {
                }
            }
        }


        private Boolean IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }

        /************************************************************************
         *  @fn     IsColorData()
         *  @brief  判断是否是彩色数据
         *  @param  enGvspPixelType         [IN]           像素格式
         *  @return 成功，返回0；错误，返回-1 
         ************************************************************************/
        private Boolean IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }

        private void ShowErrorMsg(string csMessage, int nErrorNum)
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + String.Format("{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case MyCamera.MV_E_HANDLE: errorMsg += " Error or invalid handle "; break;
                case MyCamera.MV_E_SUPPORT: errorMsg += " Not supported function "; break;
                case MyCamera.MV_E_BUFOVER: errorMsg += " Cache is full "; break;
                case MyCamera.MV_E_CALLORDER: errorMsg += " Function calling order error "; break;
                case MyCamera.MV_E_PARAMETER: errorMsg += " Incorrect parameter "; break;
                case MyCamera.MV_E_RESOURCE: errorMsg += " Applying resource failed "; break;
                case MyCamera.MV_E_NODATA: errorMsg += " No data "; break;
                case MyCamera.MV_E_PRECONDITION: errorMsg += " Precondition error, or running environment changed "; break;
                case MyCamera.MV_E_VERSION: errorMsg += " Version mismatches "; break;
                case MyCamera.MV_E_NOENOUGH_BUF: errorMsg += " Insufficient memory "; break;
                case MyCamera.MV_E_UNKNOW: errorMsg += " Unknown error "; break;
                case MyCamera.MV_E_GC_GENERIC: errorMsg += " General error "; break;
                case MyCamera.MV_E_GC_ACCESS: errorMsg += " Node accessing condition error "; break;
                case MyCamera.MV_E_ACCESS_DENIED: errorMsg += " No permission "; break;
                case MyCamera.MV_E_BUSY: errorMsg += " Device is busy, or network disconnected "; break;
                case MyCamera.MV_E_NETER: errorMsg += " Network error "; break;
            }

            MessageBox.Show(errorMsg, "PROMPT");
        }

        // -----------------------Adjust-----------------------
        //private void Learn()
        //{
        //    string RunningPath = Environment.CurrentDirectory;
        //    string StdImagePath = string.Format("{0}Resources\\PressItem_Whole.png", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\..\")));
        //    EBW8ImageLearn.Load(StdImagePath);

        //    EPatternFinder1 = new EPatternFinder();
        //    // 先學習不規則圖形
        //    // 可用於校正水平位置
        //    // Attach the roi to its parent
        //    EBW8Roi1.Attach(EBW8ImageLearn);
        //    EBW8Roi1.SetPlacement(400, 780, 690, 520);
        //    ERoi1Center = new Point(EBW8Roi1.OrgX + (EBW8Roi1.Width / 2), EBW8Roi1.OrgY + (EBW8Roi1.Height / 2));

        //    EPatternFinder1.Learn(EBW8Roi1);

        //    EPatternFinder1.AngleTolerance = 20.00f;
        //    EPatternFinder1.ScaleTolerance = 0.10f;


        //    EPatternFinder2 = new EPatternFinder();
        //    // 第二個標準點
        //    // 用於 鏡像 和 垂直翻轉
        //    EBW8Roi2.Attach(EBW8ImageLearn);
        //    EBW8Roi2.SetPlacement(2770, 750, 180, 440);
        //    EPatternFinder2.Learn(EBW8Roi2);

        //    // 由於前面都會校正一次，所以容忍值不用太高，速度會更快
        //    EPatternFinder2.AngleTolerance = 10.00f;
        //    EPatternFinder2.ScaleTolerance = 0.85f;
        //}

        private string Learn()
        {
            string RunningPath = Environment.CurrentDirectory;
            string StdImagePath = string.Format("{0}Resources\\Standard.png", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));

            if (File.Exists(StdImagePath) == false)
            {
                return "EBW8ImageLearn doesn't exsit.";
            }

            EBW8ImageLearn.Load(StdImagePath);

            // EPatternFinder1 Setting
            EPatternFinder1.AngleTolerance = 25.00f;
            EPatternFinder1.ScaleTolerance = 0.50f;

            // Create EBW8Roi1
            EBW8Roi1.Attach(EBW8ImageLearn);
            EBW8Roi1.SetPlacement(406, 1145, 880, 669);

            ERoi1Center.X = EBW8Roi1.OrgX + EBW8Roi1.Width / 2;
            ERoi1Center.Y = EBW8Roi1.OrgY + EBW8Roi1.Height / 2;

            EPatternFinder1.Learn(EBW8Roi1);

            return OK;
        }

        private string Adjust_Fixed()
        {
            // 位置校正 & 水平校正
            EPatternFinder1FoundPatterns = EPatternFinder1.Find(EBW8Image1); //找 ERoi1 的位置

            // 如果沒找到
            if (EPatternFinder1FoundPatterns[0].Score < 0.8)
            {
                EPatternFinder1FoundPatterns[0].Draw(graphics, viewRatio);
                return "找不到類似圖形，請確認圖像是否正確。";
            }
            else
            {
                //Console.WriteLine("找到圖形了。");
            }


            EImageBW8 EBW8ImageTemp = new EImageBW8();

            finder1CenterX = EPatternFinder1FoundPatterns[0].Center.X;
            finder1CenterY = EPatternFinder1FoundPatterns[0].Center.Y;
            adjustRatio = 1 / EPatternFinder1FoundPatterns[0].Scale;

            EBW8ImageTemp.SetSize(EBW8Image1);

            // 先 invert，再校正，再 invert 回來，把黑色的底圖變白色
            EasyImage.Oper(EArithmeticLogicOperation.Invert, EBW8Image1, EBW8ImageTemp);
            // 校正方式
            // 位置，比例，角度
            EasyImage.ScaleRotate(EBW8ImageTemp, finder1CenterX, finder1CenterY, ERoi1Center.X, ERoi1Center.Y, adjustRatio, adjustRatio, EPatternFinder1FoundPatterns[0].Angle, EBW8Image1, 0);
            EasyImage.Oper(EArithmeticLogicOperation.Invert, EBW8Image1, EBW8ImageTemp);

            EBW8ImageTemp.CopyTo(EBW8Image1);

            EBW8ImageTemp.Dispose();

            return OK;
        }

        // -------------------------------Method-------------------------------
        private void ListBoxAddObj(ListBox listBox, ObjectInfo obj)
        {
            string objIndex = obj.ElementIndex.ToString("000");
            string objCenterX = obj.CenterX.ToString("#.#");
            string objCenterY = obj.CenterY.ToString("#.#");

            listBox.Items.Add(objIndex + "(" + objCenterX + "," + objCenterY + ")");
        }

        private void ListBoxAddNG(ListBox listBox, ObjectInfo obj, string errorString)
        {
            string objIndex = obj.ElementIndex.ToString("000");

            listBox.Items.Add(objIndex + "(" + errorString + ")");
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

        private void ProductDataReset()
        {
            // previous product data clear
            coded1Selection.Clear();
            ObjectSetU.Clear();
            NGIndex.Clear();
            batchIndexes.Clear();

            rotateDegree = 0;

            // View Clear
            // Ratio
            adjustRatio = 0;
            viewRatio = 0;

            // Listbox clear
            listBox_Measure.Items.Clear();
            listBox_NG.Items.Clear();

            // Panel clear
            panel_Measure.Controls.Clear();
            panel_NG.Controls.Clear();
            panel_Standard.Controls.Clear();

            panelIndex = 0;
            panelNGIndex = 0;
            panelStandardIndex = 0;

            btn_Measure_Product.Enabled = false;
        }

        private ObjectInfo FindTheSameInObjectSetG(ObjectInfo objectTest)
        {
            ObjectInfo objectStandard;
            float widthThreshold = objectTest.width / 3;
            float heightThreshold = objectTest.height / 3;
            int j = 0;

            do
            {
                objectStandard = (ObjectInfo)ObjectSetG[j];
                if (
                     Math.Abs(objectTest.CenterX - objectStandard.CenterX) < widthThreshold &&
                     Math.Abs(objectTest.CenterY - objectStandard.CenterY) < heightThreshold
                   )
                {
                    break;
                }

                j++;

            } while (j < ObjectSetG.Count);

            if (j == ObjectSetG.Count)
            {
                return null;
            }

            ObjectSetInspect[j] = objectStandard;
            isFindTheSameList[j] = true;


            return objectStandard;
        }


        // -------------------------------Measure-------------------------------
        // -------------------------------Detect-------------------------------
        private string Detect(ref ECodedImage2 codedImage, ref EObjectSelection codedSelection, ref EImageBW8 image)
        {
            // 如果 EBW8Image1
            if (image.IsVoid)
            {
                return "圖片不能為空，請先載入圖片或相機截圖。";
            }

            // codedImage1Encoder 設定
            //codedImage1Encoder.GrayscaleSingleThresholdSegmenter.BlackLayerEncoded = false; //為初始設定
            //codedImage1Encoder.GrayscaleSingleThresholdSegmenter.WhiteLayerEncoded = true; //為初始設定
            //codedImage1Encoder.GrayscaleSingleThresholdSegmenter.Mode = EGrayscaleSingleThreshold.MinResidue; //為初始設定


            // codedImage1 圖層
            codedImage1Encoder.Encode(image, codedImage);

            // codedImage1ObjectSelection 設定
            codedSelection.Clear();
            codedSelection.FeretAngle = 0.00f;

            // codedImage1ObjectSelection 圖層
            codedSelection.AddObjects(codedImage);
            codedSelection.AttachedImage = image;

            // don't care area 條件
            codedSelection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, 20, ESingleThresholdMode.Less);
            codedSelection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, 150000, ESingleThresholdMode.Greater);

            return OK;

        }

        // -------------------------------ObjectSet-------------------------------

        // 清空 ObjectSet, listBox
        // 把 objectInfo 放進 ObjectSet
        // 把 objectInfo 放進 listBox
        private void BuildObjectSet(ArrayList ObjectSet, ListBox listBox)
        {
            // Build ObjectSet
            ObjectSet.Clear();

            listBox.Items.Clear();

            for (uint i = 0; i < coded1Selection.ElementCount; i++)
            {
                // Turn element to object
                ECodedElement element = coded1Selection.GetElement(i); //get element

                ObjectInfo objectInfo = ElementToObjectInfo(ref EBW8Image1, ref element, i); //element to shape

                ObjectSet.Add(objectInfo); //Add into ObjectSet

                ListBoxAddObj(listBox, objectInfo); //Add in ListBox

                element.Dispose();
            }
        }

        // checkResult = 0;
        // lengthStd = length
        private void SetObjectG()
        {
            for (int i = 0; i < ObjectSetG.Count; i++)
            {
                ObjectInfo objectInfo = (ObjectInfo)ObjectSetG[i];

                objectInfo.CheckResult = 0;
                objectInfo.widthStd = objectInfo.width;
                objectInfo.heightStd = objectInfo.height;
            }

        }

        internal int IsClickObject(ref ArrayList ObjectSet, float clickX, float clickY)
        {
            for (int index = 0; index < ObjectSet.Count; index++)
            {
                ObjectInfo objectInfo = (ObjectInfo)ObjectSet[index];

                float x_distance = Math.Abs(objectInfo.CenterX - clickX / EWorldShape1.XResolution);
                float y_distance = Math.Abs(objectInfo.CenterY - clickY / EWorldShape1.YResolution);

                if (x_distance < (objectInfo.width / 2) && y_distance < (objectInfo.height / 2))
                {
                    Console.WriteLine(objectInfo.Area);
                    return index;
                }
            }

            return -1;
        }

        // -------------------------------Inspect-------------------------------
        //internal void Inspect(decimal thresholdNG)
        //{
        //    // !!!!!!!!!!!!!!!!!! Check ObjectSetG
        //    ObjectShape shapeTest;
        //    ObjectShape shapeStandard;

        //    float sameShapeThreshold = 11;


        //    NGIndex.Clear();

        //    for (int i = 0; i < ObjectSetU.Count; i++)
        //    {
        //        shapeTest = (ObjectShape)ObjectSetU[i];
        //        int j = 0;
        //        // 看兩個 shape 位置是不是差不多，確認兩個可以做比對
        //        do
        //        {
        //            shapeStandard = (ObjectShape)ObjectSetG[j];
        //            if ((Math.Abs(shapeTest.centerX - shapeStandard.centerX) < sameShapeThreshold) &&
        //                (Math.Abs(shapeTest.centerY - shapeStandard.centerY) < sameShapeThreshold)
        //                )
        //            {
        //                break;
        //            }

        //            j++;

        //        } while (j < ObjectSetG.Count);

        //        if (j >= ObjectSetG.Count)
        //        {
        //            shapeTest.checkResult = 1;
        //            NGIndex.Add(i);
        //            continue;
        //        }

        //        // 把 standard 設置給它 (可以不用)
        //        // 比對兩個是不是同樣的圖形 (暫時不用)
        //        if (shapeTest.GetType() != shapeStandard.GetType())
        //        {
        //            Console.WriteLine("形狀不同"); // 長方形和正方形的形狀判定
        //            //shapeTest.checkResult = 1;
        //            //continue;
        //        }

        //        // 相減儲存在誤差
        //        shapeTest.SaveInspectInfo(shapeStandard);
        //        // 比對誤差是否在 Threshold 裡面
        //        if (shapeTest.Inspect(thresholdNG))
        //        {
        //            shapeTest.checkResult = 0;
        //        }
        //        else
        //        {
        //            shapeTest.checkResult = 1;

        //            NGIndex.Add(i);
        //        }
        //    }


        //}



        private string Inspect(float thresholdNG)
        {
            string errorMessage = "";

            ObjectInfo objectTest;
            ObjectInfo objectStandard;
            ObjectSetInspect = new ObjectInfo[ObjectSetG.Count];
            isFindTheSameList = new bool[ObjectSetG.Count];

            if (ObjectSetG.Count <= 0)
            {
                errorMessage = "請先測量標準物。";
                return errorMessage;
            }

            NGIndex.Clear();
            listBox_NG.Items.Clear();

            for (int i = 0; i < ObjectSetU.Count; i++)
            {
                objectTest = (ObjectInfo)ObjectSetU[i];

                // 看兩個 shape 位置是不是差不多，確認兩個可以做比對
                objectStandard = FindTheSameInObjectSetG(objectTest);

                if (objectStandard == null)
                {
                    objectTest.CheckResult = 1;

                    noStdNGList.Add((uint)i);
                    ListBoxAddNG(listBox_NG, objectTest, "No Standard");

                    NGIndex.Add(i);

                    continue;
                }

                // 比對兩個是不是同樣的圖形 (暫時不用)

                // 把 standard 設置給它
                objectTest.widthStd = objectStandard.widthStd;
                objectTest.heightStd = objectStandard.heightStd;

                // 相減儲存在誤差
                objectTest.widthError = Math.Abs(objectTest.width - objectTest.widthStd);
                objectTest.heightError = Math.Abs(objectTest.height - objectTest.heightStd);

                // 比對誤差是否在 Threshold 裡面
                if (objectTest.widthError < (float)thresholdNG && objectTest.heightError < (float)thresholdNG)
                {
                    objectTest.CheckResult = 0;
                }
                else
                {
                    objectTest.CheckResult = 1;

                    //ListBoxAddObj(listBox_NG, (ObjectInfo)ObjectSetU[i]);
                    ListBoxAddNG(listBox_NG, (ObjectInfo)ObjectSetU[i], "Size Wrong");


                    NGIndex.Add(i);
                    sizeErrorList.Add((uint)i);

                    continue;
                }

                // 比對 Area
                if (objectTest.Area > objectStandard.Area * 0.95)
                {
                    objectTest.CheckResult = 0;
                }
                else
                {
                    objectTest.CheckResult = 1;

                    //ListBoxAddObj(listBox_NG, (ObjectInfo)ObjectSetU[i]);
                    ListBoxAddNG(listBox_NG, (ObjectInfo)ObjectSetU[i], "Area Wrong");


                    NGIndex.Add(i);
                    areaErrorList.Add((uint)i);

                    continue;
                }


            }

            return errorMessage;
        }

        // 看兩個 shape 位置是不是差不多，確認兩個可以做比對

        // -------------------------------Shape-------------------------------
        internal string AutoCalibration(ref EImageBW8 image)
        {
            EImageBW8 calibrationImage = new EImageBW8(image);

            EWorldShape1.SetSensorSize(calibrationImage.Width, calibrationImage.Height);
            EWorldShape1.AutoCalibrateDotGrid(calibrationImage, calibrationX, calibrationY, false);

            if (EWorldShape1.CalibrationSucceeded())
            {
                //MessageBox.Show("校正成功", "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "校正成功";
            }
            else
            {
                //MessageBox.Show("校正失敗", "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "校正失敗";
            }
        }

        private ObjectInfo ElementToObjectInfo(ref EImageBW8 image, ref ECodedElement element, uint elementIndex)
        {
            ObjectInfo objectInfo = new ObjectInfo();

            // 給值
            objectInfo.CenterX = element.BoundingBoxCenterX / EWorldShape1.XResolution;
            objectInfo.CenterY = element.BoundingBoxCenterY / EWorldShape1.YResolution;
            objectInfo.width = element.BoundingBoxWidth / EWorldShape1.XResolution;
            objectInfo.height = element.BoundingBoxHeight / EWorldShape1.YResolution;
            objectInfo.CheckResult = -1;
            objectInfo.ElementIndex = elementIndex;
            objectInfo.Area = element.Area;

            // Set Shape Name
            //這裡需要先使用Raio判斷長方形或圓形(正方形)，亦或者分別量測，有結果的才算是有那個形狀
            if (element.BoundingBoxWidth / element.BoundingBoxHeight >= 0.95 && element.BoundingBoxWidth / element.BoundingBoxHeight <= 1.05) //正方形或圓形
            {
                //必須先量測正方形，再來量測圓形，不然會誤判
                //嘗試看看是否為正方形
                ERectangle squre = MeasureRect(ref image, ref element);

                if (squre != null)
                {
                    objectInfo.ShapeName = "rectangle";
                    return objectInfo;
                }

                ECircle circle = MeasureCircle(ref image, ref element);

                if (circle != null)
                {
                    objectInfo.ShapeName = "circle";
                    return objectInfo;
                }
                else
                {
                    objectInfo.ShapeName = "special";
                    return objectInfo;
                }
            }
            else
            {
                ERectangle rectangle = MeasureRect(ref image, ref element);

                if (rectangle != null)
                {
                    objectInfo.ShapeName = "rectangle";
                    return objectInfo;
                }

                objectInfo.ShapeName = "special";
                return objectInfo;
            }
        }

        private ERectangle MeasureRect(ref EImageBW8 image, ref ECodedElement element)
        {
            ERectangleGauge ERectangleGauge1 = new ERectangleGauge();
            EWorldShape EWorldShapeTemp = new EWorldShape();

            EWorldShapeTemp.SetSensorSize(image.Width, image.Height);
            ERectangleGauge1.Attach(EWorldShapeTemp);
            ERectangleGauge1.Center = element.BoundingBoxCenter;
            if (element.BoundingBoxWidth > element.BoundingBoxHeight) //容忍值必須以寬高數值小的為基準，不然涵蓋兩個邊
            {
                ERectangleGauge1.Tolerance = element.BoundingBoxHeight / 4;
            }
            else
            {
                ERectangleGauge1.Tolerance = element.BoundingBoxWidth / 4;
            }
            ERectangleGauge1.TransitionType = ETransitionType.Bw; //由外往裡面看
            ERectangleGauge1.SamplingStep = 1;
            ERectangleGauge1.SetSize(element.BoundingBoxWidth, element.BoundingBoxHeight);
            ERectangleGauge1.Measure(image);

            if (ERectangleGauge1.NumValidSamples < ERectangleGauge1.NumSamples * 0.9) //防呆機制，萬一給的不是長方形
            {
                return null;
            }
            else
            {
                return ERectangleGauge1.MeasuredRectangle;
            }
        }

        private ECircle MeasureCircle(ref EImageBW8 image, ref ECodedElement element)
        {
            ECircleGauge ECircleGauge = new ECircleGauge();
            EWorldShape EWorldShapeTemp = new EWorldShape();

            //先用圓形量測
            EWorldShapeTemp.SetSensorSize(image.Width, image.Height);
            ECircleGauge.Attach(EWorldShapeTemp);
            ECircleGauge.Center = element.BoundingBoxCenter;
            ECircleGauge.Diameter = element.BoundingBoxWidth;
            ECircleGauge.TransitionType = ETransitionType.Wb; //是由圓心往外看，跟Rectangle不同
            ECircleGauge.Tolerance = element.BoundingBoxWidth / 5;
            ECircleGauge.SamplingStep = 1; //每個點都要檢查
            ECircleGauge.Measure(image);
            if (ECircleGauge.NumValidSamples < ECircleGauge.NumSamples * 0.9) //防呆機制，萬一給的不是圓形
            {
                return null;
            }
            else
            {
                return ECircleGauge.MeasuredCircle;
            }

        }

        private EPoint MeasureSpecial(ref EImageBW8 image, ref ECodedElement element)
        {
            EPointGauge EPointGauge = new EPointGauge();

            //特殊形狀，只有量測精準寬與高
            //假設條件，圖案必須是上下左右對稱，有角度偏差會進行修正
            //量測方式: 以BoundingCenter為中心，修正角度後進行十字線，兩條PointGauge進行量測
            double tmpW = 0, tmpH = 0;
            EWorldShape1.SetSensorSize(image.Width, image.Height);
            EPointGauge.Attach(EWorldShape1); //將LineGauge繫結到世界座標系統
            EPointGauge.TransitionType = ETransitionType.BwOrWb; //設定邊緣轉換類型，兩端剛好有Bw與Wb
            EPointGauge.SetCenterXY(element.BoundingBoxCenterX, element.BoundingBoxCenterY);
            EPointGauge.Tolerance = element.BoundingBoxWidth / 2 + 10;
            EPointGauge.ToleranceAngle = element.MinimumEnclosingRectangleAngle;
            EPointGauge.Angle = element.MinimumEnclosingRectangleAngle;//要看那一個角度量出來比較準
            EPointGauge.Thickness = 3; //增加厚度，避免小雜訊
                                       //EPointGauge1.Angle = element.EllipseAngle;
            EPointGauge.Measure(image);
            //EPointGauge.SetZoom(scalingRatio);
            //EPointGauge.Draw(g, EDrawingMode.Actual, true);
            //檢查有沒有取到兩個點
            if (EPointGauge.NumMeasuredPoints != 2) //如果量測到的point不到兩個，表示沒有量測到邊緣
            {
                return null;
            }
            else
            {
                EPoint tmpP1, tmpP2;
                tmpP1 = EPointGauge.GetMeasuredPoint(0);
                tmpP2 = EPointGauge.GetMeasuredPoint(1);
                tmpW = Math.Sqrt(Math.Pow(tmpP1.X - tmpP2.X, 2) + Math.Pow(tmpP1.Y - tmpP2.Y, 2));
                //量測另外一個垂直方向
                //EWorldShape1.SetSensorSize(EBW8Image1.Width, EBW8Image1.Height);
                //EPointGauge1.Attach(EWorldShape1); //將LineGauge繫結到世界座標系統
                //EPointGauge1.TransitionType = ETransitionType.BwOrWb; //設定邊緣轉換類型，兩端剛好有Bw與Wb
                //EPointGauge1.SetCenterXY(element.BoundingBoxCenterX, element.BoundingBoxCenterY);
                EPointGauge.Tolerance = element.BoundingBoxHeight / 2 + 10;
                EPointGauge.ToleranceAngle = element.MinimumEnclosingRectangleAngle + 270;
                EPointGauge.Angle = element.MinimumEnclosingRectangleAngle + 270;//要看那一個角度量出來比較準，PS: 加90度居然無法量測，好怪
                                                                                 //EPointGauge1.Angle = element.EllipseAngle;
                EPointGauge.Measure(image);

                //EPointGauge.SetZoom(scalingRatio);
                //EPointGauge.Draw(g, EDrawingMode.Actual, true);
                //檢查有沒有取到兩個點
                if (EPointGauge.NumMeasuredPoints != 2) //如果量測到的point不到兩個，表示沒有量測到邊緣
                {
                    return null;
                }
                else
                {
                    tmpP1 = EPointGauge.GetMeasuredPoint(0);
                    tmpP2 = EPointGauge.GetMeasuredPoint(1);
                    tmpH = Math.Sqrt(Math.Pow(tmpP1.X - tmpP2.X, 2) + Math.Pow(tmpP1.Y - tmpP2.Y, 2));
                    return new EPoint((float)tmpW, (float)tmpH);
                }
            }
        }

        // -------------------------------View-------------------------------
        private string DrawEBW8Image(ref EImageBW8 image)
        {
            if (image == null || image.IsVoid)
            {
                return "Void Image cann't be drawn.";
            }

            pictureBox1.Image = null;
            pictureBox1.Refresh();

            viewRatio = CalcRatioWithPictureBox(pictureBox1, image.Width, image.Height);

            EBW8ImageView.SetSize(pictureBox1.Width, pictureBox1.Height);
            EBW8ImageTemp.SetSize(image);

            EasyImage.Oper(EArithmeticLogicOperation.Invert, image, EBW8ImageTemp);
            EasyImage.ScaleRotate(EBW8ImageTemp, 0, 0, 0, 0, viewRatio, viewRatio, 0, EBW8ImageView, 0);
            EBW8ImageTemp.SetSize(EBW8ImageView);
            EasyImage.Oper(EArithmeticLogicOperation.Invert, EBW8ImageView, EBW8ImageTemp);

            EasyImage.Copy(EBW8ImageTemp, EBW8ImageView);

            EBW8ImageView.Draw(graphics);
            //image.Draw(graphics, viewRatio);

            return OK;
        }

        private void DrawElement(ref ECodedElement element)
        {
            string message = Detect(ref codedImageView, ref codedViewSelection, ref EBW8ImageView);

            ECodedElement elementView;
            float centerX = element.BoundingBoxCenterX;
            float centerY = element.BoundingBoxCenterY;
            float centerViewX;
            float centerViewY;
            float widthThreshold;
            float heightThreshold;
            float deltaCenterX;
            float deltaCenterY;

            bool isFound = false;

            for (uint i = 0; i < codedViewSelection.ElementCount; i++)
            {
                elementView = codedViewSelection.GetElement(i);
                centerViewX = elementView.BoundingBoxCenterX;
                centerViewY = elementView.BoundingBoxCenterY;
                widthThreshold = elementView.BoundingBoxWidth / 2;
                heightThreshold = elementView.BoundingBoxHeight / 2;

                deltaCenterX = Math.Abs(centerX * viewRatio - centerViewX);
                deltaCenterY = Math.Abs(centerY * viewRatio - centerViewY);

                if (deltaCenterX < widthThreshold && deltaCenterY < heightThreshold)
                {
                    codedImageView.Draw(graphics, new ERGBColor(0, 0, 255), elementView);

                    elementView.Dispose();
                    isFound = true;
                    break;
                }

                elementView.Dispose();
            }

            if (isFound == false)
            {
                MessageBox.Show("無法顯示物件");
            }

            panelIndex = 0;
            panelNGIndex = 0;
            panelStandardIndex = 0;
        }

        private void DrawNG()
        {
            DrawEBW8Image(ref EBW8Image1);

            DrawNGNoStd();
            DrawNGMissing();
            DrawNGSizeError();
            DrawNGAreaError();
        }

        private void DrawNGNoStd()
        {

        }

        private void DrawNGMissing()
        {
            missingList = new List<uint>();
            for (int index = 0; index < isFindTheSameList.Count(); index++)
            {
                if (isFindTheSameList[index] == false)
                {
                    missingList.Add((uint)index);
                }
            }

            if (missingList == null)
            {
                return;
            }
            else
            {
                foreach (var index in missingList)
                {
                    ECodedElement element = codedStdSelection.GetElement(index);
                    ERGBColor color = new ERGBColor(255, 20, 147);

                    codedImageStd.Draw(graphics, color, element, viewRatio);

                    element.Dispose();
                }
            }
        }

        private void DrawNGAreaError()
        {
            if (areaErrorList == null)
            {
                return;
            }
            else
            {
                foreach (var index in areaErrorList)
                {
                    ECodedElement element = coded1Selection.GetElement(index);
                    ERGBColor color = new ERGBColor(255, 140, 0);

                    DrawNGElement(ref element, color);

                    element.Dispose();
                }
            }
        }

        private void DrawNGSizeError()
        {
            if (sizeErrorList == null)
            {
                return;
            }
            else
            {
                foreach (var index in sizeErrorList)
                {
                    ECodedElement element = coded1Selection.GetElement(index);
                    ERGBColor color = new ERGBColor(255, 0, 0);

                    DrawNGElement(ref element, color);

                    element.Dispose();

                }
            }
        }


        private void DrawNGElement(ref ECodedElement element, ERGBColor color)
        {
            string message = Detect(ref codedImageView, ref codedViewSelection, ref EBW8ImageView);

            ECodedElement elementView;
            float centerX = element.BoundingBoxCenterX;
            float centerY = element.BoundingBoxCenterY;
            float centerViewX;
            float centerViewY;
            float widthThreshold;
            float heightThreshold;
            float deltaCenterX;
            float deltaCenterY;

            bool isFound = false;

            for (uint i = 0; i < codedViewSelection.ElementCount; i++)
            {
                elementView = codedViewSelection.GetElement(i);
                centerViewX = elementView.BoundingBoxCenterX;
                centerViewY = elementView.BoundingBoxCenterY;
                widthThreshold = elementView.BoundingBoxWidth / 2;
                heightThreshold = elementView.BoundingBoxHeight / 2;

                deltaCenterX = Math.Abs(centerX * viewRatio - centerViewX);
                deltaCenterY = Math.Abs(centerY * viewRatio - centerViewY);

                if (deltaCenterX < widthThreshold && deltaCenterY < heightThreshold)
                {
                    codedImageView.Draw(graphics, color, elementView);

                    elementView.Dispose();
                    isFound = true;
                    break;
                }

                elementView.Dispose();
            }

            if (isFound == false)
            {
                MessageBox.Show("無法顯示物件");
            }

            panelIndex = 0;
            panelNGIndex = 0;
            panelStandardIndex = 0;
        }

        private void RenderShapeInfo(int index, ArrayList ObjectSet)
        {
            ObjectInfo objectInfo = (ObjectInfo)ObjectSet[index];

            panel_Measure.Controls.Clear();

            switch (objectInfo.ShapeName)
            {
                case "square":
                    AddItemInPanelMeasure("寬", objectInfo.width);
                    AddItemInPanelMeasure("高", objectInfo.height);
                    break;
                case "rectangle":
                    AddItemInPanelMeasure("寬", objectInfo.width);
                    AddItemInPanelMeasure("高", objectInfo.height);
                    break;
                case "circle":
                    AddItemInPanelMeasure("半徑", objectInfo.width);
                    break;
                case "special":
                    AddItemInPanelMeasure("寬", objectInfo.width);
                    AddItemInPanelMeasure("高", objectInfo.height);
                    break;
            }

        }

        private void RenderShapeErrorInfo(int index, ArrayList ObjectSet)
        {
            ObjectInfo objectInfo = (ObjectInfo)ObjectSet[index];

            panel_NG.Controls.Clear();

            switch (objectInfo.ShapeName)
            {
                case "square":
                    AddItemInPanelNG("寬誤差", objectInfo.widthError);
                    AddItemInPanelNG("高誤差", objectInfo.heightError);
                    break;
                case "rectangle":
                    AddItemInPanelNG("寬誤差", objectInfo.widthError);
                    AddItemInPanelNG("高誤差", objectInfo.heightError);
                    break;
                case "circle":
                    AddItemInPanelNG("半徑誤差", objectInfo.widthError);
                    break;
                case "special":
                    AddItemInPanelNG("寬誤差", objectInfo.widthError);
                    AddItemInPanelNG("高誤差", objectInfo.heightError);
                    break;
            }

        }

        private void RenderStandard(int index, ArrayList ObjectSet)
        {
            ObjectInfo objectInfo = (ObjectInfo)ObjectSet[index];

            panel_Standard.Controls.Clear();

            switch (objectInfo.ShapeName)
            {
                case "square":
                    AddItemInPanelStandard("標準寬", objectInfo.widthStd);
                    AddItemInPanelStandard("標準高", objectInfo.heightStd);
                    break;
                case "rectangle":
                    AddItemInPanelStandard("標準寬", objectInfo.widthStd);
                    AddItemInPanelStandard("標準高", objectInfo.heightStd);
                    break;
                case "circle":
                    AddItemInPanelStandard("標準半徑", objectInfo.widthStd);
                    break;
                case "special":
                    AddItemInPanelStandard("標準寬", objectInfo.widthStd);
                    AddItemInPanelStandard("標準高", objectInfo.heightStd);
                    break;
            }


        }

        private void AddItemInPanelMeasure(string labelText, float value)
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(5, 10 + panelIndex * 30);
            label_Title.Width = labelText.Length * (int)Math.Round((double)Font.Size * 2) + 3;

            Label label_Value = new Label();
            decimal number = (decimal)Math.Round(value, 2);
            label_Value.Text = number.ToString() + " mm";
            label_Value.Location = new Point(5 + label_Title.Width + 10, 10 + panelIndex * 30);
            label_Value.BackColor = Color.FromArgb(255, 224, 192);
            label_Value.Width = 80;

            panel_Measure.Controls.Add(label_Title);
            panel_Measure.Controls.Add(label_Value);
            panelIndex++;
        }

        private void AddItemInPanelNG(string labelText, float value)
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(5, 10 + panelNGIndex * 30);
            label_Title.Width = labelText.Length * (int)Math.Round((double)Font.Size * 2) + 3;

            Label label_Value = new Label();
            decimal number = (decimal)Math.Round(value, 2);
            label_Value.Text = number.ToString() + " mm";
            label_Value.Location = new Point(5 + label_Title.Width + 10, 10 + panelNGIndex * 30);
            label_Value.BackColor = Color.FromArgb(255, 224, 192);
            label_Value.Width = 80;

            panel_NG.Controls.Add(label_Title);
            panel_NG.Controls.Add(label_Value);
            panelNGIndex++;
        }

        private void AddItemInPanelStandard(string labelText, float value)
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(5, 10 + panelStandardIndex * 30);
            label_Title.Width = labelText.Length * (int)Math.Round((double)Font.Size * 2) + 3;

            NumericUpDown num_Standard = new NumericUpDown();
            num_Standard.DecimalPlaces = 2;
            num_Standard.Increment = 0.1M;
            num_Standard.Maximum = 5000;
            num_Standard.Value = (decimal)value;

            num_Standard.Location = new Point(5 + label_Title.Width + 10, 10 + panelStandardIndex * 30);
            num_Standard.Width = 80;

            Label label_MM = new Label();
            label_MM.Text = "mm";
            label_MM.Location = new Point(5 + label_Title.Width + 10 + num_Standard.Width + 10, 10 + panelStandardIndex * 30);
            label_MM.Width = labelText.Length * (int)Math.Round((double)Font.Size * 2) + 3;


            panel_Standard.Controls.Add(label_Title);
            panel_Standard.Controls.Add(num_Standard);
            panel_Standard.Controls.Add(label_MM);

            panelStandardIndex++;

        }

        private void ImageRotate(object sender, EventArgs e)
        {
            // 把選項打勾
            ToolStripMenuItem activeItem = sender as ToolStripMenuItem;

            if (activeItem != imageTranseformMenuItem && imageTranseformMenuItem != null) imageTranseformMenuItem.Checked = false;
            activeItem.Checked = true;

            imageTranseformMenuItem = activeItem.Checked ? activeItem : null;

            // 旋轉圖片
            int selectDegree = int.Parse(activeItem.Tag.ToString());
            EImageBW8 EBW8Imagetemp = new EImageBW8();

            switch (Math.Abs(selectDegree - rotateDegree))
            {
                case 0:
                case 180:

                    EBW8Imagetemp.SetSize(EBW8Image1.Width, EBW8Image1.Height);

                    EasyImage.ScaleRotate(EBW8Image1, EBW8Image1Center.X, EBW8Image1Center.Y, EBW8Image1Center.X, EBW8Image1Center.Y, 1, 1, selectDegree - rotateDegree, EBW8Imagetemp, 0);

                    EasyImage.Copy(EBW8Imagetemp, EBW8Image1);
                    EBW8Imagetemp.Dispose();

                    rotateDegree = selectDegree;

                    DrawEBW8Image(ref EBW8Image1);
                    break;
                case 90:
                case 270:
                    EBW8Imagetemp.SetSize(EBW8Image1.Height, EBW8Image1.Width);

                    EasyImage.ScaleRotate(EBW8Image1, EBW8Image1Center.X, EBW8Image1Center.Y, EBW8Image1Center.Y, EBW8Image1Center.X, 1, 1, selectDegree - rotateDegree, EBW8Imagetemp, 0);

                    EBW8Image1.SetSize(EBW8Imagetemp);
                    EasyImage.Copy(EBW8Imagetemp, EBW8Image1);
                    EBW8Imagetemp.Dispose();

                    rotateDegree = selectDegree;
                    DrawEBW8Image(ref EBW8Image1);

                    break;
            }
        }

        private void image_Flip_Horizontal_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EBW8ImageTemp.SetSize(EBW8Image1.Width, EBW8Image1.Height);

            EasyImage.ScaleRotate(EBW8Image1, EBW8Image1Center.X, EBW8Image1Center.Y, EBW8Image1Center.X, EBW8Image1Center.Y, -1.0f, 1.0f, 0, EBW8ImageTemp, 0);
            EasyImage.Copy(EBW8ImageTemp, EBW8Image1);

            DrawEBW8Image(ref EBW8Image1);
        }

        private void image_Flip_Verticle_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EBW8ImageTemp.SetSize(EBW8Image1.Width, EBW8Image1.Height);

            EasyImage.ScaleRotate(EBW8Image1, EBW8Image1Center.X, EBW8Image1Center.Y, EBW8Image1Center.X, EBW8Image1Center.Y, 1.0f, -1.0f, 0, EBW8ImageTemp, 0);
            EasyImage.Copy(EBW8ImageTemp, EBW8Image1);

            DrawEBW8Image(ref EBW8Image1);

        }




        // -------------------------------Clean-------------------------------

        private void Calibration(ref EImageBW8 dotGridImage)
        {
            EWorldShape1.SetSensorSize(dotGridImage.Width, dotGridImage.Height);
            EWorldShape1.AutoCalibrateDotGrid(dotGridImage, calibrationX, calibrationY, false);
        }

        private string GetCapture()
        {
            // 關閉攝影
            // 建立 capture
            // 確認相機運行擷取圖片
            // bmpToEImage
            // Unwarp
            // 重設產品
            // 圖片旋轉
            // 畫出圖片
            // 介面變動

            if (isStreaming)
            {
                EmguCV_Camera();
            }

            if (capture == null)
            {
                capture = new VideoCapture(0, VideoCapture.API.DShow);
                capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 4000);
                capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 3000);
            }

            if (capture.IsOpened)
            {
                Mat m = new Mat();
                try
                {
                    capture.Retrieve(m);
                    capture.Retrieve(m);
                    bmp = m.ToBitmap(); //不能使用 new Bitmap(m.Bitmap)
                }
                catch { }
            }
            else
            {
                capture = null;
                return "無法連接相機。";
            }

            BitmapToEImageBW8(bmp, ref EBW8Image1);

            UnwarpEBW8Image1();

            ProductDataReset();

            ImageRotate(imageTranseformMenuItem, new EventArgs());

            DrawEBW8Image(ref EBW8Image1);

            pictureClickView();

            return OK;
        }

        private void MeasureStdClickView()
        {
            btn_Measure_Product.Enabled = true;
            btn_Batch_Search.Visible = true;
            btn_Batch_Setting.Visible = true;
        }

        private void MeasureProductClickView()
        {
            btn_Batch_Search.Visible = false;
            btn_Batch_Setting.Visible = false;
        }


        private void OKView()
        {
            ControlPaint.DrawBorder(graphics, pictureBox1.ClientRectangle
                , Color.LightGreen, 10, ButtonBorderStyle.Solid
                , Color.LightGreen, 10, ButtonBorderStyle.Solid
                , Color.LightGreen, 10, ButtonBorderStyle.Solid
                , Color.LightGreen, 10, ButtonBorderStyle.Solid
            );
        }
        private void FormInitialize()
        {
            graphics = pictureBox1.CreateGraphics();
            imageTranseformMenuItem = image_Rotate_0_toolStripMenuItem;
            pictureBox1.MouseWheel += pictureBox_Mouse_Wheel;
        }

        private void pictureClickView()
        {
            btn_Measure_Standard.Enabled = true;

            if (ObjectSetG == null)
            {
                btn_Measure_Product.Enabled = false;
            }
            else
            {
                btn_Measure_Product.Enabled = true;
            }

            btn_Batch_Search.Visible = false;
            btn_Batch_Setting.Visible = false;
        }


    }
}
