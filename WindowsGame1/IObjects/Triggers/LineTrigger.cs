using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public enum TriggerCode { None, ScoreBoardTrigger };

    public delegate void LineTriggerEvent(LineTrigger trig);
    public class LineTrigger : ObjectBase
    {
        public LineTriggerEvent MyTriggerEvent;

        public TriggerCode MyCode;

        public AABox Box;

        public bool OverlapTriggers; // If true the trigger is set off if the box overlaps with a bob

        public override void MakeNew()
        {
            MyCode = TriggerCode.None;

            OverlapTriggers = false;

            Core.Init();
            Core.MyType = ObjectType.LineTrigger;
        }

        public LineTrigger()
        {
            Box = new AABox();

            MakeNew();
        }

        public void Init(Vector2 center, Vector2 size)
        {            
            Box.Initialize(center, size);
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            Box.Move(shift);
        }

        public override void Interact(Bob bob)
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

        public override void Draw()
        {
            if (Tools.DrawBoxes)
                Box.Draw(Tools.QDrawer, Color.Teal, 30);
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            LineTrigger TriggerA = A as LineTrigger;
            Box.Initialize(TriggerA.Box.Current.Center, TriggerA.Box.Current.Size);
            MyTriggerEvent = TriggerA.MyTriggerEvent;

            OverlapTriggers = TriggerA.OverlapTriggers;

            MyCode = TriggerA.MyCode;
        }
    }
}