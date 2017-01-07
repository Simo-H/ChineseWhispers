using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChineseWhispers
{
    class CWsystem
    {
        rx rx;
        tx tx;
        public static bool txon;
        public CWsystem()
        {
            rx = new rx();
            tx = new tx();
        }

        public void run()
        {
            new Thread(() => tx.UdpListen()).Start();
            //new Thread(() => rx.sendOffer()).Start();
            new Thread(() => tx.SendRequests()).Start();
            //new Thread(() => rx.TcpReciveConnection()).Start();

        }
    }
}
