using System;
using System.Linq;
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


        // pomocni brojac za slanje stanja
        private int sendTick = 0;

        public GameServer(Igrac igrac)
        {
            engine = new GameEngine(igrac);

            // UDP server sluša na portu 5001
            udp = new UdpClient(5001);

            // endpoint klijenta (biće popunjen nakon handshake-a)
            clientEP = new IPEndPoint(IPAddress.Any, 0);
        }

        public void Run()
        {
            try
            {
                // === UDP HANDSHAKE (blokirajuci) ===
                Console.WriteLine("Cekam UDP handshake...");
                udp.Receive(ref clientEP);
                Console.WriteLine("UDP klijent: " + clientEP);

                // posle handshake-a input postaje neblokirajuci
                udp.Client.ReceiveTimeout = 1;

                Console.WriteLine("Game loop started (UDP sending)...");

                // === GAME LOOP ===
                while (engine.State.Igrac.BrojZivota > 0)
                {
                    ReceiveInput();   // neblokirajuci input
                    engine.Update();  // logika igre

                    sendTick++;
                    if (sendTick % 2 == 0)   // saljemo stanje svaki drugi ciklus
                    {
                        SendState();
                    }

                    Thread.Sleep(150); // kontrola brzine igre
                }

                // === GAME OVER ===
                engine.State.Status = Status.GAME_OVER;
                engine.State.RangLista = engine.Igraci.OrderByDescending(i => i.BrojPoena) .ThenByDescending(i => i.BrojZivota)
    .ToList();

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
                // nema inputa u ovom ciklusu – normalno za UDP
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
