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
            PlayerManager.UploadPlayerLevels();

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
            CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.Arcade);

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
if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2667.914f, 913.8129f); _item.MyText.Scale = 1.424767f; _item.MySelectedText.Scale = 0.4747677f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2467.301f, 123.0817f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.189f, -88.36035f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2472.857f, -285.9135f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }

			MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

			EzText _t;
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(75.00003f, -741.6663f); _t.Scale = 0.72f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1016.667f, -677.7777f); _t.Scale = 0.9744995f; }
			_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(412.7824f, 694.2017f); _t.Scale = 0.6497964f; }
			_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(419.8857f, 501.3502f); _t.Scale = 0.6259654f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(922.2821f, 467.227f); _q.Size = new Vector2(281.8031f, 680.5267f); }
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }

			MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.9f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2467.301f, 123.0817f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.189f, -88.36035f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2472.857f, -285.9135f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }

			MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

			EzText _t;
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-540.2258f, -741.6663f); _t.Scale = 0.72f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1016.667f, -677.7777f); _t.Scale = 0.9744995f; }
			_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(345.3059f, 571.1564f); _t.Scale = 0.6407465f; }
			_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(-60.38756f, 402.1202f); _t.Scale = 0.5885759f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(723.8219f, 367.997f); _q.Size = new Vector2(248.9382f, 824.8477f); }
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }

			MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.9f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2467.301f, 123.0817f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.189f, -88.36035f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2472.857f, -285.9135f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }

			MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

			EzText _t;
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-294.1357f, -745.6355f); _t.Scale = 0.72f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1016.667f, -677.7777f); _t.Scale = 0.9744995f; }
			_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(432.6285f, 583.0642f); _t.Scale = 0.7405323f; }
			_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(42.81172f, 378.305f); _t.Scale = 0.606437f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(747.6375f, 356.0894f); _q.Size = new Vector2(274.7774f, 756.8558f); }
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }

			MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.9f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2467.301f, 123.0817f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.189f, -88.36035f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2472.857f, -285.9135f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }

			MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

			EzText _t;
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-317.9504f, -737.6971f); _t.Scale = 0.72f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1016.667f, -677.7777f); _t.Scale = 0.9744995f; }
			_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(182.569f, 622.7562f); _t.Scale = 0.8550834f; }
			_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(7.088623f, 354.4898f); _t.Scale = 0.6387068f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(771.4524f, 340.2126f); _q.Size = new Vector2(305.541f, 787.8156f); }
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }

			MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2687.76f, 874.1208f); _item.MyText.Scale = 1.644939f; _item.MySelectedText.Scale = 0.694939f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2467.301f, 123.0817f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.189f, -88.36035f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2472.857f, -285.9135f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }

			MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

			EzText _t;
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(51.18484f, -741.6663f); _t.Scale = 0.72f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1024.605f, -685.7161f); _t.Scale = 0.9744995f; }
			_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(269.8915f, 630.6946f); _t.Scale = 0.6558692f; }
			_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(280.9633f, 437.843f); _t.Scale = 0.5914334f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(823.0519f, 407.6891f); _q.Size = new Vector2(266.6555f, 639.9223f); }
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }

			MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.697927f; _item.MySelectedText.Scale = 0.7479278f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2467.301f, 123.0817f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.189f, -88.36035f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
			_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2472.857f, -285.9135f); _item.MyText.Scale = 0.95f; _item.MySelectedText.Scale = 0.95f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }

			MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

			EzText _t;
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(75.00003f, -741.6663f); _t.Scale = 0.72f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1016.667f, -677.7777f); _t.Scale = 0.9744995f; }
			_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(301.6447f, 7.530289f); _t.Scale = 0.6619421f; }
			_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(86.4729f, -193.2597f); _t.Scale = 0.6619265f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(672.2224f, -219.4444f); _q.Size = new Vector2(290.9718f, 690.6484f); }
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }

			MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
		}
    }
}