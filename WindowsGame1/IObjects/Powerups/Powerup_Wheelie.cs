using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class Powerup_Wheelie : InteractWithBlocks
    {
        public Powerup_Wheelie() : base(false)
        {
            MyQuad = new QuadClass("Wood_Circle", 115);
            Size = MyQuad.Size;
            MySound = Tools.SoundWad.FindByName("BoxHero_Land");
        }

        public override void OnTouch(Bob bob)
        {
            base.OnTouch(bob);

            if (bob.MyPhsx is BobPhsxWheel)
                return;

            // Get rid of the Wheelie
            this.CollectSelf();

            // Particle effect
            ParticleEffects.HeroExplosion(Core.MyLevel, Pos);

            // Change bob into Wheelie hero
            BobPhsx hero = BobPhsxWheel.Instance;
            Core.MyLevel.SwitchHeroType(bob, hero);
        }
    }
}