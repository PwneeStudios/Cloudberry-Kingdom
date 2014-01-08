using System.IO;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

using System.Collections.Generic;
using CloudberryKingdom.Levels;

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
            Platforms.ForEach(platform => platform.CoreData.GenData.Used = true);
        }

        public override void OnMarkedForDeletion()
        {
            if (!CoreData.DeletedByBob) return;

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

            CoreData.Init();
            CoreData.MyType = ObjectType.BlockEmitter;
            CoreData.DrawLayer = 1;
            GiveLayer = false;

            Range = new Vector2(8000, 3000);
            GiveCustomRange = false;

            if (DoPreEmit)
                SetToPreEmit = true;
        }

        public BlockEmitter(bool BoxesOnly)
        {            
            CoreData.BoxesOnly = BoxesOnly;

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
            MovingPlatform block = (MovingPlatform)CoreData.Recycle.GetObject(ObjectType.MovingPlatform, CoreData.BoxesOnly);

            block.Parent = this;
            block.Init(EmitData.Position, Size, MyLevel, MyBoxStyle);

            if (GiveLayer) block.CoreData.DrawLayer = CoreData.DrawLayer;

            block.CoreData.Tag = CoreData.Tag;

            block.CoreData.Active = true;
            block.Range = Range;
            if (CoreData.MyLevel.Geometry == LevelGeometry.Up || CoreData.MyLevel.Geometry == LevelGeometry.Down)
                block.Range.X = 300;
            else
                block.Range.Y = 300;
            if (GiveCustomRange)
                block.Range = Range;

            EmitData.Acceleration.Y = EmitData.Velocity.Y;
            block.CoreData.StartData = block.BlockCore.Data = EmitData;

            block.MyMoveType = MyMoveType;
            block.Amp = Amp;
            block.Offset = offset;

            // Set Gen data
            if (CoreData.BoxesOnly)
            {
                block.CoreData.GenData.RemoveIfOverlap = CoreData.GenData.RemoveIfOverlap;
                block.BlockCore.Virgin = true;
                block.BlockCore.Finalized = true;
                block.CoreData.GenData.AlwaysLandOn_Reluctantly = true;
            }

            CoreData.MyLevel.AddBlock(block);
            AddPlatform(block);
            block.CoreData.WakeUpRequirements = true;
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
                if (CoreData.Data.Position.X > CoreData.MyLevel.MainCamera.TR.X + Range.X ||
                    CoreData.Data.Position.X < CoreData.MyLevel.MainCamera.BL.X - Range.X ||
                    CoreData.Data.Position.Y > CoreData.MyLevel.MainCamera.TR.Y + Range.Y ||
                    CoreData.Data.Position.Y < CoreData.MyLevel.MainCamera.BL.Y - Range.Y)
                {
                    CoreData.SkippedPhsx = true;
                    return;
                }
            CoreData.SkippedPhsx = false;

            if (CoreMath.Modulo(CoreData.GetPhsxStep(), Delay) == Offset)
            {
                Emit(CoreData.MyLevel.CurPhsxStep);
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
            CoreData.Clone(A.CoreData);

            BlockEmitter EmitterA = A as BlockEmitter;
            Init(A.CoreData.Data.Position, A.MyLevel, EmitterA.MyBoxStyle);

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