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
        public static bool txon;
        //TcpClient tcp;
        private Socket tcpClient;
        private Socket udp;

        public tx()
        {
            txon = false;
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endpoint = new IPEndPoint(rx.GetLocalIPAddress(), 0);
            udp.Bind(endpoint);
            udp.EnableBroadcast = true;

        }
        public void SendRequests()
        {
            while (true)
            {
                while (!txon && !rx.rxon)
                {
                    byte[] msg = new byte[20];
                    byte[] networking17 = Encoding.ASCII.GetBytes("Networking17COOL");// + new Random().Next());
                    byte[] randomInt = BitConverter.GetBytes(new Random().Next());
                    Array.Copy(networking17, msg, networking17.Length);
                    Array.Copy(randomInt, 0, msg, networking17.Length, randomInt.Length);
                    udp.SendTo(msg, new IPEndPoint(IPAddress.Broadcast, 6000));
                    Console.WriteLine("Sending Broadcast...");
                    Thread.Sleep(1000);
                }
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
            }
        }

        private void ConnectTcp(EndPoint remoteEndPoint)
        {
            try
            {
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpClient.Connect(remoteEndPoint);
                txon = true;
                byte[] msg;
                if (rx.rxon)
                {
                    while (rx.message == null)
                    {

                    }
                    msg = Encoding.ASCII.GetBytes(rx.message);
                    rx.message = null;
                }
                else
                {
                    Console.WriteLine("Please enter a message");
                    string userMessage = Console.ReadLine();
                    if (rx.rxon)
                    {
                        while (rx.message == null)
                        {

                        }
                        msg = Encoding.ASCII.GetBytes(rx.message);
                        rx.message = null;
                    }
                    else
                    {
                        msg = Encoding.ASCII.GetBytes(userMessage);
                    }
                }
                tcpClient.Send(msg);
                Console.WriteLine("Message sent");
                tcpClient.Disconnect(true);
                txon = false;
            }
            catch (Exception e)
            {
                txon = false;
                Console.WriteLine(e);

            }
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

