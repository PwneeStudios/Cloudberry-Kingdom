using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_Arcade : ArcadeMenu
    {
        public TitleGameData_MW Title;
        public StartMenu_MW_Arcade(TitleGameData_MW Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Kobbler);
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;

            StartMenu.SetItemProperties_Green(item, false);

            item.MyText.Scale = item.MySelectedText.Scale = 1;
        }

        protected override void Go(MenuItem item)
        {
            MyArcadeItem = item as ArcadeItem;
            if (MyArcadeItem.IsLocked()) return;

            if (MyArcadeItem.MyChallenge == Challenge_Escalation.Instance ||
                MyArcadeItem.MyChallenge == Challenge_TimeCrisis.Instance)
            {
                Call(new StartMenu_MW_HeroSelect(Title, this, MyArcadeItem));
            }
            else
            {
                Challenge.ChosenHero = null;
                
                //int TopLevelForHero = MyArcadeItem.MyChallenge.TopLevel();
                int TopLevelForHero = MyArcadeItem.MyChallenge.CalcTopGameLevel(null);

                StartLevelMenu levelmenu = new StartLevelMenu(TopLevelForHero);

                levelmenu.MyMenu.SelectItem(StartLevelMenu.PreviousMenuIndex);
                levelmenu.StartFunc = StartFunc;
                levelmenu.ReturnFunc = null;

                Call(levelmenu);
            }

            Hide();
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            SetPos();
        }

        void SetPos()
        {
			MenuItem _item;
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.9f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2467.301f, 123.0817f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.189f, -88.36035f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2472.857f, -285.9135f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }

			MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

			EzText _t;
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(75.00003f, -741.6663f); _t.Scale = 0.72f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1016.667f, -677.7777f); _t.Scale = 0.9744995f; }
			_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(55.55463f, 47.22229f); _t.Scale = 0.8550834f; }
			_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(233.3332f, -169.4445f); _t.Scale = 0.8706668f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(672.2224f, -219.4444f); _q.Size = new Vector2(302.8795f, 689.2195f); }
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }

			MyPile.Pos = new Vector2(83.33417f, 130.9524f);
		}
    }
}