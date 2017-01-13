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
        public static bool rxon;
        Socket tcpListener;
        Socket udp;
        IPAddress ipLocal;
        public static string message;
        public static IPAddress connectedIp;
        
        public rx()
        {
            rxon = false;

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

        public void startTCP()
        {
            for(int i = 6001; i < 7000; i++)
                    {
                try
                {
                    tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    tcpListener.Bind(new IPEndPoint(ipLocal, i));
                    

                    break;
                }
                catch
                {
                    continue;
                }
            }
        }
        public void sendOffer()
        {
            while (true)
            {

                while (!rxon)
                {
                    byte[] dataBuffer = new byte[20];
                    try
                    {
                        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint remote = (EndPoint)(sender);
                        int recv = udp.ReceiveFrom(dataBuffer, ref remote);
                        string strData = Encoding.ASCII.GetString(dataBuffer);
                        List<byte> msgList = new List<byte>();
                        if (recv != 20 || ((IPEndPoint)remote).Address.ToString().Equals(ipLocal.ToString())||(connectedIp!=null&& ((IPEndPoint)remote).Address.ToString().Equals(connectedIp.ToString())))
                        {
                            continue;
                        }
                        byte[] message = new byte[26];
                        Array.Copy(dataBuffer, 0, message, 0, 16);
                        if (!Encoding.ASCII.GetString(message).Contains("Networking17"))
                        {
                            continue;
                        }
                        byte[] networking17 = new byte[16];
                        Array.Copy(dataBuffer, 0, networking17, 0, 16);
                        byte[] randomInt = new byte[4];
                        Array.Copy(dataBuffer, 16, randomInt, 0, 4);
                        CWsystem.writer.WriteToLog("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " received UDP broadcast message: " + Encoding.ASCII.GetString(networking17)+BitConverter.ToInt32(randomInt,0) + " From IP:" + ((IPEndPoint)(remote)).Address + " Port " + ((IPEndPoint)(remote)).Port);
                        Console.WriteLine("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " received UDP broadcast message: " + Encoding.ASCII.GetString(networking17) + BitConverter.ToInt32(randomInt, 0) + " From IP:" + ((IPEndPoint)(remote)).Address + " Port " + ((IPEndPoint)(remote)).Port);
                        Array.Copy(dataBuffer, 16, message, 16, 4);
                        byte[] IP = ipLocal.GetAddressBytes();
                        Array.Copy(IP, 0, message, 20, 4);
                        byte[] Port = BitConverter.GetBytes(Convert.ToInt16(((IPEndPoint)tcpListener.LocalEndPoint).Port));
                        Array.Copy(Port, 0, message, 24, 2);
                        udp.SendTo(message, remote);
                        
                        CWsystem.writer.WriteToLog("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " sent UDP offer message: " + Encoding.ASCII.GetString(networking17) + BitConverter.ToInt32(randomInt, 0) + " From IP:" + ((IPEndPoint)(remote)).Address + " Port " + ((IPEndPoint)(remote)).Port);
                        Console.WriteLine("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " sent UDP offer message: " + Encoding.ASCII.GetString(networking17) + BitConverter.ToInt32(randomInt, 0) + " From IP:" + ((IPEndPoint)(remote)).Address + " Port " + ((IPEndPoint)(remote)).Port);

                    }
                    catch (Exception e)
                    {
                        CWsystem.writer.WriteToLog(e.Message);
                        Console.WriteLine(e);
                    }
                }
            }
        }


        public void TcpReciveConnection()
        {
            while (true)
            {
                //startTCP();
                Socket accepted;
                try
                {

                    accepted = tcpListener.Accept();
                    connectedIp = ((IPEndPoint)accepted.RemoteEndPoint).Address;
                    CWsystem.writer.WriteToLog("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " accepted TCP Connection From IP: " + connectedIp + " Port " + ((IPEndPoint)accepted.RemoteEndPoint).Port);
                    Console.WriteLine ("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " accepted TCP Connection From IP: " + connectedIp + " Port " + ((IPEndPoint)accepted.RemoteEndPoint).Port);

                    rxon = true;
                    while (true)
                    {
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
                        int place = rnd.Next(0, strData.Length - 1);
                        int letterrandom = rnd.Next(97, 122);
                        char insertRandomChar = (char)letterrandom;
                        //var aStringBuilder = new StringBuilder(strData);
                        strData = strData.Remove(place, 1);
                        strData = strData.Insert(place, insertRandomChar.ToString());
                        //  strData = aStringBuilder.ToString();
                        CWsystem.writer.WriteToLog("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " received message: " + Encoding.ASCII.GetString(formatted) + " From IP: " + connectedIp + " Port " + ((IPEndPoint)accepted.RemoteEndPoint).Port + " Changed it to: " + strData);
                        Console.WriteLine("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " received message: " + Encoding.ASCII.GetString(formatted) + " From IP: " + connectedIp + " Port " + ((IPEndPoint)accepted.RemoteEndPoint).Port + " Changed it to: " + strData);

                        if (tx.txon)
                        {
                            message = strData;

                        }
                        else
                        {
                            Console.WriteLine(strData);
                        }
                    }
                }
                catch (Exception e)
                {
                    //accepted.Disconnect(true);
                    Console.Write(e);
                    rxon = false;
                    continue;
                }
              
                // message=strData;
            }
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
