using System;
using System.IO;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Blocks
{
    public class MovingPlatform : Block
    {
        public void TextDraw() { }

        public enum MoveType { Normal, Sine };
        public MoveType MyMoveType;
        public float Amp;
        public int Offset;

        public Vector2 Range;

        public BlockEmitter Parent;

        public AABox MyBox;
        public QuadClass MyQuad;

        public AABox Box { get { return MyBox; } }
        
        bool Active;
        public bool IsActive { get { return Active; } set { Active = value; } }

        public BlockData CoreData;
        public BlockData BlockCore { get { return CoreData; } }
        public ObjectData Core { get { return CoreData as BlockData; } }

        BlockEmitter_Parameters Params
        {
            get { return (BlockEmitter_Parameters)Core.MyLevel.Style.FindParams(BlockEmitter_AutoGen.Instance); }
        }

        public bool PermissionToUse()
        {
            if (Params.MyStyle == BlockEmitter_Parameters.Style.Separated)
            {
                // Don't let the computer use another elevator too soon after using another one.
                if (Core.MyLevel.CurPhsxStep - Params.LastUsedTimeStamp > 5)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        public void LandedOn(Bob bob)
        {
            if (Core.MyLevel.PlayMode == 2)
            {
                Params.LastUsedTimeStamp = Core.MyLevel.CurPhsxStep;

                if (Params.MyStyle == BlockEmitter_Parameters.Style.Separated)
                    Core.GenData.EdgeJumpOnly = true;
            }
        }
        public void OnUsed()
        {
            Params.LastUsedTimeStamp = Core.MyLevel.CurPhsxStep;
            Parent.StampAsUsed(Core.MyLevel.CurPhsxStep);
        }

        public void OnMarkedForDeletion()
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

        public void MakeNew()
        {
            MyMoveType = MoveType.Normal;

            Core.Init();
            CoreData.MyType = ObjectType.MovingPlatform;
            Core.DrawLayer = 4;

            MyBox.TopOnly = true;

            /*
            // Remove from the emitter's list when this platform is deleted
            Core.GenData.OnMarkedForDeletion += OnMarkedForDeletion;

            // When this platform is used mark the parent as used
            if (Core.BoxesOnly)
                Core.GenData.OnUsed = OnUsed;
             */
        }

        public void Release()
        {
            BlockCore.Release();
            Core.MyLevel = null;
            Parent = null;
            MyQuad = null;
            MyBox = null;
        }

        public MovingPlatform(bool BoxesOnly)
        {
            CoreData = new BlockData();

            MyQuad = new QuadClass();
            MyBox = new AABox();

            Core.BoxesOnly = BoxesOnly;
            MakeNew();
        }

        public void Init(Vector2 center, Vector2 size)
        {
            if (Parent != null && Parent.Core.MyLevel.DefaultHeroType is BobPhsxSpaceship && Box.TopOnly)
            {
                Box.TopOnly = false;
            }

            Range = new Vector2(1800, 1800);
                        
            Active = true;

            CoreData.Layer = .3f;
            MyBox.Initialize(center, size);
            CoreData.Data.Position = CoreData.StartData.Position = center;

            if (!Core.BoxesOnly)
            {
                MyQuad.SetToDefault();
                MyQuad.TextureName = "Palette";
                MyQuad.Quad.SetColor(new Color(210, 210, 210));

                MyQuad.Base.e1.X = size.X;
                MyQuad.Base.e2.Y = size.Y;

                Update();
            }

            Core.RemoveOnReset = true;
            CoreData.HitHead = true;
        }


        public void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);
        }

        public void Hit(Bob bob) { }
        public void HitHeadOn(Bob bob) { } public void SideHit(Bob bob) { } 

        public void Reset(bool BoxesOnly)
        {
            CoreData.BoxesOnly = BoxesOnly;

            Active = true;

            CoreData.Data = CoreData.StartData;

            MyBox.Current.Center = CoreData.StartData.Position;
            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();
        }

        public void PhsxStep()
        {
            if (!Active) return;

            if (CoreData.Data.Position.X > CoreData.MyLevel.MainCamera.TR.X + Range.X ||
                CoreData.Data.Position.X < CoreData.MyLevel.MainCamera.BL.X - Range.X ||
                CoreData.Data.Position.Y > CoreData.MyLevel.MainCamera.TR.Y + Range.Y ||
                CoreData.Data.Position.Y < CoreData.MyLevel.MainCamera.BL.Y - Range.Y)
            {
                this.CollectSelf();

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
            MyBox.Target.Center = Core.StartData.Position + Step * CoreData.Data.Velocity;

            switch (MyMoveType)
            {
                case MoveType.Sine:
                    MyBox.Target.Center.X = Core.StartData.Position.X + Amp * (float)Math.Cos(2 * Math.PI * t);
                    break;

                case MoveType.Normal:
                    break;
            }

            //CoreData.Data.Velocity += CoreData.Data.Acceleration;
            CoreData.Data.Position = MyBox.Target.Center;

            if (!Core.BoxesOnly) Update();

            MyBox.SetTarget(MyBox.Target.Center, MyBox.Current.Size);
        }

        void Update()
        {
            MyQuad.Pos = MyBox.Target.Center;

            MyQuad.Update();
        }

        public void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }


        public void Draw()
        {
            if (!Active || (Parent != null && !Parent.Active)) return;

            Vector2 BL = MyBox.Current.BL;//MyQuad.BL();
            if (MyBox.Current.BL.X > CoreData.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > CoreData.MyLevel.MainCamera.TR.Y)
                return;
            Vector2 TR = MyBox.Current.TR;// MyQuad.TR();
            if (MyBox.Current.TR.X < CoreData.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < CoreData.MyLevel.MainCamera.BL.Y)
                return;

            if (Tools.DrawBoxes)
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);

            if (CoreData.BoxesOnly) return;

            if (Tools.DrawGraphics)
            {
                MyQuad.Draw();
            }
        }

        public void Extend(Side side, float pos) { }
        public void Interact(Bob bob) { }

        public void Clone(IObject A)
        {
            MovingPlatform BlockA = A as MovingPlatform;
            BlockCore.Clone(A.Core);

            Parent = BlockA.Parent;

            Amp = BlockA.Amp;
            Offset = BlockA.Offset;

            Range = BlockA.Range;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);

            Active = BlockA.Active;
        }

        public void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnAttachedToBlock() { }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}
