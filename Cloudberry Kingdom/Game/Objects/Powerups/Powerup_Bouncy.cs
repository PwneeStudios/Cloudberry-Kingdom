using System;
using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class Powerup_Bouncy : InteractWithBlocks
    {
        public Powerup_Bouncy() : base(false)
        {
            MyQuad = new QuadClass("BouncyBlock2", 115);
            MyQuad.Quad.MirrorUV_Vertical();
            Size = MyQuad.Size * .965f;
            MySound = Tools.SoundWad.FindByName("BoxHero_Land");
        }

        public override void OnTouch(Bob bob)
        {
            base.OnTouch(bob);

            if (bob.MyPhsx is BobPhsxBouncy)
                return;

            // Get rid of the Bouncy
            this.CollectSelf();

            // Particle effect
            ParticleEffects.HeroExplosion(Core.MyLevel, Pos);
            ParticleEffects.HeroExplosion(Core.MyLevel, bob.Pos);

            // Change bob into Bouncy hero
            BobPhsx hero = BobPhsxBouncy.Instance;
            Core.MyLevel.SwitchHeroType(bob, hero);

            int count = 0;
            Game.AddToDo(() =>
            {
                count++;
                if (count < 10)
                {
                    bob.MyPhsx.Vel = new Vector2(0, 0);
                    return false;
                }
                else
                {
                    bob.MyPhsx.Vel = new Vector2(0, 25);
                    return true;
                }
            });
        }
    }
}