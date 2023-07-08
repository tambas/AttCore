using Giny.Core;
using Giny.IO.D2I;
using Giny.IO.D2O;
using Giny.IO.D2OClasses;
using Giny.IO.DLM.Elements;
using Giny.IO.ELE.Repertory;
using Giny.ORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Giny.Graphics.Gfx;

namespace Giny.MapEditor
{
    class GfxOrderManager
    {
        public const string SUBAREA_D2O_PATH = @"Dofus/SubAreas.d2o";

        public const string D2I_PATH = @"Dofus/i18n_fr.d2i";

        static int currentPercentage;

        public static event Action<int> Progress;

        public static void Order()
        {
            string d2iPath = Path.Combine(Environment.CurrentDirectory, D2I_PATH);
            string subareaPath = Path.Combine(Environment.CurrentDirectory, SUBAREA_D2O_PATH);

            if (!File.Exists(d2iPath))
            {
                throw new Exception("Unable to locate " + d2iPath);
            }
            if (!File.Exists(subareaPath))
            {
                throw new Exception("Unable to locate " + d2iPath);
            }

            List<int> dones = new List<int>();

            D2OReader reader = new D2OReader(subareaPath);
            var subareas = reader.EnumerateObjects();

            var d2iFile = new D2IFile(d2iPath);

            Dictionary<short, string> subareaNames = new Dictionary<short, string>();

            foreach (var subarea in subareas.Cast<SubArea>())
            {
                subareaNames.Add((short)subarea.id, d2iFile.GetText((int)subarea.NameId));

            }

            double n = 0;

            var elements = ElementsManager.GetElements();
            var maps = MapsManager.GetMaps();

            foreach (var map in maps)
            {
                foreach (var layer in map.Layers)
                {
                    foreach (var cell in layer.Cells)
                    {
                        foreach (var element in cell.Elements.OfType<GraphicalElement>())
                        {
                            if (elements.ContainsKey(((int)element.ElementId)))
                            {
                                var ele = elements[(int)element.ElementId];

                                var graphical = ele as NormalGraphicalElementData;

                                if (graphical != null)
                                {
                                    if (graphical.Gfx != 0 && dones.Contains(graphical.Gfx) == false)
                                    {
                                        MoveFile(graphical.Gfx, map.SubareaId, subareaNames);
                                        dones.Add(graphical.Gfx);
                                    }
                                }
                            }

                        }
                    }
                }
                n++;

                int percentage = (int)(n / maps.Length * 100d);

                if (percentage != currentPercentage)
                {
                    currentPercentage = percentage;
                    Progress?.Invoke(currentPercentage);
                }
            }
            string noCategoryPath = Path.Combine(GfxManager.GetPngDirectory(), "No Category");

            Directory.CreateDirectory(noCategoryPath);

            foreach (var file in Directory.GetFiles(GfxManager.GetPngDirectory()))
            {
                File.Move(file, Path.Combine(noCategoryPath, Path.GetFileName(file)));
            }

            currentPercentage = 0;
        }
        private static void MoveFile(int gfxId, short subareaId, Dictionary<short, string> subareaNames)
        {
            if (!subareaNames.ContainsKey(subareaId))
            {
                return;
            }

            var pngPath = GfxManager.GetPngDirectory();

            string path = Path.Combine(pngPath, subareaNames[subareaId] + "/");

            Directory.CreateDirectory(path);

            string filePath = Path.Combine(pngPath, gfxId + ".png");
            if (File.Exists(filePath))
                File.Move(filePath, Path.Combine(path, gfxId + ".png"));
        }
    }
}
