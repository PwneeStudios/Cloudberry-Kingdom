using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CloudberryKingdom
{
#if PC
    public class ControlItem : MenuItem
    {
        public QuadClass MyQuad;
        public Keys MyKey;
        public Action<Keys> SetSecondaryKey;
        public Action<ControlItem> Reset;

		public bool AllowSpecialKeys = false;

        public ControlItem(Localization.Words description, Keys key)
            : base(new EzText(description, Resources.Font_Grobold42, 2000, false, false, .65f))
        {
			Init(description, key);
		}

		public ControlItem(Localization.Words description, Keys key, bool AllowSpecialKeys)
			: base(new EzText(description, Resources.Font_Grobold42, 2000, false, false, .65f))
		{
			this.AllowSpecialKeys = AllowSpecialKeys;

			Init(description, key);
		}

		void Init(Localization.Words description, Keys key)
		{
            MyKey = key;
            MyQuad = new QuadClass("White", 72);
            MyQuad.Quad.SetColor(new Color(240,240,240));
            SetKey(MyKey);
        }

        public void SetKey(Keys key)
        {
			if (!AllowSpecialKeys &&
				(key == Keys.Space || key == Keys.Enter || key == Keys.Escape || key == Keys.Back))
			{
				return;
			}

            MyKey = key;
            //SetSecondaryKey(key);
            SetQuad();
        }

		public override void Draw(bool Text, Camera cam, bool Selected)
		{
			if (MyQuad != null)
			{
				MyQuad.Pos = Pos + new Vector2(-150, -115) + cam.Pos;
				//MyQuad.Pos = Tools.CurCamera.Pos;
				MyQuad.Draw();
			}

			base.Draw(Text, cam, Selected);
		}

        public void SetQuad()
        {
            MyQuad.TextureName = ButtonString.KeyToTexture(MyKey);
            MyQuad.ScaleYToMatchRatio();
        }
    }

    public class CustomControlsMenu : CkBaseMenu
    {
        public static Color SecondaryKeyColor = Color.SkyBlue;
        protected override void ReleaseBody()
        {
            base.ReleaseBody();
        }

        void Save()
        {
            // Before we exit make sure secondary keys match up to what the user just specified.
            foreach (MenuItem item in MyMenu.Items)
            {
                ControlItem citem = item as ControlItem;
                if (null != citem)
                    citem.SetSecondaryKey(citem.MyKey);
            }

            PlayerManager.SaveRezAndKeys();
        }

        protected override void SetTextProperties(EzText text)
        {
            base.SetTextProperties(text);

            //text.Shadow = false;
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Shadow = false;
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

            // Add the associated quad
            ControlItem citem = item as ControlItem;
            if (null == citem) return;

            //MyPile.Add(citem.MyQuad);
        }

        public CustomControlsMenu() : base(false)
        {
            EnableBounce();

            Constructor();
        }

        protected QuadClass Backdrop;
        public virtual void MakeBackdrop()
        {
			if (UseSimpleBackdrop)
            {
                Backdrop = new QuadClass("Arcade_BoxLeft", 1500, true);
            }
            else
            {
                Backdrop = new QuadClass("Backplate_1230x740", 1500, true);
            }
   
            MyPile.Add(Backdrop);

			if (!UseSimpleBackdrop)
			{
				EpilepsySafe(.9f);
			}

            Backdrop.Size = new Vector2(1376.984f, 1077.035f);
            Backdrop.Pos = new Vector2(-18.6521f, -10.31725f);
        }

        public void MakeInstructions()
        {
            //var backdrop = new QuadClass("score_screen", 1500, true);
            //MyPile.Add(backdrop);
            //MyPile.Add(backdrop);
            //backdrop.Size = new Vector2(603.1736f, 429.5635f);
            //backdrop.Pos = new Vector2(1157.441f, 51.19061f);
            //backdrop.Quad.SetColor(new Color(215, 215, 215, 255));

            //var text = new EzText("Press any key to switch control of selected item.", Resources.Font_Grobold42, 700, true, true, .675f);
            //text.Scale *= .68f;
            //text.Pos = new Vector2(1158.73f, 79.36493f);
            //MyPile.Add(text, "instructions");
        }

        void Reset(MenuItem _item)
        {
            ButtonCheck.Reset();
            foreach (var item in MyMenu.Items)
            {
                ControlItem c = item as ControlItem;
                if (null == c) continue;

                c.Reset(c);
            }
        }

        void MakeBack()
        {
            MenuItem item;

            // Customize
            item = new MenuItem(new EzText(Localization.Words.Reset, ItemFont));
            item.Name = "Reset";
            item.Go = Reset;
            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();

            ItemPos = new Vector2(698.9696f, 892.0638f);
            item.UnaffectedByScroll = true;
            AddItem(item);
            item.ScaleText(.5f);

            // Back
            ItemPos = new Vector2(810.0829f, 752.6987f);
            item = MakeBackButton();
            item.UnaffectedByScroll = true;
            item.ScaleText(.5f);
        }

        public override void Init()
        {
            base.Init();

            PauseGame = true;

            ReturnToCallerDelay = 10;
            SlideInLength = 26;
            SlideOutLength = 26;

            this.SlideInFrom = PresetPos.Right;
            this.SlideOutTo = PresetPos.Right;

            FontScale = .8f;

            MyPile = new DrawPile();

            // Make the backdrop
            MakeBackdrop();

            MakeInstructions();

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.Control = Control;

            MakeBack();
            ItemSetup();

            ControlItem item;

            item = new ControlItem(Localization.Words.QuickSpawn, ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey, true);
            item.Name = "quickspawn";
            item.SetSecondaryKey = key => ButtonCheck.Quickspawn_KeyboardKey.Set(key);
			item.Reset = _item => _item.SetKey(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey);
            AddItem(item);

            item = new ControlItem(Localization.Words.PowerUpMenu, ButtonCheck.Help_KeyboardKey.KeyboardKey, true);
            item.Name = "powerup";
            item.SetSecondaryKey = key => ButtonCheck.Help_KeyboardKey.Set(key);
            item.Reset = _item => _item.SetKey(ButtonCheck.Help_KeyboardKey.KeyboardKey);
            AddItem(item);

            item = new ControlItem(Localization.Words.Left, ButtonCheck.Left_Secondary);
            item.Name = "left";
            item.SetSecondaryKey = key => ButtonCheck.Left_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.Left_Secondary);
            AddItem(item);

            item = new ControlItem(Localization.Words.Right, ButtonCheck.Right_Secondary);
            item.Name = "right";
            item.SetSecondaryKey = key => ButtonCheck.Right_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.Right_Secondary);
            AddItem(item);

            item = new ControlItem(Localization.Words.Up, ButtonCheck.Up_Secondary);
            item.Name = "up";
            item.SetSecondaryKey = key => ButtonCheck.Up_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.Up_Secondary);
            AddItem(item);

            item = new ControlItem(Localization.Words.Down, ButtonCheck.Down_Secondary);
            item.Name = "down";
            item.SetSecondaryKey = key => ButtonCheck.Down_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.Down_Secondary);
            AddItem(item);

            item = new ControlItem(Localization.Words.ReplayPrev, ButtonCheck.ReplayPrev_Secondary);
            item.Name = "replayprev";
            item.SetSecondaryKey = key => ButtonCheck.ReplayPrev_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.ReplayPrev_Secondary);
            AddItem(item);

            item = new ControlItem(Localization.Words.ReplayNext, ButtonCheck.ReplayNext_Secondary);
            item.Name = "replaynext";
            item.SetSecondaryKey = key => ButtonCheck.ReplayNext_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.ReplayNext_Secondary);
            AddItem(item);

			item = new ControlItem(Localization.Words.ReplayToggle, ButtonCheck.SlowMoToggle_Secondary);
            item.Name = "replaytoggle";
			item.SetSecondaryKey = key => ButtonCheck.SlowMoToggle_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.SlowMoToggle_Secondary);
            AddItem(item);

			//item = new ControlItem(Localization.Words.ActivateSlowMo, ButtonCheck.SlowMoToggle_Secondary);
			//item.Name = "toggleslowmo";
			//item.SetSecondaryKey = key => ButtonCheck.SlowMoToggle_Secondary = key;
			//item.Reset = _item => _item.SetKey(ButtonCheck.SlowMoToggle_Secondary);
			//AddItem(item);

            ButtonCheck.KillSecondary();
            MyMenu.OnX = MyMenu.OnB =
                _m =>
                {
                    Save();
                    return MenuReturnToCaller(_m);
                };

            // Shift everything
            EnsureFancy();

            MyMenu.SelectItem(2);

            SetPos();
        }

        void SetPos()
        {
			MenuItem _item;
			_item = MyMenu.FindItemByName("Reset"); if (_item != null) { _item.SetPos = new Vector2(648.9698f, 861.5082f); _item.MyText.Scale = 0.4f; _item.MySelectedText.Scale = 0.4f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(676.7499f, 761.0321f); _item.MyText.Scale = 0.4f; _item.MySelectedText.Scale = 0.4f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("quickspawn"); if (_item != null) { _item.SetPos = new Vector2(-886.1108f, 870.3333f); _item.MyText.Scale = 0.5840001f; _item.MySelectedText.Scale = 0.5840001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("powerup"); if (_item != null) { _item.SetPos = new Vector2(-886.1108f, 683.2222f); _item.MyText.Scale = 0.5840001f; _item.MySelectedText.Scale = 0.5840001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("left"); if (_item != null) { _item.SetPos = new Vector2(-788.8887f, 432.2228f); _item.MyText.Scale = 0.5840001f; _item.MySelectedText.Scale = 0.5840001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("right"); if (_item != null) { _item.SetPos = new Vector2(-788.8887f, 253.4447f); _item.MyText.Scale = 0.5840001f; _item.MySelectedText.Scale = 0.5840001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("up"); if (_item != null) { _item.SetPos = new Vector2(-788.8887f, 74.66669f); _item.MyText.Scale = 0.5840001f; _item.MySelectedText.Scale = 0.5840001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("down"); if (_item != null) { _item.SetPos = new Vector2(-788.8887f, -104.1114f); _item.MyText.Scale = 0.5840001f; _item.MySelectedText.Scale = 0.5840001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("replayprev"); if (_item != null) { _item.SetPos = new Vector2(-858.3333f, -363.4443f); _item.MyText.Scale = 0.539f; _item.MySelectedText.Scale = 0.539f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("replaynext"); if (_item != null) { _item.SetPos = new Vector2(-858.3333f, -544.9998f); _item.MyText.Scale = 0.5430002f; _item.MySelectedText.Scale = 0.5430002f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("replaytoggle"); if (_item != null) { _item.SetPos = new Vector2(-858.3333f, -726.5553f); _item.MyText.Scale = 0.4623334f; _item.MySelectedText.Scale = 0.4623334f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(2.777832f, 0f);
			MyPile.Pos = new Vector2(0f, 0f);
		}

        private void ItemSetup()
        {
            ItemPos = new Vector2(0f, 700f) + new Vector2(-850, 187);//214.2859f);
            PosAdd = new Vector2(0, -176);//-165);
            FontScale *= .73f;// .778f;
        }

		//int ActiveCount = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active || MyMenu == null || MyMenu.Released) return;

			//ActiveCount++;
			//if (ActiveCount < 

            ControlItem item = MyMenu.CurItem as ControlItem;
            if (null != item)
            {
				foreach (Keys key in ButtonString.KeyToString.Keys)
				{
					if (key != Keys.Escape && key != Keys.Back &&
						ButtonCheck.State(key).Pressed)
					{
						//// Make sure there are no double keys
						//foreach (MenuItem other in MyMenu.Items)
						//{
						//    ControlItem citem = other as ControlItem;
						//    if (null != citem && citem.MyKey == key)
						//        citem.SetKey(item.MyKey);
						//}

						item.SetKey(key);
					}
				}
            }
        }
    }
#endif
}