using Common.Models;
using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {


                TcpLoginServer login = new TcpLoginServer();
                var igrac = login.Start();
                


                Console.WriteLine("Igra pocinje...");

                GameServer server = new GameServer(igrac);

                System.Threading.Thread gameThread =
                    new System.Threading.Thread(server.Run);

                gameThread.Start();

                Console.WriteLine("Server running. Press ENTER to stop.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SERVER ERROR: " + ex.Message);
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
