using Giny.IO.D2O;
using Giny.IO.D2OClasses;
using Giny.IO.D2P;
using Giny.IO.DLM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.MapEditor
{
    public class D2PManager
    {
        public static void ModifyMap(DlmMap map)
        {
            var newFile = new D2PFile();

            var targetD2P = new D2PFile(MapsManager.GetD2PFileName(map.Id));

            string ouputPath = targetD2P.FilePath;

            foreach (var link in targetD2P.Links)
            {
                newFile.AddLink(Path.GetFileName(link.FilePath));
            }

            foreach (var entry in targetD2P.GetEntriesOfInstanceOnly())
            {
                if (entry.GetMapId() == map.Id)
                {
                    newFile.AddFile(entry.FullFileName, map.Serialize());
                }
                else
                {
                    newFile.AddFile(entry.FullFileName, targetD2P.ReadFile(entry));
                }
            }

            File.Delete(ouputPath);

            newFile.SaveAs(ouputPath);

            newFile.Dispose();

            new D2PFile(ouputPath); // test file corrupted to remove when sure about d2p serialization

        }
        public static void AddMap(DlmMap map)
        {
            int packageId = GetD2PPackageId(map.Id);

            var newFile = new D2PFile();

            var targetD2P = new D2PFile(GetD2PFileFromPackageId(packageId).FilePath);

            string ouputPath = targetD2P.FilePath;

            foreach (var link in targetD2P.Links)
            {
                newFile.AddLink(Path.GetFileName(link.FilePath));
            }
            foreach (var entry in targetD2P.GetEntriesOfInstanceOnly())
            {
                newFile.AddFile(entry.FullFileName, targetD2P.ReadFile(entry));
            }

            newFile.AddFile(string.Format("{1}/{0}.dlm", packageId, map.Id), map.Serialize());

            File.Delete(ouputPath);

            newFile.SaveAs(ouputPath);

            newFile.Dispose();

            MapsManager.ReloadEntries();

        }

        static int GetD2PPackageId(int mapId)
        {
            return mapId % 10;
        }

        static D2PFile GetD2PFileFromPackageId(int packageId)
        {
            return GetD2PFileFromPackageIdRecursive(packageId, MapsManager.GetMaps0(), 0);
        }
        static D2PFile GetD2PFileFromPackageIdRecursive(int packageId, D2PFile current, int packageIdCurrent)
        {
            if (packageIdCurrent == packageId)
            {
                return current;
            }
            else
            {
                return GetD2PFileFromPackageIdRecursive(packageId, current.Links[0], packageIdCurrent + 1);
            }
        }
    }
}
