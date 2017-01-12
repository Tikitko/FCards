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
    class LobbyMaker
    {
        private List<ClientSandbox> Clients;
        private List<int> Lobby;
        private Thread Maker;
        public LobbyMaker()
        {
            Clients = new List<ClientSandbox>();
            Maker = new Thread(MakerT);
            Maker.Start();
        }
        public void AddClient(ClientSandbox Client)
        {
            Clients.Add(Client);
            Clients[Clients.Count - 1].ExternalStatus = 2;
        }
        public void MakerT()
        {
            while(true)
            {
                Lobby = new List<int>();
                while (Clients.Count == 0)
                    Thread.Sleep(100);
                for (int i = 0; i < Clients.Count && Lobby.Count < 2; i++)
                {
                    if (Clients[i].ExternalStatus == 2 && Clients[i].GetStatus() == 1)
                    {
                        Lobby.Add(i);
                        Clients[i].ExternalStatus = 3;
                    }
                    if (i == Clients.Count - 1)
                        i = -1;
                    Thread.Sleep(10);
                }
                Console.Write("Lobby with clients ");
                foreach (int v in Lobby)
                    Console.Write(Clients[v].ClientName + " ");
                Console.WriteLine("created! Starting game...");
                new Game(ref Clients, Lobby);
            }
        }
    }
}
