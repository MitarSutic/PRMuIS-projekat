using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Common.Net;

namespace Server
{
     class UdpGameServer
    {
        private UdpClient udpClient;
        private IPEndPoint clientEP;

        public UdpGameServer(int port)
        {
            udpClient = new UdpClient(port);
            clientEP = new IPEndPoint(IPAddress.Any, 0);
        }

        public void SendState(GameState state)
        {
            byte[] data = BinarySerializer.Serialize(state);
            udpClient.Send(data, data.Length, clientEP);
        }

        public void ReceiveClientEndpoint()
        {
            byte[] data = udpClient.Receive(ref clientEP);
        }
    }
}
