using Giny.Core.DesignPattern;
using Giny.Core.Network;
using Giny.Core.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Zaap.Network
{
    public class ZaapServer
    {
        private TcpServer Server
        {
            get;
            set;
        }

        internal void Start()
        {
            this.Server = new TcpServer("127.0.0.1", MainWindow.PORT);
            this.Server.OnSocketConnected += Server_OnSocketConnected;
            Server.Start();

        }

        private void Server_OnSocketConnected(System.Net.Sockets.Socket obj)
        {
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                var zaapClient = new ZaapClient(obj, MainWindow.Instance.userName.Text, MainWindow.Instance.password.Text);
            });
        }
    }
}
