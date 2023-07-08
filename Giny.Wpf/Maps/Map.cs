using Giny.Graphics.Gfx;
using Giny.Graphics.Misc;
using Giny.IO.DLM;
using Giny.IO.DLM.Elements;
using Giny.IO.ELE;
using Giny.IO.ELE.Repertory;
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
    public class Map
    {
        /// <summary>
        /// Il ne peut pas y avoir plus de 20 000 element par layer. ( ça reste raisonable :3 )
        /// </summary>
        public static Dictionary<LayerEnum, int> DEFAULT_Z_INDEXES = new Dictionary<LayerEnum, int>()
        {
            {LayerEnum.LAYER_GROUND,0 },
            {LayerEnum.LAYER_ADDITIONAL_GROUND,20000 },
            {LayerEnum.LAYER_DECOR,40000 },
            {LayerEnum.LAYER_ADDITIONAL_DECOR,60000 },
        };

        public const int GRID_ZINDEX = 39999;

        public static Color DEFAULT_GRID_COLOR = Color.FromArgb(40, 0, 0, 0);

        public static Color DEFAULT_BACKGROUND_COLOR = Colors.White;

        public int Id
        {
            get;
            set;
        }
        public int Top
        {
            get;
            set;
        }
        public int Bottom
        {
            get;
            set;
        }
        public int Right
        {
            get;
            set;
        }
        public int Left
        {
            get;
            set;
        }
        public short SubareaId
        {
            get;
            set;
        }
        public Point Position
        {
            get
            {
                return new Point((CanvasWidth - ((CellCountX + 0.5) * CellWidth)) / 2,
                    (CanvasHeight - ((CellCountY + 0.5) * CellHeight)) / 2);
            }
        }
        public Point Center
        {
            get
            {
                return new Point(Position.X + Width / 2, Position.Y + Height / 2);
            }
        }
        public int CellCountY
        {
            get;
            set;
        }
        public int CellCountX
        {
            get;
            set;
        }
        public double Width
        {
            get
            {
                return CellCountX * CellWidth;
            }
        }
        public double Height
        {
            get
            {
                return CellCountY * CellHeight;
            }
        }
        public double CanvasWidth
        {
            get
            {
                return Canvas.Width;
            }
        }
        public double CanvasHeight
        {
            get
            {
                return Canvas.Height;
            }
        }
        public double CellHeight
        {
            get;
            private set;
        }

        public double CellWidth
        {
            get;
            private set;
        }
        public Canvas Canvas
        {
            get;
            private set;
        }
        public Color GridColor
        {
            get;
            set;
        }
        public Color BackgroundColor
        {
            get;
            set;
        }
        public sbyte Version
        {
            get;
            set;
        }
        private List<Fixture> BackgroundFixures /* todo : display it in world editor and edit it? */
        {
            get;
            set;
        }
        private List<Fixture> ForegroundFixure
        {
            get;
            set;
        }
        public event Action<Cell> MouseEnter;
        public event Action<Cell> MouseLeave;
        public event Action<Cell, MouseButtonEnum> MouseClick;

        public Map(Canvas canvas, sbyte version)
        {
            this.CellCountY = Constants.MAP_HEIGHT;
            this.CellCountX = Constants.MAP_WIDTH;
            this.Canvas = canvas;
            this.Version = version;
            this.CellWidth = Constants.CELL_WIDTH;
            this.CellHeight = Constants.CELL_HEIGHT;
            this.BuildCells();
            this.BuildMap();
            this.BindEvents();
            GridColor = DEFAULT_GRID_COLOR;
            BackgroundColor = DEFAULT_BACKGROUND_COLOR;
            this.BackgroundFixures = new List<Fixture>();
            this.ForegroundFixure = new List<Fixture>();
        }
        private void BuildCells()
        {
            Cells = new Cell[Constants.MAP_WIDTH * Constants.MAP_HEIGHT * 2];

            short cellId = 0;
            for (int y = 0; y < CellCountY; y++)
            {
                for (int x = 0; x < CellCountX * 2; x++)
                {
                    var cell = new Cell(this, cellId++);
                    Cells[cell.Id] = cell;
                }
            }
        }
        public void ChangeGridColor(Color color)
        {
            foreach (var cell in Cells)
            {
                cell.GetPolygon().Stroke = new SolidColorBrush(color);
            }
            GridColor = color;
        }
        private void BindEvents()
        {
            foreach (var cell in Cells)
            {
                cell.MouseClick += Cell_MouseClick;
                cell.MouseEnter += Cell_MouseEnter;
                cell.MouseLeave += Cell_MouseLeave;
            }
        }

        private void Cell_MouseLeave(Cell obj)
        {
            MouseLeave?.Invoke(obj);
        }

        private void Cell_MouseEnter(Cell obj)
        {
            MouseEnter?.Invoke(obj);
        }

        private void Cell_MouseClick(Cell arg1, MouseButtonEnum arg2)
        {
            MouseClick?.Invoke(arg1, arg2);
        }

        public void BuildMap()
        {
            int cellId = 0;
            double cellWidth = Constants.CELL_WIDTH;
            double cellHeight = Constants.CELL_HEIGHT;
            double offsetX = Position.X;
            double offsetY = Position.Y;

            double midCellHeight = cellHeight / 2;
            double midCellWidth = cellWidth / 2;

            for (double y = 0; y < (2 * CellCountY); y += 1)
            {
                if (y % 2 == 0)
                    for (int x = 0; x < CellCountX; x++)
                    {
                        var left = new Point(offsetX + x * cellWidth, offsetY + y * midCellHeight + midCellHeight);
                        var top = new Point(offsetX + x * cellWidth + midCellWidth, offsetY + y * midCellHeight);
                        var right = new Point(offsetX + x * cellWidth + cellWidth, offsetY + y * midCellHeight + midCellHeight);
                        var down = new Point(offsetX + x * cellWidth + midCellWidth, offsetY + y * midCellHeight + cellHeight);

                        Cells[cellId++].SetPoints(new[] { left, top, right, down });
                    }
                else
                    for (int x = 0; x < CellCountX; x++)
                    {
                        var left = new Point(offsetX + x * cellWidth + midCellWidth, offsetY + y * midCellHeight + midCellHeight);
                        var top = new Point(offsetX + x * cellWidth + cellWidth, offsetY + y * midCellHeight);
                        var right = new Point(offsetX + x * cellWidth + cellWidth + midCellWidth, offsetY + y * midCellHeight + midCellHeight);
                        var down = new Point(offsetX + x * cellWidth + cellWidth, offsetY + y * midCellHeight + cellHeight);


                        Cells[cellId++].SetPoints(new[] { left, top, right, down });
                    }
            }
            CellWidth = cellWidth;
            CellHeight = cellHeight;

        }
        bool PointInPoly(Point p, Point[] poly)
        {
            double xnew, ynew;
            double xold, yold;
            double x1, y1;
            double x2, y2;
            bool inside = false;

            if (poly.Length < 3)
                return false;

            xold = poly[poly.Length - 1].X;
            yold = poly[poly.Length - 1].Y;

            foreach (Point t in poly)
            {
                xnew = t.X;
                ynew = t.Y;

                if (xnew > xold)
                {
                    x1 = xold;
                    x2 = xnew;
                    y1 = yold;
                    y2 = ynew;
                }
                else
                {
                    x1 = xnew;
                    x2 = xold;
                    y1 = ynew;
                    y2 = yold;
                }

                if ((xnew < p.X) == (p.X <= xold) && (p.Y - (long)y1) * (x2 - x1) < (y2 - (long)y1) * (p.X - x1))
                {
                    inside = !inside;
                }
                xold = xnew;
                yold = ynew;
            }
            return inside;
        }
        public Cell GetCell(short id)
        {
            return Cells[id];
        }
        public bool IsPointOnBounds(Point p)
        {
            if (p.X <= 0 || p.Y <= 0 || p.X >= CanvasWidth || p.Y >= CanvasHeight)
                return false;
            return true;
        }
        public Cell[] Cells
        {
            get;
            set;
        }
        public void Draw(params LayerEnum[] layers)
        {
            if (layers.Contains(LayerEnum.LAYER_GROUND))
                DrawLayer(LayerEnum.LAYER_GROUND);

            if (layers.Contains(LayerEnum.LAYER_ADDITIONAL_GROUND))
                DrawLayer(LayerEnum.LAYER_ADDITIONAL_GROUND);

            foreach (var cell in Cells)
            {
                cell.DrawPolygon();
            }

            if (layers.Contains(LayerEnum.LAYER_DECOR))
                DrawLayer(LayerEnum.LAYER_DECOR);
            if (layers.Contains(LayerEnum.LAYER_ADDITIONAL_DECOR))
                DrawLayer(LayerEnum.LAYER_ADDITIONAL_DECOR);

            foreach (var cell in Cells)
            {
                cell.DrawEventPolygon();
            }
        }
        private void DrawLayer(LayerEnum layer)
        {
            int zIndex = Map.DEFAULT_Z_INDEXES[layer];

            foreach (var cell in Cells)
            {
                cell.DrawLayer(layer, ref zIndex);
            }
        }
        public void ToogleGrid(bool display)
        {
            if (display)
            {
                foreach (var cell in Cells)
                {
                    cell.DrawPolygon();
                }
            }
            else
            {
                foreach (var cell in Cells)
                {
                    cell.DestroyPolygon();
                }
            }
        }
        private LayerEnum[] GetConsistantLayers()
        {
            List<LayerEnum> result = new List<LayerEnum>();

            foreach (LayerEnum layer in Enum.GetValues(typeof(LayerEnum)))
            {
                foreach (var cell in Cells)
                {
                    if (cell.IsLayerConsistant(layer))
                    {
                        if (!result.Contains(layer))
                            result.Add(layer);
                    }
                }
            }
            return result.ToArray();
        }
        private Cell[] DispatchCells(LayerEnum layer)
        {
            List<Cell> result = new List<Cell>();

            foreach (var cell in Cells)
            {
                if (cell.IsLayerConsistant(layer))
                {
                    result.Add(cell);
                }
            }
            return result.ToArray();
        }
        public DlmMap ToDLM()
        {
            DlmMap map = new DlmMap()
            {
                MapVersion = Version,
                Id = Id,
                TopNeighbourId = Top,
                BottomNeighbourId = Bottom,
                RightNeighbourId = Right,
                LeftNeighbourId = Left,
                SubareaId = SubareaId,
                BackgroundFixtures = BackgroundFixures,
                ForegroundFixtures = ForegroundFixure,
                BackgroundAlpha = BackgroundColor.A,
                BackgroundBlue = BackgroundColor.B,
                BackgroundGreen = BackgroundColor.G,
                BackgroundRed = BackgroundColor.R,

                GridAlpha = GridColor.A,
                GridBlue = GridColor.B,
                GridGreen = GridColor.G,
                GridRed = GridColor.R,
            };

            map.Layers = new List<Layer>();

            foreach (LayerEnum layerEnum in Enum.GetValues(typeof(LayerEnum)))
            {
                Layer layer = new Layer();
                layer.Cells = new List<DlmCell>();
                layer.LayerId = (int)layerEnum;

                foreach (var cell in Cells)
                {
                    DlmCell dlmCell = new DlmCell();
                    dlmCell.CellId = cell.Id;
                    dlmCell.Elements = new List<BasicElement>();

                    foreach (var element in cell.GetElements()[layerEnum])
                    {

                        dlmCell.Elements.Add(element.GetBasicElement());
                    }

                    layer.Cells.Add(dlmCell);
                }

                map.Layers.Add(layer);
            }

            map.Cells = new CellData[Constants.MAP_WIDTH * Constants.MAP_HEIGHT * 2];

            for (short i = 0; i < map.Cells.Length; i++)
            {
                map.Cells[i] = this.GetCell(i).Data;
            }
            return map;
        }
        public static Map FromDLM(Canvas canvas, DlmMap dlmMap, Dictionary<int, EleGraphicalData> elements)
        {
            Map map = new Map(canvas, dlmMap.MapVersion);

            map.Id = dlmMap.Id;
            map.Top = dlmMap.TopNeighbourId;
            map.Left = dlmMap.LeftNeighbourId;
            map.Right = dlmMap.RightNeighbourId;
            map.Bottom = dlmMap.BottomNeighbourId;
            map.SubareaId = dlmMap.SubareaId;
            map.BackgroundFixures = dlmMap.BackgroundFixtures;
            map.ForegroundFixure = dlmMap.ForegroundFixtures;
            map.BackgroundColor = Color.FromArgb((byte)dlmMap.BackgroundAlpha, (byte)dlmMap.BackgroundRed, (byte)dlmMap.BackgroundGreen, (byte)dlmMap.BackgroundBlue);

            foreach (var dlmLayer in dlmMap.Layers)
            {
                foreach (var dlmCell in dlmLayer.Cells)
                {
                    foreach (var dlmElement in dlmCell.Elements.OfType<GraphicalElement>())
                    {
                        EleGraphicalData element = null;

                        if (!elements.TryGetValue((int)dlmElement.ElementId, out element))
                        {
                            throw new Exception("Cannot find element: " + dlmElement.ElementId);
                        }

                        var graphicalData = element as NormalGraphicalElementData;

                        if (graphicalData != null)
                        {
                            if (GfxManager.GfxExists(graphicalData.Gfx))
                            {
                                var cell = map.GetCell(dlmCell.CellId);
                                cell.AddElement(dlmLayer.LayerId, dlmElement, graphicalData);
                            }
                            else
                            {
                                Console.WriteLine("Unknown gfx " + graphicalData.Gfx);
                            }
                        }
                    }
                }
            }

            foreach (var cellData in dlmMap.Cells)
            {
                map.GetCell(cellData.Id).Data = cellData;
            }
            return map;
        }

    }
}
