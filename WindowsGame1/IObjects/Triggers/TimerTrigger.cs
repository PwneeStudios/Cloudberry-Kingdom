using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public delegate void TimerTriggerEvent(TimerTrigger trig);
    public class TimerTrigger : ObjectBase
    {
        public override void TextDraw() { }
        public override void Release()
        {
            Core.Release();
        }

        public TimerTriggerEvent MyTrigger;

        public int Length;
        int Count;

        public bool Repeat;

        public override void MakeNew()
        {
        }

        public TimerTrigger()
        {
            MakeNew();
        }

        public void Init(int Length, bool Repeat)
        {
            Core.MyType = ObjectType.TimerTrigger;

            this.Length = Length;
            this.Repeat = Repeat;

            Count = 0;
        }

        public override void PhsxStep()
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

        public override void PhsxStep2()
        {
        }


        public void Update()
        {
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public override void Move(Vector2 shift)
        {
        }

        public override void Interact(Bob bob)
        {
        }

        public override void Draw()
        {
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            TimerTrigger TriggerA = A as TimerTrigger;
            Length = TriggerA.Length;
            Repeat = TriggerA.Repeat;
            MyTrigger = TriggerA.MyTrigger;
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