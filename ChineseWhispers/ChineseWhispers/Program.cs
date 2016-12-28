using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseWhispers
{
    class Program
    {
        static void Main(string[] args)
        {
            rx rx = new rx();
            tx tx = new tx();
            tx.sendRequests();

        }
    }
}
