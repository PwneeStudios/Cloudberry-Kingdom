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
            Name = Localization.Words.Porkchop;
            Adjective = "Meat";
			Icon = new PictureIcon(Tools.TextureWad.FindByName("Fblock_Cave_3"), Color.White, 1.2f * DefaultIconWidth);
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

			if (block is BouncyBlock)
			{
				IsStuck = false;
			}

            StepsSinceSide = 0;
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
                IsStuck = false;

                StepsSinceSide = StickyGracePeriod;

                xVel += -19.5f * StickyDir;

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
            MyBob.UseCustomCapePos = true;

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
            if (StepsSinceSide < 2)
                StepsOnSide++;
            else
                StepsOnSide = 0;

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
        }

        public bool WantToLandOnTop = false;
        Vector2 Target;
        Vector2 AlwaysForward;
        int StraightUpDuration = 0;
        float yVelCutoff = 0;
        public override void GenerateInput(int CurPhsxStep)
        {
            WantToLandOnTop = false;

			if (Geometry == LevelGeometry.Right)
			{
				if (Pos.X > Target.X - 200)
					NewTarget();
			}
			else
			{
				if (Target.X < Pos.X - 10000)
				{
					yVelCutoff = 20;
					NewTarget();
				}

				if (Pos.X >  1400) PrefferedDir.X = -1;
				if (Pos.X < -1400) PrefferedDir.X =  1;

				if (Math.Abs(xVel) < 5 && yVel > 5 && OnGround)
					if (Math.Abs(Target.X - Pos.X) < 200)
					{
						NewTarget();
						if (Pos.X > Cam.Pos.X && Target.X > Cam.Pos.X) NewTarget();
						if (Pos.X < Cam.Pos.X && Target.X < Cam.Pos.X) NewTarget();
					}
			}

            MyBob.WantsToLand = Pos.Y < Target.Y;
            MyBob.CurInput.A_Button = Pos.Y < Target.Y;

            int StickyWaitLength = 7;// 9;

            // Move right/left if target is to our right/left.
			if (Geometry == LevelGeometry.Right)
			{
				MyBob.CurInput.xVec.X = PrefferedDir.X;
			}
			else
			{
				if (Pos.Y > MyLevel.Fill_TR.Y + 65)
					MyBob.CurInput.xVec.X = Math.Sign(Target.X - Pos.X);
				else
					MyBob.CurInput.xVec.X = PrefferedDir.X;
			}

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
			if (Geometry == LevelGeometry.Right)
			{
				if (StepsSinceSide >= 4 && Pos.Y < Target.Y + 250)
					MyBob.CurInput.A_Button = true;
			}
			else
			{
				if (StepsSinceSide >= 4)
					MyBob.CurInput.A_Button = true;
			}

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
            if (yVel > yVelCutoff) MyBob.WantsToLand = false;
            else MyBob.WantsToLand = true;

            // Don't use too many blocks in a row
            if (CurPhsxStep < LastUsedStamp + 12) MyBob.WantsToLand = false;

            if (Pos.X > Cam.TR.X - 550 ||
                Pos.X < Cam.BL.X + 550)
            {
                if (yVel < 5)
                    WantToLandOnTop = true;
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

			if (Style is SingleData)
			{
			}
			else
			{
				Style.BlockFillType = StyleData._BlockFillType.Sideways;
				makeData.BlocksAsIs = true;
			}

            // Don't keep anything extra
            Style.ChanceToKeepUnused = 0;

            // Square mblocks, vertical motion
            var MParams = (MovingBlock_Parameters)Style.FindParams(MovingBlock_AutoGen.Instance);
            MParams.Aspect = MovingBlock_Parameters.AspectType.Square;

            var GhParams = (GhostBlock_Parameters)Style.FindParams(GhostBlock_AutoGen.Instance);
            GhParams.BoxType = GhostBlock_Parameters.BoxTypes.Long;
            var FParams = (FallingBlock_Parameters)Style.FindParams(FallingBlock_AutoGen.Instance);
            var BParams = (BouncyBlock_Parameters)Style.FindParams(BouncyBlock_AutoGen.Instance);
            var NParams = (NormalBlock_Parameters)Style.FindParams(NormalBlock_AutoGen.Instance);

            if (ModSize)
            {
                BParams.Size = size;
                GhParams.Width = size;
                FParams.Width = size;
            }
        }

        public override void ModLadderPiece(PieceSeedData piece)
        {
            base.ModLadderPiece(piece);

            piece.ElevatorBoxStyle = BlockEmitter_Parameters.BoxStyle.Meatboy;
        }

		static CoreTexture standing, jumping, falling;
		protected override void DrawObject()
		{
            base.DrawObject();

            var quad = Obj.FindQuad("Rocket");
            quad.Show = false;

            Vector2 shift = TranscendentOffset;

			if (standing == null)
			{
				standing = Tools.Texture("Fblock_Cave_1");
				jumping  = Tools.Texture("Fblock_Cave_2");
				falling  = Tools.Texture("Fblock_Cave_3");
			}

			MyBob.UseCustomCapePos = true;
			if (Obj != null)
			{
				CapeOffset_Ducking = new Vector2(-10, -4);
				MyBob.CustomCapePos = new Vector2(Obj.xFlip ? 20 : -20, 25);
			}

			var texture = standing;
			if (yVel > 2) texture = jumping;
			else if (yVel < -5) texture = falling;

			var clr = ColorHelper.HsvTransform(1, 1, 170) * MyBob.MyColorScheme.SkinColor.M;
			Tools.QDrawer.SetColorMatrix(clr, -232425 /* intentionally invalid signature */);

            Tools.QDrawer.DrawQuad(.5f * (MyBob.Box.Current.BL + MyBob.Box.BL) + new Vector2(-5, -2) + shift,
                                   .5f * (MyBob.Box.Current.TR + MyBob.Box.TR) + new Vector2(5, 3) + shift,
								   Color.White, texture, Tools.HslEffect);

            if (Obj != null)
                Obj.Draw(true);
		}
    }
}