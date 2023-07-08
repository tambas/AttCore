using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Maps.Textures
{
    public class TextureRecord
    {
        private const bool PixelInterpolation = false;

        public string Path
        {
            get;
            set;
        }
        public string Name
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }
        }
        public bool Loaded
        {
            get
            {
                return Texture != null;
            }
        }
        public Texture Texture
        {
            get;
            private set;
        }

        public TextureRecord(string path)
        {
            this.Path = path;
        }

        public void Load()
        {
            Texture = new Texture(Path);
            Texture.Smooth = PixelInterpolation;
        }

        public Sprite CreateSprite()
        {
            Sprite result = new Sprite(Texture);
            result.Origin = new Vector2f(result.TextureRect.Width / 2, result.TextureRect.Height / 2);
            return result;
        }
    }
}
