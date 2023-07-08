using Giny.IO.DLM;
using Giny.Maps.SFML;
using Giny.Maps.Textures;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Maps.Maps
{
    public class Map : IDrawable
    {
        private const int MapWidth = 14;

        private const int MapHeight = 20;

        private const int CellWidth = 86;

        private const int CellHeigth = 43;

        public static Vector2f PixelSize = new Vector2f(MapWidth * CellWidth, MapHeight * CellHeigth);

        private static LayerEnum[] LayersValues = (LayerEnum[])Enum.GetValues(typeof(LayerEnum));

        public Vector2f Position
        {
            get;
            private set;
        }


        public int MusicId
        {
            get;
            private set;
        }
        public bool DisplayBorders
        {
            get;
            set;
        }

        public Cell GetCell(int id)
        {
            return Cells[id];
        }
        /// <summary>
        /// Get Cell from Pixel position
        /// </summary>
        /// <param name="position">Pixel position</param>
        public Cell GetCell(Vector2f position)
        {
            return Cells.FirstOrDefault(x => x.Contains(position));
        }

        public float ZoomScale
        {
            get;
            private set;
        }
        public Cell[] Cells
        {
            get;
            private set;
        }
        public Dictionary<LayerEnum, Layer> Layers
        {
            get;
            private set;
        }
        private VertexBuffer GridBuffer
        {
            get;
            set;
        }

        public Map(Vector2f position)
        {
            this.Position = position;
            CreateCells();
            Build();
        }
        public void AddElement(LayerEnum layer, int cellId, string spriteName, Vector2f scale)
        {
            TextureRecord record = TextureManager.GetTextureRecord(spriteName);

            if (record == null)
            {
                throw new Exception("Unable to find texture record! ");
            }

            Cell cell = GetCell(cellId);

            if (!Layers.ContainsKey(layer))
            {
                Layers.Add(layer, new Layer());
            }

            Layers[layer].AddElement(cell, record, scale);
        }
        public GraphicalElement GetElement(LayerEnum layer, int cellId)
        {
            if (!Layers.ContainsKey(layer))
            {
                return null;
            }
            Cell cell = GetCell(cellId);
            return Layers[layer].GetElement(cell);
        }
        public void RemoveElement(LayerEnum layer, int cellId)
        {
            if (Layers.ContainsKey(layer))
            {
                Cell cell = GetCell(cellId);

                Layers[layer].RemoveElement(cell);
            }
            else
            {
                throw new Exception("Unable to remove element, Unknown layer.");
            }
        }
        public void Draw(RenderWindow window, LayerEnum layersEnum)
        {
            View view = window.GetView();

            foreach (var layer in Layers.Where(x => x.Key < LayerEnum.LAYER_DECOR))
            {
                if (layersEnum.HasFlag(layer.Key))
                {
                    layer.Value.Draw(window);
                }
            }

            RectangleShape rectangle = new RectangleShape(PixelSize);
            rectangle.Position = Position;
            rectangle.OutlineColor = Color.Blue;
            rectangle.OutlineThickness = 3f;

            window.Draw(rectangle);
            if (DisplayBorders)
            {
                GridBuffer.Draw(window, RenderStates.Default);
            }

            foreach (var layer in Layers.Where(x => x.Key >= LayerEnum.LAYER_DECOR))
            {
                if (layersEnum.HasFlag(layer.Key))
                {
                    layer.Value.Draw(window);
                }
            }
        }
        /// <summary>
        /// Redondance, voir si HasFlag reduit les performances.
        /// </summary>
        /// <param name="window"></param>
        public void Draw(RenderWindow window)
        {
            Draw(window, LayerEnum.LAYER_GROUND | LayerEnum.LAYER_ADDITIONAL_GROUND | LayerEnum.LAYER_DECOR | LayerEnum.LAYER_ADDITIONAL_DECOR);
        }
        private void CreateCells()
        {
            this.Cells = new Cell[MapWidth * MapHeight * 2];

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new Cell(this, i);
            }

            this.Layers = new Dictionary<LayerEnum, Layer>();

            ToogleBorders(true);
        }

        public void ToogleBorders(bool display)
        {
            DisplayBorders = display;
        }

        private void Build()
        {
            int cellId = 0;
            float cellWidth = CellWidth;
            float cellHeight = CellHeigth;
            float offsetX = Position.X;
            float offsetY = Position.Y;

            float midCellHeight = cellHeight / 2;
            float midCellWidth = cellWidth / 2;

            for (float y = 0; y < (2 * MapHeight); y += 1)
            {
                if (y % 2 == 0)
                {
                    for (int x = 0; x < MapWidth; x++)
                    {
                        var left = new Vector2f(offsetX + x * cellWidth, offsetY + y * midCellHeight + midCellHeight);
                        var top = new Vector2f(offsetX + x * cellWidth + midCellWidth, offsetY + y * midCellHeight);
                        var right = new Vector2f(offsetX + x * cellWidth + cellWidth, offsetY + y * midCellHeight + midCellHeight);
                        var down = new Vector2f(offsetX + x * cellWidth + midCellWidth, offsetY + y * midCellHeight + cellHeight);

                        Cells[cellId++].SetPoints(new[] { left, top, right, down });
                    }
                }
                else
                {
                    for (int x = 0; x < MapWidth; x++)
                    {
                        var left = new Vector2f(offsetX + x * cellWidth + midCellWidth, offsetY + y * midCellHeight + midCellHeight);
                        var top = new Vector2f(offsetX + x * cellWidth + cellWidth, offsetY + y * midCellHeight);
                        var right = new Vector2f(offsetX + x * cellWidth + cellWidth + midCellWidth, offsetY + y * midCellHeight + midCellHeight);
                        var down = new Vector2f(offsetX + x * cellWidth + cellWidth, offsetY + y * midCellHeight + cellHeight);


                        Cells[cellId++].SetPoints(new[] { left, top, right, down });
                    }
                }
            }


            foreach (var cell in Cells)
            {
                cell.ComputePolygon();

            }

            this.GridBuffer = new VertexBuffer(Cell.VerticesCount * (uint)Cells.Count(), PrimitiveType.Lines, VertexBuffer.UsageSpecifier.Static);

            uint i = 0;

            foreach (var cell in Cells)
            {
                Vertex[] cellVertices = cell.GetLineVertices();
                this.GridBuffer.Update(cellVertices, (uint)cellVertices.Length, i);
                i += (uint)cellVertices.Count();
            }
        }

        public DlmMap ToDLM()
        {
            return new DlmMap()
            {

            };
        }


    }
}
