using Giny.Core.Network;
using Giny.IO;
using Giny.Zaap.Network;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Giny.Zaap
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public static int InstanceId = 1;

        public const int PORT = 3001;

        private string ClientPath
        {
            get;
            set;
        }
        private ZaapServer ZaapServer
        {
            get;
            set;
        }
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ClientPath = ConfigurationManager.AppSettings["clientPath"];

            ZaapServer = new ZaapServer();
            ZaapServer.Start();

        }

        private void Server_OnSocketConnected(System.Net.Sockets.Socket obj)
        {
            throw new NotImplementedException();
        }

        private void Server_OnServerStarted()
        {
            Console.WriteLine("Server started.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dofusPath = Path.Combine(ClientPath, ClientConstants.ExePath);

            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = dofusPath;
            ps.Arguments = string.Format("--port={0} --gameName=dofus --gameRelease=main --instanceId={1} --hash=464e4625-67f1-4706-985c-8358f8661e3c --canLogin=true", PORT, InstanceId++);
            Process process = new Process();
            process.StartInfo = ps;
            process.Start();
        }
    }
}
