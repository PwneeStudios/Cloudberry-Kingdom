using Microsoft.Xna.Framework;

using System;




namespace CloudberryKingdom
{
    public class BobPhsxInvert : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(0, 0, 3, 0);
            Name = Localization.Words.Viridian;
            Adjective = "Anti-Grav";
            
            Icon = new PictureIcon(Tools.TextureWad.FindByName("Bob_Run_0024"), Color.White, DefaultIconWidth * -1.2f);

            HeroDollShift = new Vector2(0, 100);
        }
        static readonly BobPhsxInvert instance = new BobPhsxInvert();
        public static new BobPhsxInvert Instance { get { return instance; } }

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
                normal.BobInitialJumpSpeed = 0;
                normal.Gravity = 4;
                normal.BobMaxFallSpeed = -30;
            }
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            BobJumpAccel = -Math.Abs(BobJumpAccel);

            Gravity = Math.Abs(Gravity);
            BobInitialJumpSpeed = Math.Abs(BobInitialJumpSpeed);
            BobInitialJumpSpeed2 = Math.Abs(BobInitialJumpSpeed2);
            BobInitialJumpSpeedDucking = Math.Abs(BobInitialJumpSpeedDucking);
            BobInitialJumpSpeedDucking2 = Math.Abs(BobInitialJumpSpeedDucking2);
            BobMaxFallSpeed = -Math.Abs(BobMaxFallSpeed);
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            Obj.yFlip = Gravity < 0;

            // Cape is always fallling opposite to the direction of Bob
            //CapeGravity = Math.Sign(-Gravity) * new Vector2(0, -1.45f) / 1.45f;

            // Cape is always fallling up
            CapeGravity = new Vector2(0, -1.45f) / 1.45f;

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

            BobInitialJumpSpeed *= -1;
        }

        public void Invert()
        {
            BobJumpAccel *= -1;

            Gravity *= -1;
            ForceDown *= -1;

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
                    float ChanceToPause = .3f;
                    if (MyLevel.CurPhsxStep > 600) ChanceToPause = .15f;
                    else if (MyLevel.CurPhsxStep > 850) ChanceToPause = .05f;
                    else if (MyLevel.CurPhsxStep > 950) ChanceToPause = 0f;

                    if (MyLevel.Rnd.RndFloat() < ChanceToPause)
                    {
                        CurBehavior = Behavior.Pause;
                        BehaviorLength = MyLevel.Rnd.RndInt(3, 10);
                    }
                    else
                    {
                        CurBehavior = Behavior.Regular;
                        BehaviorLength = MyLevel.Rnd.RndInt(35, 60);
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

            if (Pos.Y > 600 && Gravity < 0 ||
                Pos.Y < -550 && Gravity > 0)
            {
                MyBob.CurInput.A_Button = true;
                MyBob.Count_ButtonA = 0;
            }
            if (Pos.Y > 600 && Gravity > 0 ||
                Pos.Y < -550 && Gravity < 0)
            {
                MyBob.CurInput.A_Button = false;
                MyBob.Count_ButtonA = 0;
            }
        }

        int Count = 0;
        protected override void SetTarget(RichLevelGenData GenData)
        {
            if (Count <= 0 || Math.Abs(MyBob.TargetPosition.Y - Pos.Y) < 200)
            {
                Count = MyLevel.Rnd.RndInt(30, 60);

                if (Pos.Y > Cam.Pos.Y)
                    MyBob.TargetPosition.Y = MyBob.MoveData.MinTargetY;
                else
                    MyBob.TargetPosition.Y = MyBob.MoveData.MaxTargetY - MyLevel.Rnd.RndFloat(400, 900);
            }
            else
                Count--;
        }

        protected override void PreventEarlyLandings(RichLevelGenData GenData)
        {
            // Do nothing
        }

        public override void ModData(ref Level.MakeData makeData, StyleData Style)
        {
            base.ModData(ref makeData, Style);

            makeData.TopLikeBottom_Thin = true;
            makeData.BlocksAsIs = true;

            var Ceiling_Params = (Ceiling_Parameters)Style.FindParams(Ceiling_AutoGen.Instance);
            Ceiling_Params.Make = false;

            Style.BlockFillType = StyleData._BlockFillType.Invertable;
            Style.OverlapCleanupType = StyleData._OverlapCleanupType.Sophisticated;

            Style.TopSpace = 50;

            var MParams = (MovingBlock_Parameters)Style.FindParams(MovingBlock_AutoGen.Instance);
            if (MParams.Aspect == MovingBlock_Parameters.AspectType.Tall)
                MParams.Aspect = MovingBlock_Parameters.AspectType.Thin;

            var GhParams = (GhostBlock_Parameters)Style.FindParams(GhostBlock_AutoGen.Instance);
            GhParams.BoxType = GhostBlock_Parameters.BoxTypes.Full;
        }

        public override void ModLadderPiece(PieceSeedData piece)
        {
            base.ModLadderPiece(piece);

            piece.ElevatorBoxStyle = BlockEmitter_Parameters.BoxStyle.FullBox;
        }

        public override bool IsBottomCollision(ColType Col, AABox box, BlockBase block)
        {
            return Col == ColType.Bottom ||
                Col != ColType.Bottom && Core.Data.Velocity.X != 0 && Math.Min(MyBob.Box.Current.TR.Y, MyBob.Box.Target.TR.Y) < box.Target.BL.Y + Math.Max(1.35 * Core.Data.Velocity.Y, 7);
        }

        public override void DollInitialize()
        {
            base.DollInitialize();

            Invert();
            LandOnSomething(true, null);
        }
    }
}