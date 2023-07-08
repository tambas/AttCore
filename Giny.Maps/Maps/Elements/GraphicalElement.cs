using Giny.IO.ELE;
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
    public class GraphicalElement : IDisposable, IDrawable
    {
        private TextureRecord TextureRecord
        {
            get;
            set;
        }
        public string SpriteName
        {
            get
            {
                return TextureRecord.Name;
            }
        }
        public Sprite Sprite
        {
            get;
            private set;
        }
        public GraphicalElement(Vector2f position, TextureRecord record, Vector2f scale)
        {
            TextureRecord = record;
            Sprite = TextureRecord.CreateSprite();
            Sprite.Position = position;
            Sprite.Scale = scale;
        }
        public GraphicalElement()
        {

        }

        public GraphicalElement ToGraphicalElement()
        {
            return new GraphicalElement()
            {
               
            };
        }

        public void Dispose()
        {
            Sprite.Dispose();
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(Sprite);
        }
    }
}
