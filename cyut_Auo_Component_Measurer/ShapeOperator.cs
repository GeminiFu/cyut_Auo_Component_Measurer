﻿using Euresys.Open_eVision_22_08;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace cyut_Auo_Component_Measurer
{
    // 說明
    // 管理圖形、圖形辨識、圖形測量
    // 
    internal class ShapeOperator
    {
        EWorldShape EWorldShape1 = new EWorldShape();
        int calibrationX = 5;
        int calibrationY = 5;


        internal int CalibrationX { get { return calibrationX; } set { calibrationX = value; } }
        internal int CalibrationY { get { return calibrationY; } set { calibrationY = value; } }


        internal void AutoCalibration(ref EImageBW8 image, int x, int y)
        {
            EWorldShape1.AutoCalibrateDotGrid(image, x, y, false);


            if (EWorldShape1.CalibrationSucceeded() == false)
            {
                MessageBox.Show("Calibration Failed");
                return;
            }
            else
            {
                calibrationX = x;
                calibrationY = y;
            }
        }

        internal ShapeOperator()
        {

        }

        internal ObjectShape ShapeDetermine(ref ECodedElement element, uint index)
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

                if ((area - (float)element.Area) > threshold)
                {
                    ObjectCircle circle = new ObjectCircle();

                    circle.SetCenter(ref element);
                    circle.SetDiameter(ref element);
                    circle.index = index;

                    // 圓型
                    return circle;
                }
                else
                {
                    ObjectRectangle square = new ObjectRectangle();
                    square.SetSquare();
                    square.SetCenter(ref element);
                    square.SetWidthHeight(ref element);
                    square.index = index;

                    // 方形
                    return square;
                }
            }
            else
            {
                threshold = area * 0.9f;

                if (element.Area > threshold)
                {
                    ObjectRectangle rectangle = new ObjectRectangle();
                    rectangle.SetCenter(ref element);
                    rectangle.SetWidthHeight(ref element);
                    rectangle.index = index;
                    return rectangle;
                }
                else
                {
                    ObjectSpecial1 special1 = new ObjectSpecial1();
                    special1.SetCenter(ref element);
                    special1.SetWidthHeight(ref element);
                    special1.index = index;

                    return special1;
                }

            }

        }
    }


    internal class ObjectShape
    {
        // 用 internal，deserielize 會無法賦予值
        public int shapeNo; //-1: unknown, 0: rectangle, 1: circle, 2: special
        public string shapeName;
        //internal EPoint center; //注意:原本使用這個物件，Serialize的時候會導致StackOverflow的例外
        public float centerX, centerY;
        public int checkResult; //紀錄該物件是否被檢查過，以及結果 -1: 還沒有檢查 0:OK  1:NG
        public uint index; //紀錄codedImage中的索引

        private string ok = "Shape is ok.";

        internal string OK { get { return ok; } }


        internal ObjectShape()
        {
            shapeNo = -1;
            checkResult = -1;
        }

        internal void SetCenter(ref ECodedElement element)
        {
            centerX = element.BoundingBoxCenterX;
            centerY = element.BoundingBoxCenterY;
        }

        internal bool IsSameCenter(ObjectShape shape1, ObjectShape shape2, float threshold)
        {
            if ((Math.Abs(shape1.centerX - shape2.centerX) < threshold) && (Math.Abs(shape1.centerY - shape2.centerY) < threshold))
                return true;
            else
                return false;
        }

        // -------------------------------Is In Shape-------------------------------

        protected delegate bool InShapeEvent(float X, float Y);

        protected InShapeEvent inShapeEvent;

        internal bool IsInShape(float X, float Y)
        {
            return inShapeEvent.Invoke(X, Y);
        }

        // -------------------------------Inspect-------------------------------
        protected delegate bool InspectEvent(decimal thresholdNG);

        protected InspectEvent inspectEvent;

        internal bool Inspect(decimal thresholdNG)
        {
            return inspectEvent.Invoke(thresholdNG);
        }

        // -------------------------------Save Inspect Info-------------------------------

        protected delegate void SaveInspectInfoEvent(ObjectShape objectStandard);

        protected SaveInspectInfoEvent saveInspectInfoEvent;

        internal void SaveInspectInfo(ObjectShape objectStandard)
        {
            saveInspectInfoEvent.Invoke(objectStandard);
        }
    }

    internal class ObjectRectangle : ObjectShape
    {
        public float width;
        public float height;
        public float widthStd;
        public float heightStd;
        public float widthError;
        public float heightError;

        internal ObjectRectangle()
        {
            shapeNo = 0;
            shapeName = "rectangle";

            // -------------------delegate-------------------
            inShapeEvent += IsInRectangle;
            saveInspectInfoEvent += SaveInspectInfoRectangle;
            inspectEvent += InspectRectangle;
        }
        //square check

        internal void SetSquare()
        {
            shapeNo = 1;
            shapeName = "square";
        }

        internal string SetWidthHeight(ref ECodedElement element)
        {
            if (element == null)
            {
                return "Rectangle SetWidthHeight error, element is null.";
            }
            width = element.BoundingBoxWidth;
            height = element.BoundingBoxHeight;


            return OK;
        }

        internal bool IsInRectangle(float X, float Y)
        {
            float x_distance = Math.Abs(centerX - X);
            float y_distance = Math.Abs(centerY - Y);

            if (x_distance < (width / 2) && y_distance < (height / 2))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        internal void SaveInspectInfoRectangle(ObjectShape objectStandard)
        {
            ObjectRectangle tempRectangle = (ObjectRectangle)objectStandard;

            widthError = Math.Abs(width - tempRectangle.width);
            heightError = Math.Abs(height - tempRectangle.height);

            widthStd = tempRectangle.width;
            heightStd = tempRectangle.height;
        }

        internal bool InspectRectangle(decimal thresholdNG)
        {
            if (widthError < (float)thresholdNG && heightError < (float)thresholdNG)
            {
                return true;
            }

            return false;
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
                ERectangle rect = new ERectangle();
                return ERectangleGauge1.MeasuredRectangle;
            }
        }

    }

    //internal class ObjectSquare : ObjectRectangle
    //{
    //}

    internal class ObjectCircle : ObjectShape
    {
        public float diameter;
        public float diameterStd;
        public float diameterError;

        internal ObjectCircle()
        {
            shapeNo = 2;
            shapeName = "circle";


            // -------------------delegate-------------------
            inShapeEvent += IsInCircle;

            saveInspectInfoEvent += SaveInspectInfoCircle;

            inspectEvent += InspectCircle;
        }

        internal string SetDiameter(ref ECodedElement element)
        {
            if (element == null)
            {

                return "Circle SetDiameter error, element is null.";
            }

            diameter = element.BoundingBoxWidth;

            return OK;
        }

        private ECircle MeasureCircle(ref EWorldShape EWorldShape, ref EImageBW8 image, ref ECodedElement element)
        {
            ECircleGauge ECircleGauge = new ECircleGauge();
            //先用圓形量測
            EWorldShape.SetSensorSize(image.Width, image.Height);
            ECircleGauge.Attach(EWorldShape);
            ECircleGauge.Center = element.BoundingBoxCenter;
            ECircleGauge.Diameter = element.BoundingBoxWidth;
            ECircleGauge.TransitionType = ETransitionType.Wb; //是由圓心往外看，跟Rectangle不同
            ECircleGauge.Tolerance = element.BoundingBoxWidth / 5;
            ECircleGauge.SamplingStep = 1; //每個點都要檢查
            ECircleGauge.Measure(image);
            if (ECircleGauge.NumValidSamples < ECircleGauge.NumSamples * 0.9) //防呆機制，萬一給的不是長方形
            {
                return null;
            }
            else
            {
                return ECircleGauge.MeasuredCircle;
            }

        }

        internal bool IsInCircle(float X, float Y)
        {
            float x_distance = Math.Abs(centerX - X);
            float y_distance = Math.Abs(centerY - Y);

            if (x_distance < (diameter / 2) && y_distance < (diameter / 2))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        internal void SaveInspectInfoCircle(ObjectShape objectStandard)
        {
            ObjectCircle tempCircle = (ObjectCircle)objectStandard;
            diameterError = Math.Abs(diameter - tempCircle.diameter);

            diameterStd = tempCircle.diameter;
        }

        internal bool InspectCircle(decimal thresholdNG)
        {
            if (diameterError < (float)thresholdNG)
            {
                return true;
            }

            return false;
        }

    }

    internal class ObjectSpecial1 : ObjectShape
    {
        public float width;
        public float height;
        public float widthStd;
        public float heightStd;
        public float widthError;
        public float heightError;

        internal ObjectSpecial1()
        {
            shapeNo = 3;
            shapeName = "special1";


            inShapeEvent += IsInSpecial1;

            saveInspectInfoEvent += SaveInspectInfoSpecial1;

            inspectEvent += InspectSpecial1;
        }

        internal string SetWidthHeight(ref ECodedElement element)
        {
            if (element == null)
            {
                return "Special1 SetWidthHeight error, element is null.";
            }

            width = element.BoundingBoxWidth;
            height = element.BoundingBoxHeight;

            return OK;
        }


        //回傳量測的寬與高
        internal EPoint MeasureSpecial(ref EWorldShape EWorldShape, EImageBW8 image, ECodedElement element)
        {
            EPointGauge EPointGauge = new EPointGauge();

            //特殊形狀，只有量測精準寬與高
            //假設條件，圖案必須是上下左右對稱，有角度偏差會進行修正
            //量測方式: 以BoundingCenter為中心，修正角度後進行十字線，兩條PointGauge進行量測
            double tmpW = 0, tmpH = 0;
            EWorldShape.SetSensorSize(image.Width, image.Height);
            EPointGauge.Attach(EWorldShape); //將LineGauge繫結到世界座標系統
            EPointGauge.TransitionType = ETransitionType.BwOrWb; //設定邊緣轉換類型，兩端剛好有Bw與Wb
            EPointGauge.SetCenterXY(element.BoundingBoxCenterX, element.BoundingBoxCenterY);
            EPointGauge.Tolerance = element.BoundingBoxWidth / 2 + 10;
            EPointGauge.ToleranceAngle = element.MinimumEnclosingRectangleAngle;
            EPointGauge.Angle = element.MinimumEnclosingRectangleAngle;//要看那一個角度量出來比較準
            EPointGauge.Thickness = 3; //增加厚度，避免小雜訊
                                       //EPointGauge1.Angle = element.EllipseAngle;
            EPointGauge.Measure(image);
            //EPointGauge.SetZoom(scalingRatio);
            //EPointGauge.Draw(g, EDrawingMode.Actual, true);
            //檢查有沒有取到兩個點
            if (EPointGauge.NumMeasuredPoints != 2) //如果量測到的point不到兩個，表示沒有量測到邊緣
            {
                return null;
            }
            else
            {
                EPoint tmpP1, tmpP2;
                tmpP1 = EPointGauge.GetMeasuredPoint(0);
                tmpP2 = EPointGauge.GetMeasuredPoint(1);
                tmpW = Math.Sqrt(Math.Pow(tmpP1.X - tmpP2.X, 2) + Math.Pow(tmpP1.Y - tmpP2.Y, 2));
                //量測另外一個垂直方向
                //EWorldShape1.SetSensorSize(EBW8Image1.Width, EBW8Image1.Height);
                //EPointGauge1.Attach(EWorldShape1); //將LineGauge繫結到世界座標系統
                //EPointGauge1.TransitionType = ETransitionType.BwOrWb; //設定邊緣轉換類型，兩端剛好有Bw與Wb
                //EPointGauge1.SetCenterXY(element.BoundingBoxCenterX, element.BoundingBoxCenterY);
                EPointGauge.Tolerance = element.BoundingBoxHeight / 2 + 10;
                EPointGauge.ToleranceAngle = element.MinimumEnclosingRectangleAngle + 270;
                EPointGauge.Angle = element.MinimumEnclosingRectangleAngle + 270;//要看那一個角度量出來比較準，PS: 加90度居然無法量測，好怪
                                                                                 //EPointGauge1.Angle = element.EllipseAngle;
                EPointGauge.Measure(image);

                //EPointGauge.SetZoom(scalingRatio);
                //EPointGauge.Draw(g, EDrawingMode.Actual, true);
                //檢查有沒有取到兩個點
                if (EPointGauge.NumMeasuredPoints != 2) //如果量測到的point不到兩個，表示沒有量測到邊緣
                {
                    return null;
                }
                else
                {
                    tmpP1 = EPointGauge.GetMeasuredPoint(0);
                    tmpP2 = EPointGauge.GetMeasuredPoint(1);
                    tmpH = Math.Sqrt(Math.Pow(tmpP1.X - tmpP2.X, 2) + Math.Pow(tmpP1.Y - tmpP2.Y, 2));
                    return new EPoint((float)tmpW, (float)tmpH);
                }
            }
        }

        internal bool IsInSpecial1(float X, float Y)
        {
            float x_distance = Math.Abs(centerX - X);
            float y_distance = Math.Abs(centerY - Y);

            if (x_distance < (width / 2) && y_distance < (height / 2))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        internal void SaveInspectInfoSpecial1(ObjectShape objectStandard)
        {
            ObjectSpecial1 tempSpecial1 = (ObjectSpecial1)objectStandard;
            widthError = Math.Abs(width - tempSpecial1.width);
            heightError = Math.Abs(height - tempSpecial1.height);

            widthStd = tempSpecial1.width;
            heightStd = tempSpecial1.height;
        }

        internal bool InspectSpecial1(decimal thresholdNG)
        {
            if (widthError < (float)thresholdNG && heightError < (float)thresholdNG)
            {
                return true;
            }

            return false;
        }

    }



}