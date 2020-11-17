using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var serviceProvider = SetUpDependencyInjection();

            IGameManager gameManager = serviceProvider.GetService<IGameManager>();
            gameManager.StartGame();
        }
        
        static ServiceProvider SetUpDependencyInjection()
        {
            var serviceProvider = new ServiceCollection()
           //.AddLogging()
           .AddSingleton<IIdGenerator, IdGenerator>()
           .AddSingleton<IConnectionManager, ConnectionManager>()
           .AddSingleton<IGameManager, GameManager>()
           .AddSingleton<IFoodManager, FoodManager>()
           .BuildServiceProvider();

            return serviceProvider;
            /*
            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");
            */
            //do the actual work here
        }
    }
}
