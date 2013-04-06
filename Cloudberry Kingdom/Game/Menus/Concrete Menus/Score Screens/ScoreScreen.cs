using Microsoft.Xna.Framework;

using CoreEngine;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Stats;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom
{
    public class ScoreScreen : CkBaseMenu
    {
        bool _Add_Watch, _Add_Save;
        protected virtual void MakeMenu()
        {
            if (AsMenu)
            {
                MyMenu = new Menu(false);

                MyMenu.Control = -1;

                MyMenu.OnB = null;

                _Add_Watch = MyGame.MyLevel.ReplayAvailable;
                _Add_Save = MyGame.MyLevel.MyLevelSeed != null && MyGame.MyLevel.MyLevelSeed.Saveable;

                MenuItem item, go;

				if (InCampaign)
				{
					go = item = new MenuItem(new EzText(Localization.Words.Continue, ItemFont));
					item.Go = MenuGo_Continue;
				}
				else
				{
					go = item = new MenuItem(new EzText(Localization.Words.KeepSettings, ItemFont));
					item.Go = MenuGo_NewLevel;
				}
                item.Name = "Continue";
                AddItem(item);
                item.MyText.Scale =
                item.MySelectedText.Scale *= 1.3f;
                item.Shift(new Vector2(-86f, 65));
                item.SelectedPos.X += 6;

                if (_Add_Watch)
                {
                    item = new MenuItem(new EzText(Localization.Words.WatchReplay, ItemFont));
                    item.Name = "Replay";
                    item.Go = MenuGo_WatchReplay;
                    AddItem(item);
                }

                if (_Add_Save)
                {
                    item = new MenuItem(new EzText(Localization.Words.SaveSeed, ItemFont));
                    item.Name = "Save";
                    item.Go = MenuGo_Save;
					item.Selectable = CloudberryKingdomGame.CanSave();
                    AddItem(item);

                    // Don't gray out if any existing player has free space to save a level
                    bool GrayOut = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (PlayerManager.Players[i] != null &&
                            PlayerManager.Players[i].Exists &&
                            PlayerManager.Players[i].MySavedSeeds.SeedStrings.Count < InGameStartMenu.MAX_SEED_STRINGS)
                        {
                            GrayOut = false;
                        }
                    }

                    if (GrayOut)
				    {
                        CloudberryKingdomGame.ChangeSaveGoFunc(item);
				    }
                }

				MenuItem back;
				if (InCampaign)
				{
					back = new MenuItem(new EzText(Localization.WordString(Localization.Words.Exit), ItemFont));
					AddItem(back);
					back.Go = MenuGo_ExitCampaign;
				}
				else
				{
					if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
						back = new MenuItem(new EzText(Localization.WordString(Localization.Words.Back), ItemFont));
					else
						back = new MenuItem(new EzText(Localization.WordString(Localization.Words.BackToFreeplay), ItemFont));
					AddItem(back);
					back.Go = MenuGo_ExitFreeplay;
				}

				MyMenu.OnB = null;
                //MyMenu.OnB = Cast.ToMenu(go.Go);
				//MyMenu.OnB = Cast.ToMenu(back.Go);

                EnsureFancy();
                MyMenu.FancyPos.RelVal = new Vector2(869.0476f, -241.6667f);
            }
            else
            {
                QuadClass ContinueButton = new QuadClass(ButtonTexture.Go, 90, false);
                ContinueButton.Name = "GoButton";
                MyPile.Add(ContinueButton);
                ContinueButton.Pos = new Vector2(180f, -477.7778f) + ShiftAll;

                EzText ContinueText = new EzText(Localization.Words.Continue, ItemFont);
                ContinueText.Name = "Continue";
                SetHeaderProperties(ContinueText);
                ContinueText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
                MyPile.Add(ContinueText);
                ContinueText.Pos = new Vector2(180f, -477.7778f) + ShiftAll;

                if (MyGame.MyLevel.ReplayAvailable)
                {
                    QuadClass XButton = new QuadClass(ButtonTexture.X, 90, false);
                    XButton.Name = "XButton";
                    MyPile.Add(XButton);
                    XButton.Pos = new Vector2(180f, -325.3333f) + ShiftAll;

                    EzText ReplayText = new EzText(Localization.Words.WatchReplay, ItemFont);
                    SetHeaderProperties(ReplayText);
                    ReplayText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
                    ReplayText.MyFloatColor = new Color(184, 231, 231).ToVector4();
                    MyPile.Add(ReplayText);
                    ReplayText.Pos = new Vector2(180f, -325.3333f) + ShiftAll;
                }
            }
        }

        EzSound ScoreSound, BonusSound;

        public int DelayPhsx = 5;

        public ScoreScreen(bool CallBaseConstructor) : base(CallBaseConstructor) { }

		bool InCampaign;
        public ScoreScreen(StatGroup group, GameData game, bool InCampaign) : base(false)
        {
			this.InCampaign = InCampaign;

            MyGame = game;
            MyStatGroup = group;
            FontScale = .6f;

            Constructor();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

			//StartMenu.SetItemProperties_Red(item);	
            //CkColorHelper.GreenItem(item);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;

			//text.MyFloatColor = ColorHelper.Gray(.85f);
			//text.OutlineColor = ColorHelper.Gray(.05f);
			text.MyFloatColor = ColorHelper.Gray(.925f);
			text.OutlineColor = ColorHelper.Gray(.05f);

			text.Shadow = true;
			text.ShadowColor = new Color(.2f, .2f, .2f, .25f);
			text.ShadowOffset = new Vector2(12, 12);

			text.Scale = FontScale * .9f;
        }

// Whether to make a menu, or a static text with key bindings
#if PC_VERSION
        static bool AsMenu = true;
#else
        static bool AsMenu = true;
#endif

        protected QuadClass LevelCleared;
        Vector2 ShiftAll = new Vector2(-110f, -20f);
        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();

            //MakeDarkBack();


			MakeDarkBack();

			//QuadClass Backdrop = new QuadClass("Score_Screen", "Backdrop");
			//MyPile.Add(Backdrop);
			QuadClass Backdrop = new QuadClass("Backplate_1230x740", "Backdrop");
			MyPile.Add(Backdrop);
			//MyPile.Add(Backdrop);
			
			EpilepsySafe(.5f);
			

			//LevelCleared = new QuadClass("LevelCleared", "Header");
			//LevelCleared.Scale(.9f);
			//MyPile.Add(LevelCleared);
			//LevelCleared.Pos = new Vector2(10, 655) + ShiftAll;
			var lc = new EzText(Localization.Words.LevelCleared, Resources.Font_Grobold42_2, "LevelCleared");
			SetHeaderProperties(lc);
			lc.Shadow = true;
			lc.ShadowOffset = new Vector2(20, 20);
			lc.ShadowColor = new Color(.36f, .36f, .36f, .86f);
			MyPile.Add(lc);

            MyPile.Add(new QuadClass("Coin_Blue", "Coin"));
            MyPile.Add(new QuadClass("Stopwatch_Black", "Stopwatch"));
            MyPile.Add(new QuadClass("Bob_Dead", "Death"));

            MakeMenu();

            ScoreSound = Tools.SoundWad.FindByName("Coin");
            BonusSound = Tools.SoundWad.FindByName("Coin");
            ScoreSound.MaxInstances = 2;

            SetPos();
        }

        void SetPos()
        {
			if (InCampaign)
			{
				if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-224.5549f, 663.889f); _item.MyText.Scale = 0.9387498f; _item.MySelectedText.Scale = 0.9387498f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-302.4446f, 313.8959f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-294.1105f, 125.0071f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-310.7764f, -297.2153f); _item.MyText.Scale = 0.6325002f; _item.MySelectedText.Scale = 0.6325002f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-1119.443f, 791.6667f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-38.44455f, 566.6667f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-244.1106f, 291.6737f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-227.4435f, 66.67379f); _item.MyText.Scale = 0.5970834f; _item.MySelectedText.Scale = 0.5970834f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-369.1104f, -341.6597f); _item.MyText.Scale = 0.6360001f; _item.MySelectedText.Scale = 0.6360001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-621.7766f, 533.3334f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-335.7776f, 288.8959f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-749.6661f, 83.34048f); _item.MyText.Scale = 0.4923335f; _item.MySelectedText.Scale = 0.4923335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-571.8877f, -355.5486f); _item.MyText.Scale = 0.6360001f; _item.MySelectedText.Scale = 0.6360001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-1136.11f, 816.6666f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-477.3322f, 563.8889f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-680.2219f, 263.896f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-460.7775f, 61.11825f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-594.1104f, -355.5486f); _item.MyText.Scale = 0.6360001f; _item.MySelectedText.Scale = 0.6360001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-441.2219f, 550.0001f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-707.9993f, 227.7848f); _item.MyText.Scale = 0.5110831f; _item.MySelectedText.Scale = 0.5110831f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-571.8878f, 41.67381f); _item.MyText.Scale = 0.4922499f; _item.MySelectedText.Scale = 0.4922499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-377.4434f, -363.8819f); _item.MyText.Scale = 0.5636668f; _item.MySelectedText.Scale = 0.5636668f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-444.0002f, 563.8889f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-435.7776f, 283.3404f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-710.7775f, 86.11825f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-566.332f, -355.5486f); _item.MyText.Scale = 0.6360001f; _item.MySelectedText.Scale = 0.6360001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-1161.11f, 791.6667f); _t.Scale = 1.057333f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-471.7776f, 563.8889f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-485.7776f, 288.896f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-582.9991f, 72.22939f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-477.4443f, -349.9931f); _item.MyText.Scale = 0.6360001f; _item.MySelectedText.Scale = 0.6360001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-302.3342f, 552.7779f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-735.7776f, 294.4515f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-485.7775f, 91.67379f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-696.8877f, -330.5486f); _item.MyText.Scale = 0.6360001f; _item.MySelectedText.Scale = 0.6360001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-221.7776f, 588.8889f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-435.7776f, 283.3404f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-438.5548f, 75.00716f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-438.5547f, -274.993f); _item.MyText.Scale = 0.6337501f; _item.MySelectedText.Scale = 0.6337501f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-719.4434f, 824.9999f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-491.2219f, 566.6667f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-435.7776f, 283.3404f); _item.MyText.Scale = 0.6167499f; _item.MySelectedText.Scale = 0.6167499f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-594.1105f, 86.11825f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-413.5547f, -311.1042f); _item.MyText.Scale = 0.6360001f; _item.MySelectedText.Scale = 0.6360001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
			}
			else
			{
				if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-427.3342f, 536.1111f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-308.0003f, 277.7849f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-313.5558f, 94.45162f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-774.666f, -338.8819f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-594.0002f, 738.889f); _item.MyText.Scale = 0.9545831f; _item.MySelectedText.Scale = 0.9545831f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-344.1106f, 397.2292f); _item.MyText.Scale = 0.67125f; _item.MySelectedText.Scale = 0.67125f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-346.8889f, 213.896f); _item.MyText.Scale = 0.67125f; _item.MySelectedText.Scale = 0.67125f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-602.4443f, -302.7708f); _item.MyText.Scale = 0.67125f; _item.MySelectedText.Scale = 0.67125f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-655.5558f, 22.22227f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-649.9994f, -402.7777f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-655.5553f, 411.1111f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -141.6665f); _q.Size = new Vector2(190.8688f, 190.8688f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-913.8881f, -513.8888f); _q.Size = new Vector2(180.9532f, 211.6065f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-983.3334f, 230.5556f); _q.Size = new Vector2(301.7774f, 221.3034f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-830.111f, 500.0001f); _item.MyText.Scale = 0.6186666f; _item.MySelectedText.Scale = 0.6186666f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-177.4445f, 275.007f); _item.MyText.Scale = 0.540917f; _item.MySelectedText.Scale = 0.540917f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-827.4455f, 97.22939f); _item.MyText.Scale = 0.5631669f; _item.MySelectedText.Scale = 0.5631669f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(36.44333f, -247.2152f); _item.MyText.Scale = 0.5860835f; _item.MySelectedText.Scale = 0.5860835f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(885.7141f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-688.8889f, 22.22227f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-677.7777f, -402.7777f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-691.6666f, 411.1111f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-944.4442f, -169.4443f); _q.Size = new Vector2(150.3689f, 150.3689f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-886.1108f, -513.8888f); _q.Size = new Vector2(159.8058f, 186.773f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-983.3334f, 219.4445f); _q.Size = new Vector2(281.6637f, 206.5533f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-793.9996f, 516.6667f); _item.MyText.Scale = 0.6325831f; _item.MySelectedText.Scale = 0.6325831f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-538.5558f, 266.6737f); _item.MyText.Scale = 0.5548335f; _item.MySelectedText.Scale = 0.5548335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-410.7795f, 91.67379f); _item.MyText.Scale = 0.5871667f; _item.MySelectedText.Scale = 0.5871667f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-866.3331f, -424.993f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-688.8889f, 22.22227f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-677.7777f, -402.7777f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-691.6666f, 411.1111f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-944.4442f, -169.4443f); _q.Size = new Vector2(150.3689f, 150.3689f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-886.1108f, -513.8888f); _q.Size = new Vector2(159.8058f, 186.773f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-983.3334f, 219.4445f); _q.Size = new Vector2(281.6637f, 206.5533f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-1055.111f, 516.6667f); _item.MyText.Scale = 0.6325831f; _item.MySelectedText.Scale = 0.6325831f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-774.6672f, 275.007f); _item.MyText.Scale = 0.5548335f; _item.MySelectedText.Scale = 0.5548335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-724.6672f, 91.67379f); _item.MyText.Scale = 0.5871667f; _item.MySelectedText.Scale = 0.5871667f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-780.2227f, -424.993f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-688.8889f, 22.22227f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-677.7777f, -402.7777f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-691.6666f, 411.1111f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-944.4442f, -169.4443f); _q.Size = new Vector2(150.3689f, 150.3689f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-886.1108f, -513.8888f); _q.Size = new Vector2(159.8058f, 186.773f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-983.3334f, 219.4445f); _q.Size = new Vector2(281.6637f, 206.5533f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-1180.111f, 505.5556f); _item.MyText.Scale = 0.6251668f; _item.MySelectedText.Scale = 0.6251668f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-646.8889f, 266.6737f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-641.3332f, 91.67379f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-808f, -424.993f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-1138.888f, 788.8889f); _t.Scale = 1.1235f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8879f, -133.3333f); _t.Scale = 0.7865838f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5541f, -391.6666f); _t.Scale = 0.7871667f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-652.777f, 127.7778f); _t.Scale = 0.84325f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-847.2215f, -286.111f); _q.Size = new Vector2(130.5354f, 130.5354f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-844.4448f, -522.2222f); _q.Size = new Vector2(101.0536f, 118.1064f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-913.8881f, -38.88887f); _q.Size = new Vector2(220.6411f, 161.8035f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-1063.444f, 513.8889f); _item.MyText.Scale = 0.5346668f; _item.MySelectedText.Scale = 0.5346668f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-508.0002f, 330.5626f); _item.MyText.Scale = 0.5419167f; _item.MySelectedText.Scale = 0.5419167f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-291.3332f, 147.2363f); _item.MyText.Scale = 0.5166666f; _item.MySelectedText.Scale = 0.5166666f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-880.2227f, -341.6455f); _item.MyText.Scale = 0.5209166f; _item.MySelectedText.Scale = 0.5209166f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-719.4445f, 22.22227f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-661.1107f, -402.7777f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-655.5553f, 411.1111f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-955.5555f, -141.6665f); _q.Size = new Vector2(168.1188f, 168.1188f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-886.1108f, -513.8888f); _q.Size = new Vector2(180.9532f, 211.6065f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-983.3334f, 230.5556f); _q.Size = new Vector2(321.4366f, 235.7202f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-1018.999f, 508.3333f); _item.MyText.Scale = 0.6360832f; _item.MySelectedText.Scale = 0.6360832f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-646.8889f, 266.6737f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-641.3332f, 91.67379f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-808f, -424.993f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-719.4445f, 22.22227f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-661.1107f, -402.7777f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-655.5553f, 411.1111f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-955.5555f, -141.6665f); _q.Size = new Vector2(168.1188f, 168.1188f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-886.1108f, -513.8888f); _q.Size = new Vector2(180.9532f, 211.6065f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-983.3334f, 230.5556f); _q.Size = new Vector2(321.4366f, 235.7202f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-624.5549f, 522.2223f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-416.3333f, 269.4515f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-441.3332f, 94.45156f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-630.2217f, -330.5486f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
				else
				{
					MenuItem _item;
					_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(-846.7776f, 516.6667f); _item.MyText.Scale = 0.78f; _item.MySelectedText.Scale = 0.78f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(-416.3333f, 269.4515f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName("Replay"); if (_item != null) { _item.SetPos = new Vector2(-591.3332f, 86.11825f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }
					_item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-774.666f, -338.8819f); _item.MyText.Scale = 0.6f; _item.MySelectedText.Scale = 0.6f; _item.SelectIconOffset = new Vector2(0f, 0f); }

					MyMenu.Pos = new Vector2(902.3811f, -136.1111f);

					EzText _t;
					_t = MyPile.FindEzText("LevelCleared"); if (_t != null) { _t.Pos = new Vector2(-930.5547f, 797.2222f); _t.Scale = 1.195833f; }
					_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-638.8889f, -2.777716f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Blobs"); if (_t != null) { _t.Pos = new Vector2(-630.5551f, -386.1111f); _t.Scale = 1f; }
					_t = MyPile.FindEzText("Deaths"); if (_t != null) { _t.Pos = new Vector2(-636.111f, 388.8889f); _t.Scale = 1f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.887f, 5000f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77808f, -6.666618f); _q.Size = new Vector2(1509.489f, 943.4307f); }
					_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-930.5555f, -163.8887f); _q.Size = new Vector2(174.7021f, 174.7021f); }
					_q = MyPile.FindQuad("Stopwatch"); if (_q != null) { _q.Pos = new Vector2(-916.6664f, -522.2221f); _q.Size = new Vector2(177.0607f, 206.9397f); }
					_q = MyPile.FindQuad("Death"); if (_q != null) { _q.Pos = new Vector2(-969.4437f, 202.7778f); _q.Size = new Vector2(291.891f, 214.0534f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
			}
        }

        FancyVector2 zoom = new FancyVector2();
        public static bool UseZoomIn = true;

        protected StatGroup MyStatGroup = StatGroup.Level;
        public override void OnAdd()
        {
 	        base.OnAdd();

            if (UseZoomIn)
            {
                SlideIn(0);
                zoom.MultiLerp(6, DrawPile.BubbleScale.Map(v => (v - Vector2.One) * .3f + Vector2.One));
            }

            // Calculate scores
            PlayerManager.CalcScore(MyStatGroup);

            int Coins = PlayerManager.PlayerSum(p => p.GetStats(MyStatGroup).Coins);
            int CoinTotal = PlayerManager.PlayerMax(p => p.GetStats(MyStatGroup).TotalCoins);
            int Blobs = PlayerManager.PlayerSum(p => p.GetStats(MyStatGroup).Blobs);
            int BlobTotal = PlayerManager.PlayerMax(p => p.GetStats(MyStatGroup).TotalBlobs);

			
			EzText text;
			
			text = new EzText(Tools.ScoreString(Coins, CoinTotal), ItemFont, "Coins");
			SetHeaderProperties(text);
            MyPile.Add(text);

            text = new EzText(CoreMath.ShortTime(PlayerManager.Score_Time), ItemFont, "Blobs");
			SetHeaderProperties(text);
			MyPile.Add(text);

            text = new EzText(Tools.ScoreString(PlayerManager.Score_Attempts), ItemFont, "Deaths");
			SetHeaderProperties(text);
			MyPile.Add(text);


            // Prevent menu interactions for a second
            MyMenu.Active = false;

            SetPos();
            MyMenu.SortByHeight();

            MyGame.WaitThenDo(DelayPhsx, () => MyMenu.Active = true);
        }

        protected override void MyDraw()
        {
            if (Core.MyLevel.Replay || Core.MyLevel.Watching) return;

            Vector2 SaveZoom = MyGame.Cam.Zoom;
            Vector2 SaveHoldZoom = MyGame.Cam.HoldZoom;
            Tools.QDrawer.Flush();

            if (zoom != null)
            {
                MyGame.Cam.Zoom = .001f * zoom.Update();
                MyGame.Cam.SetVertexCamera();
                EzText.ZoomWithCamera_Override = true;
            }

            Pos.SetCenter(Core.MyLevel.MainCamera, true);
            Pos.Update();

            base.MyDraw();

            Tools.Render.EndSpriteBatch();

            if (zoom != null)
            {
                MyGame.Cam.Zoom = SaveZoom;
                MyGame.Cam.HoldZoom = SaveHoldZoom;
                MyGame.Cam.SetVertexCamera();
                EzText.ZoomWithCamera_Override = false;
                Tools.QDrawer.Flush();
            }
        }

        protected override void MyPhsxStep()
        {
            Level level = Core.MyLevel;

            if (level != null)
                level.PreventReset = true;

            if (level.Replay || level.Watching)
                return;

            if (Active)
            {
                if (!ShouldSkip())
                {
					// Check if we should gray out save level
					bool GrayOut = true;
					for (int i = 0; i < 4; i++)
					{
						if (PlayerManager.Players[i] != null &&
							PlayerManager.Players[i].Exists &&
							PlayerManager.Players[i].MySavedSeeds.SeedStrings.Count < InGameStartMenu.MAX_SEED_STRINGS)
						{
							GrayOut = false;
						}
					}

                    if ((!Tools.CurLevel.CanLoadLevels && !Tools.CurLevel.CanSaveLevel)
                        || GrayOut)
                    {
                        var item = MyMenu.FindItemByName("Save");
                        if (item != null && item.MyText.MyFloatColor.W > .9f)
                        {
                            MyMenu.SelectItem(0);

                            CloudberryKingdomGame.ChangeSaveGoFunc(item);
                        }
                    }

                    if (AsMenu)
                        base.MyPhsxStep();
                    else
                        GUI_Phsx();
                }
            }
        }

        /// <summary>
        /// Play another level with the same seed
        /// </summary>
        protected void MenuGo_NewLevel(MenuItem item)
        {
            SlideOut(PresetPos.Left);

            Active = false;

            //Tools.SongWad.FadeOut();
            MyGame.EndMusicOnFinish = false;

            MyGame.WaitThenDo(36, () => MyGame.EndGame(true));
            return;
        }

        /// <summary>
        /// Called when 'Continue' is selected from the menu.
        /// The Score Screen slides out and the current game's EndGame function is called.
        /// </summary>
        protected virtual void MenuGo_Continue(MenuItem item)
        {
			SaveGroup.SaveAll();

            SlideOut(PresetPos.Left);

			if (InCampaign)
			{
				StringWorldGameData stringworld = Tools.WorldMap as StringWorldGameData;

				Door door = (ILevelConnector)Tools.CurLevel.FindIObject(LevelConnector.EndOfLevelCode) as Door;
				door.OnOpen = d => GameData.EOL_DoorAction(d);

				if (stringworld != null)
				{
					bool fade = door.MyLevel.MyLevelSeed != null && door.MyLevel.MyLevelSeed.FadeOut;
					if (fade)
						door.OnEnter = stringworld.EOL_StringWorldDoorEndAction_WithFade;
					else
						door.OnEnter = EOL_WaitThenDoEndAction;
								   
					stringworld.EOL_StringWorldDoorAction(door);
				}
			}
			else
			{
				MyGame.WaitThenDo(SlideOutLength + 2, () => MyGame.EndGame(false));
			}
        }

		void EOL_WaitThenDoEndAction(Door door)
		{
			StringWorldGameData stringworld = Tools.WorldMap as StringWorldGameData;

			if (stringworld != null)
			{
				door.Game.WaitThenDo(35, () => stringworld.EOL_StringWorldDoorEndAction(door));
			}
		}

        /// <summary>
        /// Called when 'Exit Freeplay' is selected from the menu.
        /// The Score Screen slides out and the current game's EndGame function is called.
        /// </summary>
        protected virtual void MenuGo_ExitFreeplay(MenuItem item)
        {
            SlideOut(PresetPos.Left);

			//if (MyGame.ParentGame != null)
			//{
			//    CustomLevel_GUI.ExitFreeplay = true;
			//}

            MyGame.WaitThenDo(SlideOutLength + 2, () => MyGame.EndGame(false));
        }

		void MenuGo_ExitCampaign(MenuItem item)
		{
			Tools.CurrentAftermath = new AftermathData();
			Tools.CurrentAftermath.Success = false;
			Tools.CurrentAftermath.EarlyExit = true;

			Tools.CurGameData.EndGame(false);
		}

        protected void MenuGo_Stats(MenuItem item)
        {
            Call(new StatsMenu(MyStatGroup), 19);
        }

        /// <summary>
        /// Called when 'Watch Replay' is selected from the menu.
        /// The level's replay is loaded, with the level's current information saved.
        /// </summary>
        protected void MenuGo_WatchReplay(MenuItem item)
        {
            if (AsMenu)
            {
                Active = false;

                MyGame.WaitThenDo(35, () =>
                    {
                        OnReturnTo(); // Re-activate the Score Screen object
                        Core.MyLevel.WatchReplay(true); // Start the replay
                    });
            }
            else
            {
                Core.MyLevel.WatchReplay(true);
            }
        }

        protected void MenuGo_Save(MenuItem item)
        {
            if (CloudberryKingdomGame.IsDemo)
            {
                Call(new UpSellMenu(Localization.Words.UpSell_SaveLoad, MenuItem.ActivatingPlayer), 0);
                Hide(PresetPos.Left, 0);
            }
            else
            {
#if PC_VERSION
                PlayerData player = MenuItem.GetActivatingPlayerData();
                SaveLoadSeedMenu.MakeSave(this, player)(item);
                Hide(PresetPos.Left);
#elif XBOX
				PlayerData player = MenuItem.GetActivatingPlayerData();
				if (CloudberryKingdomGame.CanSave(player.MyPlayerIndex))
				{
                    if (player != null && player.MySavedSeeds.SeedStrings.Count < InGameStartMenu.MAX_SEED_STRINGS)
                    {
                        SaveLoadSeedMenu.MakeSave(this, player)(item);
                    }
                    else
                    {
                        CloudberryKingdomGame.ShowError_CanNotSaveLevel_NoSpace();
                    }
				}
				else
				{
					CloudberryKingdomGame.ShowError_CanNotSaveNoDevice();
				}
#else
				PlayerData player = MenuItem.GetActivatingPlayerData();
				SaveLoadSeedMenu.MakeSave(this, player)(item);
#endif
            }
        }

        int LastActive;
        bool ShouldSkip()
        {
            if (LastActive + 5 < Tools.TheGame.PhsxCount)
            {
                LastActive = Tools.TheGame.PhsxCount;
                return true;
            }
            else
            {
                LastActive = Tools.TheGame.PhsxCount;
                return false;
            }
        }

        public void GUI_Phsx()
        {
            Level level = Core.MyLevel;

            if (MyGame.MyLevel.ReplayAvailable)
            {
                bool WatchReplay = false;
                if (level.CanWatchReplay && ButtonCheck.State(ControllerButtons.X, -1).Pressed)
                    WatchReplay = true;
#if PC_VERSION
            if (Tools.Keyboard.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape) ||
                Tools.PrevKeyboard.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape))
                WatchReplay = false;
#endif

                if (WatchReplay)
                    MenuGo_WatchReplay(null);
            }

            if (ButtonCheck.State(ControllerButtons.A, -1).Pressed)
                MenuGo_Continue(null);
        }
    }
}