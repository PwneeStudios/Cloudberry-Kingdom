using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class VerifyQuitGameMenu2 : VerifyBaseMenu
    {
        public VerifyQuitGameMenu2(int Control) : base(Control, true)
        {
            EnableBounce();
        }

        //public static int Version = 0;
        public static int Version = 1;

        QuadClass Berry;
        public override void MakeBackdrop()
        {
            QuadClass backdrop = new QuadClass(null, true, false);
            //backdrop.TextureName = "Backplate_1230x740";
            backdrop.TextureName = "Arcade_BoxLeft";
            backdrop.ScaleYToMatchRatio(1000);
            MyPile.Add(backdrop, "Backdrop");

            Berry = new QuadClass(null, true, false);
            Berry.TextureName = "cb_crying";
            MyPile.Add(Berry, "Berry");
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            //item.MyText.Scale *= 1.15f;
            //item.MySelectedText.Scale *= 1.3f;
            //CkColorHelper.GreenItem(item);

            StartMenu.SetItemProperties_Red(item);
            item.MyText.Shadow = item.MySelectedText.Shadow = false;
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;
            text.Scale *= 1.15f;

            text.MyFloatColor = ColorHelper.Gray(.9f);
        }

        public override void Init()
        {
            base.Init();
            
            SlideInFrom = SlideOutTo = PresetPos.Bottom;
            DestinationScale = new Vector2(1.223f);

            // Make the menu
            MenuItem item;

            // Header
            EzText HeaderText = new EzText(Localization.Words.ExitGame, ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            if (Version == 0)
                HeaderText.Pos = new Vector2(-915.4741f, 967.5232f);
            if (Version == 1)
                HeaderText.Pos = new Vector2(-701.1883f, 816.7295f);

            // Yes
            item = new MenuItem(new EzText(Localization.Words.Yes, ItemFont));
            item.Name = "Yes";
            item.Go = Cast.ToItem(Tools.TheGame.Exit);
            item.AdditionalOnSelect = () => Berry.TextureName = "cb_crying";
            AddItem(item);

            // No
            item = new MenuItem(new EzText(Localization.Words.No, ItemFont));
            item.Name = "No";
            item.Go = ItemReturnToCaller;
            item.AdditionalOnSelect = () => Berry.TextureName = "cb_enthusiastic";
            item.SelectSound = null;
            BackSound = null;
            AddItem(item);

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            EnsureFancy();
            SetPosition();
        }

        void SetPosition()
        {
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.9183335f; _item.MySelectedText.Scale = 0.9183335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(680.5556f, 138.7778f); _item.MyText.Scale = 0.8952501f; _item.MySelectedText.Scale = 0.8952501f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-463.4936f, -274.2063f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-101.1874f, 450.0627f); _t.Scale = 0.8553333f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7120835f; _item.MySelectedText.Scale = 0.7120835f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.6890001f; _item.MySelectedText.Scale = 0.6890001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-396.8268f, -265.873f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-140.0763f, 383.3961f); _t.Scale = 0.6214168f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7628335f; _item.MySelectedText.Scale = 0.7628335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.73975f; _item.MySelectedText.Scale = 0.73975f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-452.3822f, -257.5397f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-137.2986f, 441.7294f); _t.Scale = 0.6013337f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7628335f; _item.MySelectedText.Scale = 0.7628335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.73975f; _item.MySelectedText.Scale = 0.73975f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-419.049f, -268.6508f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-151.1877f, 383.3961f); _t.Scale = 0.6100834f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.8590001f; _item.MySelectedText.Scale = 0.8590001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.8359166f; _item.MySelectedText.Scale = 0.8359166f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-538.4935f, -157.5397f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-670.6317f, 663.9516f); _t.Scale = 0.7720002f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7628335f; _item.MySelectedText.Scale = 0.7628335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.73975f; _item.MySelectedText.Scale = 0.73975f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-396.8268f, -268.6508f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-715.0768f, 666.7294f); _t.Scale = 0.7095835f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7628335f; _item.MySelectedText.Scale = 0.7628335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.73975f; _item.MySelectedText.Scale = 0.73975f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-369.049f, -290.873f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-123.41f, 380.6181f); _t.Scale = 0.6072503f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.8525003f; _item.MySelectedText.Scale = 0.8525003f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.8294169f; _item.MySelectedText.Scale = 0.8294169f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-538.4935f, -171.4286f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-776.1881f, 711.1738f); _t.Scale = 0.9946669f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7628335f; _item.MySelectedText.Scale = 0.7628335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.73975f; _item.MySelectedText.Scale = 0.73975f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-538.4934f, -276.9841f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-756.743f, 680.6182f); _t.Scale = 0.8351667f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
			else
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7628335f; _item.MySelectedText.Scale = 0.7628335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.73975f; _item.MySelectedText.Scale = 0.73975f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-396.8268f, -265.873f);

				EzText _t;
				_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-26.18762f, 394.5072f); _t.Scale = 0.7095835f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
				_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

				MyPile.Pos = new Vector2(13.8877f, -1.984146f);
			}
        }
    }
}