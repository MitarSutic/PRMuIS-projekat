using System;
using System.Collections.Generic;
using Common.Models;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // === TCP LOGIN (1 ili 2 igraca) ===
                TcpLoginServer login = new TcpLoginServer();
                List<Igrac> igraci = login.Start();

                Console.WriteLine("Igra pocinje...");

                // === GAME SERVER (UDP + GAME LOOP) ===
                GameServer server = new GameServer(igraci);

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
