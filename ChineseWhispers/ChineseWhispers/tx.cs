using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            while (true)
            {
                udp.EnableBroadcast = true;
                byte[] msg = new byte[20];
                byte[] networking17 = Encoding.ASCII.GetBytes("Networking17COOL");// + new Random().Next());
                byte[] randomInt = BitConverter.GetBytes(new Random().Next());
                Array.Copy(networking17, msg, networking17.Length);
                Array.Copy(randomInt, 0, msg, networking17.Length, randomInt.Length);
                udp.SendTo(msg, new IPEndPoint(IPAddress.Broadcast, 6000));
                Console.WriteLine("Sending Broadcast...");
            Thread.Sleep(5000);
        }

    }

        public void UdpListen()
        {
            try
            {
                //udp.Listen(1);
                while (true)
                {
                    byte[] dataByte = new byte[26];
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint remote = (EndPoint)(sender);
                    int recv = udp.ReceiveFrom(dataByte, ref remote);
                    if (recv != 26)
                    {
                        continue;
                    }
                    Console.WriteLine("Message accepted");
                    string networking17;
                    int randomInt;
                    EndPoint remotEndPoint;
                    readOfferMessage(dataByte, out networking17, out randomInt, out remotEndPoint);
                    if (!networking17.Contains("Networking17"))
                    {
                        continue;
                    }
                    Console.WriteLine("Connecting to tcp..");
                    ConnectTcp(remotEndPoint);
                    //Console.WriteLine("Message: ");
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

        private void readOfferMessage(byte[] dataByte, out string networking17, out int randomInt, out EndPoint remoteEndPoint)
        {
            byte[] networking17byte = new byte[16];
            Array.Copy(dataByte, 0, networking17byte, 0, 16);
            byte[] randomIntbyte = new byte[4];
            Array.Copy(dataByte, 16, randomIntbyte, 0, 4);
            byte[] ipByte = new byte[4];
            Array.Copy(dataByte, 20, ipByte, 0, 4);
            byte[] portShort = new byte[2];
            Array.Copy(dataByte, 24, portShort, 0, 2);
            networking17 = Encoding.ASCII.GetString(networking17byte);
            randomInt = BitConverter.ToInt32(randomIntbyte, 0);
            IPAddress ip = new IPAddress(ipByte);
            short port = BitConverter.ToInt16(portShort, 0);
            remoteEndPoint = new IPEndPoint(ip, port);
        }
    }
}

