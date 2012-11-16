using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class MakeDarkBottom : MakeFinalDoorVertical
    {
        public MakeDarkBottom(Level level) : base(level) { }

        public override void Phase3()
        {
            base.Phase3();

            float MoveAll = 1450;

            // Add dark bottom region
            Region region = new Region(MadeDoor.Pos + new Vector2(0, 100 - MoveAll), new Vector2(5000, 2000));
            region.AttachedDoor = MadeDoor;
            MyLevel.AddObject(region);

            float Amount = 560 + MoveAll;// 900;
            MyLevel.FinalCamZone.End.Y -= Amount;
            MyLevel.FinalDoor.Move(new Vector2(0, -Amount - 900));
        }
    }
}
