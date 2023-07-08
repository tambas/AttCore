using Giny.Graphics.Gfx;
using Giny.IO.ELE;
using Giny.IO.ELE.Repertory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Giny.MapEditor
{
    class ElementsManager
    {
        public const string ELEMENTS_PATH = "content/maps/elements.ele";

        static Dictionary<int, EleGraphicalData> m_elements;

        public static void Initialize()
        {
            m_elements = new EleReader(Path.Combine(ConfigFile.Instance.ClientPath, ELEMENTS_PATH)).ReadElements();
        }

        public static Dictionary<int, EleGraphicalData> GetElements()
        {
            return m_elements;
        }
        private static NormalGraphicalElementData GetElement(Func<NormalGraphicalElementData, bool> predicate)
        {
            return m_elements.Values.OfType<NormalGraphicalElementData>().FirstOrDefault(predicate);
        }
        public static NormalGraphicalElementData GetElementId(int selectedGfxId)
        {
            return GetElement(x => x.Gfx == selectedGfxId && x.HorizontalSymmetry == false);
        }

        public static Point ComputePixelOffset(int gfx, Point origin)
        {
            var sprite = GfxManager.GetImageSource(gfx);

            Point result = new Point();
            result.X -= sprite.Width / 2;
            result.Y -= sprite.Height / 2;

            result.X += origin.X;
            result.Y += origin.Y;
            return result;
        }
    }
}
