using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class FlyingBlock : BlockBase
    {
        public override void TextDraw() { }

        public Vector2 Orbit;
        public Vector2 Radii;
        public int Period, Offset;

        public SimpleObject MyObject;

        public float MyAnimSpeed;

        public override void Interact(Bob bob) { }

        public override void MakeNew()
        {
            MyAnimSpeed = .36f;

            Core.Init();
            BlockCore.MyType = ObjectType.FlyingBlock;

            BlockCore.Layer = .35f;
            Core.DrawLayer = 5;
            Core.DrawLayer2 = 8;

            SetAnimation();

            Orbit = Vector2.Zero;
            
            //MyObject.Boxes[0].Animated = false;
        }

        public override void Release()
        {
            BlockCore.Release();
        }

        void SetAnimation()
        {
            MyObject.Read(0, 0);
            MyObject.Play = true;
            MyObject.Loop = true;
            MyObject.EnqueueAnimation(0, (float)MyLevel.Rnd.Rnd.NextDouble() * 1.85f, true);
            MyObject.DequeueTransfers();
            MyObject.Update();
        }

        public FlyingBlock(bool BoxesOnly)
        {
            MyObject = new SimpleObject(Prototypes.FlyingBlockObj, BoxesOnly);

            MyObject.Boxes[0].Animated = false;

            MyBox = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Init(Vector2 center, Vector2 size)
        {
            Active = true;

            MyBox.Initialize(center, size);

            Orbit = Core.StartData.Position = Core.Data.Position = center;

            Update();
        }

        public override void Hit(Bob bob) { }
        public override void LandedOn(Bob bob)
        {
        }
        public override void HitHeadOn(Bob bob) { } public override void SideHit(Bob bob) { } 

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            Active = true;

            BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();

            MyObject.PlayUpdate(MyLevel.Rnd.RndFloat(0, .75f));
        }
        
        public void AnimStep()
        {
            if (MyObject.DestinationAnim() == 0 && MyObject.Loop)
                MyObject.PlayUpdate(MyAnimSpeed);
        }

        public Vector2 GetPos()
        {
            double t = 2 * Math.PI * (Core.GetPhsxStep() + Offset) / (float)Period;
            Core.Data.Position = Tools.AngleToDir(t) * Radii + Orbit;
            return Core.Data.Position;
        }

        public override void PhsxStep()
        {
            GetPos();

            Vector2 PhsxCutoff = new Vector2(400);
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = new Vector2(-100, 400);
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                return;
            }
            Core.SkippedPhsx = false;

            if (!Core.BoxesOnly && Active && Core.Active) AnimStep();

            MyBox.Target.Center = Core.Data.Position;

            Update();

            MyBox.SetTarget(MyBox.Target.Center, MyBox.Current.Size);


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

            MyObject.Base.Origin -= MyObject.Boxes[0].Center() - MyBox.Current.Center;

            MyObject.Base.e1.X = 1;
            MyObject.Base.e2.Y = 1;
            MyObject.Update();           

            Vector2 CurSize = MyObject.Boxes[0].Size() / 2;
            Vector2 Scale = MyBox.Current.Size / CurSize;
            MyObject.Base.e1.X = Scale.X;
            MyObject.Base.e2.Y = Scale.Y;

            MyObject.Update();   
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

            Orbit += shift;
            Box.Move(shift);

            Update();
        }

        bool DrawSelf = true;
        public override void Draw()
        {
            if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
            {
                if (Core.Held)
                    DrawSelf = true;
                else
                {
                    DrawSelf = true;
                    if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 500))
                        DrawSelf = false;
                }
            }
            else
                if (!DrawSelf) return;

            if (DrawSelf)
            {
                if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
                {
                    Update();

                    if (Tools.DrawBoxes)
                        MyBox.Draw(Tools.QDrawer, Color.Olive, 15);
                }
            }

            if (Tools.DrawGraphics)
            {
                if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
                {
                    if (DrawSelf && !BlockCore.BoxesOnly)
                        MyObject.Draw(Tools.QDrawer, Tools.EffectWad, 0, 1);
                }
                else
                {
                    if (DrawSelf && !BlockCore.BoxesOnly)
                        MyObject.Draw(Tools.QDrawer, Tools.EffectWad, 2, 2);
                    
                    BlockCore.Draw();
                }
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            FlyingBlock BlockA = A as FlyingBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);

            Period = BlockA.Period;
            Offset = BlockA.Offset;
            Radii = BlockA.Radii;
            Orbit = BlockA.Orbit;
        }

        public override void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
        }
        public override void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public override void OnUsed() { }
public override void OnMarkedForDeletion() { }
public override void OnAttachedToBlock() { }
public override bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public override void Smash(Bob bob) { }
public override bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
