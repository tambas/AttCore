using Giny.Core;
using Giny.Core.Commands;
using Giny.Core.DesignPattern;
using Giny.Core.IO;
using Giny.Core.Network;
using Giny.Core.Network.IPC;
using Giny.Core.Network.Messages;
using Giny.IO.D2UI;
using Giny.IO.RawPatch;
using Giny.ORM;
using Giny.Protocol;
using Giny.Protocol.Custom.Enums;
using Giny.Protocol.Messages;
using Giny.World.Managers.Maps;
using Giny.World.Modules;
using Giny.World.Network;
using Giny.World.Records.Idols;
using Giny.World.Records.Items;
using Giny.World.Records.Spells;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Giny.World
{
    /// <summary>
    /// Entry point.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            /* WIPManager.Analyse(Assembly.GetExecutingAssembly());
              Console.Read(); */

            Logger.DrawLogo();
            StartupManager.Instance.Initialize(Assembly.GetExecutingAssembly());
            IPCManager.Instance.ConnectToAuth();
            ConsoleCommandsManager.Instance.ReadCommand();
        }

        [StartupInvoke("Protocol Manager", StartupInvokePriority.SecondPass)]
        public static void InitializeProtocolManager()
        {
            ProtocolMessageManager.Initialize(Assembly.GetAssembly(typeof(RawDataMessage)), Assembly.GetAssembly(typeof(Program)));
            ProtocolTypeManager.Initialize();
        }
        [StartupInvoke("Raw Patches", StartupInvokePriority.Last)]
        public static void InitializeRawPatches()
        {
            RawPatchManager.Instance.Initialize();
        }
        [StartupInvoke("Console Commands", StartupInvokePriority.Last)]
        public static void InitializeConsoleCommand()
        {
            ConsoleCommandsManager.Instance.Initialize(AssemblyCore.GetTypes());
        }
    }
}
