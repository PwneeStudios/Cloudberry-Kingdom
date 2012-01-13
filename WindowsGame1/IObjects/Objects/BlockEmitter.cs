using System.IO;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class BlockEmitter : IObject
    {
        public void TextDraw() { }
        public MovingPlatform.MoveType MyMoveType;
        public float Amp;

        List<MovingPlatform> Platforms = new List<MovingPlatform>();

        public void Release()
        {
            Core.Release();

            Platforms.Clear();
        }

        public bool AlwaysOn, Active;
        public Vector2 Range;

        public PhsxData EmitData;
        public int Delay, Offset;
        public Vector2 Size;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void OnUsed()
        {
            Platforms.ForEach(platform => platform.Core.GenData.Used = true);
        }

        public void OnMarkedForDeletion()
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
        
        public void MakeNew()
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
            //Range = new Vector2(3000, 3000);
            GiveCustomRange = false;

            /*
            if (Core.BoxesOnly) Core.GenData.OnUsed = OnUsed;
            */

            if (DoPreEmit)
                SetToPreEmit = true;
        }

        public BlockEmitter(bool BoxesOnly)
        {            
            CoreData = new ObjectData();
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

        public void Reset(bool BoxesOnly)
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
            block.Init(EmitData.Position, Size);

            if (GiveLayer) block.Core.DrawLayer = Core.DrawLayer;

            block.Core.Tag = Core.Tag;

            block.Core.Active = true;
            block.Range = Range;
            if (Core.MyLevel.Geometry == LevelGeometry.Up || Core.MyLevel.Geometry == LevelGeometry.Down)
                block.Range.X = 300;
            else
                block.Range.Y = 300;// 1800;
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
                //block.Core.GenData.AlwaysLandOn = true;
                block.Core.GenData.RemoveIfOverlap = Core.GenData.RemoveIfOverlap;
                block.BlockCore.Virgin = true;
                block.BlockCore.Finalized = true;
                block.Core.GenData.AlwaysLandOn_Reluctantly = true;
            }

            Core.MyLevel.AddBlock(block);
            AddPlatform(block);
        }

        public void AddPlatform(MovingPlatform platform)
        {
            Platforms.Add(platform);
        }

        public void RemovePlatform(MovingPlatform platform)
        {
            Platforms.Remove(platform);
        }

        public void PhsxStep()
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

            

            int Step = Core.GetPhsxStep();

            //if (Core.GetPhsxStep() % Delay == Offset)
            if (Tools.Modulo(Core.GetPhsxStep(), Delay) == Offset)
            {
                Emit(Core.MyLevel.CurPhsxStep);
            }
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            EmitData.Position += shift;
            Core.StartData.Position += shift;
        }

        public void PhsxStep2() { }
        public void Draw() { }
        public void Interact(Bob bob) { }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            BlockEmitter EmitterA = A as BlockEmitter;
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

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}