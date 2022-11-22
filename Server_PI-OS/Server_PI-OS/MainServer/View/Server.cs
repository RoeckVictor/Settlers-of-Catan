using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Network.Controller;

namespace MainServer.View
{
    public class Server
    {
        private TcpListener server = null;
        private int count;
        private NetworkManager clients = new NetworkManager();

        public Server(int port)
        {
            count = 0;
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("-- The local End point is  :" + server.LocalEndpoint);
            Console.WriteLine("-- Waiting for a connection.....");
            StartListener();
        }
        private void StartListener()
        {
            try
            {
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    count++;
                    Console.WriteLine("Nombre de communiations {0}, nouvel identifiant -> {1} ", count, getHash(client));
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDevice));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        private void HandleDevice(Object obj)
        {
            clients.Communication(obj);
        }

        private int getHash(TcpClient client)
        {
            return client.GetStream().GetHashCode();
        }

    }
}
