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
    // 說明
    // 管理三大功能的邏輯
    // Detect => codedImageEncoder, codedImage, codedImageObjectSelection
    // codedImageObjectSelection => ObjectSet
    // Inspect => NG index

    public class Measure
    {
        EImageEncoder codedImage1Encoder = new EImageEncoder();
        internal ECodedImage2 codedImage1 = new ECodedImage2();
        internal EObjectSelection codedSelection = new EObjectSelection();

        internal delegate ObjectShape ElementsFunction(ref ECodedElement element, uint index);

        internal ArrayList ObjectSetG = new ArrayList();
        internal ArrayList ObjectSetU = new ArrayList();

        List<int> NGIndex = new List<int>();

        internal List<int> GetNGIndex { get { return NGIndex; } }

        internal Measure() { }

        // -------------------------------Detect-------------------------------
        internal void Detect(ref EImageBW8 image)
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
            codedImage1Encoder.Encode(image, codedImage1);

            // codedImage1ObjectSelection 設定
            codedSelection.Clear();
            codedSelection.FeretAngle = 0.00f;

            // codedImage1ObjectSelection 圖層
            codedSelection.AddObjects(codedImage1);
            codedSelection.AttachedImage = image;

            // don't care area 條件
            codedSelection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, 20, ESingleThresholdMode.Less);
            codedSelection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, 150000, ESingleThresholdMode.Greater);
        }

        // -------------------------------ObjectSet-------------------------------
        internal void BuildObjectSet(ArrayList ObjectSet, ElementsFunction elementsFunction)
        {
            uint length = codedSelection.ElementCount;
            ECodedElement element;

            ObjectSet.Clear();

            for (uint i = 0; i < length; i++)
            {
                element = codedSelection.GetElement(i);

                ObjectSet.Add(elementsFunction(ref element, i));

                element.Dispose();
            }
        }

        internal void SetObjectG(ArrayList ObjectSetG)
        {
            foreach (ObjectShape shape in ObjectSetG)
            {
                shape.checkResult = 0; //OK
            }
        }

        internal void SetObjectU(ref ArrayList ObjectSetG, ref ArrayList ObjectSetU)
        {
            // inspect
            // setting standard
            // setting error
        }

        internal int IsClickObject(ref ArrayList ObjectSet, float clickX, float clickY)
        {
            foreach (ObjectShape shape in ObjectSet)
            {
                if (shape.IsInShape(clickX, clickY))
                {
                    return (int)shape.index;
                }
            }

            return -1;
        }

        // -------------------------------Inspect-------------------------------
        internal void Inspect(ref ArrayList ObjectSetG, ref ArrayList ObjectSetU, decimal thresholdNG)
        {
            // !!!!!!!!!!!!!!!!!! Check ObjectSetG

            ObjectShape shapeTest;
            ObjectShape shapeStandard;

            float sameShapeThreshold = 10;
            for (int i = 0; i < ObjectSetU.Count; i++)
            {
                shapeTest = (ObjectShape)ObjectSetU[i];
                int j = 0;
                // 看兩個 shape 位置是不是差不多，確認兩個可以做比對
                do
                {
                    shapeStandard = (ObjectShape)ObjectSetG[j];
                    if ((Math.Abs(shapeTest.centerX - shapeStandard.centerX) < sameShapeThreshold) && 
                        (Math.Abs(shapeTest.centerY - shapeStandard.centerY) < sameShapeThreshold)
                        )
                    {
                        break;
                    }

                    j++;

                }while(j < ObjectSetG.Count);

                if (j > ObjectSetG.Count)
                {
                    shapeTest.checkResult = 1;
                    NGIndex.Add(i);
                    continue;
                }

                // 把 standard 設置給它 (可以不用)
                // 比對兩個是不是同樣的圖形 (暫時不用)

                if(shapeTest.GetType() != shapeStandard.GetType())
                {
                    Console.WriteLine("形狀不同"); // 長方形和正方形的形狀判定
                    //shapeTest.checkResult = 1;
                    //continue;
                }

                // 相減儲存在誤差
                shapeTest.SaveInspectInfo(shapeStandard);
                // 比對誤差是否在 Threshold 裡面
                if (shapeTest.Inspect(thresholdNG))
                {
                    shapeTest.checkResult = 0;
                }
                else
                {
                    shapeTest.checkResult = 1;

                    NGIndex.Add(i);
                }
            }

            
        }
    }
}
