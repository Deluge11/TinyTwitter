using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp6
{
    class Grid
    {
        public int maxWidth { get; set; }
        public int maxHeight { get; set; }
        public string content { get; set; }

        public Grid(int w,int h,string cont)
        {
            maxWidth = w;
            maxHeight = h;
            content = cont;
        }
    }
}
