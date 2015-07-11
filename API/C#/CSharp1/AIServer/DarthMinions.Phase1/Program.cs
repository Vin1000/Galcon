using AIServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarthMinions.Phase1
{
    class Program
    {
        private AI _player;

        public const int PORT = 8889;
        static void Main(string[] args)
        {
            var tcpServer = new TCPServer(PORT);
            var player = new AI(tcpServer);

            new Thread(Waiter).Start();

        }

        public void ReceiveData()
        {

        }
    }
}
