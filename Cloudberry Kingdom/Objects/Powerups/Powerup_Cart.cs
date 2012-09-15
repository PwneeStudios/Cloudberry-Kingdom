using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class Powerup_Cart : InteractWithBlocks
    {
        public Powerup_Cart() : base(false)
        {
            MyQuad = new QuadClass("Cart", 115);
            Size = MyQuad.Size;
            MySound = Tools.SoundWad.FindByName("BoxHero_Land");
        }

        public override void OnTouch(Bob bob)
        {
            base.OnTouch(bob);

            if (bob.MyPhsx is BobPhsxRocketbox)
                return;

            // Get rid of the cart
            this.CollectSelf();

            // Particle effect
            ParticleEffects.HeroExplosion(Core.MyLevel, Pos);

            // Change bob into cart hero
            BobPhsx hero = BobPhsxRocketbox.Instance;
            Core.MyLevel.SwitchHeroType(bob, hero);
        }
    }
}