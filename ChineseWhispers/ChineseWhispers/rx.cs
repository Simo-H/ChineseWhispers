﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        IPEndPoint ipLocal;
        public rx()
        {
            on = false;
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
            ipLocal = (IPEndPoint)tcpListener.LocalEndPoint;


                    for (int i = 6001; i < 7000; i++)
                    {
                        try {
                            tcpListener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
                            tcpListener.Bind(new IPEndPoint(ipLocal.Address, i));
                            udp.Bind(new IPEndPoint(ipLocal.Address, 6000));

                            break;
                        } 
                        catch
                        {
                            continue;
                        }
                    }                   
                       
            tcpListener.Listen(1);
            udp.Listen(1);
        }
        public void sendOffer()
        {
            Socket accepted = udp.Accept();
            
            Buffer = new byte[accepted.SendBufferSize];
            int bytesRead = accepted.Receive(Buffer);
            byte[] formatted = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
            {
                formatted[i] = Buffer[i];
            }
            string strData = Encoding.ASCII.GetString(formatted);
            EndPoint cameFrom= accepted.RemoteEndPoint;
            byte[] msg = Encoding.ASCII.GetBytes(strData + ipLocal.Address + ((IPEndPoint)(tcpListener.LocalEndPoint)).Port.ToString());
            udp.SendTo(msg, cameFrom);
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




    }
}
