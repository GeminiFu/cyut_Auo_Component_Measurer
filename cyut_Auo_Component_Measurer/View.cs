using Euresys.Open_eVision_22_08;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cyut_Auo_Component_Measurer
{
    internal class View
    {
        PictureBox pictureBox = new PictureBox();
        Graphics graphics;
        float scalingRatio;

        public View(ref PictureBox pb) 
        {
            pictureBox = pb;

            graphics = pictureBox.CreateGraphics();
        }

        public void DrawEBW8Image(EImageBW8 img)
        {
            pictureBox.Image = null;
            pictureBox.Refresh();
            scalingRatio = CalcRatioWithPictureBox(pictureBox, img.Width, img.Height);
            img.Draw(graphics, scalingRatio);
        }


        public void DrawBitmap(Bitmap bmp)
        {
            pictureBox.Image = bmp;
        }





        // -----------------------Method-----------------------
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

            float ratioBetween;

            if (imageRatio > pbRatio)
                ratioBetween = pbWidth / imageWidth;
            else
                ratioBetween = pbHeight / imageHeight;

            return ratioBetween;
        }
    }
}
