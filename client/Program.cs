using System;

namespace signalr
{
    public class Program
    {
        private static string ip = "213.168.249.164";
        private static int port = 80;
        static void Main(string[] args)
        {
            var connector = new SocketConnector(ip, port);
        }
    }
}

