using System;
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
        private Igrac igrac;

        public TcpLoginServer()
        {
            listener = new TcpListener(IPAddress.Any, 5000);
        }

        public Igrac Start()
        {
            listener.Start();
            Console.WriteLine("TCP Login server pokrenut (1 igrac)...");

            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Klijent povezan.");

            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[2048];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            byte[] data = new byte[bytesRead];
            Array.Copy(buffer, data, bytesRead);

            igrac = BinarySerializer.Deserialize<Igrac>(data);

            igrac.X = 20;
            igrac.Y = 19;

            Console.WriteLine("Prijavljen igrac: " +
                igrac.Ime + " " + igrac.Prezime);

            string odgovor = "USPESNA_PRIJAVA";
            byte[] response = Encoding.UTF8.GetBytes(odgovor);
            stream.Write(response, 0, response.Length);

            client.Close();
            listener.Stop();

            return igrac;
        }
    }
}
