using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Graphics.Maps
{
    public struct CellRectangle
    {
        public double X;

        public double Y;

        public double Width;

        public double Height;

        public CellRectangle(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
    }
}
