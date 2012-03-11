using Microsoft.Xna.Framework;
using Drawing;
using System;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class BobPhsxMeat : BobPhsxNormal
    {
        public override void Release()
        {
            base.Release();

            LastStickyBlock = null;
        }

        public override void Set(BobPhsx phsx)
        {
            Set(phsx, Vector2.One);
        }
        public void Set(BobPhsx phsx, Vector2 modsize)
        {
            phsx.ModInitSize = new Vector2(.4f, .285f) * modsize;
            phsx.CapePrototype = Cape.CapeType.Small;

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.BobJumpLength = (int)(normal.BobJumpLength * 1.5f);
                normal.BobJumpAccel *= .5f;

                normal.Gravity *= .85f;
                normal.SetAccels();

                normal.ForcedJumpDamping = .9f;
            }
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Name = "Meat";
            Adjective = "Meat";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroImage_Meat"), Color.White, 1.2f * DefaultIconWidth);
        }
        static readonly BobPhsxMeat instance = new BobPhsxMeat();
        public static BobPhsxMeat Instance { get { return instance; } }

        public override InteractWithBlocks MakePowerup()
        {
            return new Powerup_Jetpack();
        }

        // Instancable class
        public BobPhsxMeat()
        {
            Set(this);
        }

        public override void Init(Bobs.Bob bob)
        {
            base.Init(bob);

            WallJumpCount = StepsSinceSide = StepsOnSide = 0;
            CanWallJump = false;

            Target = new Vector2(float.MinValue, float.MinValue);
        }

        bool LastJumpWasSticky = false;
        int StepsSinceSide, StepsOnSide;
        public int StickyDuration = 60;
        ColType StickySide;
        Block LastStickyBlock;
        public override void SideHit(ColType side, Block block)
        {
            base.SideHit(side, block);

            if (block == null) return;

            LastStickyBlock = block;

            if (side == ColType.Right) StickySide = ColType.Left;
            if (side == ColType.Left) StickySide = ColType.Right;

            //if (yVel < 0) yVel = 0;
            if (yVel < 0)
                yVel *= Tools.LerpRestrict(.3f, 1f, (float)StepsOnSide / StickyDuration);
            else
                yVel *= .765f;

            if (StepsSinceSide < 2)
                StepsOnSide++;
            else
                StepsOnSide = 0;

            StepsSinceSide = 0;

            //OnGround = true;
            //CurJump = 1;
            //JetPackCount = 0;
            //if (StepsOnSide < StickyDuration)
            //    base.LandOnSomething(false);
        }

        public override void LandOnSomething(bool MakeReadyToJump)
        {
            base.LandOnSomething(MakeReadyToJump);

            LastJumpWasSticky = false;
            StepsOnSide = 0;
            StepsSinceSide = 0;
        }

        bool CanWallJump;
        int WallJumpCount;
        int StickyGracePeriod = 5;
        public override void Jump()
        {
            base.Jump();

            if (!MyBob.CurInput.A_Button) CanWallJump = true;

            if (CanWallJump && (StepsSinceSide < StickyGracePeriod && yVel <= 0 || StepsSinceSide < 2) && StepsOnSide < StickyDuration && !CanJump && MyBob.CurInput.A_Button)
            {
                StepsSinceSide = StickyGracePeriod;

                xVel += -25 * StickyDir;
                    //(StickySide == ColType.Right ? -1 : 1);

                DoJump();

                LastJumpWasSticky = true;

                yVel -= 2;
                JumpCount -= 1;
                WallJumpCount = 0;
            }
        }

        float SideToDir(ColType side)
        {
            switch (side) {
                case ColType.Right: return 1;
                case ColType.Left: return -1;
                default: return 0;
            }
        }

        float StickyDir { get { return SideToDir(StickySide); } }

        public override void PhsxStep()
        {
            BobJumpLength = 27;
            BobJumpAccel = .18f;
            Gravity = 2;
            MaxSpeed = 38.5f;
            XAccel = .5f;
            XFriction = .4f;

            MaxSpeed = 38;
            XAccel = .7f;
            XFriction = .7f;


            //if (WallJumpCount < 5 && MyBob.CurInput.xVec.X < -.5f)
            //{
            //    WallJumpCount++;
            //    xVel -= Tools.LerpRestrict(3f, 0f, (float)WallJumpCount/ 5);
            //}

            StepsSinceSide++;

            base.PhsxStep();
        }

        public override void DoXAccel()
        {
            base.DoXAccel();
        }

        public override float GetXAccel()
        {
            // For a few frames after jumping off a wall, force the player to move in the opposite direction
            if (LastJumpWasSticky && StepsSinceSide > StickyGracePeriod)
            {
                if (StepsSinceSide < StickyGracePeriod + 8)
                {
                    MyBob.CurInput.xVec.X = -StickyDir;
                    return base.GetXAccel() * .6f;
                }
                else if (StepsSinceSide < StickyGracePeriod + 20)
                    return base.GetXAccel() * 1.35f;
            }

            return base.GetXAccel();
        }

        protected override void DoJump()
        {
            base.DoJump();

            CanWallJump = false;
            LastJumpWasSticky = false;
        }

        public override bool ShouldStartJumpAnim()
        {
            return StartJumpAnim;
        }

        protected override void SetTarget(Levels.RichLevelGenData GenData)
        {
            base.SetTarget(GenData);

            //if (MyBob.TargetPosition.Y > Cam.Pos.Y)
            //    MyBob.TargetPosition.Y -= 900;
        }

        void NewTarget()
        {
            if (MyLevel.Geometry == LevelGeometry.Right)
            {
                AlwaysForward = Vector2.Max(AlwaysForward, Pos) + new Vector2(300);
                Target = new Vector2(.5f * (Pos.X + MyLevel.Rnd.RndFloat(-400, 3000) + AlwaysForward.X),
                    MyLevel.Rnd.RndFloat(Cam.BL.Y + 400, Cam.TR.Y - 300));
            }

            if (MyLevel.Geometry == LevelGeometry.Up)
            {
                AlwaysForward = Vector2.Max(AlwaysForward, Pos) + new Vector2(300);
                Target = new Vector2(
                    MyLevel.Rnd.RndFloat(Cam.BL.X + 600, Cam.TR.X - 600),
                    .5f * (Pos.Y + MyLevel.Rnd.RndFloat(-400, 3000) + AlwaysForward.Y));
            }
        }

        public bool WantToLandOnTop = false;
        Vector2 Target;
        Vector2 AlwaysForward;
        int StraightUpDuration = 0;
        float yVelCutoff = 0;
        public override void GenerateInput(int CurPhsxStep)
        {
            WantToLandOnTop = false;

            //if (OnGround || StepsOnSide > 2)
            //{
            //    if (Target.X < Pos.X - 2000) NewTarget();
            //    //if ((Target - Pos).Length() < 300) NewTarget();
            //    if (Math.Abs(Target.X - Pos.X) < 300) NewTarget();
            //    //if (CurPhsxStep % 60 == 0) NewTarget();
            //}
            if (Math.Abs(xVel) < 15)
                if (Math.Abs(Target.X - Pos.X) < 200) NewTarget();

            MyBob.WantsToLand = Pos.Y < Target.Y;
            MyBob.CurInput.A_Button = Pos.Y < Target.Y;

            int StickyWaitLength = 9;

            // Move right/left if target is to our right/left.
            MyBob.CurInput.xVec.X = Math.Sign(Target.X - Pos.X);


            // Move right/lefto if we are sticking to a wall to our right/left.
            if (StepsSinceSide < 5 && (StickySide == ColType.Right || StickySide == ColType.Left))
            {
                if (StepsOnSide < StickyWaitLength && (LastStickyBlock == null || LastStickyBlock.Box.BL.Y < MyBob.Box.TR.Y - 15))
                {
                    MyBob.CurInput.xVec.X = StickyDir;
                    MyBob.CurInput.A_Button = false;
                }
                else
                {
                    MyBob.CurInput.A_Button = true;

                    if (StepsOnSide == StickyWaitLength)
                    {
                        yVelCutoff = MyLevel.Rnd.RndFloat(-15, 12);
                    }

                    {
                        NewTarget();
                        for (int i = 0; i < 2; i++)
                        {
                            if (StickyDir > 0 && Target.X > Pos.X) NewTarget();
                            if (StickyDir < 0 && Target.X < Pos.X) NewTarget();
                        }
                    }
                }
            }


            // Full force wall jump
            if (StepsSinceSide >= 4)
                MyBob.CurInput.A_Button = true;

            // Regular jump
            if (OnGround)
                StraightUpDuration = 18;
            if (StraightUpDuration > 0)
            {
                MyBob.CurInput.xVec.X = -StickyDir;
                MyBob.CurInput.A_Button = true;
                StraightUpDuration--;

                //if (Pos.X > Cam.TR.X - 900 ||
                //    Pos.X < Cam.BL.X + 900)
                MyBob.CurInput.xVec.X = Math.Sign(Target.X - Pos.X);
            }

            // Don't wall jump if we are going up fast
            //if (yVel > 14) MyBob.WantsToLand = false;
            //if (yVel > -5) MyBob.WantsToLand = false;
            //if (yVel > -15) MyBob.WantsToLand = false;
            if (yVel > yVelCutoff) MyBob.WantsToLand = false;
            else MyBob.WantsToLand = true;

            // Don't use too many blocks in a row
            if (CurPhsxStep < LastUsedStamp + 12) MyBob.WantsToLand = false;

            if (Pos.X > Cam.TR.X - 550 ||
                Pos.X < Cam.BL.X + 550)
            {
                if (yVel < 5)
                    WantToLandOnTop = true;
                //MyBob.WantsToLand = false;
            }
        }

        public override bool IsTopCollision(ColType Col, AABox box, Block block)
        {
            return Col != ColType.NoCol && Col == ColType.Top;
        }

        public override bool IsBottomCollision(ColType Col, AABox box, Block block)
        {
            return Col == ColType.Bottom;
        }
    }
}