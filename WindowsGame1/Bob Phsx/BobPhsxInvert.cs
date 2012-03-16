using Microsoft.Xna.Framework;
using Drawing;
using System;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BobPhsxInvert : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Invert";
            Adjective = "Invert";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Invert"), Color.White, 1.2f * DefaultIconWidth);
        }
        static readonly BobPhsxInvert instance = new BobPhsxInvert();
        public static BobPhsxInvert Instance { get { return instance; } }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_Jetpack();
        }

        // Instancable class
        public BobPhsxInvert() { }

        public override void Init(Bobs.Bob bob)
        {
            base.Init(bob);

            BobJumpAccel = -Math.Abs(BobJumpAccel);

            Gravity = Math.Abs(Gravity);
            BobInitialJumpSpeed = Math.Abs(BobInitialJumpSpeed);
            BobInitialJumpSpeed2 = Math.Abs(BobInitialJumpSpeed2);
            BobInitialJumpSpeedDucking = Math.Abs(BobInitialJumpSpeedDucking);
            BobInitialJumpSpeedDucking2 = Math.Abs(BobInitialJumpSpeedDucking2);
            BobMaxFallSpeed = -Math.Abs(BobMaxFallSpeed);
        }

        protected override void DoJump()
        {
            BobJumpAccel *= -1;

            base.DoJump();

            Gravity *= -1;
            ForceDown *= -1;
            BobMaxFallSpeed *= -1;

            BobInitialJumpSpeed *= -1;
        }

        public override void LandOnSomething(bool MakeReadyToJump)
        {
            if (Gravity < 0)
                base.HitHeadOnSomething();
            else
                base.LandOnSomething(MakeReadyToJump);
        }

        public override void HitHeadOnSomething()
        {
            if (Gravity < 0)
                base.LandOnSomething(false);
            else
                base.HitHeadOnSomething();                
        }

        public override bool ShouldStartJumpAnim()
        {
            return StartJumpAnim;
        }

        protected override void SetTarget(Levels.RichLevelGenData GenData)
        {
            base.SetTarget(GenData);

            if (MyBob.TargetPosition.Y > Cam.Pos.Y)
                MyBob.TargetPosition.Y -= 900;
        }

        protected override void PreventEarlyLandings(Levels.RichLevelGenData GenData)
        {
            // Do nothing
        }

        public override void ModData(ref Level.MakeData makeData, StyleData Style)
        {
            base.ModData(ref makeData, Style);

            var Ceiling_Params = (Ceiling_Parameters)Style.FindParams(Ceiling_AutoGen.Instance);
            Ceiling_Params.Make = false;
        }
    }
}