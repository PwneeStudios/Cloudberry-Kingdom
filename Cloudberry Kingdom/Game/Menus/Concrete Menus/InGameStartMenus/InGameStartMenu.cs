using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Stats;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class InGameStartMenu : CkBaseMenu
    {
        public static int MAX_SEED_STRINGS = 50;

        public static bool PreventMenu = false;

        bool CenterItems;
        public InGameStartMenu(int Control) : base(false)
        {
			// Test Remove Me menu
			//PlayerManager.Players[1].Exists = true;
			//PlayerManager.Players[2].Exists = true;
			//PlayerManager.Players[3].Exists = true;

            CenterItems = false;

            EnableBounce();

            this.Control = Control;

            Constructor();
        }

        public static GameObject MakeListener()
        {
            return MakeListener_Base(listener => 
                new InGameStartMenu(listener.TriggeringPlayerIndex));
        }

        public static GameObject MakeListener_Base(Func<Listener, GUI_Panel> Make)
        {
            Listener listener = new Listener();
            listener.MyButton = ControllerButtons.Start;

            listener.Tags += GameObject.Tag.RemoveOnLevelFinish;

            listener.MyAction = () =>
            {
                PlayerData Player = null;
                if (listener.TriggeringPlayerIndex >= 0)
                    Player = PlayerManager.Get(listener.TriggeringPlayerIndex);

                bool PlayerExists = false;
                if (Player != null && Player.Exists) PlayerExists = true;

                if (!PlayerExists && PlayerManager.NumExistingPlayers() == 0)
                    PlayerExists = true;

                //if (!PreventMenu)
                if (!PreventMenu && PlayerExists)
                    listener.Call(Make(listener));
            };

            return listener;
        }

        public override void OnAdd()
        {
            base.OnAdd();

            MyMenu.Control = Control;
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Scale = FontScale * 1.45f;
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();

            if (MyMenu.CurItem == RemoveMe && VerifyRemoveMenu.YesChosen)
                ReturnToCaller(false);
		    else
		    {
			    if ( ( !Tools.CurLevel.CanLoadLevels && !Tools.CurLevel.CanSaveLevel )
				    || ( PlayerManager.Players[0] != null && PlayerManager.Players[0].MySavedSeeds.SeedStrings.Count >= MAX_SEED_STRINGS ) )
			    {
				    var item = MyMenu.FindItemByName( "SaveLoadSeed" );
                    if (item != null)
                    {
                        item.Selectable = false;
                        item.GrayOutOnUnselectable = true;
                        item.GrayOut();
                        MyMenu.SelectItem(0);
                    }
			    }
		    }
        }

        MenuItem RemoveMe;
        public override void Init()
        {
            base.Init();

            PauseGame = true;

            CallDelay = 15;

            FontScale = .775f;

            MyPile = new DrawPile();

            this.CallDelay = 5;
            this.SlideLength = 14;
            this.SelectedItemShift = new Vector2(0, 0);

            MakeDarkBack();

            // Make the backdrop
            QuadClass backdrop;
			if (UseSimpleBackdrop)
			{
				backdrop = new QuadClass("Arcade_BoxLeft", 1500, true);
				backdrop.Name = "Backdrop";
			}
			else
			{
				backdrop = new QuadClass("Backplate_1080x840", 1500, true);
				backdrop.Name = "Backdrop";
			}

            MyPile.Add(backdrop);

			if (!UseSimpleBackdrop)
			{
				EpilepsySafe(.9f);
			}
			backdrop.Pos = new Vector2(-975.6945f, 54.86111f);

            // Make the menu
            MyMenu = new Menu(false);

            MyMenu.OnB = null;

            MenuItem item;

            ItemPos = new Vector2(-1560.333f, 600f);
            PosAdd = new Vector2(0, -270);

            bool RemoveMeOption = false;
            if (PlayerManager.GetNumPlayers() > 1 && Control >= 0)
            {
                RemoveMeOption = true;
                PosAdd.Y += 44;
            }

            // Resume
            item = new MenuItem(new EzText(
#if XBOX
				Localization.Words.ResumeGame
#else	
				Localization.Words.Resume
#endif
				, ItemFont, CenterItems));

            item.Name = "Resume";
            item.Go = Cast.ToItem(ReturnToCaller);
            item.MyText.Scale *= 1.1f;
            item.MySelectedText.Scale *= 1.1f;
            AddItem(item);
            item.SelectSound = null;
            
            // Statistics
            item = new MenuItem(new EzText(Localization.Words.Statistics, ItemFont, CenterItems));
            item.Name = "Stats";
            item.Go = Cast.ToItem(GoStats);
            AddItem(item);

            // SaveLoadSeed
            Localization.Words word = Tools.CurLevel.CanLoadLevels ? Localization.Words.SaveLoad : Localization.Words.SaveSeed;
            item = new MenuItem(new EzText(word, ItemFont, CenterItems));
            item.Name = "SaveLoadSeed";
            item.Go = Cast.ToItem(GoSaveLoad);

		    if ( ( !Tools.CurLevel.CanLoadLevels && !Tools.CurLevel.CanSaveLevel )
			    || ( PlayerManager.Players[0] != null && PlayerManager.Players[0].MySavedSeeds.SeedStrings.Count >= MAX_SEED_STRINGS ) )
            //if (!Tools.CurLevel.CanLoadLevels && !Tools.CurLevel.CanSaveLevel)
            {
                item.Selectable = false;
                item.GrayOutOnUnselectable = true;
                item.GrayOut();
            }
            AddItem(item);

            // Options
            item = new MenuItem(new EzText(Localization.Words.Options, ItemFont, CenterItems));
            item.Name = "Options";
            item.Go = Cast.ToItem(GoOptions);
            AddItem(item);

            //// Controls
            //item = new MenuItem(new EzText(Localization.Words.Controls, ItemFont, CenterItems));
            //item.Name = "Controls";
            //item.Go = Cast.ToItem(GoControls);
            //AddItem(item);

            // Remove player
            if (RemoveMeOption)
            {
                item = new MenuItem(new EzText(Localization.Words.RemoveMe, ItemFont, CenterItems));
                item.Name = "Remove";
                item.Go = Cast.ToItem(GoRemove);
                AddItem(item);
                RemoveMe = item;
            }

            // Exit level
            MakeExitItem();

            // Button interactions
            MyMenu.OnStart = MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Shift everything
            EnsureFancy();
            Shift(new Vector2(1045.139f, -10.41669f));

            if (RemoveMeOption)
                SetPos_WithRemoveMe();
            else
                SetPos();

            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        public override bool MenuReturnToCaller(Menu menu)
        {
			//if (Active)
			//{
			//    MyLevel.ReplayPaused = true;

			//    HoldLevel = MyLevel;
			//    MyGame.WaitThenDo(17, Unpause);
			//}

            return base.MenuReturnToCaller(menu);
        }

        Level HoldLevel;
        void Unpause()
        {
            if (HoldLevel != null) HoldLevel.ReplayPaused = false;
            HoldLevel = null;
        }

		protected override void ReleaseBody()
		{
			base.ReleaseBody();

			HoldLevel = null;
		}

        private void GoRemove()
        {
            VerifyRemoveMenu verify = new VerifyRemoveMenu(Control);
            Call(verify);

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        private void GoControls()
        {
            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                MyGame.WaitThenDo(4, () =>
                {
                    Hide(PresetPos.Left, 40);
                    PauseGame = true;
                });
            }

            ControlScreen screen = new ControlScreen(Control);
            Call(screen, 22);
        }

        private void GoOptions()
        {
            Call(new SoundMenu(Control, false), 0);

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        private void GoSaveLoad()
        {
            if (CloudberryKingdomGame.IsDemo)
            {
                Call(new UpSellMenu(Localization.Words.UpSell_SaveLoad, MenuItem.ActivatingPlayer), 0);
            }
            else
            {
#if XBOX
				PlayerData player = MenuItem.GetActivatingPlayerData();

                if (!CloudberryKingdomGame.CanSave(player.MyPlayerIndex))
                {
                    CloudberryKingdomGame.ShowError_CanNotSaveNoDevice();
                    return;
                }
#endif

                // If this isn't a PC, and we can't load seeds right now, then go directly to the SaveAs menu.
#if !PC_VERSION
                if (!MyLevel.CanLoadLevels)
                {
                    SaveLoadSeedMenu.MakeSave(this, player)(null);
                    //Call(new SaveSeedAs(Control, MenuItem.GetActivatingPlayerData()), 0);
                    return;
                }
                else
#endif
                {
#if PC_VERSION
					Call(new SaveLoadSeedMenu(Control, MyLevel.CanLoadLevels, MyLevel.CanSaveLevel), 0);
#else
                    Call(new SaveLoadSeedMenu(Control, MyLevel.CanLoadLevels, MyLevel.CanSaveLevel), 0);
#endif
                }
            }

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        private void GoStats()
        {
			Call(new StatsMenu(StatGroup.Lifetime), UseBounce ? 0 : 19 );

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        void SetPos()
        {
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1496.444f, -252.7777f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(1078.473f, -68.75002f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1496.444f, -252.7777f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(739.5834f, -63.19448f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1496.444f, -252.7777f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(897.9171f, -68.75002f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1496.444f, -252.7777f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(722.9171f, -68.75002f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1496.444f, -252.7777f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(834.0282f, -65.97225f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1496.444f, -252.7777f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(872.9169f, -68.75002f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 231.6664f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, -6.667023f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, -245.0005f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(1003.472f, -60.4167f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 231.6664f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, -6.667023f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, -245.0005f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(936.8058f, -65.97218f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1496.444f, -252.7777f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(975.6947f, -68.75002f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
        }

        void SetPos_WithRemoveMe()
        {
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Remove"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -249.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -488.8888f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(786.8054f, 22.91662f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-942.361f, 27.08332f); _q.Size = new Vector2(1167.945f, 908.121f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Remove"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -249.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -488.8888f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(897.9163f, 34.0277f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-942.361f, 27.08332f); _q.Size = new Vector2(1167.945f, 908.121f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Remove"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -249.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -488.8888f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(736.8054f, 34.0277f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-942.361f, 27.08332f); _q.Size = new Vector2(1167.945f, 908.121f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Remove"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -249.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -488.8888f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(792.3611f, 31.24993f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-942.361f, 27.08332f); _q.Size = new Vector2(1167.945f, 908.121f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Remove"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -249.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -488.8888f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(945.1382f, 34.0277f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-942.361f, 27.08332f); _q.Size = new Vector2(1167.945f, 908.121f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Remove"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -249.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -488.8888f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(914.583f, 28.4721f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-942.361f, 27.08332f); _q.Size = new Vector2(1167.945f, 908.121f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
			else
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Remove"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -249.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -488.8888f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(1014.583f, 31.24993f);

				QuadClass _q;
				_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-942.361f, 27.08332f); _q.Size = new Vector2(1167.945f, 908.121f); }

				MyPile.Pos = new Vector2(995.1394f, -13.19449f);
			}
		}

        protected virtual void MakeExitItem()
        {
            MenuItem item = new MenuItem(new EzText(Localization.Words.ExitLevel, ItemFont, CenterItems));
            item.Go = Cast.ToItem(VerifyExit);
            item.Name = "Exit";
            AddItem(item);
        }

        private void VerifyExit()
        {
            Call(new VerifyQuitLevelMenu(Control), 0);

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        protected override void MyDraw()
        {
            Pos.Update();

            base.MyDraw();
        }
    }
}