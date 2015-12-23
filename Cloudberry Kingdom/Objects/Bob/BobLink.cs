using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Bobs
{
    public class BobLink
    {
        public int _j, _k;
        public Bob j, k;
        public float L, a_in, a_out, MaxForce;

        public void Release()
        {
            j = null;
            k = null;
        }

        public BobLink()
        {
            j = k = null;

            L = 0;
            a_out = 0;

            ////a_in = .005f;
            ////MaxForce = 5;

            a_in = .00525f;
            MaxForce = 5.15f;
        }

        public bool Inactive
        {
            get
            {
                // Don't draw the bungee if we are a dead spaceship or if we explode on death and are dead
                if ((Bob.AllExplode && !Bob.ShowCorpseAfterExplode) || j.Core.MyLevel.DefaultHeroType is BobPhsxSpaceship && (j.Dead || j.Dying || k.Dead || k.Dying))
                    return true;

                // Don't draw the bungee if one of the players isn't being drawn.
                if (!j.Core.Show || !k.Core.Show)
                    return true;

                return false;
            }
        }

        public void Draw()
        {
            if (Inactive) return;

            Draw(j.Core.Data.Position, k.Core.Data.Position);
        }

        public static void Draw(Vector2 p1, Vector2 p2)
        {
            Tools.QDrawer.DrawLine(p1, p2, Color.WhiteSmoke, 15);
        }

        public void PhsxStep(Bob bob)
        {
            if (Inactive) return;

            float Length = (j.Core.Data.Position - k.Core.Data.Position).Length();
            
            Vector2 Tangent = (j.Core.Data.Position - k.Core.Data.Position);
            
            if (Length < 1) Tangent = Vector2.Zero;
            else Tangent /= Length;

            float Force;
            if (Length < L) Force = a_out * (Length - L);
            else Force = a_in * (Length - L);
            if (Math.Abs(Force) > MaxForce)
                Force = Math.Sign(Force) * MaxForce;

            Vector2 Bottom = Vector2.Min(j.Core.Data.Position, k.Core.Data.Position);
            if (bob.Core.Data.Position.Y > Bottom.Y)
                Force /= 5;

            Vector2 VectorForce = Force * Tangent;
            if (bob == j) VectorForce *= -1;

            Tangent = VectorForce;
            Tangent.Normalize();
            float v = Vector2.Dot(bob.Core.Data.Velocity, Tangent);
            if (v < 25)
                bob.Core.Data.Velocity += VectorForce;
        }

        public void Connect(Bob bob1, Bob bob2)
        {
            j = bob1;
            k = bob2;
            if (bob1.MyBobLinks == null) bob1.MyBobLinks = new List<BobLink>();
            if (bob2.MyBobLinks == null) bob2.MyBobLinks = new List<BobLink>();
            bob1.MyBobLinks.Add(this);
            bob2.MyBobLinks.Add(this);
        }
    }
}