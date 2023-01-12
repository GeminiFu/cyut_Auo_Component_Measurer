using Euresys.Open_eVision_22_08;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cyut_Auo_Component_Measurer
{
    internal class Control
    {
        OpenFileDialog openFileDialog1;

        public Control()
        {
            openFileDialog1 = new OpenFileDialog();



            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "Jpg|*.jpg|PNG|*.png|Json|*.json|All files|*.*";

            //saveFileDialog1.InitialDirectory = Application.StartupPath;
            //saveFileDialog1.Filter = "Json|*.json|BMP|*.bmp|All files|*.*";

        }

        public void LoadEImageBW8(ref EImageBW8 image)
        {
            openFileDialog1.FilterIndex = 2; //png
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image.Load(openFileDialog1.FileName);
            }
        }
    }
}
