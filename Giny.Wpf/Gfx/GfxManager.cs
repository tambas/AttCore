using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace Giny.Graphics.Gfx
{
    public class GfxManager
    {
        private const string PNG_PATH = "PNG/";

        static Dictionary<int, string> m_gfxs = new Dictionary<int, string>();

        static Dictionary<int, ImageSource> m_loadedGfx = new Dictionary<int, ImageSource>();

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

            foreach (var file in Directory.EnumerateFiles(GetPngDirectory(), "*.*", SearchOption.AllDirectories))
            {
                int id = int.Parse(Path.GetFileNameWithoutExtension(file));
                m_gfxs.Add(id, Path.GetFullPath(file));
            }
            return true;
        }
        public static ImageSource GetIcon(int gfxId)
        {
            return WpfUtils.GetImageSource(m_gfxs[gfxId]);
        }
        public static bool GfxExists(int gfxId)
        {
            return m_gfxs.ContainsKey(gfxId);
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
                value = WpfUtils.GetImageSource(m_gfxs[gfxId]);
                m_loadedGfx.Add(gfxId, value);
                return value;
            }
        }
        public static string GetPngDirectory()
        {
            return Path.Combine(m_gfxPath, PNG_PATH);
        }
    }
}
