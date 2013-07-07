using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CloudberryKingdom
{
    public class SaveSeedAs : EnterTextGui
    {
        PlayerData Player;

        public SaveSeedAs(int Control, PlayerData Player)
            : base(false)
        {
            this.Player = Player;

            HeaderWord = Localization.Words.SaveRandomSeedAs;
            SuggestedString = Tools.CurLevel.MyLevelSeed.SuggestedName();

            Constructor(Control);
        }

        protected override void OnEnter()
        {
            if (TextBox.Text.Length <= 0)
                return;

            // Save the seed
            if (TextBox.Text.Length > 0)
            {
                OnSuccess();
            }
            else
            {
                OnFailure();
            }

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
                Active = false;
            }
        }

        protected override void OnFailure()
        {
            // Failure!
            var ok = new AlertBaseMenu(Control, Localization.Words.NoNameGiven, Localization.Words.Oh);
            ok.OnOk = OnOk;
            Call(ok);
        }

        protected override void OnSuccess()
        {
			Player.MySavedSeeds.SaveSeed(Tools.CurLevel.MyLevelSeed.ToString(), TextBox.Text);
//#if PC_VERSION
//            for (int i = 0; i < 4; i++)
//            {
//                PlayerManager.Players[i].MySavedSeeds.SaveSeed(Tools.CurLevel.MyLevelSeed.ToString(), TextBox.Text);
//            }
//#else
//            Player.MySavedSeeds.SaveSeed(Tools.CurLevel.MyLevelSeed.ToString(), TextBox.Text);
//#endif

            // Success!
            var ok = new AlertBaseMenu(Control, Localization.Words.SeedSavedSuccessfully, Localization.Words.Hooray);
            ok.OnOk = OnOk;
            Call(ok);

            SavedSeedsGUI.LastSeedSave_TimeStamp = Tools.DrawCount;
        }
    }

    public class EnterTextGui : VerifyBaseMenu
    {
        public EnterTextGui(bool CallBaseConstructor)
            : base(CallBaseConstructor)
        {
        }

        protected void Constructor(int Control)
        {
            EnableBounce();

            this.Control = Control;
            FixedToCamera = true;

            Constructor();
        }

		ClickableBack Back;
        protected Localization.Words HeaderWord = Localization.Words.None;

        protected GUI_TextBox TextBox;
        EzText HeaderText;
        public override void Init()
        {
            base.Init();

            this.FontScale *= .9f;
            CallDelay = 0;
            ReturnToCallerDelay = 0;
            
            // Header
            HeaderText = new EzText(HeaderWord, ItemFont, true, false);
            HeaderText.Name = "Header";
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);

			// Console version: Start to save
			if (ButtonCheck.ControllerInUse)
			{
				EzText start = new EzText(Localization.Words.PressStart, ItemFont);
				MyPile.Add(start, "Start");
				SetHeaderProperties(start);
			}
			else
			// PC version: Enter to save
			{
				EzText start = new EzText(Localization.Words.Press + "{s80,0}" + ButtonString.Enter(140), ItemFont);
				MyPile.Add(start, "Start");
				SetHeaderProperties(start);
			}

			if (ButtonCheck.ControllerInUse)
			{
                // X Button
				var x = new EzText(Localization.Words.Delete, ItemFont);
				MyPile.Add(x, "Delete");
				x.MyFloatColor = Menu.DefaultMenuInfo.UnselectedXColor;
				x.OutlineColor = Color.Black.ToVector4();

				MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_X"));

                // A Button
                var a = new EzText(Localization.Words.Next, ItemFont);
                MyPile.Add(a, "Next");
                a.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
                a.OutlineColor = Color.Black.ToVector4();

                MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_A"));
			}

			Back = new ClickableBack(MyPile, false, true);

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            SetPosition();
            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        protected virtual void OnFailure()
        {
        }

        protected virtual void OnSuccess()
        {
        }

        public override void OnReturnTo()
        {
            // Do nothing
        }

        protected void OnOk()
        {
            this.SlideOutTo = PresetPos.Left;
            ReturnToCaller(false);
            
            this.Hid = true;
            this.Active = false;
        }

        public override void Release()
        {
            base.Release();

            TextBox.Release();
        }

		private void SetPosition()
		{
			if (ButtonCheck.ControllerInUse)
			{
				MyMenu.Pos = new Vector2(-1180.001f, -240f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null)
				{
					_t.Pos = new Vector2(1130.555f, 813.5558f);
					_t.Scale = 0.8019168f;
					float w = _t.GetWorldWidth();
					if (w > 1900)
						_t.Scale *= 1900.0f / w;
				}

                _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(1130.555f, 813.5558f); _t.Scale = 0.8019168f; }
                _t = MyPile.FindEzText("Start"); if (_t != null) { _t.Pos = new Vector2(705.5557f, -125f); _t.Scale = 0.6350001f; }
                _t = MyPile.FindEzText("Delete"); if (_t != null) { _t.Pos = new Vector2(2094.445f, -13.88877f); _t.Scale = 0.4135835f; }
                _t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(2088.889f, 113.8888f); _t.Scale = 0.4626667f; }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1175.696f, 233.3334f); _q.Size = new Vector2(1500f, 803.2258f); }
                _q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(2044.444f, -97.22224f); _q.Size = new Vector2(46.08334f, 46.08334f); }
                _q = MyPile.FindQuad("Button_A"); if (_q != null) { _q.Pos = new Vector2(2044.444f, 25.00002f); _q.Size = new Vector2(46.08331f, 46.08331f); }
                _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(2327.78f, -212.2218f); _q.Size = new Vector2(56.24945f, 56.24945f); }
                _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

                MyPile.Pos = new Vector2(-1180.001f, -240f);
			}
			else
			{
				MyMenu.Pos = new Vector2(-1180.001f, -240f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null)
				{
					_t.Pos = new Vector2(1130.555f, 813.5558f);
					_t.Scale = 0.8019168f;
					float w = _t.GetWorldWidth();
					if (w > 1900)
						_t.Scale *= 1900.0f / w;
				}

				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(1130.555f, 813.5558f); _t.Scale = 0.8019168f; }
				_t = MyPile.FindEzText("Start"); if (_t != null) { _t.Pos = new Vector2(705.5557f, -125f); _t.Scale = 0.6350001f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1175.696f, 233.3334f); _q.Size = new Vector2(1500f, 803.2258f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(2408.336f, -270.5551f); _q.Size = new Vector2(56.24945f, 56.24945f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

				MyPile.Pos = new Vector2(-1180.001f, -240f);
			}
		}

        protected string SuggestedString = "Type something!";

        public override void OnAdd()
        {
            base.OnAdd();

            TextBox = new GUI_TextBox(SuggestedString, Vector2.Zero, new Vector2(1.85f, .65f), .95f);
            TextBox.Control = Control;
            TextBox.MaxLength = 36;
            TextBox.FixedToCamera = false;
            TextBox.Pos.SetCenter(MyPile.FancyPos);
            TextBox.Pos.RelVal = new Vector2(830, 277.7778f);
			TextBox.MyText.X -= 0;
            TextBox.OnEnter += OnEnter;
            TextBox.OnEscape += OnEscape;
            MyGame.AddGameObject(TextBox);

			TextBox.MyText.OutlineColor = ColorHelper.Gray(.1f);
			StartMenu.SetTextSelected_Red(TextBox.MyText);

			MyMenu.Active = false;

            SetPosition();
        }

        void OnEscape()
        {
            TextBox.Active = false;
            ReturnToCaller();
        }

        protected virtual void OnEnter()
        {
        }
    }
}