using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Giny.Graphics.Gfx;
using Giny.Graphics.Misc;
using Giny.IO.DLM;
using Giny.IO.DLM.Elements;
using Giny.IO.ELE.Repertory;

namespace Giny.Graphics.Maps
{
    public class Cell
    {
        Dictionary<LayerEnum, List<Element>> Elements = new Dictionary<LayerEnum, List<Element>>();

        public CellData Data
        {
            get;
            set;
        }
        private Polygon Polygon
        {
            get;
            set;
        }
        private Polygon EventPolygon
        {
            get;
            set;
        }
        private CellRectangle Rectangle
        {
            get;
            set;
        }

        public short Id
        {
            get;
            set;
        }

        private Label Label
        {
            get;
            set;
        }


        public Point[] VerticalDiagonal
        {
            get
            {
                return new Point[] { Points[1], Points[3] };
            }
        }
        public Point[] HorizontalDiagonal
        {
            get
            {
                return new Point[] { Points[0], Points[2] };
            }
        }
        public Point Center
        {
            get
            {
                return new Point((Points[0].X + Points[2].X) / 2, (Points[1].Y + Points[3].Y) / 2);
            }
        }
        public Point Position
        {
            get
            {
                return Points[0];
            }
        }
        public Point[] Points
        {
            get;
            private set;
        }
        public Map Map
        {
            get;
            set;
        }

        public int GetZIndex(LayerEnum layerEnum)
        {
            var element = Elements[layerEnum].LastOrDefault(x => x.GetZIndex() != -1);

            if (element == null)
            {
                return Map.DEFAULT_Z_INDEXES[layerEnum] + Id;
            }
            return element.GetZIndex() + 1;
        }

        public event Action<Cell, MouseButtonEnum> MouseClick;

        public event Action<Cell> MouseEnter;

        public event Action<Cell> MouseLeave;

        public Cell(Map map, short id)
        {
            Id = id;
            this.Map = map;

            Elements.Add(LayerEnum.LAYER_GROUND, new List<Element>());
            Elements.Add(LayerEnum.LAYER_ADDITIONAL_GROUND, new List<Element>());
            Elements.Add(LayerEnum.LAYER_DECOR, new List<Element>());
            Elements.Add(LayerEnum.LAYER_ADDITIONAL_DECOR, new List<Element>());

            Data = new CellData()
            {
                Mov = true, // walkable by default
            };
        }

        public void RemoveElement(LayerEnum drawingLayer)
        {
            var element = Elements[drawingLayer].LastOrDefault();

            if (element != null)
            {
                element.Destroy();
            }
            Elements[drawingLayer].Remove(element);
        }

        public Dictionary<LayerEnum, List<Element>> GetElements()
        {
            return Elements;
        }
        public bool IsLayerConsistant(LayerEnum layer)
        {
            return Elements[layer].Count > 0;
        }
        public void AddElement(int elementId, LayerEnum layer, int gfxId, Point origin, Point pixelOffset, bool horizontalSymetry)
        {
            var imageSource = GfxManager.GetImageSource(gfxId);
            Elements[layer].Add(new ElementGraphical(elementId, this, layer, origin, gfxId, imageSource, horizontalSymetry, pixelOffset, 0));
        }
        public void ComputeRectangle()
        {
            double x = Points.Min(entry => entry.X);
            double y = Points.Min(entry => entry.Y);

            double width = Points.Max(entry => entry.X) - x;
            double height = Points.Max(entry => entry.Y) - y;

            Rectangle = new CellRectangle(x, y, width, height);
        }

        public Polygon GetPolygon()
        {
            return Polygon;
        }

        public void Fill(Color color)
        {
            EventPolygon.Fill = new SolidColorBrush(color);
        }
        public void Stroke(Color color)
        {
            Polygon.Stroke = new SolidColorBrush(color);
        }
        public void Unstroke()
        {
            Polygon.Stroke = new SolidColorBrush(Map.GridColor);
        }
        public void SetThickness(float thickness)
        {
            Polygon.StrokeThickness = thickness;
        }
        public void ComputePolygon()
        {
            Polygon = new Polygon();
            Polygon.StrokeThickness = 1f;
            Polygon.Fill = Brushes.Transparent;
            Polygon.Stroke = new SolidColorBrush(Map.DEFAULT_GRID_COLOR);
            EventPolygon = new Polygon();
            EventPolygon.Fill = Brushes.Transparent;


            foreach (var point in Points)
            {
                Polygon.Points.Add(point);
                EventPolygon.Points.Add(point);
            }

            EventPolygon.MouseEnter += EventPolygon_MouseEnter;
            EventPolygon.MouseRightButtonDown += EventPolygon_MouseRightButtonDown;
            EventPolygon.MouseLeftButtonDown += EventPolygon_MouseLeftButtonDown;
            EventPolygon.MouseLeave += EventPolygon_MouseLeave;

        }

        private void EventPolygon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MouseLeave?.Invoke(this);
        }

        private void EventPolygon_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseClick?.Invoke(this, MouseButtonEnum.Left);
        }

        private void EventPolygon_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseClick?.Invoke(this, MouseButtonEnum.Right);
        }

        private void EventPolygon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MouseEnter?.Invoke(this);
        }

        public void SetPoints(Point[] points)
        {
            this.Points = points;
            ComputeRectangle();
            ComputePolygon();
        }

        public void DrawPolygon()
        {
            Panel.SetZIndex(Polygon, Map.GRID_ZINDEX);
            Map.Canvas.Children.Add(Polygon);
        }
        public void DestroyPolygon()
        {
            Map.Canvas.Children.Remove(Polygon);
        }
        public void SetText(string text)
        {
            if (Label != null)
            {
                Map.Canvas.Children.Remove(Label);
            }

            this.Label = new Label();
            Label.Content = text;
            Label.Foreground = Brushes.Black;
            Label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(Label, Center.X - Label.DesiredSize.Width / 2);
            Canvas.SetTop(Label, Center.Y - Label.DesiredSize.Height / 2);
            Map.Canvas.Children.Add(Label);


        }
        public void Refresh(LayerEnum[] displayedLayer)
        {
            foreach (var pair in Elements)
            {
                if (displayedLayer.Contains(pair.Key))
                {
                    foreach (var element in Elements[pair.Key].OfType<ElementGraphical>())
                    {
                        element.Refresh();
                    }
                }
            }
        }
        public void DrawLayer(LayerEnum layer, ref int zIndex)
        {

            foreach (var element in Elements[layer])
            {
                zIndex++;
                element.Draw(zIndex);
            }
        }


        public void AddElement(int layerId, GraphicalElement graphicalElement, NormalGraphicalElementData graphicalData)
        {
            Elements[(LayerEnum)layerId].Add(new ElementGraphical((int)graphicalData.Id, this, (LayerEnum)layerId, new Point(graphicalData.OriginX, graphicalData.OriginY),
                graphicalData.Gfx, GfxManager.GetImageSource(graphicalData.Gfx), graphicalData.HorizontalSymmetry, new Point(graphicalElement.PixelOffsetX, graphicalElement.PixelOffsetY),
                graphicalElement.Altitude));
        }

        public void DrawEventPolygon()
        {
            Panel.SetZIndex(EventPolygon, int.MaxValue);
            Map.Canvas.Children.Add(EventPolygon);
        }

        public void Unfill()
        {
            Fill(Colors.Transparent);
        }
    }
}
