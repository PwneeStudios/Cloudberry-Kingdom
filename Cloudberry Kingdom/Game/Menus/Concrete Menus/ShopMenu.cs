using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Stats;

using CoreEngine;

namespace CloudberryKingdom
{
	public class SmallErrorMenu : VerifyBaseMenu
	{
		Localization.Words Word;
		public SmallErrorMenu(Localization.Words Word)
			: base(false)
		{
			this.Word = Word;

			EnableBounce();

			this.Control = -1;

            PauseLevel = true;
            PauseGame = true;

			Constructor();

			MyMenu.OnB = null;

			Core.DrawLayer++;
		}

		public override void MakeBackdrop()
		{
			Backdrop = new QuadClass(null, true, true);
			Backdrop.TextureName = "WidePlaque";
			Backdrop.Size = new Vector2(1750f, 284.8255f);
			Backdrop.Pos = new Vector2(-11.9043f, 59.52365f);
			Backdrop.Degrees = 0;

			MyPile.Add(Backdrop, "ArcadeBox");
			MyPile.Pos = new Vector2(0, -800);

			//base.MakeBackdrop();
		}

		QuadClass Black;
		public override void Init()
		{
			base.Init();

			// Make the message
			var Description = new EzText(Word, Resources.Font_Grobold42_2, 1800, true, true, .575f);
			Description.Pos = new Vector2(0, 100);
			Description.Scale *= .6f;
			MyPile.Add(Description, "Description");

			SetPos();
		}

		protected override void MyPhsxStep()
		{
			base.MyPhsxStep();
		}

        protected override void MyDraw()
        {
            return;
            //base.MyDraw();
        }

		void SetPos()
		{
			MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

			EzText _t;
			_t = MyPile.FindEzText("Description"); if (_t != null) { _t.Pos = new Vector2(19.44458f, 36.11111f); _t.Scale = 0.6f; }

			QuadClass _q;
			_q = MyPile.FindQuad("ArcadeBox"); if (_q != null) { _q.Pos = new Vector2(10.31873f, -11.11108f); _q.Size = new Vector2(1123.342f, 176.4081f); }

			MyPile.Pos = new Vector2(72.22217f, 36.11115f);
		}
	}
	
    public class UpSellMenu : VerifyBaseMenu
    {
		Localization.Words Word;
        public UpSellMenu(Localization.Words Word, int Control) : base(false)
        {
			this.Word = Word;

			EnableBounce();

            this.Control = Control;

            Constructor();
        }

        void Yes(MenuItem item)
        {
            CloudberryKingdomGame.ShowFor = (PlayerIndex)CoreMath.Restrict(0, 3, MenuItem.ActivatingPlayer);
            CloudberryKingdomGame.ShowMarketplace = true;
        }

        void No(MenuItem item)
        {
			if (Word == Localization.Words.UpSell_Exit)
			{
				Tools.TheGame.Exit();
			}
			else if (Word == Localization.Words.FreePlay)
			{
				UseBounce = false;
				SlideOutTo = PresetPos.Right;
				SlideOutLength = 17;
			}

            ReturnToCaller();
        }

        public override void MakeBackdrop()
        {
			Black = new QuadClass("White");
			Black.Alpha = 0;
			Black.FullScreen(Tools.CurCamera);
			Black.Quad.SetColor(ColorHelper.Gray(.2f));
			MyPile.Add(Black, "Black");

			base.MakeBackdrop();
        }

		QuadClass Black;
        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
            EzText HeaderText = new EzText(Word, ItemFont, 1500, true, false, .7f);
            HeaderText.Scale *= .85f;
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText, "Header");
            HeaderText.Pos = HeaderPos + new Vector2(-200, 200);

			// Berry
			var Berry = new QuadClass(null, true, false);
			Berry.TextureName = "cb_crying";
			MyPile.Add(Berry, "Berry");

            // Yes
            item = new MenuItem(new EzText(Localization.Words.Yes, ItemFont, true));
            item.Go = Yes;
			item.Name = "Yes";
            AddItem(item);
			StartMenu.SetItemProperties_Red(item);
            item.SelectSound = null;

            // No
            item = new MenuItem(new EzText(Localization.Words.No, ItemFont, true));
            item.Go = No;
			item.Name = "No";
            AddItem(item);
			StartMenu.SetItemProperties_Red(item);
            item.SelectSound = null;

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

			SetPos();
        }

		protected override void MyPhsxStep()
		{
			base.MyPhsxStep();

			if (MyMenu.CurIndex == 1)
				MyPile.FindQuad("Berry").TextureName = "cb_crying";
			else
				MyPile.FindQuad("Berry").TextureName = "cb_enthusiastic";

			if (Black != null)
			{
				//Black.FullScreen(Tools.CurCamera);

				if (Black.Alpha < .44f)
				{
					Black.Alpha += .01f;
				}
				else
				{
					Black.Alpha = .44f;
				}
			}
		}

		void SetPos()
		{
			if (Word == Localization.Words.UpSell_FreePlay)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(1091.667f, 230.4445f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(1091.667f, -47.33334f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-1319.446f, -333.3333f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(1169.444f, 1119.111f); _t.Scale = 0.6118331f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Black"); if (_q != null) { _q.Pos = new Vector2(1127.779f, 319.4446f); _q.Size = new Vector2(1886.415f, 1886.415f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1178.473f, 250.0002f); _q.Size = new Vector2(1500f, 2454.545f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(1455.556f, -11.11105f); _q.Size = new Vector2(266.1786f, 358.9999f); }

				MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
			}
			else if (Word == Localization.Words.UpSell_Exit)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(1091.667f, 230.4445f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(1091.667f, -47.33334f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-1319.446f, -333.3333f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(1194.444f, 991.3332f); _t.Scale = 0.6118331f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Black"); if (_q != null) { _q.Pos = new Vector2(1127.779f, 319.4446f); _q.Size = new Vector2(1886.415f, 1886.415f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1178.473f, 250.0002f); _q.Size = new Vector2(1500f, 2454.545f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(1455.556f, -11.11105f); _q.Size = new Vector2(266.1786f, 358.9999f); }

				MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
			}
			else
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(1091.667f, 230.4445f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(1091.667f, -47.33334f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-1319.446f, -333.3333f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(1186.111f, 885.7778f); _t.Scale = 0.6118331f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Black"); if (_q != null) { _q.Pos = new Vector2(1127.779f, 319.4446f); _q.Size = new Vector2(1886.415f, 1886.415f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1178.473f, 250.0002f); _q.Size = new Vector2(1500f, 2454.545f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(1455.556f, -11.11105f); _q.Size = new Vector2(266.1786f, 358.9999f); }

				MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
			}
		}
    }
}