using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using EmgucvDemo.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EmgucvDemo
{
    public class WordDetect
    {
        static int kot = 20;//逼近阈值

        public static void ParseLines(string fileName)
        {
            var img = CvInvoke.Imread(fileName,Emgu.CV.CvEnum.ImreadModes.Grayscale);
            Mat edges = new Mat();
            CvInvoke.Canny(img, edges, 50, 150, 3);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(edges, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            CvInvoke.Imshow("edges", edges);
            for (int i = 0; i < contours.Size; i++)
            {
                var temp = contours[i].ToArray();
                int minX = temp.Min(m => m.X);
                int minY = temp.Min(m => m.Y);
                int maxX = temp.Max(m => m.X);
                int maxY = temp.Max(m => m.Y);

                Point radomPt = temp[0];
                double maxDistance = 0;
                List<Point> result = new List<Point>();
                Point? startPt = null, endPt = null;
                foreach(var pt1 in temp)
                    foreach(var pt2 in temp)
                    {
                        var res = GetDistance(pt1, pt2);
                        if (res > maxDistance)
                        {
                            maxDistance = res;
                            startPt = pt1;
                            endPt = pt2;
                        }
                    }
                maxDistance = 0;
                foreach (Point pt in temp)
                {
                    double res = DistanceForPointToABLine(pt, startPt.Value, endPt.Value);
                    if(res > maxDistance)
                    {
                        maxDistance = res;
                        if(kot < maxDistance)
                        {
                            result.Add(pt);
                        }
                    }
                }
                if (result.Count > 0)
                {
                    CvInvoke.Line(img, startPt.Value, result[0], new MCvScalar(0, 255, 0));
                    CvInvoke.Line(img, result[0], endPt.Value, new MCvScalar(0, 255, 0));
                }
                else
                    CvInvoke.Line(img, startPt.Value, endPt.Value, new MCvScalar(0, 255, 0));
                CvInvoke.Imshow("lines", img);
            }
        }
        public static double DistanceForPointToABLine(Point pt, Point pt1, Point pt2)//所在点到AB线段的垂线长度
        {
            double x = pt.X;
            double y = pt.Y;
            double x1 = pt1.X;
            double y1 = pt1.Y;
            double x2 = pt2.X;
            double y2 = pt2.Y;
            double reVal = 0f;
            bool retData = false;

            double cross = (x2 - x1) * (x - x1) + (y2 - y1) * (y - y1);
            if (cross <= 0)
            {
                reVal = (float)Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1));
                retData = true;
            }

            double d2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
            if (cross >= d2)
            {
                reVal = (float)Math.Sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2));
                retData = true;
            }

            if (!retData)
            {
                double r = cross / d2;
                double px = x1 + (x2 - x1) * r;
                double py = y1 + (y2 - y1) * r;
                reVal = (float)Math.Sqrt((x - px) * (x - px) + (py - y) * (py - y));
            }

            return reVal;

        }

        public static void GetLines(string fileName)
        {
            var img = CvInvoke.Imread(fileName);
            Mat gray = new Mat();
            Mat edges = new Mat();
            CvInvoke.CvtColor(img, gray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.Canny(gray, edges, 50, 150, 3);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(edges, contours, null, Emgu.CV.CvEnum.RetrType.External,Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            CvInvoke.Imshow("edges", edges);
            for(int i = 0; i < contours.Size; i++)
            {
                var temp = contours[i].ToArray();
                int minX = temp.Min(m => m.X);
                int minY = temp.Min(m => m.Y);

                int maxX = temp.Max(m => m.X);
                int maxY = temp.Max(m => m.Y);

                Point ptL, ptR,ptB,ptT,startPt,endPt;
                if (temp.Where(m => m.X > minX && m.X< minX+5).Count()/temp.Count() < 0.01)
                {
                    var ys = temp.Where(m => m.X == minX).Select(m => m.Y);
                    int minX_YValue = ys.Aggregate((a, b) => a + b) / ys.Count();
                    ptL = new Point(minX, minX_YValue);
                    if (temp.Where(m=>m.Y < maxY && m.Y > maxY-5).Count()/temp.Count() < 0.01)
                    {
                        //最下的点
                        var xs = temp.Where(m => m.Y == maxY).Select(m => m.X);
                        int maxY_XVlaue = xs.Aggregate((a, b) => a + b) / xs.Count();
                        ptB = new Point(maxY_XVlaue, maxY);
                        //最上的点
                        xs = temp.Where(m => m.Y == minY).Select(m => m.X);
                        int minY_XValue = xs.Aggregate((a, b) => a + b) / xs.Count();
                        ptT = new Point(minY_XValue, minY);
                        //最右的点
                        ys = temp.Where(m => m.X == maxX).Select(m => m.Y);
                        int maxX_YValue = ys.Aggregate((a, b) => a + b) / ys.Count();
                        ptR = new Point(maxX, maxX_YValue);
                        double minDistance = 0,tempDis = 0;
                        minDistance = tempDis = GetDistance(ptL, ptT);
                        tempDis = GetDistance(ptL, ptB);
                        if (tempDis < minDistance)
                        {
                            minDistance = tempDis;
                            startPt = new Point((ptL.X + ptB.X) / 2, (ptL.Y + ptB.Y) / 2);
                            endPt = new Point((ptR.X + ptT.X) / 2, (ptR.Y + ptT.Y) / 2);
                        }
                        else
                        {
                            startPt = new Point((ptL.X + ptT.X) / 2, (ptL.Y + ptT.Y) / 2);
                            endPt = new Point((ptR.X + ptB.X) / 2, (ptR.Y + ptB.Y) / 2);
                        }
                    }
                    else
                    {
                        //水平直线
                        startPt = new Point(minX, (minY+maxY)/2);
                        endPt = new Point(maxX, (minY + maxY) / 2);
                    }
                }
                else
                {
                    //垂直直线
                    startPt = new Point((minX + maxX) / 2, minY);
                    endPt = new Point((minX + maxX) / 2, maxY);
                }
                CvInvoke.Line(img, startPt, endPt, new MCvScalar(0,255, 0));

            }
            CvInvoke.Imshow("lines", img);
        }
        private static double GetDistance(Point pt1,Point pt2)
        {
            return Math.Sqrt((Math.Pow(Math.Abs(pt1.X - pt2.X), 2) + Math.Pow(Math.Abs(pt1.Y - pt2.Y), 2)));
        }
        public static void Invoke(Mat imgData)
        {
            Matrix<byte> data = new Matrix<byte>(imgData.Rows, imgData.Cols, 1);
            imgData.CopyTo(data);
            int temp = 0;
            Point? startPt = null,previousPt = null;
            List<Point> pts = new List<Point>();
            for(int h=0;h<data.Height;h+=3)
                for(int w = 0;w<data.Width;w+=3)
                {
                    temp = (int)data[h, w];
                    if (temp < 200)
                    {
                        if (!startPt.HasValue)
                        {
                            startPt = new Point(w, h);
                            previousPt = new Point(w, h);
                        }
                        else
                        {

                        }
                    }
                }
        }
    }

    public class WordBaseInfo
    {
        public static List<StrokeInfo> Strokes = new List<StrokeInfo>();
        static WordBaseInfo()
        {
            Strokes.AddRange(new List<StrokeInfo> {
            new StrokeInfo()
            {
                Descr = "点",
                OrientationSequence = new Direction[] { Direction.LeftTop,Direction.RightBottom},
                IsShort = true
            },
            new StrokeInfo()
            {
                Descr = "横",
                OrientationSequence = new Direction[] { Direction.Right, Direction.Right }
            },new StrokeInfo()
            {
                Descr = "竖",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.Bottom }
            },new StrokeInfo()
            {
                Descr = "撇",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.LeftBottom}
            },new StrokeInfo()
            {
                Descr = "捺",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.RightBottom}
            },new StrokeInfo()
            {
                Descr = "提",
                OrientationSequence = new Direction[] { Direction.LeftBottom, Direction.RightTop}
            },new StrokeInfo()
            {
                Descr = "撇点",
                OrientationSequence = new Direction[] { Direction.RightTop, Direction.LeftBottom, Direction.RightBottom }
            },new StrokeInfo()
            {
                Descr = "竖提",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.RightTop}
            },new StrokeInfo()
            {
                Descr = "横折提",
                OrientationSequence = new Direction[] { Direction.Right, Direction.Bottom, Direction .RightTop}
            },new StrokeInfo()
            {
                Descr = "竖钩",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.LeftTop}
            },new StrokeInfo()
            {
                Descr = "竖弯钩",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.Right, Direction.Top }
            },new StrokeInfo()
            {
                Descr = "斜钩",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.RightBottom, Direction.Top}
            },new StrokeInfo()
            {
                Descr = "横钩",
                OrientationSequence = new Direction[] { Direction.Right, Direction.LeftBottom}
            },new StrokeInfo()
            {
                Descr = "横折钩",
                OrientationSequence = new Direction[] { Direction.Right, Direction.Bottom, Direction.LeftTop}
            },new StrokeInfo()
            {
                Descr = "横折弯钩",
                OrientationSequence = new Direction[] { Direction.Right, Direction.Bottom, Direction.Right,Direction.Top}
            },new StrokeInfo()
            {
                Descr = "横撇弯钩",
                OrientationSequence = new Direction[] { Direction.Right, Direction.LeftBottom, Direction.RightBottom, Direction.LeftTop}
            },new StrokeInfo()
            {
                Descr = "横折折折钩",
                OrientationSequence = new Direction[] { Direction.Right, Direction.LeftBottom, Direction.Right, Direction.Bottom, Direction.LeftTop}
            },new StrokeInfo()
            {
                Descr = "竖弯",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.Right}
            },new StrokeInfo()
            {
                Descr = "横折弯",
                OrientationSequence = new Direction[] { Direction.Right, Direction.Bottom, Direction.Right}
            },new StrokeInfo()
            {
                Descr = "横折",
                OrientationSequence = new Direction[] { Direction.Right, Direction.Bottom }
            },new StrokeInfo()
            {
                Descr = "竖折",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.Right, Direction.Right }
            },new StrokeInfo()
            {
                Descr = "撇折",
                OrientationSequence = new Direction[] { Direction.LeftBottom,Direction.Right}
            },new StrokeInfo()
            {
                Descr = "横撇",
                OrientationSequence = new Direction[] { Direction.Right, Direction.LeftBottom, Direction.LeftBottom }
            },new StrokeInfo()
            {
                Descr = "横折折撇",
                OrientationSequence = new Direction[] { Direction.Right, Direction.LeftBottom, Direction.Right, Direction.LeftBottom }
            },
            new StrokeInfo()
            {
                Descr = "竖折撇",
                OrientationSequence = new Direction[] { Direction.Bottom, Direction.Right, Direction.LeftBottom }
            }
            });
        }
        public static void Invoke()
        {           
            Strokes.ForEach(m => {
                int cnt = Strokes.Count(x => CompareArray(x.OrientationSequence,m.OrientationSequence));
                if (cnt > 1)
                {
                    Console.WriteLine($"{m.Descr}");
                }
            });
        }
        public static bool CompareArray(Direction[] bt1, Direction[] bt2)
        {
            var len1 = bt1.Length;
            var len2 = bt2.Length;
            if (len1 != len2)
            {
                return false;
            }
            for (var i = 0; i < len1; i++)
            {
                if (bt1[i] != bt2[i])
                    return false;
            }
            return true;
        }
    }

}
