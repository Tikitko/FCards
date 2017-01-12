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
    static class SocketFunctions
    {
        public static bool GetClient(Socket sListener,ref Socket Client, ref SocketException exr)
        {
            try
            {
                Client =  sListener.Accept();
                return true;
            }
            catch (SocketException ex)
            {
                exr = ex;
                return false;
            }
        }
        public static bool CloseClient(Socket Client, ref SocketException exr)
        {
            try
            {
                Client.Shutdown(SocketShutdown.Both);
                Client.Close();
                return true;
            }
            catch (SocketException ex)
            {
                exr = ex;
                return false;
            }
        }
        public static bool ReceiveClient(Socket Client,ref string smsg, ref SocketException exr)
        {
            try
            {
                string data = string.Empty;
                byte[] bytes = new byte[1024];
                int bytesRec = Client.Receive(bytes);
                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                smsg = data;
                return true;
            }
            catch (SocketException ex)
            {
                exr = ex;
                return false;
            }
        }
        public static bool SendClient(Socket Client, string smsg, ref SocketException exr)
        {
            try
            {
                byte[] msg = Encoding.UTF8.GetBytes(smsg);
                Client.Send(msg);
                return true;
            }
            catch (SocketException ex)
            {
                exr = ex;
                return false;
            }
        }
    }
}
