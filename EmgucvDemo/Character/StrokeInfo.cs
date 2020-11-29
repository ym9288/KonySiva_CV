using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmgucvDemo.Character
{
    public class StrokeInfo
    {
        public int StrokeId { get; set; }
        public Direction[] OrientationSequence { get; set; }
        public bool IsShort { get; set; }

        public string Descr { get; set; }
    }
}
