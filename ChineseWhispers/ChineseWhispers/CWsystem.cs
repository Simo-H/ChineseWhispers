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
        public static Thread t2;
        public static Thread t3;
        public static LogWriter writer;
        public CWsystem()
        {
            rx = new rx();
            tx = new tx();
            writer = LogWriter.Instance;
            
        }

        public void run()
        {
            Thread t1 = new Thread(tx.UdpListen);
            t1.Start();
            t2 = new Thread(rx.sendOffer);
            t2 =new Thread(tx.SendRequests);
            t2.Start();
            new Thread(() => rx.TcpReciveConnection()).Start();
            while (!rx.rxon)
            {
                
            }
            t2.Abort();
        }
    }
}
