using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server
{
    public class Circle
    {
        public Vector2 position;
        public float radius;

        public bool IsCollision(Circle circle)
        {
            var dx = position.X - circle.position.X;
            var dy = position.Y - circle.position.Y;
            var distance = Math.Pow(dx * dx + dy * dy, 2);
            Console.WriteLine("Player position:" + position);
            Console.WriteLine("Food position: " + circle.position);
            Console.WriteLine(distance);
            Console.WriteLine("radius + circle.radius" + (radius + circle.radius));
            if (distance < radius + circle.radius)
            {
                return true;
            }
            return false;
        }
    }
}
