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
        public static new BobPhsxInvert Instance { get { return instance; } }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_Jetpack();
        }

        // Instancable class
        public BobPhsxInvert()
        {
            Set(this);
        }

        public override void Set(BobPhsx phsx)
        {
            Set(phsx, Vector2.One);
        }
        public void Set(BobPhsx phsx, Vector2 modsize)
        {
            //MustHitGroundToReadyJump = true;

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.BobJumpLength = 1;
                normal.BobJumpAccel = 0;
                normal.BobInitialJumpSpeed = 6;
            }
        }

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

            BobPhsxNormal normal = this as BobPhsxNormal;
            if (null != normal)
            {
                normal.BobJumpLength = 1;
                normal.BobJumpAccel = 0;
                normal.BobInitialJumpSpeed = 0;
                normal.Gravity = 4;
                normal.BobMaxFallSpeed = -30;
            }
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            Obj.yFlip = Gravity < 0;

            // If we are falling (and not falling too fast already),
            // accelerate if the player is pressing (A).
            if (!OnGround && MyBob.CurInput.A_Button && AirTime > 7 && DynamicGreaterThan(yVel, BobMaxFallSpeed * 2))
                yVel -= Gravity;
        }

        public override void UpdateReadyToJump()
        {
            base.UpdateReadyToJump();

            if (MyBob.Count_ButtonA > 4)
                ReadyToJump = false;
        }

        protected override void DoJump()
        {
            BobJumpAccel *= -1;

            base.DoJump();

            Gravity *= -1;
            ForceDown *= -1;
            //BobMaxFallSpeed *= -1;

            BobInitialJumpSpeed *= -1;
        }

        public void Invert()
        {
            BobJumpAccel *= -1;

            Gravity *= -1;
            ForceDown *= -1;
            //BobMaxFallSpeed *= -1;

            BobInitialJumpSpeed *= -1;
        }

        public override void Forced(Vector2 Dir)
        {
            base.Forced(Dir);

            if (Math.Sign(Dir.Y) == Math.Sign(Gravity))
                Invert();
        }

        public override void LandOnSomething(bool MakeReadyToJump, ObjectBase ThingLandedOn)
        {
            if (Gravity < 0)
                base.HitHeadOnSomething(ThingLandedOn);
            else
                base.LandOnSomething(MakeReadyToJump, ThingLandedOn);
        }

        public override void HitHeadOnSomething(ObjectBase ThingHit)
        {
            if (Gravity < 0)
                base.LandOnSomething(false, ThingHit);
            else
                base.HitHeadOnSomething(ThingHit);
        }

        public override bool ShouldStartJumpAnim()
        {
            return StartJumpAnim;
        }

        enum Behavior { Pause, Regular };
        Behavior CurBehavior = Behavior.Pause;
        int BehaviorLength;
        public override void GenerateInput(int CurPhsxStep)
        {
            base.GenerateInput(CurPhsxStep);

            // Change behavior
            if (CurPhsxStep < 10)
            {
                CurBehavior = Behavior.Pause;
                BehaviorLength = 0;
            }
            else
            {
                if (BehaviorLength == 0)
                {
                    if (MyLevel.Rnd.RndFloat() > .7f)
                    {
                        CurBehavior = Behavior.Pause;
                        BehaviorLength = MyLevel.Rnd.RndInt(5, 10);
                        //BehaviorLength = MyLevel.Rnd.RndInt(5, 40);
                    }
                    else
                    {
                        CurBehavior = Behavior.Regular;
                        BehaviorLength = MyLevel.Rnd.RndInt(25, 60);
                    }
                }
                else
                    BehaviorLength--;
            }

            // Act according to behavior
            switch (CurBehavior)
            {
                case Behavior.Pause:
                    MyBob.CurInput.xVec.X = 0;
                    break;
                case Behavior.Regular:
                    break;
            }
        }

        int Count = 0;
        protected override void SetTarget(Levels.RichLevelGenData GenData)
        {
            //base.SetTarget(GenData);

            if (Count <= 0 || Math.Abs(MyBob.TargetPosition.Y - Pos.Y) < 200)
            {
                Count = MyLevel.Rnd.RndInt(30, 60);
                //MyBob.TargetPosition.Y = MyLevel.Rnd.RndFloat(MyBob.MoveData.MinTargetY, MyBob.MoveData.MaxTargetY);

                if (Pos.Y > Cam.Pos.Y)
                    MyBob.TargetPosition.Y = MyBob.MoveData.MinTargetY;
                else
                    MyBob.TargetPosition.Y = MyBob.MoveData.MaxTargetY - MyLevel.Rnd.RndFloat(400, 900);

                if (MyLevel.Rnd.RndFloat() > .85f)
                {
                    CurBehavior = Behavior.Pause;
                    BehaviorLength = MyLevel.Rnd.RndInt(15, 30);
                }

                //MyLevel.Rnd.RndFloat(Cam.Pos.Y, MyBob.MoveData.MaxTargetY);
            }
            else
                Count--;

            //if (MyBob.TargetPosition.Y > Cam.Pos.Y)
            //    MyBob.TargetPosition.Y -= 900;
        }

        protected override void PreventEarlyLandings(Levels.RichLevelGenData GenData)
        {
            // Do nothing
        }

        public override void ModData(ref Level.MakeData makeData, StyleData Style)
        {
            base.ModData(ref makeData, Style);

            makeData.TopLikeBottom = true;
            makeData.BlocksAsIs = true;

            var Ceiling_Params = (Ceiling_Parameters)Style.FindParams(Ceiling_AutoGen.Instance);
            Ceiling_Params.Make = false;

            Style.BlockFillType = StyleData._BlockFillType.Invertable;

            Style.TopSpace = 50;

            var MParams = (MovingBlock_Parameters)Style.FindParams(MovingBlock_AutoGen.Instance);
            if (MParams.Aspect == MovingBlock_Parameters.AspectType.Tall)
                MParams.Aspect = MovingBlock_Parameters.AspectType.Thin;
        }

        public override bool IsBottomCollision(ColType Col, AABox box, BlockBase block)
        {
            //return base.IsBottomCollision(Col, box, block);
            return Col == ColType.Bottom ||
                Col != ColType.Bottom && Core.Data.Velocity.X != 0 && Math.Min(MyBob.Box.Current.TR.Y, MyBob.Box.Target.TR.Y) < box.Target.BL.Y + Math.Max(1.35 * Core.Data.Velocity.Y, 7);
        }
    }
}