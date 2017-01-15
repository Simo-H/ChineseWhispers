using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChineseWhispers
{
    /// <summary>
    /// This is the ctor of the complete system
    /// </summary>
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
        /// <summary>
        /// This method start of the system components.
        /// </summary>
        public void run()
        {
            Thread t1 = new Thread(tx.UdpListen);
            t1.Start();
            t3 = new Thread(rx.sendOffer);
            t3.Start();
            t2 =new Thread(tx.SendRequests);
            t2.Start();
            new Thread(() => rx.TcpReciveConnection()).Start();
            
        }
    }
}
