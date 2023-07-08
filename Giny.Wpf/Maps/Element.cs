using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Giny.IO.DLM.Elements;
using Giny.IO.ELE.Repertory;

namespace Giny.Graphics.Maps
{
    public abstract class Element
    {
        protected Cell Cell
        {
            get;
            private set;
        }

        protected Point PixelOffset
        {
            get;
            private set;
        }
        public LayerEnum Layer
        {
            get;
            set;
        }
        public Element(Cell cell, LayerEnum layer, Point pixelOffset)
        {
            this.PixelOffset = pixelOffset;
            this.Layer = layer;
            this.Cell = cell;
        }

        public abstract int GetZIndex();

        public abstract void Draw(int zIndex);

        public abstract void Destroy();

        public abstract BasicElement GetBasicElement();

    }
}

