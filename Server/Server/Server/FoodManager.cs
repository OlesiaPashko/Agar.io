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
        public Vector2 CreateFood();
    }
    public class FoodManager : IFoodManager
    {
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
    }
}
