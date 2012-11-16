using System;
using System.IO;
using Microsoft.Xna.Framework;






namespace CloudberryKingdom
{
    public enum BouncyBlockState { Regular, SuperStiff };
    public class BouncyBlock : BlockBase
    {
        public class BouncyBlockTileInfo : TileInfoBase
        {
            public BlockGroup Group = PieceQuad.BouncyGroup;
            public EzSound BounceSound = Tools.SoundWad.FindByName("BouncyBlock_Bounce");
        }

        BouncyBlockState State;
        public Vector2 Offset, SizeOffset;
        public float speed;

        int TouchedCountdown = 0;

        public override void MakeNew()
        {
            Core.Init();
            //BlockCore.GivesVelocity = false;
            Core.DrawLayer = 3;
            BlockCore.MyType = ObjectType.BouncyBlock;

            SideDampening = 1f;

            SetState(BouncyBlockState.Regular);
        }

        public void SetState(BouncyBlockState NewState) { SetState(NewState, false); }
        public void SetState(BouncyBlockState NewState, bool ForceSet)
        {
            if (State != NewState || ForceSet)
            {
                switch (NewState)
                {
                    case BouncyBlockState.Regular:
                        if (!Core.BoxesOnly) MyDraw.MyPieces.CalcTexture(0, 0);
                        break;
                    case BouncyBlockState.SuperStiff:
                        if (!Core.BoxesOnly) MyDraw.MyPieces.CalcTexture(0, 1);
                        break;
                }
            }

            State = NewState;
         }

        public BouncyBlock(bool BoxesOnly)
        {
            MyBox = new AABox();
            MyDraw = new NormalBlockDraw();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Init(Vector2 center, Vector2 size, float speed, Level level)
        {
            Active = true;
            
            this.speed = speed;

            BlockCore.Layer = .35f;

            base.Init(ref center, ref size, level, level.Info.BouncyBlocks.Group);

            SetState(BouncyBlockState.Regular, true);

            Update();
        }

        public float SideDampening;
        void Snap(Bob bob)
        {
            TouchedCountdown = 3;

            if (Offset.X != 0)
                bob.Core.Data.Velocity.X = 1.1f * speed * Offset.X * SideDampening;
            if (Offset.Y != 0)
            {
                bob.NewVel = speed * Offset.Y;
                bob.MyPhsx.OnGround = false;
                bob.MyPhsx.DisableJump(5);
                bob.MyPhsx.DampForcedJump();
            }

            float scale = 1.2f;
            Offset *= 42 * scale;
            SizeOffset = new Vector2(14 * scale);

            //Offset *= 52;
            //SizeOffset = new Vector2(18);

            SetState(BouncyBlockState.SuperStiff);
        }

        public override void SideHit(Bob bob)
        {            
            Offset = new Vector2(Math.Sign(bob.Core.Data.Position.X - Core.Data.Position.X), 0);
            bob.MyPhsx.Forced(Offset);

            Snap(bob);
        }

        public override void LandedOn(Bob bob)
        {
            bob.MyPhsx.OverrideSticky = true;

            Offset = new Vector2(0, 1);
            bob.MyPhsx.Forced(Offset);

            bob.MyPhsx.MaxJumpAccelMultiple = 2.5f;
            bob.MyPhsx.JumpLengthModifier = .85f;
            Snap(bob);
        }
        
        public override void HitHeadOn(Bob bob)
        {
            bob.MyPhsx.KillJump();
            Offset = new Vector2(0, -1);
            bob.MyPhsx.Forced(Offset);

            Snap(bob);
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            Active = true;

            Offset = Vector2.Zero;
            SizeOffset = Vector2.Zero;

            SetState(BouncyBlockState.Regular, true);

            BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }

        public override void PhsxStep()
        {
            Active = Core.Active = true;
            if (!Core.Held)
            {
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y + 50)
                    Active = Core.Active = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 150)
                    Active = Core.Active = false;
            }

            Update();

            //if (!Core.BoxesOnly)
            //{
            //    if ((Pos - MyLevel.Bobs[0].Pos).Length() > 500)
            //        MyDraw.MyPieces.CalcTexture(0, 2);
            //    else
            //        //if (MyDraw.MyPieces.t > 0)
            //            MyDraw.MyPieces.CalcTexture(0, 0);
            //}

            if (TouchedCountdown == 0)
            {
                Offset *= .5f;
                SizeOffset *= .5f;
            }
            else
                TouchedCountdown--;

            // Update the block's apparent center according to attached objects
            BlockCore.UseCustomCenterAsParent = true;
            BlockCore.CustomCenterAsParent = Box.Target.Center + Offset;
            BlockCore.OffsetMultAsParent = Vector2.One + Vector2.Divide(SizeOffset, Box.Current.Size);

            if (SizeOffset.X < .1f && State == BouncyBlockState.SuperStiff)
                SetState(BouncyBlockState.Regular);

            MyBox.SetTarget(Core.Data.Position, MyBox.Current.Size);

            BlockCore.StoodOn = false;
        }

        public override void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }

        public void Update()
        {
            if (BlockCore.BoxesOnly) return;           
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

            Update();

            BlockCore.StartData.Position = MyBox.Current.Center;
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public override void Draw()
        {
            Update();

            if (Tools.DrawBoxes)
            {
                //MyBox.Draw(Tools.QDrawer, Color.Olive, 15);
                MyBox.DrawFilled(Tools.QDrawer, Color.SpringGreen);
            }

            if (Tools.DrawGraphics)
            {
                if (!BlockCore.BoxesOnly)
                {
                    //MyDraw.MyPieces.Center.MyEffect = Tools.EffectWad.FindByName("Hsl");
                    MyDraw.Update();
                    MyDraw.MyPieces.Base.Origin += Offset;
                    //MyDraw.MyPieces.Base.Origin += Offset
                    //    + new Vector2(MyLevel.Rnd.Rnd.Next(-10, 10), MyLevel.Rnd.Rnd.Next(-10, 10)); ;

                    MyDraw.Draw();
                }
            }

            BlockCore.Draw();
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            BouncyBlock BlockA = A as BouncyBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, BlockA.speed, BlockA.MyLevel);

            SideDampening = BlockA.SideDampening;

            State = BlockA.State;
        }
    }
}
