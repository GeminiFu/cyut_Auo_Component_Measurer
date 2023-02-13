using Euresys.Open_eVision_22_08;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cyut_Auo_Component_Measurer
{
    // 說明：
    // 做 data, file 轉換
    internal class Control
    {
        OpenFileDialog openFileDialog1;
        FolderBrowserDialog folderBrowserDialog1;
        SaveFileDialog saveFileDialog1;

        string ok = "control OK";

        string pathSave;

        public string OK { get { return this.ok; } }

        public Control()
        {
            openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "Jpg|*.jpg|PNG|*.png|Json|*.json|All files|*.*";

            folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;

            //saveFileDialog1.InitialDirectory = Application.StartupPath;
            //saveFileDialog1.Filter = "Json|*.json|BMP|*.bmp|All files|*.*";

            saveFileDialog1 = new SaveFileDialog();

            folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;

        }

        public string LoadEImageBW8(ref EImageBW8 image)
        {
            openFileDialog1.FilterIndex = 2; //png
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image.Load(openFileDialog1.FileName);

                return this.ok;
            }
            else
            {
                return "檔案讀取錯誤";
            }
        }

        // -------------------------------Setting-------------------------------
        // 初始化
        // 目的：載入預設標準資料、建立新的歷史資料夾
        internal string InitializeSetting(ref EImageBW8 standard,ref ArrayList ObjectSetG, ref EImageBW8 dotGridImage, int x, int y)
        {
            string errorMessage;

            string path = Environment.CurrentDirectory;
            // load ObjectSetG, Dot grid
            errorMessage = LoadLocalSetting(ref ObjectSetG, ref dotGridImage, ref x, ref y, path);

            if (errorMessage != ok) //防呆
                return errorMessage;

            // 儲存歷史設定檔
            errorMessage = SaveHistorySetting(ref standard, ObjectSetG, ref dotGridImage, x, y);

            if (errorMessage != ok) //防呆
                return errorMessage;

            return this.ok;
        }

        // 載入本地標準資料
        // 目的：載入本地的 ObjectSetG、Dot Grid Image、Calibration XY
        internal string LoadLocalSetting(ref ArrayList ObjectSetG, ref EImageBW8 dotGridImage, ref int x, ref int y, string selectedPath)
        {
            string errorMessage;
            string path = selectedPath;

            // 檢查本地標準資料是否存在
            errorMessage = CheckLocalSetting(selectedPath);

            if (errorMessage != ok)
                return errorMessage;

            path += "\\Setting";

            // ObjectSetG
            string jsonString = File.ReadAllText($@"{path}\ObjectSetG.json");
            ObjectSetG = JsonConvert.DeserializeObject<ArrayList>(jsonString);

            for (int i = 0; i < ObjectSetG.Count; i++)
            {
                Object obj = ObjectSetG[i];
                ObjectShape shape;

                shape = JsonConvert.DeserializeObject<ObjectShape>(obj.ToString());

                switch (shape.shapeName)
                {
                    case "square":
                        ObjectRectangle square = JsonConvert.DeserializeObject<ObjectRectangle>(obj.ToString());
                        ObjectSetG[i] = square;
                        break;
                    case "rectangle":
                        ObjectRectangle rect = JsonConvert.DeserializeObject<ObjectRectangle>(obj.ToString());
                        ObjectSetG[i] = rect;
                        break;
                    case "circle":
                        ObjectCircle circle = JsonConvert.DeserializeObject<ObjectCircle>(obj.ToString());
                        ObjectSetG[i] = circle;
                        break;
                    case "special1":
                        ObjectSpecial1 special1 = JsonConvert.DeserializeObject<ObjectSpecial1>(obj.ToString());
                        ObjectSetG[i] = special1;
                        break;
                    default:
                        Console.WriteLine(shape.shapeName);
                        break;
                }
            }

            // dot grid image
            dotGridImage.Load(path + "\\Dot_Grid.png");
            // calibration x y 
            x = int.Parse(File.ReadAllText(path + "\\Calibration_X.txt"));
            y = int.Parse(File.ReadAllText(path + "\\Calibration_Y.txt"));

            return ok;
        }

        // 確認本地標準資料存在
        // 目的：確認本地 ObjectSetG、Dot Grid Image、Calibration XY 存在
        internal string CheckLocalSetting(string selectedPath)
        {
            string path = selectedPath + "\\Setting";
            string pathObjectSetG = path + "\\ObjectSetG.json";
            string pathDotGridImage = path + "\\Dot_Grid.png";
            string pathCalibrationX = path + "\\Calibration_X.txt";
            string pathCalibrationY = path + "\\Calibration_Y.txt";


            if (!Directory.Exists(path))
                return "沒有找到資料夾";

            if (!File.Exists(pathObjectSetG))
                return "沒有找到 ObjectSetG";

            if (!File.Exists(pathDotGridImage))
                return "沒有找到點圖";

            if (!File.Exists(pathCalibrationX))
                return "沒有找到 X 文字檔";

            if (!File.Exists(pathCalibrationY))
                return "沒有找到 Y 文字檔";

            return ok;
        }

        // 建立歷史資料夾
        // 目的：建立歷史資料夾、並儲存路徑
        internal void BuildHistoryFolder()
        {
            string componentName = "1U2N2G-B550_2T-ChASSIS";

            // Result Folder
            string path = Environment.CurrentDirectory + "\\result";
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            // Date Folder
            path += "\\" + DateTime.Now.ToString("yyyyMMdd");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            // Component Name Folder
            path += "\\" + componentName;
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            // Time Folder
            path += "\\" + componentName + "_" + DateTime.Now.ToString("HHmmss");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            pathSave = path;

            // Setting
            path += "\\Setting";
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        // 儲存歷史設定
        // 目的：儲存 Standard Image、ObjectSetG、Dot Grid Image、Calibration XY 
        internal string SaveHistorySetting(ref EImageBW8 standard, ArrayList ObjectSetG, ref EImageBW8 dotGridImage, int x, int y)
        {
            string errorMessage;


            errorMessage = CheckSetting(ObjectSetG, ref dotGridImage);

            if (errorMessage != ok)
                return errorMessage;

            // 建立歷史資料夾
            BuildHistoryFolder();

            string path = pathSave + "\\Setting";

            // save standard image，有就存；沒有沒關係，主要是給人看的
            if (standard.IsVoid == false)
            {
                standard.SavePng(path + "\\Standard.png");
            }

            // save ObjectGSet
            string jsonString = JsonConvert.SerializeObject(ObjectSetG);
            File.WriteAllText(path + "\\ObjectSetG.json", jsonString);

            // save dot grid image
            dotGridImage.SavePng(path + "\\Dot_Grid.png");

            // save calibration x y 
            File.WriteAllText(path + "\\Calibration_X.txt", x.ToString());
            File.WriteAllText(path + "\\Calibration_Y.txt", y.ToString());

            return ok;
        }

        internal string CheckSetting(ArrayList ObjectSetG, ref EImageBW8 dotGridImage)
        {
            // check dot grid image
            if (dotGridImage.IsVoid)
                return "請先校正點圖";

            // check ObjectSetG
            if (ObjectSetG.Count <= 0)
                return "請先量測標準物";

            return ok;
        }

        // 手動儲存設定
        // 目的：做手動儲存設定的事前檢查
        internal string MenuSaveSetting(ref EImageBW8 standard, ArrayList ObjectSetG, ref EImageBW8 dotGridImage, int x, int y)
        {
            string errorMessage;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;

                errorMessage = CheckLocalSetting(selectedPath);

                if (errorMessage == ok)
                {
                    if (MessageBox.Show("確定要覆蓋目前的工件設定內容嗎?", "開啟設定檔", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        errorMessage = BuildNewLocalSetting(ref standard, ObjectSetG, ref dotGridImage, x, y, folderBrowserDialog1.SelectedPath);

                        if (errorMessage != ok)
                            return errorMessage;
                    }
                    else
                    {
                        return "不覆蓋已存在檔案";
                    }
                }
                else
                {
                    errorMessage = BuildNewLocalSetting(ref standard, ObjectSetG, ref dotGridImage, x, y, folderBrowserDialog1.SelectedPath);

                    if (errorMessage != ok)
                        return errorMessage;
                }
            }
            else
            {
                return "檔案存取錯誤";
            }

            return ok;
        }

        internal string BuildNewLocalSetting(ref EImageBW8 standard, ArrayList ObjectSetG, ref EImageBW8 dotGridImage, int x, int y, string seletedPath)
        {
            string errorMessage;

            errorMessage = CheckSetting(ObjectSetG, ref dotGridImage);

            if (errorMessage != ok)
                return errorMessage;

            string path = seletedPath + "\\Setting";

            // Setting 資料夾
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // save standard
            if (standard.IsVoid == false)
            {
                standard.SavePng(path + "\\Standard.png");
            }


            // save ObjectGSet
            string jsonString = JsonConvert.SerializeObject(ObjectSetG);
            File.WriteAllText(path + "\\ObjectSetG.json", jsonString);

            // save dot grid image
            dotGridImage.SavePng(path + "\\Dot_Grid.png");

            // save calibration x y 
            File.WriteAllText(path + "\\Calibration_X.txt", x.ToString());
            File.WriteAllText(path + "\\Calibration_Y.txt", y.ToString());

            return ok;
        }

        // 手動載入設定
        // 目的：做手動載入設定的事前檢查
        internal string MenuLoadSetting(ref EImageBW8 standard, ArrayList ObjectSetG, ref EImageBW8 dotGridImage, int x, int y)
        {
            string errorMessage;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;

                errorMessage = LoadLocalSetting(ref ObjectSetG, ref dotGridImage, ref x, ref y, selectedPath);

                if (errorMessage != ok)
                    return errorMessage;
            }

            // save history
            SaveHistorySetting(ref standard, ObjectSetG, ref dotGridImage, x, y);

            return ok;
        }

        internal string SaveInspectResult(ref EImageBW8 testImage, bool isOK)
        {
            string errorMessage = "";
            string fileName = DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss"); ;

            if (isOK)
            {
                fileName = "OK_" + fileName;
            }
            else
            {
                fileName = "NG_" + fileName;
            }
            string savePath = pathSave + "\\" + fileName + ".png";

            Console.WriteLine(savePath);

            testImage.Save(savePath);

            return OK;
        }

        internal string LoadOldImage(ref EImageBW8 image, ref ArrayList ObjectSetG, ref EImageBW8 dotGridImage, ref int x, ref int y)
        {
            string errorMessage = "";

            openFileDialog1.FilterIndex = 2;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog1.FileName;
                string folderPath = imagePath + @"\..";


                image.Load(imagePath);



                errorMessage = CheckLocalSetting(folderPath);

                if (errorMessage != ok)
                    return errorMessage;

                LoadLocalSetting(ref ObjectSetG, ref dotGridImage, ref x, ref y, folderPath);
            }

            return OK;
        }

        // -------------------------------Setting-------------------------------
    }
}
