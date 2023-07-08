using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Giny.Maps.WPF
{
    class GfxManager
    {
        static Dictionary<string, string> m_gfxs = new Dictionary<string, string>();

        static Dictionary<string, ImageSource> m_loadedGfx = new Dictionary<string, ImageSource>();

        static string m_gfxPath;

        public static string GfxPath
        {
            get
            {
                return m_gfxPath;
            }
        }
        public static bool Initialize(string gfxPath)
        {
            m_gfxPath = gfxPath;

            if (!Directory.Exists(gfxPath))
            {
                return false;
            }

            foreach (var file in Directory.EnumerateFiles(GetPath(), "*.*", SearchOption.AllDirectories))
            {
                string id = Path.GetFileNameWithoutExtension(file);
                m_gfxs.Add(id, Path.GetFullPath(file));
            }
            return true;
        }
        public static ImageSource GetIcon(string gfxId)
        {
            return WpfUtils.GetImageSource(m_gfxs[gfxId]);
        }
        public static bool GfxExists(string gfxId)
        {
            return m_gfxs.ContainsKey(gfxId);
        }
        public static ImageSource GetImageSource(string gfxId)
        {
            ImageSource value = null;

            if (m_loadedGfx.TryGetValue(gfxId, out value))
            {
                return value;
            }
            else
            {
                value = WpfUtils.GetImageSource(m_gfxs[gfxId]);
                m_loadedGfx.Add(gfxId, value);
                return value;
            }
        }
        public static string GetPath()
        {
            return m_gfxPath;
        }
    }
}
