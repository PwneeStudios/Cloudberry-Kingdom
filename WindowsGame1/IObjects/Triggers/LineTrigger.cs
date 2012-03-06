using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public enum TriggerCode { None, ScoreBoardTrigger };

    public delegate void LineTriggerEvent(LineTrigger trig);
    public class LineTrigger : IObject
    {
        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
        }

        public LineTriggerEvent MyTriggerEvent;

        public TriggerCode MyCode;

        public AABox Box;

        public bool OverlapTriggers; // If true the trigger is set off if the box overlaps with a bob

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void MakeNew()
        {
            MyCode = TriggerCode.None;

            OverlapTriggers = false;

            Core.Init();
            Core.MyType = ObjectType.LineTrigger;
        }

        public LineTrigger()
        {
            CoreData = new ObjectData();
            Box = new AABox();

            MakeNew();
        }

        public void Init(Vector2 center, Vector2 size)
        {            
            Box.Initialize(center, size);
        }

        public void PhsxStep()
        {
        }

        public void PhsxStep2()
        {
        }


        public void Update()
        {
        }

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            Box.Move(shift);
        }

        public void Interact(Bob bob)
        {
            if (MyTriggerEvent == null) return;

            if (!Core.Active) return;

            if (OverlapTriggers)
            {
                if (Phsx.BoxBoxOverlap(bob.Box, Box))
                    MyTriggerEvent(this);
            }
            else
            {
                ColType col = Phsx.CollisionTest(bob.Box, Box);
                if (col != ColType.NoCol)
                    MyTriggerEvent(this);
            }
        }


        public void Draw()
        {
            if (Tools.DrawBoxes)
                Box.Draw(Tools.QDrawer, Color.Teal, 30);
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            LineTrigger TriggerA = A as LineTrigger;
            Box.Initialize(TriggerA.Box.Current.Center, TriggerA.Box.Current.Size);
            MyTriggerEvent = TriggerA.MyTriggerEvent;

            OverlapTriggers = TriggerA.OverlapTriggers;

            MyCode = TriggerA.MyCode;
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
public bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}