using System;
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
        bool on;
        TcpListener tcp;
        UdpClient udp;
        public rx()
        {
            on = false;
            udp = new UdpClient(6000);
            IPHostEntry host;            
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    for (int i = 6001; i < 7000; i++)
                    {
                        try {
                            tcp = new TcpListener(ip, i);
                            break;
                        } 
                        catch
                        {
                            continue;
                        }
                    }                   
                }
            }
            
        }

        
    }
}
