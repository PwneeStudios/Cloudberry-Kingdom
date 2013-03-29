using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CloudberryKingdom
{
    public class SaveSeedAs : VerifyBaseMenu
    {
        public SaveSeedAs(int Control, PlayerData Player)
            : base(false)
        {
            EnableBounce();

            this.Control = Control;
            this.Player = Player;
            FixedToCamera = true;

            Constructor();
        }

        PlayerData Player;
        GUI_TextBox TextBox;
        EzText HeaderText;
        public override void Init()
        {
            base.Init();

            this.FontScale *= .9f;
            CallDelay = 0;
            ReturnToCallerDelay = 0;
            
            MenuItem item;

            // Header
            HeaderText = new EzText(Localization.Words.SaveRandomSeedAs, ItemFont, true, false);
            HeaderText.Name = "Header";
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);

            // Save seed
			//item = new MenuItem(new EzText(Localization.Words.SaveSeed, ItemFont));
			//item.Name = "Save";
			//item.Go = Save;
			//AddItem(item);
			
			// Console version: Start to save
			var start = new EzText(Localization.Words.PressStart, ItemFont);
			MyPile.Add(start, "Start");
			SetHeaderProperties(start);

			var x = new EzText(Localization.Words.Delete, ItemFont);
            MyPile.Add(x, "Delete");
			x.MyFloatColor = Menu.DefaultMenuInfo.UnselectedXColor;
			x.OutlineColor = Color.Black.ToVector4();

			MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_X"));

			var back = new EzText(Localization.Words.Back, ItemFont);
			MyPile.Add(back, "Back");
			back.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
			back.OutlineColor = Color.Black.ToVector4();

			MyPile.Add(new QuadClass(ButtonTexture.Back, 90, "Button_B"));

//#if PC_VERSION
//            MakeBackButton();
//#else
//            MakeBackButton();
//#endif

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            SetPosition();
            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        void Save(MenuItem _item)
        {
            // Save the seed
            if (TextBox.Text.Length > 0)
            {
                Player.MySavedSeeds.SaveSeed(Tools.CurLevel.MyLevelSeed.ToString(), TextBox.Text);

                // Success!
                var ok = new AlertBaseMenu(Control, Localization.Words.SeedSavedSuccessfully, Localization.Words.Hooray);
                ok.OnOk = OnOk;
                Call(ok);

				SavedSeedsGUI.LastSeedSave_TimeStamp = Tools.DrawCount;
            }
            else
            {
                // Failure!
                var ok = new AlertBaseMenu(Control, Localization.Words.NoNameGiven, Localization.Words.Oh);
                ok.OnOk = OnOk;
                Call(ok);
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

        public override void OnReturnTo()
        {
            // Do nothing
        }

        void OnOk()
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
			MyMenu.Pos = new Vector2(-1180.001f, -240);

			EzText _t;
			_t = MyPile.FindEzText("Header"); if (_t != null)
			{
				_t.Pos = new Vector2(1130.555f, 813.5558f);
				_t.Scale = 0.8019168f;
				float w = _t.GetWorldWidth();
				if (w > 1900)
					_t.Scale *= 1900.0f / w;
			}
			_t = MyPile.FindEzText("Start"); if (_t != null) { _t.Pos = new Vector2(705.5557f, -125f); _t.Scale = 0.6350001f; }
			_t = MyPile.FindEzText("Delete"); if (_t != null) { _t.Pos = new Vector2(2080.556f, 102.7779f); _t.Scale = 0.4135835f; }
			_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(2080.554f, -5.340576E-05f); _t.Scale = 0.4390834f; }

			QuadClass _q;
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1175.696f, 233.3334f); _q.Size = new Vector2(1500f, 803.2258f); }
			_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(2033.333f, 27.77777f); _q.Size = new Vector2(46.08334f, 46.08334f); }
			_q = MyPile.FindQuad("Button_B"); if (_q != null) { _q.Pos = new Vector2(2033.333f, -83.33337f); _q.Size = new Vector2(45.91659f, 45.91659f); }

			MyPile.Pos = new Vector2(-1180.001f, -240);
		}

        public override void OnAdd()
        {
            base.OnAdd();

            TextBox = new GUI_TextBox(Tools.CurLevel.MyLevelSeed.SuggestedName(), Vector2.Zero, new Vector2(1.85f, .65f), .95f);
            TextBox.Control = Control;
            TextBox.MaxLength = 36;
            TextBox.FixedToCamera = false;
            TextBox.Pos.SetCenter(MyPile.FancyPos);
            TextBox.Pos.RelVal = new Vector2(830, 277.7778f);
			TextBox.MyText.X -= 0;
            TextBox.OnEnter += OnEnter;
            TextBox.OnEscape += OnEscape;
            MyGame.AddGameObject(TextBox);

			//TextBox.MyText.MyFloatColor = 
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

        void OnEnter()
        {
            if (TextBox.Text.Length <= 0)
                return;

            Save(null);
            //MyGame.WaitThenDo(35, () =>
            //{
            //    float width = MyGame.Cam.GetWidth();
            //    TextBox.Pos.LerpTo(new Vector2(-width, 0), 20);
            //});
        }
    }
}