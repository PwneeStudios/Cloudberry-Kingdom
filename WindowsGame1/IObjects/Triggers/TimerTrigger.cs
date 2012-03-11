using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public delegate void TimerTriggerEvent(TimerTrigger trig);
    public class TimerTrigger : ObjectBase, IObject
    {
        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
        }

        public TimerTriggerEvent MyTrigger;

        public int Length;
        int Count;

        public bool Repeat;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void MakeNew()
        {
        }

        public TimerTrigger()
        {
            CoreData = new ObjectData();

            MakeNew();
        }

        public void Init(int Length, bool Repeat)
        {
            Core.MyType = ObjectType.TimerTrigger;

            this.Length = Length;
            this.Repeat = Repeat;

            Count = 0;
        }

        public void PhsxStep()
        {
            Count++;
            if (Count >= Length)
            {
                if (MyTrigger != null)
                    MyTrigger(this);

                if (Repeat)
                    Count = 0;
                else
                    this.CollectSelf();
            }
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
        }

        public void Interact(Bob bob)
        {
        }

        public void Draw()
        {
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            TimerTrigger TriggerA = A as TimerTrigger;
            Length = TriggerA.Length;
            Repeat = TriggerA.Repeat;
            MyTrigger = TriggerA.MyTrigger;
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