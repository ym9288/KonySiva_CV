using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmgucvDemo
{
    public class ImageHelper
    {
        public static Mat Load(string imagePath)
        {
            return CvInvoke.Imread(imagePath, Emgu.CV.CvEnum.ImreadModes.AnyColor);
        }
    }
}
