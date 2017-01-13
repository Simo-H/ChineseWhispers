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
        public static Mutex m;
        public static bool txon;
        //TcpClient tcp;
        private Socket tcpClient;
        private Socket udp;

        public tx()
        {
            try
            {
                m= new Mutex();
                txon = false;
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endpoint = new IPEndPoint(rx.GetLocalIPAddress(), 0);
                udp.Bind(endpoint);
                udp.EnableBroadcast = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CWsystem.writer.WriteToLog(e.Message);
            }

        }
        public void SendRequests()
        {
            while (!txon && !rx.rxon)
            {
                byte[] msg = new byte[20];
                byte[] networking17 = Encoding.ASCII.GetBytes("Networking17COOL");// + new Random().Next());
                byte[] randomInt = BitConverter.GetBytes(new Random().Next());
                Array.Copy(networking17, msg, networking17.Length);
                Array.Copy(randomInt, 0, msg, networking17.Length, randomInt.Length);
                udp.SendTo(msg, new IPEndPoint(IPAddress.Broadcast, 6000));
                CWsystem.writer.WriteToLog("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " Sent UDP Broadcast to port 6000...");
                Console.WriteLine("IP:" + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " Sent UDP Broadcast to port 6000...");

                Thread.Sleep(1000);
            }
        }

        public void UdpListen()
        {
            try
            {
                
                while (!rx.rxon)
                {

                    byte[] dataByte = new byte[26];
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint remote = (EndPoint)(sender);
                    int recv = udp.ReceiveFrom(dataByte, ref remote);
                    if (recv != 26)
                    {
                        continue;
                    }
                    string networking17;
                    int randomInt;
                    EndPoint remotEndPoint;
                    readOfferMessage(dataByte, out networking17, out randomInt, out remotEndPoint);
                    if (!networking17.Contains("Networking17"))
                    {
                        continue;
                    }

                    CWsystem.writer.WriteToLog("IP: " + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " received UDP offer message: " + networking17+" "+randomInt + " From IP :" + ((IPEndPoint)(remote)).Address + " Port " + ((IPEndPoint)(remote)).Port);
                    System.Console.WriteLine("IP: " + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " received UDP offer message: " + networking17 + " " + randomInt + " From IP :" + ((IPEndPoint)(remote)).Address + " Port " + ((IPEndPoint)(remote)).Port);

                    CWsystem.writer.WriteToLog("IP: " + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " Trying to connect via TCP to IP: " + ((IPEndPoint)(remotEndPoint)).Address + " Port " + ((IPEndPoint)(remotEndPoint)).Port);
                    System.Console.WriteLine("IP: " + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " Trying to connect via TCP to IP: " + ((IPEndPoint)(remotEndPoint)).Address + " Port " + ((IPEndPoint)(remotEndPoint)).Port);
                    ConnectTcp(remotEndPoint);
                    //Console.WriteLine("Message: ");
                }
            }
            catch (Exception e)
            {
                CWsystem.writer.WriteToLog(e.Message);
                Console.WriteLine(e);
            }
        }

        private void ConnectTcp(EndPoint remoteEndPoint)
        {
            try
            {
                //m.WaitOne();
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //if (rx.connectedIp != null && rx.connectedIp.ToString().Equals(((IPEndPoint)remoteEndPoint).Address.ToString()))
                //{
                //    return;
                //}
                //tcpClient.Connect(remoteEndPoint);
                rx.connectedIp = ((IPEndPoint)(remoteEndPoint)).Address;
                Console.WriteLine("IP: " + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " Connected succesfully via TCP to IP: " + ((IPEndPoint)(remoteEndPoint)).Address + " Port " + ((IPEndPoint)(remoteEndPoint)).Port);
                CWsystem.writer.WriteToLog("IP: " + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " Connected succesfully via TCP to IP: " + ((IPEndPoint)(remoteEndPoint)).Address + " Port " + ((IPEndPoint)(remoteEndPoint)).Port);
                txon = true;
                //m.ReleaseMutex();
               
                byte[] msg;
                while (true)
                {
                    if (rx.rxon)
                    {
                        while (rx.message == null)
                        {

                        }
                        msg = Encoding.ASCII.GetBytes(rx.message);
                        CWsystem.writer.WriteToLog("Message delivered from another machine: " + msg);
                        Console.WriteLine("Message delivered from another machine: " + msg);
                        rx.message = null;
                        tcpClient.Send(msg);
                       
                    }
                    else
                    {
                        Thread t = new Thread(readThread);
                        t.Start();
                        while (rx.message == null)
                        {

                        }
                        if (t.IsAlive)
                        {
                            t.Abort();
                        }
                        CWsystem.writer.WriteToLog("Message: " + rx.message);
                        Console.WriteLine("Message: " + rx.message);
                        msg = Encoding.ASCII.GetBytes(rx.message);
                        rx.message = null;
                        tcpClient.Send(msg);
                    }
                }

            }
            catch (Exception e)
            {
              //  m.ReleaseMutex();

                txon = false;

                CWsystem.writer.WriteToLog("IP: " + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " Failed to connect via TCP to IP: " + ((IPEndPoint)(remoteEndPoint)).Address + " Port " + ((IPEndPoint)(remoteEndPoint)).Port);
                Console.WriteLine("IP: " + rx.GetLocalIPAddress().ToString() + " Port: " + ((IPEndPoint)(udp.LocalEndPoint)).Port.ToString() + " Failed to connect via TCP to IP: " + ((IPEndPoint)(remoteEndPoint)).Address + " Port " + ((IPEndPoint)(remoteEndPoint)).Port);

                CWsystem.writer.WriteToLog(e.Message);
                Console.WriteLine(e.Message);
                UdpListen();
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

        public void readThread()
        {
            Console.WriteLine("Please enter a message");
            rx.message = Console.ReadLine();
        }
    }
}

