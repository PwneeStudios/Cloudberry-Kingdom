using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class VerifyQuitLevelMenu : VerifyBaseMenu
    {
        public VerifyQuitLevelMenu(bool CallBaseConstructor) : base(CallBaseConstructor) { }
        public VerifyQuitLevelMenu(int Control) : base(Control) { }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
            EzText HeaderText = new EzText(Localization.Words.ExitLevelQuestion, ItemFont);
            SetHeaderProperties(HeaderText);
            HeaderText.Name = "Header";
            MyPile.Add(HeaderText);

            // Ok
            item = new MenuItem(new EzText(Localization.Words.Yes, ItemFont));
            item.Go = new VerifyQuitOkLambda();
            item.Name = "Yes";
            AddItem(item);

            // No
            item = new MenuItem(new EzText(Localization.Words.No, ItemFont));
            item.Go = Cast.ToItem(new ReturnToCallerProxy(this));
            item.Name = "No";
            AddItem(item);

            MyMenu.OnX = MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            SetPos();
        }

        class VerifyQuitOkLambda : Lambda_1<MenuItem>
        {
            public VerifyQuitOkLambda()
            {
            }

            public void Apply(MenuItem item)
            {
                Tools.CurrentAftermath = new AftermathData();
                Tools.CurrentAftermath.Success = false;
                Tools.CurrentAftermath.EarlyExit = true;

                Tools.CurGameData.EndGame.Apply(false);
            }
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(800f, 361f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(800f, 61f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-1000.001f, -302.7777f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(597.2208f, 746.8888f); _t.Scale = 0.96f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1161.806f, 316.6668f); _q.Size = new Vector2(1500f, 902.439f); }

            MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
        }
    }
}