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

        string ok = "control OK";

        string pathSetting = Environment.CurrentDirectory + "\\Setting";
        string savePath;

        public string OK { get { return this.ok; } }

        public Control()
        {
            openFileDialog1 = new OpenFileDialog();



            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "Jpg|*.jpg|PNG|*.png|Json|*.json|All files|*.*";

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

        internal string InitializeSetting(List<ObjectShape> ObjectSetG, ref EImageBW8 image, int x, int y)
        {

            // 要先 check 新還舊
            // 載入舊的
            // 或設定新的

            string errorMessage;
            // 再 build history folder
            errorMessage = BuildHistoryFolder(ref image, ObjectSetG);
            if (errorMessage != this.ok)
            {
                Console.WriteLine(errorMessage);
                return errorMessage;
            }


            //if (ObjectSetG.Count > 0) //檢查是否有設定檔了
            //{
            //    if (MessageBox.Show("確定要覆蓋目前的工件設定內容嗎?", "開啟設定檔", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        // 設定 ObjectSetG
            //    }
            //    else
            //    {
            //        return "初始化錯誤";
            //    }
            //}
            //else
            //{
            //    // 設定 ObjectSetG
            //}
            return this.ok;
        }




        internal string CheckInitializeSetting()
        {
            string pathObjectSetG = pathSetting + "\\ObjectSetG";
            string pathDotGridImage = pathSetting + "\\DotGridImage";
            string pathCalibrationXY = pathSetting + "\\CalibrationXY";

            if (
                File.Exists(pathSetting) &&
                File.Exists(pathObjectSetG) &&
                File.Exists(pathDotGridImage) &&
                File.Exists(pathCalibrationXY)
                )
            {
                return this.ok;
            }
            else
            {
                return "請先初始化：校正點圖、測量標準物";
            }
        }

        internal string BuildHistoryFolder(ref EImageBW8 image, List<ObjectShape> ObjectSetG)
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

            // 存圖檔
            if (image == null || (image.Width == 0 && image.Height == 0))
            {
                return "請先載入圖片";
            }
            else
            {
                image.SavePng(path + "\\Standard.png");
            }

            // 存ObjectGSet
            string jsonString = JsonConvert.SerializeObject(ObjectSetG);
            File.WriteAllText(path + "\\ObjectSetG.json", jsonString);

            savePath = path;

            return this.ok;
        }

        internal void BuildNewSetting()
        {

        }
    }
}
