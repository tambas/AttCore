using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raiders.Uplauncher
{
    public class Config
    {
        private const string Path = "config.json";

        public static Config Instance;

        public string CurrentVersion
        {
            get;
            set;
        } = "0";


        public static void Initialize()
        {
            if (!File.Exists(Path))
            {
                Instance = new Config();
                Save();
            }
            else
            {
                Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path));
            }
        }
        public static void Save()
        {
            File.WriteAllText(Path, JsonConvert.SerializeObject(Instance));
        }
    }
}
