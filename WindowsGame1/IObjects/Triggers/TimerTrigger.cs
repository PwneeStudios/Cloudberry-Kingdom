using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public delegate void TimerTriggerEvent(TimerTrigger trig);
    public class TimerTrigger : ObjectBase
    {
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
                    CollectSelf();
            }
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            TimerTrigger TriggerA = A as TimerTrigger;
            Length = TriggerA.Length;
            Repeat = TriggerA.Repeat;
            MyTrigger = TriggerA.MyTrigger;
        }
    }
}