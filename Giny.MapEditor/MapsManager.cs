using Giny.IO.DLM;
using Giny.IO.D2P;
using Giny.IO.ELE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giny.IO.DLM.Elements;
using Giny.IO.ELE.Repertory;

namespace Giny.MapEditor
{
    class MapsManager
    {
        public const string MAPS0_FILENAME = "maps0.d2p";

        public const string D2P_PATH = "content/maps";

        static Dictionary<int, D2PEntry> m_entries;

        static D2PFile m_maps0;

        public static void ReloadEntries()
        {
            m_entries = new Dictionary<int, D2PEntry>();

            m_maps0 = new D2PFile(Path.Combine(ConfigFile.Instance.ClientPath, Path.Combine(D2P_PATH, MAPS0_FILENAME)));

            foreach (var entry in m_maps0.Entries)
            {
                m_entries.Add(entry.GetMapId(), entry);
            }


        }

        public static D2PFile GetMaps0()
        {
            return m_maps0;
        }

        public static bool MapExistsInD2P(int mapId)
        {
            return m_entries.ContainsKey(mapId);
        }
        public static string GetD2PFileName(int mapId)
        {
            return m_entries[mapId].Container.FilePath;
        }
        /// <summary>
        /// Really slow for sprite order only.
        /// </summary>
        [Obsolete]
        public static DlmMap[] GetMaps()
        {
            return m_entries.Values.Select(x => new DlmMap(x)).ToArray();
        }

        public static DlmMap LoadDlmMap(int id)
        {
            return m_entries.ContainsKey(id) ? new DlmMap(m_entries[id]) : null;
        }

    }
}
