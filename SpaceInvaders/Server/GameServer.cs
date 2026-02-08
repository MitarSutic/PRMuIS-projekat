using System;
using System.Collections.Generic;
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

        // UDP endpoint -> igrac
        private Dictionary<IPEndPoint, Igrac> endpointMap =
            new Dictionary<IPEndPoint, Igrac>();

        // svi UDP klijenti
        private List<IPEndPoint> clientEndpoints =
            new List<IPEndPoint>();

        private int sendTick = 0;

        public GameServer(List<Igrac> igraci)
        {
            engine = new GameEngine(igraci);

            // UDP server slusa na portu 5001
            udp = new UdpClient(5001);
        }

        public void Run()
        {
            try
            {
                // === UDP HANDSHAKE ZA SVE IGRACE ===
                Console.WriteLine("Cekam UDP handshake-e...");

                while (clientEndpoints.Count < engine.Igraci.Count)
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                    udp.Receive(ref ep);

                    if (!clientEndpoints.Any(e => e.Equals(ep)))
                    {
                        clientEndpoints.Add(ep);

                        // mapiranje endpointa na igraca (redosled prijave)
                        Igrac igrac = engine.Igraci[clientEndpoints.Count - 1];
                        endpointMap[ep] = igrac;

                        Console.WriteLine($"UDP klijent povezan: {ep} -> {igrac.Ime}");
                    }
                }

                // neblokirajuci UDP
                udp.Client.ReceiveTimeout = 1;

                Console.WriteLine("Svi UDP klijenti povezani.");
                Console.WriteLine("Game loop started (UDP sending)...");

                // === GAME LOOP ===
                while (engine.Igraci.Any(i => i.BrojZivota > 0))
                {
                    ReceiveInput();   // prima input svih igraca
                    engine.Update();  // update igre

                    sendTick++;
                    if (sendTick % 2 == 0)
                        SendState();

                    Thread.Sleep(150);
                }

                // === GAME OVER ===
                engine.State.Status = Status.GAME_OVER;
                engine.State.RangLista = engine.Igraci
                    .OrderByDescending(i => i.BrojPoena)
                    .ThenByDescending(i => i.BrojZivota)
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

                if (!endpointMap.ContainsKey(ep))
                    return;

                var cmd = BinarySerializer.Deserialize<InputCommand>(data);
                var igrac = endpointMap[ep];

                engine.HandleInput(cmd, igrac);
            }
            catch (SocketException)
            {
                // nema inputa u ovom ciklusu
            }
        }

        private void SendState()
        {
            string json = JsonConvert.SerializeObject(engine.State);
            byte[] data = Encoding.UTF8.GetBytes(json);

            foreach (var ep in clientEndpoints)
            {
                udp.Send(data, data.Length, ep);
            }
        }
    }
}
