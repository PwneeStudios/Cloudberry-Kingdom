using Microsoft.Xna.Framework;
using System;

using CoreEngine;
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

            Specification = new HeroSpec(6, 0, 0, 0);
            Name = Localization.Words.Rocketbox;
            NameTemplate = "rocketbox";
            //Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Cart"), Color.White, DefaultIconWidth);
            Icon = new PictureIcon(Tools.TextureWad.FindByName("CartIcon"), Color.White, DefaultIconWidth * 1.315f);
            ((PictureIcon)Icon).IconQuad.Quad.Shift(new Vector2(0, 0));
        }
        static readonly BobPhsxRocketbox instance = new BobPhsxRocketbox();
        public static new BobPhsxRocketbox Instance { get { return instance; } }

        // Instancable class
        public BobPhsxRocketbox()
        {
        }

        public override void Init(Bob bob)
        {
            StandAnim = 17; JumpAnim = 18; DuckAnim = 19;

            ExtraQuadString = "MainQuad";
            ExtraTextureString = "CartAlone";

            CapeOffset += new Vector2(-50, 50);
            CapeOffset_Ducking += new Vector2(-50, 50);

            base.Init(bob);
            
            if (Prototype != null && MyBob.PlayerObject != null && MyBob.PlayerObject.QuadList != null)
            {
                LeftWheel = MyBob.PlayerObject.FindQuad("Wheel_Left") as Quad;
                LeftWheel.Show = true;
                LeftWheel.SetColor(ColorHelper.GrayColor(.5f));
                LeftWheel.MyEffect = Tools.HslGreenEffect;
                RightWheel = MyBob.PlayerObject.FindQuad("Wheel_Right") as Quad;
                RightWheel.Show = true;
                RightWheel.MyEffect = Tools.HslGreenEffect;
                RightWheel.SetColor(ColorHelper.GrayColor(.5f));
            }
        }

        public override void SideHit(ColType side, BlockBase block)
        {
            base.SideHit(side, block);

            if (MyBob.CanDie)
                MyBob.Die(BobDeathType.Other);
        }

        public override void DefaultValues()
        {
            base.DefaultValues();

            MaxSpeed = 30;
            XAccel = .38f;
        }

        public override void DoXAccel()
        {
            bool HoldDucking = Ducking;
            Ducking = false;
            MyBob.CurInput.xVec.X = 1;
            base.ParentDoXAccel();
            Ducking = HoldDucking;
        }

        Quad LeftWheel, RightWheel;
        float WheelAngle = 0;
        public override void AnimStep()
        {
            base.AnimStep();

            if (MyBob.CoreData.MyLevel.PlayMode == 0)
            {
                WheelAngle -= xVel * 1f / 60f;

                LeftWheel.MyEffect = Tools.HslEffect;
                RightWheel.MyEffect = Tools.HslEffect;
                LeftWheel.PointxAxisTo(CoreMath.AngleToDir(WheelAngle));
                RightWheel.PointxAxisTo(CoreMath.AngleToDir(WheelAngle));
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
            if (MyBob.CoreData.MyLevel.PlayMode == 0)
            {
                //Vector2 pos = new Vector2(-45f, -30);
                //pos.Y -= 40;
                //Vector2 dir = new Vector2(-1.115f, -.025f);

                //if (MyBob.PlayerObject.xFlip)
                //{
                //    pos.X *= -1;
                //    dir.X *= -1;
                //}
                //pos += Pos;

                //int layer = Math.Max(1, MyBob.Core.DrawLayer - 1);
                //ParticleEffects.CartThrust(MyBob.Core.MyLevel, layer, pos, dir, Vector2.Zero);

                int layer = Math.Max(1, MyBob.CoreData.DrawLayer - 1);
                float intensity = 1.3f * Math.Min(.3f + (MyBob.CurInput.xVec.X + .3f), 1f);
                ParticleEffects.Thrust(MyBob.CoreData.MyLevel, layer, Pos + new Vector2(0, -20), new Vector2(-1, 0), new Vector2(-4, yVel), intensity);
            }
        }
    }
}