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
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title ="FCards Server";
            Console.WriteLine("FCards-Server V0.0.2 Alpha");
            Console.WriteLine("Server Started!\n");
            Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n");
            
            (new Thread(new ThreadStart(new ClientsConnector().Connector))).Start();

            while (true)
               Console.ReadLine();
        }
    }
}
