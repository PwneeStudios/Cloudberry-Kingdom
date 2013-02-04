using Microsoft.Xna.Framework;
using System;

namespace CloudberryKingdom
{
    public class AlertBaseMenu : CkBaseMenu
    {
        /// <summary>
        /// Called when the user presses OK.
        /// </summary>
        public Action OnOk;

        protected void Ok()
        {
            ReturnToCaller();

            if (OnOk != null) OnOk();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            OnOk = null;
        }

        public AlertBaseMenu() { }

        Localization.Words Message, OkText;
        public AlertBaseMenu(int Control, Localization.Words Message, Localization.Words OkText)
            : base(false)
        {
            EnableBounce();

            this.Control = Control;
            this.Message = Message;
            this.OkText = OkText;

            Constructor();
        }

        public AlertBaseMenu(bool CallBaseConstructor) : base(CallBaseConstructor) { }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Scale = FontScale * 1.2f;
        }

        protected QuadClass Backdrop;
        public virtual void MakeBackdrop()
        {
			if (UseSimpleBackdrop)
                Backdrop = new QuadClass("Arcade_BoxLeft", 1500, true);
            else
                Backdrop = new QuadClass("Backplate_1230x740", 1500, true);

            Backdrop.Name = "Backdrop";
            MyPile.Add(Backdrop);
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

            var message = new EzText(Message, ItemFont, 700, true, true, .75f);
            message.Name = "Message";
            MyPile.Add(message);

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.Control = Control;

            var OkItem = new MenuItem(new EzText(OkText, ItemFont, true, true), "Message");
            OkItem.Go = Cast.ToItem(Ok);
            AddItem(OkItem);
            OkItem.SelectSound = null;

            MyMenu.OnA = MyMenu.OnX = MyMenu.OnB = Cast.ToMenu(Ok);

            EnsureFancy();



            MenuItem _item;
            _item = MyMenu.FindItemByName("Message"); if (_item != null) { _item.SetPos = new Vector2(-2.44482f, -334.4445f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(0f, 0f);

            EzText _t;
            _t = MyPile.FindEzText("Message"); if (_t != null) { _t.Pos = new Vector2(-13.88892f, 375f); _t.Scale = 1f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1500f, 902.439f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }
    }
}