using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseWhispers
{
    class CWsystem
    {
        rx rx;
        tx tx;
        public CWsystem()
        {
            rx = new rx();
            tx = new tx();
        }

    }
}
