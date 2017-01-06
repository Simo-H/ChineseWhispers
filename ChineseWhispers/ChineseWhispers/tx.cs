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
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endpoint = new IPEndPoint(rx.GetLocalIPAddress(), 0);
            udp.Bind(endpoint);

        }
        public void SendRequests()
        {
            udp.EnableBroadcast = true;
            byte[] msg = Encoding.ASCII.GetBytes("Networking17" + new Random().Next());
            udp.SendTo(msg, new IPEndPoint(IPAddress.Broadcast, 6000));
            Console.WriteLine("Sending Broadcast...");
            
        }

        public void UdpListen()
        {
            try
            {
                //udp.Listen(1);
                while (true)
                {
                    byte[] dataByte = new byte[26];
                    udp.Receive(dataByte);
                    string data = null;
                    Console.WriteLine("Message accepted");
                    // An incoming connection needs to be processed.
                    
                    data += Encoding.ASCII.GetString(dataByte);
                    
                    //EndPoint remoteEndPoint = handler.RemoteEndPoint;
                    Console.WriteLine("Message: "+ data);
                }
            }
                catch (Exception e)
            {
                Console.WriteLine(e);
                udp.Close();
            }
        } 

        private void ConnectTcp(EndPoint remoteEndPoint)
        {
            tcpClient.Connect(remoteEndPoint);

        }
    }
}

