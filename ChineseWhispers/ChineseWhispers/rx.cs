using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ChineseWhispers
{
    class rx
    {
       static byte[] Buffer { get; set; }
        bool on;
        Socket tcpListener;
        Socket udp;
        IPAddress ipLocal;
        public rx()
        {
            on = false;

            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            ipLocal = GetLocalIPAddress();


                    for (int i = 6001; i < 7000; i++)
                    {
                        try {
                            tcpListener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
                            tcpListener.Bind(new IPEndPoint(ipLocal, i));
                            udp.Bind(new IPEndPoint(ipLocal, 6000));

                            break;
                        } 
                        catch
                        {
                            continue;
                        }
                    }                   
                       
            tcpListener.Listen(1);
            //udp.Listen(1);
        }
        public void sendOffer()
        {
            while (true)
            {
                byte[] dataBuffer = new byte[20];
                try
                {
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint remote = (EndPoint)(sender);
                    int recv = udp.ReceiveFrom(dataBuffer, ref remote);
                    string strData = Encoding.ASCII.GetString(dataBuffer);
                    List<byte> msgList = new List<byte>();
                    //if (recv != 20 || ((IPEndPoint)remote).Address != ipLocal)
                    //{
                    //    continue;
                    //}
                    byte[] message = new byte[26];
                    Array.Copy(dataBuffer, 0, message, 0, 16);
                    if (!Encoding.ASCII.GetString(message).Contains("Networking17"))
                    {
                        continue;
                    }
                    Array.Copy(dataBuffer, 16, message,16, 4);
                    byte[] IP = ipLocal.GetAddressBytes();
                    Array.Copy(IP, 0, message,20, 4);
                    Console.WriteLine(((IPEndPoint)tcpListener.LocalEndPoint).Port);
                    Console.WriteLine(Convert.ToInt16(((IPEndPoint)tcpListener.LocalEndPoint).Port));
                    byte[] Port = BitConverter.GetBytes(Convert.ToInt16(((IPEndPoint)tcpListener.LocalEndPoint).Port));
                    Array.Copy(Port, 0, message, 24, 2);
                    udp.SendTo(message, remote);
                    Console.WriteLine("get request send offer");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        public string TcpReciveConnection()
        {
            Socket accepted = tcpListener.Accept();
            udp.Close();
            on = true;
            Buffer = new byte[accepted.SendBufferSize];
            int bytesRead = accepted.Receive(Buffer);
            byte[] formatted = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
            {
                formatted[i] = Buffer[i];
            }
            string strData = Encoding.ASCII.GetString(formatted);
            ///////remember to add change randomly
            Random rnd = new Random();
            int place = rnd.Next(1, strData.Length-1);
            int letterrandom = rnd.Next(0, 26); // Zero to 25
            char insertRandomChar = (char)letterrandom;
            var aStringBuilder = new StringBuilder(strData);
            aStringBuilder.Remove(place, 1);
            aStringBuilder.Insert(place, insertRandomChar);
            strData = aStringBuilder.ToString();
            return strData;
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }


    }
}
