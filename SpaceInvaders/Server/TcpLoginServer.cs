using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.Models;
using Common.Net;

namespace Server
{
    class TcpLoginServer
    {
        private TcpListener listener;
        private List<Igrac> igraci = new List<Igrac>();
        private List<NetworkStream> streams = new List<NetworkStream>();
        private int maxPlayers = 1;

        public TcpLoginServer()
        {
            listener = new TcpListener(IPAddress.Any, 5000);
        }

        // Vraca listu igraca (1 ili 2)
        public List<Igrac> Start()
        {
            listener.Start();
            Console.WriteLine("TCP Login server pokrenut...");

            while (igraci.Count < maxPlayers)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Klijent povezan.");

                NetworkStream stream = client.GetStream();
                streams.Add(stream);

                // ===== PRIMANJE IGRACA =====
                byte[] buffer = new byte[2048];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                byte[] data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);

                Igrac igrac = BinarySerializer.Deserialize<Igrac>(data);

                // ===== PRIMANJE MODA (SAMO PRVI IGRAC) =====
                if (igraci.Count == 0)
                {
                    int mode = stream.ReadByte(); // 1 ili 2
                    maxPlayers = (mode == 2) ? 2 : 1;
                    Console.WriteLine($"Izabran mod: {maxPlayers} igrac(a)");
                }

                // ===== INICIJALIZACIJA IGRACA =====
                igrac.BrojZivota = 3;
                igrac.BrojPoena = 0;
                igrac.Y = 19;
                igrac.X = (igraci.Count == 0) ? 10 : 30;

                igraci.Add(igrac);
                Console.WriteLine($"Prijavljen igrac: {igrac.Ime} {igrac.Prezime}");

                // ===== AKO CEKA DRUGOG IGRACA =====
                if (igraci.Count < maxPlayers)
                {
                    byte[] waitMsg = Encoding.UTF8.GetBytes("CEKANJE");
                    stream.Write(waitMsg, 0, waitMsg.Length);
                    // konekcija ostaje otvorena
                }
            }

            // ===== SVI IGRACI SU TU → SALJEMO START SVIMA =====
            byte[] startMsg = Encoding.UTF8.GetBytes("START");

            foreach (var stream in streams)
            {
                stream.Write(startMsg, 0, startMsg.Length);
                stream.Close();
            }

            listener.Stop();
            Console.WriteLine("Svi igraci prijavljeni. Igra pocinje.");

            return igraci;
        }
    }
}
