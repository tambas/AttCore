using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Maps.Textures
{
    public class TextureManager
    {
        private static readonly Dictionary<string, TextureRecord> m_textures = new Dictionary<string, TextureRecord>();

        private static string m_path;

        public static void Initialize(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            m_path = path;

            foreach (var file in Directory.EnumerateFiles(GetPath(), "*.*", SearchOption.AllDirectories))
            {
                AddTexture(file);
            }


            Console.WriteLine(m_textures.Count + " sprites loaded.");
        }
        private static void AddTexture(string filePath)
        {
            TextureRecord sprite = new TextureRecord(filePath);
            string name = Path.GetFileNameWithoutExtension(filePath);
            m_textures.Add(name, sprite);
        }
        public static string GetPath()
        {
            return m_path;
        }
        public static IEnumerable<TextureRecord> GetTextureRecords()
        {
            return m_textures.Values;
        }
        public static TextureRecord GetTextureRecord(string name)
        {
            TextureRecord textureRecord = null;

            if (!m_textures.TryGetValue(name, out textureRecord))
            {
                return null;
            }
            else
            {
                if (!textureRecord.Loaded)
                {
                    textureRecord.Load();
                }

                return textureRecord;
            }
        }
    }
}
