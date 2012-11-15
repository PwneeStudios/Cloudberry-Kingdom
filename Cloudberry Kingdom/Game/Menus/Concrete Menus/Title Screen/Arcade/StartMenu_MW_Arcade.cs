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

        public override void Go(MenuItem item)
        {
            MyArcadeItem = item as ArcadeItem;
            if (MyArcadeItem.Locked) return;

            if (MyArcadeItem.MyChallenge == Challenge_Escalation.Instance ||
                MyArcadeItem.MyChallenge == Challenge_TimeCrisis.Instance)
            {
                Call(new StartMenu_MW_HeroSelect(Title, this, MyArcadeItem));
            }
            else
            {
                Challenge.ChosenHero = null;
                StartLevelMenu levelmenu = new StartLevelMenu(MyArcadeItem.MyChallenge.TopLevel());

                levelmenu.MyMenu.SelectItem(StartLevelMenu.PreviousMenuIndex);
                levelmenu.StartFunc = new StartFuncProxy(this);
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
            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            SetPos();
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.960415f; _item.MySelectedText.Scale = 1f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1)); }
            _item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 320.413f); _item.MyText.Scale = 1f; _item.MySelectedText.Scale = 1f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1)); }
            _item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2467.301f, 117.0817f); _item.MyText.Scale = 1f; _item.MySelectedText.Scale = 1f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1)); }
            _item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.189f, -97.36035f); _item.MyText.Scale = 1f; _item.MySelectedText.Scale = 1f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1)); }
            _item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2472.857f, -297.9135f); _item.MyText.Scale = 1f; _item.MySelectedText.Scale = 1f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1)); }

            MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }
    }
}