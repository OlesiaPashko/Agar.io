using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
namespace Server
{
    public interface IGameManager
    {
        public void StartGame();
        public void UpdateField(UdpClient udpServer, IPEndPoint remoteEP);
    }
    public class GameManager : IGameManager
    {
        public static ConcurrentBag<Player> players = new ConcurrentBag<Player>();
        private readonly IIdGenerator idGenerator;
        private readonly IConnectionManager connectionManager;
        private readonly IFoodManager foodManager;
        public List<Circle> SpawnedFood => new List<Circle>();
        public Vector2 CreateFood()
        {
            Random r = new Random();
            float x = r.Next(-50, 50);
            float y = r.Next(-50, 50);
            Vector2 foodPosition = new Vector2(x, y);
            SpawnedFood.Add(new Circle { position = foodPosition, radius = 50f });
            return foodPosition;
        }
        public GameManager(IIdGenerator idGenerator, IConnectionManager connectionManager, IFoodManager foodManager)
        {
            this.idGenerator = idGenerator;
            this.connectionManager = connectionManager;
            this.foodManager = foodManager;
        }
        public void StartGame()
        {
            UdpClient udpServer = new UdpClient(11000);

            var remoteEP = new IPEndPoint(IPAddress.Any, 11000);

            remoteEP = connectionManager.ReceiveUsername(udpServer, remoteEP);

            Console.Write("receive data from " + remoteEP.ToString());
            int id = idGenerator.GetId();
            udpServer.Send(BitConverter.GetBytes(id), 4, remoteEP); // reply back
            players.Add(new Player { Id = id, radius = 50 });
            UpdateField(udpServer, remoteEP);
        }
        public void UpdateField(UdpClient udpServer, IPEndPoint remoteEP)
        {
            DateTime _nextLoop = DateTime.Now;

            Thread thread = new Thread(() => connectionManager.SendFoodPosition(udpServer, remoteEP));
            thread.Start();
            while (true)
            {
                while (_nextLoop < DateTime.Now)
                {
                    foreach (var player in players)
                    {
                        UpdatePlayerPosition(udpServer, remoteEP, player);
                    }
                    CheckCollisions(udpServer, remoteEP);
                    _nextLoop = _nextLoop.AddMilliseconds(20);
                    if (_nextLoop > DateTime.Now)
                    {
                        // If the execution time for the next tick is in the future, aka the server is NOT running behind
                        Thread.Sleep(_nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
                    }
                }
            }
        }

        private void UpdatePlayerPosition(UdpClient udpServer, IPEndPoint remoteEP, Player player)
        {
            Vector2 direction = connectionManager.ReceiveDirection(udpServer, remoteEP);
            connectionManager.SendPosition(udpServer, direction, player, remoteEP);
        }

        private void CheckCollisions(UdpClient udpServer, IPEndPoint remoteEP)
        {
            Console.WriteLine("-----------------------------------------------------------------------");
            var food = SpawnedFood;
            Console.WriteLine("food.Count " + food.Count);
            foreach(var player in players)
            {

                Console.WriteLine("Player");
                foreach (var foodItem in food)
                {

                    Console.WriteLine("Food");
                    if (player.IsCollision(foodItem))
                    {
                        connectionManager.SendCollision(udpServer, remoteEP, player, foodItem);
                    }
                }
            }
        }
    }
}
