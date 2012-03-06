using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class LavaBlock : BlockBase, Block
    {
        public void TextDraw() { }

        public AABox MyBox;

        public QuadClass MyQuad;
        
        public AABox Box { get { return MyBox; } }

        public bool Active;
        public bool IsActive { get { return Active; } set { Active = value; } }

        public BlockData CoreData;
        public BlockData BlockCore { get { return CoreData; } }
        public ObjectData Core { get { return CoreData as BlockData; } }
        public void Interact(Bob bob) { }

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 9;
            CoreData.MyType = ObjectType.LavaBlock;

            MyQuad.EffectName = "Lava";
        }

        public void Release()
        {
            BlockCore.Release();
            Core.MyLevel = null;
            MyQuad = null;
            MyBox = null;
        }

        public LavaBlock(bool BoxesOnly)
        {
            CoreData = new BlockData();

            MyQuad = new QuadClass();

            MyBox = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        float u_offset = 0;
        Vector2 texture_size = new Vector2(1400, 1000);
        void SetUV()
        {
            float repeats = Size.X / texture_size.X;
            MyQuad.Quad.UVFromBounds(new Vector2(repeats + u_offset, 1), new Vector2(0 + u_offset, 0));
        }

        float Height(float x)
        {
            Box.CalcBounds();

            float s = (x - Box.BL.X) / (Box.TR.X - Box.BL.X);
            float repeats = Size.X / texture_size.X;
            float u = (1 - s) * (repeats + u_offset) + s * (0 + u_offset);
            u = (u + 1000) % 1;

            return -530 + 400 * (1 - 9.7f * Tools.TheGame.LavaHeight(u)) + Box.TR.Y;
        }

        public void Init(float top, float left, float right, float depth)
        {
            float width = right - left;
            Vector2 centerTop = new Vector2((left + right) / 2, top);
            centerTop.Y -= depth / 2;
            Init(centerTop, new Vector2(width, depth / 2));
        }

        Vector2 Size;
        public void Init(Vector2 center, Vector2 size)
        {
            Active = true;
            Size = size;

            MyBox.Initialize(center, size);

            CoreData.Layer = .35f;
            
            MyBox = new AABox(center, size);

            SetQuad(center, size);

            SetUV();

            Update();
        }

        void SetQuad(Vector2 center, Vector2 size)
        {
            MyQuad.Base.Origin = CoreData.Data.Position = CoreData.StartData.Position = center;

            MyQuad.Base.e1.X = size.X;
            MyQuad.Base.e2.Y = size.Y;
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

            BlockCore.StoodOn = false;

            MyBox.Current.Center = CoreData.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
            SetUV();
        }

        void CollisionCheck(Bob bob)
        {
            float h = Height(bob.Pos.X);
            
            if (bob.Box.BL.Y < h - 40)
                bob.Die(Bob.BobDeathType.Lava);
        }

        public void PhsxStep()
        {
            Active = Core.Active = true;
            if (!Core.Held)
            {
                if (!Core.MyLevel.MainCamera.OnScreen(MyBox.Current.BL, MyBox.Current.TR, 10))
                    Active = Core.Active = false;
            }

            if (Core.MyLevel.PlayMode == 0)
            {
                Active = Core.Active = false;
                foreach (Bob bob in Core.MyLevel.Bobs)
                    if (bob.CanDie)
                        CollisionCheck(bob);
            }

            // Update the block's apparent center according to attached objects
            BlockCore.UseCustomCenterAsParent = true;
            BlockCore.CustomCenterAsParent = Box.Target.Center;

            Update();

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);

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
            SetQuad(MyBox.Target.Center, MyBox.Target.Size);
            SetUV();

            CoreData.StartData.Position = MyBox.Current.Center;
        }

        public void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }
        
        public void Draw()
        {
            Update();

            if (Tools.DrawGraphics)
            {
                if (!CoreData.BoxesOnly)
                {
                    Tools.TheGame.DoLavaUpdate = true;

                    Tools.EffectWad.FindByName("Lava").effect.Parameters["EdgeColor"].SetValue(new Color(169, 18, 18).ToVector4());
                    Tools.EffectWad.FindByName("Lava").effect.Parameters["LavaColor"].SetValue(new Color(255, 0, 0).ToVector4());

                    MyQuad.Base.Origin = MyBox.Current.Center + new Vector2(0, 118);
                    MyQuad.Draw();
                    Tools.QDrawer.Flush();
                }

                BlockCore.Draw();
            }

            if (Tools.DrawBoxes)
            {
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);

                float x = Box.BL.X;
                while (x < Box.TR.X)
                {
                    Tools.QDrawer.DrawCircle(new Vector2(x, Height(x)), 20, Color.LimeGreen);
                    x += 200;
                }
            }
        }

        public bool PreDecision(Bob bob)
        {
            // If the computer gets close, move the lava block down
            if (bob.Box.Current.TR.X > Box.Current.BL.X &&
                bob.Box.Current.BL.X < Box.Current.TR.X)
            {
                bob.Core.MyLevel.PushLava(bob.Box.Target.BL.Y - 60, this);
            }

            return true;
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            LavaBlock BlockA = A as LavaBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);
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
//StubStubStubEnd7
    }
}
