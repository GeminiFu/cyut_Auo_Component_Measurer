using Emgu.CV;
using Euresys.Open_eVision_22_08;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cyut_Auo_Component_Measurer
{
    public class Measure
    {

        public Measure() { }

        public void Detect(ref EImageBW8 image, ref EImageEncoder codedImageEncoder, ref ECodedImage2 codedImage, ref EObjectSelection codedImageObjectSelection)
        {
            // 如果 EBW8Image1
            if (image == null || (image.Width == 0 && image.Height == 0))
            {
                return;
            }
            // codedImage1Encoder 設定
            //codedImage1Encoder.GrayscaleSingleThresholdSegmenter.BlackLayerEncoded = false; //為初始設定
            //codedImage1Encoder.GrayscaleSingleThresholdSegmenter.WhiteLayerEncoded = true; //為初始設定
            //codedImage1Encoder.GrayscaleSingleThresholdSegmenter.Mode = EGrayscaleSingleThreshold.MinResidue; //為初始設定

            // codedImage1 圖層
            codedImageEncoder.Encode(image, codedImage);

            // codedImage1ObjectSelection 設定
            codedImageObjectSelection.Clear();
            codedImageObjectSelection.FeretAngle = 0.00f;

            // codedImage1ObjectSelection 圖層
            codedImageObjectSelection.AddObjects(codedImage);
            codedImageObjectSelection.AttachedImage = image;

            // don't care area 條件
            codedImageObjectSelection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, 20, ESingleThresholdMode.Less);
            codedImageObjectSelection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, 150000, ESingleThresholdMode.Greater);
        
            
        }
    }
}
