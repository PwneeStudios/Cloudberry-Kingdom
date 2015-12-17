using Microsoft.Xna.Framework;
using System;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BobPhsxFourWay : BobPhsxNormal
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
            Gravity = 0;
            MaxSpeed = 38.5f;
            XAccel = 0;
            XFriction = 0;

            MaxSpeed = 38;
            XAccel = 0;
            XFriction = 0;
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(5, 0, 0, 0);
            Name = Localization.Words.Serpent;
            Adjective = "FourWay";
			Icon = new PictureIcon(Tools.TextureWad.FindByName("SerpentHead_Castle_1"), Color.White, 1.2f * DefaultIconWidth);
        }
        static readonly BobPhsxFourWay instance = new BobPhsxFourWay();
        public static new BobPhsxFourWay Instance { get { return instance; } }

        // Instancable class
        public BobPhsxFourWay()
        {
            Set(this);
        }

		static int AnimIndex;
        public override void Init(Bobs.Bob bob)
        {
            base.Init(bob);

			ReadyToDash = true;

            WallJumpCount = StepsSinceSide = StepsOnSide = 0;

            Target = new Vector2(float.MinValue, float.MinValue);

			AnimIndex = MyBob.PlayerObject.FindAnim("Wheel");
			MyBob.PlayerObject.Read(AnimIndex, 0);
        }
		
		public override void AnimStep()
		{
			MyBob.PlayerObject.ContainedQuadAngle -= 1;

			MyBob.PlayerObject.Read(AnimIndex, 0);
		}

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
			else
			{
				ReadyToDash = true;
			}

            StepsSinceSide = 0;
        }

        public override void LandOnSomething(bool MakeReadyToJump, ObjectBase ThingLandedOn)
        {
            base.LandOnSomething(MakeReadyToJump, ThingLandedOn);

            StickySide = ColType.NoCol;

            StepsOnSide = 0;
            StepsSinceSide = 0;

			if (ThingLandedOn is BouncyBlock)
			{
			}
			else
			{
				//ReadyToDash = true;
			}
        }

		public override void HitHeadOnSomething(ObjectBase ThingHit)
		{
			base.HitHeadOnSomething(ThingHit);

			if (ThingHit is BouncyBlock)
			{
			}
			else
			{
				ReadyToDash = true;
			}
		}

        public int WallJumpCount;
        public float Max_yVel_ForWallJump = 20;

		bool ReadyToDash = true;
        public override void Jump()
        {
			JumpDelayCount--;

			//UpdateReadyToJump();

			if (ExternalPreventJump)
			{
				DisableJumpCount--;
				return;
			}

			if ((ReadyToDash || OnGround) && MyBob.CurInput.xVec.Length() > .5f)
			{
				DoJump();
			}
        }

		public override void DoYAccel()
		{
			if (StartedJump && JumpCount > 0)
			{
				if (CurJump == 1)
					Vel += BobJumpAccel * (float)(JumpCount) * JumpDir;
				else
					Vel += BobJumpAccel2 * (float)(JumpCount) * JumpDir;
				JumpCount -= 1;
			}
			else
				StartedJump = false;
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

		Vector2 JumpDir;
		protected override void DoJump()
		{
			//if (OnGround || FallingCount < BobFallDelay + 5)
				//CurJump = 0;

			Gravity = -.0001f * JumpDir.Y;

			AirTime = 0;
			Jumped = true;

			StartJumpAnim = true;

			CurJump++;
			JumpDelayCount = JumpDelay;

			ReadyToDash = false;

			FallingCount = 1000;

			float speed;
			if (CurJump == 1)
			{
				speed = BobInitialJumpSpeed;
				JumpCount = BobJumpLength;
			}
			else
			{
				speed = BobInitialJumpSpeed2;
				JumpCount = BobJumpLength2;
			}

			NoStickPeriod = 3;
			JumpCount = (int)(JumpCount * JumpLengthModifier);

			if (MyBob.Core.MyLevel.PlayMode == 0 && !MyBob.CharacterSelect)
			{
				PlayJumpSound();
			}

			if (Math.Abs(MyBob.CurInput.xVec.X) > Math.Abs(MyBob.CurInput.xVec.Y))
			{
				JumpDir = new Vector2(Math.Sign(MyBob.CurInput.xVec.X), 0);
			}
			else
			{
				JumpDir = new Vector2(0, Math.Sign(MyBob.CurInput.xVec.Y));
			}

			speed = JumpAccelModifier * speed;
			Vel = JumpDir * speed;

			StartedJump = true;

			JumpStartPos = Pos;
			ApexReached = false;
			ApexY = -20000000;

			IncrementJumpCounter();
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
            //MyBob.CurInput.A_Button = Pos.Y < Target.Y;
			if (Pos.Y < Target.Y)
			{
				MyBob.CurInput.xVec = new Vector2(0, 1);
			}

            int StickyWaitLength = 7;

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
                    //MyBob.CurInput.xVec.X = StickyDir;
                    //MyBob.CurInput.A_Button = false;
					MyBob.CurInput.xVec = -JumpDir;
                }
                else
                {
                    //MyBob.CurInput.A_Button = true;
					MyBob.CurInput.xVec = -JumpDir;

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
				{
					//MyBob.CurInput.A_Button = true;
					MyBob.CurInput.xVec = -JumpDir;
				}
			}
			else
			{
				if (StepsSinceSide >= 4)
				{
					//MyBob.CurInput.A_Button = true;
					MyBob.CurInput.xVec = -JumpDir;
				}
			}

            // Regular jump
            if (OnGround)
                StraightUpDuration = 18;
            if (StraightUpDuration > 0)
            {
                MyBob.CurInput.xVec.X = -StickyDir;
                //MyBob.CurInput.A_Button = true;
				MyBob.CurInput.xVec = -JumpDir;
                StraightUpDuration--;

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

			if (JumpDir.Y > 0 && AirTime > 40)
				MyBob.WantsToLand = true;
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

			makeData.TopLikeBottom_Thin = true;
			makeData.BlocksAsIs = true;
			Style.UseLowerBlockBounds = true;
			Style.OverlapCleanupType = StyleData._OverlapCleanupType.Sophisticated;


			Style.NoTopOnly = true;

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