using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace FCards_Server
{
    class ClientsConnector
    {
        private Socket sListener;
        private LobbyMaker LM;
        public ClientsConnector()
        {
            sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sListener.Bind(new IPEndPoint(IPAddress.Any, 27015)); // 27015
            sListener.Listen(10);
            LM = new LobbyMaker();
        }
        public void Connector()
        {
            Console.WriteLine("Clients Connector Started!");
            while (true)
                LM.AddClient(new ClientSandbox(sListener));
        }
    }
}
