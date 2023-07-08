using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Giny.Graphics.Gfx
{
    public class FixtureManager
    {
        private const string FIXTURES_PATH = "JPG/";

        static Dictionary<int, string> m_fixtures = new Dictionary<int, string>();

        static Dictionary<int, ImageSource> m_loadedGfx = new Dictionary<int, ImageSource>();

        static string m_gfxPath;

        public static void Initialize(string gfxPath)
        {
            m_gfxPath = gfxPath;

            foreach (var file in Directory.EnumerateFiles(GetPngDirectory(), "*.*", SearchOption.AllDirectories))
            {
                int id = int.Parse(Path.GetFileNameWithoutExtension(file));
                m_fixtures.Add(id, file);
            }
        }
        public static ImageSource GetImageSource(int gfxId)
        {
            ImageSource value = null;

            if (m_loadedGfx.TryGetValue(gfxId, out value))
            {
                return value;
            }
            else
            {
                value = WpfUtils.GetImageSource(m_fixtures[gfxId]);
                m_loadedGfx.Add(gfxId, value);
                return value;
            }
        }
        public static string GetPngDirectory()
        {
            return Path.Combine(m_gfxPath, FIXTURES_PATH);
        }
    }
}
