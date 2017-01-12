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
    class ClientSandbox
    {
        public int ExternalStatus;
        private string SendString;
        private string ReceiveString;
        private SocketException ex;
        private Socket Client;
        public string ClientName;
        private int SenderStatus;
        private int ReceiverStatus;
        private Thread Sender;
        private Thread Receiver;

        public delegate void ReceiveStringEventHandler(object sender, string ReceiveString);
        public event ReceiveStringEventHandler RSEH;

        public ClientSandbox(Socket sListener)
        {
            SendString = ReceiveString = string.Empty;
            SenderStatus = ReceiverStatus = -1;
            ex = new SocketException();
            Client = null;
            ClientName = null;
            if (SocketFunctions.GetClient(sListener, ref Client, ref ex))
            {
                ClientName = Client.RemoteEndPoint.ToString();
                Console.WriteLine("Client " + ClientName + " connected!");
                Sender = new Thread(new ThreadStart(ClientSenderThread));
                Receiver = new Thread(new ThreadStart(ClientReceiverThread));
                Sender.Start();
                Receiver.Start();
            }

        }
        public string GetReceiveString()
        {
            return ReceiveString;
        }
        public void SetSendString(string msg)
        {
            SendString = msg;
        }
        public int GetStatus()
        {
            if (SenderStatus == -1 && ReceiverStatus == -1)
                return -1;
            return (SenderStatus * ReceiverStatus);
        }
        public SocketException GetException()
        {
            return ex;
        }
        public bool CloseClient()
        {
            if (GetStatus() == 0)
            {
                Console.WriteLine("Client " + ClientName + " disconnected!");
                SocketFunctions.CloseClient(Client, ref ex);
                return true;
            }
            return false;
        }
        public void ClientReceiverThread()
        {
            Thread.Sleep(1000);
            ReceiverStatus = 1;
            while (ex.ErrorCode == 0 && SenderStatus != 0)
            {
                if (SocketFunctions.ReceiveClient(Client, ref ReceiveString, ref ex))
                    RSEH?.Invoke(this, ReceiveString);
                Thread.Sleep(10);
            }
            CloseClient();
            ReceiverStatus = 0;
        }
        public void ClientSenderThread()
        {
            Thread.Sleep(1000);
            SenderStatus = 1;
            while (ex.ErrorCode == 0 && ReceiverStatus != 0)
            {
                SocketFunctions.SendClient(Client, SendString, ref ex);
                Thread.Sleep(500);
            }
            CloseClient();
            SenderStatus = 0;
        }
    }
}
