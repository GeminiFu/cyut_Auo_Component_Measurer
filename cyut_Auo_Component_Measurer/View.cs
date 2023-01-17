using Euresys.Open_eVision_22_08;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace cyut_Auo_Component_Measurer
{
    // 說明
    // 視覺層面的輸出，操作 Form Control(包括 Enable)
    internal class View
    {
        PictureBox pictureBox = new PictureBox();
        Graphics graphics;
        float scalingRatio;
        int pennelIndex = 0;

        internal float GetScalingRatio { get { return scalingRatio; } }

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

        public void DrawAllElements(ref ECodedImage2 codedImage, ref EObjectSelection codedImageObjectSelection)
        {
            codedImage.Draw(graphics, codedImageObjectSelection, scalingRatio);
        }

        public void DrawElement(ref ECodedImage2 codedImage, ref ECodedElement element)
        {
            codedImage.Draw(graphics, element, scalingRatio);
        }



        internal void AddItemInPanel(Panel panel, string labelText, float value)
        {
            Label label_Title = new Label();
            label_Title.Text = labelText;
            label_Title.Text += ":";
            label_Title.Location = new Point(0, pennelIndex * 25);

            Label label_Value = new Label();
            decimal number = decimal.Round((decimal)value, 1);
            label_Value.Text = number.ToString();
            label_Value.Location = new Point(100, pennelIndex * 25);
            label_Value.BackColor = Color.FromArgb(255, 224, 192);
            label_Value.Width = 50;

            panel.Controls.Add(label_Title);
            panel.Controls.Add(label_Value);
            pennelIndex++;
        }

        internal void ListBoxAddObj(ListBox listBox, ObjectShape obj)
        {
            string objIndex = obj.index.ToString("000");
            string objCenterX = obj.centerX.ToString("#.#");
            string objCenterY = obj.centerY.ToString("#.#");

            listBox.Items.Add(objIndex + "(" + objCenterX + "," + objCenterY + ")");
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

            if (imageRatio > pbRatio)
                return  pbWidth / imageWidth;
            else
                return pbHeight / imageHeight;
        }
    }
}
