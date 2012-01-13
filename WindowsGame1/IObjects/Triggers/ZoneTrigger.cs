using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public delegate void ZoneTriggerEvent(ZoneTrigger trig);
    public class ZoneTrigger : IObject
    {
        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
        }

        public ZoneTriggerEvent MyContainsEvent;

        AABox Box;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public virtual void MakeNew()
        {
            Core.Init();
            Core.MyType = ObjectType.ZoneTrigger;
            //Core.Show = false;
        }

        public ZoneTrigger()
        {
            CoreData = new ObjectData();
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

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public virtual void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Box.Move(shift);
        }

        public void Interact(Bob bob)
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

        public virtual void Clone(IObject A)
        {
            Core.Clone(A.Core);

            ZoneTrigger TriggerA = A as ZoneTrigger;
            Box.Initialize(TriggerA.Box.Current.Center, TriggerA.Box.Current.Size);
            MyContainsEvent = TriggerA.MyContainsEvent;
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
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
//StubStubStubEnd6
    }
}