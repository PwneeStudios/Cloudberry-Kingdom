using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public struct PhsxData
    {
        public Vector2 Position, Velocity, Acceleration;

        public PhsxData(float pos_x, float pos_y, float vel_x, float vel_y, float acc_x, float acc_y)
        {
            Position = new Vector2(pos_x, pos_y);
            Velocity = new Vector2(vel_x, vel_y);
            Acceleration = new Vector2(acc_x, acc_y);
        }

        public void UpdatePosition() { Position += Velocity; }

        public void Integrate()
        {
            Velocity += Acceleration;
            Position += Velocity;
        }
    }
}