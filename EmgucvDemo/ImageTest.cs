﻿using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmgucvDemo
{
   public class ImageTest
    {
        public void Load(string fileName)
        {
            Mat img1 = CvInvoke.Imread(fileName, Emgu.CV.CvEnum.ImreadModes.Grayscale);
            Mat img2 = CvInvoke.Imread(fileName, Emgu.CV.CvEnum.ImreadModes.Color);
            Mat img3 = CvInvoke.Imread(fileName, Emgu.CV.CvEnum.ImreadModes.AnyColor);
            Mat img4 = CvInvoke.Imread(fileName, Emgu.CV.CvEnum.ImreadModes.AnyDepth);
        }

        public int GetMostGrayValue(Mat input)
        {
            Mat temp = input;
            if (input.NumberOfChannels > 1)
            {
                temp = new Mat(,)
            }
            int[] arry = new int[256];

        }

        public void ClearBackground(string fileName)
        {
            Mat img1 = CvInvoke.Imread(fileName, Emgu.CV.CvEnum.ImreadModes.Grayscale);
        }
    }
}