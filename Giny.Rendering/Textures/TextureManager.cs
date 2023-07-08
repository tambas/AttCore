using Giny.Core.DesignPattern;
using Giny.IO.D2P;
using Giny.Rendering.Textures;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Rendering.GFX
{
    public enum TextureType
    {
        Png,
        Jpg,
        Swf,
        Unknown,
    }
    public class TextureManager : Singleton<TextureManager>
    {
        private const string RelativePath = @"content\gfx\world\gfx0.d2p";

        private Dictionary<TextureType, Dictionary<int, TextureRecord>> m_cache = new Dictionary<TextureType, Dictionary<int, TextureRecord>>();

        private D2PFile File
        {
            get;
            set;
        }

        public void Initialize(string clientPath)
        {
            string path = Path.Combine(clientPath, RelativePath);
            D2PFile d2pFile = new D2PFile(path);
            this.File = d2pFile;

            m_cache.Add(TextureType.Png, new Dictionary<int, TextureRecord>());
            m_cache.Add(TextureType.Jpg, new Dictionary<int, TextureRecord>());
            m_cache.Add(TextureType.Swf, new Dictionary<int, TextureRecord>());
            m_cache.Add(TextureType.Unknown, new Dictionary<int, TextureRecord>());

            foreach (var entry in d2pFile.Entries)
            {
                TextureRecord record = new TextureRecord(entry);
                int id = int.Parse(Path.GetFileNameWithoutExtension(entry.FileName));
                var type = GetTextureType(entry);

                if (!m_cache[type].ContainsKey(id))
                {
                    m_cache[type].Add(id, record);
                }
            }
        }

        private TextureType GetTextureType(D2PEntry entry)
        {
            var extension = Path.GetExtension(entry.FileName).ToLower();

            switch (extension)
            {
                case ".png":
                    return TextureType.Png;
                case ".jpg":
                    return TextureType.Jpg;
                case ".swf":
                    return TextureType.Swf;
            }
            return TextureType.Unknown;

        }


        public TextureRecord GetTextureRecord(int id, TextureType type = TextureType.Png)
        {
            if (m_cache[type].ContainsKey(id))
            {
                var record = m_cache[type][id];

                if (!record.Loaded)
                {
                    record.Load(File);
                }

                return record;
            }
            else
            {
                return null;
            }
        }

        public bool Exist(int id, TextureType type = TextureType.Png)
        {
            return m_cache[type].ContainsKey(id);
        }

    }
}
