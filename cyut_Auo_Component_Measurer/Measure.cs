﻿using Emgu.CV;
using Euresys.Open_eVision_22_08;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cyut_Auo_Component_Measurer
{
    public class Measure
    {
        internal delegate ObjectShape ElementsFunction(ref ECodedElement element, uint index);

        internal Measure() { }

        internal void Detect(ref EImageBW8 image, ref EImageEncoder codedImageEncoder, ref ECodedImage2 codedImage, ref EObjectSelection codedImageObjectSelection)
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

        internal void SetObjectSet(ref List<ObjectShape> ObjectSet, ref EObjectSelection codedSelector, ElementsFunction elementsFunction)
        {
            uint length = codedSelector.ElementCount;
            ECodedElement element;

            for (uint i = 0; i < length; i++)
            {
                element = codedSelector.GetElement(i);

                ObjectSet.Add(elementsFunction(ref element, i));

                element.Dispose();
            }
        }

        internal void SetObjectG(ref List<ObjectShape> ObjectSetG)
        {
            foreach(ObjectShape shape in ObjectSetG)
            {
                shape.checkResult = 0; //OK
            }
        }

        internal void SetObjectU(ref List<ObjectShape> ObjectSetG, ref List<ObjectShape> ObjectSetU)
        {
            // inspect
            // setting standard
            // setting error
        }

        internal int IsClickObject(ref List<ObjectShape> ObjectSet, float clickX, float clickY)
        {
            foreach(ObjectShape shape in ObjectSet)
            {
                if (shape.IsInShape(clickX, clickY))
                {
                    return (int)shape.index;
                }
            }

            return -1;
        }
    }
}
