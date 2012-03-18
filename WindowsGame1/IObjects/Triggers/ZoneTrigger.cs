using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public delegate void ZoneTriggerEvent(ZoneTrigger trig);
    public class ZoneTrigger : ObjectBase
    {
        public override void TextDraw() { }
        public override void Release()
        {
            Core.Release();
        }

        public ZoneTriggerEvent MyContainsEvent;

        AABox Box;

        public virtual void MakeNew()
        {
            Core.Init();
            Core.MyType = ObjectType.ZoneTrigger;
            //Core.Show = false;
        }

        public ZoneTrigger()
        {
            Box = new AABox();

            MakeNew();
        }

        public void Init(Vector2 center, Vector2 size)
        {
            Core.Data.Position = center;
            Box.Initialize(center, size);
        }

        public virtual void PhsxStep()
        {
        }

        public virtual void PhsxStep2()
        {
        }


        public void Update()
        {
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public virtual void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Box.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (MyContainsEvent == null) return;

            if (!Core.Active) return;
            bool Overlap = Phsx.BoxBoxOverlap(bob.Box, Box);
            if (Overlap)
                MyContainsEvent(this);
        }


        public virtual void Draw()
        {
            if (Tools.DrawBoxes && Core.Active)
                Box.Draw(Tools.QDrawer, Color.Teal, 30);
        }

        public virtual void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            ZoneTrigger TriggerA = A as ZoneTrigger;
            Box.Initialize(TriggerA.Box.Current.Center, TriggerA.Box.Current.Size);
            MyContainsEvent = TriggerA.MyContainsEvent;
        }

        public override void Write(BinaryWriter writer)
        {
            Core.Write(writer);
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