using Microsoft.Xna.Framework;
using Drawing;
using System.IO;
using System;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public class SwitchBlock : BlockBase, Block
    {
        public void TextDraw() { }

        public AABox MyBox;
        public AABox Box { get { return MyBox; } }

        bool Active;
        public bool IsActive { get { return Active; } set { Active = value; } }

        public BlockData CoreData;
        public BlockData BlockCore { get { return CoreData; } }

        QuadClass MyQuad;

        public ObjectData Core { get { return CoreData as BlockData; } }
        public void Interact(Bob bob) { }

        public void MakeNew()
        {
            BlockCore.Init();
            CoreData.MyType = ObjectType.OnOffBlock;

            Core.EditHoldable = false;

            VibrateIntensity = 0;
            Offset = Vector2.Zero;
        }

        public void Release()
        {
            Core.MyLevel = null;
            MyBox = null;
        }

        public Action OnActivate;

        public SwitchBlock(Vector2 Pos)
        {
            Active = true;
            CoreData = new BlockData();

            Vector2 Size = new Vector2(100);
            MyBox = new AABox(Pos, Size);
            MyQuad = new QuadClass("difficultybox1", Size.X * 1.05f);

            MakeNew();

            Core.StartData.Position = Core.Data.Position = Pos;

            BlockCore.Layer = .9f;

            Update();
        }

        public void Update()
        {
            MyQuad.Pos = Box.Current.Center;
        }

        public void PhsxStep()
        {
            CountSinceActivation++;

            bool HoldActive = Active;
            Core.Show = true;

            Vibrate();

            Update();

            Box.SetTarget(BlockCore.Data.Position + Offset, Box.Current.Size);
        }

        Vector2 Offset;
        float VibrateIntensity;
        void Vibrate()
        {
            int step = Core.MyLevel.GetPhsxStep();
            if (step % 2 == 0)
                Offset = Vector2.Zero;

            // Update the block's apparent center according to attached objects
            BlockCore.UseCustomCenterAsParent = true;
            BlockCore.CustomCenterAsParent = Box.Target.Center + Offset;

            if (VibrateIntensity > 0)
            {
                if (Core.MyLevel.GetPhsxStep() % 2 == 0)
                    Offset = VibrateIntensity * new Vector2(Tools.Rnd.Next(-10, 10), Tools.Rnd.Next(-10, 10));

                VibrateIntensity -= .0585f;
            }
        }

        public void PhsxStep2()
        {
            Box.SwapToCurrent();
        }

        public void Draw()
        {
            MyQuad.Draw();

            if (Tools.DrawBoxes)
                Box.Draw(Tools.QDrawer, Color.Olive, 15);
        }

        public void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public void Hit(Bob bob) { }
        public void LandedOn(Bob bob)
        {
        }

        public int DelayToActive = 40;
        int CountSinceActivation;
        public void HitHeadOn(Bob bob)
        {
            VibrateIntensity = 1.2f;

            if (OnActivate != null && CountSinceActivation > DelayToActive) OnActivate();

            CountSinceActivation = 0;
        }
        public void SideHit(Bob bob) { }

        public void Extend(Side side, float pos) { }

        public void Reset(bool BoxesOnly)
        {
            CountSinceActivation = 0;

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
