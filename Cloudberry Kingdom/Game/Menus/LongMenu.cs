using System;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class LongMenu : Menu
    {
        public float Offset = 0;
        public float OffsetStep = 50;

        public LongMenu() { Init(); }

        public override void SelectItem(int Index)
        {
            base.SelectItem(Index);
        }

        public float MaxBottomSpace = 500;
        public override void PhsxStep()
        {
            base.PhsxStep();

            if (SkipPhsx)
            {
                SkipPhsx = false;
                return;
            }

#if PC_VERSION
            if (!ButtonCheck.MouseInUse) Scroll();
#else
            Scroll();
#endif
        }

        public float Height()
        {
            foreach (MenuItem item in Items) item.UpdatePos();

            int HoldIndex = CurIndex;
            Vector2 HoldPos = FancyPos.RelVal;

            CurIndex = Items.Count - 1;
            SuperScroll();

            float height = FancyPos.RelVal.Y;

            CurIndex = HoldIndex;
            FancyPos.RelVal = HoldPos;

            return height;
        }

        void SuperScroll() { for (int i = 0; i < 120+120; i++) Scroll(); }

        public void FastScroll()
        {
            for (int i = 0; i < 5; i++) Scroll();
        }

        public void Scroll()
        {
            //// Lock unaffected
            //foreach (MenuItem item in Items)
            //    if (item.UnaffectedByScroll)
            //        item.Selectable = false;

            //if (Items[CurIndex].UnaffectedByScroll) return;
            MenuItem LastItem = Items[0];
            foreach (var item in Items)
                if (!item.UnaffectedByScroll && item.Selectable)
                    LastItem = item;
            //LastItem = Items[Items.Count - 1];


            // Scroll menu as needed
			float min = Math.Min(Items[CurIndex].MyText.Pos.Y, CurIndex > 0 ? Items[CurIndex - 1].MyText.Pos.Y : 100000);
            if (min < Tools.CurLevel.MainCamera.Pos.Y + 300)
            {
                FancyPos.RelValY += OffsetStep;
                if (LastItem.Pos.Y + FancyPos.RelVal.Y > -Tools.CurCamera.GetHeight() / 2 + MaxBottomSpace)
                    FancyPos.RelValY = -LastItem.Pos.Y - Tools.CurCamera.GetHeight() / 2 + MaxBottomSpace;
            }
            if (min > Tools.CurLevel.MainCamera.Pos.Y - 300)
                FancyPos.RelValY -= OffsetStep;

            if (FancyPos.RelVal.Y < 0) FancyPos.RelValY = 0;

            //// Unlock unaffected
            //foreach (MenuItem item in Items)
            //    if (item.UnaffectedByScroll)
            //        item.Selectable = true;
        }
    }
}