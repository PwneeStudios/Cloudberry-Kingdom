using Microsoft.Xna.Framework;
using Drawing;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public delegate void ButtonCallback();
    public class ButtonBlock : BlockBase, Block
    {
        public void TextDraw() { }

        SimpleQuad MyQuad;
        BasePoint Base;

        public int TypeIndex;
        public bool Correlate;

        public ButtonCallback HitCallback;

        public void Interact(Bob bob) { }

        public void MakeNew()
        {
        }

        public void Release()
        {
            Core.MyLevel = null;
            MyBox = null;
        }
        
        public ButtonBlock(ButtonBlock block)
        {
            Active = block.Active;
            CoreData = new BlockData();
            CoreData.MyType = block.BlockCore.MyType;

            MyBox = new AABox(block.Box.Current.Center, block.Box.Current.Size);

            BlockCore.Data = block.BlockCore.Data;
            CoreData.StartData = block.BlockCore.StartData;

            BlockCore.Layer = block.BlockCore.Layer;

            Base = block.Base;

            MyQuad.Init();
            MyQuad.MyEffect = block.MyQuad.MyEffect;
            MyQuad.MyTexture = block.MyQuad.MyTexture;

            Correlate = block.Correlate;
            TypeIndex = block.TypeIndex;
            Update();
        }

        public ButtonBlock(Vector2 Pos, Vector2 Size)
        {
            Active = true;
            CoreData = new BlockData();
            CoreData.MyType = ObjectType.Button;
                        

            Vector2 BoxSize = Size; BoxSize.Y = BoxSize.X * .6f;
            Vector2 BoxPos = Pos; BoxPos.Y -= BoxSize.X * .6f;
            MyBox = new AABox(BoxPos, BoxSize);
            
            BlockCore.Data.Position = BoxPos;
            CoreData.StartData.Position = BoxPos;

            BlockCore.Layer = 1;

            Base = new BasePoint();
            Base.e1 = new Vector2(Size.X, 0);
            Base.e2 = new Vector2(0, Size.Y);
            //Base.Origin = Pos + new Vector2(0, -12);

            MyQuad.Init();
            MyQuad.MyEffect = Tools.BasicEffect;
            MyQuad.MyTexture = Tools.TextureWad.FindByName("Button");

            Correlate = true;
            TypeIndex = 0;
            Update();
        }

        public void Update()
        {
            Color color = Globals.OnOffBlockColors[TypeIndex];
            if (Active) MyQuad.MyTexture = Tools.TextureWad.FindByName("Button");
            else MyQuad.MyTexture = Tools.TextureWad.FindByName("ButtonDown");
            MyQuad.SetColor(color);

            Base.Origin = Box.Current.Center + new Vector2(0, 24);
        }

        public void PhsxStep()
        {
            bool HoldActive = Active;

            if (Correlate) Active = !Globals.ColorSwitch[TypeIndex];
            else Active = Globals.ColorSwitch[TypeIndex];

            if (HoldActive != Active) Update();

            Box.SetTarget(BlockCore.Data.Position, Box.Current.Size);
        }

        public void PhsxStep2()
        {
            Box.SwapToCurrent();
        }

        public void Draw()
        {
            MyQuad.Update(ref Base);
            Tools.QDrawer.DrawQuad(MyQuad);

            if (Tools.DrawBoxes)
            {
                Box.Draw(Tools.QDrawer, Color.Olive, 15);
                Box.DrawT(Tools.QDrawer, Color.Olive, 15);
            }
        }

        public void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public Block Clone() { return new ButtonBlock(this); }
        public void Hit(Bob bob) { }
        public void SideHit(Bob bob) { }
        public void LandedOn(Bob bob)
        {
            if (HitCallback != null)
                HitCallback();

            Active = false;
            if (Correlate) Globals.ColorSwitch[TypeIndex] = true;
            else Globals.ColorSwitch[TypeIndex] = false;

            bob.Core.Data.Velocity.Y = 4;

            bob.MyPhsx.LandOnSomething(true);

            Update();
        }

        public void HitHeadOn(Bob bob)
        {
        }

        public void Extend(Side side, float pos) { }

        public void Reset(bool BoxesOnly)
        {
            CoreData.BoxesOnly = BoxesOnly;

            Active = true;

            CoreData.Data = CoreData.StartData;

            MyBox.Current.Center = CoreData.StartData.Position;
            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);
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
