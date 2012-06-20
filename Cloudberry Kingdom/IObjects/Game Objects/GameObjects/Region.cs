using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Region : GUI_Panel
    {
        public Region(Vector2 pos, Vector2 size)
        {
            Init(pos, size);
        }

        AABox Box;
        public void Init(Vector2 pos, Vector2 size)
        {
            FixedToCamera = false;
            Core.DrawLayer = 9;

            MyPile = new DrawPile();

            //QuadClass Backdrop = new QuadClass("White");
            QuadClass Backdrop = new QuadClass("dungeon_smoke_thick");
            MyPile.Add(Backdrop);

            //Backdrop.Quad.SetColor(Color.Black);
            Backdrop.Size = size;

            //MyPile.Pos = pos + new Vector2(0, 1500);
            float shift = -1250;
            MyPile.Pos = pos + new Vector2(0, shift);

            Box = new AABox(pos + new Vector2(0, shift - 2430), size);

            Active = true;
            Hid = false;
        }

        public Door AttachedDoor;
        protected override void MyPhsxStep()
        {
            if (AttemptToReAdd()) return;

            base.MyPhsxStep();

            if (!Active) return;

            foreach (Bob bob in Core.MyLevel.Bobs)
                if (bob.CanDie && Phsx.BoxBoxOverlap(bob.Box, Box))
                    if (AttachedDoor != null)
                    {
                        Active = false;
                        AttachedDoor.HaveBobUseDoor(bob);
                    }
        }

        public override bool OnScreen()
        {
            //return base.OnScreen();
            return true;
        }

        protected override void MyDraw()
        {
            if (AttemptToReAdd()) return;

            base.MyDraw();
        }
    }
}