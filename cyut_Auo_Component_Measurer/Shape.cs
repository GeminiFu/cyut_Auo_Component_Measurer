using Euresys.Open_eVision_22_08;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyut_Auo_Component_Measurer
{
    internal class Shape
    {
        public Shape() { }

        public string ShapeDeterminer(ref ECodedElement element)
        {
            float threshold;
            float area = element.BoundingBoxWidth * element.BoundingBoxHeight;
            //這裡需要先使用Raio判斷長方形或圓形(正方形)，亦或者分別量測，有結果的才算是有那個形狀
            if (element.BoundingBoxWidth / element.BoundingBoxHeight >= 0.95 && element.BoundingBoxWidth / element.BoundingBoxHeight <= 1.05) //正方形或圓形
            {
                //必須先量測正方形，再來量測圓形，不然會誤判
                //嘗試看看是否為正方形


                // threshold =  1/4 area - 1/16 * PI * area
                threshold = (area * 4 / 9) - (area / 9 * (float)Math.PI); // 直角面積 - 1/3圓角面積

                if ((area -(float)element.Area) > threshold)
                {
                    // 圓型
                    return "circle";
                }
                else
                {
                    // 方形
                    return "square";
                }
            }
            else
            {
                threshold = area * 0.9f;

                if (element.Area > threshold)
                {
                    return "rectangle";
                }
                else
                {
                    return "sapecial";
                }

            }

        }


        private ERectangle MeasureRect(ref EWorldShape EWorldShape, ref EImageBW8 image, ref ECodedElement element)
        {
            ERectangleGauge ERectangleGauge1 = new ERectangleGauge();
            EWorldShape.SetSensorSize(image.Width, image.Height);
            ERectangleGauge1.Attach(EWorldShape);
            ERectangleGauge1.Center = element.BoundingBoxCenter;
            if (element.BoundingBoxWidth > element.BoundingBoxHeight) //容忍值必須以寬高數值小的為基準，不然涵蓋兩個邊
            {
                ERectangleGauge1.Tolerance = element.BoundingBoxHeight / 4;
            }
            else
            {
                ERectangleGauge1.Tolerance = element.BoundingBoxWidth / 4;
            }
            ERectangleGauge1.TransitionType = ETransitionType.Bw; //由外往裡面看
            ERectangleGauge1.SamplingStep = 1;
            ERectangleGauge1.SetSize(element.BoundingBoxWidth, element.BoundingBoxHeight);
            ERectangleGauge1.Measure(image);

            if (ERectangleGauge1.NumValidSamples < ERectangleGauge1.NumSamples * 0.9) //防呆機制，萬一給的不是長方形
            {
                return null;
            }
            else
            {
                return ERectangleGauge1.MeasuredRectangle;
            }
        }

        //private ECircle MeasureCircle(ref EImageBW8 img, ref ECodedElement element)
        //{
        //    //先用圓形量測
        //    EWorldShape1.SetSensorSize(EBW8Image1.Width, EBW8Image1.Height);
        //    ECircleGauge1.Attach(EWorldShape1);
        //    ECircleGauge1.Center = element.BoundingBoxCenter;
        //    ECircleGauge1.Diameter = element.BoundingBoxWidth;
        //    ECircleGauge1.TransitionType = ETransitionType.Wb; //是由圓心往外看，跟Rectangle不同
        //    ECircleGauge1.Tolerance = element.BoundingBoxWidth / 5;
        //    ECircleGauge1.SamplingStep = 1; //每個點都要檢查
        //    ECircleGauge1.Measure(img);
        //    if (ECircleGauge1.NumValidSamples < ECircleGauge1.NumSamples * 0.9) //防呆機制，萬一給的不是長方形
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return ECircleGauge1.MeasuredCircle;
        //    }
        //}
        ////回傳量測的寬與高
        //public EPoint MeasureSpecial(EImageBW8 img, ECodedElement element)
        //{
        //    //特殊形狀，只有量測精準寬與高
        //    //假設條件，圖案必須是上下左右對稱，有角度偏差會進行修正
        //    //量測方式: 以BoundingCenter為中心，修正角度後進行十字線，兩條PointGauge進行量測
        //    double tmpW = 0, tmpH = 0;
        //    EWorldShape1.SetSensorSize(img.Width, img.Height);
        //    EPointGauge1.Attach(EWorldShape1); //將LineGauge繫結到世界座標系統
        //    EPointGauge1.TransitionType = ETransitionType.BwOrWb; //設定邊緣轉換類型，兩端剛好有Bw與Wb
        //    EPointGauge1.SetCenterXY(element.BoundingBoxCenterX, element.BoundingBoxCenterY);
        //    EPointGauge1.Tolerance = element.BoundingBoxWidth / 2 + 10;
        //    EPointGauge1.ToleranceAngle = element.MinimumEnclosingRectangleAngle;
        //    EPointGauge1.Angle = element.MinimumEnclosingRectangleAngle;//要看那一個角度量出來比較準
        //    EPointGauge1.Thickness = 3; //增加厚度，避免小雜訊
        //                                //EPointGauge1.Angle = element.EllipseAngle;
        //    EPointGauge1.Measure(img);
        //    EPointGauge1.SetZoom(scalingRatio);
        //    EPointGauge1.Draw(g, EDrawingMode.Actual, true);
        //    //檢查有沒有取到兩個點
        //    if (EPointGauge1.NumMeasuredPoints != 2) //如果量測到的point不到兩個，表示沒有量測到邊緣
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        EPoint tmpP1, tmpP2;
        //        tmpP1 = EPointGauge1.GetMeasuredPoint(0);
        //        tmpP2 = EPointGauge1.GetMeasuredPoint(1);
        //        tmpW = Math.Sqrt(Math.Pow(tmpP1.X - tmpP2.X, 2) + Math.Pow(tmpP1.Y - tmpP2.Y, 2));
        //        //量測另外一個垂直方向
        //        //EWorldShape1.SetSensorSize(EBW8Image1.Width, EBW8Image1.Height);
        //        //EPointGauge1.Attach(EWorldShape1); //將LineGauge繫結到世界座標系統
        //        //EPointGauge1.TransitionType = ETransitionType.BwOrWb; //設定邊緣轉換類型，兩端剛好有Bw與Wb
        //        //EPointGauge1.SetCenterXY(element.BoundingBoxCenterX, element.BoundingBoxCenterY);
        //        EPointGauge1.Tolerance = element.BoundingBoxHeight / 2 + 10;
        //        EPointGauge1.ToleranceAngle = element.MinimumEnclosingRectangleAngle + 270;
        //        EPointGauge1.Angle = element.MinimumEnclosingRectangleAngle + 270;//要看那一個角度量出來比較準，PS: 加90度居然無法量測，好怪
        //                                                                          //EPointGauge1.Angle = element.EllipseAngle;
        //        EPointGauge1.Measure(img);

        //        EPointGauge1.SetZoom(scalingRatio);
        //        EPointGauge1.Draw(g, EDrawingMode.Actual, true);
        //        //檢查有沒有取到兩個點
        //        if (EPointGauge1.NumMeasuredPoints != 2) //如果量測到的point不到兩個，表示沒有量測到邊緣
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            tmpP1 = EPointGauge1.GetMeasuredPoint(0);
        //            tmpP2 = EPointGauge1.GetMeasuredPoint(1);
        //            tmpH = Math.Sqrt(Math.Pow(tmpP1.X - tmpP2.X, 2) + Math.Pow(tmpP1.Y - tmpP2.Y, 2));
        //            return new EPoint((float)tmpW, (float)tmpH);
        //        }
        //    }
        //}

    }
}
