using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class FlyingBlock : BlockBase, Block
    {
        public void TextDraw() { }

        public Vector2 Orbit;
        public Vector2 Radii;
        public int Period, Offset;


        public AABox MyBox;

        public SimpleObject MyObject;

        public float MyAnimSpeed;

        public AABox Box { get { return MyBox; } }

        public bool Active;
        public bool IsActive { get { return Core.Active; } set { Core.Active = value; } }

        public BlockData CoreData;
        public BlockData BlockCore { get { return CoreData; } }
        public ObjectData Core { get { return CoreData as BlockData; } }
        public void Interact(Bob bob) { }

        public void MakeNew()
        {
            MyAnimSpeed = .36f;

            Core.Init();
            CoreData.MyType = ObjectType.FlyingBlock;

            CoreData.Layer = .35f;
            Core.DrawLayer = 5;
            Core.DrawLayer2 = 8;

            SetAnimation();

            Orbit = Vector2.Zero;
            
            //MyObject.Boxes[0].Animated = false;
        }

        public void Release()
        {
            BlockCore.Release();
        }

        void SetAnimation()
        {
            MyObject.Read(0, 0);
            MyObject.Play = true;
            MyObject.Loop = true;
            MyObject.EnqueueAnimation(0, (float)Tools.Rnd.NextDouble() * 1.85f, true);
            MyObject.DequeueTransfers();
            MyObject.Update();
        }

        public FlyingBlock(bool BoxesOnly)
        {
            CoreData = new BlockData();

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

        public void Hit(Bob bob) { }
        public void LandedOn(Bob bob)
        {
        }
        public void HitHeadOn(Bob bob) { } public void SideHit(Bob bob) { } 

        public void Reset(bool BoxesOnly)
        {
            CoreData.BoxesOnly = BoxesOnly;

            Active = true;

            CoreData.Data = CoreData.StartData;

            MyBox.Current.Center = CoreData.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();

            MyObject.PlayUpdate(Tools.RndFloat(0, .75f));
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

        public void PhsxStep()
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


            CoreData.StoodOn = false;
        }

        public void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }


        public void Update()
        {
            if (CoreData.BoxesOnly) return;

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

        public void Extend(Side side, float pos)
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

            CoreData.StartData.Position = MyBox.Current.Center;
        }


        public void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Orbit += shift;
            Box.Move(shift);

            Update();
        }

        bool DrawSelf = true;
        public void Draw()
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
                    if (DrawSelf && !CoreData.BoxesOnly)
                        MyObject.Draw(Tools.QDrawer, Tools.EffectWad, 0, 1);
                }
                else
                {
                    if (DrawSelf && !CoreData.BoxesOnly)
                        MyObject.Draw(Tools.QDrawer, Tools.EffectWad, 2, 2);
                    
                    BlockCore.Draw();
                }
            }
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            FlyingBlock BlockA = A as FlyingBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);

            Period = BlockA.Period;
            Offset = BlockA.Offset;
            Radii = BlockA.Radii;
            Orbit = BlockA.Orbit;
        }

        public void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
public bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
