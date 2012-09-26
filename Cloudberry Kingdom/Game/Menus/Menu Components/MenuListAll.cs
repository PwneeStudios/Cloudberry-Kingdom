using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Drawing;

namespace CloudberryKingdom
{
    public class Brackets
    {
        QuadClass Br1, Br2;

        public Brackets()
        {
            float Size = 50;
            Br1 = new QuadClass("Edge2", Size, true);
            Br2 = new QuadClass("Edge2", Size, true);

            //float Size = 53;
            //Br1 = new QuadClass("Edge", Size, true);
            //Br2 = new QuadClass("Edge", Size, true);
        }

        public void Draw(Vector2 Center)
        {
            Br1.Degrees = 0;
            Br2.Degrees = 180;

            Br1.Pos = new Vector2(-100, 120) + Center;
            Br2.Pos = new Vector2(100, -120) + Center;

            Br1.Draw();
            Br2.Draw();
        }

        public void AddToDrawPile(DrawPile pile)
        {
            pile.Add(Br1);
            pile.Add(Br2);
        }
    }

    public class MenuListAll : MenuList
    {
        public MenuItem SelectedItem;

#if WINDOWS
        Vector2 ListPadding = new Vector2(65, 0);
        Vector2 TotalPadding = Vector2.Zero;
        MenuItem LastHitItem;
        public override bool HitTest(Vector2 pos, Vector2 padding)
        {
            TotalPadding = Padding + ListPadding;

            bool hit = false;
            foreach (var item in MyList)
            {
                if (item.HitTest(pos, padding))
                {
                    hit = true;
                    LastHitItem = item;
                }
            }

            return hit;
        }
#endif

        Brackets MyBrackets = new Brackets();
        public MenuListAll()
        {
            ExpandOnGo = false;
            ClickForNextItem = false;
        }

        public override void PhsxStep(bool Selected)
        {
#if WINDOWS
            if (!ButtonCheck.MouseInUse)
            {
                if (SelectedItem != null && CurMenuItem != SelectedItem)
                    SetSelectedItem(SelectedItem);
            }
#endif

            base.PhsxStep(Selected);

#if WINDOWS
            if (ButtonCheck.MouseInUse)
            {
                if (ButtonCheck.State(ControllerButtons.A, Control).Pressed &&
                            !ButtonCheck.KeyboardGo())
                    SelectedItem = LastHitItem;

                if (LastHitItem != null && CurMenuItem != LastHitItem)
                    SetSelectedItem(LastHitItem);
            }
#endif
        }

        public override void SetIndex(int NewIndex)
        {
            base.SetIndex(NewIndex);

#if WINDOWS
            LastHitItem = CurMenuItem;

            if (!ButtonCheck.MouseInUse)
                SelectedItem = CurMenuItem;
#else
            SelectedItem = CurMenuItem;
#endif
        }

        public float ShiftAmount = 250;
        public override void Draw(bool Text, Camera cam, bool Selected)
        {
            if (MyMenu.CurDrawLayer != MyDrawLayer || !Show)
                return;

            float Shift = 0;
            foreach (var item in MyList)
            {
                // The unselected text of the current menu item may not ever have been drawn,
                // so update its CameraZoom manually
                item.MyText.MyCameraZoom = MyCameraZoom;

                item.Icon.FancyPos.RelVal.X = Shift;

                item.MyMenu = MyMenu;
                
                //var icon = item.Icon as PictureIcon;
                //if (null != icon)
                //{
                //    if (item == CurMenuItem)
                //        icon.IconQuad.Quad.SetColor(Color.White);
                //    else
                //        icon.IconQuad.Quad.SetColor(CoreMath.GrayColor(.8f));
                //}

                item.Draw(Text, cam, Selected && item == CurMenuItem);

                Shift += ShiftAmount;
            }

            if (SelectedItem == null) SelectedItem = CurMenuItem;
            Vector2 pos = SelectedItem.Icon.FancyPos.Update();
            MyBrackets.Draw(pos);

            if (DrawBase(Text, cam, Selected)) return;
        }
    }
}