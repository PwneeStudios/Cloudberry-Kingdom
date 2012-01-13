using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class VerifyBaseMenu : StartMenuBase
    {
        public VerifyBaseMenu() { }

        public VerifyBaseMenu(int Control) : base(false)
        {
            this.Control = Control;

            Constructor();
        }

        public VerifyBaseMenu(bool CallBaseConstructor) : base(CallBaseConstructor) { }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Scale = FontScale * 1.2f;
        }

        protected QuadClass Backdrop;
        public virtual void MakeBackdrop()
        {
            Backdrop = new QuadClass("WoodMenu_5", 1500, true);
            MyPile.Add(Backdrop);
            Backdrop.Pos =
                //new Vector2(1525.001f, 200.0001f);
                new Vector2(1181.251f, 241.6668f);
        }

        protected Vector2 HeaderPos = new Vector2(413.8888f, 713.5555f);
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