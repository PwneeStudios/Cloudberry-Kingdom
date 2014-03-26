using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_Clouds_Arcade : ArcadeMenu
    {
        public TitleGameData_Clouds Title;
        public StartMenu_Clouds_Arcade(TitleGameData_Clouds Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            PlayerManager.UploadPlayerLevels();

            Title.BackPanel.SetState(TitleBackgroundState.Scene_Arcade);
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

            if (MyArcadeItem.MyChallenge == Challenge_Freeplay.Instance)
            {
                SkipCallSound = true;
                Call(new StartMenu_Clouds_CustomLevel(Title));
            }
            else
            {
                if (MyArcadeItem.MyChallenge == Challenge_Escalation.Instance ||
                    MyArcadeItem.MyChallenge == Challenge_TimeCrisis.Instance)
                {
                    Call(new StartMenu_Clouds_HeroSelect(Title, this, MyArcadeItem));
                }
                else
                {
                    Challenge.ChosenHero = null;

                    int TopLevelForHero = MyArcadeItem.MyChallenge.CalcTopGameLevel(null);

                    StartLevelMenu levelmenu = new StartLevelMenu(TopLevelForHero);

                    levelmenu.MyMenu.SelectItem(StartLevelMenu.PreviousMenuIndex);
                    levelmenu.StartFunc = StartFunc;
                    levelmenu.ReturnFunc = null;

                    Call(levelmenu);
                }
            }

            Hide();
        }

        public override void OnAdd()
        {
            CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.Arcade);

            base.OnAdd();
        }

		ClickableBack Back;

		protected override void MyPhsxStep()
		{
			base.MyPhsxStep();

#if PC_VERSION
			if (!Active) return;

			// Update the back button and the scroll bar
			if (Back.UpdateBack(MyCameraZoom))
			{
				MenuReturnToCaller(MyMenu);
				return;
			}
#endif
		}

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            SetPos();

#if PC_VERSION
			// Back button
			Back = new ClickableBack(MyPile, true, true);
#endif
        }

        protected override void SetPos()
        {
			base.SetPos();
		}
    }
}