using Giny.Core.DesignPattern;
using Giny.IO.D2P;
using Giny.IO.DLM;
using Giny.IO.ELE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Rendering.Maps
{
    public class MapsManager : Singleton<MapsManager>
    {
        private const string MapsRelativePath = @"content\maps\maps0.d2p";

        private const string EleRelativePath = @"content\maps\elements.ele";

        private D2PFile MapsFile
        {
            get;
            set;
        }
        public Dictionary<int, EleGraphicalData> Elements
        {
            get;
            private set;
        }
        public void Initialize(string clientPath)
        {
            this.MapsFile = new D2PFile(Path.Combine(clientPath, MapsRelativePath));

            EleReader reader = new EleReader(Path.Combine(clientPath, EleRelativePath));
            this.Elements = reader.ReadElements();
        }

        public DlmMap ReadMap(int mapId)
        {
            var packageId = mapId % 10;
            var entry = MapsFile.TryGetEntry(string.Format("{0}/{1}.dlm", packageId, mapId));

            if (entry == null)
            {
                return null;
            }
            return new DlmMap(entry);
        }
    }
}
