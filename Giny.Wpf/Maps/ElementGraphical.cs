using Giny.Graphics.Gfx;
using Giny.IO.DLM.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Giny.Graphics.Maps
{
    public class ElementGraphical : Element
    {
        public int ElementId
        {
            get;
            set;
        }
        public Point Origin
        {
            get;
            set;
        }
        public int GfxId
        {
            get;
            set;
        }
        private ImageSource ImageSource
        {
            get;
            set;
        }
        private Rectangle Rectangle
        {
            get;
            set;
        }
        public bool HorizontalSymmetry
        {
            get;
            set;
        }
        public int Altitude
        {
            get;
            set;
        }
      
        public ElementGraphical(int elementId, Cell cell, LayerEnum layer, Point origin, int gfxId, ImageSource imageSource, bool horizontalSymmetry, Point pixelOffset, int altitude) : base(cell, layer, pixelOffset)
        {
            this.GfxId = gfxId;
            this.ElementId = elementId;
            this.Origin = origin;
            this.HorizontalSymmetry = horizontalSymmetry;
            this.ImageSource = imageSource;
            this.Altitude = altitude;
        }

        public void Refresh()
        {
            var zIndex = 0;

            if (Rectangle != null)
            {
                zIndex = GetZIndex();
                Cell.Map.Canvas.Children.Remove(Rectangle);
            }
            else
            {
                zIndex = Cell.GetZIndex(Layer);
            }
            Draw(zIndex);
        }
        public static Point ComputePosition(Point cellCenter, Point origin, Point pixelOffset, int altitude)
        {
            var finalPosition = new Point((cellCenter.X), (cellCenter.Y));

            finalPosition.X -= origin.X;
            finalPosition.Y -= origin.Y;

            finalPosition.X += pixelOffset.X;
            finalPosition.Y += pixelOffset.Y;

            if (altitude != 0)
            {
                finalPosition.Y = finalPosition.Y + Math.Round(Constants.CELL_HALF_HEIGHT - (altitude * 10) + pixelOffset.Y) - Constants.CELL_HALF_HEIGHT;
            }
            return finalPosition;
          
        }
        public override void Draw(int zIndex)
        {
            this.Rectangle = new Rectangle();
            Rectangle.Fill = new ImageBrush(ImageSource);

            Rectangle.Width = ImageSource.Width;
            Rectangle.Height = ImageSource.Height;

            var cellCenter = Cell.Center;

            var finalPosition = ComputePosition(cellCenter, Origin, PixelOffset, Altitude);

            if (HorizontalSymmetry)
            {
                Rectangle.Fill.RelativeTransform = new ScaleTransform(-1, 1, 0.5f, 0.5f);
            }

            Canvas.SetLeft(Rectangle, finalPosition.X);
            Canvas.SetTop(Rectangle, finalPosition.Y);

            Cell.Map.Canvas.Children.Add(Rectangle);

            Canvas.SetZIndex(Rectangle, zIndex);
        }

        public override int GetZIndex()
        {
            return Rectangle == null ? -1 : Canvas.GetZIndex(Rectangle);
        }

        public override void Destroy()
        {
            Cell.Map.Canvas.Children.Remove(Rectangle);
        }

        public override BasicElement GetBasicElement()
        {
            GraphicalElement graphicalElement = new GraphicalElement()
            {
                Altitude = Altitude,
                ElementId = (uint)ElementId,
                Hue1 = 0,
                Hue2 = 0,
                Hue3 = 0,
                Identifier = 0,
                OffsetX = 0,
                OffsetY = 0,
                PixelOffsetX = (int)PixelOffset.X,
                PixelOffsetY = (int)PixelOffset.Y,
                Shadow1 = 0,
                Shadow2 = 0,
                Shadow3 = 0,
            };
            return graphicalElement;
        }
    }
}
