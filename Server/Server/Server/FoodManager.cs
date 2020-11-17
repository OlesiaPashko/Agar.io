using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Server
{
    public interface IFoodManager
    {
        public List<Circle> SpawnedFood { get;}
        public Vector2 CreateFood(out int id);
    }
    public class FoodManager : IFoodManager
    {
        public List<Circle> SpawnedFood { get; set; }
        private readonly IIdGenerator idGenerator;
        public FoodManager(IIdGenerator idGenerator)
        {
            this.idGenerator = idGenerator;
            SpawnedFood = new List<Circle>();
            Console.WriteLine("In constructor");
        }
        public Vector2 CreateFood(out int id)
        {
            Random r = new Random();
            float x = r.Next(-50, 50);
            float y = r.Next(-50, 50);
            Vector2 foodPosition = new Vector2(x, y);
            id = idGenerator.GetId();
            SpawnedFood.Add(new Circle { position = foodPosition, radius = 0.2f, id = id });
            Console.Write("+++++++++++++++++++++++++");
            Console.WriteLine("SpawnedFood.Count " + SpawnedFood.Count);
            return foodPosition;
        }
    }
}
