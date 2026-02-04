using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Models;
using Common.Net;
using Newtonsoft.Json;

namespace Server
{
    class GameServer
    {
        private GameEngine engine;
        private UdpClient udp;
        private IPEndPoint clientEP;

        public GameServer(Igrac igrac)
        {
            engine = new GameEngine(igrac);
            udp = new UdpClient(9001);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            clientEP = new IPEndPoint(IPAddress.Any, 0);

            byte[] handshake = udp.Receive(ref clientEP);
            Console.WriteLine("UDP kliejnt: " + clientEP);
        }

        public void Run()
        {
            Console.WriteLine("Game loop started (UDP sending)...");

            try
            {
                while (engine.State.Igrac.BrojZivota > 0)
                {
                    ReceiveInput();     // UDP od klijenta (ako postoji)
                    engine.Update();    // pomeranja, prepreke
                    SendState();        // UDP ka klijentu
                    Thread.Sleep(150);
                }
                engine.State.Status = Status.GAME_OVER;
                SendState();
                Console.WriteLine("GAME OVER");
            }
            catch (Exception ex)
            {
                Console.WriteLine("GameServer error: " + ex.Message);
            }
        }


        private void ReceiveInput()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udp.Receive(ref ep);

                var cmd = BinarySerializer.Deserialize<InputCommand>(data);
                engine.HandleInput(cmd);
            }
            catch (SocketException)
            {
                // nema inputa
            }
        }


        private void SendState()
        {
            string json = JsonConvert.SerializeObject(engine.State);
            byte[] data = Encoding.UTF8.GetBytes(json);

            udp.Send(data, data.Length, clientEP);
        }

    }
}
