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
    // 說明
    // 人類操作的檢查
    // 與 Local 交換資料的邏輯
    internal class Control
    {
        OpenFileDialog openFileDialog1;
        FolderBrowserDialog folderBrowserDialog1;

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



        internal string InitializeSetting(ref EImageBW8 standard, ref List<ObjectShape> ObjectSetG, EImageBW8 dotGridImage, ref int x, ref int y)
        {
            string errorMessage;

            // load ObjectSetG, Dot grid
            errorMessage = LoadSetting(ref ObjectSetG, ref dotGridImage, ref x, ref y, Environment.CurrentDirectory);

            if (errorMessage != ok) //防呆
                return errorMessage;

            // build history folder
            BuildHistoryFolder(Environment.CurrentDirectory);

            // 儲存歷史設定檔
            errorMessage = SaveHistorySetting(ref standard, ObjectSetG, ref dotGridImage, x, y);

            if (errorMessage != ok) //防呆
                return errorMessage;

            return this.ok;
        }

        internal string LoadSetting(ref List<ObjectShape> ObjectSetG, ref EImageBW8 dotGridImage, ref int x, ref int y, string selectedPath)
        {
            string errorMessage;
            string path = selectedPath;

            errorMessage = CheckSettingFile(selectedPath);


            if (errorMessage != ok)
                return errorMessage;

            path += "\\Setting";

            // ObjectSetG
            string jsonString = File.ReadAllText(selectedPath + "\\ObjectSetG.json");
            ObjectSetG = JsonConvert.DeserializeObject<List<ObjectShape>>(jsonString);

            // dot grid image
            dotGridImage.Load(path + "\\Dot_Grid.png");

            // calibration x y 
            x = int.Parse(File.ReadAllText(path + "\\Calibration_X.txt"));
            y = int.Parse(File.ReadAllText(path + "\\Calibration_Y.txt"));


            return ok;
        }

        internal string CheckSettingFile(string selectedPath)
        {
            string pathObjectSetG = selectedPath + "\\ObjectSetG.json";
            string pathDotGridImage = selectedPath + "\\Dot_Grid.png";
            string pathCalibrationX = selectedPath + "\\Calibration_X.txt";
            string pathCalibrationY = selectedPath + "\\Calibration_Y.txt";

            if (!Directory.Exists(selectedPath))
                return "沒有找到資料夾";

            if (Directory.Exists(pathObjectSetG))
                return "沒有找到 ObjectSetG";

            if (!File.Exists(pathDotGridImage))
                return "沒有找到點圖";

            if (!File.Exists(pathCalibrationX))
                return "沒有找到 X 文字檔";

            if (!File.Exists(pathCalibrationY))
                return "沒有找到 Y 文字檔";

            return ok;
        }

        internal void BuildHistoryFolder(string seletedPath)
        {
            string componentName = "1U2N2G-B550_2T-ChASSIS";

            // Result Folder
            string path = seletedPath + "\\result";
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
            path += "\\" + componentName + DateTime.Now.ToString("HHmmss");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            pathSave = path;
        }

        internal string SaveHistorySetting(ref EImageBW8 standard, List<ObjectShape> ObjectSetG, ref EImageBW8 dotGridImage, int x, int y)
        {
            string errorMessage;

            errorMessage = CheckSetting(ref standard, ref ObjectSetG, ref dotGridImage);

            if (errorMessage != ok)
                return errorMessage;

            // save dot grid image
            dotGridImage.Load(pathSave + "\\Dot_Grid.png");

            // save calibration x y 
            File.WriteAllText(pathSave + "\\Calibration_X.txt", x.ToString());
            File.WriteAllText(pathSave + "\\Calibration_Y.txt", y.ToString());

            // save standard image
            if (standard.IsVoid == false)
            {
                standard.SavePng(pathSave + "\\Standard.png");
            }

            // save ObjectGSet
            string jsonString = JsonConvert.SerializeObject(ObjectSetG);
            File.WriteAllText(pathSave + "\\ObjectSetG.json", jsonString);

            return ok;
        }

        internal string CheckSetting(ref EImageBW8 standard, ref List<ObjectShape> ObjectSetG, ref EImageBW8 dotGridImage)
        {
            // check dot grid image
            if (dotGridImage.IsVoid)
                return "請先校正點圖";

            // check Standard image
            if (standard.IsVoid)
                return "請先載入標準物";

            // check ObjectSetG
            if (ObjectSetG.Count <= 0)
                return "請先量測標準物";

            return ok;
        }





        internal string BuildNewSetting(ref EImageBW8 image, List<ObjectShape> ObjectSetG, string seletedPath)
        {
            string path = seletedPath + "\\Setting";
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            // !!!!!!!!!!!!!!!!!!!!!!!!!    dot grid image
            // !!!!!!!!!!!!!!!!!!!!!!!!!    calibration x y 


            // check image, check objectSetG
            if (image.IsVoid)
            {
                Console.WriteLine("沒有儲存 Standard image");
            }
            else
            {
                image.SavePng(path + "\\Standard.png");
            }

            // 存ObjectGSet
            if (ObjectSetG.Count > 0)
            {
                string jsonString = JsonConvert.SerializeObject(ObjectSetG);
                File.WriteAllText(path + "\\ObjectSetG.json", jsonString);
            }
            else
            {
                return "請先量測圖片";
            }



            return ok;

        }

        internal string MenuSaveSetting(ref EImageBW8 image, List<ObjectShape> ObjectSetG)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;

                if (Directory.Exists(folderBrowserDialog1.SelectedPath + "\\Setting"))
                {
                    if (MessageBox.Show("確定要覆蓋目前的工件設定內容嗎?", "開啟設定檔", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        BuildNewSetting(ref image, ObjectSetG, folderBrowserDialog1.SelectedPath);
                    }
                    else
                    {
                        return "不覆蓋已存在檔案";
                    }
                }
                else
                {
                    Console.WriteLine(selectedPath);

                    BuildNewSetting(ref image, ObjectSetG, selectedPath);
                }
            }

            return ok;
        }

        internal string MenuLoadSetting(ref EImageBW8 standard, ref List<ObjectShape> ObjectSetG, ref EImageBW8 dotGridImage, ref int x, ref int y)
        {
            string errorMessage;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;

                errorMessage = CheckSettingFile(selectedPath);

                if(errorMessage != ok)
                {
                    return errorMessage;
                }

                errorMessage = LoadSetting(ref ObjectSetG, ref dotGridImage, ref x, ref y, selectedPath);

                if (errorMessage != ok)
                    return errorMessage;
            }

            // build history folder
            BuildHistoryFolder(Environment.CurrentDirectory);

            // save history
            SaveHistorySetting(ref standard, ObjectSetG,ref dotGridImage, x, y);

            return ok;
        }



    }
}
