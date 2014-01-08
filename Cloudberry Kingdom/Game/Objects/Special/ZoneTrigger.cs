using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public delegate void ZoneTriggerEvent(ZoneTrigger trig);
    public class ZoneTrigger : ObjectBase
    {
        public ZoneTriggerEvent MyContainsEvent;

        public AABox Box;

        public override void MakeNew()
        {
            CoreData.Init();
            CoreData.MyType = ObjectType.ZoneTrigger;
            //Core.Show = false;
        }

        public ZoneTrigger()
        {
            Box = new AABox();

            MakeNew();
        }

        public void Init(Vector2 center, Vector2 size)
        {
            CoreData.Data.Position = center;
            Box.Initialize(center, size);
        }

        public void Update()
        {
        }

        public override void Reset(bool BoxesOnly)
        {
            CoreData.Active = true;
        }

        public override void Move(Vector2 shift)
        {
            CoreData.Data.Position += shift;
            Box.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (MyContainsEvent == null) return;

            if (!CoreData.Active) return;
            bool Overlap = Phsx.BoxBoxOverlap(bob.Box, Box);
            if (Overlap)
                MyContainsEvent(this);
        }


        public override void Draw()
        {
            //if (Tools.DrawBoxes && Core.Active)
            //    Box.Draw(Tools.QDrawer, Color.Teal, 30);
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            ZoneTrigger TriggerA = A as ZoneTrigger;
            Box.Initialize(TriggerA.Box.Current.Center, TriggerA.Box.Current.Size);
            MyContainsEvent = TriggerA.MyContainsEvent;
        }
    }
}