using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server
{
    public class Player: Circle
    {
        public Vector2 velocity = new Vector2(0, 0);
        public float speed = 10f;
        public float calcTime = 0.1f;//we calculate time 100 times per second
        public int Id { get; set; }
        public Vector2 UpdatePosition(Vector2 direction)
        {
            velocity = new Vector2(direction.X , direction.Y);
            position += calcTime * velocity;
            return position;
        }
    }
}
