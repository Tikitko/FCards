using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace FCards_Client
{
    class SocketFunctions
    {
        public static bool Receive(Socket Client, ref string smsg, ref SocketException exr)
        {
            try
            {
                string data = string.Empty;
                byte[] bytes = new byte[1024];
                data += Encoding.UTF8.GetString(bytes, 0, Client.Receive(bytes));
                smsg = data;
                return true;
            }
            catch (SocketException ex)
            {
                exr = ex;
                return false;
            }
        }
        public static bool Send(Socket Client, string smsg, ref SocketException exr)
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
