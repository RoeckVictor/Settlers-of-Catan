using Network.View;
using Noyau.View;
using System;
using System.Net;
using System.Threading;
using Util.View;
using AI.Controller;


namespace Server_PI_OS
{
    class Program
    {
        static void Main(string[] args)
        {
            GameView.Instance.RegisterControllerHandlers();
            NetworkView.Instance.RegisterControllerHandlers();
            AIManager.Instance.RegisterEventHandlers();
            Thread t = new Thread(delegate ()
            {
                MainServer.View.Server myserver = new MainServer.View.Server(6321);
            });
            t.Start();

            Console.WriteLine("Server Started...!");


        }
    }
}


