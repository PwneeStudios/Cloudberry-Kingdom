using Microsoft.Xna.Framework;
using System;

namespace CloudberryKingdom
{
    public class VerifyMenu : VerifyBaseMenu
    {
        string Message;
        Localization.Words Yes, No;
        Action<VerifyMenu> OnYes, OnNo;

        public VerifyMenu(bool CallBaseConstructor)
            : base(CallBaseConstructor)
        {
            EnableBounce();
        }
        public VerifyMenu(int Control,
            string Message, Localization.Words Yes, Localization.Words No,
            Action<VerifyMenu> OnYes, Action<VerifyMenu> OnNo)
            : base(false)
        {
            this.Message = Message;
            this.Yes = Yes;
            this.No = No;

            this.OnYes = OnYes;            
            this.OnNo  = OnNo;

            if (OnNo == null)
            {
                this.OnNo = This => This.ReturnToCaller(false);
            }

            EnableBounce();
            this.Control = Control;
            Constructor();
        }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
            Text HeaderText = new Text(Message, ItemFont, 1600, true, false, .7f);
            SetHeaderProperties(HeaderText);
            HeaderText.Name = "Header";
            MyPile.Add(HeaderText);

            // Ok
            item = new MenuItem(new Text(Yes, ItemFont, true));
            item.Go = _item => OnYes(this);
            item.Name = "Yes";
            AddItem(item);

            // No
            item = new MenuItem(new Text(No, ItemFont, true));
            item.Go = _item => OnNo(this);
            item.Name = "No";
            AddItem(item);

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            SetPos();
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(0f, 349.8889f); _item.MyText.Scale = 0.5410002f; _item.MySelectedText.Scale = 0.5410002f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(0f, 127.6667f); _item.MyText.Scale = 0.5438334f; _item.MySelectedText.Scale = 0.5438334f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-2.777588f, -330.5555f);

            Text _t;
            _t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(0f, 777.4443f); _t.Scale = 0.5221667f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(0f, 316.6668f); _q.Size = new Vector2(1281.083f, 729.4391f); }

            MyPile.Pos = new Vector2(0f, -319.4444f);
        }
    }

    public class VerifyBaseMenu : CkBaseMenu
    {
        /// <summary>
        /// Called when the user chooses yes/no.
        /// Bool is set to the user's choice.
        /// </summary>
        public Action<bool> OnSelect;

        protected void DoSelect(bool choice)
        {
            if (OnSelect != null) OnSelect(choice);
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            OnSelect = null;
        }

        public VerifyBaseMenu()
        {
        }

        public VerifyBaseMenu(int Control, bool DoEnableBounce) : base(false)
        {
            if (DoEnableBounce) EnableBounce();

            this.Control = Control;

            Constructor();
        }

        public VerifyBaseMenu(bool CallBaseConstructor) : base(CallBaseConstructor) { }

        protected override void SetHeaderProperties(Text text)
        {
            base.SetHeaderProperties(text);

            text.Scale = FontScale * 1.2f;
        }

        protected QuadClass Backdrop;
        public virtual void MakeBackdrop()
        {
            QuadClass Backdrop;
			if (UseSimpleBackdrop)
                Backdrop = new QuadClass("Arcade_BoxLeft", 1500, true);
            else
                Backdrop = new QuadClass("Backplate_1230x740", 1500, true);
            
            Backdrop.Name = "Backdrop";
            MyPile.Add(Backdrop);
            Backdrop.Pos = new Vector2(1181.251f, 241.6668f);

			if (!UseSimpleBackdrop)
			{
				EpilepsySafe(.9f);
			}
        }

        protected Vector2 HeaderPos = new Vector2(413.8888f, 713.5555f);
        public override void Init()
        {
            base.Init();

            PauseGame = true;

            if (!UseBounce)
            {
                ReturnToCallerDelay = 10;
                SlideInLength = 26;
                SlideOutLength = 26;
            }

            this.SlideInFrom = PresetPos.Right;
            this.SlideOutTo = PresetPos.Right;

            FontScale = .8f;

            MyPile = new DrawPile();

            // Make the backdrop
            MakeBackdrop();

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.Control = Control;

            ItemPos = new Vector2(800f, 361f);
            PosAdd = new Vector2(0, -300);
            
            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Shift everything
            EnsureFancy();
            Shift(new Vector2(-1125.001f, -319.4444f));
        }
    }
}