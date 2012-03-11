using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class BobPhsxSpaceship: BobPhsx
    {
        public static float KeepUnused(float UpgradeLevel)
        {
            return .5f + .03f * UpgradeLevel;
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Spaceship";
            NameTemplate = "spaceship";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("Spaceship_1"), Color.White, 1.08f * DefaultIconWidth);
        }
        static readonly BobPhsxSpaceship instance = new BobPhsxSpaceship();
        public static BobPhsxSpaceship Instance { get { return instance; } }

        // Instancable class
        int AutoMoveLength, AutoMoveType, AutoStrafeLength;
        int AutoDirLength, AutoDir;

        int RndMoveType;

        public BobPhsxSpaceship()
        {
            DefaultValues();
        }

        public override void DefaultValues()
        {
            MaxSpeed = 24f;
            XAccel = 2.3f;

            base.DefaultValues();
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            bob.DieSound = Tools.SoundWad.FindByName("Spaceship_Explode");

            OnGround = false;
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            MyBob.Core.Data.Velocity *= .86f;

            if (MyBob.CurInput.xVec.Length() > .2f)
            {
                MyBob.Core.Data.Velocity += XAccel * MyBob.CurInput.xVec;

                float Magnitude = MyBob.Core.Data.Velocity.Length();
                if (Magnitude > MaxSpeed)
                {
                    MyBob.Core.Data.Velocity.Normalize();
                    MyBob.Core.Data.Velocity *= MaxSpeed;
                }
            }
         
            OnGround = false;
        }

        public override void PhsxStep2()
        {
            base.PhsxStep2();
        }

        public override bool CheckFor_xFlip()
        {
            return false;
        }

        public virtual void Jump()
        {
        }

        public override void LandOnSomething(bool MakeReadyToJump)
        {
            base.LandOnSomething(MakeReadyToJump);

            MyBob.Core.Data.Velocity.Y = MyBob.Core.Data.Velocity.Y + 5;

            MyBob.BottomCol = true;

            OnGround = true;
        }

        public override void HitHeadOnSomething()
        {
            base.HitHeadOnSomething();
        }

        int TurnCountdown = 0, Dir = 0;
        public void GenerateInput_Vertical(int CurPhsxStep)
        {
            //MyBob.CurInput.A_Button = false;
            //if (TurnCountdown <= 0)
            //{
            //    if (Dir == 0) Dir = 1;

            //    Dir *= -1;
            //    TurnCountdown = MyLevel.Rnd.RndInt(0, 135);
            //}
            //else
            //    TurnCountdown--;

            //Camera cam = MyBob.Core.MyLevel.MainCamera;
            //float HardBound = 1000; float SoftBound = 1500;
            //if (Pos.X > cam.TR.X - HardBound) Dir = -1;
            //if (Pos.X < cam.BL.X + HardBound) Dir = 1;
            //if (Pos.X > cam.TR.X - SoftBound && Dir == 1) TurnCountdown -= 2;
            //if (Pos.X < cam.BL.X + SoftBound && Dir == -1) TurnCountdown -= 2;

            //MyBob.CurInput.xVec.X = Dir;

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
                MyBob.CurInput.xVec.Y = AutoDir;


            float RetardFactor = .01f * MyBob.Core.MyLevel.CurMakeData.GenData.Get(DifficultyParam.JumpingSpeedRetardFactor, MyBob.Core.Data.Position);
            if (MyBob.Core.Data.Velocity.Y > RetardFactor * MaxSpeed)
                MyBob.CurInput.xVec.Y = 0;

            MyBob.CurInput.xVec.Y *= Math.Min(1, (float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 65f) + 1.35f);

            float t = 0;
            if (RndMoveType == 0)
                t = ((float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 40f) + 1) / 2;
            if (RndMoveType == 1)
                t = ((float)Math.Sin(MyBob.Core.MyLevel.GetPhsxStep() / 40f) + 1) / 2;
            if (RndMoveType == 2)
                t = Math.Abs((MyBob.Core.MyLevel.GetPhsxStep() % 120) / 120f);

            MyBob.TargetPosition.X = MyBob.MoveData.MinTargetY - 160 + t * (200 + MyBob.MoveData.MaxTargetY - MyBob.MoveData.MinTargetY);
            //+ 200 * (float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 20f);

            if (MyBob.Core.Data.Position.X < MyBob.TargetPosition.X)
                MyBob.CurInput.xVec.X = 1;
            if (MyBob.Core.Data.Position.X > MyBob.TargetPosition.X)
                MyBob.CurInput.xVec.X = -1;
            MyBob.CurInput.xVec.X *= Math.Min(1, Math.Abs(MyBob.TargetPosition.X - MyBob.Core.Data.Position.X) / 100);


            if (MyBob.Core.Data.Position.Y > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.Y ||
                MyBob.Core.Data.Position.X > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.X)
            {
                MyBob.CurInput.xVec.Y = 0;
            }
        }

        public override void GenerateInput(int CurPhsxStep)
        {
            base.GenerateInput(CurPhsxStep);

            switch (Geometry)
            {
                case LevelGeometry.Right:
                    GenerateInput_Right(CurPhsxStep);
                    break;

                case LevelGeometry.Down:
                case LevelGeometry.Up:
                    GenerateInput_Vertical(CurPhsxStep);
                    break;

                default:
                    break;
            }

            AdditionalGenerateInputChecks(CurPhsxStep);
        }

        void GenerateInput_Right(int CurPhsxStep)
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

            MyBob.TargetPosition.Y = MyBob.MoveData.MinTargetY - 200 + t * (-90 + MyBob.MoveData.MaxTargetY - MyBob.MoveData.MinTargetY);
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
        }

        public override void AnimStep()
        {
            base.AnimStep();
        }

        public override bool ReadyToPlace()
        {
            return false;
        }

        public override void ToSprites(Dictionary<int, SpriteAnim> SpriteAnims, Vector2 Padding)
        {
            ObjectClass Obj = MyBob.PlayerObject;
            SpriteAnims.Add(0, Obj.AnimToSpriteFrames(0, 1, true, Padding));
        }

        public override void Die(Bob.BobDeathType DeathType)
        {
            base.Die(DeathType);

            MyBob.Core.Data.Velocity = new Vector2(0, 25);
            MyBob.Core.Data.Acceleration = new Vector2(0, -1.9f);

            Fireball.Explosion(MyBob.Core.Data.Position, MyBob.Core.MyLevel);
        }

        public override void BlockInteractions()
        {
            if (Core.MyLevel.PlayMode != 0) return;

            foreach (Block block in Core.MyLevel.Blocks)
            {
                if (!block.Core.MarkedForDeletion && block.IsActive && block.Core.Active && Phsx.BoxBoxOverlap(MyBob.Box2, block.Box))
                {
                    if (!MyBob.Immortal)
                        MyBob.Die(Bob.BobDeathType.Other);
                    else
                        block.Hit(MyBob);
                }
            }            
        }
    }
}