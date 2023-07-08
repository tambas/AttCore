using Giny.Maps.SFML;
using Giny.Maps.Textures;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DlmLayer = Giny.IO.DLM.Layer;

namespace Giny.Maps.Maps
{
    public class Layer : IDrawable
    {
        public Layer()
        {
            this.Elements = new SortedDictionary<Cell, GraphicalElement>();
        }
        private SortedDictionary<Cell, GraphicalElement> Elements
        {
            get;
            set;
        }

        public void AddElement(Cell cell, TextureRecord record, Vector2f scale)
        {
            GraphicalElement element = new GraphicalElement(cell.GetElementPosition(record.Texture), record, scale);

            if (Elements.ContainsKey(cell))
            {
                Elements[cell].Dispose();
                Elements[cell] = element;
            }
            else
            {
                Elements.Add(cell, element);
            }
        }

        public void RemoveElement(Cell cell)
        {
            if (Elements.ContainsKey(cell))
            {
                Elements[cell].Dispose();
                Elements.Remove(cell);
            }
        }

        public void Draw(RenderWindow window)
        {
            foreach (var element in Elements.Values)
            {
                element.Draw(window);
            }
        }

        public DlmLayer GetRecord()
        {
            return new DlmLayer()
            {
                 
            };
        }

        public GraphicalElement GetElement(Cell cell)
        {
            if (Elements.ContainsKey(cell))
                return Elements[cell];
            else
                return null;
        }
    }
}
