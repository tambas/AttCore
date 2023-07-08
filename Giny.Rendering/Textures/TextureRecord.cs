using Giny.IO.D2P;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Rendering.Textures
{
    public class TextureRecord
    {
        private const bool PixelInterpolation = true;

        private D2PEntry Entry
        {
            get;
            set;
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

        public int Id => int.Parse(Path.GetFileNameWithoutExtension(Name));

        public string Name => Entry.FileName;

        public TextureRecord(D2PEntry entry)
        {
            this.Entry = entry;
        }

        public void Load(D2PFile file)
        {
            Texture = new Texture(file.ReadFile(Entry));
            Texture.Smooth = PixelInterpolation;
        }

        public Sprite CreateSprite()
        {
            Sprite result = new Sprite(Texture);
            return result;
        }
    }
}
