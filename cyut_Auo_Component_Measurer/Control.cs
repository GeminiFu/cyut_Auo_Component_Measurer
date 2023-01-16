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

        string pathSetting = Environment.CurrentDirectory + "\\Setting";
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

        internal string InitializeSetting(ref List<ObjectShape> ObjectSetG, ref EImageBW8 image, int x, int y)
        {
            string errorMessage;

            // 載入 Setting
            if (CheckSetting(pathSetting))
            {
                

                // ObjectSetG
                string jsonString = File.ReadAllText(pathSetting + "\\ObjectSetG.json");
                ObjectSetG = JsonConvert.DeserializeObject<List<ObjectShape>>(jsonString);
            }
            else
            {
                errorMessage = "請先初始化：校正點圖、測量標準物";
                return errorMessage;
            }

            // 再 build history folder
            errorMessage = BuildHistoryFolder(ref image, ObjectSetG, Environment.CurrentDirectory);
            if (errorMessage != this.ok)
            {
                Console.WriteLine(errorMessage);
                return errorMessage;
            }

            return this.ok;
        }



        internal bool CheckSetting(string selectedPath)
        {
            string pathObjectSetG = selectedPath + "\\ObjectSetG.json";
            //string pathDotGridImage = seletedPath + "\\DotGridImage";
            //string pathCalibrationXY = seletedPath + "\\CalibrationXY";

            if (
                Directory.Exists(selectedPath) &&
                //File.Exists(pathDotGridImage) &&
                //File.Exists(pathCalibrationXY)&&
                File.Exists(pathObjectSetG)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal string BuildHistoryFolder(ref EImageBW8 image, List<ObjectShape> ObjectSetG, string seletedPath)
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
            else
            {
                if (MessageBox.Show("確定要覆蓋目前的工件設定內容嗎?", "開啟設定檔", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    return "不覆蓋已存在檔案";
                }
            }

            // !!!!!!!!!!!!!!!!!!!!!!!!!    dot grid image
            // !!!!!!!!!!!!!!!!!!!!!!!!!    calibration x y 

            // 存圖檔
            if (image == null || (image.Width == 0 && image.Height == 0))
            {
                Console.WriteLine("沒有儲存 Standard image");
            }
            else
            {
                image.SavePng(path + "\\Standard.png");
            }

            // 存ObjectGSet
            string jsonString = JsonConvert.SerializeObject(ObjectSetG);
            File.WriteAllText(path + "\\ObjectSetG.json", jsonString);

            pathSave = path;

            return this.ok;
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
            if (image == null || (image.Width == 0 && image.Height == 0))
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
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
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

        internal string MenuLoadSetting(ref EImageBW8 image, ref List<ObjectShape> ObjectSetG)
        {
            string errorMessage;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;

                if (CheckSetting(selectedPath))
                {
                    // !!!!!!!!!!!!!!!!!!!!!!!!!    dot grid image
                    // !!!!!!!!!!!!!!!!!!!!!!!!!    calibration x y 

                    // ObjectSetG
                    string jsonString = File.ReadAllText(selectedPath + "\\ObjectSetG.json");
                    ObjectSetG = JsonConvert.DeserializeObject<List<ObjectShape>>(jsonString);

                }
                else
                {
                    return "沒有設定檔";
                }
            }

            errorMessage = BuildHistoryFolder(ref image, ObjectSetG, Environment.CurrentDirectory);
            if(errorMessage != ok)
            {
                return errorMessage;
            }

            return ok;
        }


   
    }
}
