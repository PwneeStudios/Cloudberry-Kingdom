using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public enum FallingBlockState { Regular, Touched, Falling, Angry };
    public class FallingBlock : BlockBase
    {
        public bool TouchedOnce, HitGround;

        public bool Thwomp;
        public Vector2 AngryAccel;
        public float AngryMaxSpeed;

        public int StartLife, Life;
        //public int StartLife { get { return _StartLife; } set { _StartLife = value; if (value > 20) Console.WriteLine("!");  } }

        FallingBlockState State;
        bool EmittedExplosion;
        public Vector2 Offset;
        int ResetTimer;
        public static int ResetTimerLength = 12;

        public override void MakeNew()
        {
            Thwomp = false;

            EmittedExplosion = HitGround = false;

            Core.Init();
            Core.DrawLayer = 3;
            BlockCore.MyType = ObjectType.FallingBlock;

            SetState(FallingBlockState.Regular);
        }

        public override void Release()
        {
            base.Release();

            MyDraw.Release();
            MyDraw = null;
        }

        public void SetState(FallingBlockState NewState) { SetState(NewState, false); }
        public void SetState(FallingBlockState NewState, bool ForceSet)
        {
            if (State != NewState || ForceSet)
            {
                switch (NewState)
                {
                    case FallingBlockState.Regular:
                        TouchedOnce = HitGround = false;
                        if (!Core.BoxesOnly) MyDraw.MyPieces.CalcTexture(0, 0);
                        break;
                    case FallingBlockState.Touched:
                        HitGround = false;
                        if (!Core.BoxesOnly) MyDraw.MyPieces.CalcTexture(0, 1);
                        break;
                    case FallingBlockState.Falling:
                        if (!Core.BoxesOnly) MyDraw.MyPieces.CalcTexture(0, 2);
                        break;
                    case FallingBlockState.Angry:
                        if (!Core.BoxesOnly) MyDraw.MyPieces.CalcTexture(0, 3);
                        break;
                }
            }

            State = NewState;
        }

        public FallingBlock(bool BoxesOnly)
        {
            MyBox = new AABox();
            MyDraw = new NormalBlockDraw();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Init(Vector2 center, Vector2 size, int life, Level level)
        {
            Active = true;

            Life = StartLife = life;

            BlockCore.Layer = .35f;

            var tile = Core.MyTileSet = level.MyTileSet;
            Tools.Assert(Core.MyTileSet != null);

            // Set the size based on tileset limitations.
            if (tile.FixedWidths)
            {
                tile.FallingBlocks.SnapWidthUp(ref size);
                MyBox.Initialize(center, size);
                MyDraw.MyTemplate = tile.GetPieceTemplate(this, level.Rnd, tile.FallingBlocks);
                //MyDraw.MyTemplate = tile.GetPieceTemplate(this, level.Rnd, tile.Pillars);
            }
            else
            {
                MyBox = new AABox(center, size);
                MyDraw.MyTemplate = PieceQuad.FallingBlock;
            }

            BlockCore.StartData.Position = Pos = center;

            if (!Core.BoxesOnly)
                MyDraw.Init(this);

            SetState(FallingBlockState.Regular, true);
        }

        public override void HitHeadOn(Bob bob)
        {
            /* Use this if you want inverted Bobs to set off falling blocks
             * 
            // We only care about inverted Bobs landing on us upside down.
            if (bob.MyPhsx.Gravity > 0) return;

            // Don't register as a land if the Bob is moving downward.
            if (bob.Core.Data.Velocity.Y < -3)
            {
                BlockCore.StoodOn = false;
                return;
            }

            if (State == FallingBlockState.Regular)
                SetState(FallingBlockState.Touched);

            if (bob.Core.Data.Velocity.Y > 10)
                Life -= 8;
             * */
        }

        public override void LandedOn(Bob bob)
        {
            // Don't register as a land if the Bob is moving upward.
            if (bob.Core.Data.Velocity.Y > 3)
            {
                BlockCore.StoodOn = false;
                return;
            }

            if (State == FallingBlockState.Regular)
                    SetState(FallingBlockState.Touched);

            if (bob.Core.Data.Velocity.Y < -10)
                Life -= 8;
        }

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            if (!Core.BoxesOnly)
                ResetPieces();           

            Active = true;

            Life = StartLife;
            TouchedOnce = false;
            EmittedExplosion = false;

            ResetTimer = 0;

            SetState(FallingBlockState.Regular, true);

            BlockCore.Data = BlockCore.StartData;

            BlockCore.StoodOn = false;

            MyBox.Current.Center = BlockCore.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();
        }

        public override void PhsxStep()
        {
            Active = Core.Active = true;
            if (!Core.Held)
            {
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y)
                    Active = Core.Active = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 200)
                    Active = Core.Active = false;
            }


            if (Core.MyLevel.GetPhsxStep() % 2 == 0)
                Offset = Vector2.Zero;

            // Update the block's apparent center according to attached objects
            BlockCore.UseCustomCenterAsParent = true;
            BlockCore.CustomCenterAsParent = Box.Target.Center + Offset;

            if (BlockCore.StoodOn)
            {
                ResetTimer = ResetTimerLength;

                TouchedOnce = true;

                if (Core.MyLevel.GetPhsxStep() % 2 == 0)
                    if (Life > 0)
                    {
                        Offset = new Vector2(MyLevel.Rnd.Rnd.Next(-10, 10), MyLevel.Rnd.Rnd.Next(-10, 10));
                    }
            }
            else
            {
                if (State == FallingBlockState.Touched)
                {
                    ResetTimer--;
                    if (ResetTimer <= 0)
                        SetState(FallingBlockState.Regular);
                }
            }

            if (State == FallingBlockState.Angry)
            {
                BlockCore.Data.Velocity.Y += AngryAccel.Y;
                if (BlockCore.Data.Velocity.Y > AngryMaxSpeed) BlockCore.Data.Velocity.Y = AngryMaxSpeed;
                MyBox.Current.Center.Y += BlockCore.Data.Velocity.Y;
            }
            else
            {
                if (BlockCore.StoodOn || Life < StartLife / 3) Life--;
                if (Life <= 0)
                {
                    if (State != FallingBlockState.Falling)
                    {
                        if (Thwomp)
                            SetState(FallingBlockState.Angry);
                        else
                            SetState(FallingBlockState.Falling);
                    }
                    BlockCore.Data.Velocity.Y -= 1;
                    if (BlockCore.Data.Velocity.Y < -20) BlockCore.Data.Velocity.Y = -20;
                    MyBox.Current.Center.Y += BlockCore.Data.Velocity.Y;
                }

                // Check for hitting bottom of screen
                if (State == FallingBlockState.Falling && 
                    (Core.MyLevel.Geometry != LevelGeometry.Up && Core.MyLevel.Geometry != LevelGeometry.Down))
                {
                    if (MyBox.Current.Center.Y < BlockCore.MyLevel.MainCamera.BL.Y - 200)
                    {
                        // Emit a dust plume if the game is in draw mode and there isn't any lava
                        if (Core.MyLevel.PlayMode == 0 && !EmittedExplosion &&
                            !Tools.CurGameData.HasLava)
                        {
                            EmittedExplosion = true;
                            ParticleEffects.DustCloudExplosion(Core.MyLevel, MyBox.Current.Center);
                        }
                    }
                    if (MyBox.Current.Center.Y < BlockCore.MyLevel.MainCamera.BL.Y - 500) Active = false;
                }
            }

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);

            BlockCore.StoodOn = false;
        }

        public override void Extend(Side side, float pos)
        {
            switch (side)
            {
                case Side.Left:
                    MyBox.Target.BL.X = pos;
                    break;
                case Side.Right:
                    MyBox.Target.TR.X = pos;
                    break;
                case Side.Top:
                    MyBox.Target.TR.Y = pos;
                    break;
                case Side.Bottom:
                    MyBox.Target.BL.Y = pos;
                    break;
            }

            MyBox.Target.FromBounds();
            MyBox.SwapToCurrent();

            BlockCore.StartData.Position = MyBox.Current.Center;
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);
        }
        public override void Draw()
        {
            bool DrawSelf = true;
            if (!Core.Held)
            {
                if (!Active) DrawSelf = false;

                if (!Core.MyLevel.MainCamera.OnScreen(MyBox.Current.Center, 600))
                //if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 600))
                    DrawSelf = false;
            }

            if (DrawSelf)
            {
                if (Tools.DrawBoxes)
                {
                    //MyBox.Draw(Tools.QDrawer, Color.Olive, 15);
                    MyBox.DrawFilled(Tools.QDrawer, Color.Red);
                }
            }

            if (Tools.DrawGraphics)
            {
                //if (State != FallingBlockState.Falling) return;

                if (DrawSelf && !BlockCore.BoxesOnly)
                if (!BlockCore.BoxesOnly)
                {
                    MyDraw.Update();
                    MyDraw.MyPieces.Base.Origin += Offset;

                    MyDraw.Draw();
                }
            }

            BlockCore.Draw();
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            FallingBlock BlockA = A as FallingBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, BlockA.StartLife, BlockA.MyLevel);

            TouchedOnce = BlockA.TouchedOnce;
            StartLife = BlockA.StartLife;
            Life = BlockA.Life;

            EmittedExplosion = BlockA.EmittedExplosion;

            Thwomp = BlockA.Thwomp;
        }
    }
}
