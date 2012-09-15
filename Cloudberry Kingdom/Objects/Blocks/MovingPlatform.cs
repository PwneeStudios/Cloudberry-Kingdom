using System;
using System.IO;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Blocks
{
    public class MovingPlatform : BlockBase
    {
        public class ElevatorTileInfo : TileInfoBase
        {
            public BlockGroup Group = PieceQuad.ElevatorGroup;
        }

        public enum MoveType { Normal, Sine };
        public MoveType MyMoveType;
        public float Amp;
        public int Offset;

        public Vector2 Range;

        public BlockEmitter Parent;

        BlockEmitter_Parameters MyParams
        {
            get { return (BlockEmitter_Parameters)Core.MyLevel.Style.FindParams(BlockEmitter_AutoGen.Instance); }
        }

        public override bool PermissionToUse()
        {
            if (MyParams.MyStyle == BlockEmitter_Parameters.Style.Separated)
            {
                // Don't let the computer use another elevator too soon after using another one.
                if (Core.MyLevel.CurPhsxStep - MyParams.LastUsedTimeStamp > 5)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        public override void LandedOn(Bob bob)
        {
            if (Core.MyLevel.PlayMode == 2)
            {
                MyParams.LastUsedTimeStamp = Core.MyLevel.CurPhsxStep;

                if (MyParams.MyStyle == BlockEmitter_Parameters.Style.Separated)
                    Core.GenData.EdgeJumpOnly = true;
            }
        }
        public override void OnUsed()
        {
            MyParams.LastUsedTimeStamp = Core.MyLevel.CurPhsxStep;
            Parent.StampAsUsed(Core.MyLevel.CurPhsxStep);
        }

        public override void OnMarkedForDeletion()
        {
            if (Parent != null)
                Parent.RemovePlatform(this);

            if (!Core.DeletedByBob) return;

            if (Parent != null)
            {
                Parent.Core.DeletedByBob = true;
                Parent.CollectSelf();
            }
        }

        public override void MakeNew()
        {
            MyMoveType = MoveType.Normal;

            Core.Init();
            BlockCore.MyType = ObjectType.MovingPlatform;
            Core.DrawLayer = 4;

            MyBox.TopOnly = true;
        }

        public override void Release()
        {
            base.Release();

            Parent = null;
        }

        public MovingPlatform(bool BoxesOnly)
        {
            MyBox = new AABox();
            MyDraw = new NormalBlockDraw();

            Core.BoxesOnly = BoxesOnly;
            MakeNew();
        }

        BlockEmitter_Parameters.BoxStyle MyBoxStyle;
        public void Init(Vector2 center, Vector2 size, Level level, BlockEmitter_Parameters.BoxStyle boxstyle)
        {
            MyBoxStyle = boxstyle;

            if (boxstyle == BlockEmitter_Parameters.BoxStyle.FullBox)
                Box.TopOnly = false;

            //// Not TopOnly if hero is a spaceship.
            //if (Parent != null && Parent.Core.MyLevel.DefaultHeroType is BobPhsxSpaceship && Box.TopOnly)
            //{
            //    Box.TopOnly = false;
            //}

            Range = new Vector2(1800, 1800);
                        
            Active = true;

            BlockCore.Layer = .3f;

            base.Init(ref center, ref size, level, level.Info.Elevators.Group);
            Reset(level.BoxesOnly);

            Core.RemoveOnReset = true;
            BlockCore.HitHead = true;
        }


        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            Active = true;

            BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;
            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();
        }

        public override void PhsxStep()
        {
            if (!Active) return;

            if (BlockCore.Data.Position.X > BlockCore.MyLevel.MainCamera.TR.X + Range.X ||
                BlockCore.Data.Position.X < BlockCore.MyLevel.MainCamera.BL.X - Range.X ||
                BlockCore.Data.Position.Y > BlockCore.MyLevel.MainCamera.TR.Y + Range.Y ||
                BlockCore.Data.Position.Y < BlockCore.MyLevel.MainCamera.BL.Y - Range.Y)
            {
                CollectSelf();

                Active = false;
                return;
            }

            /*
            int Period = 180;
            int Step = (Core.GetPhsxStep() + Offset) % Period;
            float t = (float)Step / Period;*/

            int Step = Core.GetPhsxStep() - Offset;
            int Period = 180;
            float t = (float)Step / Period;

            //MyBox.Target.Center = MyBox.Current.Center + CoreData.Data.Velocity;
            MyBox.Target.Center = Core.StartData.Position + Step * BlockCore.Data.Velocity;

            switch (MyMoveType)
            {
                case MoveType.Sine:
                    MyBox.Target.Center.X = Core.StartData.Position.X + Amp * (float)Math.Cos(2 * Math.PI * t);
                    break;

                case MoveType.Normal:
                    break;
            }

            //CoreData.Data.Velocity += CoreData.Data.Acceleration;
            BlockCore.Data.Position = MyBox.Target.Center;

            MyBox.SetTarget(MyBox.Target.Center, MyBox.Current.Size);

            if (Core.WakeUpRequirements)
            {
                MyBox.SwapToCurrent();
                Core.WakeUpRequirements = false;
            }
        }

        public override void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }

        public override void Draw()
        {
            if (!Active || (Parent != null && !Parent.Active)) return;

            Vector2 BL = MyBox.Current.BL;//MyQuad.BL();
            if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y)
                return;
            Vector2 TR = MyBox.Current.TR;// MyQuad.TR();
            if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y)
                return;

            if (Tools.DrawBoxes)
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);

            if (BlockCore.BoxesOnly) return;

            if (Tools.DrawGraphics)
            {
                MyDraw.Update();
                MyDraw.Draw();
            }
        }

        public override void Clone(ObjectBase A)
        {
            MovingPlatform BlockA = A as MovingPlatform;
            BlockCore.Clone(A.Core);

            Parent = BlockA.Parent;

            Amp = BlockA.Amp;
            Offset = BlockA.Offset;

            Range = BlockA.Range;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, A.MyLevel, MyBoxStyle);

            Active = BlockA.Active;
        }
    }
}
