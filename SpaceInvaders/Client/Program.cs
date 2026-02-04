using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.Models;
using Common.Net;
using Newtonsoft.Json;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client pokrenut...");

            UdpClient udp = new UdpClient(6001);
            udp.Client.ReceiveTimeout = 50;
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 5001);

            // handshake
            udp.Send(new byte[] { 1 }, 1, serverEP);

            Renderer renderer = new Renderer();

            while (true)
            {
                SendInput(udp, serverEP);
                ReceiveState(udp, renderer);
            }
        }

        static void SendInput(UdpClient udp, IPEndPoint ep)
        {
            if (!Console.KeyAvailable) return;

            ConsoleKey key = Console.ReadKey(true).Key;
            InputCommand cmd = null;

            if (key == ConsoleKey.LeftArrow)
                cmd = new InputCommand(InputType.LEFT);

            else if (key == ConsoleKey.RightArrow)
                cmd = new InputCommand(InputType.RIGHT);

            else if (key == ConsoleKey.Spacebar)
                cmd = new InputCommand(InputType.SHOOT);

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

                renderer.Draw(state);
            }
            catch (System.Net.Sockets.SocketException)
            {
                // nema paketa trenutno
            }

        }
    }
}
