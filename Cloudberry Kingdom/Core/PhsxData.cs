using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public struct PhsxData
    {
        public PhsxData(float pos_x, float pos_y, float vel_x, float vel_y, float acc_x, float acc_y)
        {
            Position.X = pos_x;
            Position.Y = pos_y;
            Velocity.X = vel_x;
            Velocity.Y = vel_y;
            Acceleration.X = acc_x;
            Acceleration.Y = acc_y;
        }

        public Vector2 Position, Velocity, Acceleration;

        public void UpdatePosition() { Position += Velocity; }

        public void Integrate()
        {
            Velocity += Acceleration;
            Position += Velocity;
        }
    }
}