using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class ZoneTrigger : ObjectBase
    {
        public Lambda_1<ZoneTrigger> MyContainsEvent;

        AABox Box;

        public override void MakeNew()
        {
            Core.Init();
            Core.MyType = ObjectType.ZoneTrigger;
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

        public void Update()
        {
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
            if (MyContainsEvent == null) return;

            if (!Core.Active) return;
            bool Overlap = Phsx.BoxBoxOverlap(bob.Box, Box);
            if (Overlap)
                MyContainsEvent.Apply(this);
        }


        public override void Draw()
        {
            //if (Tools.DrawBoxes && Core.Active)
            //    Box.Draw(Tools.QDrawer, Color.Teal, 30);
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            ZoneTrigger TriggerA = A as ZoneTrigger;
            Box.Initialize(TriggerA.Box.Current.Center, TriggerA.Box.Current.Size);
            MyContainsEvent = TriggerA.MyContainsEvent;
        }
    }
}