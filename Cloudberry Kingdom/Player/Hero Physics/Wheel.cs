using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class BobPhsxWheel : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(2, 0, 0, 0);
            Name = "Wheelie";
            NameTemplate = "wheelie";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Wheel"), Color.White, 1.4f * DefaultIconWidth);
        }
        static readonly BobPhsxWheel instance = new BobPhsxWheel();
        new public static BobPhsxWheel Instance { get { return instance; } }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_Wheelie();
        }

        // Instancable class
        new EzSound LandSound;

        float AngleSpeed = 0;

        public BobPhsxWheel()
        {
            LandSound = Tools.SoundWad.FindByName("BoxHero_Land");

            DefaultValues();
        }

        public override void DefaultValues()
        {
 	        base.DefaultValues();

            MaxSpeed = 21f;// 22f;
            XAccel = .2f;
            XFriction = .2f;

            SpritePadding = new Vector2(90, 0);
        }

        static int AnimIndex;
        public override void Init(Bob bob)
        {
            base.Init(bob);

            MyBob.JumpSound = Tools.SoundWad.FindByName("BoxHero_Jump");

            OnGround = false;
            StartedJump = false;
            JumpCount = 0;
            FallingCount = 10000;

            Ducking = false;

            AnimIndex = MyBob.PlayerObject.FindAnim("Wheel");
            MyBob.PlayerObject.Read(AnimIndex, 0);
        }

        public override void DuckingPhsx()
        {
            return;
        }
        
        public float MaxAngleSpeed = .0878f, AngleAcc = .00125f;
        public override void DoXAccel()
        {
            float _AngleAcc = AngleAcc;
            //if (!OnGround) AngleAcc *= 1.5f;
            float AngleFriction = .5f * _AngleAcc;

/*            float MaxAngleSpeed = .15f;
            float AngleAcc = .00175f;
            if (!OnGround) AngleAcc *= 1.5f;
            float AngleFriction = .5f * AngleAcc;
            */
            float xVec = MyBob.CurInput.xVec.X;
            int xVecSign = Math.Sign(xVec);

            if (Math.Abs(xVec) > .2f)
            {
                // Faster acc if we are trying to reverse directions
                if (Math.Sign(xVec) != Math.Sign(AngleSpeed))
                    _AngleAcc *= 1.65f;
                AngleSpeed += _AngleAcc * MyBob.CurInput.xVec.X;
            }
            else
            {
                // Friction
                AngleSpeed -= AngleFriction * Math.Sign(AngleSpeed);
                if (Math.Abs(AngleSpeed) < AngleFriction * 1.2f)
                    AngleSpeed = 0;
            }

            // Max speed
            if (AngleSpeed > MaxAngleSpeed) AngleSpeed = MaxAngleSpeed;
            if (AngleSpeed < -MaxAngleSpeed) AngleSpeed = -MaxAngleSpeed;

            //if (OnGround || this.FallingCount < 2)
            {
                //base.XAccel();

                float DesiredSpeed = AngleToDist(AngleSpeed);
                MyBob.Core.Data.Velocity.X += 1f * (DesiredSpeed - MyBob.Core.Data.Velocity.X);
            }

            //Console.WriteLine("angle speed {0}, max {1}", AngleSpeed, MaxAngleSpeed);
        }

        float AngleToDist(float Angle)
        {
            return 340 * Angle;
        }


        public override float RetardxVec()
        {
            float RetardFactor = .01f * MyBob.Core.MyLevel.CurMakeData.GenData.Get(DifficultyParam.JumpingSpeedRetardFactor, MyBob.Core.Data.Position);
            if (!OnGround && AngleSpeed > RetardFactor * MaxAngleSpeed)
                return 0;
            else
                return 1;
        }

        public override void LandOnSomething(bool MakeReadyToJump, ObjectBase ThingLandedOn)
        {
            if (MyBob.Core.MyLevel.PlayMode == 0 && ObjectLandedOn is BlockBase && !PrevOnGround)
                LandSound.Play(.47f);
            base.LandOnSomething(MakeReadyToJump, ThingLandedOn);
        }

        public override void HitHeadOnSomething(ObjectBase ThingHit)
        {
            base.HitHeadOnSomething(ThingHit);
        }

        public override void GenerateInput(int CurPhsxStep)
        {
            base.GenerateInput(CurPhsxStep);
        }

        public override void AnimStep()
        {
            MyBob.PlayerObject.ContainedQuadAngle -= AngleSpeed * 1.25f;

            MyBob.PlayerObject.Read(AnimIndex, 0);

            if (MyBob.IsSpriteBased)
            {
                //MyBob.PlayerObject.ParentQuad.PointxAxisTo(Tools.AngleToDir(MyBob.PlayerObject.ContainedQuadAngle));
            }
        }

        public override bool ReadyToPlace()
        {
            return base.ReadyToPlace();
        }

        public override bool CheckFor_xFlip()
        {
            return false;
        }

        public override void SideHit(ColType side, BlockBase block)
        {
            base.SideHit(side, block);

            if (Math.Abs(AngleSpeed) > .5f * MaxAngleSpeed)
            {
                if (side == ColType.Left) MyBob.Core.Data.Velocity.Y += .15f * (AngleToDist(AngleSpeed) - MyBob.Core.Data.Velocity.Y);
                if (side == ColType.Right) MyBob.Core.Data.Velocity.Y -= .15f * (AngleToDist(AngleSpeed) - MyBob.Core.Data.Velocity.Y);
            }

            AngleSpeed *= .75f;
        }

        public override void Die(Bob.BobDeathType DeathType)
        {
            if (Bob.AllExplode)
            {
                Explode();
            }

            SetDeathVel(DeathType);
        }

        public override void ToSprites(Dictionary<int, SpriteAnim> SpriteAnims, Vector2 Padding)
        {
            ObjectClass Obj = MyBob.PlayerObject;
            SpriteAnims.Add(0, Obj.AnimToSpriteFrames(AnimIndex, 1, false, Padding));
        }
    }
}