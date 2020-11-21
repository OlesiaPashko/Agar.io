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
        public static List<Player> players = new List<Player>();
        private readonly IIdGenerator idGenerator;
        private readonly IConnectionManager connectionManager;
        private readonly IFoodManager foodManager;
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
            players.Add(new Player { id = id, radius = 0.5f, mass = 30 });
            UpdateField(udpServer, remoteEP);
        }
        public void UpdateField(UdpClient udpServer, IPEndPoint remoteEP)
        {
            DateTime _nextLoop = DateTime.Now;

            Thread thread = new Thread(() => connectionManager.SendFoodPosition(udpServer, remoteEP));
            thread.Start();
            //int counter = 0;
            while (true)
            {
                while (_nextLoop < DateTime.Now)
                {

                    //counter++;
                    foreach (var player in players)
                    {
                        UpdatePlayerPosition(udpServer, remoteEP, player);
                    }
                    CheckCollisions(udpServer, remoteEP);
                    //if (counter >= 10)
                    //{
                        //connectionManager.SendFoodPosition(udpServer, remoteEP);
                        //counter = 0;
                    //}
                    _nextLoop = _nextLoop.AddMilliseconds(40);
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
            var food = foodManager.SpawnedFood;
            Console.WriteLine("food.Count " + food.Count);
            for(int i = 0; i < players.Count;i++)
            {
                var player = players[i];
                Console.WriteLine("Player");
                for(int j = 0;j<food.Count;j++)
                {
                    var foodItem = food[j];
                    Console.WriteLine("Food");
                    if (player.IsCollision(foodItem))
                    {
                        player.radius += foodItem.mass / player.mass;
                        player.mass += foodItem.mass;
                        connectionManager.SendCollision(udpServer, remoteEP, player, foodItem);
                        food.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
    }
}
