using System;
using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class Powerup_DoubleJump : InteractWithBlocks
    {
        QuadClass Fire;
        public Powerup_DoubleJump() : base(false)
        {
            Fire = new QuadClass("powerup_double", 110);

            MyQuad = new QuadClass("Bubble", 115);
            Size = MyQuad.Size * .965f;
            MySound = Tools.SoundWad.FindByName("BoxHero_Land");
        }

        public override void MyDraw()
        {
            Fire.Pos = MyQuad.Pos;
            Fire.Draw();

            base.MyDraw();
        }

        public override void OnTouch(Bob bob)
        {
            base.OnTouch(bob);

            if (bob.MyPhsx is BobPhsxJetman)
                return;

            // Get rid of the DoubleJump
            this.CollectSelf();

            // Particle effect
            ParticleEffects.HeroExplosion(Core.MyLevel, Pos);
            ParticleEffects.HeroExplosion(Core.MyLevel, bob.Pos);

            // Change bob into DoubleJump hero
            BobPhsx hero = BobPhsxDouble.Instance;
            Core.MyLevel.SwitchHeroType(bob, hero);
        }
    }
}