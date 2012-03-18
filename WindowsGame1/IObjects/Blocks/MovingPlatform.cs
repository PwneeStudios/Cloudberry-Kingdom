using System;
using System.IO;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Blocks
{
    public class MovingPlatform : BlockBase
    {
        public override void TextDraw() { }

        public enum MoveType { Normal, Sine };
        public MoveType MyMoveType;
        public float Amp;
        public int Offset;

        public Vector2 Range;

        public BlockEmitter Parent;

        public QuadClass MyQuad;

        BlockEmitter_Parameters Params
        {
            get { return (BlockEmitter_Parameters)Core.MyLevel.Style.FindParams(BlockEmitter_AutoGen.Instance); }
        }

        public override bool PermissionToUse()
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

        public override void LandedOn(Bob bob)
        {
            if (Core.MyLevel.PlayMode == 2)
            {
                Params.LastUsedTimeStamp = Core.MyLevel.CurPhsxStep;

                if (Params.MyStyle == BlockEmitter_Parameters.Style.Separated)
                    Core.GenData.EdgeJumpOnly = true;
            }
        }
        public override void OnUsed()
        {
            Params.LastUsedTimeStamp = Core.MyLevel.CurPhsxStep;
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

            /*
            // Remove from the emitter's list when this platform is deleted
            Core.GenData.OnMarkedForDeletion += OnMarkedForDeletion;

            // When this platform is used mark the parent as used
            if (Core.BoxesOnly)
                Core.GenData.OnUsed = OnUsed;
             */
        }

        public override void Release()
        {
            BlockCore.Release();
            Core.MyLevel = null;
            Parent = null;
            MyQuad = null;
            MyBox = null;
        }

        public MovingPlatform(bool BoxesOnly)
        {
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

            BlockCore.Layer = .3f;
            MyBox.Initialize(center, size);
            BlockCore.Data.Position = BlockCore.StartData.Position = center;

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
            BlockCore.HitHead = true;
        }


        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);
        }

        public override void Hit(Bob bob) { }
        public override void HitHeadOn(Bob bob) { } public override void SideHit(Bob bob) { } 

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

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

            if (!Core.BoxesOnly) Update();

            MyBox.SetTarget(MyBox.Target.Center, MyBox.Current.Size);
        }

        void Update()
        {
            MyQuad.Pos = MyBox.Target.Center;

            MyQuad.Update();
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
                MyQuad.Draw();
            }
        }

        public override void Extend(Side side, float pos) { }
        public override void Interact(Bob bob) { }

        public override void Clone(ObjectBase A)
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

        public override void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
        }
        public override void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public override void OnAttachedToBlock() { }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public override void Smash(Bob bob) { }
public override bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
