using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Aruco;
using Emgu.CV.DepthAI;
using Emgu.CV.Flann;
using Emgu.CV.Reg;
using Euresys.Open_eVision_22_08;
using static System.Net.Mime.MediaTypeNames;
using MvCamCtrl.NET;

namespace cyut_Auo_Component_Measurer
{
    // c_measure.ObjectSetG: Object Set Golden，標準
    // c_measure.ObjectSetU: Object Set Unknown，待測物
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
        EImageBW8 EBW8Image1 = new EImageBW8();

        bool isGolden;
        // --------------------------Instance--------------------------
        Control c_control;
        Measure c_measure;
        ShapeOperator c_shape;

        // 要開給 FormDotGrid 讓它傳上父輩
        public int x = 5;
        public int y = 5;

        // --------------------------View--------------------------
        float scalingRatio;
        Graphics graphics;

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
            errorMessage = c_control.InitializeSetting(ref EBW8Image1, c_measure.ObjectSetG, ref c_shape.EBW8ImageDotGrid, c_shape.CalibrationX, c_shape.CalibrationY);

            c_shape.CalibrationX = x;
            c_shape.CalibrationY = y;

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

        // --------------------------Menu--------------------------
        private void Menu_Save_Setting_Click(object sender, EventArgs e)
        {
            string errorMessage;
            errorMessage = c_control.MenuSaveSetting(ref EBW8Image1, c_measure.ObjectSetG, ref c_shape.EBW8ImageDotGrid, c_shape.CalibrationX, c_shape.CalibrationY);

            if (errorMessage != c_control.OK)
                MessageBox.Show(errorMessage);
        }

        private void Menu_Load_Setting_Click(object sender, EventArgs e)
        {
            c_control.MenuLoadSetting(ref EBW8Image1, c_measure.ObjectSetG, ref c_shape.EBW8ImageDotGrid, c_shape.CalibrationX, c_shape.CalibrationY);
            c_shape.CalibrationX = x;
            c_shape.CalibrationY = y;
        }

        private void dotGridToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // 開相機
            if (isStreaming == false)
            {
                btn_MVSCamera_Click(sender, e);
            }

            // 設定 x, y
            x = c_shape.CalibrationX;
            y = c_shape.CalibrationY;

            Form_Dot_Grid f2 = new Form_Dot_Grid(x, y);
            f2.ShowDialog(this);

            btn_MVSCamera_Click(sender, e);

            c_shape.EBW8ImageDotGrid.SetSize(EBW8Image1);
            EasyImage.Copy(EBW8Image1, c_shape.EBW8ImageDotGrid);

            c_shape.AutoCalibration(ref c_shape.EBW8ImageDotGrid, x, y);
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

            DrawEBW8Image1();
        }

        private void btn_Camera_Click(object sender, EventArgs e)
        {

        }

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

            DrawEBW8Image1();

            // 畫出 finder
            EPatternFinder1FoundPatterns = EPatternFinder1.Find(EBW8Image1); //找 ERoi1 的位置
            EPatternFinder2FoundPatterns = EPatternFinder2.Find(EBW8Image1); //找 ERoi2 的位置
            EPatternFinder1FoundPatterns[0].Draw(graphics, scalingRatio);
            EPatternFinder2FoundPatterns[0].Draw(graphics, scalingRatio);
        }

        // -------------------------------Measure-------------------------------
        private void btn_Measure_Standard_Click(object sender, EventArgs e)
        {
            DrawEBW8Image1();

            c_measure.Detect(ref EBW8Image1);

            // Build ObjectSet
            c_measure.ObjectSetG.Clear();
            
            for (uint i = 0; i < c_measure.codedSelection.ElementCount; i++)
            {
                ECodedElement element = c_measure.codedSelection.GetElement(i); //get element

                ObjectShape shape = c_shape.ShapeDetermine(ref element, i); //element to shape

                c_measure.ObjectSetG.Add(shape); //Add into ObjectSet

                ListBoxAddObj(listBox_Measure, shape); //Add in ListBox

                element.Dispose();
            }

            c_measure.SetObjectG();

            isGolden = true;
        }

        private void btn_Measure_Product_Click(object sender, EventArgs e)
        {
            DrawEBW8Image1();

            c_measure.Detect(ref EBW8Image1);

            listBox_Measure.Items.Clear();

            // Build ObjectSet
            c_measure.ObjectSetU.Clear();

            for (uint i = 0; i < c_measure.codedSelection.ElementCount; i++)
            {
                ECodedElement element = c_measure.codedSelection.GetElement(i); //get element

                ObjectShape shape = c_shape.ShapeDetermine(ref element, i); //element to shape

                c_measure.ObjectSetU.Add(shape); //Add into ObjectSet


                ListBoxAddObj(listBox_Measure, shape); //Add in ListBox

                element.Dispose();
            }

            // Inspect
            c_measure.Inspect(num_Threshold_NG.Value);

            // View
            listBox_NG.Items.Clear();

            foreach (var index in c_measure.GetNGIndex)
            {
                ECodedElement element = c_measure.codedSelection.GetElement((uint)index);

                DrawNGElement(ref element);

                element.Dispose();


                ObjectShape shape = (ObjectShape)c_measure.ObjectSetU[index];

                ListBoxAddObj(listBox_NG, shape);
            }

            isGolden = false;
        }

        // -------------------------------Batch-------------------------------

        private void btn_Batch_Search_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox_Measure.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            ObjectShape shape = (ObjectShape)c_measure.ObjectSetG[selectedIndex];
            string selectedShape = shape.shapeName;

            batchIndexes.Clear();

            for (int i = 0; i < c_measure.ObjectSetG.Count; i++)
            {
                shape = (ObjectShape)c_measure.ObjectSetG[i];

                if (shape.shapeName == selectedShape)
                {
                    batchIndexes.Add(i);

                    // View
                    ECodedElement element;
                    element = c_measure.codedSelection.GetElement((uint)i);
                    DrawElement(ref element);
                    element.Dispose();
                }
            }


        }

        private void btn_Batch_Setting_Click(object sender, EventArgs e)
        {
            float widthStd = 0;
            float heightStd = 0;
            float diameterStd = 0;

            if(panel_Standard.Controls.Count == 2)
            {

                NumericUpDown controlDiameter = (NumericUpDown)panel_Standard.Controls[1];

                diameterStd = (float)controlDiameter.Value;
            }
            else
            {
                NumericUpDown controlWidth = (NumericUpDown)panel_Standard.Controls[1];
                NumericUpDown controlHeight = (NumericUpDown)panel_Standard.Controls[3];

                widthStd = (float)controlWidth.Value;
                heightStd = (float)controlHeight.Value;
            }


            foreach (var index in batchIndexes)
            {
                ObjectShape shape = (ObjectShape)c_measure.ObjectSetG[index];

                switch (shape.shapeName)
                {
                    case "square":
                        ObjectRectangle square = (ObjectRectangle)shape;

                        square.widthStd = widthStd;
                        square.heightStd = heightStd;
                        break;
                    case "rectangle":
                        ObjectRectangle rect = (ObjectRectangle)shape;
                        rect.widthStd = widthStd;
                        rect.heightStd = heightStd;
                        break;
                    case "circle":
                        ObjectCircle circle = (ObjectCircle)shape;
                        circle.diameterStd = diameterStd;
                        break;
                    case "special1":
                        ObjectSpecial1 special1 = (ObjectSpecial1)shape;
                        special1.widthStd = widthStd;
                        special1.heightStd = heightStd;
                        break;

                }

            }
        }

        // --------------------------Picturebox--------------------------
        private void pictureBox_Mose_Down(object sender, MouseEventArgs e)
        {
            // 點擊判定
            // 畫面有沒有圖案
            // 有沒有 ObjectSet

            int index;

            if (isGolden)
            {
                index = c_measure.IsClickObject(ref c_measure.ObjectSetG, e.X / scalingRatio, e.Y / scalingRatio);
            }
            else
            {
                index = c_measure.IsClickObject(ref c_measure.ObjectSetU, e.X / scalingRatio, e.Y / scalingRatio);
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

        // -------------------------------Listbox-------------------------------
        private void listBox_Measure_Selected_Changed(object sender, EventArgs e)
        {
            if (listBox_Measure.SelectedItem == null)
            {
                return;
            }

            int selectedIndex = int.Parse(listBox_Measure.SelectedItem.ToString().Substring(0, 3));

            ECodedElement element = c_measure.codedSelection.GetElement((uint)selectedIndex);

            DrawEBW8Image1();
            DrawElement(ref element);

            if (isGolden)
            {
                RenderShapeInfo(selectedIndex, c_measure.ObjectSetG);
                RenderStandard(selectedIndex, c_measure.ObjectSetG);

            }
            else
            {
                RenderShapeInfo(selectedIndex, c_measure.ObjectSetU);
                RenderStandard(selectedIndex, c_measure.ObjectSetU);

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

            ECodedElement element = c_measure.codedSelection.GetElement((uint)selectedIndex);

            DrawEBW8Image1();
            DrawNGElement(ref element);

            if (!isGolden)
            {
                RenderShapeErrorInfo(selectedIndex, c_measure.ObjectSetU);
            }

            element.Dispose();
        }

        // --------------------------Camera--------------------------
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

        // -----------------------MVSCamera-----------------------
        private void btn_MVSCamera_Click(object sender, EventArgs e)
        {
            // 如果設備還沒載入
            if (m_pMyCamera == null)
            {
                Search_Device(sender, e);
                Open_Camera(sender, e);
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


                scalingRatio = CalcRatioWithPictureBox(pictureBox1, EBW8Image1.Width, EBW8Image1.Height);

                pictureBox1.Image = null;
                pictureBox1.Refresh();
                EBW8Image1.Draw(graphics, scalingRatio);
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



        // -------------------------------Method-------------------------------
        internal void ListBoxAddObj(ListBox listBox, ObjectShape obj)
        {
            string objIndex = obj.index.ToString("000");
            string objCenterX = obj.centerX.ToString("#.#");
            string objCenterY = obj.centerY.ToString("#.#");

            listBox.Items.Add(objIndex + "(" + objCenterX + "," + objCenterY + ")");
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

        // -------------------------------View-------------------------------
        private void DrawEBW8Image1()
        {
            pictureBox1.Image = null;
            pictureBox1.Refresh();
            float width = EBW8Image1.Width;
            scalingRatio = CalcRatioWithPictureBox(pictureBox1, EBW8Image1.Width, EBW8Image1.Height);
            EBW8Image1.Draw(graphics, scalingRatio);
        }

        private void DrawElement(ref ECodedElement element)
        {
            c_measure.codedImage1.Draw(graphics, new ERGBColor(0, 0, 255), element, scalingRatio);

            panelIndex = 0;
            panelNGIndex = 0;
            panelStandardIndex = 0;
        }

        private void DrawNGElement(ref ECodedElement element)
        {
            c_measure.codedImage1.Draw(graphics, element, scalingRatio);

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

        private void AddItemInPanelMeasure(string labelText, float value)
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(0, panelNGIndex * 30);
            label_Title.Width = labelText.Length * (int)Math.Round((double)Font.Size * 1.5) + 3;

            Label label_Value = new Label();
            decimal number = decimal.Round((decimal)value, 1);
            label_Value.Text = number.ToString();
            label_Value.Location = new Point(label_Title.Width + 10, panelNGIndex * 30);
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
            label_Title.Location = new Point(0, panelNGIndex * 30);
            label_Title.Width = labelText.Length * (int)Math.Round((double)Font.Size * 1.5) + 3;

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

        private void AddItemInPanelStandard(string labelText, float value)
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(0, panelStandardIndex * 30);
            label_Title.Width = labelText.Length * (int)Math.Round((double)Font.Size * 1.5) + 3;

            NumericUpDown num_Standard = new NumericUpDown();
            decimal number = decimal.Round((decimal)value, 1);
            num_Standard.DecimalPlaces = 1;
            num_Standard.Increment = 0.1M;
            num_Standard.Maximum = 5000;
            num_Standard.Value = number;

            num_Standard.Location = new Point(label_Title.Width + 10, panelStandardIndex * 30);
            num_Standard.Width = 80;

            panel_Standard.Controls.Add(label_Title);
            panel_Standard.Controls.Add(num_Standard);
            panelStandardIndex++;

        }

        // -------------------------------Adjust-------------------------------
        private void Learn()
        {
            string RunningPath = Environment.CurrentDirectory;
            string StdImagePath = string.Format("{0}Resources\\PressItem.png", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\..\")));
            Console.WriteLine(RunningPath);
            Console.WriteLine(StdImagePath);
            Console.WriteLine(File.Exists(StdImagePath));
            EBW8ImageStd.Load(StdImagePath);

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
    }
}
