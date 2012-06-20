using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class BobPhsxTeleport : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Wizard";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Big"), Color.White, DefaultIconWidth);
        }
        static readonly BobPhsxTeleport instance = new BobPhsxTeleport();
        public static BobPhsxTeleport Instance { get { return instance; } }

        // Instancable class
        int AutoMoveLength, AutoMoveType, AutoStrafeLength;
        int AutoDirLength, AutoDir;

        int RndMoveType;

        int TeleportDelay;
        int TeleportCount;
        float TeleportDist;

        public BobPhsxTeleport()
        {
            DefaultValues();
        }

        public override void DefaultValues()
        {
            base.DefaultValues();

            TeleportDelay = 30;

            TeleportDist = 500;

            BobFallDelay = 15;
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            TeleportCount = 10;

            OnGround = false;
        }

        public override void DoXAccel()
        {
        }

        public override void DoYAccel()
        {
            StartedJump = false;
            base.DoYAccel();
        }

        public override void DuckingPhsx()
        {
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            if (TeleportCount >= TeleportDelay)
            {
                if (MyBob.CurInput.xVec.Length() > .5f)
                {
                    Vector2 shift = MyBob.CurInput.xVec;
 
                    if (Math.Abs(shift.X) > .5f)
                        shift.X = Math.Sign(shift.X);
                    if (Math.Abs(shift.Y) > .5f)
                        shift.Y = Math.Sign(shift.Y);

                    MyBob.Move(shift * TeleportDist);

                    FallingCount = 0;

                    MyBob.Core.Data.Velocity.Y = 0;

                    TeleportCount = 0;
                }
            }
            TeleportCount++;
        }


        public override bool CheckFor_xFlip()
        {
            return false;
        }

        public virtual void Jump()
        {
        }

        public override void LandOnSomething(bool MakeReadyToJump, ObjectBase ThingLandedOn)
        {
            base.LandOnSomething(MakeReadyToJump, ThingLandedOn);
        }

        public override void HitHeadOnSomething(ObjectBase ThingHit)
        {
            base.HitHeadOnSomething(ThingHit);
        }

        public override void GenerateInput(int CurPhsxStep)
        {
            MyBob.CurInput.B_Button = false;

            if (MyBob.Core.MyLevel.GetPhsxStep() % 60 == 0)
                RndMoveType = MyLevel.Rnd.Rnd.Next(0, 3);

            if (AutoDirLength == 0)
            {
                if (AutoDir == 1) AutoDir = -1; else AutoDir = 1;
                if (AutoDir == 1)
                    AutoDirLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.ForwardLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.ForwardLengthBase, MyBob.Core.Data.Position);
                else
                    AutoDirLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.BackLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.BackLengthBase, MyBob.Core.Data.Position);
            }

            if (AutoMoveLength == 0)
            {
                int rnd = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveWeight, MyBob.Core.Data.Position) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitWeight, MyBob.Core.Data.Position));
                if (rnd < MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveWeight, MyBob.Core.Data.Position))
                {
                    AutoMoveType = 1;
                    AutoMoveLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveLengthBase, MyBob.Core.Data.Position);
                }
                else
                {
                    AutoMoveType = 0;
                    AutoMoveLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitLengthBase, MyBob.Core.Data.Position);
                }
            }
        
            AutoMoveLength--;
            AutoStrafeLength--;          
            AutoDirLength--;
           
            if (AutoMoveType == 1)
                MyBob.CurInput.xVec.X = AutoDir;

      
            float RetardFactor = .01f * MyBob.Core.MyLevel.CurMakeData.GenData.Get(DifficultyParam.JumpingSpeedRetardFactor, MyBob.Core.Data.Position);
            if (!OnGround && MyBob.Core.Data.Velocity.X > RetardFactor * MaxSpeed)
                MyBob.CurInput.xVec.X = 0;

            MyBob.CurInput.xVec.X *= Math.Min(1, (float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 65f) + 1.35f);

            float t = 0;
            if (RndMoveType == 0)
                t = ((float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 40f) + 1) / 2;
            if (RndMoveType == 1)
                t = ((float)Math.Sin(MyBob.Core.MyLevel.GetPhsxStep() / 40f) + 1) / 2;
            if (RndMoveType == 2)
                t = Math.Abs((MyBob.Core.MyLevel.GetPhsxStep() % 120) / 120f);

            MyBob.TargetPosition.Y = MyBob.MoveData.MinTargetY - 160 + t * (200 + MyBob.MoveData.MaxTargetY - MyBob.MoveData.MinTargetY);
                    //+ 200 * (float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 20f);
            
            if (MyBob.Core.Data.Position.Y < MyBob.TargetPosition.Y)
                MyBob.CurInput.xVec.Y = 1;
            if (MyBob.Core.Data.Position.Y > MyBob.TargetPosition.Y)
                MyBob.CurInput.xVec.Y = -1;
            MyBob.CurInput.xVec.Y *= Math.Min(1, Math.Abs(MyBob.TargetPosition.Y - MyBob.Core.Data.Position.Y) / 100);


            if (MyBob.Core.Data.Position.X > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.X ||
                MyBob.Core.Data.Position.Y > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.Y)
            {
                MyBob.CurInput.xVec.X = 0;
            }

            if (CurPhsxStep < 100 && MyBob.ComputerWaitAtStart)
            {
                MyBob.CurInput.xVec = Vector2.Zero;
            }

            if (MyBob.MyPieceIndex > 0 && MyBob.MoveData.Copy >= 0)
            {
                MyBob.CurInput = MyBob.Core.MyLevel.Bobs[MyBob.MoveData.Copy].CurInput;
            }

            // Decide if the computer should want to land or not
            MyBob.WantsToLand = MyBob.Core.Data.Position.Y < MyBob.TargetPosition.Y;
        }

        public override void AnimStep()
        {
            base.AnimStep();
        }

        public override bool ReadyToPlace()
        {
            return false;
        }
    }
}