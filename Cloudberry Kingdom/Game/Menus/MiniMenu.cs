using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class MiniMenu : Menu
    {
        public int ItemsToShow = 5;
        public int TopItem = 0;
        public int BottomItem
        {
            get
            {
                return TopItem + ItemsToShow - 1;
            }
            set
            {
                TopItem = value + 1 - ItemsToShow;
                if (TopItem < 0) TopItem = 0;
            }
        }

        public MiniMenu() { Init(); }

        public override void SelectItem(int Index)
        {
            base.SelectItem(Index);
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            if (SkipPhsx)
            {
                SkipPhsx = false;
                return;
            }

#if PC
			TopItem -= (int)System.Math.Sign(Tools.DeltaScroll);
			TopItem = CoreMath.Restrict(0, Items.Count - ItemsToShow, TopItem);
#endif
        }

        public Vector2 Shift = new Vector2(0, -80);
        public override void DrawText(int Layer)
        {
            MyCameraZoom = Tools.CurCamera.Zoom;
            if (!Show) return;

            CurDrawLayer = Layer;

            // Update index bounds
#if PC
			if (!ButtonCheck.MouseInUse)
#endif
			{
				if (CurIndex < TopItem) TopItem = CurIndex;
				if (CurIndex > BottomItem) BottomItem = CurIndex;
			}

#if PC
			foreach (var item in Items)
				item.MouseSelectable = false;
#endif

            // Draw item text
            for (int i = TopItem; i <= BottomItem; i++)
            {
                if (i >= Items.Count) break;

                var item = Items[i];

#if PC
				item.MouseSelectable = true;
#endif
                
                item.SetPos = Vector2.Zero;
                item.PosOffset = Pos + Shift * (i - TopItem);

                item.Draw(true, Tools.CurLevel.MainCamera, DrawItemAsSelected(item));
            }
        }
    }
}