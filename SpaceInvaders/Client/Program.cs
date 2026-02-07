using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Models;
using Common.Net;
using Newtonsoft.Json;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PRMuIS Client";
            Console.CursorVisible = true;

            Console.WriteLine("=== PRIJAVA IGRACA ===");
            Console.Write("Unesi ime: ");
            string ime = Console.ReadLine();

            Console.Write("Unesi prezime: ");
            string prezime = Console.ReadLine();

            // === TCP LOGIN ===
            Igrac igrac = new Igrac
            {
                Ime = ime,
                Prezime = prezime
            };

            try
            {
                TcpClient tcp = new TcpClient("127.0.0.1", 5000);
                NetworkStream stream = tcp.GetStream();

                byte[] data = BinarySerializer.Serialize(igrac);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Server: " + response);
                tcp.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greska pri TCP loginu: " + ex.Message);
                Console.ReadLine();
                return;
            }

            // === UDP DEO IGRE ===
            Console.Clear();
            Console.CursorVisible = false;
            Console.WriteLine("Client pokrenut...");

            UdpClient udp = new UdpClient();
            udp.Client.ReceiveTimeout = 100;

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 5001);

            // UDP handshake
            udp.Send(new byte[] { 1 }, 1, serverEP);

            Renderer renderer = new Renderer();

            Console.WriteLine("Povezan sa serverom. Igra pocinje...");
            Thread.Sleep(500);

            while (true)
            {
                var start = DateTime.Now;

                SendInput(udp, serverEP);
                ReceiveState(udp, renderer);

                var elapsed = (DateTime.Now - start).Milliseconds;
                int frameTime = 100; // ~10 FPS

                if (elapsed < frameTime)
                    Thread.Sleep(frameTime - elapsed);
            }
        }

        static void SendInput(UdpClient udp, IPEndPoint ep)
        {
            if (!Console.KeyAvailable)
                return;

            ConsoleKey key = Console.ReadKey(true).Key;
            InputCommand cmd = null;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    cmd = new InputCommand(InputType.LEFT);
                    break;

                case ConsoleKey.RightArrow:
                    cmd = new InputCommand(InputType.RIGHT);
                    break;

                case ConsoleKey.Spacebar:
                    cmd = new InputCommand(InputType.SHOOT);
                    break;
            }

            if (cmd != null)
            {
                byte[] data = BinarySerializer.Serialize(cmd);
                udp.Send(data, data.Length, ep);
            }
        }

        static void ReceiveState(UdpClient udp, Renderer renderer)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udp.Receive(ref ep);

                string json = Encoding.UTF8.GetString(data);
                GameState state = JsonConvert.DeserializeObject<GameState>(json);

                if (state == null)
                    return;

                renderer.Draw(state);

                if (state.Status == Status.GAME_OVER)
                {
                    Console.WriteLine();
                    Console.WriteLine("GAME OVER!");
                    Console.WriteLine("Poeni: " + state.Igrac.BrojPoena);
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            catch (SocketException)
            {
                // nema paketa u ovom ciklusu
            }
        }
    }
}
