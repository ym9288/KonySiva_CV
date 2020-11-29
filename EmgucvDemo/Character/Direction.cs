using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmgucvDemo
{
    public enum Direction
    {
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        LeftTop = 16,
        LeftBottom = 32,
        RightTop = 64,
        RightBottom = 128
    }
}
