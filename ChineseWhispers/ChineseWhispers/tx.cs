using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChineseWhispers
{
    class tx
    {
        bool on;
        TcpClient tcp;
        UdpClient udp;
        
        public tx()
        {
            on = false;
            tcp = new TcpClient();
            udp = new UdpClient();
        }
        public void sendRequests()
        {
            Ping p = new Ping();

        }
    }
}
