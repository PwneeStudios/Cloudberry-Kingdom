using System;
using System.IO;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    public class Powerup_Jetpack : InteractWithBlocks
    {
        QuadClass Fire;
        public Powerup_Jetpack() : base(false)
        {
            bool FireStyle = false; // If true powerup is a fireball in a bubble

            Fire = new QuadClass("White", 110);
            if (FireStyle)
                Fire.Quad.MyTexture = Fireball.EmitterTexture;
            else
            {
                Fire.Quad.TextureName = "powerup_jetman";
                Fire.ScaleXToMatchRatio();
            }

            MyQuad = new QuadClass("Bubble", 115);
            Size = MyQuad.Size * .965f;
            MySound = Tools.SoundWad.FindByName("BoxHero_Land");
            if (!FireStyle)
                MyQuad.Show = false;
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

            // Get rid of the Jetpack
            this.CollectSelf();

            // Particle effect
            ParticleEffects.HeroExplosion(Core.MyLevel, Pos);
            ParticleEffects.HeroExplosion(Core.MyLevel, bob.Pos);

            // Change bob into Jetpack hero
            BobPhsx hero = BobPhsxJetman.Instance;
            Core.MyLevel.SwitchHeroType(bob, hero);
        }
    }
}