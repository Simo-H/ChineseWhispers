using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ChineseWhispers
{
    class tx
    {
        bool on;
        //TcpClient tcp;
        private Socket tcpClient;
        Socket udp;

        public tx()
        {
            on = false;
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);

        }
        public void SendRequests()
        {
            udp.EnableBroadcast = true;
            byte[] msg = Encoding.ASCII.GetBytes("Networking17" + new Random().Next());
            udp.SendTo(msg, new IPEndPoint(IPAddress.Broadcast, 6000));
            
        }

        public void UdpListen()
        {
            try
            {
                IPEndPoint endpoint = (IPEndPoint)udp.LocalEndPoint;
                udp.Bind(endpoint);
                udp.Listen(1);
                while (true)
                {
                    Socket handler = udp.Accept();
                    string data = null;

                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        byte[] bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }
                    EndPoint remoteEndPoint = handler.RemoteEndPoint;
                }
            }
                catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
        } 

        private void ConnectTcp(EndPoint remoteEndPoint)
        {
            tcpClient.Connect(remoteEndPoint);

        }
    }
}

