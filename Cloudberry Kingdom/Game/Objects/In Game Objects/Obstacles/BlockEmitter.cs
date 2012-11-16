using System.IO;
using Microsoft.Xna.Framework;




using System.Collections.Generic;


namespace CloudberryKingdom
{
    public class BlockEmitter : ObjectBase
    {
        public MovingPlatform.MoveType MyMoveType;
        public float Amp;

        List<MovingPlatform> Platforms = new List<MovingPlatform>();

        public override void Release()
        {
            base.Release();

            Platforms.Clear();
        }

        public bool AlwaysOn, Active;
        public Vector2 Range;

        public PhsxData EmitData;
        public int Delay, Offset;
        public Vector2 Size;

        public override void OnUsed()
        {
            foreach (BlockBase platform in Platforms)
                platform.Core.GenData.Used = true;
        }

        public override void OnMarkedForDeletion()
        {
            if (!Core.DeletedByBob) return;

            // Delete all children platforms
            foreach (MovingPlatform platform in Platforms)
            {
                platform.Parent = null; // Make sure we destroy this connection to prevent an infinite recursion
                platform.CollectSelf();
            }
            
            Platforms.Clear();
        }
        
        public override void MakeNew()
        {
            Platforms.Clear();

            MyMoveType = MovingPlatform.MoveType.Normal;
            Amp = 0;

            Active = true;

            Core.Init();
            Core.MyType = ObjectType.BlockEmitter;
            Core.DrawLayer = 1;
            GiveLayer = false;

            Range = new Vector2(8000, 3000);
            GiveCustomRange = false;

            if (DoPreEmit)
                SetToPreEmit = true;
        }

        public BlockEmitter(bool BoxesOnly)
        {            
            Core.BoxesOnly = BoxesOnly;

            MakeNew();
        }

        /// <summary>
        /// If true the emitter emits multiple blocks prior to the level starting.
        /// This simulates the behavior of the level having been running before starting.
        /// </summary>
        public bool DoPreEmit = true;

        /// <summary>
        /// When true the emitter will PreEmit on the next phsx step
        /// </summary>
        bool SetToPreEmit = false;

        public override void Reset(bool BoxesOnly)
        {
            if (DoPreEmit)
                SetToPreEmit = true;
        }

        void PreEmit()
        {
            SetToPreEmit = false;

            int step = Offset - Delay;
            if (Offset == 0) step = 0;
            for (; step > -60 * 10; step -= Delay)
                Emit(step);
        }

        public bool GiveCustomRange = false;
        public bool GiveLayer = false;
        void Emit(int offset)
        {
            MovingPlatform block = (MovingPlatform)Core.Recycle.GetObject(ObjectType.MovingPlatform, Core.BoxesOnly);

            block.Parent = this;
            block.Init(EmitData.Position, Size, MyLevel, MyBoxStyle);

            if (GiveLayer) block.Core.DrawLayer = Core.DrawLayer;

            block.Core.Tag = Core.Tag;

            block.Core.Active = true;
            block.Range = Range;
            if (Core.MyLevel.Geometry == LevelGeometry.Up || Core.MyLevel.Geometry == LevelGeometry.Down)
                block.Range.X = 300;
            else
                block.Range.Y = 300;
            if (GiveCustomRange)
                block.Range = Range;

            EmitData.Acceleration.Y = EmitData.Velocity.Y;
            block.Core.StartData = block.BlockCore.Data = EmitData;

            block.MyMoveType = MyMoveType;
            block.Amp = Amp;
            block.Offset = offset;

            // Set Gen data
            if (Core.BoxesOnly)
            {
                block.Core.GenData.RemoveIfOverlap = Core.GenData.RemoveIfOverlap;
                block.BlockCore.Virgin = true;
                block.BlockCore.Finalized = true;
                block.Core.GenData.AlwaysLandOn_Reluctantly = true;
            }

            Core.MyLevel.AddBlock(block);
            AddPlatform(block);
            block.Core.WakeUpRequirements = true;
        }

        public void AddPlatform(MovingPlatform platform)
        {
            Platforms.Add(platform);
        }

        public void RemovePlatform(MovingPlatform platform)
        {
            Platforms.Remove(platform);
        }

        public override void PhsxStep()
        {
            if (SetToPreEmit) PreEmit();

            if (!Active) return;

            if (!AlwaysOn)
                if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + Range.X ||
                    Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - Range.X ||
                    Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + Range.Y ||
                    Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - Range.Y)
                {
                    Core.SkippedPhsx = true;
                    return;
                }
            Core.SkippedPhsx = false;

            if (CoreMath.Modulo(Core.GetPhsxStep(), Delay) == Offset)
            {
                Emit(Core.MyLevel.CurPhsxStep);
            }
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);

            EmitData.Position += shift;
        }

        BlockEmitter_Parameters.BoxStyle MyBoxStyle;
        public void Init(Vector2 pos, Levels.Level level, BlockEmitter_Parameters.BoxStyle MyBoxStyle)
        {
            base.Init(pos, level);

            this.MyBoxStyle = MyBoxStyle;
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            BlockEmitter EmitterA = A as BlockEmitter;
            Init(A.Pos, A.MyLevel, EmitterA.MyBoxStyle);

            EmitData = EmitterA.EmitData;
            Delay = EmitterA.Delay;
            Offset = EmitterA.Offset;
            Size = EmitterA.Size;
            GiveLayer = EmitterA.GiveLayer;

            Amp = EmitterA.Amp;
            MyMoveType = EmitterA.MyMoveType;

            Range = EmitterA.Range;
            GiveCustomRange = EmitterA.GiveCustomRange;
        }
    }
}