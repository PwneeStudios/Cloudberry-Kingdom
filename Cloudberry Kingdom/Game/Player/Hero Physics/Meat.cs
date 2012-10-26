using Microsoft.Xna.Framework;
using CoreEngine;
using System;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;

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
            phsx.ModInitSize = 1.25f * new Vector2(.27f, .27f) * modsize;
            phsx.CapePrototype = Cape.CapeType.Small;

            BobPhsxNormal normal = phsx as BobPhsxNormal;
            if (null != normal)
            {
                normal.BobJumpLength = (int)(normal.BobJumpLength * 1.5f);
                normal.BobJumpAccel *= .5f;

                //normal.Gravity *= .85f;
                normal.Gravity *= .7f;
                normal.SetAccels();

                normal.ForcedJumpDamping = .9f;
            }

            BobJumpLength = 27;
            BobJumpAccel = .18f;
            Gravity = 2;
            MaxSpeed = 38.5f;
            XAccel = .5f;
            XFriction = .4f;

            MaxSpeed = 38;
            XAccel = .7f;
            XFriction = .7f;
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(5, 0, 0, 0);
            Name = "Meat";
            Adjective = "Meat";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Meat"), Color.White, 1.2f * DefaultIconWidth);
        }
        static readonly BobPhsxMeat instance = new BobPhsxMeat();
        public static new BobPhsxMeat Instance { get { return instance; } }

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
        public int StepsSinceSide, StepsOnSide;
        public int StickyDuration = 60;
        ColType StickySide;
        BlockBase LastStickyBlock;
        bool IsStuck = false;
        public override void SideHit(ColType side, BlockBase block)
        {
            base.SideHit(side, block);

            if (block == null) return;

            LastStickyBlock = block;
            IsStuck = true;

            if (side == ColType.Right) StickySide = ColType.Left;
            if (side == ColType.Left) StickySide = ColType.Right;

            //float Factor;
            //if (yVel < 0)
            //    Factor = CoreMath.LerpRestrict(.3f, 1f, (float)StepsOnSide / StickyDuration);
            //else
            //    Factor = .765f;
            //float BlockSpeed = block.Box.Target.TR.Y - block.Box.Current.TR.Y;
            //yVel = Factor * (yVel - BlockSpeed) + BlockSpeed;


            StepsSinceSide = 0;

            //OnGround = true;
            //CurJump = 1;
            //JetPackCount = 0;
            //if (StepsOnSide < StickyDuration)
            //    base.LandOnSomething(false);
        }

        public override void LandOnSomething(bool MakeReadyToJump, ObjectBase ThingLandedOn)
        {
            base.LandOnSomething(MakeReadyToJump, ThingLandedOn);

            StickySide = ColType.NoCol;

            LastJumpWasSticky = false;
            StepsOnSide = 0;
            StepsSinceSide = 0;
        }

        bool CanWallJump;
        public int WallJumpCount;
        int StickyGracePeriod = 8;
        public float Max_yVel_ForWallJump = 20;

        int SideJumpLength = 10;
        float SideJumpStr = 5;

        public override void Jump()
        {
            base.Jump();

            if (ExternalPreventJump) return;

            if (!MyBob.CurInput.A_Button) CanWallJump = true;

            if (yVel < Max_yVel_ForWallJump && StickySide != ColType.NoCol && CanWallJump && (StepsSinceSide < StickyGracePeriod && yVel <= 0 || StepsSinceSide < 2) && !CanJump && MyBob.CurInput.A_Button)
            {
                //StickySide = ColType.NoCol;
                IsStuck = false;

                StepsSinceSide = StickyGracePeriod;

                xVel += -19.5f * StickyDir;
                    //(StickySide == ColType.Right ? -1 : 1);

                DoJump();

                LastJumpWasSticky = true;

                yVel -= 2;
                JumpCount -= 1;
                WallJumpCount = SideJumpLength;
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
            // Additional wall jumping phsx
            if (WallJumpCount > 0)
            {
                if (!MyBob.CurInput.A_Button ||
                    Math.Abs(MyBob.CurInput.xVec.X) > .3f && Math.Sign(MyBob.CurInput.xVec.X) == StickyDir)
                    WallJumpCount = 0;
                else
                {
                    xVel -= SideJumpStr * StickyDir * WallJumpCount / (float)SideJumpLength;
                    WallJumpCount--;
                }
            }

            StepsSinceSide++;

            // Additional sticky phsx
            //if (IsStuck)
            {
                if (StepsSinceSide < 2)
                    StepsOnSide++;
                else
                    StepsOnSide = 0;
            }
            if (IsStuck && LastStickyBlock != null)
            {
                if (LastStickyBlock.Box.TR.Y > MyBob.Box.BL.Y &&
                    LastStickyBlock.Box.BL.Y < MyBob.Box.TR.Y)
                {
                    if (StickySide == ColType.Right)
                    {
                        float speed = LastStickyBlock.Box.LeftSpeed() + 1;
                        if (xVel < speed) xVel = speed;
                        SideHit(ColType.Left, LastStickyBlock);
                    }

                    if (StickySide == ColType.Left)
                    {
                        float speed = LastStickyBlock.Box.RightSpeed() - 1;
                        if (xVel > speed) xVel = speed;
                        SideHit(ColType.Right, LastStickyBlock);
                    }
                }
                else
                {
                    IsStuck = false;
                    StepsSinceSide += 3;
                }

                if (StickySide == ColType.Right && MyBob.CurInput.xVec.X < -.3f)
                    IsStuck = false;
                if (StickySide == ColType.Left && MyBob.CurInput.xVec.X > .3f)
                    IsStuck = false;
            }

            base.PhsxStep();
        }

        public override void DoXAccel()
        {
            base.DoXAccel();
        }

        public override float GetXAccel()
        {
            //// For a few frames after jumping off a wall, force the player to move in the opposite direction
            //if (LastJumpWasSticky && StepsSinceSide > StickyGracePeriod)
            //{
            //    if (StepsSinceSide < StickyGracePeriod + 8)
            //    {
            //        MyBob.CurInput.xVec.X = -StickyDir;
            //        return base.GetXAccel() * .6f;
            //    }
            //    else if (StepsSinceSide < StickyGracePeriod + 20)
            //        return base.GetXAccel() * 1.35f;
            //}

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

        Vector2 PrefferedDir;
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

            PrefferedDir.X = Math.Sign(Target.X - Pos.X);
            //if (MyLevel.Rnd.RndBool())
            //    Target = new Vector2(Cam.TR.X, 100000);
            //else
            //    Target = new Vector2(Cam.BL.X, 100000);
        }

        public bool WantToLandOnTop = false;
        Vector2 Target;
        Vector2 AlwaysForward;
        int StraightUpDuration = 0;
        float yVelCutoff = 0;
        public override void GenerateInput(int CurPhsxStep)
        {
            WantToLandOnTop = false;

            if (Target.X < Pos.X - 10000)
            {
                yVelCutoff = 20;
                NewTarget();
            }

            if (Math.Abs(xVel) < 5 && yVel > 5 && OnGround)
                if (Math.Abs(Target.X - Pos.X) < 200)
                {
                    NewTarget();
                    if (Pos.X > Cam.Pos.X && Target.X > Cam.Pos.X) NewTarget();
                    if (Pos.X < Cam.Pos.X && Target.X < Cam.Pos.X) NewTarget();
                }

            MyBob.WantsToLand = Pos.Y < Target.Y;
            MyBob.CurInput.A_Button = Pos.Y < Target.Y;

            int StickyWaitLength = 7;// 9;

            // Move right/left if target is to our right/left.
            if (Pos.Y > MyLevel.Fill_TR.Y + 65)
                MyBob.CurInput.xVec.X = Math.Sign(Target.X - Pos.X);
            else
                MyBob.CurInput.xVec.X = PrefferedDir.X;


            // Move right/left if we are sticking to a wall to our right/left.
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
                        // Make these higher to make the AI use blocks more often (and attempt less epically long jumps)
                        yVelCutoff = MyLevel.Rnd.RndFloat(-2, 12);

                        //yVelCutoff = MyLevel.Rnd.RndFloat(-15, 12);
                        //yVelCutoff = MyLevel.Rnd.RndFloat(-25, 12);
                    //}
                    //{
                        NewTarget();
                        for (int i = 0; i < 2; i++)
                        {
                            if (StickyDir > 0 && Target.X > Pos.X) NewTarget();
                            if (StickyDir < 0 && Target.X < Pos.X && MyLevel.Rnd.RndBool(.5f)) NewTarget();
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

            // Better jump control: don't use full extent of jump
            if (StepsSinceSide >= 5)
            {
                float RetardFactor = .01f * MyBob.Core.MyLevel.CurMakeData.GenData.Get(DifficultyParam.JumpingSpeedRetardFactor, Pos);
                MyBob.CurInput.xVec.X *= RetardFactor;

                int RetardJumpLength = GenData.Get(DifficultyParam.RetardJumpLength, Pos);
                if (!OnGround && RetardJumpLength >= 1 && JumpCount < RetardJumpLength && JumpCount > 1)
                    MyBob.CurInput.A_Button = false;
            }
        }

        public override bool IsTopCollision(ColType Col, AABox box, BlockBase block)
        {
            return Col != ColType.NoCol && Col == ColType.Top;
        }

        public override bool IsBottomCollision(ColType Col, AABox box, BlockBase block)
        {
            return Col == ColType.Bottom;
        }

        public override void ModData(ref Level.MakeData makeData, StyleData Style)
        {
            base.ModData(ref makeData, Style);

            float size = 90; bool ModSize = false;

            Style.BlockFillType = StyleData._BlockFillType.Sideways;
            makeData.BlocksAsIs = true;

            // Don't keep anything extra
            Style.ChanceToKeepUnused = 0;

            // Square mblocks, vertical motion
            var MParams = (MovingBlock_Parameters)Style.FindParams(MovingBlock_AutoGen.Instance);
            MParams.Aspect = MovingBlock_Parameters.AspectType.Square;
            //MParams.Motion = MovingBlock_Parameters.MotionType.Vertical;
            //MParams.Motion = MovingBlock_Parameters.MotionType.Horizontal;
            //MParams.Size = size;

            var GhParams = (GhostBlock_Parameters)Style.FindParams(GhostBlock_AutoGen.Instance);
            GhParams.BoxType = GhostBlock_Parameters.BoxTypes.Long;
            var FParams = (FallingBlock_Parameters)Style.FindParams(FallingBlock_AutoGen.Instance);
            var BParams = (BouncyBlock_Parameters)Style.FindParams(BouncyBlock_AutoGen.Instance);
            //var GParams = (Goomba_Parameters)Style.FindParams(Goomba_AutoGen.Instance);
            var NParams = (NormalBlock_Parameters)Style.FindParams(NormalBlock_AutoGen.Instance);
            //NParams.CustomWeight = true;
            //NParams.FillWeight.Val = 1;

            Style.ModNormalBlockWeight = 1f;

            if (ModSize)
            {
                BParams.Size = size;
                GhParams.Width = size;
                FParams.Width = size;
            }
        }
    }
}