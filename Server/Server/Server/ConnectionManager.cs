using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Server
{
    public interface IConnectionManager
    {
        public IPEndPoint ReceiveUsername(UdpClient udpServer, IPEndPoint remoteEP);
        public Vector2 ReceiveDirection(UdpClient udpServer, IPEndPoint remoteEP);
        public void SendPosition(UdpClient udpServer, Vector2 direction, Player player, IPEndPoint remoteEP);
        public void SendFoodPosition(UdpClient udpServer, IPEndPoint remoteEP);
        public void SendCollision(UdpClient udpServer, IPEndPoint remoteEP, Player player, Circle circle);
    }
    public class ConnectionManager : IConnectionManager
    {
        private readonly IFoodManager foodManager;
        public ConnectionManager(IFoodManager foodManager)
        {
            this.foodManager = foodManager;
        }
        public IPEndPoint ReceiveUsername(UdpClient udpServer, IPEndPoint remoteEP)
        {
            var data = udpServer.Receive(ref remoteEP); // listen on port 11000
            string value;
            try
            {
                value = Encoding.ASCII.GetString(data, 0, data.Length); // Convert the bytes to a string
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
            Console.WriteLine(value);
            return remoteEP;
        }

        public Vector2 ReceiveDirection(UdpClient udpServer, IPEndPoint remoteEP)
        {
            var directionBytes = udpServer.Receive(ref remoteEP); // listen on port 11000
            Console.Write("receive data from " + remoteEP.ToString());
            float x, y;
            try
            {
                x = BitConverter.ToSingle(directionBytes, 0); // Convert the bytes to a string
                y = BitConverter.ToSingle(directionBytes, 4);
                Console.WriteLine($"x = {x}; y = {y}");
                return new Vector2(x, y);
            }
            catch
            {
                throw new Exception("Could not read value of type '2 floats'!");
            }

        }

        public void SendPosition(UdpClient udpServer, Vector2 direction, Player player, IPEndPoint remoteEP)
        {
            Vector2 position = player.UpdatePosition(direction);
            Console.WriteLine(position);
            List<byte> message = new List<byte>();
            message.AddRange(BitConverter.GetBytes(0));
            message.AddRange(BitConverter.GetBytes(position.X));
            message.AddRange(BitConverter.GetBytes(position.Y));
            udpServer.Send(message.ToArray(), message.Count, remoteEP);
        }

        public void SendFoodPosition(UdpClient udpServer, IPEndPoint remoteEP)
        {
            while (true)
            {
                Vector2 position = foodManager.CreateFood();
                Console.WriteLine("Food spawned = " + position);
                List<byte> message = new List<byte>();
                message.AddRange(BitConverter.GetBytes(1));
                message.AddRange(BitConverter.GetBytes(position.X));
                message.AddRange(BitConverter.GetBytes(position.Y));
                udpServer.Send(message.ToArray(), message.Count, remoteEP);
                Thread.Sleep(100);
            }
        }

        public void SendCollision(UdpClient udpServer, IPEndPoint remoteEP, Player player, Circle circle)
        {
            Console.WriteLine("Collisiont = " + player.position + "  " + circle.position);
            List<byte> message = new List<byte>();
            message.AddRange(BitConverter.GetBytes(2));
            message.AddRange(BitConverter.GetBytes(player.Id));
            message.AddRange(BitConverter.GetBytes(circle.position.X));
            message.AddRange(BitConverter.GetBytes(circle.position.Y));
            udpServer.Send(message.ToArray(), message.Count, remoteEP);            
        }
    }
}
