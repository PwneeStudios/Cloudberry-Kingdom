using Microsoft.Xna.Framework;
using System;

using Drawing;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class BobPhsxRocketbox : BobPhsxBox
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Rocketbox!";
            NameTemplate = "rocketbox";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Cart"), Color.White, DefaultIconWidth);
        }
        static readonly BobPhsxRocketbox instance = new BobPhsxRocketbox();
        public static BobPhsxRocketbox Instance { get { return instance; } }

        // Instancable class

        public BobPhsxRocketbox()
        {
        }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_Cart();
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);
            //if (MyBob.Core.MyLevel.PlayMode == 0)
            if (Prototype != null && MyBob.PlayerObject != null && MyBob.PlayerObject.QuadList != null)
            {
                LeftWheel = MyBob.PlayerObject.FindQuad("Wheel_Left") as Quad;
                RightWheel = MyBob.PlayerObject.FindQuad("Wheel_Right") as Quad;
            }
        }

        public override void SideHit(ColType side, Block block)
        {
            base.SideHit(side, block);

            if (MyBob.CanDie)
                MyBob.Die(Bob.BobDeathType.Other);
        }

        public override void DefaultValues()
        {
            base.DefaultValues();

            MaxSpeed = 30;
            XAccel = .38f;
        }

        public override void DoXAccel()
        {
            MyBob.CurInput.xVec.X = 1;
            base.ParentDoXAccel();
        }

        Quad LeftWheel, RightWheel;
        float WheelAngle = 0;
        public override void AnimStep()
        {
            base.AnimStep();

            if (MyBob.Core.MyLevel.PlayMode == 0)
            {
                WheelAngle -= xVel * .33f / 60f;

                LeftWheel.PointxAxisTo(Tools.AngleToDir(WheelAngle));
                RightWheel.PointxAxisTo(Tools.AngleToDir(WheelAngle));
            }
        }

        public override void GenerateInput(int CurPhsxStep)
        {
            base.ParentGenerateInput(CurPhsxStep);

            // Full jumps
            if (!OnGround && yVel > 0)
                MyBob.CurInput.A_Button = true;
        }

        public override void PhsxStep2()
        {
            base.PhsxStep2();

            // Rocketman thrust
            if (MyBob.Core.MyLevel.PlayMode == 0)
            {
                Vector2 pos = new Vector2(-45f, -30);
                pos.Y -= 40;
                Vector2 dir = new Vector2(-1.115f, -.025f);

                if (MyBob.PlayerObject.xFlip)
                {
                    pos.X *= -1;
                    dir.X *= -1;
                }
                pos += Pos;

                int layer = Math.Max(1, MyBob.Core.DrawLayer - 1);
                ParticleEffects.CartThrust(MyBob.Core.MyLevel, layer, pos, dir, Vector2.Zero);
                //ParticleEffects.Thrust(MyBob.Core.MyLevel, layer, pos, dir, Vel / 1.5f);
            }
        }
    }
}