using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
	public class BobPhsxBlobby : BobPhsxSpaceship
	{
		// Singleton
		protected override void InitSingleton()
		{
			base.InitSingleton();

			Specification = new HeroSpec(7, 0, 0, 0);
            Name = Localization.Words.FlappyBlob;
			NameTemplate = "blob ";
			Icon = new PictureIcon(Tools.Texture("Blob_Cave_1"), Color.White, 1.15f * DefaultIconWidth);
		}
		static readonly BobPhsxBlobby instance = new BobPhsxBlobby();
		public static BobPhsxBlobby Instance { get { return instance; } }

		// Instancable class
		public override void DefaultValues()
		{
			base.DefaultValues();

			Gravity = 3f;
			MaxSpeed = 17f;

			XAccel = 2.3f;
		}

        public override void PreObjectDraw()
        {
            base.PreObjectDraw();
        }

		float Downward = 0;
		public override void PhsxStep()
		{
			MyBob.CurInput.xVec.X = 1;
			
			xVel = MaxSpeed;
			yVel -= Gravity;
			if (yVel < BobMaxFallSpeed) yVel = BobMaxFallSpeed;

			MyLevel.MyCamera.MovingCamera = true;

			OnGround = false;

			Jump();

			if (Oscillate) OscillatePhsx();

			if (!MyBob.BoxesOnly)
			{
				var quad = (Quad)MyBob.PlayerObject.QuadList[1];

				int frame = (Tools.DrawCount / 4) % 4 + 1;
				quad.MyTexture = Tools.Texture("blob_cave_" + frame.ToString());

				if (yVel < 0)
				{
					Downward += 1f * (float)Math.Pow(yVel / BobMaxFallSpeed, 2) * (Downward / 10f + .1f);
				}
				else
				{
					Downward = 0;
				}

				quad.PointxAxisTo(new Vector2(1, -Math.Min(10f, Downward) / 15.5f));
			}
		}

		public override void PhsxStep2()
		{
			//base.PhsxStep2();
		}

		bool CanJump = false;
		public override void Jump()
		{
			if (!MyBob.CurInput.A_Button) CanJump = true;

			if (JumpDelayCount <= 0 && CanJump && MyBob.CurInput.A_Button)
			{
				CanJump = false;
				DoJump();
			}
			else
			{
				JumpDelayCount--;
			}
		}

		int JumpDelayCount = 0;
		const int JumpDelay = 5;
		void DoJump()
		{
			JumpDelayCount = JumpDelay;
			yVel = 40;
		}

		protected override void GenerateInput_Right(int CurPhsxStep)
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

			if (MyBob.Core.Data.Position.Y < MyBob.TargetPosition.Y)
				MyBob.CurInput.xVec.Y = 1;
			if (MyBob.Core.Data.Position.Y > MyBob.TargetPosition.Y)
				MyBob.CurInput.xVec.Y = -1;
			MyBob.CurInput.xVec.Y *= Math.Min(1, Math.Abs(MyBob.TargetPosition.Y - MyBob.Core.Data.Position.Y) / 100);

			if (Pos.X > CurPhsxStep * 1.1f * (4000f / 600f))
			{
				if (Pos.Y > MyBob.TargetPosition.Y && (CurPhsxStep / 40) % 3 == 0)
					MyBob.CurInput.xVec.X = -1;
				if (Pos.Y < MyBob.TargetPosition.Y && (CurPhsxStep / 25) % 4 == 0)
					MyBob.CurInput.xVec.X = -1;
			}
			if (Pos.Y < MyBob.TargetPosition.Y && Pos.X < CurPhsxStep * (4000f / 900f))
			{
				MyBob.CurInput.xVec.X = 1;
			}

			if (Pos.X < MyLevel.MainCamera.BL.X + 400)
				MyBob.CurInput.xVec.X = 1;
			if (Pos.X > MyLevel.MainCamera.TR.X - 500 && MyBob.CurInput.xVec.X > 0)
				MyBob.CurInput.xVec.X /= 2;

			if (Pos.X > MyLevel.Fill_TR.X - 1200)
			{
				if (Pos.Y > MyLevel.MainCamera.TR.Y - 600) MyBob.CurInput.xVec.Y = -1;
				if (Pos.Y < MyLevel.MainCamera.BL.Y + 600) MyBob.CurInput.xVec.Y = 1;
			}

			if (MyBob.Core.Data.Position.X > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.X ||
				MyBob.Core.Data.Position.Y > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.Y)
			{
				MyBob.CurInput.xVec.X = 0;
			}

			if (MyBob.CurInput.xVec.Y > 0)
				MyBob.CurInput.A_Button = true;
			else
				MyBob.CurInput.A_Button = false;
			MyBob.CurInput.xVec.Y = 0;

			// Let go of A for 1 frame
			if (MyBob.PrevInput.A_Button && yVel < -5)
				MyBob.CurInput.A_Button = false;

            MyBob.CurInput.xVec.X = 1;
		}
	}
}